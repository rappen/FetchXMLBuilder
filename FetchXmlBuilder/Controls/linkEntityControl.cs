﻿using Cinteros.Xrm.FetchXmlBuilder.AppCode;
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
            ValidationSuspended = true;
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
                    var greatparententityname = Node.Parent.Parent != null ? TreeNodeHelper.GetAttributeFromNode(Node.Parent.Parent, "name") : "";
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
            if (cmbEntity.SelectedItem != null)
            {
                var linkentity = cmbEntity.SelectedItem.ToString();
                cmbFrom.Items.AddRange(GetRelevantLinkAttributes(linkentity));
            }
            var parententity = TreeNodeHelper.GetAttributeFromNode(Node.Parent, "name");
            cmbTo.Items.AddRange(GetRelevantLinkAttributes(parententity));
            ValidationSuspended = false;
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
                var parent = TreeNodeHelper.GetAttributeFromNode(Node.Parent, "name");
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
                        var greatparent = TreeNodeHelper.GetAttributeFromNode(Node.Parent.Parent, "name");
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

                BeginInit();
                cmbEntity.Text = entity;
                cmbFrom.Text = from;
                cmbTo.Text = to;
                chkIntersect.Checked = intersect;
                EndInit();
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
            if (control == cmbRelationship && string.IsNullOrWhiteSpace(cmbEntity.Text) && string.IsNullOrWhiteSpace(cmbFrom.Text) && string.IsNullOrWhiteSpace(cmbTo.Text))
            {
                return new ControlValidationResult(ControlValidationLevel.Info, "Select a relationship to populate fields below");
            }
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
                    if (string.IsNullOrWhiteSpace(cmbEntity.Text))
                    {
                        return new ControlValidationResult(ControlValidationLevel.Info, "Enter entity name and then select From attribute");
                    }
                    else
                    {
                        return new ControlValidationResult(ControlValidationLevel.Error, "From attribute is required");
                    }
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
                    if (string.IsNullOrWhiteSpace(cmbEntity.Text))
                    {
                        return new ControlValidationResult(ControlValidationLevel.Info, "Enter entity name and then select To attribute");
                    }
                    else
                    {
                        return new ControlValidationResult(ControlValidationLevel.Error, "To attribute is required");
                    }
                }

                if (fxb.Service != null && !cmbTo.Items.OfType<string>().Any(i => i == cmbTo.Text))
                {
                    return new ControlValidationResult(ControlValidationLevel.Warning, "To attribute is not valid");
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
    }
}