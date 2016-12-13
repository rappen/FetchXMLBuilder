using Cinteros.Xrm.FetchXmlBuilder.AppCode;
using Cinteros.Xrm.FetchXmlBuilder.Controls;
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
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using XrmToolBox.Extensibility;
using XrmToolBox.Extensibility.Interfaces;
using XrmToolBox.Extensibility.Args;
using Cinteros.Xrm.CRMWinForm;

namespace Cinteros.Xrm.FetchXmlBuilder
{
    public partial class FetchXmlBuilder : PluginControlBase, IGitHubPlugin, IPayPalPlugin, IMessageBusHost, IHelpPlugin, IStatusBarMessenger
    {
        #region Declarations
        private XmlDocument fetchDoc;
        private HistoryManager historyMgr = new HistoryManager();
        internal static Dictionary<string, EntityMetadata> entities;
        internal static List<string> entityShitList = new List<string>(); // Oops, did I name that one??
        internal static Dictionary<string, List<Entity>> views;
        private static string fetchTemplate = "<fetch count=\"50\"><entity name=\"\"/></fetch>";
        private string fileName;
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
                    tsmiSaveFile.Text = "Save File: " + System.IO.Path.GetFileName(value);
                }
                else
                {
                    tsmiSaveFile.Text = "Save File";
                }
            }
        }
        private Entity view;
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
                    tsmiSaveView.Text = "Save View: " + view["name"];
                }
                else
                {
                    tsmiSaveView.Text = "Save View";
                }
            }
        }
        private Entity dynml;
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
        private string cwpfeed;
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
        internal bool working = false;
        internal FXBSettings currentSettings;
        internal static bool friendlyNames = false;
        private string treeChecksum = "";
        private string attributesChecksum = "";
        private bool fetchChanged = false;
        private bool FetchChanged
        {
            get { return fetchChanged; }
            set
            {
                fetchChanged = value;
                EnableControls(buttonsEnabled);
                //toolStripButtonSave.Enabled = value;
            }
        }
        private bool buttonsEnabled = true;

        private XmlContentDisplayDialog xmlLiveUpdate;
        private string liveUpdateXml = "";
        private MessageBusEventArgs callerArgs = null;
        #endregion Declarations

        public FetchXmlBuilder()
        {
            InitializeComponent();
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
                LogUse("Close");
            }
        }

        private void FetchXmlBuilder_Load(object sender, EventArgs e)
        {
            if (ParentForm != null)
            {
                ParentForm.LocationChanged += FetchXmlBuilder_FormChanged;
            }
            LoadSetting();
            var tasks = new List<Task>
            {
                VersionCheck.LaunchVersionCheck(Assembly.GetExecutingAssembly().GetName().Version.ToString(), "Cinteros", "FetchXMLBuilder", "http://fxb.xrmtoolbox.com/?src=FXB.{0}", currentSettings.lastUpdateCheck.Date, this),
                this.LogUsageTask("Load")
            };
            currentSettings.lastUpdateCheck = DateTime.Now;
            tasks.ForEach(x => x.Start());
            TreeNodeHelper.AddContextMenu(null, this);
            EnableControls(true);
        }

        private void FetchXmlBuilder_ConnectionUpdated(object sender, ConnectionUpdatedEventArgs e)
        {
            entities = null;
            entityShitList.Clear();
            View = null;
            views = null;
            if (!working)
            {
                LoadEntities();
            }
            EnableControls(buttonsEnabled);
        }

        public void OnIncomingMessage(MessageBusEventArgs message)
        {
            if (message.TargetArgument != null)
            {
                callerArgs = message;
                var fetchXml = string.Empty;
                var requestedType = "FetchXML";
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
                if (!string.IsNullOrWhiteSpace(fetchXml))
                {
                    ParseXML(fetchXml, false);
                }
                tsbReturnToCaller.ToolTipText = "Return " + requestedType + " to " + callerArgs.SourcePlugin;
                RecordHistory("called from " + message.SourcePlugin);
                LogUse("CalledBy." + callerArgs.SourcePlugin);
            }
            EnableControls(true);
        }

        /// <summary>When SiteMap component properties are saved, they arecopied in the current selected TreeNode</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void CtrlSaved(object sender, SaveEventArgs e)
        {
            tvFetch.SelectedNode.Tag = e.AttributeCollection;
            TreeNodeHelper.SetNodeText(tvFetch.SelectedNode, currentSettings.useFriendlyNames);
            FetchChanged = treeChecksum != GetTreeChecksum(null);
            var origin = "";
            if (sender is IDefinitionSavable)
            {
                origin = sender.ToString().Replace("Cinteros.Xrm.FetchXmlBuilder.Controls.", "").Replace("Control", "");
                foreach (var attr in e.AttributeCollection)
                {
                    origin += "\n  " + attr.Key + "=" + attr.Value;
                }
            }
            RecordHistory(origin);
            UpdateLiveXML();
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
            fetchDoc = new XmlDocument();
            fetchDoc.LoadXml(fetchTemplate);
            DisplayDefinition();
            UpdateLiveXML();
            treeChecksum = GetTreeChecksum(null);
            FetchChanged = false;
            EnableControls(true);
            RecordHistory("new");
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
            var xml = GetFetchString(false);
            var xcdDialog = XmlContentDisplayDialog.Show(xml, "FetchXML", true, true, Service != null, SaveFormat.XML, this);
            if (xcdDialog.DialogResult == DialogResult.OK)
            {
                XmlNode resultNode = xcdDialog.result;
                EnableControls(false);
                ParseXML(resultNode.OuterXml, !xcdDialog.execute);
                UpdateLiveXML();
                RecordHistory("manual edit");
                if (xcdDialog.execute)
                {
                    FetchResults(resultNode.OuterXml);
                }
            }
        }

        private void tsbExecute_Click(object sender, EventArgs e)
        {
            tvFetch.Focus();
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

        private void nodeMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            HandleNodeMenuClick(e.ClickedItem);
        }

        private void tvFetch_AfterSelect(object sender, TreeViewEventArgs e)
        {
            HandleNodeSelection(e.Node);
        }

        private void tvFetch_KeyDown(object sender, KeyEventArgs e)
        {
            HandleTVKeyDown(e);
        }

        private void tvFetch_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                HandleNodeSelection(e.Node);
            }
        }

        private void toolStripButtonMoveDown_Click(object sender, EventArgs e)
        {
            moveDownToolStripMenuItem.Enabled = false;
            working = true;
            TreeNode tnmNode = tvFetch.SelectedNode;
            TreeNode tnmNextNode = tnmNode.NextNode;
            if (tnmNextNode != null)
            {
                int idxBegin = tnmNode.Index;
                int idxEnd = tnmNextNode.Index;
                TreeNode tnmNodeParent = tnmNode.Parent;
                if (tnmNodeParent != null)
                {
                    tnmNode.Remove();
                    tnmNextNode.Remove();
                    tnmNodeParent.Nodes.Insert(idxBegin, tnmNextNode);
                    tnmNodeParent.Nodes.Insert(idxEnd, tnmNode);
                    tvFetch.SelectedNode = tnmNode;
                    UpdateLiveXML();
                    RecordHistory("move down " + tnmNode.Name);
                }
            }
            working = false;
            moveDownToolStripMenuItem.Enabled = true;
        }

        private void toolStripButtonMoveUp_Click(object sender, EventArgs e)
        {
            moveUpToolStripMenuItem.Enabled = false;
            working = true;
            TreeNode tnmNode = tvFetch.SelectedNode;
            TreeNode tnmPreviousNode = tnmNode.PrevNode;
            if (tnmPreviousNode != null)
            {
                int idxBegin = tnmNode.Index;
                int idxEnd = tnmPreviousNode.Index;
                TreeNode tnmNodeParent = tnmNode.Parent;
                if (tnmNodeParent != null)
                {
                    tnmNode.Remove();
                    tnmPreviousNode.Remove();
                    tnmNodeParent.Nodes.Insert(idxEnd, tnmNode);
                    tnmNodeParent.Nodes.Insert(idxBegin, tnmPreviousNode);
                    tvFetch.SelectedNode = tnmNode;
                    UpdateLiveXML();
                    RecordHistory("move up " + tnmNode.Name);
                }
            }
            working = false;
            moveUpToolStripMenuItem.Enabled = true;
        }

        private void toolStripMain_Click(object sender, EventArgs e)
        {
            tvFetch.Focus();
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
                        ParseXML(xmlLiveUpdate.txtXML.Text, false);
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
            RestoreHistoryPosition(1);
        }

        private void tsbRedo_Click(object sender, EventArgs e)
        {
            RestoreHistoryPosition(-1);
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
                ApplyCurrentSettings();
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
            currentSettings.fetchxml = GetFetchString(false);
            SettingsManager.Instance.Save(typeof(FetchXmlBuilder), currentSettings, ConnectionDetail?.ConnectionName);
        }

        /// <summary>Loads configurations from file</summary>
        private void LoadSetting()
        {
            if (!SettingsManager.Instance.TryLoad<FXBSettings>(typeof(FetchXmlBuilder), out currentSettings, ConnectionDetail?.ConnectionName) &&
                !SettingsManager.Instance.TryLoad<FXBSettings>(typeof(FetchXmlBuilder), out currentSettings))
            {
                currentSettings = new FXBSettings();
            }
            ApplySettings();
        }

        private void ApplySettings()
        {
            if (currentSettings != null && !string.IsNullOrWhiteSpace(currentSettings.fetchxml))
            {
                fetchDoc = new XmlDocument();
                fetchDoc.LoadXml(currentSettings.fetchxml);
                DisplayDefinition();
                UpdateLiveXML();
                RecordHistory("loaded from last session");
            }
            ShowQuickActions();
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
        private void EnableControls(bool enabled)
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
                    tsbReturnToCaller.Visible = tvFetch.Nodes.Count > 0 && CallerWantsResults();
                    tsbSave.Enabled = enabled;
                    tsmiSaveFile.Enabled = enabled && FetchChanged && !string.IsNullOrEmpty(FileName);
                    tsmiSaveFileAs.Enabled = enabled && tvFetch.Nodes.Count > 0;
                    tsmiSaveView.Enabled = enabled && Service != null && View != null;
                    tsmiSaveML.Enabled = enabled && Service != null && DynML != null;
                    tsmiSaveCWP.Visible = enabled && Service != null && entities != null && entities.ContainsKey("cint_feed");
                    tsmiSaveCWP.Enabled = enabled && Service != null && FetchChanged && !string.IsNullOrEmpty(CWPFeed);
                    tsmiToQureyExpression.Enabled = enabled && Service != null;
                    tsmiToSQLQuery.Enabled = enabled && Service != null;
                    tsbView.Enabled = enabled;
                    tsbExecute.Enabled = enabled && tvFetch.Nodes.Count > 0 && Service != null;
                    selectAttributesToolStripMenuItem.Enabled = enabled && Service != null;
                    gbFetchTree.Enabled = enabled;
                    gbProperties.Enabled = enabled;
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
        private void DisplayDefinition()
        {
            if (fetchDoc == null)
            {
                return;
            }
            XmlNode definitionXmlNode = fetchDoc.DocumentElement;
            tvFetch.Nodes.Clear();
            TreeNodeHelper.AddTreeViewNode(tvFetch, definitionXmlNode, this);
            tvFetch.ExpandAll();
            ManageMenuDisplay();
        }

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
        private void ManageMenuDisplay()
        {
            TreeNode selectedNode = tvFetch.SelectedNode;
            moveUpToolStripMenuItem.Enabled = selectedNode != null && selectedNode.Parent != null &&
                                            selectedNode.Index != 0;
            moveDownToolStripMenuItem.Enabled = selectedNode != null && selectedNode.Parent != null &&
                                              selectedNode.Index != selectedNode.Parent.Nodes.Count - 1;
        }

        private XmlDocument GetFetchDocument()
        {
            var doc = new XmlDocument();
            if (tvFetch.Nodes.Count > 0)
            {
                XmlNode rootNode = doc.CreateElement("root");
                doc.AppendChild(rootNode);
                TreeNodeHelper.AddXmlNode(tvFetch.Nodes[0], rootNode);
                var xmlbody = doc.SelectSingleNode("root/fetch").OuterXml;
                doc.LoadXml(xmlbody);
            }
            return doc;
        }

        private string GetFetchString(bool format)
        {
            var xml = "";
            if (tvFetch.Nodes.Count > 0)
            {
                var doc = GetFetchDocument();
                xml = doc.OuterXml;
                if (currentSettings.useSingleQuotation)
                {
                    xml = xml.Replace("'", "&apos;");
                    xml = xml.Replace("\"", "'");
                }
            }
            if (format)
            {
                XDocument doc = XDocument.Parse(xml);
                xml = doc.ToString();
            }
            return xml;
        }

        private bool BuildAndValidateXml(bool validate = true)
        {
            if (tvFetch.Nodes.Count == 0)
            {
                return false;
            }
            fetchDoc = GetFetchDocument();
            var result = "";
            if (validate)
            {
                try
                {
                    Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                    string assemblyname = assembly.ManifestModule.ToString();
                    if (assemblyname.ToLower().EndsWith(".dll"))
                    {
                        assemblyname = assemblyname.Substring(0, assemblyname.Length - 4);
                    }
                    assemblyname = assemblyname.Replace("Merged", "");
                    assemblyname = assemblyname.Replace("..", ".");
                    Stream stream = assembly.GetManifestResourceStream(assemblyname + ".Resources.fetch.xsd");
                    if (stream == null)
                    {
                        result = "Cannot find resource " + assemblyname + ".Resources.fetch.xsd";
                    }
                    else
                    {
                        fetchDoc.Schemas.Add(null, XmlReader.Create(stream));
                        fetchDoc.Validate(null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    result = ex.Message;
                }
            }
            return string.IsNullOrEmpty(result);
        }

        private bool SaveIfChanged()
        {
            var ok = true;
            if (FetchChanged)
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
                BuildAndValidateXml();
                fetchDoc.Save(FileName);
                treeChecksum = GetTreeChecksum(null);
                FetchChanged = false;
                if (!silent)
                {
                    MessageBox.Show(this, "FetchXML saved!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                result = true;
                EnableControls(true);
            }
            return result;
        }

        private void HandleNodeMenuClick(ToolStripItem ClickedItem)
        {
            if (ClickedItem == null || ClickedItem.Tag == null || ClickedItem.Tag.ToString() == "Add")
                return;
            TreeNode updateNode = null;
            if (ClickedItem.Tag.ToString() == "Delete")
            {
                updateNode = DeleteNode();
            }
            else if (ClickedItem.Tag.ToString() == "Comment")
            {
                CommentNode();
            }
            else if (ClickedItem.Tag.ToString() == "Uncomment")
            {
                UncommentNode();
            }
            else if (ClickedItem.Tag.ToString() == "SelectAttributes")
            {
                SelectAttributes();
            }
            else
            {
                string nodeText = ClickedItem.Tag.ToString();
                updateNode = TreeNodeHelper.AddChildNode(tvFetch.SelectedNode, nodeText);
                RecordHistory("add " + updateNode.Name);
                HandleNodeSelection(updateNode);
            }
            if (updateNode != null)
            {
                TreeNodeHelper.SetNodeTooltip(updateNode);
            }
            FetchChanged = treeChecksum != GetTreeChecksum(null);
            UpdateLiveXML();
        }

        private void HandleNodeSelection(TreeNode node)
        {
            if (!working)
            {
                if (tvFetch.SelectedNode != node)
                {
                    tvFetch.SelectedNode = node;
                    return;
                }

                UserControl ctrl = null;
                Control existingControl = panelContainer.Controls.Count > 0 ? panelContainer.Controls[0] : null;
                if (node != null)
                {
                    TreeNodeHelper.AddContextMenu(node, this);
                    this.deleteToolStripMenuItem.Text = "Delete " + node.Name;
                    var collec = (Dictionary<string, string>)node.Tag;

                    switch (node.Name)
                    {
                        case "fetch":
                            ctrl = new fetchControl(collec, this);
                            break;
                        case "entity":
                            ctrl = new entityControl(collec, this);
                            break;
                        case "link-entity":
                            if (node.Parent != null)
                            {
                                switch (node.Parent.Name)
                                {
                                    case "entity":
                                    case "link-entity":
                                        var entityName = TreeNodeHelper.GetAttributeFromNode(node.Parent, "name");
                                        if (NeedToLoadEntity(entityName))
                                        {
                                            if (!working)
                                            {
                                                LoadEntityDetails(entityName, RefreshSelectedNode);
                                            }
                                            break;
                                        }
                                        break;
                                }
                            }
                            var linkEntityName = TreeNodeHelper.GetAttributeFromNode(node, "name");
                            if (NeedToLoadEntity(linkEntityName))
                            {
                                if (!working)
                                {
                                    LoadEntityDetails(linkEntityName, RefreshSelectedNode);
                                }
                                break;
                            }
                            ctrl = new linkEntityControl(node, this);
                            break;
                        case "attribute":
                        case "order":
                            if (node.Parent != null)
                            {
                                switch (node.Parent.Name)
                                {
                                    case "entity":
                                    case "link-entity":
                                        var entityName = TreeNodeHelper.GetAttributeFromNode(node.Parent, "name");
                                        if (NeedToLoadEntity(entityName))
                                        {
                                            if (!working)
                                            {
                                                LoadEntityDetails(entityName, RefreshSelectedNode);
                                            }
                                            break;
                                        }
                                        AttributeMetadata[] attributes = GetDisplayAttributes(entityName);
                                        if (node.Name == "attribute")
                                        {
                                            ctrl = new attributeControl(node, attributes, this);
                                        }
                                        else if (node.Name == "order")
                                        {
                                            ctrl = new orderControl(node, attributes, this);
                                        }
                                        break;
                                }
                            }
                            break;
                        case "filter":
                            ctrl = new filterControl(collec, this);
                            break;
                        case "condition":
                            ctrl = new conditionControl(node, this);
                            break;
                        case "value":
                            ctrl = new valueControl(collec, this);
                            break;
                        case "#comment":
                            ctrl = new commentControl(collec, this);
                            break;

                        default:
                            {
                                panelContainer.Controls.Clear();
                            }
                            break;
                    }
                }
                if (ctrl != null)
                {
                    panelContainer.Controls.Add(ctrl);
                    ctrl.BringToFront();
                    ctrl.Dock = DockStyle.Fill;
                }
                if (existingControl != null) panelContainer.Controls.Remove(existingControl);
            }
            ManageMenuDisplay();
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

        private void RefreshSelectedNode()
        {
            HandleNodeSelection(tvFetch.SelectedNode);
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
                TreeNodeHelper.SetNodeText(tvFetch.SelectedNode, currentSettings.useFriendlyNames);
            }
            working = false;
        }

        private string GetTreeChecksum(TreeNode node)
        {
            if (node == null)
            {
                if (tvFetch.Nodes.Count > 0)
                {
                    node = tvFetch.Nodes[0];
                }
                else
                {
                    return "";
                }
            }
            var result = "$" + node.Name;
            if (node.Tag is Dictionary<string, string>)
            {
                var coll = (Dictionary<string, string>)node.Tag;
                foreach (var key in coll.Keys)
                {
                    result += "@" + key + "=" + coll[key];
                }
            }
            foreach (TreeNode subnode in node.Nodes)
            {
                result += GetTreeChecksum(subnode);
            }
            return result;
        }

        private void FetchResults(string fetch = "")
        {
            if (!tsbExecute.Enabled)
            {
                return;
            }
            if (string.IsNullOrEmpty(fetch))
            {
                if (!BuildAndValidateXml(true))
                {
                    return;
                }
                fetch = GetFetchString(false);
            }
            if (working)
            {
                MessageBox.Show("Busy doing something...\n\nPlease wait until current transaction is done.");
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
            WorkAsync(new WorkAsyncInfo("Executing FetchXML...",
                (eventargs) =>
                {
                    QueryBase query;
                    try
                    {
                        query = GetQueryExpression();
                    }
                    catch (FetchIsAggregateException)
                    {
                        query = new FetchExpression(fetch);
                    }
                    var start = DateTime.Now;
                    EntityCollection resultCollection = Service.RetrieveMultiple(query);
                    var stop = DateTime.Now;
                    var duration = stop - start;
                    SendMessageToStatusBar(this, new StatusBarMessageEventArgs($"Execution time: {duration}"));
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
                        if (MessageBox.Show(completedargs.Error.Message + "\n\nTry with result as ExecuteFetch?", "Execute", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                        {
                            ExecuteFetch(fetch);
                        }
                    }
                    else if (completedargs.Result is EntityCollection)
                    {
                        if (outputtype == 0)
                        {
                            var gridDialog = new ResultGrid((EntityCollection)completedargs.Result, this);
                            gridDialog.StartPosition = FormStartPosition.CenterParent;
                            gridDialog.ShowDialog();
                        }
                        else if (outputtype == 1)
                        {
                            if (outputstyle == 0)
                            {
                                var serialized = EntityCollectionSerializer.Serialize((EntityCollection)completedargs.Result, SerializationStyle.Explicit);
                                XmlContentDisplayDialog.Show(serialized.OuterXml, "XML Serialized RetrieveMultiple result", false, false, false, SaveFormat.XML, this);
                            }
                            else if (outputstyle == 1)
                            {
                                var serialized = EntityCollectionSerializer.Serialize((EntityCollection)completedargs.Result, SerializationStyle.Basic);
                                XmlContentDisplayDialog.Show(serialized.OuterXml, "XML Serialized RetrieveMultiple result", false, false, false, SaveFormat.XML, this);
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
                fetchDoc = new XmlDocument();
                fetchDoc.Load(ofd.FileName);

                if (fetchDoc.DocumentElement.Name != "fetch" ||
                    fetchDoc.DocumentElement.ChildNodes.Count > 0 &&
                    fetchDoc.DocumentElement.ChildNodes[0].Name == "fetch")
                {
                    MessageBox.Show(this, "Invalid Xml: Definition XML root must be fetch!", "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    //LoadUsedEntities();
                    FileName = ofd.FileName;
                    DisplayDefinition();
                    UpdateLiveXML();
                    treeChecksum = GetTreeChecksum(null);
                    FetchChanged = false;
                    LogUse("OpenFile");
                    RecordHistory("open file");
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
                        fetchDoc = new XmlDocument();
                        fetchDoc.LoadXml(View["fetchxml"].ToString());
                        DisplayDefinition();
                        UpdateLiveXML();
                        treeChecksum = GetTreeChecksum(null);
                        FetchChanged = false;
                        attributesChecksum = GetAttributesSignature(null);
                        LogUse("OpenView");
                        RecordHistory("open view");
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
                    fetchDoc = new XmlDocument();
                    fetchDoc.LoadXml(DynML["query"].ToString());
                    DisplayDefinition();
                    UpdateLiveXML();
                    treeChecksum = GetTreeChecksum(null);
                    FetchChanged = false;
                    LogUse("OpenML");
                    RecordHistory("open marketing list");
                }
                else
                {
                    if (MessageBox.Show("The selected marketing list does not contain any FetchXML.\nPlease select another one.", "Open Marketing List",
                        MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                    {
                        OpenView();
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
                var fetch = feed["cint_fetchxml"].ToString();
                fetchDoc = new XmlDocument();
                fetchDoc.LoadXml(fetch);
                DisplayDefinition();
                UpdateLiveXML();
                treeChecksum = GetTreeChecksum(null);
                FetchChanged = false;
                LogUse("OpenCWP");
                RecordHistory("open CWP feed");
            }
            EnableControls(true);
        }

        private void SaveView()
        {
            var currentAttributes = GetAttributesSignature(null);
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
                    var xml = GetFetchString(false);
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
                }
            });
        }

        private void SaveML()
        {
            var msg = "Saving {0}...";
            WorkAsync(new WorkAsyncInfo(string.Format(msg, DynML["listname"]),
                (eventargs) =>
                {
                    var xml = GetFetchString(false);
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
            feed.Attributes.Add("cint_fetchxml", GetFetchString(true));
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

        private string GetAttributesSignature(XmlNode entity)
        {
            var result = "";
            if (entity == null)
            {
                var xml = GetFetchDocument();
                entity = xml.SelectSingleNode("fetch/entity");
            }
            if (entity != null)
            {
                var alias = entity.Attributes["alias"] != null ? entity.Attributes["alias"].Value + "." : "";
                var entityAttributes = entity.SelectNodes("attribute");
                foreach (XmlNode attr in entityAttributes)
                {
                    if (attr.Attributes["alias"] != null)
                    {
                        result += alias + attr.Attributes["alias"].Value + "\n";
                    }
                    else if (attr.Attributes["name"] != null)
                    {
                        result += alias + attr.Attributes["name"].Value + "\n";
                    }
                }
                var linkEntities = entity.SelectNodes("link-entity");
                foreach (XmlNode link in linkEntities)
                {
                    result += GetAttributesSignature(link);
                }
            }
            return result;
        }

        private void HandleTVKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (deleteToolStripMenuItem.Enabled)
                {
                    if (MessageBox.Show(deleteToolStripMenuItem.Text + " ?", "Confirm", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                    {
                        HandleNodeMenuClick(deleteToolStripMenuItem);
                    }
                }
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Insert)
            {
                addMenu.Show(tvFetch.PointToScreen(tvFetch.Location));
            }
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

        private void SelectAttributes()
        {
            if (Service == null)
            {
                MessageBox.Show("Must be connected to CRM", "Select attributes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var entityNode = tvFetch.SelectedNode;
            if (entityNode.Name != "entity" &&
                entityNode.Name != "link-entity")
            {
                MessageBox.Show("Cannot select attributes for node " + entityNode.Name, "Select attributes", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var entityName = TreeNodeHelper.GetAttributeFromNode(entityNode, "name");
            if (string.IsNullOrWhiteSpace(entityName))
            {
                MessageBox.Show("Cannot find valid entity name from node " + entityNode.Name, "Select attributes", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (NeedToLoadEntity(entityName))
            {
                LoadEntityDetails(entityName, SelectAttributes);
                return;
            }
            var attributes = new List<AttributeMetadata>(GetDisplayAttributes(entityName));
            var selected = new List<string>();
            foreach (TreeNode subnode in entityNode.Nodes)
            {
                if (subnode.Name == "attribute")
                {
                    var attr = TreeNodeHelper.GetAttributeFromNode(subnode, "name");
                    if (!string.IsNullOrEmpty(attr))
                    {
                        selected.Add(attr);
                    }
                }
            }
            var selectAttributesDlg = new SelectAttributesDialog(attributes, selected);
            selectAttributesDlg.StartPosition = FormStartPosition.CenterParent;
            if (selectAttributesDlg.ShowDialog() == DialogResult.OK)
            {
                var i = 0;
                while (i < entityNode.Nodes.Count)
                {
                    TreeNode subnode = entityNode.Nodes[i];
                    if (subnode.Name == "attribute")
                    {
                        entityNode.Nodes.Remove(subnode);
                    }
                    else
                    {
                        i++;
                    }
                }
                var selectedAttributes = selectAttributesDlg.GetSelectedAttributes();
                foreach (var attribute in selectedAttributes)
                {
                    var attrNode = TreeNodeHelper.AddChildNode(entityNode, "attribute");
                    var coll = new Dictionary<string, string>();
                    coll.Add("name", attribute.LogicalName);
                    attrNode.Tag = coll;
                    TreeNodeHelper.SetNodeText(attrNode, currentSettings.useFriendlyNames);
                }
                FetchChanged = treeChecksum != GetTreeChecksum(null);
                UpdateLiveXML();
                RecordHistory("select attributes");
            }
        }

        private TreeNode DeleteNode()
        {
            var node = tvFetch.SelectedNode;
            var updateNode = node.Parent;
            node.Remove();
            RecordHistory("delete " + node.Name);
            return updateNode;
        }

        private void CommentNode()
        {
            var node = tvFetch.SelectedNode;
            if (node != null)
            {
                var doc = new XmlDocument();
                XmlNode rootNode = doc.CreateElement("root");
                doc.AppendChild(rootNode);
                TreeNodeHelper.AddXmlNode(node, rootNode);
                XDocument xdoc = XDocument.Parse(rootNode.InnerXml);
                var comment = xdoc.ToString();
                if (node.Nodes != null && node.Nodes.Count > 0)
                {
                    comment = "\r\n" + comment + "\r\n";
                }
                if (comment.Contains("--"))
                {
                    comment = comment.Replace("--", "~~");
                }
                if (comment.EndsWith("-"))
                {
                    comment = comment.Substring(0, comment.Length - 1) + "~";
                }
                var commentNode = doc.CreateComment(comment);
                var parent = node.Parent;
                var index = node.Index;
                node.Parent.Nodes.Remove(node);
                tvFetch.SelectedNode = TreeNodeHelper.AddTreeViewNode(parent, commentNode, this, index);
                RecordHistory("comment");
            }
        }

        private void UncommentNode()
        {
            var node = tvFetch.SelectedNode;
            if (node != null && node.Tag is Dictionary<string, string>)
            {
                var coll = node.Tag as Dictionary<string, string>;
                if (coll.ContainsKey("#comment"))
                {
                    var comment = coll["#comment"];
                    if (comment.Contains("~~"))
                    {
                        comment = comment.Replace("~~", "--");
                    }
                    if (comment.EndsWith("~"))
                    {
                        comment = comment.Substring(0, comment.Length - 1) + "-";
                    }
                    var doc = new XmlDocument();
                    try
                    {
                        doc.LoadXml(comment);
                        var parent = node.Parent;
                        var index = node.Index;
                        node.Parent.Nodes.Remove(node);
                        tvFetch.SelectedNode = TreeNodeHelper.AddTreeViewNode(parent, doc.DocumentElement, this, index);
                        tvFetch.SelectedNode.Expand();
                        RecordHistory("uncomment");
                    }
                    catch (XmlException ex)
                    {
                        var msg = "Comment does contain well formatted xml.\nError description:\n\n" + ex.Message;
                        MessageBox.Show(msg, "Uncomment", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }

        private void ParseXML(string xml, bool validate)
        {
            fetchDoc = new XmlDocument();
            fetchDoc.LoadXml(xml);
            treeChecksum = "";
            if (fetchDoc.DocumentElement.Name != "fetch" ||
                fetchDoc.DocumentElement.ChildNodes.Count > 0 &&
                fetchDoc.DocumentElement.ChildNodes[0].Name == "fetch")
            {
                MessageBox.Show(this, "Invalid Xml: Definition XML root must be fetch!", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                DisplayDefinition();
                FetchChanged = true;
                EnableControls(true);
                BuildAndValidateXml(validate);
            }
        }

        private void UpdateLiveXML()
        {
            if (tsmiLiveUpdate.Checked)
            {
                liveUpdateXml = GetFetchString(false);
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
                var QEx = GetQueryExpression();
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

        private QueryExpression GetQueryExpression()
        {
            if (IsFetchAggregate(tvFetch.Nodes.Count > 0 ? tvFetch.Nodes[0] : null))
            {
                throw new FetchIsAggregateException("QueryExpression does not support aggregate queries.");
            }
            var xml = GetFetchDocument();
            if (Service == null)
            {
                throw new Exception("Must be connected to CRM to convert to QueryExpression.");
            }
            var convert = (FetchXmlToQueryExpressionResponse)Service.Execute(new FetchXmlToQueryExpressionRequest() { FetchXml = xml.OuterXml });
            return convert.Query;
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
            FetchType fetch = GetFetchType();
            var odata = ODataCodeGenerator.GetODataQuery(fetch, ConnectionDetail.OrganizationDataServiceUrl, this);
            return odata;
        }

        private void DisplaySQLQuery()
        {
            FetchType fetch = GetFetchType();
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

        private FetchType GetFetchType()
        {
            var fetchstr = GetFetchString(false);
            var serializer = new XmlSerializer(typeof(FetchType));
            object result;
            using (TextReader reader = new StringReader(fetchstr))
            {
                result = serializer.Deserialize(reader);
            }
            return result as FetchType;
        }

        private Task LogUsageTask(string action)
        {
            return new Task(() =>
            {
                LogUse(action);
            });
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
            var message = new MessageBusEventArgs(callerArgs.SourcePlugin);
            if (callerArgs.TargetArgument is FXBMessageBusArgument)
            {
                var fxbArgs = (FXBMessageBusArgument)callerArgs.TargetArgument;
                switch (fxbArgs.Request)
                {
                    case FXBMessageBusRequest.FetchXML:
                        fxbArgs.FetchXML = GetFetchString(true);
                        break;
                    case FXBMessageBusRequest.QueryExpression:
                        fxbArgs.QueryExpression = GetQueryExpression();
                        break;
                    case FXBMessageBusRequest.OData:
                        fxbArgs.OData = GetOData();
                        break;
                }
                message.TargetArgument = fxbArgs;
            }
            else
            {
                message.TargetArgument = GetFetchString(true);
            }
            OnOutgoingMessage(this, message);
        }

        private void RecordHistory(string action)
        {
            var fetch = GetFetchString(false);
            historyMgr.RecordHistory(action, fetch);
            EnableDisableHistoryButtons();
        }

        private void RestoreHistoryPosition(int delta)
        {
            LogUse(delta < 0 ? "Undo" : "Redo");
            var fetch = historyMgr.RestoreHistoryPosition(delta) as string;
            if (fetch != null)
            {
                ParseXML(fetch, false);
                RefreshSelectedNode();
            }
            EnableDisableHistoryButtons();
        }

        private void EnableDisableHistoryButtons()
        {
            historyMgr.SetupUndoButton(tsbUndo);
            historyMgr.SetupRedoButton(tsbRedo);
        }

        private void ShowQuickActions()
        {
            gbQuickActions.Visible = currentSettings.showQuickActions;
            panelButtonSpacer.Visible = currentSettings.showQuickActions;
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

        internal static bool IsFetchAggregate(TreeNode node)
        {
            var aggregate = false;
            while (node != null && node.Name != "fetch")
            {
                node = node.Parent;
            }
            if (node != null && node.Name == "fetch")
            {
                aggregate = TreeNodeHelper.GetAttributeFromNode(node, "aggregate") == "true";
            }
            return aggregate;
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

        private void ApplyCurrentSettings()
        {
            BuildAndValidateXml(false);
            DisplayDefinition();
            HandleNodeSelection(tvFetch.SelectedNode);
            UpdateLiveXML();
            ShowQuickActions();
        }
    }
}
