using Cinteros.Xrm.FetchXmlBuilder.DockControls;
using System.Collections.Generic;

namespace Cinteros.Xrm.FetchXmlBuilder.Controls
{
    public partial class filterControl : FetchXmlElementControlBase
    {
        public filterControl() : this(null, null)
        {
        }

        public filterControl(Dictionary<string, string> collection, TreeBuilderControl tree)
        {
            InitializeComponent();
            InitializeFXB(collection, null, tree, null);
        }

        protected override void PopulateControls()
        {
            base.PopulateControls();
            EnableQFControls();
        }

        private void EnableQFControls()
        {
            chkOverrideQFLimit.Enabled = chkIsQF.Checked;
            if (!chkOverrideQFLimit.Enabled && chkOverrideQFLimit.Checked)
            {
                chkOverrideQFLimit.Checked = false;
            }
        }

        private void chkIsQF_CheckedChanged(object sender, System.EventArgs e)
        {
            EnableQFControls();
        }
    }
}