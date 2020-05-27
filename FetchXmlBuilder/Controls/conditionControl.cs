using Cinteros.Xrm.FetchXmlBuilder.AppCode;
using Cinteros.Xrm.FetchXmlBuilder.DockControls;
using Cinteros.Xrm.XmlEditorUtils;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Windows.Forms;
using xrmtb.XrmToolBox.Controls;

namespace Cinteros.Xrm.FetchXmlBuilder.Controls
{
    public partial class conditionControl : FetchXmlElementControlBase
    {
        #region Public Constructors

        public conditionControl() : this(null, null, null)
        {
        }

        public conditionControl(TreeNode node, FetchXmlBuilder fetchXmlBuilder, TreeBuilderControl tree)
        {
            InitializeComponent();
            txtLookup.OrganizationService = fetchXmlBuilder.Service;
            dlgLookup.Service = fetchXmlBuilder.Service;
            InitializeFXB(null, fetchXmlBuilder, tree, node);
            RefreshAttributes();
        }

        #endregion Public Constructors

        #region Protected Methods

        protected override void PopulateControls()
        {
            cmbEntity.Items.Clear();
            var closestEntity = GetClosestEntityNode(Node);
            if (closestEntity != null && closestEntity.Name == "entity")
            {
                cmbEntity.Items.Add("");
                cmbEntity.Items.AddRange(GetEntities(Tree.tvFetch.Nodes[0]).ToArray());
            }
            cmbEntity.Enabled = cmbEntity.Items.Count > 0;
            cmbOperator.Items.Clear();
            foreach (var oper in Enum.GetValues(typeof(ConditionOperator)))
            {
                cmbOperator.Items.Add(new OperatorItem((ConditionOperator)oper));
            }
        }

        protected override bool RequiresSave()
        {
            return base.RequiresSave() ||
                cmbOperator.SelectedItem is OperatorItem op && op.IsMultipleValuesType && !string.IsNullOrEmpty(cmbValue.Text);
        }

        protected override void SaveInternal(bool silent)
        {
            if (!silent && cmbOperator.SelectedItem != null && cmbOperator.SelectedItem is OperatorItem)
            {
                ExtractCommaSeparatedValues();
            }

            base.SaveInternal(silent);
        }

        protected override ControlValidationResult ValidateControl(Control control)
        {
            if (control == cmbAttribute)
            {
                if (string.IsNullOrWhiteSpace(cmbAttribute.Text))
                {
                    return new ControlValidationResult(ControlValidationLevel.Error, "Attribute is required");
                }

                if (fxb.Service != null && !cmbAttribute.Items.OfType<AttributeItem>().Any(i => i.ToString() == cmbAttribute.Text))
                {
                    return new ControlValidationResult(ControlValidationLevel.Warning, "Attribute is not valid");
                }
            }
            else if (control == cmbOperator || control == cmbValue)
            {
                if (control == cmbOperator && string.IsNullOrWhiteSpace(cmbOperator.Text))
                {
                    return new ControlValidationResult(ControlValidationLevel.Error, "Operator is required");
                }

                if (control == cmbOperator && !cmbOperator.Items.OfType<OperatorItem>().Any(i => i.ToString() == cmbOperator.Text))
                {
                    return new ControlValidationResult(ControlValidationLevel.Error, "Operator is not valid");
                }

                if (cmbOperator.SelectedItem != null && cmbOperator.SelectedItem is OperatorItem oper && (!oper.IsMultipleValuesType || Node.Nodes.Count > 0))
                {
                    AttributeItem attribute = null;
                    if (cmbAttribute.SelectedItem != null && cmbAttribute.SelectedItem is AttributeItem)
                    {   // Get type from condition attribute
                        attribute = (AttributeItem)cmbAttribute.SelectedItem;
                    }
                    var valueType = oper.ValueType;
                    var attributeType = oper.AttributeType;
                    var value = ControlUtils.GetValueFromControl(cmbValue).Trim();
                    if (valueType == AttributeTypeCode.ManagedProperty)
                    {   // Type not defined by operator
                        if (attribute != null)
                        {   // Get type from condition attribute
                            valueType = attribute.Metadata.AttributeType;
                        }
                        else
                        {   // Default, cannot determine type
                            valueType = AttributeTypeCode.String;
                        }
                    }

                    if (attributeType != null && attribute != null && control == cmbOperator)
                    {
                        if (attributeType != attribute.Metadata.AttributeType)
                        {
                            // Some attribute type combinations are ok
                            if (attributeType == AttributeTypeCode.String && attribute.Metadata.AttributeType == AttributeTypeCode.Memo)
                            {
                                // This is ok
                            }
                            else if (attributeType == AttributeTypeCode.Lookup && attribute.Metadata.AttributeType == AttributeTypeCode.Owner)
                            {
                                // This is ok
                            }
                            else if (attributeType == AttributeTypeCode.Lookup && attribute.Metadata.AttributeType == AttributeTypeCode.Customer)
                            {
                                // This is ok
                            }
                            else if (attributeType == AttributeTypeCode.Lookup && attribute.Metadata.AttributeType == AttributeTypeCode.Uniqueidentifier)
                            {
                                // This is also ok
                            }
                            else
                            {
                                return new ControlValidationResult(ControlValidationLevel.Error, "Operator " + oper.ToString() + " is not valid for attribute of type " + attribute.Metadata.AttributeType.ToString());
                            }
                        }
                    }

                    if (control == cmbValue)
                    {
                        switch (valueType)
                        {
                            case null:
                                if (!string.IsNullOrWhiteSpace(value))
                                {
                                    return new ControlValidationResult(ControlValidationLevel.Error, "Operator " + oper.ToString() + " does not allow value");
                                }
                                break;
                            case AttributeTypeCode.Boolean:
                                if (value != "0" && value != "1")
                                {
                                    return new ControlValidationResult(ControlValidationLevel.Error, "Value must be 0 or 1");
                                }
                                break;
                            case AttributeTypeCode.DateTime:
                                DateTime date;
                                if (!DateTime.TryParse(value, out date))
                                {
                                    return new ControlValidationResult(ControlValidationLevel.Error, "Operator " + oper.ToString() + " requires date value");
                                }
                                break;
                            case AttributeTypeCode.Integer:
                            case AttributeTypeCode.State:
                            case AttributeTypeCode.Status:
                            case AttributeTypeCode.Picklist:
                            case AttributeTypeCode.BigInt:
                                int intvalue;
                                if (!int.TryParse(value, out intvalue))
                                {
                                    return new ControlValidationResult(ControlValidationLevel.Error, "Operator " + oper.ToString() + " requires whole number value");
                                }
                                break;
                            case AttributeTypeCode.Decimal:
                            case AttributeTypeCode.Double:
                            case AttributeTypeCode.Money:
                                decimal decvalue;
                                if (!decimal.TryParse(value, out decvalue))
                                {
                                    return new ControlValidationResult(ControlValidationLevel.Error, "Operator " + oper.ToString() + " requires decimal value");
                                }
                                break;
                            case AttributeTypeCode.Lookup:
                            case AttributeTypeCode.Customer:
                            case AttributeTypeCode.Owner:
                            case AttributeTypeCode.Uniqueidentifier:
                                Guid guidvalue;
                                if (!Guid.TryParse(value, out guidvalue))
                                {
                                    return new ControlValidationResult(ControlValidationLevel.Error, "Operator " + oper.ToString() + " requires a proper guid with format: " + Guid.Empty.ToString());
                                }
                                break;
                            case AttributeTypeCode.String:
                            case AttributeTypeCode.Memo:
                            case AttributeTypeCode.EntityName:
                            case AttributeTypeCode.Virtual:
                                break;
                            case AttributeTypeCode.PartyList:
                            case AttributeTypeCode.CalendarRules:
                                //case AttributeTypeCode.ManagedProperty:   // ManagedProperty is a bit "undefined", so let's accept all values for now... ref issue #67
                                return new ControlValidationResult(ControlValidationLevel.Error, "Unsupported condition attribute type: " + valueType);
                        }
                    }
                }
            }

            return base.ValidateControl(control);
        }

        #endregion Protected Methods

        #region Private Methods

        private static TreeNode GetClosestEntityNode(TreeNode node)
        {
            var parentNode = node.Parent;
            while (parentNode != null && parentNode.Name != "entity" && parentNode.Name != "link-entity")
            {
                parentNode = parentNode.Parent;
            }
            return parentNode;
        }

        private void ExtractCommaSeparatedValues()
        {
            var oper = (OperatorItem)cmbOperator.SelectedItem;
            if (oper.IsMultipleValuesType && !string.IsNullOrWhiteSpace(cmbValue.Text))
            {
                // Now we need to generate value nodes under this node instead of just adding the value
                foreach (var valuestr in cmbValue.Text.Split(','))
                {
                    var value = valuestr.Trim();
                    var attrNode = TreeNodeHelper.AddChildNode(Node, "value");
                    var coll = new Dictionary<string, string>();
                    coll.Add("#text", value);
                    attrNode.Tag = coll;
                    TreeNodeHelper.SetNodeText(attrNode, fxb);
                }
                cmbValue.Text = "";
            }
        }

        private List<EntityNode> GetEntities(TreeNode node)
        {
            var result = new List<EntityNode>();
            if (node.Name == "link-entity")
            {
                result.Add(new EntityNode(node));
            }
            foreach (TreeNode child in node.Nodes)
            {
                result.AddRange(GetEntities(child));
            }
            return result;
        }

        private void RefreshAttributes()
        {
            if (Initializing)
            {
                return;
            }
            cmbAttribute.Items.Clear();
            var entityNode = cmbEntity.SelectedItem is EntityNode ? (EntityNode)cmbEntity.SelectedItem : null;
            if (entityNode == null)
            {
                entityNode = new EntityNode(GetClosestEntityNode(Node));
            }
            if (entityNode == null)
            {
                return;
            }
            var entityName = entityNode.EntityName;
            if (fxb.NeedToLoadEntity(entityName))
            {
                if (!fxb.working)
                {
                    fxb.LoadEntityDetails(entityName, RefreshAttributes);
                }
                return;
            }
            Initializing = true;
            var attributes = fxb.GetDisplayAttributes(entityName);
            attributes.ToList().ForEach(a => AttributeItem.AddAttributeToComboBox(cmbAttribute, a, true, FetchXmlBuilder.friendlyNames));
            // RefreshFill now that attributes are loaded
            ReFillControl(cmbAttribute);
            ReFillControl(cmbValue);
            Initializing = false;
            RefreshOperators();
            UpdateValueField();
        }

        private void RefreshOperators()
        {
            if (Initializing)
            {
                return;
            }
            if (cmbAttribute.SelectedItem is AttributeItem attributeItem && attributeItem.Metadata.AttributeType is AttributeTypeCode attributeType)
            {
                //cmbOperator.SelectedItem = null;
                cmbOperator.Items.Clear();
                cmbOperator.Items.AddRange(OperatorItem.GetConditionsByAttributeType(attributeType));
                ReFillControl(cmbOperator);
            }
        }

        private void UpdateValueField()
        {
            if (Initializing)
            {
                return;
            }
            panValue.Visible = true;
            panValueGuids.Visible = false;
            panValueLookup.Visible = false;
            cmbValue.Items.Clear();
            cmbValue.DropDownStyle = ComboBoxStyle.Simple;
            lblValueHint.Visible = false;
            if (cmbOperator.SelectedItem == null || !(cmbOperator.SelectedItem is OperatorItem oper))
            {
                return;
            }
            var valueType = oper.ValueType;
            var attribute = cmbAttribute.SelectedItem as AttributeItem;
            if (valueType == AttributeTypeCode.ManagedProperty && attribute != null)
            {   // Indicates value type is determined by selected attribute
                valueType = attribute.Metadata.AttributeType;
                if (oper.IsMultipleValuesType)
                {
                    if (Node.Nodes.Count == 0)
                    {
                        lblValueHint.Text = "Enter comma-separated " + valueType.ToString() + " values or add sub-nodes.";
                        lblValueHint.Visible = true;
                    }
                    else
                    {
                        valueType = null;
                    }
                }
                else if (attribute.Metadata is EnumAttributeMetadata enummeta &&
                         enummeta.OptionSet is OptionSetMetadata options &&
                         !(attribute.Metadata is EntityNameAttributeMetadata))
                {
                    cmbValue.Items.AddRange(options.Options.Select(o => new OptionsetItem(o)).ToArray());
                    cmbValue.DropDownStyle = ComboBoxStyle.DropDownList;
                }
                else if (attribute.Metadata is LookupAttributeMetadata lookupmeta)
                {
                    if (fxb.settings.UseLookup)
                    {
                        if (Guid.TryParse(cmbValue.Text, out Guid id) && !Guid.Empty.Equals(id))
                        {
                            var loookuptargets = new List<string>();
                            if (!string.IsNullOrWhiteSpace(txtUitype.Text))
                            {
                                loookuptargets.Add(txtUitype.Text.Trim());
                            }
                            else
                            {
                                loookuptargets.AddRange(lookupmeta.Targets);
                            }
                            foreach (var target in loookuptargets)
                            {
                                try
                                {
                                    txtLookup.LogicalName = target;
                                    txtLookup.Id = id;
                                    txtUitype.Text = target;
                                    break;
                                }
                                catch (FaultException<OrganizationServiceFault>)
                                {
                                    // really nothing to do here, loading the record is simply nice to have
                                }
                            }
                        }
                    }
                    else
                    {
                        txtUitype.Text = string.Empty;
                        txtLookup.Text = string.Empty;
                    }
                }
            }
            if (valueType == null)
            {
                cmbValue.Text = "";
                cmbValue.Enabled = false;
            }
            else
            {
                cmbValue.Enabled = true;

                if (cmbValue.Items.Count > 0 && cmbValue.SelectedIndex == -1 && !string.IsNullOrWhiteSpace(cmbValue.Text))
                {
                    var item = cmbValue.Items.OfType<OptionsetItem>().FirstOrDefault(i => i.ToString() == cmbValue.Text);
                    cmbValue.SelectedItem = item;
                }
            }
            if (valueType == AttributeTypeCode.Lookup ||
                valueType == AttributeTypeCode.Customer ||
                valueType == AttributeTypeCode.Owner ||
                valueType == AttributeTypeCode.Uniqueidentifier)
            {
                var showlookup = fxb.settings.UseLookup;
                if (showlookup)
                {
                    dlgLookup.LogicalNames = null;
                    if (attribute?.Metadata is LookupAttributeMetadata lookupmeta)
                    {
                        dlgLookup.LogicalNames = lookupmeta.Targets;
                    }
                    else if (attribute?.Metadata is AttributeMetadata attrmeta && attrmeta.IsPrimaryId == true && attrmeta.IsLogical == false)
                    {
                        var entitynode = new EntityNode(GetClosestEntityNode(Node));
                        dlgLookup.LogicalName = entitynode.EntityName;
                    }
                    showlookup = dlgLookup.LogicalNames?.Length > 0;
                }
                panValue.Visible = !showlookup;
                panValueGuids.Visible = !showlookup;
                panValueLookup.Visible = showlookup;
            }
        }

        #endregion Private Methods

        #region Private Event Handlers

        private void btnGetGuid_Click(object sender, EventArgs e)
        {
            cmbValue.Text = Guid.NewGuid().ToString();
        }

        private void btnGetGuidEmpty_Click(object sender, EventArgs e)
        {
            cmbValue.Text = Guid.Empty.ToString();
        }

        private void btnLookup_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            switch (dlgLookup.ShowDialog(this))
            {
                case DialogResult.OK:
                    txtLookup.Entity = dlgLookup.Entity;
                    txtUitype.Text = dlgLookup.Entity.LogicalName;
                    break;
                case DialogResult.Abort:
                    txtLookup.Entity = null;
                    break;
            }
            cmbValue.Text = (txtLookup?.Entity?.Id ?? Guid.Empty).ToString();
            Cursor = Cursors.Default;
        }

        private void cmbAttribute_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshOperators();
        }

        private void cmbEntity_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshAttributes();
        }

        private void cmbOperator_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateValueField();
        }

        private void txtLookup_RecordClick(object sender, CDSRecordEventArgs e)
        {
            var url = fxb.ConnectionDetail.GetEntityUrl(e.Entity);
            url = fxb.ConnectionDetail.GetEntityReferenceUrl(txtLookup.EntityReference);
            if (!string.IsNullOrEmpty(url))
            {
                fxb.LogUse("OpenRecord");
                Process.Start(url);
            }
        }

        #endregion Private Event Handlers
    }
}