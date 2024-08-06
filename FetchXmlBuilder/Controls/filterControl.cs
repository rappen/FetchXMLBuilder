using Rappen.XTB.FetchXmlBuilder.DockControls;
using Rappen.XTB.Helpers;
using System.Collections.Generic;

namespace Rappen.XTB.FetchXmlBuilder.Controls
{
    public partial class filterControl : FetchXmlElementControlBase
    {
        public filterControl() : this(null, null, null)
        {
        }

        public filterControl(Dictionary<string, string> collection, FetchXmlBuilder fetchXmlBuilder, TreeBuilderControl tree)
        {
            InitializeComponent();
            InitializeFXB(collection, fetchXmlBuilder, tree, null);
        }

        protected override void PopulateControls()
        {
            base.PopulateControls();
            EnableQFControls();
        }

        private void EnableQFControls()
        {
            chkOverrideQFLimit.Enabled = chkIsQF.Checked;
            chkOverrideQFLimitBypass.Enabled = chkIsQF.Checked;
            if (!chkOverrideQFLimit.Enabled)
            {
                chkOverrideQFLimit.Checked = false;
            }
            if (!chkOverrideQFLimitBypass.Enabled)
            {
                chkOverrideQFLimitBypass.Checked = false;
            }
        }

        private void chkIsQF_CheckedChanged(object sender, System.EventArgs e)
        {
            EnableQFControls();
        }

        private void helpIcon_Click(object sender, System.EventArgs e)
        {
            UrlUtils.OpenUrl(sender);
        }
    }
}