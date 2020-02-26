using Cinteros.Xrm.FetchXmlBuilder.AppCode;
using Cinteros.Xrm.FetchXmlBuilder.DockControls;
using Cinteros.Xrm.XmlEditorUtils;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Cinteros.Xrm.FetchXmlBuilder.Controls
{
    public partial class conditionControl : FetchXmlElementControlBase
    {
        public conditionControl() : this(null, null, null)
        {
        }

        public conditionControl(TreeNode node, FetchXmlBuilder fetchXmlBuilder, TreeBuilderControl tree)
        {
            InitializeComponent();
            InitializeFXB(null, fetchXmlBuilder, tree, node);
            RefreshAttributes();
        }

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

        private void cmbEtity_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshAttributes();
        }

        private void RefreshAttributes()
        {
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
            var attributes = fxb.GetDisplayAttributes(entityName);
            foreach (var attribute in attributes)
            {
                AttributeItem.AddAttributeToComboBox(cmbAttribute, attribute, true, FetchXmlBuilder.friendlyNames);
            }
            // RefreshFill now that attributes are loaded
            FillControl(cmbAttribute);
            FillControl(cmbValue);
            ValidateControls(true);
        }

        private static TreeNode GetClosestEntityNode(TreeNode node)
        {
            var parentNode = node.Parent;
            while (parentNode != null && parentNode.Name != "entity" && parentNode.Name != "link-entity")
            {
                parentNode = parentNode.Parent;
            }
            return parentNode;
        }

        private void cmbOperator_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateValueField();
        }

        private void UpdateValueField()
        {
            btnGetGuid.Visible = false;
            cmbValue.Items.Clear();
            cmbValue.DropDownStyle = ComboBoxStyle.Simple;
            lblValueHint.Visible = false;
            if (cmbOperator.SelectedItem != null && cmbOperator.SelectedItem is OperatorItem)
            {
                var oper = (OperatorItem)cmbOperator.SelectedItem;
                var valueType = oper.ValueType;
                if (valueType == AttributeTypeCode.ManagedProperty)
                {   // Indicates value type is determined by selected attribute
                    if (cmbAttribute.SelectedItem != null && cmbAttribute.SelectedItem is AttributeItem)
                    {
                        var attribute = (AttributeItem)cmbAttribute.SelectedItem;
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
                        else if (attribute.Metadata is EnumAttributeMetadata && !(attribute.Metadata is EntityNameAttributeMetadata))
                        {
                            var options = ((EnumAttributeMetadata)attribute.Metadata).OptionSet;
                            if (options != null)
                            {
                                foreach (var option in options.Options)
                                {
                                    cmbValue.Items.Add(new OptionsetItem(option));
                                }
                            }
                            cmbValue.DropDownStyle = ComboBoxStyle.DropDownList;
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
                if (valueType == AttributeTypeCode.Lookup || valueType == AttributeTypeCode.Customer || valueType == AttributeTypeCode.Owner || valueType == AttributeTypeCode.Uniqueidentifier)
                {
                    btnGetGuid.Visible = true;
                }
            }
        }

        private void btnGetGuid_Click(object sender, EventArgs e)
        {
            cmbValue.Text = Guid.NewGuid().ToString();
        }

        private void cmbAttribute_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbAttribute.SelectedItem != null)
            {
                var attributeType = ((AttributeItem)cmbAttribute.SelectedItem).Metadata.AttributeType;
                if (attributeType.HasValue)
                {
                    var tmpColl = ControlUtils.GetAttributesCollection(this.Controls, false);
                    cmbOperator.SelectedItem = null;
                    cmbOperator.Items.Clear();
                    cmbOperator.Items.AddRange(OperatorItem.GetConditionsByAttributeType(attributeType.Value));
                    ControlUtils.FillControl(tmpColl, cmbOperator, this);
                }
            }
            UpdateValueField();
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
    }

    public class EntityNode
    {
        public string EntityName;
        private string name;

        public EntityNode(TreeNode node)
        {
            EntityName = TreeNodeHelper.GetAttributeFromNode(node, "name");
            var alias = TreeNodeHelper.GetAttributeFromNode(node, "alias");
            name = !string.IsNullOrEmpty(alias) ? alias : EntityName;
        }

        public override string ToString()
        {
            return name;
        }
    }
}