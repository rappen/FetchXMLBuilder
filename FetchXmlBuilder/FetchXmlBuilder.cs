using Cinteros.Xrm.FetchXmlBuilder.AppCode;
using Cinteros.Xrm.FetchXmlBuilder.DockControls;
using Cinteros.Xrm.FetchXmlBuilder.Forms;
using Cinteros.Xrm.XmlEditorUtils;
using MarkMpn.FetchXmlToWebAPI;
using McTools.Xrm.Connection;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using NuGet.Packaging;
using Rappen.XTB.Helpers.Extensions;
using Rappen.XTB.Helpers.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
using WeifenLuo.WinFormsUI.Docking;
using XrmToolBox;
using XrmToolBox.Extensibility;
using XrmToolBox.Extensibility.Args;
using XrmToolBox.Extensibility.Interfaces;

namespace Cinteros.Xrm.FetchXmlBuilder
{
    public partial class FetchXmlBuilder : PluginControlBase, IGitHubPlugin, IPayPalPlugin, IMessageBusHost, IHelpPlugin, IStatusBarMessenger, IShortcutReceiver, IAboutPlugin, IDuplicatableTool/*, ISettingsPlugin*/
    {
        private const string aiEndpoint = "https://dc.services.visualstudio.com/v2/track";
        private const string aiKey = "eed73022-2444-45fd-928b-5eebd8fa46a6";    // jonas@rappen.net tenant, XrmToolBox
        //private const string aiKey = "b6a4ec7c-ab43-4780-97cd-021e99506337";   // jonas@jonasr.app, XrmToolBoxInsights

        #region Internal Fields

        internal Dictionary<string, EntityMetadata> entities;
        internal static bool friendlyNames = false;
        internal Dictionary<string, List<Entity>> views;
        internal FXBSettings settings = new FXBSettings();
        internal TreeBuilderControl dockControlBuilder;
        internal bool working = false;
        internal Version CDSVersion = new Version();
        internal HistoryManager historyMgr = new HistoryManager();
        internal bool historyisavailable = true;

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
        private ODataControl dockControlOData2;
        private ODataControl dockControlOData4;
        private FlowListControl dockControlFlowList;
        private XmlContentControl dockControlQExp;
        private XmlContentControl dockControlSQL;
        private MetadataControl dockControlMeta;
        private Entity dynml;
        private string fileName;
        private int resultpanecount = 0;
        private Entity view;
        private readonly AppInsights ai;
        private QueryRepository repository = new QueryRepository();
        private bool inSql4Cds;
        private bool bduexists;

        #endregion Private Fields

        #region Public Constructors

        public FetchXmlBuilder()
        {
            InitializeComponent();

            // Tips to handle all errors from
            // https://stackoverflow.com/questions/5762526/how-can-i-make-something-that-catches-all-unhandled-exceptions-in-a-winforms-a
            // Add the event handler for handling non-UI thread exceptions to the event. 
            //AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(Error_UnhandledException);

            ai = new AppInsights(aiEndpoint, aiKey, Assembly.GetExecutingAssembly(), "FetchXML Builder");
            var theme = new VS2015LightTheme();
            dockContainer.Theme = theme;
            dockContainer.Theme.Skin.DockPaneStripSkin.TextFont = Font;
            //dockContainer.DockBackColor = SystemColors.Window;
            MetadataExtensions.attributeProperties = MetadataExtensions.attributeProperties.Union(new string[] {
                "DisplayName",
                "AttributeType",
                "IsValidForRead",
                "AttributeOf",
                "IsManaged",
                "IsCustomizable",
                "IsCustomAttribute",
                "IsValidForAdvancedFind",
                "IsPrimaryId",
                "IsPrimaryName",
                "OptionSet",
                "SchemaName",
                "Targets",
                "IsLogical",
                "EntityLogicalName"
            }).ToArray();
        }

        private void Error_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.IsTerminating.ToString());
            if (e.ExceptionObject is Exception ex)
            {
                LogError($"Unhandling error: {ex}");
                ErrorDetail.ShowDialog(this, e.ExceptionObject as Exception);
            }
            else
            {
                LogError($"Unhandling error: {e}");
                MessageBox.Show("Unhandeled error:\n" + e.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion Public Constructors

        #region Public Events

        public event EventHandler<MessageBusEventArgs> OnOutgoingMessage;

        public event EventHandler<StatusBarMessageEventArgs> SendMessageToStatusBar;

        public event EventHandler<DuplicateToolArgs> DuplicateRequested;

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
            get { return "https://fetchxmlbuilder.com?utm_source=XTBHelp"; }
        }

        public string RepositoryName
        {
            get { return "FetchXMLBuilder"; }
        }

        public string UserName
        {
            get { return "rappen"; }
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
                return;
            }
            SaveDockPanels();
            dockControlBuilder?.Close();
            dockControlFetchXml?.Close();
            dockControlFetchXmlCs?.Close();
            dockControlFetchXmlJs?.Close();
            dockControlFetchResult?.Close();
            dockControlGrid?.Close();
            dockControlOData2?.Close();
            dockControlOData4?.Close();
            dockControlFlowList?.Close();
            dockControlQExp?.Close();
            dockControlSQL?.Close();
            dockControlMeta?.Close();
            SaveSetting();
            LogUse("Close");
        }

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

        public void ShowAboutDialog()
        {
            tslAbout_Click(null, null);
        }

        public void ShowSettings()
        {
            ShowFXBSettings();
        }

        public void ApplyState(object state)
        {
            if (state is string fetch && fetch.ToLowerInvariant().StartsWith("<fetch"))
            {
                dockControlBuilder.Init(fetch, null, false);
            }
        }

        public object GetState()
        {
            return dockControlBuilder?.GetFetchString(false, false);
        }

        #endregion Public Methods

        #region Internal Methods

        internal AttributeMetadata GetAttribute(string entityName, string attributeName)
        {
            if (entities != null && entities.ContainsKey(entityName) && entities[entityName].Attributes is AttributeMetadata[] attrs)
            {
                return attrs.FirstOrDefault(a => a.LogicalName == attributeName);
            }
            return null;
        }

        internal string GetAttributeDisplayName(string entityName, string attributeName)
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

        internal string GetEntityDisplayName(string entityName)
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
            if (friendlyNames)
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
                    tsmiSaveFile.Enabled = enabled && dockControlBuilder?.FetchChanged == true && !string.IsNullOrEmpty(FileName);
                    tsmiSaveFileAs.Enabled = enabled;
                    tsmiSaveView.Enabled = enabled && Service != null && View != null && View.IsCustomizable();
                    tsmiSaveViewAs.Enabled = tsmiSaveView.Enabled;
                    tsmiSaveML.Enabled = enabled && Service != null && DynML != null;
                    tsmiSaveCWP.Visible = enabled && Service != null && entities != null && entities.ContainsKey("cint_feed");
                    tsmiSaveCWP.Enabled = enabled && Service != null && dockControlBuilder?.FetchChanged == true && !string.IsNullOrEmpty(CWPFeed);
                    tsbView.Enabled = enabled;
                    tsbExecute.Enabled = enabled && Service != null;
                    tsmiSelect.Enabled = enabled && Service != null;
                    tsbAbort.Visible = settings.Results.RetrieveAllPages;
                    tsbBDU.Visible = bduexists && callerArgs?.SourcePlugin != "Bulk Data Updater";
                    tsbBDU.Enabled = enabled && (dockControlBuilder?.IsFetchAggregate() == false);
                    dockControlBuilder?.EnableControls(enabled);
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

        internal void EnableDisableHistoryButtons()
        {
            if (historyisavailable)
            {
                historyMgr.SetupUndoButton(tsbUndo);
                historyMgr.SetupRedoButton(tsbRedo);
            }
            else
            {
                tsbUndo.Enabled = false;
                tsbRedo.Enabled = false;
            }
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
            switch (settings.Results.ResultOutput)
            {
                case ResultOutput.Grid:
                case ResultOutput.XML:
                case ResultOutput.JSON:
                case ResultOutput.JSONWebAPI:
                    RetrieveMultiple(fetch);
                    break;

                case ResultOutput.Raw:
                    ExecuteFetch(fetch);
                    break;

                default:
                    MessageBox.Show("Select valid output type under Options.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }

        internal AttributeMetadata[] GetDisplayAttributes(string entityName) => GetDisplayAttributes(entityName, settings.ShowAttributes);

        internal AttributeMetadata[] GetDisplayAttributes(string entityName, ShowMetaTypesAttribute selectattributes)
        {
            var result = new List<AttributeMetadata>();
            if (entities != null && entities.ContainsKey(entityName))
            {
                var attributes = entities[entityName].Attributes;
                if (attributes != null)
                {
                    foreach (var attribute in attributes)
                    {
                        if (!CheckMetadata(selectattributes.IsManaged, attribute.IsManaged)) { continue; }
                        if (!CheckMetadata(selectattributes.IsCustom, attribute.IsCustomAttribute)) { continue; }
                        if (!CheckMetadata(selectattributes.IsCustomizable, attribute.IsCustomizable)) { continue; }
                        if (!CheckMetadata(selectattributes.IsValidForAdvancedFind, attribute.IsValidForAdvancedFind)) { continue; }
                        if (!CheckMetadata(selectattributes.IsAuditEnabled, attribute.IsAuditEnabled)) { continue; }
                        if (!CheckMetadata(selectattributes.IsLogical, attribute.IsLogical)) { continue; }
                        if (!CheckMetadata(selectattributes.IsValidForRead, attribute.IsValidForRead)) { continue; }
                        if (!CheckMetadata(selectattributes.IsValidForGrid, attribute.IsValidForGrid)) { continue; }
                        if (!CheckMetadata(selectattributes.IsFiltered, attribute.IsFilterable)) { continue; }
                        if (!CheckMetadata(selectattributes.IsRetrievable, attribute.IsRetrievable)) { continue; }
                        if (!CheckMetadata(selectattributes.AttributeOf, !string.IsNullOrEmpty(attribute.AttributeOf))) { continue; }
                        result.Add(attribute);
                    }
                }
            }
            return result.ToArray();
        }

        internal string GetPrimaryIdAttribute(string entityName)
        {
            if (entities != null && entities.TryGetValue(entityName, out var entity))
            {
                return entity.PrimaryIdAttribute;
            }

            return null;
        }

        internal Dictionary<string, EntityMetadata> GetDisplayEntities() => GetDisplayEntities(settings.ShowEntities);

        internal Dictionary<string, EntityMetadata> GetDisplayEntities(ShowMetaTypesEntity selectentities)
        {
            var result = new Dictionary<string, EntityMetadata>();
            if (entities != null)
            {
                foreach (var entity in entities)
                {
                    if (!CheckMetadata(selectentities.IsManaged, entity.Value.IsManaged)) { continue; }
                    if (!CheckMetadata(selectentities.IsCustom, entity.Value.IsCustomEntity)) { continue; }
                    if (!CheckMetadata(selectentities.IsCustomizable, entity.Value.IsCustomizable)) { continue; }
                    if (!CheckMetadata(selectentities.IsValidForAdvancedFind, entity.Value.IsValidForAdvancedFind)) { continue; }
                    if (!CheckMetadata(selectentities.IsAuditEnabled, entity.Value.IsAuditEnabled)) { continue; }
                    if (!CheckMetadata(selectentities.IsLogical, entity.Value.IsLogicalEntity)) { continue; }
                    if (!CheckMetadata(selectentities.IsIntersect, entity.Value.IsIntersect)) { continue; }
                    if (!CheckMetadata(selectentities.IsActivity, entity.Value.IsActivity)) { continue; }
                    if (!CheckMetadata(selectentities.IsActivityParty, entity.Value.IsActivityParty)) { continue; }
                    if (!CheckMetadata(selectentities.Virtual, entity.Value.DataProviderId.HasValue)) { continue; }
                    if (!CheckMetadata(selectentities.Ownerships, entity.Value.OwnershipType)) { continue; }
                    result.Add(entity.Key, entity.Value);
                }
            }
            return result;
        }

        private static bool CheckMetadata(CheckState checkstate, bool? metafield)
        {
            if (metafield != null & metafield.HasValue)
            {
                switch (checkstate)
                {
                    case CheckState.Checked:
                        return metafield.Value == true;
                    case CheckState.Unchecked:
                        return metafield.Value == false;
                }
            }
            return true;
        }

        private static bool CheckMetadata(CheckState checkstate, BooleanManagedProperty metafield)
        {
            return CheckMetadata(checkstate, metafield?.Value);
        }

        private static bool CheckMetadata(OwnershipTypes[] options, OwnershipTypes? metafield)
        {
            if (options != null && options.Length > 0)
            {
                return metafield != null && options.Contains(metafield.Value);
            }
            return true;
        }

        internal string GetOData(int version)
        {
            try
            {
                if (Service == null || ConnectionDetail == null || ConnectionDetail.OrganizationDataServiceUrl == null)
                {
                    throw new Exception("Must have an active connection to CRM to compose OData query.");
                }
                var odata = string.Empty;
                switch (version)
                {
                    case 2:
                        odata = ODataCodeGenerator.GetODataQuery(dockControlBuilder.GetFetchType(), ConnectionDetail.OrganizationDataServiceUrl, this);
                        break;

                    case 4:
                        // Find correct WebAPI base url
                        var baseUrl = ConnectionDetail.WebApplicationUrl;
                        if (!baseUrl.EndsWith("/"))
                            baseUrl += "/";
                        var url = new Uri(new Uri(baseUrl), $"api/data/v{ConnectionDetail.OrganizationMajorVersion}.{ConnectionDetail.OrganizationMinorVersion}");
                        var converter = new FetchXmlToWebAPIConverter(new WebAPIMetadataProvider(this), url.ToString());
                        odata = converter.ConvertFetchXmlToWebAPI(dockControlBuilder.GetFetchString(false, false));
                        break;
                }
                return odata;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        internal void LoadEntityDetails(string entityName, Action detailsLoaded, bool async = true, bool update = true)
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
                        eventargs.Result = MetadataExtensions.LoadEntityDetails(Service, entityName);
                    })
                {
                    PostWorkCallBack = (completedargs) =>
                    {
                        LoadEntityDetailsCompleted(entityName, completedargs.Error == null ? completedargs.Result as EntityMetadata : null, completedargs.Error, update);
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
                    var resp = MetadataExtensions.LoadEntityDetails(Service, entityName);
                    LoadEntityDetailsCompleted(entityName, resp, null, update);
                }
                catch (Exception e)
                {
                    LoadEntityDetailsCompleted(entityName, null, e, update);
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
                    qexs.ColumnSet = new ColumnSet("name", "returnedtypecode", "fetchxml", "layoutxml", "iscustomizable");
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
                    qexu.ColumnSet = new ColumnSet("name", "returnedtypecode", "fetchxml", "layoutxml");
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
                        ErrorDetail.ShowDialog(this, completedargs.Error);
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
            if (dockControlOData2?.Visible == true && entities != null)
            {
                dockControlOData2.DisplayOData(GetOData(2));
            }
            if (dockControlOData4?.Visible == true && entities != null)
            {
                dockControlOData4.DisplayOData(GetOData(4));
            }
            if (dockControlFlowList?.Visible == true && entities != null)
            {
                dockControlFlowList.DisplayFlowList(GetOData(4));
            }
            if (dockControlQExp?.Visible == true && entities != null)
            {
                dockControlQExp.UpdateXML(GetQueryExpressionCode());
            }
            if (dockControlSQL?.Visible == true && entities != null)
            {
                var sql = GetSQLQuery(out var sql4cds);
                dockControlSQL.UpdateSQL(sql, sql4cds);
            }
            if (dockControlFetchXmlCs?.Visible == true)
            {
                dockControlFetchXmlCs.UpdateXML(GetCSharpCode());
            }
            if (dockControlFetchXmlJs?.Visible == true)
            {
                dockControlFetchXmlJs.UpdateXML(GetJavaScriptCode());
            }
            if (dockControlMeta?.Visible == true)
            {
                dockControlMeta.UpdateMeta(dockControlBuilder.SelectedMetadata());
            }
        }

        internal void ShowMetadata(MetadataBase meta)
        {
            if (dockControlMeta?.Visible == true)
            {
                dockControlMeta.UpdateMeta(meta);
            }
        }

        internal void QueryExpressionToFetchXml(string query)
        {
            working = true;
            LogUse("QueryExpressionToFetchXml");
            WorkAsync(new WorkAsyncInfo("Translating QueryExpression to FetchXML...",
                (eventargs) =>
                {
                    var start = DateTime.Now;
                    string fetchXml = QueryExpressionCodeGenerator.GetFetchXmlFromCSharpQueryExpression(query, Service);
                    var stop = DateTime.Now;
                    var duration = stop - start;
                    LogUse("QueryExpressionToFetchXml", false, null, duration.TotalMilliseconds);
                    SendMessageToStatusBar(this, new StatusBarMessageEventArgs($"Execution time: {duration}"));
                    eventargs.Result = fetchXml;
                })
            {
                PostWorkCallBack = (completedargs) =>
                {
                    if (completedargs.Error != null)
                    {
                        ErrorDetail.ShowDialog(this, completedargs.Error, "Parse QueryExpression");
                    }
                    else if (completedargs.Result is string)
                    {
                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml(completedargs.Result.ToString());
                        dockControlBuilder.Init(doc.OuterXml, "parse QueryExpression", true);
                    }
                    working = false;
                }
            });
        }

        internal static void HelpClick(object sender)
        {
            if (sender is Control control && control.Tag is string tag && tag.StartsWith("http"))
            {
                OpenURL(tag);
            }
        }

        internal static void OpenURL(string url)
        {
            url = Utils.ProcessURL(url);
            Process.Start(url);
        }

        #endregion Internal Methods

        #region Private Methods

        private static string GetDockFileName()
        {
            return Path.Combine(Paths.SettingsPath, "Cinteros.Xrm.FetchXmlBuilder_[DockPanels].xml");
        }

        private void ApplySettings(bool reloadquery)
        {
            var connsett = GetConnectionSetting();
            toolStripMain.Items.OfType<ToolStripItem>().ToList().ForEach(i => i.DisplayStyle = settings.ShowButtonTexts ? ToolStripItemDisplayStyle.ImageAndText : ToolStripItemDisplayStyle.Image);
            tsbRepo.Visible = settings.ShowRepository;
            if (reloadquery && connsett != null && !string.IsNullOrWhiteSpace(connsett.FetchXML))
            {
                dockControlBuilder.Init(connsett.FetchXML, "loaded from last session", false);
            }
            dockControlBuilder.lblQAExpander.GroupBoxSetState(null, settings.QueryOptions.ShowQuickActions);
            var ass = Assembly.GetExecutingAssembly().GetName();
            var version = ass.Version.ToString();
            if (!version.Equals(settings.CurrentVersion))
            {
                // Reset some settings when new version is deployed
                settings.CurrentVersion = version;
                SaveSetting();
                LogUse("ShowWelcome");
                Welcome.ShowWelcome(this);
            }
        }

        private bool CallerWantsResults()
        {
            return callerArgs != null;
        }

        private void CheckIntegrationTools()
        {
            bduexists = PluginManagerExtended.Instance.Plugins.Any(p =>
                p.Metadata.Name == "Bulk Data Updater" && new Version(p.Value.GetVersion()) > new Version(1, 2020, 12, 4));
        }

        private void CreateRepoMenuItem(QueryDefinition query)
        {
            ToolStripDropDownItem folder = tsbRepo;
            var nameparts = query.Name.Split('\\');
            for (var i = 0; i < nameparts.Length - 1; i++)
            {
                var foldername = nameparts[i];
                folder = GetMenuFolder(folder, foldername);
            }
            var name = nameparts[nameparts.Length - 1];
            var menu = new ToolStripMenuItem(name) { Tag = query };
            menu.Click += tsmiRepoOpen_Click;
            folder.DropDownItems.Add(menu);
        }

        private ToolStripMenuItem GetMenuFolder(ToolStripDropDownItem parent, string label)
        {
            var result = parent.DropDownItems.Cast<ToolStripItem>().FirstOrDefault(m => m.Text == label && m.Tag as string == "folder") as ToolStripMenuItem;
            if (result == null)
            {
                result = new ToolStripMenuItem(label) { Tag = "folder" };
                parent.DropDownItems.Add(result);
            }
            return result;
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
                    LogUse("ExecuteFetch", false, null, duration.TotalMilliseconds);
                    SendMessageToStatusBar(this, new StatusBarMessageEventArgs($"Execution time: {duration}"));
                    eventargs.Result = resp.FetchXmlResult;
                })
            {
                PostWorkCallBack = (completedargs) =>
                {
                    working = false;
                    if (completedargs.Error != null)
                    {
                        ErrorDetail.ShowDialog(this, completedargs.Error);
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

        private string GetSQLQuery(out bool sql4cds)
        {
            sql4cds = false;

            if (settings.UseSQL4CDS)
            {
                var param = new Dictionary<string, object>
                {
                    ["FetchXml"] = dockControlBuilder.GetFetchString(false, false),
                    ["ConvertOnly"] = true
                };

                inSql4Cds = true;
                OnOutgoingMessage(this, new MessageBusEventArgs("SQL 4 CDS") { TargetArgument = param });
                inSql4Cds = false;

                if (param.TryGetValue("Sql", out var s))
                {
                    sql4cds = true;
                    return (string)s;
                }
            }

            var sql = string.Empty;
            var fetch = dockControlBuilder.GetFetchType();
            try
            {
                sql = SQLQueryGenerator.GetSQLQuery(fetch, this);
            }
            catch (Exception ex)
            {
                sql = "Failed to generate SQL Query.\n\n" + ex.Message;
            }
            return sql;
        }

        public void EditInSQL4CDS()
        {
            OnOutgoingMessage(this, new MessageBusEventArgs("SQL 4 CDS") { TargetArgument = dockControlBuilder.GetFetchString(false, false) });
        }

        private string GetCSharpCode()
        {
            var cs = string.Empty;
            var fetch = dockControlBuilder.GetFetchString(true, false);
            try
            {
                cs = CSharpCodeGenerator.GetCSharpCode(fetch);
            }
            catch (Exception ex)
            {
                cs = "Failed to generate C# code.\n\n" + ex.Message;
            }
            return cs;
        }

        private string GetJavaScriptCode()
        {
            var js = string.Empty;
            var fetch = dockControlBuilder.GetFetchString(true, false);
            try
            {
                js = JavascriptCodeGenerator.GetJavascriptCode(fetch);
            }
            catch (Exception ex)
            {
                js = "Failed to generate JavaScript code.\n\n" + ex.Message;
            }
            return js;
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

                    //if (ConnectionDetail.MetadataCacheLoader != null)
                    //{
                    //    var loader = ConnectionDetail.MetadataCacheLoader;
                    //    loader.ContinueWith(task =>
                    //    {
                    //        entities = ConnectionDetail.MetadataCache.ToDictionary(e => e.LogicalName);
                    //    });
                    //}

                    eventargs.Result = Service.LoadEntities(ConnectionDetail.OrganizationMajorVersion, ConnectionDetail.OrganizationMinorVersion);
                })
            {
                PostWorkCallBack = (completedargs) =>
                {
                    if (completedargs.Error != null)
                    {
                        ErrorDetail.ShowDialog(this, completedargs.Error, "Load Entities");
                    }
                    else
                    {
                        if (completedargs.Result is RetrieveMetadataChangesResponse meta)
                        {
                            entities = new Dictionary<string, EntityMetadata>();
                            entities.AddRange(meta.EntityMetadata.ToDictionary(e => e.LogicalName));
                        }
                    }
                    UpdateLiveXML();
                    working = false;
                    EnableControls(true);
                    dockControlBuilder.ApplyCurrentSettings();
                }
            });
        }

        private void LoadEntityDetailsCompleted(string entityName, EntityMetadata Result, Exception Error, bool update)
        {
            if (Error != null)
            {
                entityShitList.Add(entityName);
                MessageBox.Show(Error.Message, "Load attribute metadata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                if (Result != null)
                {
                    if (entities.ContainsKey(entityName))
                    {
                        entities[entityName] = Result;
                    }
                    else
                    {
                        entities.Add(entityName, Result);
                    }
                }
                else
                {
                    entityShitList.Add(entityName);
                    MessageBox.Show("Metadata not found for entity " + entityName, "Load attribute metadata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                working = false;

                if (update)
                {
                    dockControlBuilder.UpdateAllNode();
                    UpdateLiveXML();
                }
            }
            working = false;
        }

        /// <summary>Loads configurations from file</summary>
        private void LoadSetting()
        {
            settings = null;
            try
            {
                SettingsManager.Instance.TryLoad(typeof(FetchXmlBuilder), out settings, "[Common]");
            }
            catch (InvalidOperationException) { }
            if (settings == null)
            {
                settings = new FXBSettings();
            }
            repository = null;
            try
            {
                SettingsManager.Instance.TryLoad(typeof(FetchXmlBuilder), out repository, "[QueryRepository]");
            }
            catch (InvalidOperationException) { }
            if (repository == null)
            {
                repository = new QueryRepository();
            }
            repository.SortQueries();
        }

        private FXBConnectionSettings GetConnectionSetting()
        {
            try
            {
                if (SettingsManager.Instance.TryLoad(typeof(FetchXmlBuilder), out FXBConnectionSettings connsett, ConnectionDetail?.ConnectionName))
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

        private void RebuildRepositoryMenu(QueryDefinition selectedquery)
        {
            tsbRepo.Tag = selectedquery;
            dockControlBuilder.SetFetchName(selectedquery != null ? $"Repo: {selectedquery.Name}" : null);
            var oldqueries = tsbRepo.DropDownItems.Cast<ToolStripItem>().Where(m => m.Tag is QueryDefinition || m.Tag?.ToString() == "folder").ToList();
            foreach (var oldmenu in oldqueries)
            {
                tsbRepo.DropDownItems.Remove(oldmenu);
            }
            foreach (var query in repository.Queries)
            {
                CreateRepoMenuItem(query);
            }
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
            LogUse("RetrieveMultiple-" + settings.Results.ResultOutput.ToString());
            SendMessageToStatusBar(this, new StatusBarMessageEventArgs("Retrieving..."));
            tsbAbort.Enabled = true;
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Executing FetchXML...",
                IsCancelable = true,
                Work = (worker, eventargs) =>
                {
                    QueryBase query = new FetchExpression(fetch);
                    var attributessignature = dockControlBuilder.GetAttributesSignature(null);
                    var start = DateTime.Now;
                    EntityCollection resultCollection = null;
                    EntityCollection tmpResult = null;
                    var page = 0;
                    do
                    {
                        if (worker.CancellationPending)
                        {
                            eventargs.Cancel = true;
                            break;
                        }
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
                        if (settings.Results.RetrieveAllPages && tmpResult.MoreRecords)
                        {
                            if (query is QueryExpression qex)
                            {
                                qex.PageInfo.PageNumber++;
                                qex.PageInfo.PagingCookie = tmpResult.PagingCookie;
                            }
                            else if (query is FetchExpression fex && fex.Query is string pagefetch)
                            {
                                var pagedoc = new XmlDocument();
                                pagedoc.LoadXml(pagefetch);
                                if (pagedoc.SelectSingleNode("fetch") is XmlElement fetchnode)
                                {
                                    if (!int.TryParse(fetchnode.GetAttribute("page"), out int pageno))
                                    {
                                        pageno = 1;
                                    }
                                    pageno++;
                                    fetchnode.SetAttribute("page", pageno.ToString());
                                    fetchnode.SetAttribute("pagingcookie", tmpResult.PagingCookie);
                                    query = new FetchExpression(pagedoc.OuterXml);
                                }
                            }
                            else
                            {
                                MessageBox.Show("Unable to retrieve more pages, unexpected query.", "Retrieve all pages", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                break;
                            }
                        }
                        page++;
                        var duration = DateTime.Now - start;
                        var pageinfo = page == 1 ? "first page" : $"{page} pages";
                        worker.ReportProgress(0, $"Retrieved {pageinfo} in {duration.TotalSeconds:F2} sec");
                        SendMessageToStatusBar(this, new StatusBarMessageEventArgs($"Retrieved {resultCollection.Entities.Count} records on {pageinfo} in {duration.TotalSeconds:F2} seconds"));
                    }
                    while (!eventargs.Cancel && settings.Results.RetrieveAllPages && (query is QueryExpression || query is FetchExpression) && tmpResult.MoreRecords);
                    LogUse("RetrieveMultiple", false, resultCollection?.Entities?.Count, (DateTime.Now - start).TotalMilliseconds);
                    if (settings.Results.ResultOutput == ResultOutput.JSON)
                    {
                        var json = EntityCollectionSerializer.ToJSONComplex(resultCollection, Formatting.Indented);
                        eventargs.Result = json;
                    }
                    else if (settings.Results.ResultOutput == ResultOutput.JSONWebAPI)
                    {
                        var json = EntityCollectionSerializer.ToJSONSimple(resultCollection, Formatting.Indented);
                        eventargs.Result = json;
                    }
                    else
                    {
                        eventargs.Result = new QueryInfo
                        {
                            Query = query,
                            AttributesSignature = attributessignature,
                            Results = resultCollection
                        };
                    }
                },
                ProgressChanged = (changeargs) =>
                {
                    SetWorkingMessage(changeargs.UserState.ToString());
                },
                PostWorkCallBack = (completedargs) =>
                {
                    working = false;
                    tsbAbort.Enabled = false;
                    if (completedargs.Error != null)
                    {
                        LogError("RetrieveMultiple error: {0}", completedargs.Error);
                        ErrorDetail.ShowDialog(this, completedargs.Error);
                    }
                    else if (completedargs.Cancelled)
                    {
                        MessageBox.Show($"Manual abort.", "Execute", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    else if (completedargs.Result is QueryInfo queryinfo)
                    {
                        switch (settings.Results.ResultOutput)
                        {
                            case ResultOutput.Grid:
                                if (settings.Results.AlwaysNewWindow)
                                {
                                    var newresults = new ResultGrid(this);
                                    resultpanecount++;
                                    newresults.Text = $"Results ({resultpanecount})";
                                    newresults.Show(dockContainer, settings.DockStates.ResultView);
                                    newresults.SetData(queryinfo);
                                }
                                else
                                {
                                    if (dockControlGrid?.IsDisposed != false)
                                    {
                                        dockControlGrid = new ResultGrid(this);
                                        dockControlGrid.Show(dockContainer, settings.DockStates.ResultView);
                                    }
                                    dockControlGrid.SetData(queryinfo);
                                    dockControlGrid.Activate();
                                }
                                break;

                            case ResultOutput.XML:
                                var serialized = EntityCollectionSerializer.Serialize(queryinfo.Results, SerializationStyle.Explicit);
                                ShowResultControl(serialized.OuterXml, ContentType.Serialized_Result_XML, SaveFormat.XML, settings.DockStates.FetchResult);
                                break;
                        }
                    }
                    else if ((settings.Results.ResultOutput == ResultOutput.JSON || settings.Results.ResultOutput == ResultOutput.JSONWebAPI) && completedargs.Result is string json)
                    {
                        ShowResultControl(json, ContentType.Serialized_Result_JSON, SaveFormat.JSON, settings.DockStates.FetchResult);
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
            var newfile = prompt ? "" : FileName;
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
                        ErrorDetail.ShowDialog(this, completedargs.Error, "Save Marketing List");
                    }
                    else
                    {
                        dockControlBuilder.ClearChanged();
                    }
                }
            });
        }

        private void SaveRepository()
        {
            SettingsManager.Instance.Save(typeof(FetchXmlBuilder), repository, "[QueryRepository]");
        }

        /// <summary>Saves various configurations to file for next session</summary>
        private void SaveSetting()
        {
            SettingsManager.Instance.Save(typeof(FetchXmlBuilder), settings, "[Common]");
            var connsett = new FXBConnectionSettings
            {
                FetchXML = dockControlBuilder.GetFetchString(false, false)
            };
            SettingsManager.Instance.Save(typeof(FetchXmlBuilder), connsett, ConnectionDetail?.ConnectionName);
        }

        private void SaveView(bool saveas)
        {
            var currentAttributes = dockControlBuilder.GetAttributesSignature(null);
            if (currentAttributes != attributesChecksum)
            {
                MessageBox.Show("Cannot save view, returned attributes must not be changed.\n\nExpected attributes:\n  " +
                    attributesChecksum.Replace("\n", "\n  ") + "\nCurrent attributes:\n  " + currentAttributes.Replace("\n", "\n  "),
                    "Cannot save view", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var viewname = View["name"].ToString();
            if (saveas)
            {
                var newviewname = Prompt.ShowDialog("Enter name for the new personal view", "Save View As", viewname);
                if (string.IsNullOrEmpty(newviewname))
                {
                    MessageBox.Show("No name for new view.", "Save View As", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (newviewname.ToLowerInvariant() == viewname.ToLowerInvariant())
                {
                    MessageBox.Show("Enter a new name for this view.", "Save View As", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                viewname = newviewname;
            }
            else if (View.LogicalName == "savedquery")
            {
                if (MessageBox.Show("This will update and publish the saved query in CRM.\n\nConfirm!", "Confirm",
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.Cancel)
                {
                    return;
                }
            }
            var xml = dockControlBuilder.GetFetchString(false, false);
            var entityname = View["returnedtypecode"].ToString();
            var newView = new Entity(saveas ? "userquery" : View.LogicalName);
            newView["fetchxml"] = xml;
            if (saveas)
            {
                newView["name"] = viewname;
                newView["layoutxml"] = View["layoutxml"];
                newView["returnedtypecode"] = View["returnedtypecode"];
                newView["querytype"] = 0;
            }
            else
            {
                newView.Id = View.Id;
            }

            var msg = newView.LogicalName == "savedquery" ? "Saving and publishing {0}..." : "Saving {0}...";
            WorkAsync(new WorkAsyncInfo(string.Format(msg, viewname),
                (worker, eventargs) =>
                {
                    if (newView.Id.Equals(Guid.Empty))
                    {
                        newView.Id = Service.Create(newView);
                        eventargs.Result = newView;
                    }
                    else
                    {
                        Service.Update(newView);
                    }
                    LogUse("SaveView");
                    if (newView.LogicalName == "savedquery")
                    {
                        var pubRequest = new PublishXmlRequest();
                        pubRequest.ParameterXml = string.Format(
                            @"<importexportxml><entities><entity>{0}</entity></entities><nodes/><securityroles/><settings/><workflows/></importexportxml>",
                            View["returnedtypecode"].ToString());
                        Service.Execute(pubRequest);
                    }
                })
            {
                PostWorkCallBack = (completedargs) =>
                {
                    if (completedargs.Error != null)
                    {
                        ErrorDetail.ShowDialog(this, completedargs.Error, "Save view");
                    }
                    else
                    {
                        if (completedargs.Result is Entity newview)
                        {
                            entityname = newview["returnedtypecode"].ToString();
                            if (!views.ContainsKey(entityname + "|U"))
                            {
                                views.Add(entityname + "|U", new List<Entity>());
                            }
                            views[entityname + "|U"].Add(newView);
                            View = newview;
                        }
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

        internal void ShowMetadataControl()
        {
            ShowMetadataControl(ref dockControlMeta, DockState.DockRight);
        }

        private void ShowMetadataControl(ref MetadataControl control, DockState defaultstate)
        {
            LogUse($"Show-Metadata");
            if (control?.IsDisposed != false)
            {
                control = new MetadataControl(this);
                control.Show(dockContainer, defaultstate);
            }
            else
            {
                control.EnsureVisible(dockContainer, defaultstate);
            }
            UpdateLiveXML();
        }

        private void ShowFXBSettings()
        {
            var settingDlg = new Settings(this);
            LogUse("OpenOptions");
            if (settingDlg.ShowDialog(this) == DialogResult.OK)
            {
                LogUse("SaveOptions");
                settings = settingDlg.GetSettings();
                views = null;
                ApplySettings(false);
                dockControlBuilder.ApplyCurrentSettings();
                dockControlFetchXml?.ApplyCurrentSettings();
                EnableControls();
            }
        }

        internal void ShowSelectSettings()
        {
            if (ShowMetadataOptions.Show(this))
            {
                LogUse("SaveSelect");
                SaveSetting();
                ApplySettings(false);
                dockControlBuilder.ApplyCurrentSettings();
                dockControlFetchXml?.ApplyCurrentSettings();
                EnableControls();
            }
        }

        private void ShowODataControl(ref ODataControl control, int version)
        {
            LogUse($"Show-OData{version}.0");
            if (control?.IsDisposed != false)
            {
                control = new ODataControl(this, version);
                control.Show(dockContainer, DockState.DockBottom);
            }
            else
            {
                control.EnsureVisible(dockContainer, DockState.DockBottom);
            }
            UpdateLiveXML();
        }

        private void ShowFlowListControl(ref FlowListControl control, DockState defaultstate)
        {
            LogUse($"Show-FlowList");
            if (control?.IsDisposed != false)
            {
                control = new FlowListControl(this);
                var defaultfloatsize = dockContainer.DefaultFloatWindowSize;
                dockContainer.DefaultFloatWindowSize = dockControlFlowList.Size;
                control.Show(dockContainer, defaultstate);
                dockContainer.DefaultFloatWindowSize = defaultfloatsize;
            }
            else
            {
                control.EnsureVisible(dockContainer, defaultstate);
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
            CDSVersion = new Version(e.ConnectionDetail.OrganizationVersion);
            LogInfo("Connected CRM version: {0} (Major: {1} Minor: {2})",
                CDSVersion, e.ConnectionDetail.OrganizationMajorVersion, e.ConnectionDetail.OrganizationMinorVersion);
            // Verifying version where MetadataChanges request exists https://msdn.microsoft.com/en-us/library/jj863599(v=crm.5).aspx
            // According to TechNet 2011 UR12 is 05.00.9690.3218 https://social.technet.microsoft.com/wiki/contents/articles/8062.crm-2011-build-and-version-numbers-for-update-rollups.aspx
            var orgok = CDSVersion >= new Version(05, 00, 9690, 3218);
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
                MessageBox.Show($"RetrieveMetadataChangesRequest was introduced in\nMicrosoft Dynamics CRM 2011 UR12 (5.0.9690.3218)\nCurrent version is {CDSVersion}\n\nPlease connect to a newer organization to use this cool tool.",
                    "Organization too old", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            EnableControls(orgok && buttonsEnabled);
        }

        private void FetchXmlBuilder_Load(object sender, EventArgs e)
        {
            LoadSetting();
            LogUse("Load");
            CheckIntegrationTools();
            SetupDockControls();
            ApplySettings(true);
            RebuildRepositoryMenu(null);
            TreeNodeHelper.AddContextMenu(null, dockControlBuilder);
            EnableControls(true);
        }

        private void toolStripMain_Click(object sender, EventArgs e)
        {
            dockControlBuilder?.tvFetch?.Focus();
        }

        private void tslAbout_Click(object sender, EventArgs e)
        {
            LogUse("OpenAbout");
            var about = new About();
            about.StartPosition = FormStartPosition.CenterParent;
            about.lblVersion.Text = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            about.ShowDialog();
        }

        private void tsbAbort_Click(object sender, EventArgs e)
        {
            tsbAbort.Enabled = false;
            CancelWorker();
        }

        private void tsbClone_Click(object sender, EventArgs e)
        {
            var query = dockControlBuilder.GetFetchString(false, false);
            var newconnection = sender == tsmiCloneNewConnection;
            LogUse(newconnection ? "Clone-Connect" : "Clone");
            DuplicateRequested?.Invoke(this, new DuplicateToolArgs(query, newconnection));
        }

        private void tsbExecute_Click(object sender, EventArgs e)
        {
            dockControlBuilder?.tvFetch?.Focus();
            FetchResults();
        }

        private void tsbNew_Click(object sender, EventArgs e)
        {
            if (sender == tsmiNew)
            {
                if (!SaveIfChanged())
                {
                    return;
                }
                LogUse("New");
                dockControlBuilder.Init(null, "new", false);
                return;
            }
            var newconnection = sender == tsmiNewNewConnection;
            LogUse(newconnection ? "New-NewConnection" : "New-New");
            DuplicateRequested?.Invoke(this, new DuplicateToolArgs(settings.QueryOptions.NewQueryTemplate, newconnection));
        }

        private void tsbOptions_Click(object sender, EventArgs e)
        {
            ShowFXBSettings();
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
            SaveView(sender == tsmiSaveViewAs);
        }

        private void tsmiShowFetchXML_Click(object sender, EventArgs e)
        {
            ShowContentControl(ref dockControlFetchXml, ContentType.FetchXML, SaveFormat.None, settings.DockStates.FetchXML);
        }

        private void tsmiShowMetadata_Click(object sender, EventArgs e)
        {
            ShowMetadataControl(ref dockControlMeta, DockState.DockRight);
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
            ShowODataControl(ref dockControlOData2, 2);
        }

        private void tsmiShowOData4_Click(object sender, EventArgs e)
        {
            ShowODataControl(ref dockControlOData4, 4);
        }

        private void tsmiShowFlow_Click(object sender, EventArgs e)
        {
            ShowFlowListControl(ref dockControlFlowList, settings.DockStates.FlowList);
        }

        private void tsmiShowQueryExpression_Click(object sender, EventArgs e)
        {
            ShowContentControl(ref dockControlQExp, ContentType.QueryExpression, SaveFormat.None, settings.DockStates.QueryExpression);
        }

        private void tsmiShowSQL_Click(object sender, EventArgs e)
        {
            ShowContentControl(ref dockControlSQL, ContentType.SQL_Query, SaveFormat.SQL, settings.DockStates.SQLQuery);
        }

        private void tsbRepo_DropDownOpening(object sender, EventArgs e)
        {
            var query = tsbRepo.Tag as QueryDefinition;
            tsmiRepoSave.Text = $"Update {query?.Name}";
            tsmiRepoDelete.Text = $"Delete {query?.Name}";
            tsmiRepoSave.Enabled = query != null;
            tsmiRepoDelete.Enabled = query != null;
        }

        private void tsmiRepoDelete_Click(object sender, EventArgs e)
        {
            if (!(tsbRepo.Tag is QueryDefinition query))
            {
                return;
            }
            if (MessageBox.Show($"Confirm delete query {query.Name} from repository", "Confirm", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) != DialogResult.OK)
            {
                return;
            }
            repository.Queries.Remove(query);
            SaveRepository();
            RebuildRepositoryMenu(null);
        }

        private void tsmiRepoOpen_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem menu && menu.Tag is QueryDefinition query)
            {
                dockControlBuilder.Init(query.Fetch, $"open repo {query.Name}", false);
                tsbRepo.Tag = query;
                dockControlBuilder.SetFetchName($"Repo: {query.Name}");
            }
        }

        private void tsmiRepoSave_Click(object sender, EventArgs e)
        {
            if (!(tsbRepo.Tag is QueryDefinition query))
            {
                return;
            }
            query.Fetch = dockControlBuilder.GetFetchString(true, false);
            SaveRepository();
            MessageBox.Show($"Query {query.Name} updated in repository", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void tsmiRepoSaveAs_Click(object sender, EventArgs e)
        {
            if (!(Prompt.ShowDialog("Enter name for the query. Use backslashes \\ to create folder structure.", "Save Query")?.Trim() is string queryname))
            {
                return;
            }
            if (string.IsNullOrEmpty(queryname))
            {
                MessageBox.Show("No name for query.", "Save Query", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (repository.Queries.Any(q => q.Name == queryname))
            {
                if (MessageBox.Show($"Query {queryname} already exists.\nOverwrite?", "Save Query", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) != DialogResult.Yes)
                {
                    return;
                }
                repository.Queries.Remove(repository.Queries.FirstOrDefault(q => q.Name == queryname));
            }
            var fetch = dockControlBuilder.GetFetchString(true, false);
            var query = new QueryDefinition { Name = queryname, Fetch = fetch };
            repository.Queries.Add(query);
            repository.SortQueries();
            SaveRepository();
            RebuildRepositoryMenu(query);
            MessageBox.Show($"Query {query.Name} saved in repository", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void tsmiRepoExport_Click(object sender, EventArgs e)
        {
            var sfd = new SaveFileDialog
            {
                Title = "Select a location and file name to save the repository",
                Filter = "FXB Repository (*.fxbrepo)|*.fxbrepo"
            };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                McTools.Xrm.Connection.XmlSerializerHelper.SerializeToFile(repository, sfd.FileName);
                MessageBox.Show($"The entire repository has been saved to file\n{sfd.FileName}", "Export repository", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void tsmiRepoImport_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                Title = "Select a FetchXML Builder Repository file",
                Filter = "FXB Repository (*.fxbrepo)|*.fxbrepo"
            };

            if (ofd.ShowDialog() == DialogResult.OK && File.Exists(ofd.FileName))
            {
                try
                {
                    var document = new XmlDocument();
                    document.Load(ofd.FileName);
                    var repo = (QueryRepository)McTools.Xrm.Connection.XmlSerializerHelper.Deserialize(document.OuterXml, typeof(QueryRepository));
                    var reponame = Path.ChangeExtension(Path.GetFileName(ofd.FileName), "").Trim('.');
                    if (MessageBox.Show($"Confirm importing {repo.Queries.Count} queries into repository folder \"{reponame}\".", "Confirm", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
                    {
                        return;
                    }
                    repo.Queries.ForEach(q => repository.Queries.Add(new QueryDefinition { Name = reponame + "\\" + q.Name, Fetch = q.Fetch }));
                    SaveRepository();
                    RebuildRepositoryMenu(null);
                    MessageBox.Show($"Repository {reponame} has been imported.", "Import repository", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error attempting to load and deserialize file \"{ofd.FileName}\"", ex);
                }
            }
        }

        private void tsmiRepoDeleteAll_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show($"Confirm deleting all {repository.Queries.Count} queries in the repository!\nThis can not be undone.", "Confirm", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
            {
                return;
            }
            repository.Queries.Clear();
            SaveRepository();
            RebuildRepositoryMenu(null);
        }

        private void tsbBDU_Click(object sender, EventArgs e)
        {
            OnOutgoingMessage(this, new MessageBusEventArgs("Bulk Data Updater", true) { TargetArgument = dockControlBuilder.GetFetchString(true, true) });
            //OnOutgoingMessage(this, new MessageBusEventArgs(XrmToolBoxToolIds.BulkDataUpdater.ToString(), true) { TargetArgument = dockControlBuilder.GetFetchString(true, true) });
        }

        private void tsmiSelect_Click(object sender, EventArgs e)
        {
            ShowSelectSettings();
        }

        #endregion Private Event Handlers
    }
}