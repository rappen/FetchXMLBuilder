using Cinteros.Xrm.FetchXmlBuilder.AppCode;
using System;
using System.Windows.Forms;

namespace Cinteros.Xrm.FetchXmlBuilder.Forms
{
    public partial class Settings : Form
    {
        private FetchXmlBuilder fxb;

        public Settings(FetchXmlBuilder fxb)
        {
            InitializeComponent();
            this.fxb = fxb;
            PopulateSettings(fxb.settings);
        }

        private void PopulateSettings(FXBSettings settings)
        {
            if (settings == null)
            {
                settings = new FXBSettings();
            }
            chkAppFriendly.Checked = settings.UseFriendlyNames;
            chkAppSingle.Checked = settings.QueryOptions.UseSingleQuotation;
            chkAppNoSavePrompt.Checked = settings.DoNotPromptToSave;
            chkAppResultsNewWindow.Checked = settings.Results.AlwaysNewWindow;
            chkAppAllowUncustViews.Checked = settings.OpenUncustomizableViews;
            chkUseSQL4CDS.Checked = settings.UseSQL4CDS;
            chkUseLookup.Checked = settings.UseLookup;
            switch (settings.Results.ResultOption)
            {
                case 1: rbResSerialized.Checked = true; break;
                case 3: rbResRaw.Checked = true; break;
                default: rbResGrid.Checked = true; break;
            }
            cmbSeralizationStyle.SelectedIndex = settings.Results.SerializeStyle;
            chkResAllPages.Checked = settings.Results.RetrieveAllPages;
            propXmlColors.SelectedObject = settings.XmlColors;
            settings.XmlColors.ApplyToControl(txtFetch);
            txtFetch.Text = settings.QueryOptions.NewQueryTemplate;
            txtFetch.Process();
            chkEntAll.Checked = settings.Entity.All;
            if (!settings.Entity.All)
            {
                chkEntCustomizable.Checked = settings.Entity.Customizable;
                chkEntCustom.Checked = settings.Entity.Custom;
                chkEntIntersect.Checked = settings.Entity.Intersect;
                chkEntManaged.Checked = settings.Entity.Managed;
                chkEntOnlyAF.Checked = settings.Entity.OnlyValidAF;
                chkEntStandard.Checked = settings.Entity.Standard;
                chkEntUncustomizable.Checked = settings.Entity.Uncustomizable;
                chkEntUnmanaged.Checked = settings.Entity.Unmanaged;
            }
            chkAttAll.Checked = settings.Attribute.All;
            if (!settings.Attribute.All)
            {
                chkAttCustomizable.Checked = settings.Attribute.Customizable;
                chkAttCustom.Checked = settings.Attribute.Custom;
                chkAttManaged.Checked = settings.Attribute.Managed;
                chkAttOnlyAF.Checked = settings.Attribute.OnlyValidAF;
                chkAttOnlyRead.Checked = settings.Attribute.OnlyValidRead;
                chkAttStandard.Checked = settings.Attribute.Standard;
                chkAttUncustomizable.Checked = settings.Attribute.Uncustomizable;
                chkAttUnmanaged.Checked = settings.Attribute.Unmanaged;
            }
        }

        internal FXBSettings GetSettings()
        {
            var settings = fxb.settings;
            settings.UseFriendlyNames = chkAppFriendly.Checked;
            settings.QueryOptions.UseSingleQuotation = chkAppSingle.Checked;
            settings.QueryOptions.NewQueryTemplate = txtFetch.Text;
            settings.DoNotPromptToSave = chkAppNoSavePrompt.Checked;
            settings.Results.AlwaysNewWindow = chkAppResultsNewWindow.Checked;
            settings.Results.ResultOption = rbResSerialized.Checked ? 1 : rbResRaw.Checked ? 3 : 0;
            settings.Results.SerializeStyle = cmbSeralizationStyle.SelectedIndex;
            settings.Results.RetrieveAllPages = chkResAllPages.Checked;
            settings.OpenUncustomizableViews = chkAppAllowUncustViews.Checked;
            settings.UseSQL4CDS = chkUseSQL4CDS.Checked;
            settings.UseLookup = chkUseLookup.Checked;
            settings.XmlColors = propXmlColors.SelectedObject as XmlColors;
            settings.Entity.All = chkEntAll.Checked;
            settings.Entity.Customizable = chkEntCustomizable.Checked;
            settings.Entity.Uncustomizable = chkEntUncustomizable.Checked;
            settings.Entity.Managed = chkEntManaged.Checked;
            settings.Entity.Unmanaged = chkEntUnmanaged.Checked;
            settings.Entity.Custom = chkEntCustom.Checked;
            settings.Entity.Standard = chkEntStandard.Checked;
            settings.Entity.Intersect = chkEntIntersect.Checked;
            settings.Entity.OnlyValidAF = chkEntOnlyAF.Checked;
            settings.Attribute.All = chkAttAll.Checked;
            settings.Attribute.Managed = chkAttManaged.Checked;
            settings.Attribute.Unmanaged = chkAttUnmanaged.Checked;
            settings.Attribute.Customizable = chkAttCustomizable.Checked;
            settings.Attribute.Uncustomizable = chkAttUncustomizable.Checked;
            settings.Attribute.Custom = chkAttCustom.Checked;
            settings.Attribute.Standard = chkAttStandard.Checked;
            settings.Attribute.OnlyValidAF = chkAttOnlyAF.Checked;
            settings.Attribute.OnlyValidRead = chkAttOnlyRead.Checked;
            return settings;
        }

        private void chkEntAll_CheckedChanged(object sender, EventArgs e)
        {
            if (chkEntAll.Checked)
            {
                chkEntManaged.Checked = true;
                chkEntUnmanaged.Checked = true;
                chkEntCustomizable.Checked = true;
                chkEntUncustomizable.Checked = true;
                chkEntCustom.Checked = true;
                chkEntStandard.Checked = true;
                chkEntIntersect.Checked = true;
                chkEntOnlyAF.Checked = false;
            }
            chkEntManaged.Enabled = !chkEntAll.Checked;
            chkEntUnmanaged.Enabled = !chkEntAll.Checked;
            chkEntCustomizable.Enabled = !chkEntAll.Checked;
            chkEntUncustomizable.Enabled = !chkEntAll.Checked;
            chkEntCustom.Enabled = !chkEntAll.Checked;
            chkEntStandard.Enabled = !chkEntAll.Checked;
            chkEntIntersect.Enabled = !chkEntAll.Checked;
            chkEntOnlyAF.Enabled = !chkEntAll.Checked;
        }

        private void chkAttAll_CheckedChanged(object sender, EventArgs e)
        {
            if (chkAttAll.Checked)
            {
                chkAttManaged.Checked = true;
                chkAttUnmanaged.Checked = true;
                chkAttCustomizable.Checked = true;
                chkAttUncustomizable.Checked = true;
                chkAttCustom.Checked = true;
                chkAttStandard.Checked = true;
                chkAttOnlyAF.Checked = false;
                chkAttOnlyRead.Checked = false;
            }
            chkAttManaged.Enabled = !chkAttAll.Checked;
            chkAttUnmanaged.Enabled = !chkAttAll.Checked;
            chkAttCustomizable.Enabled = !chkAttAll.Checked;
            chkAttUncustomizable.Enabled = !chkAttAll.Checked;
            chkAttCustom.Enabled = !chkAttAll.Checked;
            chkAttStandard.Enabled = !chkAttAll.Checked;
            chkAttOnlyAF.Enabled = !chkAttAll.Checked;
            chkAttOnlyRead.Enabled = !chkAttAll.Checked;
        }

        private void chkEntMgd_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkEntAll.Checked && !chkEntManaged.Checked && !chkEntUnmanaged.Checked)
            {   // Neither managed nor unmanaged is not such a good idea...
                chkEntUnmanaged.Checked = true;
            }
        }

        private void chkEntCust_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkEntAll.Checked && !chkEntCustomizable.Checked && !chkEntUncustomizable.Checked)
            {   // Neither customizable nor uncustomizable is not such a good idea...
                chkEntCustomizable.Checked = true;
            }
        }

        private void chkEntCustom_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkEntAll.Checked && !chkEntCustom.Checked && !chkEntStandard.Checked)
            {   // Neither custom nor standard is not such a good idea...
                chkEntCustom.Checked = true;
            }
        }

        private void chkAttMgd_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkAttAll.Checked && !chkAttManaged.Checked && !chkAttUnmanaged.Checked)
            {   // Neither managed nor unmanaged is not such a good idea...
                chkAttUnmanaged.Checked = true;
            }
        }

        private void chkAttCust_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkAttAll.Checked && !chkAttCustomizable.Checked && !chkAttUncustomizable.Checked)
            {   // Neither customizable nor uncustomizable is not such a good idea...
                chkAttCustomizable.Checked = true;
            }
        }

        private void chkAttCustom_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkAttAll.Checked && !chkAttCustom.Checked && !chkAttStandard.Checked)
            {   // Neither custom nor standard is not such a good idea...
                chkAttCustom.Checked = true;
            }
        }

        internal static object ResultOption2String(int outputtype, int outputstyle)
        {
            switch (outputtype)
            {
                case 0: return "View";
                case 1: return outputstyle == 1 ? "Basic" : outputstyle == 2 ? "JSON" : outputstyle == 3 ? "EntityCollection" : "Explicit";
                case 3: return "FetchResult";
            }
            return "unknown";
        }

        private void rbResSerialized_CheckedChanged(object sender, EventArgs e)
        {
            cmbSeralizationStyle.Enabled = rbResSerialized.Checked;
        }

        private void llShowWelcome_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            fxb.LogUse("ShowWelcome-Manual");
            Welcome.ShowWelcome(this);
        }

        private void btnFormatQuery_Click(object sender, EventArgs e)
        {
            try
            {
                txtFetch.Process();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "XML Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtFetch_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFetch.Text))
            {
                txtFetch.Text = QueryOptions.DefaultNewQuery;
                txtFetch.Process();
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                txtFetch.Process();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "XML Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DialogResult = DialogResult.None;
            }
        }

        private void btnDefaultQuery_Click(object sender, EventArgs e)
        {
            txtFetch.Text = QueryOptions.DefaultNewQuery;
            txtFetch.Process();
        }

        private void propXmlColors_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (propXmlColors.SelectedObject is XmlColors colors)
            {
                colors.ApplyToControl(txtFetch);
            }
        }

        private void btnResetXmlColors_Click(object sender, EventArgs e)
        {
            var colors = new XmlColors();
            propXmlColors.SelectedObject = colors;
            colors.ApplyToControl(txtFetch);
        }
    }
}