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

namespace Cinteros.Xrm.FetchXmlBuilder.Controls
{
    public partial class conditionControl : FetchXmlElementControlBase
    {
        #region Private Properties

        private bool valueOfSupported = false;

        #endregion Private Properties

        #region Public Constructors

        public conditionControl() : this(null, null, null)
        {
        }

        public conditionControl(TreeNode node, FetchXmlBuilder fetchXmlBuilder, TreeBuilderControl tree)
        {
            InitializeComponent();
            BeginInit();
            valueOfSupported = fetchXmlBuilder.CDSVersion >= new Version(9, 1, 0, 19562);
            xrmRecord.Service = fetchXmlBuilder.Service;
            dlgLookup.Service = fetchXmlBuilder.Service;
            rbUseLookup.Checked = fetchXmlBuilder.settings.UseLookup;
            rbEnterGuid.Checked = !rbUseLookup.Checked;
            InitializeFXB(null, fetchXmlBuilder, tree, node);
            EndInit();
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

        protected override Dictionary<string, string> GetAttributesCollection()
        {
            var attr = base.GetAttributesCollection();

            if (attr.TryGetValue("operator", out var op) && attr.TryGetValue("value", out var value))
            {
                if (op == "begins-with")
                {
                    attr["operator"] = "like";
                    attr["value"] = EscapeLikeWildcards(value) + "%";
                }
                else if (op == "not-begin-with")
                {
                    attr["operator"] = "not-like";
                    attr["value"] = EscapeLikeWildcards(value) + "%";
                }
                else if (op == "ends-with")
                {
                    attr["operator"] = "like";
                    attr["value"] = "%" + EscapeLikeWildcards(value);
                }
                else if (op == "not-end-with")
                {
                    attr["operator"] = "not-like";
                    attr["value"] = "%" + EscapeLikeWildcards(value);
                }
                else if (op == "contains")
                {
                    attr["operator"] = "like";
                    attr["value"] = "%" + EscapeLikeWildcards(value) + "%";
                }
                else if (op == "does-not-contain")
                {
                    attr["operator"] = "not-like";
                    attr["value"] = "%" + EscapeLikeWildcards(value) + "%";
                }
            }

            return attr;
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
                        if (!string.IsNullOrWhiteSpace(cmbValueOf.Text) && !string.IsNullOrWhiteSpace(cmbValue.Text))
                        {
                            return new ControlValidationResult(ControlValidationLevel.Error, "Value and Value Of cannot both be set");
                        }

                        if (!string.IsNullOrWhiteSpace(cmbValueOf.Text))
                        {
                            return null;
                        }

                        if (oper.GetValue() == "like" && !string.IsNullOrWhiteSpace(cmbValue.Text))
                        {
                            // Check for mismatched square brackets
                            var inBrackets = false;

                            foreach (var ch in cmbValue.Text)
                            {
                                if (ch == '[')
                                {
                                    inBrackets = true;
                                }
                                else if (ch == ']')
                                {
                                    inBrackets = false;
                                }
                            }

                            if (inBrackets)
                            {
                                // Last open bracket was not closed
                                return new ControlValidationResult(ControlValidationLevel.Error, "LIKE pattern has mismatched brackets. Add closing brackets for each character range.\r\n\r\nTo match the character '[', use '[[]'");
                            }
                        }

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
                            case AttributeTypeCode.EntityName:
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
            if (!IsInitialized)
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
            BeginInit();
            var attributes = fxb.GetDisplayAttributes(entityName);
            attributes.ToList().ForEach(a => AttributeItem.AddAttributeToComboBox(cmbAttribute, a, true, FetchXmlBuilder.friendlyNames));
            // RefreshFill now that attributes are loaded
            ReFillControl(cmbAttribute);
            ReFillControl(cmbValue);
            EndInit();
            RefreshOperators();
            UpdateValueField();
            RefreshValueOf();
            NormalizeLike();
        }

        private void NormalizeLike()
        {
            var op = cmbOperator.SelectedItem as OperatorItem;

            if (op == null)
            {
                return;
            }

            var value = cmbValue.Text;

            if (String.IsNullOrWhiteSpace(value))
            {
                return;
            }

            if (op.GetValue() == "like" || op.GetValue() == "not-like")
            {
                string newOp = null;

                if (value.StartsWith("%") && value.EndsWith("%") && value.Length >= 2)
                {
                    newOp = "contains";
                    value = value.Substring(1, value.Length - 2);
                }
                else if (value.StartsWith("%"))
                {
                    newOp = "ends-with";
                    value = value.Substring(1);
                }
                else if (value.EndsWith("%"))
                {
                    newOp = "begins-with";
                    value = value.Substring(0, value.Length - 1);
                }

                if (newOp != null && AreAllLikeWildcardsEscaped(value))
                {
                    value = UnescapeLikeWildcards(value);

                    if (op.GetValue() == "not-like")
                    {
                        newOp = "not-" + newOp.Replace("s-", "-");

                        if (newOp == "not-contains")
                        {
                            newOp = "does-not-contain";
                        }
                    }

                    var newOpItem = cmbOperator.Items.OfType<OperatorItem>().FirstOrDefault(o => o.GetValue() == newOp);

                    if (newOpItem != null)
                    {
                        cmbOperator.SelectedItem = newOpItem;
                        cmbValue.Text = value;
                    }
                }
            }
        }

        private string EscapeLikeWildcards(string value)
        {
            return value
                .Replace("[", "[[]")
                .Replace("%", "[%]")
                .Replace("_", "[_]");
        }

        private bool AreAllLikeWildcardsEscaped(string value)
        {
            var bracketStart = -1;

            for (var i = 0; i < value.Length; i++)
            {
                var ch = value[i];

                if (ch != '%' && ch != '_' && ch != '[' && ch != ']')
                {
                    if (bracketStart != -1)
                    {
                        // We've got a non-wildcard character in brackets - it's not an escaped wildcard
                        return false;
                    }

                    continue;
                }

                if (bracketStart == -1)
                {
                    if (ch == '[')
                    {
                        bracketStart = i;
                    }
                    else
                    {
                        // We've got a wildcard character outside of brackets - it's not escaped
                        return false;
                    }
                }

                if (ch == ']')
                {
                    if (i > bracketStart + 2)
                    {
                        // We've got more than a single character in the brackets - it's not a single escaped wildcard
                        return false;
                    }

                    bracketStart = -1;
                }
            }

            return true;
        }

        private string UnescapeLikeWildcards(string value)
        {
            return value
                .Replace("[_]", "_")
                .Replace("[%]", "%")
                .Replace("[[]", "[");
        }

        private void RefreshOperators()
        {
            if (!IsInitialized)
            {
                return;
            }
            if (cmbAttribute.SelectedItem is AttributeItem attributeItem && attributeItem.Metadata.AttributeType is AttributeTypeCode attributeType)
            {
                //cmbOperator.SelectedItem = null;
                cmbOperator.Items.Clear();
                cmbOperator.Items.AddRange(OperatorItem.GetConditionsByAttributeType(attributeType, attributeItem.Metadata.AttributeTypeName?.Value));
                ReFillControl(cmbOperator);
            }
        }

        private void RefreshValueOf()
        {
            if (!IsInitialized)
            {
                return;
            }
            panValueOf.Visible = false;
            if (!valueOfSupported || !(cmbOperator.SelectedItem is OperatorItem oper) || !(cmbAttribute.SelectedItem is AttributeItem attribute))
            {
                return;
            }
            if (oper.SupportsColumnComparison && !(cmbEntity.SelectedItem is EntityNode))
            {
                panValueOf.Visible = true;
                cmbValueOf.Items.Clear();
                if (attribute != null)
                {
                    cmbValueOf.Items.AddRange(cmbAttribute.Items
                        .Cast<AttributeItem>()
                        .Where(a => a.Metadata.AttributeType == attribute.Metadata.AttributeType)
                        .Select(a => new AttributeItem(a.Metadata))
                        .ToArray());
                }
            }
            else
            {
                cmbValueOf.Text = "";
            }
            ReFillControl(cmbValueOf);
        }

        private void UpdateValueField()
        {
            if (!IsInitialized)
            {
                return;
            }
            panValue.Visible = true;
            panValueLookup.Visible = false;
            panGuidSelector.Visible = false;
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
                var managedProp = attribute.Metadata as ManagedPropertyAttributeMetadata;
                if (managedProp != null)
                {
                    valueType = managedProp.ValueAttributeTypeCode;
                }
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
                    var value = cmbValue.Text;
                    cmbValue.DropDownStyle = ComboBoxStyle.DropDownList;
                    cmbValue.SelectedItem = cmbValue.Items.OfType<OptionsetItem>().FirstOrDefault(i => i.GetValue() == value);
                }
                else if (attribute.Metadata is EntityNameAttributeMetadata)
                {
                    var entities = fxb.GetDisplayEntities();
                    if (entities != null)
                    {
                        cmbValue.Items.AddRange(entities.Select(e => new EntityNameItem(e.Value)).ToArray());
                        var value = cmbValue.Text;
                        cmbValue.DropDownStyle = ComboBoxStyle.DropDownList;
                        cmbValue.SelectedItem = cmbValue.Items.OfType<EntityNameItem>().FirstOrDefault(i => i.GetValue() == value);
                    }
                }
                else if (fxb.settings.UseLookup
                    && (attribute.Metadata is LookupAttributeMetadata
                        || attribute.Metadata.AttributeType == AttributeTypeCode.Uniqueidentifier)
                    && Guid.TryParse(cmbValue.Text, out Guid id) && !Guid.Empty.Equals(id))
                {
                    var loookuptargets = new List<string>();
                    if (!string.IsNullOrWhiteSpace(txtUitype.Text))
                    {
                        loookuptargets.Add(txtUitype.Text.Trim());
                    }
                    else if (attribute.Metadata is LookupAttributeMetadata lookupmeta)
                    {
                        loookuptargets.AddRange(lookupmeta.Targets);
                    }
                    else if (attribute.Metadata.AttributeType == AttributeTypeCode.Uniqueidentifier)
                    {
                        loookuptargets.Add(attribute.Metadata.EntityLogicalName);
                    }
                    foreach (var target in loookuptargets)
                    {
                        try
                        {
                            xrmRecord.LogicalName = target;
                            xrmRecord.Id = id;
                            txtUitype.Text = target;
                            break;
                        }
                        catch (FaultException<OrganizationServiceFault>)
                        {
                            // really nothing to do here, loading the record is simply nice to have
                        }
                    }
                }
                else if (managedProp != null && managedProp.ValueAttributeTypeCode == AttributeTypeCode.Boolean)
                {
                    cmbValue.Items.Add(new OptionsetItem(new OptionMetadata(new Microsoft.Xrm.Sdk.Label(new LocalizedLabel("False", 0), null), 0)));
                    cmbValue.Items.Add(new OptionsetItem(new OptionMetadata(new Microsoft.Xrm.Sdk.Label(new LocalizedLabel("True", 0), null), 1)));
                    var value = cmbValue.Text;
                    cmbValue.DropDownStyle = ComboBoxStyle.DropDownList;
                    cmbValue.SelectedItem = cmbValue.Items.OfType<OptionsetItem>().FirstOrDefault(i => i.GetValue() == value);
                }
                else
                {
                    xrmRecord.Record = null;
                    txtUitype.Text = string.Empty;
                    txtLookup.Text = string.Empty;
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
            }

            if (valueType == AttributeTypeCode.Lookup ||
                valueType == AttributeTypeCode.Customer ||
                valueType == AttributeTypeCode.Owner ||
                valueType == AttributeTypeCode.Uniqueidentifier)
            {
                dlgLookup.LogicalNames = null;
                if (attribute?.Metadata is LookupAttributeMetadata lookupmeta)
                {
                    dlgLookup.LogicalNames = lookupmeta.Targets;
                }
                else if (attribute?.Metadata is AttributeMetadata attrmeta && attrmeta.IsPrimaryId == true)
                {
                    if (attrmeta.IsLogical == false)
                    {
                        var entitynode = new EntityNode(GetClosestEntityNode(Node));
                        dlgLookup.LogicalName = entitynode.EntityName;
                    }
                    else if (attrmeta.LogicalName.EndsWith("addressid"))
                    {
                        dlgLookup.LogicalName = "customeraddress";
                    }
                }
                rbUseLookup.Enabled = dlgLookup.LogicalNames?.Length > 0;
                if (!rbUseLookup.Enabled)
                {
                    rbEnterGuid.Checked = true;
                }
                if (string.IsNullOrWhiteSpace(cmbValue.Text))
                {
                    cmbValue.Text = Guid.Empty.ToString();
                }
                panGuidSelector.Visible = true;
                panValue.Visible = !rbUseLookup.Checked;
                panValueLookup.Visible = rbUseLookup.Checked;
            }
        }

        #endregion Private Methods

        #region Private Event Handlers

        private void btnLookup_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            switch (dlgLookup.ShowDialog(this))
            {
                case DialogResult.OK:
                    xrmRecord.Record = dlgLookup.Record;
                    txtUitype.Text = dlgLookup.Record.LogicalName;
                    break;

                case DialogResult.Abort:
                    xrmRecord.Record = null;
                    break;
            }
            cmbValue.Text = (xrmRecord?.Record?.Id ?? Guid.Empty).ToString();
            Cursor = Cursors.Default;
        }

        private void cmbAttribute_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshOperators();
            UpdateValueField();
            RefreshValueOf();
        }

        private void cmbEntity_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshAttributes();
        }

        private void cmbOperator_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateValueField();
            RefreshValueOf();
        }

        private void rbUseLookup_CheckedChanged(object sender, EventArgs e)
        {
            UpdateValueField();
        }

        private void txtLookup_Click(object sender, EventArgs e)
        {
            var url = fxb.ConnectionDetail.GetEntityUrl(xrmRecord.Record);
            url = fxb.ConnectionDetail.GetEntityReferenceUrl(xrmRecord.Record.ToEntityReference());
            if (!string.IsNullOrEmpty(url))
            {
                fxb.LogUse("OpenRecord");
                Process.Start(url);
            }
        }

        #endregion Private Event Handlers

        private void helpIcon_Click(object sender, EventArgs e)
        {
            fxb.HelpClick(sender);
        }
    }
}