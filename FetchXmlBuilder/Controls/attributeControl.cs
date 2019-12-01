using Cinteros.Xrm.FetchXmlBuilder.AppCode;
using Cinteros.Xrm.FetchXmlBuilder.DockControls;
using Cinteros.Xrm.XmlEditorUtils;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Cinteros.Xrm.FetchXmlBuilder.Controls
{
    public partial class attributeControl : UserControl, IDefinitionSavable
    {
        private readonly Dictionary<string, string> collec;
        private string controlsCheckSum = "";
        private TreeNode node;
        private bool aggregate;

        #region Delegates

        public delegate void SaveEventHandler(object sender, SaveEventArgs e);

        #endregion Delegates

        #region Event Handlers

        public event SaveEventHandler Saved;

        #endregion Event Handlers

        public attributeControl()
        {
            InitializeComponent();
            collec = new Dictionary<string, string>();
        }

        public attributeControl(TreeNode Node, AttributeMetadata[] attributes, TreeBuilderControl tree)
            : this()
        {
            collec = (Dictionary<string, string>)Node.Tag;
            if (collec == null)
            {
                collec = new Dictionary<string, string>();
            }
            node = Node;
            PopulateControls(Node, attributes);
            ControlUtils.FillControls(collec, this.Controls, this);
            controlsCheckSum = ControlUtils.ControlsChecksum(this.Controls);
            Saved += tree.CtrlSaved;
        }

        private void PopulateControls(TreeNode node, AttributeMetadata[] attributes)
        {
            cmbAttribute.Items.Clear();
            if (attributes != null)
            {
                foreach (var attribute in attributes)
                {
                    AttributeItem.AddAttributeToComboBox(cmbAttribute, attribute, false, FetchXmlBuilder.friendlyNames);
                }
            }
            aggregate = TreeBuilderControl.IsFetchAggregate(node);
            cmbAggregate.Enabled = aggregate;
            chkGroupBy.Enabled = aggregate;
            if (!aggregate)
            {
                cmbAggregate.SelectedIndex = -1;
                chkGroupBy.Checked = false;
            }
        }

        public void Save(bool silent)
        {
            try
            {
                if (ValidateForm(silent))
                {
                    Dictionary<string, string> collection = ControlUtils.GetAttributesCollection(this.Controls, true);
                    SendSaveMessage(collection);
                    controlsCheckSum = ControlUtils.ControlsChecksum(this.Controls);
                }
            }
            catch (ArgumentNullException ex)
            {
                if (!silent)
                {
                    MessageBox.Show(ex.Message, "Validation", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    Focus();
                }
            }
        }

        private bool ValidateForm(bool silent)
        {
            if (TreeBuilderControl.IsFetchAggregate(node))
            {
                if (string.IsNullOrWhiteSpace(txtAlias.Text))
                {
                    if (!silent)
                        MessageBox.Show("Alias must be specified in aggregate queries", "Condition error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //txtAlias.Focus();
                    return false;
                }
            }
            return true;
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
                Save(false);
            }
        }

        private void chkGroupBy_CheckedChanged(object sender, EventArgs e)
        {
            EnableAggregateControls();
        }

        private void cmbAggregate_SelectedIndexChanged(object sender, EventArgs e)
        {
            EnableAggregateControls();
        }

        private void EnableAggregateControls()
        {
            cmbDateGrouping.Enabled = chkGroupBy.Checked;
            chkDistinct.Enabled = aggregate && !chkGroupBy.Checked;// && cmbAggregate.Text == "countcolumn";
            if (!chkDistinct.Enabled)
            {
                chkDistinct.Checked = false;
            }
            chkUserTZ.Enabled = chkGroupBy.Checked;
            if (!chkGroupBy.Checked)
            {
                cmbDateGrouping.SelectedIndex = -1;
                chkUserTZ.Checked = false;
            }
        }
    }
}