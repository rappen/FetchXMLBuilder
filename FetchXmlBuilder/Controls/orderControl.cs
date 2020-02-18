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
    }
}