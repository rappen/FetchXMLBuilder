using Cinteros.Xrm.FetchXmlBuilder.AppCode;
using Cinteros.Xrm.FetchXmlBuilder.DockControls;
using Microsoft.Xrm.Sdk.Metadata;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Cinteros.Xrm.FetchXmlBuilder.Controls
{
    public partial class orderControl : FetchXmlElementControlBase
    {
        private bool friendly;
        private AttributeMetadata[] attributes;

        public orderControl() : this(null, null, null)
        {
        }

        public orderControl(TreeNode node, AttributeMetadata[] attributes, TreeBuilderControl tree)
        {
            friendly = FetchXmlBuilder.friendlyNames;
            this.attributes = attributes;

            InitializeComponent();
            InitializeFXB(null, null, tree, node);
            warningProvider.Icon = WarningIcon;
        }

        protected override void PopulateControls()
        {
            var aggregate = TreeBuilderControl.IsFetchAggregate(Node);
            if (!aggregate)
            {
                cmbAttribute.Items.Clear();
                if (attributes != null)
                {
                    foreach (var attribute in attributes)
                    {
                        AttributeItem.AddAttributeToComboBox(cmbAttribute, attribute, false, friendly);
                    }
                }
            }
            else
            {
                cmbAlias.Items.Clear();
                cmbAlias.Items.Add("");
                cmbAlias.Items.AddRange(GetAliases(Tree.tvFetch.Nodes[0]).ToArray());
            }
            cmbAttribute.Enabled = !aggregate;
            cmbAlias.Enabled = aggregate;
        }

        private List<string> GetAliases(TreeNode node)
        {
            var result = new List<string>();
            if (node.Name == "entity" || node.Name == "link-entity")
            {
                foreach (TreeNode child in node.Nodes)
                {
                    if (child.Name == "attribute")
                    {
                        var alias = TreeNodeHelper.GetAttributeFromNode(child, "alias");
                        if (!string.IsNullOrEmpty(alias))
                        {
                            result.Add(alias);
                        }
                    }
                }
            }
            foreach (TreeNode child in node.Nodes)
            {
                result.AddRange(GetAliases(child));
            }
            return result;
        }

        protected override bool ValidateControls(bool silent)
        {
            var valid = base.ValidateControls(silent);

            if (cmbAttribute.Enabled && string.IsNullOrWhiteSpace(cmbAttribute.Text))
            {
                if (!silent)
                {
                    errorProvider.SetError(cmbAttribute, "Attribute is required");
                }

                valid = false;
            }

            if (cmbAlias.Enabled && string.IsNullOrWhiteSpace(cmbAlias.Text))
            {
                if (!silent)
                {
                    errorProvider.SetError(cmbAlias, "Alias is required");
                }

                valid = false;
            }

            return valid;
        }

        private void cmbAttribute_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            errorProvider.SetError(cmbAttribute, null);
            warningProvider.SetError(cmbAttribute, null);

            if (cmbAttribute.Enabled)
            {
                if (string.IsNullOrWhiteSpace(cmbAttribute.Text))
                {
                    errorProvider.SetError(cmbAttribute, "Attribute is required");
                }
                else if (cmbAttribute.SelectedIndex == -1)
                {
                    warningProvider.SetError(cmbAttribute, "Attribute is not valid");
                }
            }
        }

        private void cmbAlias_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            errorProvider.SetError(cmbAlias, null);
            warningProvider.SetError(cmbAlias, null);

            if (cmbAlias.Enabled)
            {
                if (string.IsNullOrWhiteSpace(cmbAlias.Text))
                {
                    errorProvider.SetError(cmbAlias, "Alias is required");
                }
                else if (cmbAlias.SelectedIndex == -1)
                {
                    warningProvider.SetError(cmbAlias, "Alias is not valid");
                }
            }
        }
    }
}