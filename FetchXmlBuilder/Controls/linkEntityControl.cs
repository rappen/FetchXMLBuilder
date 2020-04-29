using Cinteros.Xrm.FetchXmlBuilder.AppCode;
using Cinteros.Xrm.FetchXmlBuilder.DockControls;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Cinteros.Xrm.FetchXmlBuilder.Controls
{
    public partial class linkEntityControl : FetchXmlElementControlBase
    {
        private int relationshipWidth;

        public linkEntityControl() : this(null, null, null)
        {
        }

        public linkEntityControl(TreeNode node, FetchXmlBuilder fetchXmlBuilder, TreeBuilderControl tree)
        {
            InitializeComponent();
            InitializeFXB(null, fetchXmlBuilder, tree, node);
        }

        protected override void PopulateControls()
        {
            cmbEntity.Items.Clear();
            var entities = fxb.GetDisplayEntities();
            if (entities != null)
            {
                foreach (var entity in entities)
                {
                    cmbEntity.Items.Add(entity.Value.LogicalName);
                }
            }

            var parententityname = TreeNodeHelper.GetAttributeFromNode(Node.Parent, "name");
            if (fxb.NeedToLoadEntity(parententityname))
            {
                if (!fxb.working)
                {
                    fxb.LoadEntityDetails(parententityname, RefreshRelationships);
                }
            }
            else
            {
                RefreshRelationships();
            }
            RefreshAttributes();
        }

        private void cmbEntity_SelectedIndexChanged(object sender, EventArgs e)
        {
            var entity = cmbEntity.SelectedItem.ToString();
            if (string.IsNullOrEmpty(entity))
            {
                return;
            }
            if (fxb.NeedToLoadEntity(entity))
            {
                if (!fxb.working)
                {
                    fxb.LoadEntityDetails(entity, RefreshAttributes);
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
            var parententityname = TreeNodeHelper.GetAttributeFromNode(Node.Parent, "name");
            var entities = fxb.GetDisplayEntities();
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
                        list.Add(new EntityRelationship(rel, parententityname, fxb));
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
                        list.Add(new EntityRelationship(rel, parententityname, fxb));
                    }
                    list.Sort();
                    cmbRelationship.Items.AddRange(list.ToArray());
                }
                if (mm.Length > 0)
                {
                    var greatparententityname = Node.Parent.Parent != null ? TreeNodeHelper.GetAttributeFromNode(Node.Parent.Parent, "name") : "";
                    cmbRelationship.Items.Add("- M:M -");
                    list.Clear();
                    foreach (var rel in mm)
                    {
                        list.Add(new EntityRelationship(rel, parententityname, fxb, greatparententityname));
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
            if (cmbEntity.SelectedItem != null)
            {
                var linkentity = cmbEntity.SelectedItem.ToString();
                var linkAttributes = fxb.GetDisplayAttributes(linkentity);
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
            var parententity = TreeNodeHelper.GetAttributeFromNode(Node.Parent, "name");
            var parentAttributes = fxb.GetDisplayAttributes(parententity);
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
            if (cmbRelationship.SelectedItem != null && cmbRelationship.SelectedItem is EntityRelationship rel)
            {
                var parent = TreeNodeHelper.GetAttributeFromNode(Node.Parent, "name");
                if (rel.Relationship is OneToManyRelationshipMetadata om)
                {
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
                else if (rel.Relationship is ManyToManyRelationshipMetadata mm)
                {
                    if (parent == mm.IntersectEntityName)
                    {
                        var greatparent = TreeNodeHelper.GetAttributeFromNode(Node.Parent.Parent, "name");
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

        private void cmbRelationship_DropDown(object sender, EventArgs e)
        {
            relationshipWidth = cmbRelationship.Width;
            if (cmbRelationship.Width < 300)
            {
                cmbRelationship.Width = 350;
            }
        }

        private void cmbRelationship_DropDownClosed(object sender, EventArgs e)
        {
            if (relationshipWidth < 300)
            {
                cmbRelationship.Width = relationshipWidth;
            }
        }

        protected override ControlValidationResult ValidateControl(Control control)
        {
            if (control == cmbEntity)
            {
                if (string.IsNullOrWhiteSpace(cmbEntity.Text))
                {
                    return new ControlValidationResult(ControlValidationLevel.Error, "Entity is required");
                }

                if (fxb.Service != null && !cmbEntity.Items.OfType<string>().Any(i => i == cmbEntity.Text))
                {
                    return new ControlValidationResult(ControlValidationLevel.Warning, "Entity is not valid");
                }
            }

            if (control == cmbFrom)
            {
                if (string.IsNullOrWhiteSpace(cmbFrom.Text))
                {
                    return new ControlValidationResult(ControlValidationLevel.Error, "From attribute is required");
                }

                if (fxb.Service != null && !cmbFrom.Items.OfType<string>().Any(i => i == cmbFrom.Text))
                {
                    return new ControlValidationResult(ControlValidationLevel.Warning, "From attribute is not valid");
                }
            }

            if (control == cmbTo)
            {
                if (string.IsNullOrWhiteSpace(cmbTo.Text))
                {
                    return new ControlValidationResult(ControlValidationLevel.Error, "To attribute is required");
                }

                if (fxb.Service != null && !cmbTo.Items.OfType<string>().Any(i => i == cmbTo.Text))
                {
                    return new ControlValidationResult(ControlValidationLevel.Warning, "To attribute is not valid");
                }
            }

            return base.ValidateControl(control);
        }
    }
}