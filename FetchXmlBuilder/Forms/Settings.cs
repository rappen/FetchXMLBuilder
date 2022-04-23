using Cinteros.Xrm.FetchXmlBuilder.AppCode;
using System;
using System.Windows.Forms;

namespace Cinteros.Xrm.FetchXmlBuilder.Forms
{
    public partial class Settings : Form
    {
        private FetchXmlBuilder fxb;
        bool validateinfo;

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
            chkAddConditionToFilter.Checked = settings.AddConditionToFilter;
            chkUseSQL4CDS.Checked = settings.UseSQL4CDS;
            chkUseLookup.Checked = settings.UseLookup;
            chkShowHelp.Checked = settings.ShowHelpLinks;
            chkShowNodeTypes.Checked = settings.ShowNodeType;
            chkShowButtonTexts.Checked = settings.ShowButtonTexts;
            chkShowValidation.Checked = settings.ShowValidation;
            chkShowValidationInfo.Checked = settings.ShowValidationInfo;
            chkShowRepository.Checked = settings.ShowRepository;
            chkShowAllAttributes.Checked = settings.QueryOptions.ShowAllAttributes;
            cmbResult.SelectedIndex = SettingResultToComboBoxItem(settings.Results.ResultOutput);
            chkResAllPages.Checked = settings.Results.RetrieveAllPages;
            chkClickableLinks.Checked = settings.Results.ClickableLinks;
            propXmlColors.SelectedObject = settings.XmlColors;
            txtFetch.ConfigureForXml(settings);
            txtFetch.FormatXML(settings.QueryOptions.NewQueryTemplate, settings);
            chkTryMetadataCache.Checked = settings.TryMetadataCache;
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
            settings.QueryOptions.UseSingleQuotation = chkAppSingle.Checked;
            settings.QueryOptions.NewQueryTemplate = txtFetch.Text;
            settings.DoNotPromptToSave = chkAppNoSavePrompt.Checked;
            settings.Results.AlwaysNewWindow = chkAppResultsNewWindow.Checked;
            settings.Results.ResultOutput = ResultItemToSettingResult(cmbResult.SelectedIndex);
            settings.Results.RetrieveAllPages = chkResAllPages.Checked;
            settings.Results.ClickableLinks = chkClickableLinks.Checked;
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
            settings.QueryOptions.ShowAllAttributes = chkShowAllAttributes.Checked;
            settings.XmlColors = propXmlColors.SelectedObject as XmlColors;
            settings.TryMetadataCache = chkTryMetadataCache.Checked;
            settings.WaitUntilMetadataLoaded = chkWaitUntilMetadataLoaded.Checked;
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
    }
}