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
                var valueType = oper.GetValueType();
                var value = txtValue.Text.Trim();
                if (valueType == AttributeTypeCode.ManagedProperty)
                {   // Type not defined by operator
                    if (cmbAttribute.SelectedItem != null && cmbAttribute.SelectedItem is AttributeItem)
                    {   // Get type from condition attribute
                        var attribute = (AttributeItem)cmbAttribute.SelectedItem;
                        valueType = attribute.Metadata.AttributeType;
                    }
                    else
                    {   // Default, cannot determine type
                        valueType = AttributeTypeCode.String;
                    }
                }
                var error = "";
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
            }
            else if (FetchXmlBuilder.entities != null && FetchXmlBuilder.entities.ContainsKey(entityName))
            {
                var attributes = FetchXmlBuilder.entities[entityName].Attributes;
                if (attributes != null)
                {
                    foreach (var attribute in attributes)
                    {
                        AttributeItem.AddAttributeToComboBox(cmbAttribute, attribute, true);
                    }
                }
            }
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
            if (cmbOperator.SelectedItem != null && cmbOperator.SelectedItem is OperatorItem)
            {
                var oper = (OperatorItem)cmbOperator.SelectedItem;
                var valueType = oper.GetValueType();
                if (valueType == AttributeTypeCode.ManagedProperty)
                {
                    if (cmbAttribute.SelectedItem != null && cmbAttribute.SelectedItem is AttributeItem)
                    {
                        var attribute = (AttributeItem)cmbAttribute.SelectedItem;
                        valueType = attribute.Metadata.AttributeType;
                    }
                }
                if (valueType == null)
                {
                    txtValue.Text = "";
                    txtValue.Enabled = false;
                }
                else
                {
                    txtValue.Enabled = true;
                }
                if (valueType == AttributeTypeCode.Lookup || valueType == AttributeTypeCode.Customer || valueType == AttributeTypeCode.Owner || valueType == AttributeTypeCode.Uniqueidentifier)
                {
                    btnGetGuid.Visible = true;
                }
            }
        }

        private void btnGetGuid_Click(object sender, EventArgs e)
        {
            txtValue.Text = Guid.NewGuid().ToString();
        }

        private void cmbAttribute_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateValueField();
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
