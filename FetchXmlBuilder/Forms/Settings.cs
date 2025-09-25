using Rappen.XTB.FetchXmlBuilder.AppCode;
using Rappen.XTB.FetchXmlBuilder.Settings;
using Rappen.XTB.FXB.Settings;
using System;
using System.Windows.Forms;
using System.Linq;
using Rappen.AI.WinForm;
using Rappen.XTB.Helpers;
using System.Collections.Generic;
using Rappen.XTB.FetchXmlBuilder.DockControls;
using XrmToolBox.Extensibility;
using System.IO;
using System.Diagnostics;

namespace Rappen.XTB.FetchXmlBuilder.Forms
{
    public partial class Settings : Form
    {
        private FetchXmlBuilder fxb;
        private bool validateinfo;
        internal bool forcereloadingmetadata = false;
        private List<AiSettings> aiproviders;

        public Settings(FetchXmlBuilder fxb, string tab)
        {
            InitializeComponent();
            this.fxb = fxb;
            cmbAiProvider.Items.Clear();
            cmbAiProvider.Items.Add("");
            cmbAiProvider.Items.AddRange(OnlineSettings.Instance.AiSupport.AiProviders.ToArray());
            PopulateSettings(fxb.settings);
            tabSettings.SelectedTab = tabSettings.TabPages
                .Cast<TabPage>()
                .FirstOrDefault(t => t.Name == tab)
                ?? tabSettings.TabPages
                .Cast<TabPage>()
                .FirstOrDefault(t => t.Text == tab)
                ?? tabAppearance;
        }

        private void PopulateSettings(FXBSettings settings)
        {
            if (settings == null)
            {
                settings = new FXBSettings();
            }
            chkAppFriendly.Checked = settings.UseFriendlyNames;
            chkAppFriendlyResults.Checked = settings.Results.Friendly;
            chkAppBothNamesResults.Checked = settings.Results.BothNames;
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
            chkShowTreeviewAttributeTypes.Checked = settings.ShowTreeviewAttributeTypes;
            chkShowAttributeTypes.Checked = settings.ShowAttributeTypes;
            chkShowAllAttributes.Checked = settings.QueryOptions.ShowAllAttributes;
            chkShowOData2.Checked = settings.ShowOData2;
            cmbResult.SelectedIndex = SettingResultToComboBoxItem(settings.ExecuteOptions.ResultOutput);
            chkResAllPages.Checked = settings.ExecuteOptions.AllPages;
            chkClickableLinks.Checked = settings.Results.ClickableLinks;
            numMaxColumnWidth.Value = settings.Results.MaxColumnWidth;
            chkWorkWithLayout.Checked = settings.Layout.Enabled;
            chkLayoutUseFixedWidths.Checked = settings.Layout.UseFixedWidths;
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
            aiproviders = fxb.settings.AiProviders ?? new List<AiSettings>();
            UpdateAiSettingsList();
            if (OnlineSettings.Instance.AiSupport.Provider(settings.AiSettings.Provider) is AiProvider provider)
            {
                cmbAiProvider.SelectedItem = provider;
            }
            else
            {
                cmbAiProvider.SelectedIndex = -1;
            }
            cmbAiProvider_SelectedIndexChanged();
            txtAiApiKey.Text = settings.AiSettings.ApiKey;
            txtAiCallMe.Text = settings.AiSettings.MyName;
            chkAiLogConversation.Checked = settings.AiSettings.LogConversation;
            linkAiLogFolder.Text = Path.Combine(Paths.LogsPath, "FXB AI Chat");
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
            settings.Results.BothNames = chkAppBothNamesResults.Checked;
            settings.QueryOptions.UseSingleQuotation = chkAppSingle.Checked;
            settings.QueryOptions.NewQueryTemplate = txtFetch.Text;
            settings.DoNotPromptToSave = chkAppNoSavePrompt.Checked;
            settings.Results.AlwaysNewWindow = chkAppResultsNewWindow.Checked;
            settings.ExecuteOptions.ResultOutput = FetchXmlBuilder.ResultItemToSettingResult(cmbResult.SelectedIndex);
            settings.ExecuteOptions.AllPages = chkResAllPages.Checked;
            settings.Results.ClickableLinks = chkClickableLinks.Checked;
            settings.Results.MaxColumnWidth = (int)numMaxColumnWidth.Value;
            settings.Layout.Enabled = chkWorkWithLayout.Checked;
            settings.Layout.UseFixedWidths = chkLayoutUseFixedWidths.Checked;
            settings.OpenUncustomizableViews = chkAppAllowUncustViews.Checked;
            settings.AddConditionToFilter = chkAddConditionToFilter.Checked;
            settings.UseSQL4CDS = chkUseSQL4CDS.Checked;
            settings.UseLookup = chkUseLookup.Checked;
            settings.ShowHelpLinks = chkShowHelp.Checked;
            settings.ShowNodeType = chkShowNodeTypes.Checked;
            settings.ShowButtonTexts = chkShowButtonTexts.Checked;
            settings.ShowValidation = chkShowValidation.Checked;
            settings.ShowValidationInfo = settings.ShowValidation && chkShowValidationInfo.Checked;
            settings.ShowTreeviewAttributeTypes = chkShowTreeviewAttributeTypes.Checked;
            settings.ShowAttributeTypes = chkShowAttributeTypes.Checked;
            settings.QueryOptions.ShowAllAttributes = chkShowAllAttributes.Checked;
            settings.ShowOData2 = chkShowOData2.Checked;
            settings.XmlColors = propXmlColors.SelectedObject as XmlColors;
            settings.TryMetadataCache = chkTryMetadataCache.Checked;
            settings.WaitUntilMetadataLoaded = chkWaitUntilMetadataLoaded.Checked;
            settings.AlwaysShowAggregationProperties = chkAlwaysShowAggregateProperties.Checked;
            settings.AiSettings.Provider = cmbAiProvider.Text;
            settings.AiSettings.Model = cmbAiModel.Text;
            settings.AiSettings.ApiKey = txtAiApiKey.Enabled ? txtAiApiKey.Text : "";
            settings.AiSettings.MyName = txtAiCallMe.Text;
            settings.AiSettings.LogConversation = chkAiLogConversation.Checked;
            UpdateAiSettingsList();
            settings.AiProviders = aiproviders;
            return settings;
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
                MessageBoxEx.Show(this, ex.Message, "XML Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DialogResult = DialogResult.None;
                return;
            }
            if (cmbAiProvider.SelectedItem is AiProvider provider &&
                provider.Free &&
                string.IsNullOrEmpty(provider.ApiKeyDecrypted))
            {
                MessageBoxEx.Show(this, $"The selected Free AI provider '{provider.Name}' is not available right now. Please select another provider.", "AI Chat Settings", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                DialogResult = DialogResult.None;
                return;
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
            var resulttype = FetchXmlBuilder.ResultItemToSettingResult(cmbResult.SelectedIndex);
            panResultView.Enabled = resulttype == ResultOutput.Grid;
            linkDeprecatedExecFetchReq.Visible = resulttype == ResultOutput.Raw;
        }

        private void linkGeneral_Click(object sender, LinkLabelLinkClickedEventArgs e)
        {
            fxb.OpenUrl(tt.GetToolTip(sender as Control));
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

        private void LoadAiSettingsKey(AiProvider provider)
        {
            if (provider == null)
            {
                txtAiApiKey.Text = "";
            }
            else
            {
                var setting = aiproviders.FirstOrDefault(a => a.Provider == provider.Name);
                txtAiApiKey.Text = setting?.ApiKey;
                chkAiLogConversation.Checked = setting?.LogConversation ?? true;
            }
        }

        private void UpdateAiSettingsList()
        {
            if (aiproviders == null)
            {
                aiproviders = new List<AiSettings>();
            }
            if (cmbAiProvider.SelectedItem is AiProvider provider)
            {
                if (aiproviders.FirstOrDefault(a => a.Provider == provider.Name) is AiSettings existing)
                {
                    if (!provider.Free && !string.IsNullOrWhiteSpace(txtAiApiKey.Text))
                    {
                        existing.ApiKey = txtAiApiKey.Text;
                    }
                    existing.LogConversation = chkAiLogConversation.Checked;
                }
                else
                {
                    aiproviders.Add(new AiSettings
                    {
                        Provider = provider.Name,
                        ApiKey = !provider.Free ? txtAiApiKey.Text : "",
                        LogConversation = chkAiLogConversation.Checked,
                    });
                }
            }
        }

        private void HandlingFreeAI(AiProvider provider)
        {
            if (string.IsNullOrWhiteSpace(provider?.ApiKeyDecrypted))
            {
                txtAiApiKey.Text = "Unfortunately, this provider is currently not supported.";
            }
            else
            {
                if (AiChatControl.IsFreeAiUser(fxb))
                {
                    txtAiApiKey.Text = "API Key is handled by Jonas for this free provider.";
                }
                else
                {
                    txtAiApiKey.Text = "To use the free provider, you have to submit a form only for Jonas to make sure you are not using gazillian tokens. Click the (i) button on the provider above!";
                    picAiProvider.Tag = "FREE";
                    tt.SetToolTip(picAiProvider, $"Click to fill in the form to get free provider! at {OnlineSettings.Instance.AiSupport.UrlToUseForFree}");
                }
            }
        }

        private void cmbAiProvider_SelectedIndexChanged(object sender = null, EventArgs e = null)
        {
            cmbAiModel.Items.Clear();
            if (cmbAiProvider.SelectedItem is AiProvider provider)
            {
                tt.SetToolTip(picAiProvider, $"Read about {provider} at {provider.Url}");
                picAiProvider.Tag = provider.Url;
                if (provider.Free)
                {
                    HandlingFreeAI(provider);
                }
                else
                {
                    LoadAiSettingsKey(provider);
                }
                txtAiApiKey.Enabled = !provider.Free;
                cmbAiModel.Items.AddRange(provider.Models.ToArray());
                if (provider.Models.FirstOrDefault(m => m.Name == fxb.settings.AiSettings.Model) is AiModel model)
                {
                    cmbAiModel.SelectedItem = model;
                }
                else
                {
                    cmbAiModel.SelectedIndex = 0;
                }
            }
            else
            {
                picAiProvider.Tag = null;
                txtAiApiKey.Text = "";
                txtAiApiKey.Enabled = false;
            }
            cmbAiModel_SelectedIndexChanged();
        }

        private void cmbAiModel_SelectedIndexChanged(object sender = null, EventArgs e = null)
        {
            if (cmbAiProvider.SelectedItem is AiProvider provider && cmbAiModel.SelectedItem is AiModel model)
            {
                picAiUrl.Tag = OnlineSettings.Instance.AiSupport.Provider(cmbAiProvider.Text)?.Models.FirstOrDefault(m => m.Name == cmbAiModel.Text)?.Url;
                if (model.LogConversation != null)
                {
                    chkAiLogConversation.Checked = model.LogConversation.Value;
                    chkAiLogConversation.Enabled = false;
                    tt.SetToolTip(picAiLogConversation, $"The setting for logging conversations is determined by{Environment.NewLine}Provider: {provider}{Environment.NewLine}Model: {model}{Environment.NewLine}This cannot be changed manually.");
                }
                else
                {
                    chkAiLogConversation.Enabled = true;
                    tt.SetToolTip(picAiLogConversation, $"If checked, the conversation will be logged to Application Insights.");
                }
            }
            else
            {
                picAiUrl.Tag = null;
            }
            picAiProvider.Visible = picAiProvider.Tag != null;
            picAiUrl.Visible = picAiUrl.Tag != null;
        }

        private void picAiProvider_Click(object sender, EventArgs e)
        {
            if (sender is PictureBox pic && pic.Tag is string text && text == "FREE")
            {
                AiChatControl.PromptToUseForFree(fxb);
            }
            else
            {
                UrlUtils.OpenUrl(sender);
            }
        }

        private void chkWorkWithLayout_CheckedChanged(object sender, EventArgs e)
        {
            panLayout.Enabled = chkWorkWithLayout.Checked;
        }

        private void linkLogFolder_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("explorer.exe", linkAiLogFolder.Text);
        }

        private void picAiLog_Click(object sender, EventArgs e)
        {
            if (sender is PictureBox pic)
            {
                MessageBoxEx.Show(this, tt.GetToolTip(pic), "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}