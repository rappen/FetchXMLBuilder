using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using Rappen.XRM.Helpers.FetchXML;
using Rappen.XTB.FetchXmlBuilder.Builder;
using Rappen.XTB.FetchXmlBuilder.ControlsClasses;
using Rappen.XTB.FetchXmlBuilder.DockControls;
using Rappen.XTB.FetchXmlBuilder.Extensions;
using Rappen.XTB.XmlEditorUtils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.Windows.Forms;

namespace Rappen.XTB.FetchXmlBuilder.Controls
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
            var collect = (Dictionary<string, string>)node.Tag;
            if (collect != null && collect.TryGetValue("valueof", out var valueof) && valueof.Split('.') is string[] valueofsplits && valueofsplits.Length == 2)
            {
                collect.Add("valueofalias", valueofsplits[0]);
                collect["valueof"] = valueofsplits[1];
            }
            InitializeFXB(collect, fetchXmlBuilder, tree, node);
            EndInit();
            RefreshAttributes();
            EnableValueFields();
        }

        #endregion Public Constructors

        #region Public Override Methods

        public override MetadataBase Metadata()
        {
            if (cmbAttribute.SelectedItem is AttributeItem item)
            {
                return item.Metadata;
            }
            return base.Metadata();
        }

        public override void Focus()
        {
            cmbAttribute.Focus();
        }

        public override bool Save(bool keyPress)
        {
            EnableValueFields();
            return base.Save(keyPress);
        }

        #endregion Public Override Methods

        #region Protected Override Methods

        protected override void PopulateControls()
        {
            SetEntitiesAliases(cmbEntity, false);
            SetEntitiesAliases(cmbValueOfAlias, true);
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
                    return new ControlValidationResult(ControlValidationLevel.Error, "Attribute", ControlValidationMessage.IsRequired);
                }

                if (fxb.entities != null && !cmbAttribute.Items.OfType<AttributeItem>().Any(i => i.ToString() == cmbAttribute.Text))
                {
                    return new ControlValidationResult(ControlValidationLevel.Warning, "Attribute", ControlValidationMessage.InValid);
                }
            }
            else if (control == cmbOperator || control == cmbValue)
            {
                if (control == cmbOperator && string.IsNullOrWhiteSpace(cmbOperator.Text))
                {
                    return new ControlValidationResult(ControlValidationLevel.Error, "Operator", ControlValidationMessage.IsRequired);
                }

                if (control == cmbOperator && !cmbOperator.Items.OfType<OperatorItem>().Any(i => i.ToString() == cmbOperator.Text))
                {
                    return new ControlValidationResult(ControlValidationLevel.Error, "Operator", ControlValidationMessage.InValid);
                }

                if (control == cmbOperator && cmbOperator.SelectedItem is OperatorItem opercon && (opercon.GetValue() == "contains" || opercon.GetValue() == "does-not-contain"))
                {
                    return new ControlValidationResult(ControlValidationLevel.Warning, "Contains (and not) are available, but not supported for FetchXml.",
                        "https://docs.microsoft.com/en-us/power-apps/developer/data-platform/fetchxml-schema#:~:text=%3Cxs%3AsimpleType%20name%3D%22operator%22%3E");
                }

                if (cmbOperator.SelectedItem != null && cmbOperator.SelectedItem is OperatorItem oper && (!oper.IsMultipleValuesType || Node.Nodes.Count == 0))
                {
                    var attribute = cmbAttribute.SelectedItem as AttributeItem; ;
                    var valueType = GetValueType(oper, attribute);
                    var attributeType = oper.AttributeType;
                    var value = ControlUtils.GetValueFromControl(cmbValue).Trim();

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
                                return new ControlValidationResult(ControlValidationLevel.Error, "Operator " + oper.ToString() + " is not valid for attribute of type " + attribute.Metadata.ToTypeString());
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
                                    if (oper.IsMultipleValuesType && !string.IsNullOrWhiteSpace(value))
                                    {
                                        var split = value.Split(',');
                                        if (split.Any(v => v.Trim() != "0" && v.Trim() != "1"))
                                        {
                                            return new ControlValidationResult(ControlValidationLevel.Error, "Value must be 0 or 1, separated by commas");
                                        }
                                    }
                                    else
                                    {
                                        return new ControlValidationResult(ControlValidationLevel.Error, "Value must be 0 or 1");
                                    }
                                }
                                break;

                            case AttributeTypeCode.DateTime:
                                if (!DateTime.TryParse(value, out DateTime _))
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
                                if (!int.TryParse(value, out int _))
                                {
                                    if (oper.IsMultipleValuesType && !string.IsNullOrWhiteSpace(value))
                                    {
                                        var split = value.Split(',');
                                        if (split.Any(v => !int.TryParse(v.Trim(), out int _)))
                                        {
                                            return new ControlValidationResult(ControlValidationLevel.Error, "Operator " + oper.ToString() + " requires whole number values separated by commas");
                                        }
                                    }
                                    else
                                    {
                                        return new ControlValidationResult(ControlValidationLevel.Error, "Operator " + oper.ToString() + " requires whole number value");
                                    }
                                }
                                break;

                            case AttributeTypeCode.Decimal:
                            case AttributeTypeCode.Double:
                            case AttributeTypeCode.Money:
                                if (!decimal.TryParse(value, out decimal _))
                                {
                                    return new ControlValidationResult(ControlValidationLevel.Error, "Operator " + oper.ToString() + " requires decimal value");
                                }
                                break;

                            case AttributeTypeCode.Lookup:
                            case AttributeTypeCode.Customer:
                            case AttributeTypeCode.Owner:
                            case AttributeTypeCode.Uniqueidentifier:
                                if (!Guid.TryParse(value, out Guid _))
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

        protected override Dictionary<string, string> GetAttributesCollection()
        {
            var result = base.GetAttributesCollection();
            if (panValueOf.Visible == false && result.ContainsKey("valueof"))
            {
                result.Remove("valueof");
            }
            if (panValueOf.Visible == true && result.ContainsKey("valueof") && !string.IsNullOrWhiteSpace(result["valueof"]))
            {
                if (result.ContainsKey("value"))
                {
                    result.Remove("value");
                }
            }
            else if (!result.ContainsKey("value") && cmbValue.Enabled && cmbValue.DropDownStyle == ComboBoxStyle.Simple)
            {
                result.Add("value", "");
            }
            if (cmbOperator.SelectedItem is OperatorItem oper && oper.IsMultipleValuesType)
            {
                result.Remove("value");
                result.Remove("valueof");
            }
            if (result.TryGetValue("valueofalias", out var alias))
            {
                if (!string.IsNullOrWhiteSpace(alias) &&
                    result.TryGetValue("valueof", out var valueof) &&
                    !string.IsNullOrWhiteSpace(valueof))
                {
                    result["valueof"] = alias + "." + valueof;
                }
                result.Remove("valueofalias");
            }
            return result;
        }

        #endregion Protected Override Methods

        #region Private Methods

        private static AttributeTypeCode? GetValueType(OperatorItem oper, AttributeItem attributeitem)
        {
            if (oper == null)
            {
                return null;
            }
            var valueType = oper.ValueType;
            if (valueType == AttributeTypeCode.ManagedProperty)
            {   // Type not defined by operator
                if (attributeitem != null)
                {   // Get type from condition attribute
                    valueType = attributeitem.Metadata.AttributeType;
                }
                else
                {   // Default, cannot determine type
                    valueType = AttributeTypeCode.String;
                }
            }

            return valueType;
        }

        private void EnableValueFields()
        {
            panValueOf.Enabled = string.IsNullOrEmpty(cmbValue.Text);
            if (!panValueOf.Enabled)
            {
                cmbValueOf.Text = null;
                cmbValueOfAlias.Text = null;
            }
            cmbValue.Enabled = string.IsNullOrEmpty(cmbValueOf.Text);
            if (!cmbValue.Enabled)
            {
                cmbValue.Text = null;
            }
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
                    var attrNode = TreeNodeHelper.AddChildNode(Node, "value", fxb);
                    var coll = new Dictionary<string, string>();
                    coll.Add("#text", value);
                    attrNode.Tag = coll;
                    TreeNodeHelper.SetNodeText(attrNode, fxb);
                }
                cmbValue.Text = "";
            }
        }

        private List<EntityNode> GetEntities(TreeNode node, bool needsalias)
        {
            var result = new List<EntityNode>();
            if (node.HeritanceOfFilter())
            {
                return result;
            }
            if (node.Name == "link-entity" && (!needsalias || !string.IsNullOrWhiteSpace(node.Value("alias"))))
            {
                result.Add(new EntityNode(node));
            }
            foreach (TreeNode child in node.Nodes)
            {
                result.AddRange(GetEntities(child, needsalias));
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
                entityNode = new EntityNode(Node.LocalEntityNode());
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
            cmbAttribute.Items.AddRange(attributes?.Select(a => new AttributeItem(a, fxb.settings.ShowAttributeTypes)).ToArray());
            // RefreshFill now that attributes are loaded
            ReFillControl(cmbAttribute);
            ReFillControl(cmbValue);
            EndInit();
            RefreshOperators();
            UpdateValueField();
            RefreshValueOf();
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
                if (cmbOperator.SelectedItem == null && cmbOperator.Items.Count > 0)
                {
                    cmbOperator.SelectedIndex = 0;
                }
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

                var aliasyNode = cmbValueOfAlias.SelectedItem is EntityNode ? (EntityNode)cmbValueOfAlias.SelectedItem : null;
                if (aliasyNode == null)
                {
                    aliasyNode = new EntityNode(Node.LocalEntityNode());
                }
                if (aliasyNode == null)
                {
                    return;
                }
                var entityName = aliasyNode.EntityName;
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
                cmbValueOf.Items.AddRange(attributes?
                    .Select(a => new AttributeItem(a, fxb.settings.ShowAttributeTypes))
                    .Where(a => TypeIsCloseEnough(a.Metadata, attribute.Metadata))
                    .ToArray());
                // RefreshFill now that attributes are loaded
                //ReFillControl(cmbValueOf);
                EndInit();
            }
            else
            {
                cmbValueOf.Text = "";
            }
            //ReFillControl(cmbValueOf);
        }

        private bool TypeIsCloseEnough(AttributeMetadata comparer, AttributeMetadata target)
        {
            if (comparer.AttributeType == target.AttributeType)
            {
                return true;
            }
            if (target is StringAttributeMetadata)
            {
                return comparer is MemoAttributeMetadata;
            }
            if (target is MemoAttributeMetadata)
            {
                return comparer is StringAttributeMetadata;
            }
            if (target is EnumAttributeMetadata)
            {
                return comparer is IntegerAttributeMetadata;
            }
            if (target is IntegerAttributeMetadata)
            {
                return comparer is EnumAttributeMetadata;
            }
            return false;
        }

        private void SetEntitiesAliases(ComboBox cmb, bool needsalias)
        {
            cmb.Items.Clear();
            var closestEntity = Node.LocalEntityNode();
            if (closestEntity != null && closestEntity.Name == "entity")
            {
                cmb.Items.Add("");
                cmb.Items.AddRange(GetEntities(Tree.tvFetch.Nodes[0], needsalias).ToArray());
            }
            cmb.Enabled = cmb.Items.Count > 1;
            if (cmb.Parent is Panel pan)
            {
                pan.Visible = cmb.Enabled;
            }
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
            cmbValue.AutoCompleteMode = AutoCompleteMode.None;
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
                    cmbValue.Items.Add("");
                    cmbValue.Items.AddRange(options.Options.Select(o => new OptionsetItem(o)).ToArray());
                    var value = cmbValue.Text;
                    cmbValue.DropDownStyle = ComboBoxStyle.DropDownList;
                    cmbValue.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                    cmbValue.SelectedItem = cmbValue.Items.OfType<OptionsetItem>().FirstOrDefault(i => i.GetValue() == value);
                }
                else if (attribute.Metadata is BooleanAttributeMetadata boolmeta)
                {
                    cmbValue.Items.Add("");
                    cmbValue.Items.Add(new OptionsetItem(boolmeta.OptionSet.FalseOption));
                    cmbValue.Items.Add(new OptionsetItem(boolmeta.OptionSet.TrueOption));
                    var value = cmbValue.Text;
                    cmbValue.DropDownStyle = ComboBoxStyle.DropDownList;
                    cmbValue.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                    cmbValue.SelectedItem = cmbValue.Items.OfType<OptionsetItem>().FirstOrDefault(i => i.GetValue() == value);
                }
                else if (attribute.Metadata is EntityNameAttributeMetadata)
                {
                    var entities = fxb.GetDisplayEntities();
                    if (entities != null)
                    {
                        cmbValue.Items.Add("");
                        cmbValue.Items.AddRange(entities.Select(e => new EntityNameItem(e)).ToArray());
                        var value = cmbValue.Text;
                        cmbValue.DropDownStyle = ComboBoxStyle.DropDownList;
                        cmbValue.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
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
                    cmbValue.Items.Add("");
                    cmbValue.Items.Add(new OptionsetItem(new OptionMetadata(new Microsoft.Xrm.Sdk.Label("False", 0), 0)));
                    cmbValue.Items.Add(new OptionsetItem(new OptionMetadata(new Microsoft.Xrm.Sdk.Label("True", 0), 1)));
                    var value = cmbValue.Text;
                    cmbValue.DropDownStyle = ComboBoxStyle.DropDownList;
                    cmbValue.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
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
                        var entitynode = new EntityNode(Node.LocalEntityNode());
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
            fxb.ShowMetadata(Metadata());
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
            if (fxb?.settings != null)
            {
                fxb.settings.UseLookup = rbUseLookup.Checked;
            }
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

        private void helpIcon_Click(object sender, EventArgs e)
        {
            FetchXmlBuilder.HelpClick(sender);
        }

        private void cmbValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar))
            {
                return;
            }
            var valuetype = GetValueType(cmbOperator.SelectedItem as OperatorItem, cmbAttribute.SelectedItem as AttributeItem);
            if (valuetype.IsDecimalable())
            {
                var sepcurrent = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0];
                if (e.KeyChar == sepcurrent)
                {
                    var sepinvariant = CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator[0];
                    if (sepinvariant != sepcurrent)
                    {
                        e.KeyChar = sepinvariant;
                    }
                }
            }
        }

        private void cmbValueOfAlias_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshValueOf();
        }

        #endregion Private Event Handlers
    }
}