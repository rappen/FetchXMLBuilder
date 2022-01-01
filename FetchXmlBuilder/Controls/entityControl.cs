using Cinteros.Xrm.FetchXmlBuilder.AppCode;
using Cinteros.Xrm.FetchXmlBuilder.DockControls;
using Microsoft.Xrm.Sdk.Metadata;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Cinteros.Xrm.FetchXmlBuilder.Controls
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
                cmbEntity.Items.AddRange(entities.Select(e => new EntityItem(e.Value)).ToArray());
            }
        }

        protected override ControlValidationResult ValidateControl(Control control)
        {
            if (control == cmbEntity)
            {
                if (string.IsNullOrWhiteSpace(cmbEntity.Text))
                {
                    return new ControlValidationResult(ControlValidationLevel.Error, "Entity is required");
                }

                if (fxb.Service != null && !cmbEntity.Items.OfType<EntityItem>().Any(i => i.ToString() == cmbEntity.Text))
                {
                    return new ControlValidationResult(ControlValidationLevel.Warning, "Entity is not valid");
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
            if (fxb.entities != null && fxb.entities.TryGetValue(cmbEntity.Text, out EntityMetadata meta))
            {
                return meta;
            }
            return base.Metadata();
        }

        public override void Focus()
        {
            cmbEntity.Focus();
        }
    }
}