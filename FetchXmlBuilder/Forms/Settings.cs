using Rappen.XTB.FetchXmlBuilder.AppCode;
using Rappen.XTB.FetchXmlBuilder.Settings;
using System;
using System.Windows.Forms;

namespace Rappen.XTB.FetchXmlBuilder.Forms
{
    public partial class Settings : Form
    {
        private FetchXmlBuilder fxb;
        private bool validateinfo;
        internal bool forcereloadingmetadata = false;

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
            chkAppFriendlyResults.Checked = settings.Results.Friendly;
            chkAppSingle.Checked = settings.QueryOptions.UseSingleQuotation;
            chkAppNoSavePrompt.Checked = settings.DoNotPromptToSave;
            chkAppResultsNewWindow.Checked = settings.Results.AlwaysNewWindow;
            chkAppAllowUncustViews.Checked = settings.OpenUncustomizableViews;
            chkAddConditionToFilter.Checked = settings.AddConditionToFilter;
            chkUseSQL4CDS.Checked = settings.UseSQL4CDS;
            chkUseLookup.Checked = settings.UseLookup;
            chkShowHelp.Checked = settings.ShowHelpLinks;
            chkShowNodeTypes.Checked = settings.ShowNodeType;
            chkShowButtonTexts.Checked = settings.ShowButtonTexts;
            chkShowValidation.Checked = settings.ShowValidation;
            chkShowValidationInfo.Checked = settings.ShowValidationInfo;
            chkShowRepository.Checked = settings.ShowRepository;
            chkShowBulkDataUpdater.Checked = settings.ShowBDU;
            chkShowAllAttributes.Checked = settings.QueryOptions.ShowAllAttributes;
            chkShowOData2.Checked = settings.ShowOData2;
            cmbResult.SelectedIndex = SettingResultToComboBoxItem(settings.Results.ResultOutput);
            chkResAllPages.Checked = settings.Results.RetrieveAllPages;
            chkClickableLinks.Checked = settings.Results.ClickableLinks;
            numMaxColumnWidth.Value = settings.Results.MaxColumnWidth;
            chkWorkWithLayout.Checked = settings.Results.WorkWithLayout;
            propXmlColors.SelectedObject = settings.XmlColors;
            txtFetch.ConfigureForXml(settings);
            txtFetch.FormatXML(settings.QueryOptions.NewQueryTemplate, settings);
            chkTryMetadataCache.Checked = settings.TryMetadataCache;
            chkAlwaysShowAggregateProperties.Checked = settings.AlwaysShowAggregationProperties;
            if (chkTryMetadataCache.Checked)
            {
                chkWaitUntilMetadataLoaded.Enabled = true;
                chkWaitUntilMetadataLoaded.Checked = settings.WaitUntilMetadataLoaded;
            }
            else
            {
                chkWaitUntilMetadataLoaded.Enabled = false;
                chkWaitUntilMetadataLoaded.Checked = false;
            }
        }

        private int SettingResultToComboBoxItem(ResultOutput resultOutput)
        {
            switch (resultOutput)
            {
                case ResultOutput.XML: return 1;
                case ResultOutput.JSON: return 2;
                case ResultOutput.JSONWebAPI: return 3;
                case ResultOutput.Raw: return 4;
                default: return 0;
            }
        }

        internal FXBSettings GetSettings()
        {
            var settings = fxb.settings;
            settings.UseFriendlyNames = chkAppFriendly.Checked;
            settings.Results.Friendly = chkAppFriendlyResults.Checked;
            settings.QueryOptions.UseSingleQuotation = chkAppSingle.Checked;
            settings.QueryOptions.NewQueryTemplate = txtFetch.Text;
            settings.DoNotPromptToSave = chkAppNoSavePrompt.Checked;
            settings.Results.AlwaysNewWindow = chkAppResultsNewWindow.Checked;
            settings.Results.ResultOutput = ResultItemToSettingResult(cmbResult.SelectedIndex);
            settings.Results.RetrieveAllPages = chkResAllPages.Checked;
            settings.Results.ClickableLinks = chkClickableLinks.Checked;
            settings.Results.MaxColumnWidth = (int)numMaxColumnWidth.Value;
            settings.Results.WorkWithLayout = chkWorkWithLayout.Checked;
            settings.OpenUncustomizableViews = chkAppAllowUncustViews.Checked;
            settings.AddConditionToFilter = chkAddConditionToFilter.Checked;
            settings.UseSQL4CDS = chkUseSQL4CDS.Checked;
            settings.UseLookup = chkUseLookup.Checked;
            settings.ShowHelpLinks = chkShowHelp.Checked;
            settings.ShowNodeType = chkShowNodeTypes.Checked;
            settings.ShowButtonTexts = chkShowButtonTexts.Checked;
            settings.ShowValidation = chkShowValidation.Checked;
            settings.ShowValidationInfo = settings.ShowValidation && chkShowValidationInfo.Checked;
            settings.ShowRepository = chkShowRepository.Checked;
            settings.ShowBDU = chkShowBulkDataUpdater.Checked;
            settings.QueryOptions.ShowAllAttributes = chkShowAllAttributes.Checked;
            settings.ShowOData2 = chkShowOData2.Checked;
            settings.XmlColors = propXmlColors.SelectedObject as XmlColors;
            settings.TryMetadataCache = chkTryMetadataCache.Checked;
            settings.WaitUntilMetadataLoaded = chkWaitUntilMetadataLoaded.Checked;
            settings.AlwaysShowAggregationProperties = chkAlwaysShowAggregateProperties.Checked;
            return settings;
        }

        private ResultOutput ResultItemToSettingResult(int selectedIndex)
        {
            switch (selectedIndex)
            {
                case 1: return ResultOutput.XML;
                case 2: return ResultOutput.JSON;
                case 3: return ResultOutput.JSONWebAPI;
                case 4: return ResultOutput.Raw;
                default: return ResultOutput.Grid;
            }
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
                txtFetch.FormatXML(txtFetch.Text, fxb.settings);
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
                txtFetch.FormatXML(QueryOptions.DefaultNewQuery, fxb.settings);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                txtFetch.FormatXML(txtFetch.Text, fxb.settings);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "XML Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DialogResult = DialogResult.None;
            }
        }

        private void btnDefaultQuery_Click(object sender, EventArgs e)
        {
            txtFetch.FormatXML(QueryOptions.DefaultNewQuery, fxb.settings);
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

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
            fxb.ShowSelectSettings();
        }

        private void chkShowValidation_CheckedChanged(object sender, EventArgs e)
        {
            if (chkShowValidation.Checked)
            {
                chkShowValidationInfo.Checked = validateinfo;
            }
            else
            {
                validateinfo = chkShowValidationInfo.Checked;
                chkShowValidationInfo.Checked = false;
            }
            chkShowValidationInfo.Enabled = chkShowValidation.Checked;
        }

        private void chkTryMetadataCache_CheckedChanged(object sender, EventArgs e)
        {
            chkWaitUntilMetadataLoaded.Enabled = chkTryMetadataCache.Checked;
            if (!chkTryMetadataCache.Checked)
            {
                chkWaitUntilMetadataLoaded.Checked = false;
            }
        }

        private void cmbResult_SelectedIndexChanged(object sender, EventArgs e)
        {
            var resulttype = ResultItemToSettingResult(cmbResult.SelectedIndex);
            panResultView.Enabled = resulttype == ResultOutput.Grid;
            linkDeprecatedExecFetchReq.Visible = resulttype == ResultOutput.Raw;
        }

        private void linkLayout_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            FetchXmlBuilder.OpenURL("https://fetchxmlbuilder.com/features/layouts");
        }

        private void linkGeneral_Click(object sender, LinkLabelLinkClickedEventArgs e)
        {
            FetchXmlBuilder.HelpClick(tt.GetToolTip(sender as Control));
        }

        private void btnResetAll_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want it to get back default settings?\n\nYes or No...", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
            {
                PopulateSettings(new FXBSettings());
            }
        }

        private void btnForceReloadMetadata_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Shall we reload all metadata?\n\nYes or No...", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
            {
                forcereloadingmetadata = true;
                DialogResult = DialogResult.OK;
            }
        }
    }
}