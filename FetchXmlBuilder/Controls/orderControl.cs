using Microsoft.Xrm.Sdk.Metadata;
using Rappen.XRM.Helpers.FetchXML;
using Rappen.XTB.FetchXmlBuilder.Builder;
using Rappen.XTB.FetchXmlBuilder.ControlsClasses;
using Rappen.XTB.FetchXmlBuilder.DockControls;
using Rappen.XTB.Helpers.ControlItems;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Rappen.XTB.FetchXmlBuilder.Controls
{
    public partial class orderControl : FetchXmlElementControlBase
    {
        private AttributeMetadata[] attributes;
        private string entityName;

        public orderControl() : this(null, null, null, null)
        {
        }

        public orderControl(TreeNode node, AttributeMetadata[] attributes, FetchXmlBuilder fetchXmlBuilder, TreeBuilderControl tree)
        {
            InitializeComponent();
            BeginInit();
            this.attributes = attributes;
            InitializeFXB(null, fetchXmlBuilder, tree, node);
            EndInit();
            RefreshAttributes();
        }

        protected override void PopulateControls()
        {
            SetEntitiesAliases(cmbEntity, false);
            var aggregate = Node.IsFetchAggregate();
            cmbAttribute.Items.Clear();
            cmbAlias.Items.Clear();
            if (!aggregate)
            {
                cmbAttribute.Items.AddRange(attributes?.Select(a => new AttributeMetadataItem(a, fxb.settings.UseFriendlyNames, fxb.settings.ShowAttributeTypes)).ToArray());
            }
            else
            {
                cmbAlias.Items.Add("");
                cmbAlias.Items.AddRange(GetAliases(Tree.tvFetch.Nodes[0]).ToArray());
            }
            cmbAttribute.Enabled = !aggregate;
            cmbAlias.Enabled = aggregate;
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
            entityName = entityNode.EntityName;
            if (fxb.NeedToLoadEntity(entityName))
            {
                if (!fxb.working)
                {
                    fxb.LoadEntityDetails(entityName, null, false);
                }
                else
                {
                    return;
                }
            }
            BeginInit();
            var attributes = fxb.GetDisplayAttributes(entityName);
            cmbAttribute.Items.AddRange(attributes?.Select(a => new AttributeMetadataItem(a, fxb.settings.UseFriendlyNames, fxb.settings.ShowAttributeTypes)).ToArray());
            // RefreshFill now that attributes are loaded
            ReFillControl(cmbAttribute);
            EndInit();
        }

        private void SetEntitiesAliases(ComboBox cmb, bool needsalias)
        {
            cmb.Items.Clear();
            var closestEntity = Node.LocalEntityNode();
            if (closestEntity != null && closestEntity.Name == "entity")
            {
                cmb.Items.Add("");
                cmb.Items.AddRange(TreeNodeHelper.GetEntities(Tree.tvFetch.Nodes[0], needsalias).ToArray());
            }
            cmb.Enabled = cmb.Items.Count > 1;
            if (cmb.Parent is Panel pan)
            {
                pan.Visible = cmb.Enabled;
            }
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
                        var alias = child.Value("alias");
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

        protected override ControlValidationResult ValidateControl(Control control)
        {
            if (control == cmbAttribute && cmbAttribute.Enabled)
            {
                if (string.IsNullOrWhiteSpace(cmbAttribute.Text))
                {
                    return new ControlValidationResult(ControlValidationLevel.Error, "Attribute", ControlValidationMessage.IsRequired);
                }
                if (fxb.entities != null && !cmbAttribute.Items.OfType<AttributeMetadataItem>().Any(i => i.ToString() == cmbAttribute.Text))
                {
                    if (fxb.GetAttribute(entityName, cmbAttribute.Text) != null)
                    {
                        return new ControlValidationResult(ControlValidationLevel.Info, "Attribute", ControlValidationMessage.NotShowingNow);
                    }
                    return new ControlValidationResult(ControlValidationLevel.Warning, "Attribute", ControlValidationMessage.NotInMetadata);
                }
            }

            if (control == cmbAlias && cmbAlias.Enabled)
            {
                if (string.IsNullOrWhiteSpace(cmbAlias.Text))
                {
                    return new ControlValidationResult(ControlValidationLevel.Error, "Alias", ControlValidationMessage.IsRequired);
                }

                if (!cmbAlias.Items.OfType<string>().Any(i => i == cmbAlias.Text))
                {
                    return new ControlValidationResult(ControlValidationLevel.Warning, "Alias", ControlValidationMessage.InValid);
                }
            }

            return base.ValidateControl(control);
        }

        public override MetadataBase Metadata()
        {
            if (cmbAttribute.SelectedItem is AttributeMetadataItem item)
            {
                return item.Metadata;
            }
            return base.Metadata();
        }

        private void cmbAttribute_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            fxb.ShowMetadata(Metadata());
        }

        public override void Focus()
        {
            cmbAttribute.Focus();
        }

        private void cmbEntity_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            RefreshAttributes();
        }
    }
}