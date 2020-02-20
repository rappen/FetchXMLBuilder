using Cinteros.Xrm.FetchXmlBuilder.AppCode;
using Cinteros.Xrm.FetchXmlBuilder.DockControls;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Linq;
using System.Windows.Forms;

namespace Cinteros.Xrm.FetchXmlBuilder.Controls
{
    public partial class attributeControl : FetchXmlElementControlBase
    {
        private readonly AttributeMetadata[] attributes;
        private bool aggregate;

        public attributeControl() : this(null, null, null)
        {
        }

        public attributeControl(TreeNode node, AttributeMetadata[] attributes, TreeBuilderControl tree)
        {
            InitializeComponent();
            this.attributes = attributes;
            InitializeFXB(null, null, tree, node);
            warningProvider.Icon = WarningIcon;
        }

        protected override void PopulateControls()
        {
            cmbAttribute.Items.Clear();
            aggregate = TreeBuilderControl.IsFetchAggregate(Node);
            cmbAggregate.Enabled = aggregate;
            chkGroupBy.Enabled = aggregate;
            if (!aggregate)
            {
                cmbAggregate.SelectedIndex = -1;
                chkGroupBy.Checked = false;
            }

            if (attributes != null)
            {
                foreach (var attribute in attributes)
                {
                    AttributeItem.AddAttributeToComboBox(cmbAttribute, attribute, false, FetchXmlBuilder.friendlyNames);
                }
            }
        }

        protected override bool ValidateControls(bool silent)
        {
            return base.ValidateControls(silent) &&
                ValidateControl(cmbAttribute) &&
                ValidateControl(txtAlias);
        }

        protected override bool ValidateControl(Control control)
        {
            if (control == cmbAttribute)
            {
                var warning = cmbAttribute.Items.Count > 0 && !cmbAttribute.Items.Cast<AttributeItem>().Any(a => a.ToString() == cmbAttribute.Text);
                warningProvider.SetError(cmbAttribute, warning ? "Attribute is not valid" : null);
                var error = string.IsNullOrWhiteSpace(cmbAttribute.Text);
                errorProvider.SetError(cmbAttribute, error ? "Attribute is required" : null);
                return error;
            }
            else if (control == txtAlias)
            {
                var error = TreeBuilderControl.IsFetchAggregate(Node) && string.IsNullOrWhiteSpace(txtAlias.Text);
                errorProvider.SetError(txtAlias, error ? "Alias must be specified in aggregate queries" : null);
                return error;
            }
            return base.ValidateControl(control);
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
            chkDistinct.Enabled = aggregate && !chkGroupBy.Checked;
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

        private void input_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ValidateControl(sender as Control);
        }
    }
}