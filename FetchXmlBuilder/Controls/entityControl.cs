using Microsoft.Xrm.Sdk.Metadata;
using Rappen.XTB.FetchXmlBuilder.ControlsClasses;
using Rappen.XTB.FetchXmlBuilder.DockControls;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Rappen.XTB.FetchXmlBuilder.Controls
{
    public partial class entityControl : FetchXmlElementControlBase
    {
        public entityControl() : this(new Dictionary<string, string>(), null, null)
        {
        }

        public entityControl(Dictionary<string, string> collection, FetchXmlBuilder fetchXmlBuilder, TreeBuilderControl tree)
        {
            InitializeComponent();
            InitializeFXB(collection, fetchXmlBuilder, tree, null);
        }

        protected override void PopulateControls()
        {
            cmbEntity.Items.Clear();
            var entities = fxb.GetDisplayEntities();
            if (entities != null)
            {
                cmbEntity.Items.AddRange(entities.Select(e => new EntityItem(e)).ToArray());
            }
        }

        protected override ControlValidationResult ValidateControl(Control control)
        {
            if (control == cmbEntity)
            {
                if (string.IsNullOrWhiteSpace(cmbEntity.Text))
                {
                    return new ControlValidationResult(ControlValidationLevel.Error, "Entity", ControlValidationMessage.IsRequired);
                }

                if (!(cmbEntity.SelectedItem is EntityItem) && fxb.entities != null)
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
                if (fxb.entities != null && !cmbEntity.Items.OfType<EntityItem>().Any(i => i.ToString() == cmbEntity.Text))
                {
                    return new ControlValidationResult(ControlValidationLevel.Warning, "Entity", ControlValidationMessage.IsRequired);
                }
            }

            return base.ValidateControl(control);
        }

        private void cmbEntity_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            fxb.ShowMetadata(Metadata());
        }

        public override MetadataBase Metadata()
        {
            if (cmbEntity.SelectedItem is EntityItem item)
            {
                return item.Meta;
            }
            return fxb.GetEntity(cmbEntity.Text) ?? base.Metadata();
        }

        public override void Focus()
        {
            cmbEntity.Focus();
        }
    }
}