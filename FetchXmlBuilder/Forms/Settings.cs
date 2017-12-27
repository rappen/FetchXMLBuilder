using Cinteros.Xrm.FetchXmlBuilder.AppCode;
using System;
using System.Windows.Forms;

namespace Cinteros.Xrm.FetchXmlBuilder.Forms
{
    public partial class Settings : Form
    {
        private FXBSettings settings = null;

        public Settings(FXBSettings settings)
        {
            InitializeComponent();
            PopulateSettings(settings);
        }

        private void PopulateSettings(FXBSettings Settings)
        {
            settings = Settings;
            if (settings == null)
            {
                settings = new FXBSettings();
            }
            chkAppFriendly.Checked = settings.useFriendlyNames;
            chkAppQuick.Checked = settings.showQuickActions;
            chkAppSingle.Checked = settings.useSingleQuotation;
            chkAppNoSavePrompt.Checked = settings.doNotPromptToSave;
            chkAppResultsNewWindow.Checked = settings.resultsAlwaysNewWindow;
            switch (settings.resultOption)
            {
                case 1: rbResSerialized.Checked = true; break;
                case 3: rbResRaw.Checked = true; break;
                default: rbResGrid.Checked = true; break;
            }
            cmbSeralizationStyle.SelectedIndex = settings.resultSerializeStyle;
            chkResAllPages.Checked = settings.retrieveAllPages;
            chkEntAll.Checked = settings.showEntitiesAll;
            if (!settings.showEntitiesAll)
            {
                chkEntCustomizable.Checked = settings.showEntitiesCustomizable;
                chkEntCustom.Checked = settings.showEntitiesCustom;
                chkEntIntersect.Checked = settings.showEntitiesIntersect;
                chkEntManaged.Checked = settings.showEntitiesManaged;
                chkEntOnlyAF.Checked = settings.showEntitiesOnlyValidAF;
                chkEntStandard.Checked = settings.showEntitiesStandard;
                chkEntUncustomizable.Checked = settings.showEntitiesUncustomizable;
                chkEntUnmanaged.Checked = settings.showEntitiesUnmanaged;
            }
            chkAttAll.Checked = settings.showAttributesAll;
            if (!settings.showAttributesAll)
            {
                chkAttCustomizable.Checked = settings.showAttributesCustomizable;
                chkAttCustom.Checked = settings.showAttributesCustom;
                chkAttManaged.Checked = settings.showAttributesManaged;
                chkAttOnlyAF.Checked = settings.showAttributesOnlyValidAF;
                chkAttOnlyRead.Checked = settings.showAttributesOnlyValidRead;
                chkAttStandard.Checked = settings.showAttributesStandard;
                chkAttUncustomizable.Checked = settings.showAttributesUncustomizable;
                chkAttUnmanaged.Checked = settings.showAttributesUnmanaged;
            }
            chkStatAllow.Checked = settings.logUsage != false;
        }

        internal FXBSettings GetSettings()
        {
            settings.useFriendlyNames = chkAppFriendly.Checked;
            settings.showQuickActions = chkAppQuick.Checked;
            settings.useSingleQuotation = chkAppSingle.Checked;
            settings.doNotPromptToSave = chkAppNoSavePrompt.Checked;
            settings.resultsAlwaysNewWindow = chkAppResultsNewWindow.Checked;
            settings.resultOption = rbResSerialized.Checked ? 1 : rbResRaw.Checked ? 3 : 0;
            settings.resultSerializeStyle = cmbSeralizationStyle.SelectedIndex;
            settings.retrieveAllPages = chkResAllPages.Checked;
            settings.showEntitiesAll = chkEntAll.Checked;
            settings.showEntitiesCustomizable = chkEntCustomizable.Checked;
            settings.showEntitiesUncustomizable = chkEntUncustomizable.Checked;
            settings.showEntitiesManaged = chkEntManaged.Checked;
            settings.showEntitiesUnmanaged = chkEntUnmanaged.Checked;
            settings.showEntitiesCustom = chkEntCustom.Checked;
            settings.showEntitiesStandard = chkEntStandard.Checked;
            settings.showEntitiesIntersect = chkEntIntersect.Checked;
            settings.showEntitiesOnlyValidAF = chkEntOnlyAF.Checked;
            settings.showAttributesAll = chkAttAll.Checked;
            settings.showAttributesManaged = chkAttManaged.Checked;
            settings.showAttributesUnmanaged = chkAttUnmanaged.Checked;
            settings.showAttributesCustomizable = chkAttCustomizable.Checked;
            settings.showAttributesUncustomizable = chkAttUncustomizable.Checked;
            settings.showAttributesCustom = chkAttCustom.Checked;
            settings.showAttributesStandard = chkAttStandard.Checked;
            settings.showAttributesOnlyValidAF = chkAttOnlyAF.Checked;
            settings.showAttributesOnlyValidRead = chkAttOnlyRead.Checked;
            settings.logUsage = chkStatAllow.Checked;
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
                case 0: return "Grid";
                case 1: return outputstyle == 1 ? "Basic" : outputstyle == 2 ? "JSON" : outputstyle == 3 ? "EntityCollection" : "Explicit";
                case 3: return "FetchResult";
            }
            return "unknown";
        }

        private void chkStatAllow_CheckedChanged(object sender, EventArgs e)
        {
            if (Visible && chkStatAllow.Checked)
            {
                MessageBox.Show("Thank You!\n\nHappy fetching :)\n\n/Jonas", "Statistics", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void rbResSerialized_CheckedChanged(object sender, EventArgs e)
        {
            cmbSeralizationStyle.Enabled = rbResSerialized.Checked;
        }
    }
}
