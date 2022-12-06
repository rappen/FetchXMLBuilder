using MarkMpn.FetchXmlToWebAPI;
using McTools.Xrm.Connection;
using Microsoft.Xrm.Sdk;
using Rappen.XRM.Helpers.Extensions;
using Rappen.XRM.Helpers.FetchXML;
using Rappen.XTB.FetchXmlBuilder.AppCode;
using Rappen.XTB.FetchXmlBuilder.Builder;
using Rappen.XTB.FetchXmlBuilder.Converters;
using Rappen.XTB.FetchXmlBuilder.DockControls;
using Rappen.XTB.FetchXmlBuilder.Extensions;
using Rappen.XTB.FetchXmlBuilder.Forms;
using Rappen.XTB.FetchXmlBuilder.Settings;
using Rappen.XTB.XmlEditorUtils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
using WeifenLuo.WinFormsUI.Docking;
using XrmToolBox.Extensibility;
using XrmToolBox.Extensibility.Args;
using Entity = Microsoft.Xrm.Sdk.Entity;

namespace Rappen.XTB.FetchXmlBuilder
{
    public partial class FetchXmlBuilder : PluginControlBase
    {
        #region AI to log

        private const string aiEndpoint = "https://dc.services.visualstudio.com/v2/track";
        private const string aiKey = "eed73022-2444-45fd-928b-5eebd8fa46a6";    // jonas@rappen.net tenant, XrmToolBox

        //private const string aiKey = "b6a4ec7c-ab43-4780-97cd-021e99506337";   // jonas@jonasr.app, XrmToolBoxInsights
        private readonly AppInsights ai;

        #endregion AI to log

        #region Internal Fields

        internal Dictionary<string, List<Entity>> views;
        internal FXBSettings settings;
        internal FXBConnectionSettings connectionsettings;
        internal bool working = false;
        internal Version CDSVersion = new Version();

        #endregion Internal Fields

        #region Private Fields

        private bool buttonsEnabled = true;
        private int resultpanecount = 0;
        private Entity view;

        #endregion Private Fields

        #region Public Constructors

        public FetchXmlBuilder()
        {
            InitializeComponent();

            // Tips to handle all errors from
            // https://stackoverflow.com/questions/5762526/how-can-i-make-something-that-catches-all-unhandled-exceptions-in-a-winforms-a
            // Add the event handler for handling non-UI thread exceptions to the event.
            //AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(Error_UnhandledException);

            tslAbout.Text = Assembly.GetExecutingAssembly().GetName().Version.ToString() + " by Jonas Rapp";

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
            LoadSetting();
        }

        private void Error_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.IsTerminating.ToString());
            if (e.ExceptionObject is Exception ex)
            {
                LogError($"Unhandling error: {ex}");
                ShowErrorDialog(e.ExceptionObject as Exception);
            }
            else
            {
                LogError($"Unhandling error: {e}");
                MessageBox.Show("Unhandeled error:\n" + e.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion Public Constructors

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

        internal Entity View
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

        public void ApplyState(object state)
        {
            if (state is string fetch && fetch.ToLowerInvariant().StartsWith("<fetch"))
            {
                dockControlBuilder.Init(fetch, null, null, false);
            }
        }

        public object GetState()
        {
            return dockControlBuilder?.GetFetchString(false, false);
        }

        #endregion Public Methods

        #region Internal Methods

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

        internal ControlValidationResult GetWarning(TreeNode node)
        {
            if (!settings.ShowValidation || node == null)
            {
                return null;
            }
            var warning = Validations.GetWarning(node, this);
            if (warning?.Level == ControlValidationLevel.Info && !settings.ShowValidationInfo)
            {
                warning = null;
            }
            return warning;
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

        internal void OpenURLProfile(string url, bool addparameters)
        {
            if (addparameters)
            {
                url = Utils.ProcessURL(url);
            }
            ConnectionDetail.OpenUrlWithBrowserProfile(new Uri(url));
        }

        internal static void OpenURL(string url)
        {
            url = Utils.ProcessURL(url);
            Process.Start(url);
        }

        #endregion Internal Methods

        #region Private Methods

        private void ApplySettings(bool reloadquery)
        {
            toolStripMain.Items.OfType<ToolStripItem>().ToList().ForEach(i => i.DisplayStyle = settings.ShowButtonTexts ? ToolStripItemDisplayStyle.ImageAndText : ToolStripItemDisplayStyle.Image);
            tsbRepo.Visible = settings.ShowRepository;
            if (reloadquery && connectionsettings != null && !string.IsNullOrWhiteSpace(connectionsettings.FetchXML))
            {
                dockControlBuilder.Init(connectionsettings.FetchXML, connectionsettings.LayoutXML, "loaded from last session", false);
            }
            dockControlBuilder.lblQAExpander.GroupBoxSetState(null, settings.QueryOptions.ShowQuickActions);
            var ass = Assembly.GetExecutingAssembly().GetName();
            var version = ass.Version.ToString();
            if (!version.Equals(settings.CurrentVersion))
            {
                // Reset some settings when new version is deployed
                var oldversion = settings.CurrentVersion;
                settings.CurrentVersion = version;
                SaveSetting();
                LogUse("ShowWelcome");
                Welcome.ShowWelcome(this, oldversion);
            }
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

        private string GetQueryExpressionCode()
        {
            var code = string.Empty;
            try
            {
                var QEx = dockControlBuilder.GetQueryExpression(false);
                code = QueryExpressionCodeGenerator.GetCSharpQueryExpression(QEx, entities, settings);
            }
            catch (FetchIsAggregateException ex)
            {
                code = $"/*\nThis FetchXML is not possible to convert to QueryExpression in the current version of the SDK.\n\n{ex.Message}+n*/";
            }
            catch (Exception ex)
            {
                code = $"/*\nFailed to generate C# {settings.CodeGenerators.QExStyle} with {settings.CodeGenerators.QExFlavor} code.\n\n{ex.Message}\n*/";
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
            LoadSettingConnection();
        }

        private void LoadSettingConnection()
        {
            connectionsettings = null;
            try
            {
                SettingsManager.Instance.TryLoad(typeof(FetchXmlBuilder), out connectionsettings, ConnectionDetail?.ConnectionName);
            }
            catch (InvalidOperationException) { }
            if (connectionsettings == null)
            {
                connectionsettings = new FXBConnectionSettings();
            }
        }

        /// <summary>Saves various configurations to file for next session</summary>
        private void SaveSetting()
        {
            SettingsManager.Instance.Save(typeof(FetchXmlBuilder), settings, "[Common]");
            if (connectionsettings == null)
            {
                connectionsettings = new FXBConnectionSettings();
            }
            connectionsettings.FetchXML = dockControlBuilder.GetFetchString(false, false);
            connectionsettings.LayoutXML = dockControlBuilder.LayoutXML?.ToXML();
            SettingsManager.Instance.Save(typeof(FetchXmlBuilder), connectionsettings, ConnectionDetail?.ConnectionName);
        }

        #endregion Private Methods

        #region Private Event Handlers

        private void FetchXmlBuilder_ConnectionUpdated(object sender, ConnectionUpdatedEventArgs e)
        {
            entities = null;
            entityShitList.Clear();
            View = null;
            views = null;
            LoadSettingConnection();
            CDSVersion = new Version(e.ConnectionDetail.OrganizationVersion);
            LogInfo("Connected database version: {0} (Major: {1} Minor: {2})",
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
            LogUse("Load");
            CheckIntegrationTools();
            SetupDockControls();
            ApplySettings(true);
            RebuildRepositoryMenu(null);
            TreeNodeHelper.AddContextMenu(null, dockControlBuilder, settings.QueryOptions);
            EnableControls(true);
        }

        private void toolStripMain_Click(object sender, EventArgs e)
        {
            dockControlBuilder?.tvFetch?.Focus();
        }

        private void tslAbout_Click(object sender, EventArgs e)
        {
            ShowAboutDialog();
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
            LogUse(newconnection ? "Clone-Connection" : "Clone");
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
                dockControlBuilder.Init(null, null, "new", false);
                return;
            }
            var newconnection = sender == tsmiNewNewConnection;
            LogUse(newconnection ? "New-New-Connection" : "New-New");
            DuplicateRequested?.Invoke(this, new DuplicateToolArgs(settings.QueryOptions.NewQueryTemplate, newconnection));
        }

        private void tsbOptions_Click(object sender, EventArgs e)
        {
            ShowSettings();
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

        private void tsmiShowLayoutXML_Click(object sender, EventArgs e)
        {
            ShowContentControl(ref dockControlLayoutXml, ContentType.LayoutXML, SaveFormat.None, settings.DockStates.LayoutXML);
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
                dockControlBuilder.Init(query.Fetch, null, $"open repo {query.Name}", false);
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

        private void tmsiShowReleaseNotes_Click(object sender, EventArgs e)
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            OpenURL(Welcome.GetReleaseNotesUrl(version));
        }

        #endregion Private Event Handlers
    }
}