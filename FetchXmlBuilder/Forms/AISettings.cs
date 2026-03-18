using Rappen.XTB.Helpers;
using System;
using System.Windows.Forms;

namespace Rappen.XTB.FetchXmlBuilder.Forms
{
    public partial class AISettings : Form
    {
        private Rappen.AI.WinForm.AiSettings edited;

        public AISettings()
        {
            InitializeComponent();
        }

        internal static bool ShowAiSettingsDialog(IWin32Window owner, Rappen.AI.WinForm.AiSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            // Work on a clone so Cancel doesn't mutate the caller's instance.
            var edited = new Rappen.AI.WinForm.AiSettings
            {
                Provider = settings.Provider,
                Model = settings.Model,
                Endpoint = settings.Endpoint,
                ApiKey = settings.ApiKey,
                MyName = settings.MyName,
                Calls = settings.Calls,
                LogConversation = settings.LogConversation,
                PreferDisplayName = settings.PreferDisplayName,
                SendWithEnter = settings.SendWithEnter,
                InstructionsFlavor = settings.InstructionsFlavor
            };

            using var dlg = new AISettings();
            dlg.edited = edited;
            dlg.LoadFrom(edited);

            var result = owner != null ? dlg.ShowDialog(owner) : dlg.ShowDialog();

            if (result != DialogResult.OK)
            {
                return false;
            }

            dlg.SaveTo(edited);

            // Commit back to original only on OK
            settings.Provider = edited.Provider;
            settings.Model = edited.Model;
            settings.Endpoint = edited.Endpoint;
            settings.ApiKey = edited.ApiKey;
            settings.MyName = edited.MyName;
            settings.Calls = edited.Calls;
            settings.LogConversation = edited.LogConversation;
            settings.PreferDisplayName = edited.PreferDisplayName;
            settings.SendWithEnter = edited.SendWithEnter;
            settings.InstructionsFlavor = edited.InstructionsFlavor;

            return true;
        }

        private void LoadFrom(Rappen.AI.WinForm.AiSettings settings)
        {
            if (settings == null)
            {
                return;
            }

            txtMyFlavor.Text = settings.InstructionsFlavor ?? string.Empty;
            txtAiCallMe.Text = settings.MyName ?? string.Empty;
            rbAiPreferDisplayName.Checked = settings.PreferDisplayName;
            rbAiPreferLogicalName.Checked = !settings.PreferDisplayName;
        }

        private void SaveTo(Rappen.AI.WinForm.AiSettings settings)
        {
            if (settings == null)
            {
                return;
            }

            settings.InstructionsFlavor = txtMyFlavor.Text?.Trim();
            settings.MyName = txtAiCallMe.Text?.Trim();
            settings.PreferDisplayName = rbAiPreferDisplayName.Checked;
        }

        private void any_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            UrlUtils.OpenUrl(sender);
        }

        private void AISettings_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (DialogResult != DialogResult.OK)
            {
                return;
            }

            // Optional: centralized validation (cancel close if invalid)
            if (txtAiCallMe.Text != null && txtAiCallMe.Text.Length > 100)
            {
                MessageBox.Show(this, "My name is too long.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.Cancel = true;
            }
        }
    }
}