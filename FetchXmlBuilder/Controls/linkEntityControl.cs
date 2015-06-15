using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Cinteros.Xrm.FetchXmlBuilder.AppCode;
using Cinteros.Xrm.XmlEditorUtils;
using Microsoft.Xrm.Sdk.Metadata;

namespace Cinteros.Xrm.FetchXmlBuilder.Controls
{
    public partial class linkEntityControl : UserControl, IDefinitionSavable
    {
        private readonly Dictionary<string, string> collec;
        private string controlsCheckSum = "";
        TreeNode node;
        FetchXmlBuilder form;

        #region Delegates

        public delegate void SaveEventHandler(object sender, SaveEventArgs e);

        #endregion

        #region Event Handlers

        public event SaveEventHandler Saved;

        #endregion

        public linkEntityControl()
        {
            InitializeComponent();
            collec = new Dictionary<string, string>();
        }

        public linkEntityControl(TreeNode Node, FetchXmlBuilder fetchXmlBuilder)
            : this()
        {
            form = fetchXmlBuilder;
            node = Node;
            collec = (Dictionary<string, string>)node.Tag;
            if (collec == null)
            {
                collec = new Dictionary<string, string>();
            }
            PopulateControls();
            ControlUtils.FillControls(collec, this.Controls);
            controlsCheckSum = ControlUtils.ControlsChecksum(this.Controls);
            Saved += fetchXmlBuilder.CtrlSaved;
        }

        private void PopulateControls()
        {
            cmbEntity.Items.Clear();
            var entities = form.GetDisplayEntities();
            if (entities != null)
            {
                foreach (var entity in entities)
                {
                    cmbEntity.Items.Add(entity.Value.LogicalName);
                }
            }

            var parententityname = TreeNodeHelper.GetAttributeFromNode(node.Parent, "name");
            if (form.NeedToLoadEntity(parententityname))
            {
                if (!form.working)
                {
                    form.LoadEntityDetails(parententityname, RefreshRelationships);
                }
            }
            else
            {
                RefreshRelationships();
            }
            RefreshAttributes();
        }

        public void Save()
        {
            try
            {
                Dictionary<string, string> collection = ControlUtils.GetAttributesCollection(this.Controls, true);
                SendSaveMessage(collection);
            }
            catch (ArgumentNullException ex)
            {
                MessageBox.Show(ex.Message, "Validation", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            controlsCheckSum = ControlUtils.ControlsChecksum(this.Controls);
        }

        /// <summary></summary>
        /// <param name="collection"></param>
        private void SendSaveMessage(Dictionary<string, string> collection)
        {
            SaveEventArgs sea = new SaveEventArgs { AttributeCollection = collection };

            if (Saved != null)
            {
                Saved(this, sea);
            }
        }

        private void Control_Leave(object sender, EventArgs e)
        {
            if (controlsCheckSum != ControlUtils.ControlsChecksum(this.Controls))
            {
                Save();
            }
        }

        private void cmbEntity_SelectedIndexChanged(object sender, EventArgs e)
        {
            var entity = cmbEntity.SelectedItem.ToString();
            if (string.IsNullOrEmpty(entity))
            {
                return;
            }
            if (form.NeedToLoadEntity(entity))
            {
                if (!form.working)
                {
                    form.LoadEntityDetails(entity, RefreshAttributes);
                }
            }
            else
            {
                RefreshAttributes();
            }
        }

        private void RefreshRelationships()
        {
            cmbRelationship.Items.Clear();
            var parententityname = TreeNodeHelper.GetAttributeFromNode(node.Parent, "name");
            var entities = form.GetDisplayEntities();
            if (entities != null && entities.ContainsKey(parententityname))
            {
                var parententity = entities[parententityname];
                var mo = parententity.ManyToOneRelationships;
                var om = parententity.OneToManyRelationships;
                var mm = parententity.ManyToManyRelationships;
                var list = new List<EntityRelationship>();
                if (mo.Length > 0)
                {
                    cmbRelationship.Items.Add("- M:1 -");
                    list.Clear();
                    foreach (var rel in mo)
                    {
                        list.Add(new EntityRelationship(rel, parententityname));
                    }
                    list.Sort();
                    cmbRelationship.Items.AddRange(list.ToArray());
                }
                if (om.Length > 0)
                {
                    cmbRelationship.Items.Add("- 1:M -");
                    list.Clear();
                    foreach (var rel in om)
                    {
                        list.Add(new EntityRelationship(rel, parententityname));
                    }
                    list.Sort();
                    cmbRelationship.Items.AddRange(list.ToArray());
                }
                if (mm.Length > 0)
                {
                    var greatparententityname = node.Parent.Parent != null ? TreeNodeHelper.GetAttributeFromNode(node.Parent.Parent, "name") : "";
                    cmbRelationship.Items.Add("- M:M -");
                    list.Clear();
                    foreach (var rel in mm)
                    {
                        list.Add(new EntityRelationship(rel, parententityname, greatparententityname));
                    }
                    list.Sort();
                    cmbRelationship.Items.AddRange(list.ToArray());
                }
            }
        }

        private void RefreshAttributes()
        {
            cmbFrom.Items.Clear();
            cmbTo.Items.Clear();
            var entities = form.GetDisplayEntities();
            if (cmbEntity.SelectedItem != null)
            {
                var linkentity = cmbEntity.SelectedItem.ToString();
                var linkAttributes = form.GetDisplayAttributes(linkentity);
                foreach (var attribute in linkAttributes)
                {
                    if (attribute.IsPrimaryId == true ||
                        attribute.AttributeType == AttributeTypeCode.Lookup ||
                        attribute.AttributeType == AttributeTypeCode.Customer ||
                        attribute.AttributeType == AttributeTypeCode.Owner)
                    {
                        cmbFrom.Items.Add(attribute.LogicalName);
                    }
                }
            }
            var parententity = TreeNodeHelper.GetAttributeFromNode(node.Parent, "name");
            var parentAttributes = form.GetDisplayAttributes(parententity);
            foreach (var attribute in parentAttributes)
            {
                if (attribute.IsPrimaryId == true ||
                    attribute.AttributeType == AttributeTypeCode.Lookup ||
                    attribute.AttributeType == AttributeTypeCode.Customer ||
                    attribute.AttributeType == AttributeTypeCode.Owner)
                {
                    cmbTo.Items.Add(attribute.LogicalName);
                }
            }
        }

        private void cmbRelationship_SelectedIndexChanged(object sender, EventArgs e)
        {
            var rel = (EntityRelationship)cmbRelationship.SelectedItem;
            if (rel != null)
            {
                var parent = TreeNodeHelper.GetAttributeFromNode(node.Parent, "name");
                if (rel.Relationship is OneToManyRelationshipMetadata)
                {
                    var om = (OneToManyRelationshipMetadata)rel.Relationship;
                    if (parent == om.ReferencedEntity)
                    {
                        cmbEntity.Text = om.ReferencingEntity;
                        cmbFrom.Text = om.ReferencingAttribute;
                        cmbTo.Text = om.ReferencedAttribute;
                    }
                    else if (parent == om.ReferencingEntity)
                    {
                        cmbEntity.Text = om.ReferencedEntity;
                        cmbFrom.Text = om.ReferencedAttribute;
                        cmbTo.Text = om.ReferencingAttribute;
                    }
                    else
                    {
                        MessageBox.Show("Not a valid relationship. Please enter entity and attributes manually.");
                    }
                    chkIntersect.Checked = false;
                }
                else if (rel.Relationship is ManyToManyRelationshipMetadata)
                {
                    var mm = (ManyToManyRelationshipMetadata)rel.Relationship;
                    if (parent == mm.IntersectEntityName)
                    {
                        var greatparent = TreeNodeHelper.GetAttributeFromNode(node.Parent.Parent, "name");
                        if (greatparent == mm.Entity1LogicalName)
                        {
                            cmbEntity.Text = mm.Entity2LogicalName;
                            cmbFrom.Text = mm.Entity2IntersectAttribute;
                            cmbTo.Text = mm.Entity2IntersectAttribute;
                        }
                        else if (greatparent == mm.Entity2LogicalName)
                        {
                            cmbEntity.Text = mm.Entity1LogicalName;
                            cmbFrom.Text = mm.Entity1IntersectAttribute;
                            cmbTo.Text = mm.Entity1IntersectAttribute;
                        }
                        else
                        {
                            MessageBox.Show("Not a valid M:M-relationship. Please enter entity and attributes manually.");
                        }
                    }
                    else
                    {
                        cmbEntity.Text = mm.IntersectEntityName;
                        if (parent == mm.Entity1LogicalName)
                        {
                            cmbFrom.Text = mm.Entity1IntersectAttribute;
                            cmbTo.Text = mm.Entity1IntersectAttribute;
                        }
                        else if (parent == mm.Entity2LogicalName)
                        {
                            cmbFrom.Text = mm.Entity2IntersectAttribute;
                            cmbTo.Text = mm.Entity2IntersectAttribute;
                        }
                        else
                        {
                            MessageBox.Show("Not a valid M:M-relationship. Please enter entity and attributes manually.");
                        }
                        chkIntersect.Checked = true;
                    }
                }
            }
        }
    }
}
