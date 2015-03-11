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
using Microsoft.Xrm.Sdk.Query;

namespace Cinteros.Xrm.FetchXmlBuilder.Controls
{
    public partial class conditionControl : UserControl, IDefinitionSavable
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

        public conditionControl()
        {
            InitializeComponent();
            collec = new Dictionary<string, string>();
        }

        public conditionControl(TreeNode Node, FetchXmlBuilder fetchXmlBuilder)
            : this()
        {
            form = fetchXmlBuilder;
            node = Node;
            collec = (Dictionary<string, string>)Node.Tag;
            if (collec == null)
            {
                collec = new Dictionary<string, string>();
            }
            PopulateControls();
            RefreshAttributes();
            ControlUtils.FillControls(collec, this.Controls);
            controlsCheckSum = ControlUtils.ControlsChecksum(this.Controls);
            Saved += fetchXmlBuilder.CtrlSaved;
        }

        private void PopulateControls()
        {
            cmbEntity.Items.Clear();
            var closestEntity = GetClosestEntityNode(node);
            if (closestEntity != null && closestEntity.Name == "entity")
            {
                cmbEntity.Items.AddRange(GetEntities(form.tvFetch.Nodes[0]).ToArray());
            }
            cmbEntity.Enabled = cmbEntity.Items.Count > 0;
            cmbOperator.Items.Clear();
            foreach (var oper in System.Enum.GetValues(typeof(ConditionOperator)))
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

        public void Save()
        {
            try
            {
                if (ValidateForm())
                {
                    Dictionary<string, string> collection = ControlUtils.GetAttributesCollection(this.Controls, true);
                    SendSaveMessage(collection);
                    controlsCheckSum = ControlUtils.ControlsChecksum(this.Controls);
                }
            }
            catch (ArgumentNullException ex)
            {
                MessageBox.Show(ex.Message, "Validation", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private bool ValidateForm()
        {
            var result = true;
            if (cmbOperator.SelectedItem != null && cmbOperator.SelectedItem is OperatorItem)
            {
                var oper = (OperatorItem)cmbOperator.SelectedItem;
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
                var error = "";
                if (attributeType != null && attribute != null)
                {
                    if (attributeType != attribute.Metadata.AttributeType)
                    {
                        if (attributeType != AttributeTypeCode.Lookup ||
                            (attribute.Metadata.AttributeType != AttributeTypeCode.Owner &&
                             attribute.Metadata.AttributeType != AttributeTypeCode.Customer))
                        {
                            error = "Operator " + oper.ToString() + " is not valid for attribute of type " + attribute.Metadata.AttributeType.ToString();
                        }
                    }
                }
                switch (valueType)
                {
                    case null:
                        if (!string.IsNullOrWhiteSpace(value))
                        {
                            error = "Operator " + oper.ToString() + " does not allow value";
                        }
                        break;
                    case AttributeTypeCode.Boolean:
                        if (value != "0" && value != "1")
                        {
                            error = "Value must be 0 or 1";
                        }
                        break;
                    case AttributeTypeCode.DateTime:
                        DateTime date;
                        if (!DateTime.TryParse(value, out date))
                        {
                            error = "Operator " + oper.ToString() + " requires date value";
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
                            error = "Operator " + oper.ToString() + " requires whole number value";
                        }
                        break;
                    case AttributeTypeCode.Decimal:
                    case AttributeTypeCode.Double:
                    case AttributeTypeCode.Money:
                        decimal decvalue;
                        if (!decimal.TryParse(value, out decvalue))
                        {
                            error = "Operator " + oper.ToString() + " requires decimal value";
                        }
                        break;
                    case AttributeTypeCode.Lookup:
                    case AttributeTypeCode.Customer:
                    case AttributeTypeCode.Owner:
                    case AttributeTypeCode.Uniqueidentifier:
                        Guid guidvalue;
                        if (!Guid.TryParse(value, out guidvalue))
                        {
                            error = "Operator " + oper.ToString() + " requires a proper guid with format: " + Guid.Empty.ToString();
                        }
                        break;
                    case AttributeTypeCode.String:
                    case AttributeTypeCode.Memo:
                    case AttributeTypeCode.EntityName:
                    case AttributeTypeCode.Virtual:
                        break;
                    case AttributeTypeCode.PartyList:
                    case AttributeTypeCode.CalendarRules:
                    case AttributeTypeCode.ManagedProperty:
                        error = "Unsupported condition attribute type: " + valueType;
                        break;
                }

                if (!string.IsNullOrWhiteSpace(error))
                {
                    MessageBox.Show(error, "Condition error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    result = false;
                }
            }
            return result;
        }

        /// <summary>
        /// Sends a connection success message 
        /// </summary>
        /// <param name="service">IOrganizationService generated</param>
        /// <param name="parameters">Lsit of parameter</param>
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
                entityNode = new EntityNode(GetClosestEntityNode(node));
            }
            if (entityNode == null)
            {
                return;
            }
            var entityName = entityNode.EntityName;
            if (form.NeedToLoadEntity(entityName))
            {
                if (!form.working)
                {
                    form.LoadEntityDetails(entityName, RefreshAttributes);
                }
                return;
            }
            var entities = FetchXmlBuilder.GetDisplayEntities();
            var attributes = FetchXmlBuilder.GetDisplayAttributes(entityName);
            foreach (var attribute in attributes)
            {
                AttributeItem.AddAttributeToComboBox(cmbAttribute, attribute, true);
            }
            // RefreshFill now that attributes are loaded
            ControlUtils.FillControl(collec, cmbAttribute);
            ControlUtils.FillControl(collec, cmbValue);
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
                        if (attribute.Metadata is EnumAttributeMetadata)
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
                var attributeType = ((AttributeItem) cmbAttribute.SelectedItem).Metadata.AttributeType;
                if (attributeType.HasValue)
                {
                    ConditionsByValueType(attributeType.Value);
                }
            }
            UpdateValueField();
        }
        public void ConditionsByValueType(AttributeTypeCode valueType)
        {
            cmbOperator.SelectedItem = null;
            cmbOperator.Items.Clear();
            cmbOperator.Items.Add(new OperatorItem(ConditionOperator.Equal));
            cmbOperator.Items.Add(new OperatorItem(ConditionOperator.NotEqual));
            cmbOperator.Items.Add(new OperatorItem(ConditionOperator.In));
            cmbOperator.Items.Add(new OperatorItem(ConditionOperator.NotIn));
            cmbOperator.Items.Add(new OperatorItem(ConditionOperator.Null));
            cmbOperator.Items.Add(new OperatorItem(ConditionOperator.NotNull));

            if (valueType != AttributeTypeCode.Boolean &&
                valueType != AttributeTypeCode.DateTime &&
                valueType != AttributeTypeCode.Integer &&
                valueType != AttributeTypeCode.State &&
                valueType != AttributeTypeCode.Status &&
                valueType != AttributeTypeCode.Picklist &&
                valueType != AttributeTypeCode.BigInt &&
                valueType != AttributeTypeCode.Decimal &&
                valueType != AttributeTypeCode.Double &&
                valueType != AttributeTypeCode.Money &&
                valueType != AttributeTypeCode.Money &&
                valueType != AttributeTypeCode.Lookup &&
                valueType != AttributeTypeCode.Customer &&
                valueType != AttributeTypeCode.Owner &&
                valueType != AttributeTypeCode.Uniqueidentifier)
            {
                cmbOperator.Items.Add(new OperatorItem(ConditionOperator.BeginsWith));
                cmbOperator.Items.Add(new OperatorItem(ConditionOperator.DoesNotBeginWith));
                cmbOperator.Items.Add(new OperatorItem(ConditionOperator.Contains));
                cmbOperator.Items.Add(new OperatorItem(ConditionOperator.DoesNotContain));
                cmbOperator.Items.Add(new OperatorItem(ConditionOperator.EndsWith));
                cmbOperator.Items.Add(new OperatorItem(ConditionOperator.DoesNotEndWith));

                cmbOperator.Items.Add(new OperatorItem(ConditionOperator.Like));
                cmbOperator.Items.Add(new OperatorItem(ConditionOperator.NotLike));
            }
            if (valueType == AttributeTypeCode.DateTime ||
                valueType == AttributeTypeCode.Integer ||
                valueType == AttributeTypeCode.State ||
                valueType == AttributeTypeCode.Status ||
                valueType == AttributeTypeCode.Picklist ||
                valueType == AttributeTypeCode.BigInt ||
                valueType == AttributeTypeCode.Decimal ||
                valueType == AttributeTypeCode.Double ||
                valueType == AttributeTypeCode.Money ||
                valueType == AttributeTypeCode.Lookup ||
                valueType == AttributeTypeCode.Customer ||
                valueType == AttributeTypeCode.Owner ||
                valueType == AttributeTypeCode.Uniqueidentifier)
            {
                cmbOperator.Items.Add(new OperatorItem(ConditionOperator.Between));
                cmbOperator.Items.Add(new OperatorItem(ConditionOperator.NotBetween));
                cmbOperator.Items.Add(new OperatorItem(ConditionOperator.GreaterThan));
                cmbOperator.Items.Add(new OperatorItem(ConditionOperator.GreaterEqual));
                cmbOperator.Items.Add(new OperatorItem(ConditionOperator.LessThan));
                cmbOperator.Items.Add(new OperatorItem(ConditionOperator.LessEqual));
            }
            switch (valueType)
            {
                case AttributeTypeCode.DateTime:
                    cmbOperator.Items.Add(new OperatorItem(ConditionOperator.InFiscalPeriod));
                    cmbOperator.Items.Add(new OperatorItem(ConditionOperator.InFiscalPeriodAndYear));
                    cmbOperator.Items.Add(new OperatorItem(ConditionOperator.InFiscalYear));
                    cmbOperator.Items.Add(new OperatorItem(ConditionOperator.InOrAfterFiscalPeriodAndYear));
                    cmbOperator.Items.Add(new OperatorItem(ConditionOperator.InOrBeforeFiscalPeriodAndYear));

                    cmbOperator.Items.Add(new OperatorItem(ConditionOperator.Last7Days));
                    cmbOperator.Items.Add(new OperatorItem(ConditionOperator.LastFiscalPeriod));
                    cmbOperator.Items.Add(new OperatorItem(ConditionOperator.LastFiscalYear));
                    cmbOperator.Items.Add(new OperatorItem(ConditionOperator.LastMonth));
                    cmbOperator.Items.Add(new OperatorItem(ConditionOperator.LastWeek));

                    cmbOperator.Items.Add(new OperatorItem(ConditionOperator.LastXDays));
                    cmbOperator.Items.Add(new OperatorItem(ConditionOperator.LastXFiscalPeriods));
                    cmbOperator.Items.Add(new OperatorItem(ConditionOperator.LastXFiscalYears));
                    cmbOperator.Items.Add(new OperatorItem(ConditionOperator.LastXHours));
                    cmbOperator.Items.Add(new OperatorItem(ConditionOperator.LastXMonths));
                    cmbOperator.Items.Add(new OperatorItem(ConditionOperator.LastXWeeks));
                    cmbOperator.Items.Add(new OperatorItem(ConditionOperator.LastXYears));
                    cmbOperator.Items.Add(new OperatorItem(ConditionOperator.LastYear));

                    cmbOperator.Items.Add(new OperatorItem(ConditionOperator.Next7Days));
                    cmbOperator.Items.Add(new OperatorItem(ConditionOperator.NextFiscalPeriod));
                    cmbOperator.Items.Add(new OperatorItem(ConditionOperator.NextFiscalYear));
                    cmbOperator.Items.Add(new OperatorItem(ConditionOperator.NextMonth));
                    cmbOperator.Items.Add(new OperatorItem(ConditionOperator.NextWeek));
                    cmbOperator.Items.Add(new OperatorItem(ConditionOperator.NextXDays));

                    cmbOperator.Items.Add(new OperatorItem(ConditionOperator.NextXFiscalPeriods));
                    cmbOperator.Items.Add(new OperatorItem(ConditionOperator.NextXFiscalYears));
                    cmbOperator.Items.Add(new OperatorItem(ConditionOperator.NextXHours));
                    cmbOperator.Items.Add(new OperatorItem(ConditionOperator.NextXMonths));
                    cmbOperator.Items.Add(new OperatorItem(ConditionOperator.NextXWeeks));
                    cmbOperator.Items.Add(new OperatorItem(ConditionOperator.NextXYears));
                    cmbOperator.Items.Add(new OperatorItem(ConditionOperator.NextYear));

                    cmbOperator.Items.Add(new OperatorItem(ConditionOperator.OlderThanXMonths));
                    cmbOperator.Items.Add(new OperatorItem(ConditionOperator.On));
                    cmbOperator.Items.Add(new OperatorItem(ConditionOperator.OnOrAfter));
                    cmbOperator.Items.Add(new OperatorItem(ConditionOperator.OnOrBefore));

                    cmbOperator.Items.Add(new OperatorItem(ConditionOperator.ThisFiscalPeriod));
                    cmbOperator.Items.Add(new OperatorItem(ConditionOperator.ThisFiscalYear));
                    cmbOperator.Items.Add(new OperatorItem(ConditionOperator.ThisMonth));
                    cmbOperator.Items.Add(new OperatorItem(ConditionOperator.ThisWeek));
                    cmbOperator.Items.Add(new OperatorItem(ConditionOperator.ThisYear));

                    cmbOperator.Items.Add(new OperatorItem(ConditionOperator.Today));
                    cmbOperator.Items.Add(new OperatorItem(ConditionOperator.Tomorrow));
                    cmbOperator.Items.Add(new OperatorItem(ConditionOperator.Yesterday));
                    cmbOperator.Items.Add(new OperatorItem(ConditionOperator.NotOn));

                    break;
                case AttributeTypeCode.Lookup:
                case AttributeTypeCode.Customer:
                case AttributeTypeCode.Owner:
                case AttributeTypeCode.Uniqueidentifier:
                    cmbOperator.Items.Add(new OperatorItem(ConditionOperator.EqualBusinessId));
                    cmbOperator.Items.Add(new OperatorItem(ConditionOperator.EqualUserId));
                    cmbOperator.Items.Add(new OperatorItem(ConditionOperator.Above));
                    cmbOperator.Items.Add(new OperatorItem(ConditionOperator.AboveOrEqual));
                    cmbOperator.Items.Add(new OperatorItem(ConditionOperator.Under));
                    cmbOperator.Items.Add(new OperatorItem(ConditionOperator.UnderOrEqual));
                    cmbOperator.Items.Add(new OperatorItem(ConditionOperator.NotUnder));
                    break;
            }
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
