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

        private const string aiEndpoint = "https://dc.services.visualstudio.com/v2/track";
        //private const string aiKey = "cc7cb081-b489-421d-bb61-2ee53495c336";    // jonas@rappen.net tenant, TestAI 
        private const string aiKey = "eed73022-2444-45fd-928b-5eebd8fa46a6";    // jonas@rappen.net tenant, XrmToolBox

        #region Internal Fields

        internal static Dictionary<string, EntityMetadata> entities;
        internal static bool friendlyNames = false;
        internal static Dictionary<string, List<Entity>> views;
        internal FXBSettings settings = new FXBSettings();
        internal TreeBuilderControl dockControlBuilder;
        internal bool working = false;

        #endregion Internal Fields

        #region Private Fields

        private static List<string> entityShitList = new List<string>();
        private string attributesChecksum = "";
        private bool buttonsEnabled = true;
        private MessageBusEventArgs callerArgs = null;
        private string cwpfeed;
        private XmlContentControl dockControlFetchResult;
        private XmlContentControl dockControlFetchXml;
        private XmlContentControl dockControlFetchXmlCs;
        private XmlContentControl dockControlFetchXmlJs;
        private ResultGrid dockControlGrid;
        private ODataControl dockControlOData;
        private XmlContentControl dockControlQExp;
        private XmlContentControl dockControlSQL;
        private Entity dynml;
        private string fileName;
        private string liveUpdateXml = "";
        private int resultpanecount = 0;
        private Entity view;
        private AppInsights ai;

        #endregion Private Fields

        #region Public Constructors

        public FetchXmlBuilder()
        {
            InitializeComponent();
            ai = new AppInsights(new AiConfig(aiEndpoint, aiKey)
            {
                PluginName = "FetchXML Builder"
            });
            var theme = new VS2015LightTheme();
            dockContainer.Theme = theme;
            dockContainer.Theme.Skin.DockPaneStripSkin.TextFont = Font;
            //dockContainer.DockBackColor = SystemColors.Window;
        }

        #endregion Public Constructors

        #region Public Events

        public event EventHandler<MessageBusEventArgs> OnOutgoingMessage;

        public event EventHandler<StatusBarMessageEventArgs> SendMessageToStatusBar;

        #endregion Public Events

        #region Public Properties

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

        public string RepositoryName
        {
            get { return "FetchXMLBuilder"; }
        }

        public string UserName
        {
            get { return "Innofactor"; }
        }

        #endregion Public Properties

        #region Private Properties

        private string CWPFeed
        {
            get { return cwpfeed; }
            set
            {
                if (value == cwpfeed)
                {
                    return;
                }
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

        private Entity DynML
        {
            get { return dynml; }
            set
            {
                if (value == dynml)
                {
                    return;
                }
                if (value != null)
                {
                    ResetSourcePointers();
                }
                dynml = value;
                if (dynml != null && dynml.Contains("listname"))
                {
                    dockControlBuilder.SetFetchName($"ML: {dynml["listname"]}");
                    tsmiSaveML.Text = "Save Marketing List: " + dynml["listname"];
                }
                else
                {
                    dockControlBuilder.SetFetchName(string.Empty);
                    tsmiSaveML.Text = "Save Marketing List";
                }
            }
        }

        private string FileName
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
                    dockControlBuilder.SetFetchName($"File: {file}");
                    tsmiSaveFile.Text = $"Save File: {file}";
                }
                else
                {
                    dockControlBuilder.SetFetchName(string.Empty);
                    tsmiSaveFile.Text = "Save File";
                }
            }
        }

        private Entity View
        {
            get { return view; }
            set
            {
                if (value == view)
                {
                    return;
                }
                if (value != null)
                {
                    ResetSourcePointers();
                }
                view = value;
                if (view != null && view.Contains("name"))
                {
                    dockControlBuilder.SetFetchName($"View: {view["name"]}");
                    tsmiSaveView.Text = $"Save View: {view["name"]}";
                }
                else
                {
                    dockControlBuilder.SetFetchName(string.Empty);
                    tsmiSaveView.Text = "Save View";
                }
            }
        }

        #endregion Private Properties

        #region Public Methods

        public override void ClosingPlugin(PluginCloseInfo info)
        {
            if (!SaveIfChanged())
            {
                info.Cancel = true;
            }
            SaveDockPanels();
            dockControlBuilder?.Close();
            dockControlFetchXml?.Close();
            dockControlFetchXmlCs?.Close();
            dockControlFetchXmlJs?.Close();
            dockControlFetchResult?.Close();
            dockControlGrid?.Close();
            dockControlOData?.Close();
            dockControlQExp?.Close();
            dockControlSQL?.Close();
            SaveSetting();
            LogUse("Close");
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
            dockControlBuilder.ParseXML(fetchXml, false);
            tsbReturnToCaller.ToolTipText = "Return " + requestedType + " to " + callerArgs.SourcePlugin;
            dockControlBuilder.RecordHistory("called from " + message.SourcePlugin);
            LogUse("CalledBy." + callerArgs.SourcePlugin);
            EnableControls(true);
        }

        public void ReceiveKeyDownShortcut(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5 && tsbExecute.Enabled)
            {
                tsbExecute_Click(null, null);
            }
            else if (e.Control && e.KeyCode == Keys.E && tsmiShowFetchXML.Enabled)
            {
                tsmiShowFetchXML_Click(null, null);
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
            else if (e.Control && e.KeyCode == Keys.F)
            {
                settings.UseFriendlyNames = !settings.UseFriendlyNames;
                dockControlBuilder.ApplyCurrentSettings();
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

        #endregion Public Methods

        #region Internal Methods

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

        internal void EnableControls()
        {
            EnableControls(buttonsEnabled);
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
                    tsbOpen.Enabled = enabled;
                    tsmiOpenFile.Enabled = enabled;
                    tsmiOpenView.Enabled = enabled && Service != null;
                    tsmiOpenML.Enabled = enabled && Service != null;
                    tsmiOpenCWP.Visible = enabled && Service != null && entities != null && entities.ContainsKey("cint_feed");
                    tsbReturnToCaller.Visible = CallerWantsResults();
                    tsbSave.Enabled = enabled;
                    tsmiSaveFile.Enabled = enabled && dockControlBuilder.FetchChanged && !string.IsNullOrEmpty(FileName);
                    tsmiSaveFileAs.Enabled = enabled;
                    tsmiSaveView.Enabled = enabled && Service != null && View != null && View.IsCustomizable();
                    tsmiSaveML.Enabled = enabled && Service != null && DynML != null;
                    tsmiSaveCWP.Visible = enabled && Service != null && entities != null && entities.ContainsKey("cint_feed");
                    tsmiSaveCWP.Enabled = enabled && Service != null && dockControlBuilder.FetchChanged && !string.IsNullOrEmpty(CWPFeed);
                    tsbView.Enabled = enabled;
                    tsbExecute.Enabled = enabled && Service != null;
                    dockControlBuilder.EnableControls(enabled);
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

        internal void EnableDisableHistoryButtons(HistoryManager historyMgr)
        {
            historyMgr.SetupUndoButton(tsbUndo);
            historyMgr.SetupRedoButton(tsbRedo);
        }

        internal void FetchResults(string fetch = "")
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
                fetch = dockControlBuilder.GetFetchString(false, true);
            }
            if (string.IsNullOrEmpty(fetch))
            {
                return;
            }
            var fetchType = settings.Results.ResultOption;
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
                        if (!settings.Attribute.All)
                        {
                            if (attribute.IsValidForRead == false) { continue; }
                            if (!string.IsNullOrEmpty(attribute.AttributeOf)) { continue; }
                            if (!settings.Attribute.Managed && attribute.IsManaged == true) { continue; }
                            if (!settings.Attribute.Unmanaged && attribute.IsManaged == false) { continue; }
                            if (!settings.Attribute.Customizable && attribute.IsCustomizable.Value) { continue; }
                            if (!settings.Attribute.Uncustomizable && !attribute.IsCustomizable.Value) { continue; }
                            if (!settings.Attribute.Standard && attribute.IsCustomAttribute == false) { continue; }
                            if (!settings.Attribute.Custom && attribute.IsCustomAttribute == true) { continue; }
                            if (settings.Attribute.OnlyValidAF && attribute.IsValidForAdvancedFind.Value == false) { continue; }
                            if (settings.Attribute.OnlyValidRead && attribute.IsValidForRead.Value == false) { continue; }
                        }
                        result.Add(attribute);
                    }
                }
            }
            return result.ToArray();
        }

        internal Dictionary<string, EntityMetadata> GetDisplayEntities()
        {
            var result = new Dictionary<string, EntityMetadata>();
            if (entities != null)
            {
                foreach (var entity in entities)
                {
                    if (!settings.Entity.All)
                    {
                        if (!settings.Entity.Managed && entity.Value.IsManaged == true) { continue; }
                        if (!settings.Entity.Unmanaged && entity.Value.IsManaged == false) { continue; }
                        if (!settings.Entity.Customizable && entity.Value.IsCustomizable.Value) { continue; }
                        if (!settings.Entity.Uncustomizable && !entity.Value.IsCustomizable.Value) { continue; }
                        if (!settings.Entity.Standard && entity.Value.IsCustomEntity == false) { continue; }
                        if (!settings.Entity.Custom && entity.Value.IsCustomEntity == true) { continue; }
                        if (!settings.Entity.Intersect && entity.Value.IsIntersect == true) { continue; }
                        if (settings.Entity.OnlyValidAF && entity.Value.IsValidForAdvancedFind == false) { continue; }
                    }
                    result.Add(entity.Key, entity.Value);
                }
            }
            return result;
        }

        internal string GetOData()
        {
            if (Service == null || ConnectionDetail == null || ConnectionDetail.OrganizationDataServiceUrl == null)
            {
                throw new Exception("Must have an active connection to CRM to compose OData query.");
            }
            FetchType fetch = dockControlBuilder.GetFetchType();
            var odata = ODataCodeGenerator.GetODataQuery(fetch, ConnectionDetail.OrganizationDataServiceUrl, this);
            return odata;
        }

        internal void LiveXML_KeyUp(object sender, KeyEventArgs e)
        {
            if (dockControlFetchXml != null && dockControlFetchXml.chkLiveUpdate.Checked &&
                dockControlFetchXml.txtXML != null && dockControlFetchXml.Visible && !e.Handled)
            {
                tmLiveUpdate.Stop();
                tmLiveUpdate.Start();
            }
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
                    qexs.ColumnSet = new ColumnSet("name", "returnedtypecode", "fetchxml", "iscustomizable");
                    qexs.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
                    qexs.Criteria.AddCondition("fetchxml", ConditionOperator.NotNull);
                    if (!settings.OpenUncustomizableViews)
                    {
                        qexs.Criteria.AddCondition("iscustomizable", ConditionOperator.Equal, true);
                    }
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

        internal void LogUse(string action, bool forceLog = false, double? count = null, double? duration = null)
        {
            ai.WriteEvent(action, count, duration, HandleAIResult);
            if (settings.LogUsage == true || forceLog)
            {
                LogUsage.DoLog(action);
            }
        }

        private void HandleAIResult(string result)
        {
            if (!string.IsNullOrEmpty(result))
            {
                LogError("Failed to write to Application Insights:\n{0}", result);
            }
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

        internal void UpdateLiveXML(bool preventxmlupdate = false)
        {
            var fetch = string.Empty;
            string GetFetch()
            {
                if (string.IsNullOrWhiteSpace(fetch))
                {
                    fetch = dockControlBuilder.GetFetchString(true, false);
                }
                return fetch;
            }
            if (!preventxmlupdate && dockControlFetchXml?.Visible == true)
            {
                dockControlFetchXml.UpdateXML(GetFetch());
            }
            if (dockControlOData?.Visible == true && entities != null)
            {
                dockControlOData.DisplayOData(GetOData());
            }
            if (dockControlQExp?.Visible == true && entities != null)
            {
                dockControlQExp.UpdateXML(GetQueryExpressionCode());
            }
            if (dockControlSQL?.Visible == true && entities != null)
            {
                dockControlSQL.UpdateXML(GetSQLQuery());
            }
            if (dockControlFetchXmlCs?.Visible == true)
            {
                dockControlFetchXmlCs.UpdateXML(CSharpCodeGenerator.GetCSharpCode(GetFetch()));
            }
            if (dockControlFetchXmlJs?.Visible == true)
            {
                dockControlFetchXmlJs.UpdateXML(JavascriptCodeGenerator.GetJavascriptCode(GetFetch()));
            }
        }

        #endregion Internal Methods

        #region Private Methods

        private static string GetDockFileName()
        {
            return Path.Combine(Paths.SettingsPath, "Cinteros.Xrm.FetchXmlBuilder_[DockPanels].xml");
        }

        private void ApplySettings()
        {
            dockControlBuilder.SplitterPos = settings.QueryOptions.TreeHeight;
            var connsett = GetConnectionSetting();
            if (connsett != null && !string.IsNullOrWhiteSpace(connsett.FetchXML))
            {
                dockControlBuilder.Init(connsett.FetchXML, "loaded from last session", false);
            }
            dockControlBuilder.lblQAExpander.GroupBoxSetState(null, settings.QueryOptions.ShowQuickActions);
            var ass = Assembly.GetExecutingAssembly().GetName();
            var version = ass.Version.ToString();
            if (!version.Equals(settings.CurrentVersion))
            {
                // Reset some settings when new version is deployed
                settings.LogUsage = true;
                settings.CurrentVersion = version;
                LogUse("ShowWelcome");
                Welcome.ShowWelcome(this);
            }
        }

        private bool CallerWantsResults()
        {
            return callerArgs != null;
        }

        private IDockContent dockDeSerialization(string persistString)
        {
            if (persistString == typeof(TreeBuilderControl).ToString() && dockControlBuilder?.IsDisposed != false)
            {
                dockControlBuilder = new TreeBuilderControl(this);
                return dockControlBuilder;
            }
            else if (persistString == typeof(ResultGrid).ToString() && dockControlGrid?.IsDisposed != false)
            {
                dockControlGrid = new ResultGrid(this);
                return dockControlGrid;
            }
            else if ((persistString == XmlContentControl.GetPersistString(ContentType.FetchXML_Result) ||
                      persistString == XmlContentControl.GetPersistString(ContentType.Serialized_Result_JSON) ||
                      persistString == XmlContentControl.GetPersistString(ContentType.Serialized_Result_XML)) &&
                      dockControlFetchResult?.IsDisposed != false)
            {
                dockControlFetchResult = new XmlContentControl(ContentType.FetchXML_Result, SaveFormat.XML, this);
                return dockControlFetchResult;
            }
            else if (persistString == XmlContentControl.GetPersistString(ContentType.FetchXML) && dockControlFetchXml?.IsDisposed != false)
            {
                dockControlFetchXml = new XmlContentControl(this);
                return dockControlFetchXml;
            }
            return null;
        }

        private void ExecuteFetch(string fetch)
        {
            working = true;
            LogUse("ExecuteFetch");
            WorkAsync(new WorkAsyncInfo("Executing FetchXML...",
                (eventargs) =>
                {
                    //var fetchxml = GetFetchDocument().OuterXml;
                    var start = DateTime.Now;
                    var resp = (ExecuteFetchResponse)Service.Execute(new ExecuteFetchRequest() { FetchXml = fetch });
                    var stop = DateTime.Now;
                    var duration = stop - start;
                    ai.WriteEvent("ExecuteFetch", null, duration.TotalMilliseconds, HandleAIResult);
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
                        ShowResultControl(doc.OuterXml, ContentType.FetchXML_Result, SaveFormat.XML, settings.DockStates.FetchResult);
                    }
                }
            });
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

        private string GetQueryExpressionCode()
        {
            var code = string.Empty;
            try
            {
                var QEx = dockControlBuilder.GetQueryExpression(null, false);
                code = QueryExpressionCodeGenerator.GetCSharpQueryExpression(QEx);
            }
            catch (FetchIsAggregateException ex)
            {
                code = "This FetchXML is not possible to convert to QueryExpression in the current version of the SDK.\n\n" + ex.Message;
            }
            catch (Exception ex)
            {
                code = "Failed to generate C# QueryExpression code.\n\n" + ex.Message;
            }
            return code;
        }

        private string GetSQLQuery()
        {
            var sql = string.Empty;
            FetchType fetch = dockControlBuilder.GetFetchType();
            try
            {
                sql = SQLQueryGenerator.GetSQLQuery(fetch);
            }
            catch (Exception ex)
            {
                sql = "Failed to generate SQL Query.\n\n" + ex.Message;
            }
            return sql;
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
                    UpdateLiveXML();
                    working = false;
                    EnableControls(true);
                }
            });
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
                dockControlBuilder.UpdateCurrentNode();
                UpdateLiveXML();
            }
            working = false;
        }

        /// <summary>Loads configurations from file</summary>
        private void LoadSetting()
        {
            try
            {
                if (SettingsManager.Instance.TryLoad<FXBSettings>(typeof(FetchXmlBuilder), out settings, "[Common]"))
                {
                    return;
                }
            }
            catch (InvalidOperationException) { }
            settings = new FXBSettings();
        }

        private FXBConnectionSettings GetConnectionSetting()
        {
            try
            {
                if (SettingsManager.Instance.TryLoad<FXBConnectionSettings>(typeof(FetchXmlBuilder), out FXBConnectionSettings connsett, ConnectionDetail?.ConnectionName))
                {
                    return connsett;
                }
            }
            catch (InvalidOperationException) { }
            return new FXBConnectionSettings();
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
                dockControlBuilder.Init(feed["cint_fetchxml"].ToString(), "open CWP feed", false);
                LogUse("OpenCWP");
            }
            EnableControls(true);
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
                dockControlBuilder.Init(fetchDoc.OuterXml, "open file", true);
                LogUse("OpenFile");
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
                    dockControlBuilder.Init(DynML["query"].ToString(), "open marketing list", false);
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
                        dockControlBuilder.Init(View["fetchxml"].ToString(), "open view", false);
                        attributesChecksum = dockControlBuilder.GetAttributesSignature(null);
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

        private void ResetDockLayout()
        {
            var i = 0;
            while (i < dockContainer.Contents.Count)
            {
                if (dockContainer.Contents[i] == dockControlBuilder)
                {
                    i++;
                }
                else
                {
                    dockContainer.Contents[i].DockHandler.Close();
                }
            }
            settings.DockStates = new DockStates();
            if (dockControlBuilder?.IsDisposed != false)
            {
                dockControlBuilder = new TreeBuilderControl(this);
            }
            dockControlBuilder.Show(dockContainer, DockState.DockLeft);
        }

        private void ResetSourcePointers()
        {
            FileName = null;
            CWPFeed = null;
            View = null;
            DynML = null;
        }

        private void RetrieveMultiple(string fetch)
        {
            working = true;
            LogUse("RetrieveMultiple-" + Settings.ResultOption2String(settings.Results.ResultOption, settings.Results.SerializeStyle));
            SendMessageToStatusBar(this, new StatusBarMessageEventArgs("Retrieving..."));
            WorkAsync(new WorkAsyncInfo("Executing FetchXML...",
                (eventargs) =>
                {
                    QueryBase query;
                    try
                    {
                        query = dockControlBuilder.GetQueryExpression(fetch);
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
                        if (settings.Results.RetrieveAllPages && query is QueryExpression && tmpResult.MoreRecords)
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
                    while (settings.Results.RetrieveAllPages && query is QueryExpression && tmpResult.MoreRecords);
                    ai.WriteEvent("RetrieveMultiple", resultCollection?.Entities?.Count, (DateTime.Now - start).TotalMilliseconds, HandleAIResult);
                    if (settings.Results.ResultOption == 1 && settings.Results.SerializeStyle == 2)
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
                    else if (completedargs.Result is EntityCollection entities)
                    {
                        if (settings.Results.ResultOption == 0)
                        {
                            if (settings.Results.AlwaysNewWindow)
                            {
                                var newresults = new ResultGrid(this);
                                resultpanecount++;
                                newresults.Text = $"Results ({resultpanecount})";
                                newresults.Show(dockContainer, settings.DockStates.ResultView);
                                newresults.SetData(entities);
                            }
                            else
                            {
                                if (dockControlGrid?.IsDisposed != false)
                                {
                                    dockControlGrid = new ResultGrid(this);
                                    dockControlGrid.Show(dockContainer, settings.DockStates.ResultView);
                                }
                                dockControlGrid.SetData(entities);
                                dockControlGrid.Activate();
                            }
                        }
                        else if (settings.Results.ResultOption == 1)
                        {
                            if (settings.Results.SerializeStyle == 0)
                            {
                                var serialized = EntityCollectionSerializer.Serialize(entities, SerializationStyle.Explicit);
                                ShowResultControl(serialized.OuterXml, ContentType.Serialized_Result_XML, SaveFormat.XML, settings.DockStates.FetchResult);
                            }
                            else if (settings.Results.SerializeStyle == 1)
                            {
                                var serialized = EntityCollectionSerializer.Serialize(entities, SerializationStyle.Basic);
                                ShowResultControl(serialized.OuterXml, ContentType.Serialized_Result_XML, SaveFormat.XML, settings.DockStates.FetchResult);
                            }
                            else if (settings.Results.SerializeStyle == 3)
                            {
                                var serializer = new DataContractSerializer(typeof(EntityCollection), null, int.MaxValue, false, false, null, new KnownTypesResolver());
                                var sw = new StringWriter();
                                var xw = new XmlTextWriter(sw);
                                serializer.WriteObject(xw, entities);
                                xw.Close();
                                sw.Close();
                                var serialized = sw.ToString();
                                ShowResultControl(serialized, ContentType.Serialized_Result_XML, SaveFormat.XML, settings.DockStates.FetchResult);
                            }
                        }
                    }
                    else if (settings.Results.ResultOption == 1 && settings.Results.SerializeStyle == 2 && completedargs.Result is string)
                    {
                        var result = completedargs.Result.ToString();
                        ShowResultControl(result, ContentType.Serialized_Result_JSON, SaveFormat.JSON, settings.DockStates.FetchResult);
                    }
                }
            });
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
            feed.Attributes.Add("cint_fetchxml", dockControlBuilder.GetFetchString(true, false));
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

        private void SaveDockPanels()
        {
            var dockFile = GetDockFileName();
            dockContainer.SaveAsXml(dockFile);
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
                dockControlBuilder.Save(FileName);
                LogUse("SaveFile");
                if (!silent)
                {
                    MessageBox.Show(this, "FetchXML saved!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                result = true;
                EnableControls(true);
            }
            return result;
        }

        /// <summary>Enables buttons relevant for currently selected node</summary>
        private bool SaveIfChanged()
        {
            var ok = true;
            if (!settings.DoNotPromptToSave && dockControlBuilder?.FetchChanged == true)
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

        private void SaveML()
        {
            var msg = "Saving {0}...";
            WorkAsync(new WorkAsyncInfo(string.Format(msg, DynML["listname"]),
                (eventargs) =>
                {
                    var xml = dockControlBuilder.GetFetchString(false, false);
                    Entity newView = new Entity(DynML.LogicalName);
                    newView.Id = DynML.Id;
                    newView.Attributes.Add("query", xml);
                    Service.Update(newView);
                    LogUse("SaveML");
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
                        dockControlBuilder.ClearChanged();
                    }
                }
            });
        }

        /// <summary>Saves various configurations to file for next session</summary>
        private void SaveSetting()
        {
            settings.QueryOptions.TreeHeight = dockControlBuilder.SplitterPos;
            SettingsManager.Instance.Save(typeof(FetchXmlBuilder), settings, "[Common]");
            var connsett = new FXBConnectionSettings
            {
                FetchXML = dockControlBuilder.GetFetchString(false, false)
            };
            SettingsManager.Instance.Save(typeof(FetchXmlBuilder), connsett, ConnectionDetail?.ConnectionName);
        }

        private void SaveView()
        {
            var currentAttributes = dockControlBuilder.GetAttributesSignature(null);
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
                    var xml = dockControlBuilder.GetFetchString(false, false);
                    Entity newView = new Entity(View.LogicalName);
                    newView.Id = View.Id;
                    newView.Attributes.Add("fetchxml", xml);
                    Service.Update(newView);
                    LogUse("SaveView");
                    if (View.LogicalName == "savedquery")
                    {
                        var pubRequest = new PublishXmlRequest();
                        pubRequest.ParameterXml = string.Format(
                            @"<importexportxml><entities><entity>{0}</entity></entities><nodes/><securityroles/><settings/><workflows/></importexportxml>",
                            View["returnedtypecode"].ToString());
                        Service.Execute(pubRequest);
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
                        dockControlBuilder.ClearChanged();
                    }
                }
            });
        }

        private void SetupDockControls()
        {
            var dockFile = GetDockFileName();
            if (File.Exists(dockFile))
            {
                try
                {
                    dockContainer.LoadFromXml(dockFile, dockDeSerialization);
                }
                catch
                {
                }
            }
            if (dockControlBuilder == null ||
                dockControlBuilder.DockState == DockState.Hidden ||
                dockControlBuilder.DockState == DockState.Unknown)
            {   // Something fishy, treecontrol should always be visible
                ResetDockLayout();
            }
        }

        private void ShowContentControl(ref XmlContentControl control, ContentType contenttype, SaveFormat save, DockState state)
        {
            LogUse($"Show-{contenttype}");
            if (control?.IsDisposed != false)
            {
                control = new XmlContentControl(contenttype, save, this);
                control.Show(dockContainer, state);
            }
            else
            {
                control.EnsureVisible(dockContainer, state);
            }
            UpdateLiveXML();
        }

        private void ShowOData20Control()
        {
            LogUse("Show-OData2.0");
            if (dockControlOData?.IsDisposed != false)
            {
                dockControlOData = new ODataControl(this);
                dockControlOData.Show(dockContainer, DockState.DockBottom);
            }
            else
            {
                dockControlOData.EnsureVisible(dockContainer, DockState.DockBottom);
            }
            UpdateLiveXML();
        }

        private void ShowResultControl(string content, ContentType contenttype, SaveFormat save, DockState defaultstate)
        {
            if (content.Length > 100000 &&
                MessageBox.Show("Huge result, this may take a while!\n" + content.Length.ToString() + " characters.\n\nContinue?", "Huge result",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
            {
                return;
            }
            LogUse($"Show-{contenttype}");
            var resultControl = dockControlFetchResult;
            if (settings.Results.AlwaysNewWindow)
            {
                resultControl = new XmlContentControl(contenttype, save, this);
                resultpanecount++;
            }
            else if (resultControl?.IsDisposed != false)
            {
                resultControl = new XmlContentControl(contenttype, save, this);
                resultControl.Show(dockContainer, defaultstate);
                dockControlFetchResult = resultControl;
            }
            resultControl.SetContentType(contenttype);
            resultControl.SetFormat(save);
            if (settings.Results.AlwaysNewWindow)
            {
                resultControl.Text += $" ({resultpanecount})";
                resultControl.TabText = resultControl.Text;
            }
            resultControl.EnsureVisible(dockContainer, defaultstate);
            resultControl.UpdateXML(content);
        }

        #endregion Private Methods

        #region Private Event Handlers

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

        private void FetchXmlBuilder_Load(object sender, EventArgs e)
        {
            LoadSetting();
            LogUse("Load");
            SetupDockControls();
            ApplySettings();
            TreeNodeHelper.AddContextMenu(null, dockControlBuilder);
            EnableControls(true);
        }

        private void tmLiveUpdate_Tick(object sender, EventArgs e)
        {
            tmLiveUpdate.Stop();
            if (dockControlFetchXml != null && dockControlFetchXml.chkLiveUpdate.Checked &&
                dockControlFetchXml.txtXML != null && dockControlFetchXml.Visible)
            {
                try
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(dockControlFetchXml.txtXML.Text);
                    if (doc.OuterXml != liveUpdateXml)
                    {
                        dockControlBuilder.ParseXML(dockControlFetchXml.txtXML.Text, false);
                        UpdateLiveXML(true);
                    }
                    liveUpdateXml = doc.OuterXml;
                }
                catch (Exception)
                {
                }
            }
        }

        private void toolStripMain_Click(object sender, EventArgs e)
        {
            dockControlBuilder?.tvFetch?.Focus();
        }

        private void tsbAbout_Click(object sender, EventArgs e)
        {
            LogUse("OpenAbout");
            var about = new About();
            about.StartPosition = FormStartPosition.CenterParent;
            about.lblVersion.Text = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            about.ShowDialog();
        }

        private void tsbCloseThisTab_Click(object sender, EventArgs e)
        {
            CloseTool();
        }

        private void tsbExecute_Click(object sender, EventArgs e)
        {
            dockControlBuilder?.tvFetch?.Focus();
            FetchResults();
        }

        private void tsbNew_Click(object sender, EventArgs e)
        {
            if (!SaveIfChanged())
            {
                return;
            }
            LogUse("New");
            dockControlBuilder.Init(null, "new", false);
        }

        private void tsbOptions_Click(object sender, EventArgs e)
        {
            var allowStats = settings.LogUsage;
            var settingDlg = new Settings(this);
            LogUse("OpenOptions");
            if (settingDlg.ShowDialog(this) == DialogResult.OK)
            {
                LogUse("SaveOptions");
                settings = settingDlg.GetSettings();
                views = null;
                if (allowStats != settings.LogUsage)
                {
                    if (settings.LogUsage == true)
                    {
                        LogUse("Accept", true);
                    }
                    else if (!settings.LogUsage == true)
                    {
                        LogUse("Deny", true);
                    }
                }
                dockControlBuilder.ApplyCurrentSettings();
            }
        }

        private void tsbRedo_Click(object sender, EventArgs e)
        {
            dockControlBuilder.RestoreHistoryPosition(-1);
        }

        private void tsbReturnToCaller_Click(object sender, EventArgs e)
        {
            ReturnToCaller();
        }

        private void tsbUndo_Click(object sender, EventArgs e)
        {
            dockControlBuilder.RestoreHistoryPosition(1);
        }

        private void tsmiOpenCWP_Click(object sender, EventArgs e)
        {
            OpenCWPFeed();
        }

        private void tsmiOpenFile_Click(object sender, EventArgs e)
        {
            OpenFile();
        }

        private void tsmiOpenML_Click(object sender, EventArgs e)
        {
            OpenML();
        }

        private void tsmiOpenView_Click(object sender, EventArgs e)
        {
            OpenView();
        }

        private void tsmiResetWindowLayout_Click(object sender, EventArgs e)
        {
            LogUse("ResetWindows");
            ResetDockLayout();
        }

        private void tsmiSaveCWP_Click(object sender, EventArgs e)
        {
            SaveCWPFeed();
        }

        private void tsmiSaveFile_Click(object sender, EventArgs e)
        {
            SaveFetchXML(false, false);
        }

        private void tsmiSaveFileAs_Click(object sender, EventArgs e)
        {
            SaveFetchXML(true, false);
        }

        private void tsmiSaveML_Click(object sender, EventArgs e)
        {
            SaveML();
        }

        private void tsmiSaveView_Click(object sender, EventArgs e)
        {
            SaveView();
        }

        private void tsmiShowFetchXML_Click(object sender, EventArgs e)
        {
            ShowContentControl(ref dockControlFetchXml, ContentType.FetchXML, SaveFormat.None, settings.DockStates.FetchXML);
        }

        private void tsmiShowFetchXMLcs_Click(object sender, EventArgs e)
        {
            ShowContentControl(ref dockControlFetchXmlCs, ContentType.CSharp_Query, SaveFormat.None, settings.DockStates.FetchXMLCs);
        }

        private void tsmiShowFetchXMLjs_Click(object sender, EventArgs e)
        {
            ShowContentControl(ref dockControlFetchXmlJs, ContentType.JavaScript_Query, SaveFormat.None, settings.DockStates.FetchXMLJs);
        }

        private void tsmiShowOData_Click(object sender, EventArgs e)
        {
            ShowOData20Control();
        }

        private void tsmiShowQueryExpression_Click(object sender, EventArgs e)
        {
            ShowContentControl(ref dockControlQExp, ContentType.QueryExpression, SaveFormat.None, settings.DockStates.QueryExpression);
        }

        private void tsmiShowSQL_Click(object sender, EventArgs e)
        {
            ShowContentControl(ref dockControlSQL, ContentType.SQL_Query, SaveFormat.SQL, settings.DockStates.SQLQuery);
        }

        #endregion Private Event Handlers
    }
}