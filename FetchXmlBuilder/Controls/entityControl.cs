using Cinteros.Xrm.FetchXmlBuilder.AppCode;
using Cinteros.Xrm.FetchXmlBuilder.DockControls;
using System.Collections.Generic;

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
            warningProvider.Icon = WarningIcon;
        }

        protected override void PopulateControls()
        {
            cmbEntity.Items.Clear();
            var entities = fxb.GetDisplayEntities();
            if (entities != null)
            {
                foreach (var entity in entities)
                {
                    cmbEntity.Items.Add(new EntityItem(entity.Value));
                }
            }
        }

        protected override bool ValidateControls(bool silent)
        {
            var valid = base.ValidateControls(silent);

            if (string.IsNullOrWhiteSpace(cmbEntity.Text))
            {
                valid = false;

                if (!silent)
                {
                    errorProvider.SetError(cmbEntity, "Entity is requried");
                }
            }

            return valid;
        }

        private void cmbEntity_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            errorProvider.SetError(cmbEntity, null);
            warningProvider.SetError(cmbEntity, null);

            if (string.IsNullOrWhiteSpace(cmbEntity.Text))
            {
                errorProvider.SetError(cmbEntity, "Entity is required");
            }
            else if (cmbEntity.SelectedIndex == -1)
            {
                warningProvider.SetError(cmbEntity, "Entity is not valid");
            }
        }
    }
}