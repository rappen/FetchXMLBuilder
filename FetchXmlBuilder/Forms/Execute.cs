using Rappen.XTB.FetchXmlBuilder.Settings;
using Rappen.XTB.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace Rappen.XTB.FetchXmlBuilder.Forms
{
    public partial class Execute : Form
    {
        private FetchXmlBuilder fxb;
        private bool confirmedoldbypass;
        private bool confirmedbypass;
        private bool init;

        public static DialogResult Show(FetchXmlBuilder fxb)
        {
            var form = new Execute
            {
                fxb = fxb,
            };
            form.init = true;
            form.SetExecuteOptions(fxb.settings.ExecuteOptions);
            form.ValidateOptions();
            form.init = false;
            if (fxb.settings.ExecuteOptions.FormWidth > 0) form.Width = fxb.settings.ExecuteOptions.FormWidth;
            if (fxb.settings.ExecuteOptions.FormHeight > 0) form.Height = fxb.settings.ExecuteOptions.FormHeight;
            if (fxb.CDSVersion < fxb.bypasspluginminversion)
            {
                form.gbBypassLogic.Enabled = false;
                form.gbOldBypassCustom.Enabled = false;
                form.chkBypassSync.Checked = false;
                form.chkBypassAsync.Checked = false;
                form.chkOldBypassCustom.Checked = false;
            }
            var result = form.ShowDialog();
            fxb.settings.ExecuteOptions = form.ExecuteOptions;
            return result;
        }

        private Execute()
        {
            InitializeComponent();
        }

        public ExecuteOptions ExecuteOptions =>
            new ExecuteOptions
            {
                FormHeight = Height,
                FormWidth = Width,
                AllPages = rbAllPages.Checked,
                BypassCustom = chkOldBypassCustom.Checked,
                BypassSync = chkBypassSync.Checked,
                BypassAsync = chkBypassAsync.Checked,
                BypassSteps = GetStepGuids(),
                ConfirmedBypass = confirmedbypass,
                ConfirmedOldBypass = confirmedoldbypass,
            };

        private void SetExecuteOptions(ExecuteOptions options)
        {
            rbAllPages.Checked = options.AllPages;
            rbPageByPage.Checked = !options.AllPages;
            chkOldBypassCustom.Checked = options.BypassCustom;
            chkBypassSync.Checked = options.BypassSync;
            chkBypassAsync.Checked = options.BypassAsync;
            txtBypassSteps.Text = string.Join(",\n\r", options.BypassSteps);
            confirmedbypass = options.ConfirmedBypass;
            confirmedoldbypass = options.ConfirmedOldBypass;
        }

        private List<Guid> GetStepGuids()
        {
            try
            {
                return txtBypassSteps.Text.Split(new[] { ',', '\n' }, StringSplitOptions.RemoveEmptyEntries).Select(Guid.Parse).Distinct().ToList();
            }
            catch
            {
                return new List<Guid>();
            }
        }

        private void ValidateOptions()
        {
            var error = string.Empty;
            if (chkBypassSync.Checked || chkBypassAsync.Checked || !string.IsNullOrWhiteSpace(txtBypassSteps.Text))
            {
                if (!init && !confirmedbypass && MessageBoxEx.Show(this,
                    "Make sure you know exactly what this checkbox means.\n" +
                    "Please read the docs - click the link first!\n\n" +
                    "Are you OK to continue?", "Bypass",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) != DialogResult.Yes)
                {
                    chkBypassSync.Checked = false;
                    chkBypassAsync.Checked = false;
                    txtBypassSteps.Text = string.Empty;
                }
                confirmedbypass = true;
            }
            if (!string.IsNullOrWhiteSpace(txtBypassSteps.Text))
            {
                var steps = txtBypassSteps.Text.Split(new[] { ',', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var step in steps)
                {
                    if (!Guid.TryParse(step, out _))
                    {
                        error = "Bypassing steps must be a Guid, separated by comma.";
                    }
                }
            }
            if (chkOldBypassCustom.Checked)
            {
                if (!init && !confirmedoldbypass && MessageBoxEx.Show(this,
                    "This is the older feature, today we recomend to use BypassBusinessLogicExecution with the CustomSync value to get the same result.\n" +
                    "Please read the docs - click the link first!\n\n" +
                    "Are you OK to continue?", "Bypass Custom Business Logic",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) != DialogResult.Yes)
                {
                    chkOldBypassCustom.Checked = false;
                }
                confirmedoldbypass = true;
            }
            if (string.IsNullOrEmpty(error))
            {
                lblInfo.Text = $"Retrieving will start with Execute button!";
                lblInfo.ForeColor = System.Drawing.SystemColors.ControlText;
                btnExecute.Enabled = true;
            }
            else
            {
                lblInfo.Text = error;
                lblInfo.ForeColor = System.Drawing.Color.Red;
                btnExecute.Enabled = false;
            }
        }

        private void link_Click(object sender, LinkLabelLinkClickedEventArgs e)
        {
            UrlUtils.OpenUrl(sender, fxb.ConnectionDetail);
        }

        private void validate_Click(object sender, System.EventArgs e)
        {
            ValidateOptions();
        }
    }
}