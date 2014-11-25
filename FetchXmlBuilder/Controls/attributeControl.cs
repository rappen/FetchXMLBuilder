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

namespace Cinteros.Xrm.FetchXmlBuilder.Controls
{
    public partial class attributeControl : UserControl, IDefinitionSavable
    {
        private readonly Dictionary<string, string> collec;
        private string controlsCheckSum = "";
        TreeNode node;

        #region Delegates

        public delegate void SaveEventHandler(object sender, SaveEventArgs e);

        #endregion

        #region Event Handlers

        public event SaveEventHandler Saved;

        #endregion

        public attributeControl()
        {
            InitializeComponent();
            collec = new Dictionary<string, string>();
        }

        public attributeControl(TreeNode Node, AttributeMetadata[] attributes, FetchXmlBuilder fetchXmlBuilder)
            : this()
        {
            collec = (Dictionary<string, string>)Node.Tag;
            if (collec == null)
            {
                collec = new Dictionary<string, string>();
            }
            node = Node;
            PopulateControls(Node, attributes);
            ControlUtils.FillControls(collec, this.Controls);
            controlsCheckSum = ControlUtils.ControlsChecksum(this.Controls);
            Saved += fetchXmlBuilder.CtrlSaved;
        }

        private void PopulateControls(TreeNode node, AttributeMetadata[] attributes)
        {
            cmbAttribute.Items.Clear();
            if (attributes != null)
            {
                foreach (var attribute in attributes)
                {
                    AttributeItem.AddAttributeToComboBox(cmbAttribute, attribute, false);
                }
            }
            var aggregate = FetchXmlBuilder.IsFetchAggregate(node);
            cmbAggregate.Enabled = aggregate;
            chkGroupBy.Enabled = aggregate;
            if (!aggregate)
            {
                cmbAggregate.SelectedIndex = -1;
                chkGroupBy.Checked = false;
            }
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
            if (FetchXmlBuilder.IsFetchAggregate(node))
            {
                if (string.IsNullOrWhiteSpace(txtAlias.Text))
                {
                    MessageBox.Show("Alias mus be specified in aggregate queries", "Condition error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                Save();
            }
        }

        private void chkGroupBy_CheckedChanged(object sender, EventArgs e)
        {
            cmbDateGrouping.Enabled = chkGroupBy.Checked;
            chkUserTZ.Enabled = chkGroupBy.Checked;
            if (!chkGroupBy.Checked)
            {
                cmbDateGrouping.SelectedIndex = -1;
                chkUserTZ.Checked = false;
            }
        }
    }
}
