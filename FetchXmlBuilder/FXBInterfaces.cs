using Rappen.XTB.FetchXmlBuilder.AppCode;
using Rappen.XTB.FetchXmlBuilder.Extensions;
using Rappen.XTB.FetchXmlBuilder.Forms;
using System;
using System.Reflection;
using System.Windows.Forms;
using XrmToolBox.Extensibility;
using XrmToolBox.Extensibility.Args;
using XrmToolBox.Extensibility.Interfaces;

namespace Rappen.XTB.FetchXmlBuilder
{
    public partial class FetchXmlBuilder : IGitHubPlugin, IPayPalPlugin, IMessageBusHost, IHelpPlugin, IStatusBarMessenger, IShortcutReceiver, IAboutPlugin, IDuplicatableTool, ISettingsPlugin
    {
        private MessageBusEventArgs callerArgs = null;
        private const string URLcaller = "URL Protocol";

        #region Public Events

        public event EventHandler<MessageBusEventArgs> OnOutgoingMessage;

        public event EventHandler<StatusBarMessageEventArgs> SendMessageToStatusBar;

        public event EventHandler<DuplicateToolArgs> DuplicateRequested;

        #endregion Public Events

        #region Public Properties

        public string DonationDescription => "FetchXML Builder Fan Club";

        public string EmailAccount => "jonas@rappen.net";

        public string HelpUrl => "https://fetchxmlbuilder.com?utm_source=XTBHelp";

        public string RepositoryName => "FetchXMLBuilder";

        public string UserName => "rappen";

        #endregion Public Properties

        #region Public Methods

        public void OnIncomingMessage(MessageBusEventArgs message)
        {
            if (inSql4Cds)
            {
                return;
            }

            callerArgs = message;
            var fetchXml = string.Empty;
            var layoutxml = string.Empty;
            var requestedType = "FetchXML";
            if (message.TargetArgument != null)
            {
                if (message.TargetArgument is FXBMessageBusArgument)
                {
                    var fxbArg = (FXBMessageBusArgument)message.TargetArgument;
                    fetchXml = fxbArg.FetchXML;
                    requestedType = fxbArg.Request.ToString();
                }
                else if (message.TargetArgument is string strArgument)
                {
                    if (strArgument.ToLowerInvariant().StartsWith("view:"))
                    {
                        if (!Guid.TryParse(strArgument.Split(':')[1], out Guid id))
                        {
                            MessageBox.Show($"Incorrect Guid: {strArgument}", "Loading Views", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        View = GetViewById(id);
                        fetchXml = View?.GetAttributeValue<string>("fetchxml");
                        layoutxml = View?.GetAttributeValue<string>("layoutxml");
                    }
                    else
                    {
                        fetchXml = strArgument;
                    }
                }
            }
            dockControlBuilder.Init(fetchXml, layoutxml, $"called from {message.SourcePlugin}", false);
            attributesChecksum = dockControlBuilder.GetAttributesSignature();
            tsbReturnToCaller.Text = callerArgs.SourcePlugin == URLcaller ? "FetchXML from an URL" : "Return FetchXML";
            tsbReturnToCaller.Image =
                callerArgs.SourcePlugin == "Bulk Data Updater" ? Cinteros.Xrm.FetchXmlBuilder.Properties.Resources.BDU_2019_032_tsp :
                callerArgs.SourcePlugin == URLcaller ? Cinteros.Xrm.FetchXmlBuilder.Properties.Resources.icon_web :
                Cinteros.Xrm.FetchXmlBuilder.Properties.Resources.icon_return;
            tsbReturnToCaller.ToolTipText = callerArgs.SourcePlugin == URLcaller ? "Show 'Sharing Queries' on my website." : "Return " + requestedType + " to " + callerArgs.SourcePlugin;
            LogUse("CalledBy." + callerArgs.SourcePlugin, ai2: true);
            if (callerArgs.SourcePlugin == "View Designer" && !connectionsettings.TipsAgainstOrViewDesignerToolShown)
            {
                if (MessageBox.Show("Did you know you can work with the layouts too in the FetchXML Builder?\nClick the Help button to see how!\n\nDon't show again.",
                    "Called from View Designer", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2, 0,
                    Utils.ProcessURL("https://jonasr.app/fxb-layout/")) == DialogResult.OK)
                {
                    connectionsettings.TipsAgainstOrViewDesignerToolShown = true;
                }
            }
            EnableControls(true);
        }

        public void ReceiveKeyDownShortcut(KeyEventArgs e)
        {
            if (e.KeyDown(Keys.F5, false, false, false) && tsbExecute.Enabled)
            {
                tsbExecute_Click(null, null);
            }
            else if (e.KeyDown(Keys.E, false, true, false) && tsmiShowFetchXML.Enabled)
            {
                tsmiShowFetchXML_Click(null, null);
            }
            else if (e.KeyDown(Keys.L, false, true, false) && tsmiShowLayoutXML.Enabled)
            {
                tsmiShowLayoutXML_Click(null, null);
            }
            else if (e.KeyDown(Keys.N, false, true, false) && tsbNew.Enabled)
            {
                tsbNew_Click(null, null);
            }
            else if (e.KeyDown(Keys.O, false, true, false) && tsmiOpenFile.Enabled)
            {
                tsmiOpenFile_Click(null, null);
            }
            else if (e.KeyDown(Keys.S, false, true, false) && tsmiSaveFile.Enabled)
            {
                tsmiSaveFile_Click(null, null);
            }
            else if (e.KeyDown(Keys.F12, false, false, false) && tsmiSaveFileAs.Enabled)
            {
                tsmiSaveFileAs_Click(null, null);
            }
            else if (e.KeyDown(Keys.Z, false, true, false) && tsbUndo.Enabled)
            {
                tsbUndo_Click(null, null);
            }
            else if (e.KeyDown(Keys.Y, false, true, false) && tsbRedo.Enabled)
            {
                tsbRedo_Click(null, null);
            }
            else if (e.KeyDown(Keys.F, false, true, false))
            {
                settings.UseFriendlyNames = !settings.UseFriendlyNames;
                dockControlBuilder.ApplyCurrentSettings();
            }
            else if (e.KeyDown(Keys.F, true, true, false))
            {
                settings.Results.Friendly = !settings.Results.Friendly;
                dockControlGrid?.ApplySettingsToGrid();
            }
            else if (e.KeyDown(Keys.B, true, true, false))
            {
                settings.Results.BothNames = !settings.Results.BothNames;
                dockControlGrid?.ApplySettingsToGrid();
            }
        }

        public void ReceiveKeyPressShortcut(KeyPressEventArgs e)
        {
            // Nothing to do
        }

        public void ReceiveKeyUpShortcut(KeyEventArgs e)
        {
            // Nothing to do
        }

        public void ReceivePreviewKeyDownShortcut(PreviewKeyDownEventArgs e)
        {
            // Nothing to do
        }

        public void ShowAboutDialog()
        {
            LogUse("ShowAbout");
            var about = new About();
            about.StartPosition = FormStartPosition.CenterParent;
            about.lblVersion.Text = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            about.ShowDialog();
        }

        public void ShowSettings()
        {
            var settingDlg = new Forms.Settings(this);
            LogUse("ShowOptions");
            if (settingDlg.ShowDialog(this) == DialogResult.OK)
            {
                LogUse("SaveOptions");
                var oldtrycachemetadata = settings.TryMetadataCache;
                settings = settingDlg.GetSettings();
                if (Service != null)
                {
                    if (oldtrycachemetadata != settings.TryMetadataCache)
                    {
                        var msg = settings.TryMetadataCache ?
                            "It is now trying to retrieve chached metadata in the background, and may take a few or many seconds." :
                            "Metadata is now reloaded in the old fashioned way.";
                        if (MessageBox.Show("The 'Use cache metadata' flag has been changed.\n\n" + msg + "\n\nClick Cancel to NOT change this.",
                            "Metadata changed", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.Cancel)
                        {
                            settings.TryMetadataCache = !settings.TryMetadataCache;
                        }
                        else
                        {
                            LoadEntities(true);
                        }
                    }
                    else if (settingDlg.forcereloadingmetadata)
                    {
                        if (MessageBox.Show("Reloading all metadata.\nIt may take a while... (10-300 secs)", "Reload metadata", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                        {
                            LoadEntities(true);
                        }
                    }
                }
                SaveSetting();
                views = null;
                ApplySettings(false);
                dockControlBuilder.ApplyCurrentSettings();
                dockControlFetchXml?.ApplyCurrentSettings();
                dockControlGrid?.ApplySettingsToGrid();
                if (dockControlLayoutXml?.Visible == true && !settings.Results.WorkWithLayout)
                {
                    dockControlLayoutXml.PanelPane?.CloseActiveContent();
                }
                EnableControls();
            }
        }

        #endregion Public Methods

        private bool CallerWantsResults()
        {
            return callerArgs != null;
        }

        private void ReturnToCaller()
        {
            if (callerArgs == null)
            {
                return;
            }
            LogUse("ReturnTo." + callerArgs.SourcePlugin, ai2: true);
            if (callerArgs.SourcePlugin == URLcaller)
            {
                OpenURL("https://fetchxmlbuilder.com/sharing-queries/");
                return;
            }
            var fetch = dockControlBuilder.GetFetchString(true, true);
            if (string.IsNullOrWhiteSpace(fetch))
            {
                return;
            }
            var message = new MessageBusEventArgs(callerArgs.SourcePlugin);
            if (callerArgs.TargetArgument is FXBMessageBusArgument)
            {
                var fxbArgs = (FXBMessageBusArgument)callerArgs.TargetArgument;
                switch (fxbArgs.Request)
                {
                    case FXBMessageBusRequest.FetchXML:
                        fxbArgs.FetchXML = fetch;
                        break;

                    case FXBMessageBusRequest.QueryExpression:
                        fxbArgs.QueryExpression = dockControlBuilder.GetQueryExpression();
                        break;

                    case FXBMessageBusRequest.OData:
                        fxbArgs.OData = GetOData(2);
                        break;
                }
                message.TargetArgument = fxbArgs;
            }
            else
            {
                message.TargetArgument = fetch;
            }
            OnOutgoingMessage(this, message);
        }
    }
}