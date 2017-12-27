using Cinteros.Xrm.CRMWinForm;
using Cinteros.Xrm.FetchXmlBuilder.AppCode;
using Cinteros.Xrm.FetchXmlBuilder.DockControls;
using Cinteros.Xrm.FetchXmlBuilder.Forms;
using Cinteros.Xrm.XmlEditorUtils;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Xml;
using WeifenLuo.WinFormsUI.Docking;
using XrmToolBox.Extensibility;
using XrmToolBox.Extensibility.Args;
using XrmToolBox.Extensibility.Interfaces;

namespace Cinteros.Xrm.FetchXmlBuilder
{
    public partial class FetchXmlBuilder : PluginControlBase, IGitHubPlugin, IPayPalPlugin, IMessageBusHost, IHelpPlugin, IStatusBarMessenger, IShortcutReceiver
    {
        #region Declarations
        internal TreeBuilderControl treeControl;
        internal ResultGrid resultGrid;

        internal static Dictionary<string, EntityMetadata> entities;
        internal static List<string> entityShitList = new List<string>(); // Oops, did I name that one??
        internal static Dictionary<string, List<Entity>> views;
        private string fileName;
        private Entity view;
        private Entity dynml;
        private string cwpfeed;
        internal bool working = false;
        internal FXBSettings currentSettings = new FXBSettings();
        internal static bool friendlyNames = false;
        private string attributesChecksum = "";
        internal bool buttonsEnabled = true;
        private XmlContentDisplayDialog xmlLiveUpdate;
        private string liveUpdateXml = "";
        private MessageBusEventArgs callerArgs = null;
        private int resultpanes = 0;

        internal string FileName
        {
            get { return fileName; }
            set
            {
                if (value != null)
                {
                    ResetSourcePointers();
                }
                fileName = value;
                if (!string.IsNullOrWhiteSpace(value))
                {
                    var file = System.IO.Path.GetFileName(value);
                    treeControl.SetFetchName($"FetchXML - File: {file}");
                    tsmiSaveFile.Text = $"Save File: {file}";
                }
                else
                {
                    treeControl.SetFetchName("FetchXML");
                    tsmiSaveFile.Text = "Save File";
                }
            }
        }
        internal Entity View
        {
            get { return view; }
            set
            {
                if (value != null)
                {
                    ResetSourcePointers();
                }
                view = value;
                if (view != null && view.Contains("name"))
                {
                    treeControl.SetFetchName($"FetchXML - View: {view["name"]}");
                    tsmiSaveView.Text = $"Save View: {view["name"]}";
                }
                else
                {
                    treeControl.SetFetchName("FetchXML");
                    tsmiSaveView.Text = "Save View";
                }
            }
        }
        internal Entity DynML
        {
            get { return dynml; }
            set
            {
                if (value != null)
                {
                    ResetSourcePointers();
                }
                dynml = value;
                if (dynml != null && dynml.Contains("listname"))
                {
                    tsmiSaveML.Text = "Save Marketing List: " + dynml["listname"];
                }
                else
                {
                    tsmiSaveML.Text = "Save Marketing List";
                }
            }
        }
        internal string CWPFeed
        {
            get { return cwpfeed; }
            set
            {
                if (value != null)
                {
                    ResetSourcePointers();
                }
                cwpfeed = value;
                if (!string.IsNullOrWhiteSpace(cwpfeed))
                {
                    tsmiSaveCWP.Text = "Save CWP Feed: " + cwpfeed;
                }
                else
                {
                    tsmiSaveCWP.Text = "Save as CWP Feed...";
                }
            }
        }

        #endregion Declarations

        public FetchXmlBuilder()
        {
            InitializeComponent();
            var theme = new VS2015LightTheme();
            dockContainer.Theme = theme;
            treeControl = new TreeBuilderControl(this);
        }

        private void SetupDockControls()
        {
            string dockFile = GetDockFileName();
            if (File.Exists(dockFile))
            {
                try
                {
                    dockContainer.LoadFromXml(dockFile, dockDeSerialization);
                }
                catch
                {
                    // Restore from file failed
                }
            }
            if (treeControl?.DockState != DockState.Document)
            {
                ResetDockLayout();
            }
        }

        private void ResetDockLayout()
        {
            treeControl.Show(dockContainer, DockState.Document);
        }

        private static string GetDockFileName()
        {
            return Path.Combine(Paths.SettingsPath, "Cinteros.Xrm.FetchXmlBuilder_DockPanels.xml");
        }

        private IDockContent dockDeSerialization(string persistString)
        {
            if (persistString == typeof(TreeBuilderControl).ToString())
            {
                return treeControl;
            }
            else if (persistString == typeof(ResultGrid).ToString() && resultGrid == null)
            {   // Only reopen default result grid
                resultGrid = new ResultGrid(null, this);
                return resultGrid;
            }
            return null;
        }

        private void SaveDockPanels()
        {
            var dockFile = GetDockFileName();
            dockContainer.SaveAsXml(dockFile);
        }

        public string RepositoryName
        {
            get { return "FetchXMLBuilder"; }
        }

        public string UserName
        {
            get { return "Cinteros"; }
        }

        public string DonationDescription
        {
            get { return "FetchXML Builder Fan Club"; }
        }

        public string EmailAccount
        {
            get { return "jonas@rappen.net"; }
        }

        public string HelpUrl
        {
            get { return "http://jonasrapp.cinteros.se/p/fxb.html?src=FXBhelp"; }
        }

        #region Event handlers

        public event EventHandler<MessageBusEventArgs> OnOutgoingMessage;
        public event EventHandler<StatusBarMessageEventArgs> SendMessageToStatusBar;

        public override void ClosingPlugin(PluginCloseInfo info)
        {
            if (!SaveIfChanged())
            {
                info.Cancel = true;
            }
            else
            {
                if (xmlLiveUpdate != null)
                {
                    xmlLiveUpdate.Close();
                    xmlLiveUpdate = null;
                }
                SaveSetting();
                SaveDockPanels();
                LogUse("Close");
            }
        }

        private void FetchXmlBuilder_Load(object sender, EventArgs e)
        {
            if (ParentForm != null)
            {
                ParentForm.LocationChanged += FetchXmlBuilder_FormChanged;
            }
            SetupDockControls();
            LoadSetting();
            LogUse("Load");
            TreeNodeHelper.AddContextMenu(null, treeControl);
            EnableControls(true);
        }

        private void FetchXmlBuilder_ConnectionUpdated(object sender, ConnectionUpdatedEventArgs e)
        {
            entities = null;
            entityShitList.Clear();
            View = null;
            views = null;
            var orgver = new Version(e.ConnectionDetail.OrganizationVersion);
            LogInfo("Connected CRM version: {0}", orgver);
            // Verifying version where MetadataChanges request exists https://msdn.microsoft.com/en-us/library/jj863599(v=crm.5).aspx
            // According to TechNet 2011 UR12 is 05.00.9690.3218 https://social.technet.microsoft.com/wiki/contents/articles/8062.crm-2011-build-and-version-numbers-for-update-rollups.aspx
            var orgok = orgver >= new Version(05, 00, 9690, 3218);
            if (orgok)
            {
                if (!working)
                {
                    LoadEntities();
                }
            }
            else
            {
                LogError("CRM version too old for FXB");
                MessageBox.Show($"RetrieveMetadataChangesRequest was introduced in\nMicrosoft Dynamics CRM 2011 UR12 (5.0.9690.3218)\nCurrent version is {orgver}\n\nPlease connect to a newer organization to use this cool tool.",
                    "Organization too old", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            EnableControls(orgok && buttonsEnabled);
        }

        public void OnIncomingMessage(MessageBusEventArgs message)
        {
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
            treeControl.ParseXML(fetchXml, false);
            tsbReturnToCaller.ToolTipText = "Return " + requestedType + " to " + callerArgs.SourcePlugin;
            treeControl.RecordHistory("called from " + message.SourcePlugin);
            LogUse("CalledBy." + callerArgs.SourcePlugin);
            EnableControls(true);
        }

        private void FetchXmlBuilder_FormChanged(object sender, EventArgs e)
        {
            if (tsmiLiveUpdate.Checked && xmlLiveUpdate != null)
            {
                AlignLiveXML();
            }
        }

        private void FetchXmlBuilder_Leave(object sender, EventArgs e)
        {
            if (tsmiLiveUpdate.Checked && xmlLiveUpdate != null)
            {
                xmlLiveUpdate.Visible = false;
            }
        }

        private void FetchXmlBuilder_Enter(object sender, EventArgs e)
        {
            if (tsmiLiveUpdate.Checked && xmlLiveUpdate != null)
            {
                xmlLiveUpdate.Visible = true;
            }
        }

        private void tsbCloseThisTab_Click(object sender, EventArgs e)
        {
            CloseTool();
        }

        private void tsbNew_Click(object sender, EventArgs e)
        {
            if (!SaveIfChanged())
            {
                return;
            }
            treeControl.Init(null, "new", false);
        }

        private void tsmiOpenFile_Click(object sender, EventArgs e)
        {
            OpenFile();
        }

        private void tsmiOpenView_Click(object sender, EventArgs e)
        {
            OpenView();
        }

        private void tsmiOpenML_Click(object sender, EventArgs e)
        {
            OpenML();
        }

        private void tsmiOpenCWP_Click(object sender, EventArgs e)
        {
            OpenCWPFeed();
        }

        private void tsbEdit_Click(object sender, EventArgs e)
        {
            var xml = treeControl.GetFetchString(false, false);
            var xcdDialog = XmlContentDisplayDialog.Show(xml, "FetchXML", true, true, Service != null, SaveFormat.XML, this);
            if (xcdDialog.DialogResult == DialogResult.OK)
            {
                XmlNode resultNode = xcdDialog.result;
                EnableControls(false);
                treeControl.Init(resultNode.OuterXml, "manual edit", !xcdDialog.execute);
                UpdateLiveXML();
                if (xcdDialog.execute)
                {
                    FetchResults(resultNode.OuterXml);
                }
            }
        }

        private void tsbExecute_Click(object sender, EventArgs e)
        {
            treeControl.Focus();
            FetchResults();
        }

        private void tsmiSaveFile_Click(object sender, EventArgs e)
        {
            SaveFetchXML(false, false);
        }

        private void tsmiSaveFileAs_Click(object sender, EventArgs e)
        {
            SaveFetchXML(true, false);
        }

        private void tsmiSaveView_Click(object sender, EventArgs e)
        {
            SaveView();
        }

        private void tsmiSaveML_Click(object sender, EventArgs e)
        {
            SaveML();
        }

        private void toolStripMain_Click(object sender, EventArgs e)
        {
            treeControl.Focus();
        }

        private void tsbAbout_Click(object sender, EventArgs e)
        {
            var about = new About();
            about.StartPosition = FormStartPosition.CenterParent;
            about.lblVersion.Text = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            about.ShowDialog();
        }

        private void tsmiSaveCWP_Click(object sender, EventArgs e)
        {
            SaveCWPFeed();
        }

        private void tsmiLiveUpdate_CheckedChanged(object sender, EventArgs e)
        {
            if (tsmiLiveUpdate.Checked)
            {
                LogUse("LiveXML");
                UpdateLiveXML();
            }
            else if (xmlLiveUpdate != null)
            {
                xmlLiveUpdate.Close();
                xmlLiveUpdate = null;
            }
        }

        void LiveXML_Disposed(object sender, EventArgs e)
        {
            tsmiLiveUpdate.Checked = false;
        }

        void LiveXML_KeyUp(object sender, KeyEventArgs e)
        {
            if (xmlLiveUpdate != null && xmlLiveUpdate.txtXML != null && xmlLiveUpdate.Visible && !e.Handled)
            {
                try
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(xmlLiveUpdate.txtXML.Text);
                    if (doc.OuterXml != liveUpdateXml)
                    {
                        treeControl.Init(xmlLiveUpdate.txtXML.Text, null, false);
                    }
                    liveUpdateXml = doc.OuterXml;
                }
                catch (Exception)
                {
                }
            }
        }

        private void tsmiShowOData_CheckedChanged(object sender, EventArgs e)
        {
            panOData.Visible = tsmiShowOData.Checked;
            if (tsmiShowOData.Checked)
            {
                LogUse("ShowOdata");
            }
            UpdateLiveXML();
        }

        private void tsmiToQureyExpression_Click(object sender, EventArgs e)
        {
            DisplayQExCode();
        }

        private void tsmiToSQLQuery_Click(object sender, EventArgs e)
        {
            DisplaySQLQuery();
        }

        private void tsbReturnToCaller_Click(object sender, EventArgs e)
        {
            ReturnToCaller();
        }

        private void tsbUndo_Click(object sender, EventArgs e)
        {
            treeControl.RestoreHistoryPosition(1);
        }

        private void tsbRedo_Click(object sender, EventArgs e)
        {
            treeControl.RestoreHistoryPosition(-1);
        }

        private void linkOData_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (e.Button == MouseButtons.Left && e.Link.Enabled)
            {
                System.Diagnostics.Process.Start(e.Link.LinkData as string);
                LogUse("ExecuteOData");
            }
        }

        private void menuODataExecute_Click(object sender, EventArgs e)
        {
            if (linkOData.Links.Count > 0 && linkOData.Links[0].Enabled)
            {
                System.Diagnostics.Process.Start(linkOData.Links[0].LinkData as string);
                LogUse("ExecuteOData");
            }
            else
            {
                MessageBox.Show("No link to execute");
            }
        }

        private void menuODataCopy_Click(object sender, EventArgs e)
        {
            if (linkOData.Links.Count > 0 && linkOData.Links[0].Enabled)
            {
                Clipboard.SetText(linkOData.Links[0].LinkData as string);
                LogUse("CopyOData");
            }
            else
            {
                MessageBox.Show("No link to copy");
            }
        }

        private void tsbOptions_Click(object sender, EventArgs e)
        {
            var allowStats = currentSettings.logUsage;
            var settingDlg = new Settings(currentSettings);
            if (settingDlg.ShowDialog(this) == DialogResult.OK)
            {
                currentSettings = settingDlg.GetSettings();
                if (allowStats != currentSettings.logUsage)
                {
                    if (currentSettings.logUsage == true)
                    {
                        LogUse("Accept", true);
                    }
                    else if (!currentSettings.logUsage == true)
                    {
                        LogUse("Deny", true);
                    }
                }
                treeControl.ApplyCurrentSettings();
            }
        }

        #endregion Event handlers

        #region Instance methods

        private void ResetSourcePointers()
        {
            FileName = null;
            CWPFeed = null;
            View = null;
            DynML = null;
        }

        /// <summary>Saves various configurations to file for next session</summary>
        private void SaveSetting()
        {
            currentSettings.fetchxml = treeControl.GetFetchString(false, false);
            SettingsManager.Instance.Save(typeof(FetchXmlBuilder), currentSettings, ConnectionDetail?.ConnectionName);
        }

        /// <summary>Loads configurations from file</summary>
        private void LoadSetting()
        {
            var nosettings = false;
            if (!SettingsManager.Instance.TryLoad<FXBSettings>(typeof(FetchXmlBuilder), out currentSettings, ConnectionDetail?.ConnectionName) &&
                !SettingsManager.Instance.TryLoad<FXBSettings>(typeof(FetchXmlBuilder), out currentSettings))
            {
                // Initialize new settings instance, no settings file was found
                currentSettings = new FXBSettings();
                nosettings = true;
            }
            ApplySettings();
            if (nosettings)
            {
                // Make sure initial default settings file is available
                SettingsManager.Instance.Save(typeof(FetchXmlBuilder), currentSettings);
            }
        }

        private void ApplySettings()
        {
            if (currentSettings != null && !string.IsNullOrWhiteSpace(currentSettings.fetchxml))
            {
                treeControl.Init(currentSettings.fetchxml, "loaded from last session", false);
            }
            treeControl.ShowQuickActions();
            var ass = Assembly.GetExecutingAssembly().GetName();
            var version = ass.Version.ToString();
            if (!version.Equals(currentSettings.currentVersion))
            {
                // Reset some settings when new version is deployed
                currentSettings.logUsage = null;
                currentSettings.currentVersion = version;
            }
            if (currentSettings.logUsage == null)
            {
                currentSettings.logUsage = LogUsage.PromptToLog();
            }
        }

        /// <summary>Enables or disables all buttons on the form</summary>
        /// <param name="enabled"></param>
        internal void EnableControls(bool enabled)
        {
            MethodInvoker mi = delegate
            {
                try
                {
                    tsbNew.Enabled = enabled;
                    tsbEdit.Enabled = enabled;
                    tsbOpen.Enabled = enabled;
                    tsmiOpenFile.Enabled = enabled;
                    tsmiOpenView.Enabled = enabled && Service != null;
                    tsmiOpenML.Enabled = enabled && Service != null;
                    tsmiOpenCWP.Visible = enabled && Service != null && entities != null && entities.ContainsKey("cint_feed");
                    tsbReturnToCaller.Visible = CallerWantsResults();
                    tsbSave.Enabled = enabled;
                    tsmiSaveFile.Enabled = enabled && treeControl.FetchChanged && !string.IsNullOrEmpty(FileName);
                    tsmiSaveFileAs.Enabled = enabled;
                    tsmiSaveView.Enabled = enabled && Service != null && View != null;
                    tsmiSaveML.Enabled = enabled && Service != null && DynML != null;
                    tsmiSaveCWP.Visible = enabled && Service != null && entities != null && entities.ContainsKey("cint_feed");
                    tsmiSaveCWP.Enabled = enabled && Service != null && treeControl.FetchChanged && !string.IsNullOrEmpty(CWPFeed);
                    tsmiToQureyExpression.Enabled = enabled && Service != null;
                    tsmiToSQLQuery.Enabled = enabled && Service != null;
                    tsmiToJavascript.Enabled = enabled && Service != null;
                    tsmiToCSharp.Enabled = enabled && Service != null;
                    tsbView.Enabled = enabled;
                    tsbExecute.Enabled = enabled && Service != null;
                    treeControl.EnableControls(enabled);
                    buttonsEnabled = enabled;
                }
                catch
                {
                    // Now what?
                }
            };
            if (InvokeRequired)
            {
                Invoke(mi);
            }
            else
            {
                mi();
            }
        }

        private bool CallerWantsResults()
        {
            return callerArgs != null;
        }

        /// <summary>Repopulate the entire tree from the xml document containing the FetchXML</summary>
        private List<string> GetEntitiesFromFetch(XmlNode definitionXmlNode, List<string> result = null)
        {
            if (result == null)
            {
                result = new List<string>();
            }
            if (definitionXmlNode.Name == "entity" || definitionXmlNode.Name == "link-entity")
            {
                var entity = definitionXmlNode.Attributes["name"] != null ? definitionXmlNode.Attributes["name"].Value : "";
                if (!string.IsNullOrEmpty(entity) && !result.Contains(entity))
                {
                    result.Add(entity);
                }
            }
            foreach (var child in definitionXmlNode.ChildNodes)
            {
                if (child is XmlNode)
                {
                    GetEntitiesFromFetch((XmlNode)child, result);
                }
            }
            return result;
        }

        /// <summary>Enables buttons relevant for currently selected node</summary>
        private bool SaveIfChanged()
        {
            var ok = true;
            if (!currentSettings.doNotPromptToSave && treeControl.FetchChanged)
            {
                var result = MessageBox.Show("FetchXML has changed.\nSave changes?", "Confirm", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (result == DialogResult.Cancel)
                {
                    ok = false;
                }
                if (result == DialogResult.Yes)
                {
                    if (!SaveFetchXML(false, true))
                    {
                        ok = false;
                    }
                }
            }
            return ok;
        }

        private bool SaveFetchXML(bool prompt, bool silent)
        {
            bool result = false;
            var newfile = "";
            if (prompt || string.IsNullOrEmpty(FileName))
            {
                var sfd = new SaveFileDialog
                {
                    Title = "Select a location to save the FetchXML",
                    Filter = "Xml file (*.xml)|*.xml",
                    FileName = System.IO.Path.GetFileName(FileName)
                };
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    newfile = sfd.FileName;
                }
            }
            if (!string.IsNullOrEmpty(newfile))
            {
                EnableControls(false);
                FileName = newfile;
                treeControl.Save(FileName);
                if (!silent)
                {
                    MessageBox.Show(this, "FetchXML saved!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                result = true;
                EnableControls(true);
            }
            return result;
        }

        internal bool NeedToLoadEntity(string entityName)
        {
            return
                !string.IsNullOrEmpty(entityName) &&
                !entityShitList.Contains(entityName) &&
                Service != null &&
                (entities == null ||
                 !entities.ContainsKey(entityName) ||
                 entities[entityName].Attributes == null);
        }

        private void LoadEntities()
        {
            working = true;
            entities = null;
            entityShitList = new List<string>();
            WorkAsync(new WorkAsyncInfo("Loading entities...",
                (eventargs) =>
                {
                    EnableControls(false);
                    eventargs.Result = MetadataHelper.LoadEntities(Service);
                })
            {
                PostWorkCallBack = (completedargs) =>
                {
                    if (completedargs.Error != null)
                    {
                        MessageBox.Show(completedargs.Error.Message);
                    }
                    else
                    {
                        if (completedargs.Result is RetrieveMetadataChangesResponse)
                        {
                            entities = new Dictionary<string, EntityMetadata>();
                            foreach (var entity in ((RetrieveMetadataChangesResponse)completedargs.Result).EntityMetadata)
                            {
                                entities.Add(entity.LogicalName, entity);
                            }
                        }
                    }
                    working = false;
                    EnableControls(true);
                }
            });
        }

        internal void LoadEntityDetails(string entityName, Action detailsLoaded, bool async = true)
        {
            if (detailsLoaded != null && !async)
            {
                throw new ArgumentException("Cannot handle call-back method for synchronous loading.", "detailsLoaded");
            }
            working = true;
            var name = GetEntityDisplayName(entityName);
            if (async)
            {
                WorkAsync(new WorkAsyncInfo($"Loading {name}...",
                    (eventargs) =>
                    {
                        eventargs.Result = MetadataHelper.LoadEntityDetails(Service, entityName);
                    })
                {
                    PostWorkCallBack = (completedargs) =>
                    {
                        LoadEntityDetailsCompleted(entityName, completedargs.Result as OrganizationResponse, completedargs.Error);
                        if (completedargs.Error == null && detailsLoaded != null)
                        {
                            detailsLoaded();
                        }
                    }
                });
            }
            else
            {
                try
                {
                    var resp = MetadataHelper.LoadEntityDetails(Service, entityName);
                    LoadEntityDetailsCompleted(entityName, resp, null);
                }
                catch (Exception e)
                {
                    LoadEntityDetailsCompleted(entityName, null, e);
                }
            }
        }

        private void LoadEntityDetailsCompleted(string entityName, OrganizationResponse Result, Exception Error)
        {
            if (Error != null)
            {
                entityShitList.Add(entityName);
                MessageBox.Show(Error.Message, "Load attribute metadata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                if (Result is RetrieveMetadataChangesResponse)
                {
                    var resp = (RetrieveMetadataChangesResponse)Result;
                    if (resp.EntityMetadata.Count == 1)
                    {
                        if (entities.ContainsKey(entityName))
                        {
                            entities[entityName] = resp.EntityMetadata[0];
                        }
                        else
                        {
                            entities.Add(entityName, resp.EntityMetadata[0]);
                        }
                    }
                    else
                    {
                        entityShitList.Add(entityName);
                        MessageBox.Show("Metadata not found for entity " + entityName, "Load attribute metadata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                working = false;
                treeControl.UpdateCurrentNode();
            }
            working = false;
        }

        private void FetchResults(string fetch = "")
        {
            if (!tsbExecute.Enabled)
            {
                return;
            }
            if (working)
            {
                MessageBox.Show("Busy doing something...\n\nPlease wait until current transaction is done.");
                return;
            }
            if (string.IsNullOrEmpty(fetch))
            {
                fetch = treeControl.GetFetchString(false, true);
            }
            if (string.IsNullOrEmpty(fetch))
            {
                return;
            }
            var fetchType = currentSettings.resultOption;
            switch (fetchType)
            {
                case 0:
                case 1:
                    RetrieveMultiple(fetch);
                    break;
                case 3:
                    ExecuteFetch(fetch);
                    break;
                default:
                    MessageBox.Show("Select valid output type under Options.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }

        private void ExecuteFetch(string fetch)
        {
            working = true;
            WorkAsync(new WorkAsyncInfo("Executing FetchXML...",
                (eventargs) =>
                {
                    //var fetchxml = GetFetchDocument().OuterXml;
                    var start = DateTime.Now;
                    var resp = (ExecuteFetchResponse)Service.Execute(new ExecuteFetchRequest() { FetchXml = fetch });
                    var stop = DateTime.Now;
                    var duration = stop - start;
                    SendMessageToStatusBar(this, new StatusBarMessageEventArgs($"Execution time: {duration}"));
                    eventargs.Result = resp.FetchXmlResult;
                })
            {
                PostWorkCallBack = (completedargs) =>
                {
                    working = false;
                    if (completedargs.Error != null)
                    {
                        MessageBox.Show(completedargs.Error.Message);
                    }
                    else if (completedargs.Result is string)
                    {
                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml(completedargs.Result.ToString());
                        XmlContentDisplayDialog.Show(doc.OuterXml, "FetchXML result", false, false, false, SaveFormat.XML, this);
                    }
                }
            });
            LogUse("ExecuteFetch");
        }

        private void RetrieveMultiple(string fetch)
        {
            working = true;
            var outputtype = currentSettings.resultOption;
            var outputstyle = currentSettings.resultSerializeStyle;
            var outputtypestring = Settings.ResultOption2String(outputtype, outputstyle);
            SendMessageToStatusBar(this, new StatusBarMessageEventArgs("Retrieving..."));
            WorkAsync(new WorkAsyncInfo("Executing FetchXML...",
                (eventargs) =>
                {
                    QueryBase query;
                    try
                    {
                        query = treeControl.GetQueryExpression();
                    }
                    catch (FetchIsAggregateException)
                    {
                        query = new FetchExpression(fetch);
                    }
                    var start = DateTime.Now;
                    EntityCollection resultCollection = null;
                    EntityCollection tmpResult = null;
                    var page = 0;
                    do
                    {
                        tmpResult = Service.RetrieveMultiple(query);
                        if (resultCollection == null)
                        {
                            resultCollection = tmpResult;
                        }
                        else
                        {
                            resultCollection.Entities.AddRange(tmpResult.Entities);
                            resultCollection.MoreRecords = tmpResult.MoreRecords;
                            resultCollection.PagingCookie = tmpResult.PagingCookie;
                            resultCollection.TotalRecordCount = tmpResult.TotalRecordCount;
                            resultCollection.TotalRecordCountLimitExceeded = tmpResult.TotalRecordCountLimitExceeded;
                        }
                        if (currentSettings.retrieveAllPages && query is QueryExpression && tmpResult.MoreRecords)
                        {
                            ((QueryExpression)query).PageInfo.PageNumber++;
                            ((QueryExpression)query).PageInfo.PagingCookie = tmpResult.PagingCookie;
                        }
                        page++;
                        var duration = DateTime.Now - start;
                        if (page == 1)
                        {
                            SendMessageToStatusBar(this, new StatusBarMessageEventArgs($"Retrieved {resultCollection.Entities.Count} records on first page in {duration.TotalSeconds:F2} seconds"));
                        }
                        else
                        {
                            SendMessageToStatusBar(this, new StatusBarMessageEventArgs($"Retrieved {resultCollection.Entities.Count} records on {page} pages in {duration.TotalSeconds:F2} seconds"));
                        }
                    }
                    while (currentSettings.retrieveAllPages && query is QueryExpression && tmpResult.MoreRecords);
                    if (outputtype == 1 && outputstyle == 2)
                    {
                        var json = EntityCollectionSerializer.ToJSON(resultCollection, Formatting.Indented);
                        eventargs.Result = json;
                    }
                    else
                    {
                        eventargs.Result = resultCollection;
                    }
                })
            {
                PostWorkCallBack = (completedargs) =>
                {
                    working = false;
                    if (completedargs.Error != null)
                    {
                        LogError("RetrieveMultiple error: {0}", completedargs.Error);
                        if (MessageBox.Show(completedargs.Error.Message + "\n\nTry with result as ExecuteFetch?", "Execute", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                        {
                            ExecuteFetch(fetch);
                        }
                    }
                    if (completedargs.Result is EntityCollection entities)
                    {
                        if (outputtype == 0)
                        {
                            if (resultGrid == null)
                            {
                                resultGrid = new ResultGrid(entities, this);
                                resultGrid.Show(dockContainer, DockState.Document);
                            }
                            else if (currentSettings.resultsAlwaysNewWindow)
                            {
                                var newresults = new ResultGrid(entities, this);
                                resultpanes++;
                                newresults.Text = $"Results ({resultpanes})";
                                newresults.Show(dockContainer, DockState.Document);
                            }
                            else
                            {
                                resultGrid.SetData(entities);
                                resultGrid.Activate();
                            }
                        }
                        else if (outputtype == 1)
                        {
                            if (outputstyle == 0)
                            {
                                var serialized = EntityCollectionSerializer.Serialize(entities, SerializationStyle.Explicit);
                                XmlContentDisplayDialog.Show(serialized.OuterXml, "XML Serialized RetrieveMultiple result", false, false, false, SaveFormat.XML, this);
                            }
                            else if (outputstyle == 1)
                            {
                                var serialized = EntityCollectionSerializer.Serialize(entities, SerializationStyle.Basic);
                                XmlContentDisplayDialog.Show(serialized.OuterXml, "XML Serialized RetrieveMultiple result", false, false, false, SaveFormat.XML, this);
                            }
                            else if (outputstyle == 3)
                            {
                                var serializer = new DataContractSerializer(typeof(EntityCollection), null, int.MaxValue, false, false, null, new KnownTypesResolver());
                                var sw = new StringWriter();
                                var xw = new XmlTextWriter(sw);
                                serializer.WriteObject(xw, entities);
                                xw.Close();
                                sw.Close();
                                var serialized = sw.ToString();
                                XmlContentDisplayDialog.Show(serialized, "Serialized EntityCollection", false, false, false, SaveFormat.XML, this);
                            }
                        }
                    }
                    else if (outputtype == 1 && outputstyle == 2 && completedargs.Result is string)
                    {
                        var result = completedargs.Result.ToString();
                        XmlContentDisplayDialog.Show(result, "JSON Serialized RetrieveMultiple result", false, false, false, SaveFormat.JSON, this);
                    }
                }
            });
            LogUse("RetrieveMultiple-" + outputtypestring);
        }

        internal void LoadViews(Action viewsLoaded)
        {
            WorkAsync(new WorkAsyncInfo("Loading views...",
            (eventargs) =>
            {
                EnableControls(false);
                if (views == null || views.Count == 0)
                {
                    if (Service == null)
                    {
                        throw new Exception("Need a connection to load views.");
                    }
                    var qexs = new QueryExpression("savedquery");
                    qexs.ColumnSet = new ColumnSet("name", "returnedtypecode", "fetchxml");
                    qexs.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
                    qexs.Criteria.AddCondition("iscustomizable", ConditionOperator.Equal, true);
                    qexs.AddOrder("name", OrderType.Ascending);
                    var sysviews = Service.RetrieveMultiple(qexs);
                    foreach (var view in sysviews.Entities)
                    {
                        var entityname = view["returnedtypecode"].ToString();
                        if (!string.IsNullOrWhiteSpace(entityname) && entities.ContainsKey(entityname))
                        {
                            if (views == null)
                            {
                                views = new Dictionary<string, List<Entity>>();
                            }
                            if (!views.ContainsKey(entityname + "|S"))
                            {
                                views.Add(entityname + "|S", new List<Entity>());
                            }
                            views[entityname + "|S"].Add(view);
                        }
                    }
                    var qexu = new QueryExpression("userquery");
                    qexu.ColumnSet = new ColumnSet("name", "returnedtypecode", "fetchxml");
                    qexu.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
                    qexu.AddOrder("name", OrderType.Ascending);
                    var userviews = Service.RetrieveMultiple(qexu);
                    foreach (var view in userviews.Entities)
                    {
                        var entityname = view["returnedtypecode"].ToString();
                        if (!string.IsNullOrWhiteSpace(entityname) && entities.ContainsKey(entityname))
                        {
                            if (views == null)
                            {
                                views = new Dictionary<string, List<Entity>>();
                            }
                            if (!views.ContainsKey(entityname + "|U"))
                            {
                                views.Add(entityname + "|U", new List<Entity>());
                            }
                            views[entityname + "|U"].Add(view);
                        }
                    }
                }
            })
            {
                PostWorkCallBack = (completedargs) =>
                  {
                      EnableControls(true);
                      if (completedargs.Error != null)
                      {
                          MessageBox.Show(completedargs.Error.Message);
                      }
                      else
                      {
                          viewsLoaded();
                      }
                  }
            });
        }

        private void OpenFile()
        {
            if (!SaveIfChanged())
            {
                return;
            }
            var ofd = new OpenFileDialog
            {
                Title = "Select an XML file containing FetchXML",
                Filter = "XML file (*.xml)|*.xml"
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                EnableControls(false);
                FileName = ofd.FileName;
                var fetchDoc = new XmlDocument();
                fetchDoc.Load(ofd.FileName);
                treeControl.Init(fetchDoc.OuterXml, "open file", true);
                LogUse("OpenFile");
            }
            EnableControls(true);
        }

        private void OpenView()
        {
            if (!SaveIfChanged())
            {
                return;
            }
            if (views == null || views.Count == 0)
            {
                LoadViews(OpenView);
            }
            else
            {
                var viewselector = new SelectViewDialog(this);
                viewselector.StartPosition = FormStartPosition.CenterParent;
                if (viewselector.ShowDialog() == DialogResult.OK)
                {
                    if (viewselector.View.Contains("fetchxml") && !string.IsNullOrEmpty(viewselector.View["fetchxml"].ToString()))
                    {
                        View = viewselector.View;
                        treeControl.Init(View["fetchxml"].ToString(), "open view", false);
                        attributesChecksum = treeControl.GetAttributesSignature(null);
                        LogUse("OpenView");
                    }
                    else
                    {
                        if (MessageBox.Show("The selected view does not contain any FetchXML.\nPlease select another one.", "Open View",
                            MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                        {
                            OpenView();
                        }
                    }
                }
            }
            EnableControls(true);
        }

        private void OpenML()
        {
            if (!SaveIfChanged())
            {
                return;
            }
            var mlselector = new SelectMLDialog(this);
            mlselector.StartPosition = FormStartPosition.CenterParent;
            if (mlselector.ShowDialog() == DialogResult.OK)
            {
                if (mlselector.View.Contains("query") && !string.IsNullOrEmpty(mlselector.View["query"].ToString()))
                {
                    DynML = mlselector.View;
                    treeControl.Init(DynML["query"].ToString(), "open marketing list", false);
                    LogUse("OpenML");
                }
                else
                {
                    if (MessageBox.Show("The selected marketing list does not contain any FetchXML.\nPlease select another one.", "Open Marketing List",
                        MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                    {
                        OpenML();
                    }
                }
            }
            EnableControls(true);
        }

        private void OpenCWPFeed()
        {
            if (!SaveIfChanged())
            {
                return;
            }
            var feedid = Prompt.ShowDialog("Enter CWP Feed ID", "Open CWP Feed");
            if (string.IsNullOrWhiteSpace(feedid))
            {
                return;
            }
            Entity feed = GetCWPFeed(feedid);
            if (feed == null)
            {
                MessageBox.Show("Feed not found.", "Open CWP Feed", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (feed.Contains("cint_fetchxml"))
            {
                CWPFeed = feed.Contains("cint_id") ? feed["cint_id"].ToString() : feedid;
                treeControl.Init(feed["cint_fetchxml"].ToString(), "open CWP feed", false);
                LogUse("OpenCWP");
            }
            EnableControls(true);
        }

        private void SaveView()
        {
            var currentAttributes = treeControl.GetAttributesSignature(null);
            if (currentAttributes != attributesChecksum)
            {
                MessageBox.Show("Cannot save view, returned attributes must not be changed.\n\nExpected attributes:\n  " +
                    attributesChecksum.Replace("\n", "\n  ") + "\nCurrent attributes:\n  " + currentAttributes.Replace("\n", "\n  "),
                    "Cannot save view", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (View.LogicalName == "savedquery")
            {
                if (MessageBox.Show("This will update and publish the saved query in CRM.\n\nConfirm!", "Confirm",
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.Cancel)
                {
                    return;
                }
            }
            var msg = View.LogicalName == "savedquery" ? "Saving and publishing {0}..." : "Saving {0}...";
            WorkAsync(new WorkAsyncInfo(string.Format(msg, View["name"]),
                (eventargs) =>
                {
                    var xml = treeControl.GetFetchString(false, false);
                    Entity newView = new Entity(View.LogicalName);
                    newView.Id = View.Id;
                    newView.Attributes.Add("fetchxml", xml);
                    Service.Update(newView);
                    if (View.LogicalName == "savedquery")
                    {
                        var pubRequest = new PublishXmlRequest();
                        pubRequest.ParameterXml = string.Format(
                            @"<importexportxml><entities><entity>{0}</entity></entities><nodes/><securityroles/><settings/><workflows/></importexportxml>",
                            View["returnedtypecode"].ToString());
                        Service.Execute(pubRequest);
                        LogUse("SaveView");
                    }
                    View["fetchxml"] = xml;
                })
            {
                PostWorkCallBack = (completedargs) =>
                {
                    if (completedargs.Error != null)
                    {
                        MessageBox.Show(completedargs.Error.Message, "Save view", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        treeControl.ClearChanged();
                    }
                }
            });
        }

        private void SaveML()
        {
            var msg = "Saving {0}...";
            WorkAsync(new WorkAsyncInfo(string.Format(msg, DynML["listname"]),
                (eventargs) =>
                {
                    var xml = treeControl.GetFetchString(false, false);
                    Entity newView = new Entity(DynML.LogicalName);
                    newView.Id = DynML.Id;
                    newView.Attributes.Add("query", xml);
                    Service.Update(newView);
                    DynML["query"] = xml;
                })
            {
                PostWorkCallBack = (completedargs) =>
                {
                    if (completedargs.Error != null)
                    {
                        MessageBox.Show(completedargs.Error.Message, "Save Marketing List", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        treeControl.ClearChanged();
                    }
                }
            });
        }

        private void SaveCWPFeed()
        {
            if (string.IsNullOrWhiteSpace(CWPFeed))
            {
                var feedid = Prompt.ShowDialog("Enter CWP Feed ID (enter existing ID to update feed)", "Save CWP Feed");
                if (feedid == null)
                {
                    return;
                }
                if (string.IsNullOrWhiteSpace(feedid))
                {
                    MessageBox.Show("Feed not saved.", "Save CWP Feed", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                CWPFeed = feedid;
            }
            Entity feed = GetCWPFeed(CWPFeed);
            if (feed == null)
            {
                feed = new Entity("cint_feed");
            }
            if (feed.Contains("cint_fetchxml"))
            {
                feed.Attributes.Remove("cint_fetchxml");
            }
            feed.Attributes.Add("cint_fetchxml", treeControl.GetFetchString(true, false));
            var verb = feed.Id.Equals(Guid.Empty) ? "created" : "updated";
            if (feed.Id.Equals(Guid.Empty))
            {
                feed.Attributes.Add("cint_id", CWPFeed);
                feed.Attributes.Add("cint_description", "Created by FetchXML Builder for XrmToolBox");
                Service.Create(feed);
            }
            else
            {
                Service.Update(feed);
            }
            LogUse("SaveCWPFeed");
            MessageBox.Show("CWP Feed " + CWPFeed + " has been " + verb + "!", "Save CWP Feed", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private EntityMetadata GetEntity(int etc)
        {
            foreach (EntityMetadata entity in entities.Values)
            {
                if (entity.ObjectTypeCode == etc)
                {
                    return entity;
                }
            }
            return null;
        }

        private Entity GetCWPFeed(string feedid)
        {
            var qeFeed = new QueryExpression("cint_feed");
            qeFeed.ColumnSet.AddColumns("cint_id", "cint_fetchxml");
            qeFeed.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            qeFeed.Criteria.AddCondition("cint_id", ConditionOperator.Equal, feedid);
            var feeds = Service.RetrieveMultiple(qeFeed);
            Entity feed = feeds.Entities.Count > 0 ? feeds.Entities[0] : null;
            return feed;
        }

        internal void UpdateLiveXML()
        {
            if (tsmiLiveUpdate.Checked && xmlLiveUpdate?.Focused != true)
            {
                liveUpdateXml = treeControl.GetFetchString(false, false);
                if (xmlLiveUpdate == null)
                {
                    xmlLiveUpdate = new XmlContentDisplayDialog(liveUpdateXml, "FetchXML Live Update", false, true, false, SaveFormat.None, this);
                    xmlLiveUpdate.Disposed += LiveXML_Disposed;
                    xmlLiveUpdate.txtXML.KeyUp += LiveXML_KeyUp;
                    xmlLiveUpdate.Visible = true;
                    AlignLiveXML();
                }
                else
                {
                    xmlLiveUpdate.UpdateXML(liveUpdateXml);
                }
            }
            if (tsmiShowOData.Checked)
            {
                DisplayOData();
            }
        }

        private void AlignLiveXML()
        {
            if (xmlLiveUpdate != null && xmlLiveUpdate.Visible)
            {
                Control topparent = this;
                while (topparent.Parent != null)
                {
                    topparent = topparent.Parent;
                }
                xmlLiveUpdate.Location = new Point(topparent.Location.X + topparent.Size.Width, topparent.Location.Y);
            }
        }

        private void DisplayQExCode()
        {
            try
            {
                var QEx = treeControl.GetQueryExpression();
                var code = QueryExpressionCodeGenerator.GetCSharpQueryExpression(QEx);
                LogUse("DisplayQExCode");
                XmlContentDisplayDialog.Show(code, "QueryExpression Code", false, false, false, SaveFormat.None, this);
            }
            catch (FetchIsAggregateException ex)
            {
                MessageBox.Show("This FetchXML is not possible to convert to QueryExpression in the current version of the SDK.\n\n" + ex.Message, "QueryExpression", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to generate C# QueryExpression code.\n\n" + ex.Message, "QueryExpression", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DisplayOData()
        {
            try
            {
                var prefix = "OData: ";
                var url = GetOData();
                linkOData.Text = prefix + url;
                linkOData.LinkArea = new LinkArea(prefix.Length, url.Length);
                if (linkOData.Links.Count > 0)
                {
                    linkOData.Links[0].LinkData = url;
                }
            }
            catch (Exception ex)
            {
                linkOData.Text = ex.Message;
                linkOData.Links.Clear();
            }
        }

        private string GetOData()
        {
            if (Service == null || ConnectionDetail == null || ConnectionDetail.OrganizationDataServiceUrl == null)
            {
                throw new Exception("Must have an active connection to CRM to compose OData query.");
            }
            FetchType fetch = treeControl.GetFetchType();
            var odata = ODataCodeGenerator.GetODataQuery(fetch, ConnectionDetail.OrganizationDataServiceUrl, this);
            return odata;
        }

        private void DisplaySQLQuery()
        {
            FetchType fetch = treeControl.GetFetchType();
            try
            {
                var sql = SQLQueryGenerator.GetSQLQuery(fetch);
                LogUse("DisplaySQLQuery");
                XmlContentDisplayDialog.Show(sql, "SQL Query", false, false, false, SaveFormat.None, this);
            }
            catch (Exception ex)
            {
                LogUse("DisplaySQLQuery failed");
                MessageBox.Show("Failed to generate SQL Query.\n\n" + ex.Message, "SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        internal void LogUse(string action, bool forceLog = false)
        {
            if (currentSettings.logUsage == true || forceLog)
            {
                LogUsage.DoLog(action);
            }
        }

        private void ReturnToCaller()
        {
            if (callerArgs == null)
            {
                return;
            }
            LogUse("ReturnTo." + callerArgs.SourcePlugin);
            var fetch = treeControl.GetFetchString(true, true);
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
                        fxbArgs.QueryExpression = treeControl.GetQueryExpression();
                        break;
                    case FXBMessageBusRequest.OData:
                        fxbArgs.OData = GetOData();
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

        internal void EnableDisableHistoryButtons(HistoryManager historyMgr)
        {
            historyMgr.SetupUndoButton(tsbUndo);
            historyMgr.SetupRedoButton(tsbRedo);
        }

        #endregion Instance methods

        #region Static methods

        internal static string GetEntityDisplayName(string entityName)
        {
            if (!friendlyNames)
            {
                return entityName;
            }
            if (entities != null && entities.ContainsKey(entityName))
            {
                entityName = GetEntityDisplayName(entities[entityName]);
            }
            return entityName;
        }

        internal static string GetEntityDisplayName(EntityMetadata entity)
        {
            var result = entity.LogicalName;
            if (FetchXmlBuilder.friendlyNames)
            {
                if (entity.DisplayName.UserLocalizedLabel != null)
                {
                    result = entity.DisplayName.UserLocalizedLabel.Label;
                }
                if (result == entity.LogicalName && entity.DisplayName.LocalizedLabels.Count > 0)
                {
                    result = entity.DisplayName.LocalizedLabels[0].Label;
                }
            }
            return result;
        }

        internal static AttributeMetadata GetAttribute(string entityName, string attributeName)
        {
            if (entities != null && entities.ContainsKey(entityName))
            {
                if (entities[entityName].Attributes != null)
                {
                    foreach (var attribute in entities[entityName].Attributes)
                    {
                        if (attribute.LogicalName == attributeName)
                        {
                            return attribute;
                        }
                    }
                }
            }
            return null;
        }

        internal static string GetAttributeDisplayName(string entityName, string attributeName)
        {
            if (!friendlyNames)
            {
                return attributeName;
            }
            var attribute = GetAttribute(entityName, attributeName);
            if (attribute != null)
            {
                attributeName = GetAttributeDisplayName(attribute);
            }
            return attributeName;
        }

        internal static string GetAttributeDisplayName(AttributeMetadata attribute)
        {
            string attributeName = attribute.LogicalName;
            if (friendlyNames)
            {
                if (attribute.DisplayName.UserLocalizedLabel != null)
                {
                    attributeName = attribute.DisplayName.UserLocalizedLabel.Label;
                }
                if (attributeName == attribute.LogicalName && attribute.DisplayName.LocalizedLabels.Count > 0)
                {
                    attributeName = attribute.DisplayName.LocalizedLabels[0].Label;
                }
            }
            return attributeName;
        }

        internal Dictionary<string, EntityMetadata> GetDisplayEntities()
        {
            var result = new Dictionary<string, EntityMetadata>();
            if (entities != null)
            {
                foreach (var entity in entities)
                {
                    if (!currentSettings.showEntitiesAll)
                    {
                        if (!currentSettings.showEntitiesManaged && entity.Value.IsManaged == true) { continue; }
                        if (!currentSettings.showEntitiesUnmanaged && entity.Value.IsManaged == false) { continue; }
                        if (!currentSettings.showEntitiesCustomizable && entity.Value.IsCustomizable.Value) { continue; }
                        if (!currentSettings.showEntitiesUncustomizable && !entity.Value.IsCustomizable.Value) { continue; }
                        if (!currentSettings.showEntitiesStandard && entity.Value.IsCustomEntity == false) { continue; }
                        if (!currentSettings.showEntitiesCustom && entity.Value.IsCustomEntity == true) { continue; }
                        if (!currentSettings.showEntitiesIntersect && entity.Value.IsIntersect == true) { continue; }
                        if (currentSettings.showEntitiesOnlyValidAF && entity.Value.IsValidForAdvancedFind == false) { continue; }
                    }
                    result.Add(entity.Key, entity.Value);
                }
            }
            return result;
        }

        internal AttributeMetadata[] GetDisplayAttributes(string entityName)
        {
            var result = new List<AttributeMetadata>();
            AttributeMetadata[] attributes = null;
            if (entities != null && entities.ContainsKey(entityName))
            {
                attributes = entities[entityName].Attributes;
                if (attributes != null)
                {
                    foreach (var attribute in attributes)
                    {
                        if (!currentSettings.showAttributesAll)
                        {
                            if (attribute.IsValidForRead == false) { continue; }
                            if (!string.IsNullOrEmpty(attribute.AttributeOf)) { continue; }
                            if (!currentSettings.showAttributesManaged && attribute.IsManaged == true) { continue; }
                            if (!currentSettings.showAttributesUnmanaged && attribute.IsManaged == false) { continue; }
                            if (!currentSettings.showAttributesCustomizable && attribute.IsCustomizable.Value) { continue; }
                            if (!currentSettings.showAttributesUncustomizable && !attribute.IsCustomizable.Value) { continue; }
                            if (!currentSettings.showAttributesStandard && attribute.IsCustomAttribute == false) { continue; }
                            if (!currentSettings.showAttributesCustom && attribute.IsCustomAttribute == true) { continue; }
                            if (currentSettings.showAttributesOnlyValidAF && attribute.IsValidForAdvancedFind.Value == false) { continue; }
                            if (currentSettings.showAttributesOnlyValidRead && attribute.IsValidForRead.Value == false) { continue; }
                        }
                        result.Add(attribute);
                    }
                }
            }
            return result.ToArray();
        }

        #endregion Static methods

        private void tsmiToJavascript_Click(object sender, EventArgs e)
        {
            DisplayJavascriptCode();
        }

        private void DisplayJavascriptCode()
        {
            var fetch = treeControl.GetFetchString(true, false);
            try
            {
                var js = JavascriptCodeGenerator.GetJavascriptCode(fetch);
                LogUse("DisplayJavascriptCode");
                XmlContentDisplayDialog.Show(js, "Javascript Code", false, false, false, SaveFormat.None, this);
            }
            catch (Exception ex)
            {
                LogError("DisplayJavascriptCode failed\n{0}", fetch);
                MessageBox.Show("Failed to generate Javascript code.\n\n" + ex.Message, "Javascript", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tsmiToCSharp_Click(object sender, EventArgs e)
        {
            DisplayCSharpCode();
        }

        private void DisplayCSharpCode()
        {
            var fetch = treeControl.GetFetchString(true, false);
            try
            {
                var cs = CSharpCodeGenerator.GetCSharpCode(fetch);
                LogUse("DisplayCSharpCode");
                XmlContentDisplayDialog.Show(cs, "C# Code", false, false, false, SaveFormat.None, this);
            }
            catch (Exception ex)
            {
                LogError("DisplayCSharpCode failed\n{0}", fetch);
                MessageBox.Show("Failed to generate C# code.\n\n" + ex.Message, "C#", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void ReceiveKeyDownShortcut(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5 && tsbExecute.Enabled)
            {
                tsbExecute_Click(null, null);
            }
            else if (e.Control && e.KeyCode == Keys.E && tsbEdit.Enabled)
            {
                tsbEdit_Click(null, null);
            }
            else if (e.Control && e.KeyCode == Keys.N && tsbNew.Enabled)
            {
                tsbNew_Click(null, null);
            }
            else if (e.Control && e.KeyCode == Keys.O && tsmiOpenFile.Enabled)
            {
                tsmiOpenFile_Click(null, null);
            }
            else if (e.Control && e.KeyCode == Keys.S && tsmiSaveFile.Enabled)
            {
                tsmiSaveFile_Click(null, null);
            }
            else if (e.KeyCode == Keys.F12 && tsmiSaveFileAs.Enabled)
            {
                tsmiSaveFileAs_Click(null, null);
            }
            else if (e.Control && e.KeyCode == Keys.Z && tsbUndo.Enabled)
            {
                tsbUndo_Click(null, null);
            }
            else if (e.Control && e.KeyCode == Keys.Y && tsbRedo.Enabled)
            {
                tsbRedo_Click(null, null);
            }
            else if (e.Control && e.KeyCode == Keys.L && tsmiLiveUpdate.Enabled)
            {
                tsmiLiveUpdate.Checked = !tsmiLiveUpdate.Checked;
            }
            else if (e.Control && e.KeyCode == Keys.F)
            {
                currentSettings.useFriendlyNames = !currentSettings.useFriendlyNames;
                treeControl.ApplyCurrentSettings();
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

        private void resetWindowLayoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResetDockLayout();
        }
    }
}
