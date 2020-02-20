using Cinteros.Xrm.FetchXmlBuilder.AppCode;
using Cinteros.Xrm.FetchXmlBuilder.DockControls;
using Cinteros.Xrm.XmlEditorUtils;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
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
            var valid = base.ValidateControls(silent);

            if (string.IsNullOrWhiteSpace(cmbAttribute.Text))
            {
                if (!silent)
                {
                    errorProvider.SetError(cmbAttribute, "Attribute is required");
                }

                valid = false;
            }
            else if (cmbAttribute.SelectedIndex == -1)
            {
                if (!silent)
                {
                    errorProvider.SetError(cmbAttribute, "Attribute is not valid");
                }

                valid = false;
            }

            if (TreeBuilderControl.IsFetchAggregate(Node) && string.IsNullOrWhiteSpace(txtAlias.Text))
            {
                if (!silent)
                {
                    errorProvider.SetError(txtAlias, "Alias must be specified in aggregate queries");
                }
                    
                valid = false;
            }

            return valid;
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

        private void cmbAttribute_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(cmbAttribute.Text))
            {
                errorProvider.SetError(cmbAttribute, "Attribute is required");
            }
            else if (cmbAttribute.SelectedIndex == -1)
            {
                errorProvider.SetError(cmbAttribute, "Attribute is not valid");
            }
            else
            {
                errorProvider.SetError(cmbAttribute, null);
            }
        }

        private void txtAlias_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (TreeBuilderControl.IsFetchAggregate(Node) && string.IsNullOrWhiteSpace(txtAlias.Text))
            {
                errorProvider.SetError(txtAlias, "Alias must be specified in aggregate queries");
            }
            else
            {
                errorProvider.SetError(txtAlias, null);
            }
        }
    }
}