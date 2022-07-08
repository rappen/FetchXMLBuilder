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
            var requestedType = "FetchXML";
            if (message.TargetArgument != null)
            {
                if (message.TargetArgument is FXBMessageBusArgument)
                {
                    var fxbArg = (FXBMessageBusArgument)message.TargetArgument;
                    fetchXml = fxbArg.FetchXML;
                    requestedType = fxbArg.Request.ToString();
                }
                else if (message.TargetArgument is string)
                {
                    fetchXml = (string)message.TargetArgument;
                }
            }
            dockControlBuilder.ParseXML(fetchXml, false);
            UpdateLiveXML();
            tsbReturnToCaller.ToolTipText = "Return " + requestedType + " to " + callerArgs.SourcePlugin;
            dockControlBuilder.RecordHistory("called from " + message.SourcePlugin);
            LogUse("CalledBy." + callerArgs.SourcePlugin);
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
            else if (e.KeyDown(Keys.L, false, true, false) && tsmiShowFetchXML.Enabled)
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
            LogUse("OpenAbout");
            var about = new About();
            about.StartPosition = FormStartPosition.CenterParent;
            about.lblVersion.Text = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            about.ShowDialog();
        }

        public void ShowSettings()
        {
            var settingDlg = new Forms.Settings(this);
            LogUse("OpenOptions");
            if (settingDlg.ShowDialog(this) == DialogResult.OK)
            {
                LogUse("SaveOptions");
                var oldtrycachemetadata = settings.TryMetadataCache;
                settings = settingDlg.GetSettings();
                if (Service != null && oldtrycachemetadata != settings.TryMetadataCache)
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
                        LoadEntities(ConnectionDetail);
                    }
                }
                SaveSetting();
                views = null;
                ApplySettings(false);
                dockControlBuilder.ApplyCurrentSettings();
                dockControlFetchXml?.ApplyCurrentSettings();
                dockControlGrid?.ApplySettingsToGrid();
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
            LogUse("ReturnTo." + callerArgs.SourcePlugin);
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