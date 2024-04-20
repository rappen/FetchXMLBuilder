using Microsoft.Xrm.Sdk.Metadata;
using Rappen.XRM.Helpers.Extensions;
using Rappen.XRM.Helpers.FetchXML;
using Rappen.XTB.FetchXmlBuilder.DockControls;
using Rappen.XTB.FetchXmlBuilder.Views;
using Rappen.XTB.Helpers.ControlItems;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Rappen.XTB.FetchXmlBuilder.Controls
{
    public partial class entityControl : FetchXmlElementControlBase
    {
        private List<EntityMetadata> allentities;

        public entityControl() : this(new Dictionary<string, string>(), null, null)
        {
        }

        public entityControl(Dictionary<string, string> collection, FetchXmlBuilder fetchXmlBuilder, TreeBuilderControl tree)
        {
            InitializeComponent();
            chkIncludeLogicalName.Checked = fetchXmlBuilder.settings.UseFriendlyAndRawEntities;
            chkIncludeLogicalName.Enabled = fetchXmlBuilder.settings.UseFriendlyNames;
            InitializeFXB(collection, fetchXmlBuilder, tree, null);
        }

        protected override void PopulateControls()
        {
            allentities = fxb.GetDisplayEntities();
            FilterEntities();
        }

        protected override ControlValidationResult ValidateControl(Control control)
        {
            if (control == cmbEntity)
            {
                if (string.IsNullOrWhiteSpace(cmbEntity.Text))
                {
                    return new ControlValidationResult(ControlValidationLevel.Error, "Entity", ControlValidationMessage.IsRequired);
                }

                if (!(cmbEntity.SelectedItem is EntityMetadataItem) && fxb.entities != null)
                {
                    if (!fxb.entities.Any(e => e.LogicalName == cmbEntity.Text))
                    {
                        return new ControlValidationResult(ControlValidationLevel.Warning, "Entity", ControlValidationMessage.NotInMetadata);
                    }
                    if (!cmbEntity.Items.OfType<string>().Any(i => i == cmbEntity.Text))
                    {
                        return new ControlValidationResult(ControlValidationLevel.Info, "Entity", ControlValidationMessage.NotShowingNow);
                    }
                }
                if (fxb.entities != null && !cmbEntity.Items.OfType<EntityMetadataItem>().Any(i => i.ToString() == cmbEntity.Text))
                {
                    return new ControlValidationResult(ControlValidationLevel.Warning, "Entity", ControlValidationMessage.IsRequired);
                }
            }

            return base.ValidateControl(control);
        }

        private void cmbEntity_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            fxb.ShowMetadata(Metadata());
            if (IsInitialized &&
                cmbEntity.SelectedItem is EntityMetadataItem item &&
                item.Metadata?.LogicalName != fxb.dockControlBuilder.LayoutXML?.EntityMeta?.LogicalName)
            {
                fxb.dockControlBuilder.LayoutXML = new LayoutXML(item.Metadata, fxb);
            }
        }

        public override MetadataBase Metadata()
        {
            if (cmbEntity.SelectedItem is EntityMetadataItem item)
            {
                return item.Metadata;
            }
            return fxb.GetEntity(cmbEntity.Text) ?? base.Metadata();
        }

        public override void Focus()
        {
            cmbEntity.Focus();
        }

        private void chkIncludeLogicalName_CheckedChanged(object sender, System.EventArgs e)
        {
            if (fxb == null)
            {
                return;
            }
            fxb.settings.UseFriendlyAndRawEntities = chkIncludeLogicalName.Checked;
            SetIncLogName();
        }

        private void SetIncLogName()
        {
            SaveInternal(false);
            PopulateControls();
            ReFillControl(cmbEntity);
        }

        private void picFilter_Click(object sender, System.EventArgs e)
        {
            panFilter.Visible = !panFilter.Visible;
            FilterEntities();
        }

        private void txtFilter_TextChanged(object sender, System.EventArgs e)
        {
            FilterEntities();
        }

        private void FilterEntities()
        {
            cmbEntity.Items.Clear();
            var text = panFilter.Visible ? txtFilter.Text.ToLowerInvariant() : string.Empty;
            var entities = allentities?.Where(e =>
                string.IsNullOrWhiteSpace(text) ||
                e.LogicalName.ToLowerInvariant().Contains(text) ||
                e.ToDisplayName().ToLowerInvariant().Contains(text));
            cmbEntity.Items.AddRange(entities.Select(e => new EntityMetadataItem(e, fxb.settings.UseFriendlyNames, fxb.settings.UseFriendlyAndRawEntities)).ToArray());
            if (IsInitialized)
            {
                ReFillControl(cmbEntity);
            }
        }
    }
}