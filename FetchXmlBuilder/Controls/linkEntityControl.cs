using Cinteros.Xrm.FetchXmlBuilder.Builder;
using Cinteros.Xrm.FetchXmlBuilder.ControlsClasses;
using Cinteros.Xrm.FetchXmlBuilder.DockControls;
using Microsoft.Xrm.Sdk;
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
            BeginInit();
            rbAttrIdOnly.Checked = fetchXmlBuilder.settings.LinkEntityIdAttributesOnly;
            rbAttrAll.Checked = !rbAttrIdOnly.Checked;
            InitializeFXB(null, fetchXmlBuilder, tree, node);
            EndInit();
            RefreshAttributes();
        }

        protected override void PopulateControls()
        {
            cmbEntity.Items.Clear();
            var entities = fxb.GetDisplayEntities();
            if (entities != null)
            {
                foreach (var entity in entities)
                {
                    cmbEntity.Items.Add(entity.LogicalName);
                }
            }

            var parententityname = Node.Parent.Value("name");
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
            ValidationSuspended = true;
            var entity = cmbEntity.Text ?? cmbEntity.SelectedItem.ToString();
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
            var parententityname = Node.Parent.Value("name");
            var parententity = fxb.GetEntity(parententityname);
            if (parententity != null)
            {
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
                        list.Add(new EntityRelationship(rel, EntityRole.Referencing, parententityname, fxb));
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
                        list.Add(new EntityRelationship(rel, EntityRole.Referenced, parententityname, fxb));
                    }
                    list.Sort();
                    cmbRelationship.Items.AddRange(list.ToArray());
                }
                if (mm.Length > 0)
                {
                    var greatparententityname = Node.Parent.Parent != null ? Node.Parent.Parent.Value("name") : "";
                    cmbRelationship.Items.Add("- M:M -");
                    list.Clear();
                    foreach (var rel in mm)
                    {
                        if (rel.Entity1LogicalName == parententityname ||
                            rel.Entity1LogicalName == greatparententityname && rel.IntersectEntityName == parententityname)
                        {
                            list.Add(new EntityRelationship(rel, EntityRole.Referencing, parententityname, fxb, greatparententityname));
                        }

                        if (rel.Entity2LogicalName == parententityname ||
                            rel.Entity2LogicalName == greatparententityname && rel.IntersectEntityName == parententityname)
                        {
                            list.Add(new EntityRelationship(rel, EntityRole.Referenced, parententityname, fxb, greatparententityname));
                        }
                    }
                    list.Sort();
                    cmbRelationship.Items.AddRange(list.ToArray());
                }
            }
        }

        private void RefreshAttributes()
        {
            if (!IsInitialized)
            {
                return;
            }
            cmbFrom.Items.Clear();
            cmbTo.Items.Clear();
            var linkentity = cmbEntity.Text ?? cmbEntity.SelectedItem.ToString();
            if (!string.IsNullOrWhiteSpace(linkentity))
            {
                cmbFrom.Items.AddRange(GetRelevantLinkAttributes(linkentity));
            }
            var parententity = Node.Parent.Value("name");
            cmbTo.Items.AddRange(GetRelevantLinkAttributes(parententity));
            ValidationSuspended = false;
            fxb.ShowMetadata(Metadata());
        }

        private string[] GetRelevantLinkAttributes(string entity)
        {
            var linkAttributes = fxb.GetDisplayAttributes(entity);
            if (rbAttrIdOnly.Checked)
            {
                linkAttributes = linkAttributes
                    .Where(a => a.IsPrimaryId == true ||
                        a.AttributeType == AttributeTypeCode.Uniqueidentifier ||
                        a.AttributeType == AttributeTypeCode.Lookup ||
                        a.AttributeType == AttributeTypeCode.Customer ||
                        a.AttributeType == AttributeTypeCode.Owner)
                    .ToArray();
            }
            return linkAttributes.Select(a => a.LogicalName).ToArray();
        }

        private void cmbRelationship_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbRelationship.SelectedItem != null && cmbRelationship.SelectedItem is EntityRelationship rel)
            {
                var parent = Node.Parent.Value("name");
                string entity;
                string from;
                string to;
                bool intersect;

                if (rel.Relationship is OneToManyRelationshipMetadata om)
                {
                    if (parent == om.ReferencedEntity && rel.Role == EntityRole.Referenced)
                    {
                        entity = om.ReferencingEntity;
                        from = om.ReferencingAttribute;
                        to = om.ReferencedAttribute;
                    }
                    else if (parent == om.ReferencingEntity && rel.Role == EntityRole.Referencing)
                    {
                        entity = om.ReferencedEntity;
                        from = om.ReferencedAttribute;
                        to = om.ReferencingAttribute;
                    }
                    else
                    {
                        MessageBox.Show("Not a valid relationship. Please enter entity and attributes manually.");
                        return;
                    }
                    intersect = false;
                }
                else if (rel.Relationship is ManyToManyRelationshipMetadata mm)
                {
                    if (fxb.NeedToLoadEntity(mm.Entity1LogicalName))
                    {
                        fxb.LoadEntityDetails(mm.Entity1LogicalName, null, false);
                    }

                    if (fxb.NeedToLoadEntity(mm.Entity2LogicalName))
                    {
                        fxb.LoadEntityDetails(mm.Entity2LogicalName, null, false);
                    }

                    var entity1PrimaryKey = fxb.GetPrimaryIdAttribute(mm.Entity1LogicalName);
                    var entity2PrimaryKey = fxb.GetPrimaryIdAttribute(mm.Entity2LogicalName);

                    if (parent == mm.IntersectEntityName)
                    {
                        var greatparent = Node.Parent.Parent.Value("name");
                        if (greatparent == mm.Entity1LogicalName && rel.Role == EntityRole.Referencing)
                        {
                            entity = mm.Entity2LogicalName;
                            from = entity2PrimaryKey;
                            to = mm.Entity2IntersectAttribute;
                        }
                        else if (greatparent == mm.Entity2LogicalName && rel.Role == EntityRole.Referenced)
                        {
                            entity = mm.Entity1LogicalName;
                            from = entity1PrimaryKey;
                            to = mm.Entity1IntersectAttribute;
                        }
                        else
                        {
                            MessageBox.Show("Not a valid M:M-relationship. Please enter entity and attributes manually.");
                            return;
                        }
                        intersect = true;
                    }
                    else
                    {
                        entity = mm.IntersectEntityName;
                        if (parent == mm.Entity1LogicalName && rel.Role == EntityRole.Referencing)
                        {
                            from = mm.Entity1IntersectAttribute;
                            to = entity1PrimaryKey;
                        }
                        else if (parent == mm.Entity2LogicalName && rel.Role == EntityRole.Referenced)
                        {
                            from = mm.Entity2IntersectAttribute;
                            to = entity2PrimaryKey;
                        }
                        else
                        {
                            MessageBox.Show("Not a valid M:M-relationship. Please enter entity and attributes manually.");
                            return;
                        }
                        intersect = true;
                    }
                }
                else
                {
                    MessageBox.Show("Not a valid relationship. Please enter entity and attributes manually.");
                    return;
                }

                ValidationSuspended = true;
                cmbEntity.Text = entity;
                cmbFrom.Text = from;
                cmbTo.Text = to;
                chkIntersect.Checked = intersect;
                ValidationSuspended = false;
                Save(false);
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
            var parententityname = Node.Parent.LocalEntityName();
            var parententity = fxb.GetEntity(parententityname);
            var currententity = fxb.GetEntity(cmbEntity.Text);
            if (control == cmbRelationship && string.IsNullOrWhiteSpace(cmbEntity.Text) && string.IsNullOrWhiteSpace(cmbFrom.Text) && string.IsNullOrWhiteSpace(cmbTo.Text))
            {
                return new ControlValidationResult(ControlValidationLevel.Info, "Select a relationship to populate fields below");
            }
            if (control == cmbEntity)
            {
                if (string.IsNullOrWhiteSpace(cmbEntity.Text))
                {
                    return new ControlValidationResult(ControlValidationLevel.Error, "Entity", ControlValidationMessage.IsRequired);
                }

                if (fxb.entities != null)
                {
                    if (currententity == null)
                    {
                        return new ControlValidationResult(ControlValidationLevel.Warning, "Entity", ControlValidationMessage.NotInMetadata);
                    }
                    if (!cmbEntity.Items.OfType<string>().Any(i => i == cmbEntity.Text))
                    {
                        return new ControlValidationResult(ControlValidationLevel.Info, "Entity", ControlValidationMessage.NotShowingNow);
                    }
                }
            }

            if (control == cmbFrom)
            {
                if (string.IsNullOrWhiteSpace(cmbFrom.Text))
                {
                    if (string.IsNullOrWhiteSpace(cmbEntity.Text))
                    {
                        return new ControlValidationResult(ControlValidationLevel.Info, "Enter entity name and then select From attribute");
                    }
                    else
                    {
                        return new ControlValidationResult(ControlValidationLevel.Error, "From attribute", ControlValidationMessage.IsRequired);
                    }
                }

                if (currententity != null && !currententity.Attributes.Any(a => a.LogicalName == cmbFrom.Text))
                {
                    return new ControlValidationResult(ControlValidationLevel.Warning, "From attribute", ControlValidationMessage.NotInMetadata);
                }
                if (fxb.entities != null && !cmbFrom.Items.OfType<string>().Any(i => i == cmbFrom.Text))
                {
                    return new ControlValidationResult(ControlValidationLevel.Info, "From attribute", ControlValidationMessage.NotShowingNow);
                }
            }

            if (control == cmbTo)
            {
                if (string.IsNullOrWhiteSpace(cmbTo.Text))
                {
                    if (string.IsNullOrWhiteSpace(cmbEntity.Text))
                    {
                        return new ControlValidationResult(ControlValidationLevel.Info, "Enter entity name and then select To attribute");
                    }
                    else
                    {
                        return new ControlValidationResult(ControlValidationLevel.Error, "To attribute", ControlValidationMessage.IsRequired);
                    }
                }

                if (parententity != null && !parententity.Attributes.Any(a => a.LogicalName == cmbTo.Text))
                {
                    return new ControlValidationResult(ControlValidationLevel.Warning, "To attribute", ControlValidationMessage.NotInMetadata);
                }
                if (fxb.entities != null && !cmbTo.Items.OfType<string>().Any(i => i == cmbTo.Text))
                {
                    return new ControlValidationResult(ControlValidationLevel.Info, "To attribute", ControlValidationMessage.NotShowingNow);
                }
            }

            return base.ValidateControl(control);
        }

        private void rbAttr_CheckedChanged(object sender, EventArgs e)
        {
            if (!IsInitialized)
            {
                return;
            }
            if (fxb != null && fxb.settings != null)
            {
                fxb.settings.LinkEntityIdAttributesOnly = rbAttrIdOnly.Checked;
            }
            RefreshAttributes();
            Save(false);
        }

        public override MetadataBase Metadata()
        {
            if (cmbFrom.Focused)
            {
                if (!string.IsNullOrWhiteSpace(cmbFrom.Text) &&
                    GetEntityMetadata() is EntityMetadata emeta &&
                    emeta.Attributes.FirstOrDefault(a => a.LogicalName == cmbFrom.Text) is AttributeMetadata ameta)
                {
                    return ameta;
                }
            }
            else if (cmbTo.Focused)
            {
                var parententity = Node.Parent.Value("name");
                if (!string.IsNullOrWhiteSpace(cmbTo.Text) &&
                    fxb.GetEntity(parententity) is EntityMetadata pmeta &&
                    pmeta.Attributes.FirstOrDefault(a => a.LogicalName == cmbTo.Text) is AttributeMetadata pameta)
                {
                    return pameta;
                }
            }
            else
            {
                return GetEntityMetadata();
            }
            return base.Metadata();
        }

        private MetadataBase GetEntityMetadata()
        {
            if (cmbEntity.SelectedItem is EntityItem item)
            {
                return item.Meta;
            }
            return fxb.GetEntity(cmbEntity.Text) ?? base.Metadata();
        }

        private void cmbEntity_Enter(object sender, EventArgs e)
        {
            fxb.ShowMetadata(Metadata());
        }

        private void helpIcon_Click(object sender, System.EventArgs e)
        {
            FetchXmlBuilder.HelpClick(sender);
        }

        public override void Focus()
        {
            cmbRelationship.Focus();
        }
    }
}