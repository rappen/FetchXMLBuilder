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
using XrmToolBox.Extensibility;
using XrmToolBox.Extensibility.Interfaces;
using XrmToolBox.Forms;
using Clipboard = Cinteros.Xrm.FetchXmlBuilder.AppCode.Clipboard;

namespace Cinteros.Xrm.FetchXmlBuilder
{
    public partial class FetchXmlBuilder : PluginControlBase, IGitHubPlugin, IPayPalPlugin, IMessageBusHost, IHelpPlugin
    {
        #region Declarations
        const string settingfile = "Cinteros.Xrm.FetchXmlBuilder.Settings.xml";
        internal Clipboard clipboard = new Clipboard();
        private XmlDocument fetchDoc;
        private static Dictionary<string, EntityMetadata> entities;
        internal static List<string> entityShitList = new List<string>(); // Oops, did I name that one??
        internal static Dictionary<string, List<Entity>> views;
        private static string fetchTemplate = "<fetch count=\"50\"><entity name=\"\"/></fetch>";
        private string fileName;
        internal string FileName
        {
            get { return fileName; }
            set
            {
                fileName = value;
                if (!string.IsNullOrWhiteSpace(value))
                {
                    tsmiSaveFile.Text = "Save File: " + System.IO.Path.GetFileName(value);
                    View = null;
                    CWPFeed = null;
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
                view = value;
                if (view != null && view.Contains("name"))
                {
                    tsmiSaveView.Text = "Save View: " + view["name"];
                    FileName = null;
                    CWPFeed = null;
                }
                else
                {
                    tsmiSaveView.Text = "Save View";
                }
            }
        }
        private string cwpfeed;
        internal string CWPFeed
        {
            get { return cwpfeed; }
            set
            {
                cwpfeed = value;
                if (!string.IsNullOrWhiteSpace(cwpfeed))
                {
                    tsmiSaveCWP.Text = "Save CWP Feed: " + cwpfeed;
                    FileName = null;
                    View = null;
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
                this.LaunchVersionCheck("Cinteros", "FetchXMLBuilder", "http://fxb.xrmtoolbox.com/?src=FXB.{0}"),
                this.LogUsageTask("Load")
            };
            tasks.ForEach(x => x.Start());
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
            if (message.TargetArgument != null && message.TargetArgument is FXBMessageBusArgument)
            {
                callerArgs = message;
                var fxbArg = (FXBMessageBusArgument)message.TargetArgument;
                if (!string.IsNullOrWhiteSpace(fxbArg.FetchXML))
                {
                    ParseXML(fxbArg.FetchXML, false);
                }
                tsbReturnToCaller.ToolTipText = "Return " + fxbArg.Request.ToString() + " to " + callerArgs.SourcePlugin;
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
            if (tsmiLiveUpdate.Checked)
            {
                UpdateLiveXML();
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
            treeChecksum = GetTreeChecksum(null);
            FetchChanged = false;
            EnableControls(true);
        }

        private void tsmiOpenFile_Click(object sender, EventArgs e)
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
                    treeChecksum = GetTreeChecksum(null);
                    FetchChanged = false;
                    EnableControls(true);
                }
            }
        }

        private void tsmiOpenView_Click(object sender, EventArgs e)
        {
            OpenView();
        }

        private void tsmiOpenCWP_Click(object sender, EventArgs e)
        {
            OpenCWPFeed();
        }

        private void tsbEdit_Click(object sender, EventArgs e)
        {
            var xml = GetFetchString(false);
            var xcdDialog = new XmlContentDisplayDialog(xml, "FetchXML", true, true, this);
            xcdDialog.StartPosition = FormStartPosition.CenterParent;
            if (xcdDialog.ShowDialog() == DialogResult.OK)
            {
                EnableControls(false);
                XmlNode resultNode = xcdDialog.result;
                ParseXML(resultNode.OuterXml, true);
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
                }
            }
            moveDownToolStripMenuItem.Enabled = true;
        }

        private void toolStripButtonMoveUp_Click(object sender, EventArgs e)
        {
            moveUpToolStripMenuItem.Enabled = false;
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
                }
            }
            moveUpToolStripMenuItem.Enabled = true;
        }

        private void tsmiFriendly_CheckedChanged(object sender, EventArgs e)
        {
            currentSettings.useFriendlyNames = tsmiFriendly.Checked;
            BuildAndValidateXml(false);
            DisplayDefinition();
            HandleNodeSelection(tvFetch.SelectedNode);
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

        private void tsmiEntities_Click(object sender, EventArgs e)
        {
            if (sender != null && sender != tsmiEntitiesAll)
            {
                tsmiEntitiesAll.Checked =
                    tsmiEntitiesManaged.Checked &&
                    tsmiEntitiesUnmanaged.Checked &&
                    tsmiEntitiesCustomizable.Checked &&
                    tsmiEntitiesUncustomizable.Checked &&
                    tsmiEntitiesCustom.Checked &&
                    tsmiEntitiesStandard.Checked &&
                    tsmiEntitiesIntersect.Checked &&
                    !tsmiEntitiesOnlyValidAF.Checked;
            }
            if (!tsmiEntitiesManaged.Checked && !tsmiEntitiesUnmanaged.Checked)
            {   // Neither managed nor unmanaged is not such a good idea...
                tsmiEntitiesUnmanaged.Checked = true;
            }
            if (!tsmiEntitiesCustomizable.Checked && !tsmiEntitiesUncustomizable.Checked)
            {   // Neither customizable nor uncustomizable is not such a good idea...
                tsmiEntitiesCustomizable.Checked = true;
            }
            if (!tsmiEntitiesCustom.Checked && !tsmiEntitiesStandard.Checked)
            {   // Neither custom nor standard is not such a good idea...
                tsmiEntitiesCustom.Checked = true;
            }
            tsmiEntitiesManaged.Enabled = !tsmiEntitiesAll.Checked;
            tsmiEntitiesUnmanaged.Enabled = !tsmiEntitiesAll.Checked;
            tsmiEntitiesCustomizable.Enabled = !tsmiEntitiesAll.Checked;
            tsmiEntitiesUncustomizable.Enabled = !tsmiEntitiesAll.Checked;
            tsmiEntitiesCustom.Enabled = !tsmiEntitiesAll.Checked;
            tsmiEntitiesStandard.Enabled = !tsmiEntitiesAll.Checked;
            tsmiEntitiesIntersect.Enabled = !tsmiEntitiesAll.Checked;
            tsmiEntitiesOnlyValidAF.Enabled = !tsmiEntitiesAll.Checked;
            currentSettings.showEntitiesAll = tsmiEntitiesAll.Checked;
            currentSettings.showEntitiesManaged = tsmiEntitiesManaged.Checked;
            currentSettings.showEntitiesUnmanaged = tsmiEntitiesUnmanaged.Checked;
            currentSettings.showEntitiesCustomizable = tsmiEntitiesCustomizable.Checked;
            currentSettings.showEntitiesUncustomizable = tsmiEntitiesUncustomizable.Checked;
            currentSettings.showEntitiesCustom = tsmiEntitiesCustom.Checked;
            currentSettings.showEntitiesStandard = tsmiEntitiesStandard.Checked;
            currentSettings.showEntitiesIntersect = tsmiEntitiesIntersect.Checked;
            currentSettings.showEntitiesOnlyValidAF = tsmiEntitiesOnlyValidAF.Checked;
            HandleNodeSelection(tvFetch.SelectedNode);
        }

        private void tsmiAttributes_Click(object sender, EventArgs e)
        {
            if (sender != null && sender != tsmiAttributesAll)
            {
                tsmiAttributesAll.Checked =
                    tsmiAttributesManaged.Checked &&
                    tsmiAttributesUnmanaged.Checked &&
                    tsmiAttributesCustomizable.Checked &&
                    tsmiAttributesUncustomizable.Checked &&
                    tsmiAttributesCustom.Checked &&
                    tsmiAttributesStandard.Checked &&
                    !tsmiAttributesOnlyValidAF.Checked;
            }
            if (!tsmiAttributesManaged.Checked && !tsmiAttributesUnmanaged.Checked)
            {   // Neither managed nor unmanaged is not such a good idea...
                tsmiAttributesUnmanaged.Checked = true;
            }
            if (!tsmiAttributesCustomizable.Checked && !tsmiAttributesUncustomizable.Checked)
            {   // Neither customizable nor uncustomizable is not such a good idea...
                tsmiAttributesCustomizable.Checked = true;
            }
            if (!tsmiAttributesCustom.Checked && !tsmiAttributesStandard.Checked)
            {   // Neither custom nor standard is not such a good idea...
                tsmiAttributesCustom.Checked = true;
            }
            tsmiAttributesManaged.Enabled = !tsmiAttributesAll.Checked;
            tsmiAttributesUnmanaged.Enabled = !tsmiAttributesAll.Checked;
            tsmiAttributesCustomizable.Enabled = !tsmiAttributesAll.Checked;
            tsmiAttributesUncustomizable.Enabled = !tsmiAttributesAll.Checked;
            tsmiAttributesCustom.Enabled = !tsmiAttributesAll.Checked;
            tsmiAttributesStandard.Enabled = !tsmiAttributesAll.Checked;
            tsmiAttributesOnlyValidAF.Enabled = !tsmiAttributesAll.Checked;
            currentSettings.showAttributesAll = tsmiAttributesAll.Checked;
            currentSettings.showAttributesManaged = tsmiAttributesManaged.Checked;
            currentSettings.showAttributesUnmanaged = tsmiAttributesUnmanaged.Checked;
            currentSettings.showAttributesCustomizable = tsmiAttributesCustomizable.Checked;
            currentSettings.showAttributesUncustomizable = tsmiAttributesUncustomizable.Checked;
            currentSettings.showAttributesCustom = tsmiAttributesCustom.Checked;
            currentSettings.showAttributesStandard = tsmiAttributesStandard.Checked;
            currentSettings.showAttributesOnlyValidAF = tsmiAttributesOnlyValidAF.Checked;
            HandleNodeSelection(tvFetch.SelectedNode);
        }

        private void tsmiLiveUpdate_CheckedChanged(object sender, EventArgs e)
        {
            if (tsmiLiveUpdate.Checked)
            {
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

        private void tsmiToQureyExpression_Click(object sender, EventArgs e)
        {
            DisplayQExCode();
        }

        private void tsbReturnToCaller_Click(object sender, EventArgs e)
        {
            ReturnToCaller();
        }

        #endregion Event handlers

        #region Instance methods

        /// <summary>Saves various configurations to file for next session</summary>
        private void SaveSetting()
        {
            currentSettings.resultOption = tsmiResultOption.SelectedIndex;
            currentSettings.fetchxml = GetFetchString(false);
            currentSettings.Save();
        }

        //private void SaveControlValue(Configuration config, object control)
        //{
        //    if (control is ToolStripMenuItem)
        //    {
        //        config.AppSettings.Settings.Add(((ToolStripMenuItem)control).Name, ((ToolStripMenuItem)control).Checked ? "1" : "0");
        //    }
        //    else if (control is ToolStripComboBox)
        //    {
        //        config.AppSettings.Settings.Add(((ToolStripComboBox)control).Name, ((ToolStripComboBox)control).SelectedIndex.ToString());
        //    }
        //}

        //private void LoadControlValue(Configuration config, object control)
        //{
        //    if (control is ToolStripMenuItem)
        //    {
        //        var name = ((ToolStripMenuItem)control).Name;
        //        if (config.AppSettings.Settings[name] != null)
        //        {
        //            ((ToolStripMenuItem)control).Checked = config.AppSettings.Settings[name].Value == "1";
        //        }
        //    }
        //    else if (control is ToolStripComboBox)
        //    {
        //        var name = ((ToolStripComboBox)control).Name;
        //        if (config.AppSettings.Settings[name] != null)
        //        {
        //            var index = 0;
        //            if (int.TryParse(config.AppSettings.Settings[name].Value, out index) && ((ToolStripComboBox)control).Items.Count > index)
        //            {
        //                ((ToolStripComboBox)control).SelectedIndex = index;
        //            }
        //        }
        //    }
        //}

        /// <summary>Loads configurations from file</summary>
        private void LoadSetting()
        {
            currentSettings = FXBSettings.Load();
            if (!string.IsNullOrEmpty(currentSettings.fetchxml))
            {
                fetchDoc = new XmlDocument();
                fetchDoc.LoadXml(currentSettings.fetchxml);
                DisplayDefinition();
            }
            tsmiEntitiesAll.Checked = currentSettings.showEntitiesAll;
            tsmiEntitiesManaged.Checked = currentSettings.showEntitiesManaged;
            tsmiEntitiesUnmanaged.Checked = currentSettings.showEntitiesUnmanaged;
            tsmiEntitiesCustomizable.Checked = currentSettings.showEntitiesCustomizable;
            tsmiEntitiesUncustomizable.Checked = currentSettings.showEntitiesUncustomizable;
            tsmiEntitiesCustom.Checked = currentSettings.showEntitiesCustom;
            tsmiEntitiesStandard.Checked = currentSettings.showEntitiesStandard;
            tsmiEntitiesIntersect.Checked = currentSettings.showEntitiesIntersect;
            tsmiEntitiesOnlyValidAF.Checked = currentSettings.showEntitiesOnlyValidAF;
            tsmiAttributesAll.Checked = currentSettings.showAttributesAll;
            tsmiAttributesManaged.Checked = currentSettings.showAttributesManaged;
            tsmiAttributesUnmanaged.Checked = currentSettings.showAttributesUnmanaged;
            tsmiAttributesCustomizable.Checked = currentSettings.showAttributesCustomizable;
            tsmiAttributesUncustomizable.Checked = currentSettings.showAttributesUncustomizable;
            tsmiAttributesCustom.Checked = currentSettings.showAttributesCustom;
            tsmiAttributesStandard.Checked = currentSettings.showAttributesStandard;
            tsmiAttributesOnlyValidAF.Checked = currentSettings.showAttributesOnlyValidAF;
            tsmiResultOption.SelectedIndex = currentSettings.resultOption;
            tsmiEntities_Click(null, null);
            tsmiAttributes_Click(null, null);
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
                if (currentSettings.logUsage == true)
                {
                    LogUse("Accept", true);
                }
                else if (!currentSettings.logUsage == true)
                {
                    LogUse("Deny", true);
                }
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
                    tsmiOpenCWP.Visible = enabled && Service != null && entities != null && entities.ContainsKey("cint_feed");
                    tsbReturnToCaller.Visible = tvFetch.Nodes.Count > 0 && CallerWantsResults();
                    tsbSave.Enabled = enabled;
                    tsmiSaveFile.Enabled = enabled && FetchChanged && !string.IsNullOrEmpty(FileName);
                    tsmiSaveFileAs.Enabled = enabled && tvFetch.Nodes.Count > 0;
                    tsmiSaveView.Enabled = enabled && Service != null && FetchChanged && View != null;
                    tsmiSaveCWP.Visible = enabled && Service != null && entities != null && entities.ContainsKey("cint_feed");
                    tsmiToQureyExpression.Enabled = enabled && Service != null;
                    tsbOptions.Enabled = enabled;
                    tsmiFriendly.Enabled = enabled && tvFetch.Nodes.Count > 0 && Service != null;
                    tsmiShowEntities.Enabled = enabled && Service != null;
                    tsmiShowAttributes.Enabled = enabled && Service != null;
                    tsbExecute.Enabled = enabled && tvFetch.Nodes.Count > 0 && Service != null;
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
            return
                callerArgs != null &&
                callerArgs.TargetArgument is FXBMessageBusArgument &&
                ((FXBMessageBusArgument)callerArgs.TargetArgument).Request != FXBMessageBusRequest.None;
        }

        /// <summary>Repopulate the entire tree from the xml document containing the FetchXML</summary>
        private void DisplayDefinition()
        {
            if (fetchDoc == null)
            {
                return;
            }
            XmlNode definitionXmlNode = null;
            MethodInvoker miReadDefinition = delegate
            {
                definitionXmlNode = fetchDoc.DocumentElement;
            };
            if (InvokeRequired)
                Invoke(miReadDefinition);
            else
                miReadDefinition();

            MethodInvoker miFillTreeView = delegate
            {
                tvFetch.Nodes.Clear();
                TreeNodeHelper.AddTreeViewNode(tvFetch, definitionXmlNode, this);
                tvFetch.ExpandAll();
                ManageMenuDisplay();
                if (tsmiLiveUpdate.Checked)
                {
                    UpdateLiveXML();
                }
            };
            if (tvFetch.InvokeRequired)
                tvFetch.Invoke(miFillTreeView);
            else
                miFillTreeView();
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
            else if (ClickedItem.Tag.ToString() == "Delete")
                tvFetch.SelectedNode.Remove();
            //else if (ClickedItem.Tag.ToString() == "Cut")
            //    clipboard.Cut(tvFetch.SelectedNode);
            //else if (ClickedItem.Tag.ToString() == "Copy")
            //    clipboard.Copy(tvFetch.SelectedNode);
            //else if (ClickedItem.Tag.ToString() == "Paste")
            //    clipboard.Paste(tvFetch.SelectedNode);
            else if (ClickedItem.Tag.ToString() == "Attributes...")
            {
                AddAttributes();
            }
            else
            {
                string nodeText = ClickedItem.Tag.ToString();
                var newNode = TreeNodeHelper.AddChildNode(tvFetch.SelectedNode, nodeText);
                HandleNodeSelection(newNode);
            }
            FetchChanged = treeChecksum != GetTreeChecksum(null);
            if (tsmiLiveUpdate.Checked)
            {
                UpdateLiveXML();
            }
        }

        private void HandleNodeSelection(TreeNode node)
        {
            if (tvFetch.SelectedNode != node)
            {
                tvFetch.SelectedNode = node;
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
                //ctrl.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
                ctrl.Dock = DockStyle.Fill;
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
            WorkAsync("Loading entities...",
                (eventargs) =>
                {
                    EnableControls(false);
                    var req = new RetrieveAllEntitiesRequest()
                    {
                        EntityFilters = EntityFilters.Entity,
                        RetrieveAsIfPublished = true
                    };
                    eventargs.Result = Service.Execute(req);
                },
                (completedargs) =>
                {
                    if (completedargs.Error != null)
                    {
                        MessageBox.Show(completedargs.Error.Message);
                    }
                    else
                    {
                        if (completedargs.Result is RetrieveAllEntitiesResponse)
                        {
                            entities = new Dictionary<string, EntityMetadata>();
                            foreach (var entity in ((RetrieveAllEntitiesResponse)completedargs.Result).EntityMetadata)
                            {
                                entities.Add(entity.LogicalName, entity);
                            }
                        }
                    }
                    working = false;
                    EnableControls(true);
                });
        }

        internal void LoadEntityDetails(string entityName, Action detailsLoaded)
        {
            working = true;
            var name = GetEntityDisplayName(entityName);
            WorkAsync("Loading " + name + "...",
                (eventargs) =>
                {
                    var req = new RetrieveEntityRequest()
                    {
                        LogicalName = entityName,
                        EntityFilters = EntityFilters.Attributes | EntityFilters.Relationships,
                        RetrieveAsIfPublished = true
                    };
                    eventargs.Result = Service.Execute(req);
                },
                (completedargs) =>
                {
                    if (completedargs.Error != null)
                    {
                        entityShitList.Add(entityName);
                        MessageBox.Show(completedargs.Error.Message, "Load attribute metadata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        if (completedargs.Result is RetrieveEntityResponse)
                        {
                            var resp = (RetrieveEntityResponse)completedargs.Result;
                            if (entities.ContainsKey(entityName))
                            {
                                entities[entityName] = resp.EntityMetadata;
                            }
                            else
                            {
                                entities.Add(entityName, resp.EntityMetadata);
                            }
                        }
                        working = false;
                        TreeNodeHelper.SetNodeText(tvFetch.SelectedNode, currentSettings.useFriendlyNames);
                        detailsLoaded();
                    }
                    working = false;
                });
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

        private void FetchResults()
        {
            if (!tsbExecute.Enabled)
            {
                return;
            }
            if (!BuildAndValidateXml(true))
            {
                return;
            }
            if (working)
            {
                MessageBox.Show("Busy doing something...\n\nPlease wait until current transaction is done.");
                return;
            }
            var fetchType = tsmiResultOption.SelectedIndex;
            switch (fetchType)
            {
                case 0:
                case 1:
                case 2:
                    RetrieveMultiple();
                    break;
                case 3:
                    ExecuteFetch();
                    break;
                default:
                    MessageBox.Show("Select valid output type under Options.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }

        private void ExecuteFetch()
        {
            working = true;
            WorkAsync("Executing FetchXML...",
                (eventargs) =>
                {
                    var fetchxml = GetFetchDocument().OuterXml;
                    var resp = (ExecuteFetchResponse)Service.Execute(new ExecuteFetchRequest() { FetchXml = fetchxml });
                    eventargs.Result = resp.FetchXmlResult;
                },
                (completedargs) =>
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
                        var xcdDialog = new XmlContentDisplayDialog(doc.OuterXml, "FetchXML result", false, false, this);
                        xcdDialog.StartPosition = FormStartPosition.CenterParent;
                        xcdDialog.ShowDialog();
                    }
                });
            LogUse("ExecuteFetch");
        }

        private void RetrieveMultiple()
        {
            working = true;
            var outputtype = tsmiResultOption.SelectedIndex;
            var outputtypestring = tsmiResultOption.SelectedItem.ToString();
            WorkAsync("Executing FetchXML...",
                (eventargs) =>
                {
                    EntityCollection resultCollection = null;
                    QueryBase query;
                    try
                    {
                        query = GetQueryExpression();
                    }
                    catch (FetchIsAggregateException)
                    {
                        query = new FetchExpression(GetFetchString(false));
                    }
                    resultCollection = Service.RetrieveMultiple(query);
                    if (outputtype == 2)
                    {
                        var json = EntityCollectionSerializer.ToJSON(resultCollection, Formatting.Indented);
                        eventargs.Result = json;
                    }
                    else
                    {
                        eventargs.Result = resultCollection;
                    }
                },
                (completedargs) =>
                {
                    working = false;
                    if (completedargs.Error != null)
                    {
                        if (MessageBox.Show(completedargs.Error.Message + "\n\nTry with result as ExecuteFetch?", "Execute", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                        {
                            ExecuteFetch();
                        }
                    }
                    else if (outputtype == 0 && completedargs.Result is EntityCollection)
                    {
                        var gridDialog = new ResultGrid((EntityCollection)completedargs.Result, this);
                        gridDialog.StartPosition = FormStartPosition.CenterParent;
                        gridDialog.ShowDialog();
                    }
                    else if (outputtype == 1 && completedargs.Result is EntityCollection)
                    {
                        var serialized = EntityCollectionSerializer.Serialize((EntityCollection)completedargs.Result);
                        var xcdDialog = new XmlContentDisplayDialog(serialized.OuterXml, "XML Serialized RetrieveMultiple result", false, false, this);
                        xcdDialog.StartPosition = FormStartPosition.CenterParent;
                        xcdDialog.ShowDialog();
                    }
                    else if (outputtype == 2 && completedargs.Result is string)
                    {
                        var result = completedargs.Result.ToString();
                        var xcdDialog = new XmlContentDisplayDialog(result, "JSON Serialized RetrieveMultiple result", false, false, this);
                        xcdDialog.btnFormat.Visible = false;
                        xcdDialog.StartPosition = FormStartPosition.CenterParent;
                        xcdDialog.ShowDialog();
                    }
                });
            LogUse("RetrieveMultiple-" + outputtypestring);
        }

        internal void LoadViews(Action viewsLoaded)
        {
            WorkAsync("Loading views...",
            (eventargs) =>
            {
                EnableControls(false);
                if (views == null || views.Count == 0)
                {
                    if (Service == null)
                    {
                        throw new Exception("Need a connection to load views.");
                    }
                    var qex = new QueryExpression("savedquery");
                    qex.ColumnSet = new ColumnSet("name", "returnedtypecode", "fetchxml");
                    qex.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
                    qex.Criteria.AddCondition("querytype", ConditionOperator.In, 0, 32);
                    qex.AddOrder("name", OrderType.Ascending);
                    var sysviews = Service.RetrieveMultiple(qex);
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
                    qex.EntityName = "userquery";
                    var userviews = Service.RetrieveMultiple(qex);
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
            },
            (completedargs) =>
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
            });

        }

        private void OpenView()
        {
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
                        attributesChecksum = GetAttributesSignature(null);
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
            WorkAsync(string.Format(msg, View["name"]),
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
                },
                (completedargs) =>
                {
                    if (completedargs.Error != null)
                    {
                        MessageBox.Show(completedargs.Error.Message, "Save view", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                });
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

        private void OpenCWPFeed()
        {
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
                attributesChecksum = GetAttributesSignature(null);
                LogUse("OpenCWPFeed");
            }
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

        private void AddAttributes()
        {
            if (Service == null)
            {
                MessageBox.Show("Must be connected to CRM", "Add attributes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var entityNode = tvFetch.SelectedNode;
            if (entityNode.Name != "entity" &&
                entityNode.Name != "link-entity")
            {
                MessageBox.Show("Cannot add attributes to node " + entityNode.Name, "Add attributes", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var entityName = TreeNodeHelper.GetAttributeFromNode(entityNode, "name");
            if (string.IsNullOrWhiteSpace(entityName))
            {
                MessageBox.Show("Cannot find valid entity name from node " + entityNode.Name, "Add attributes", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (NeedToLoadEntity(entityName))
            {
                LoadEntityDetails(entityName, AddAttributes);
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
                if (tsmiLiveUpdate.Checked)
                {
                    UpdateLiveXML();
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
            liveUpdateXml = GetFetchString(false);
            if (xmlLiveUpdate == null)
            {
                xmlLiveUpdate = new XmlContentDisplayDialog(liveUpdateXml, "FetchXML Live Update", false, true, this);
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
                var view = new XmlContentDisplayDialog(code, "QueryExpression Code", false, false, this);
                LogUse("DisplayQExCode");
                view.ShowDialog();
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

        private string GetOData()
        {
            throw new NotImplementedException("OData output is not yet implemented.");
        }

        private Task LaunchVersionCheck(string ghUser, string ghRepo, string dlUrl)
        {
            return new Task(() =>
            {
                var currentVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                var cvc = new XrmToolBox.AppCode.GithubVersionChecker(currentVersion, ghUser, ghRepo);

                cvc.Run();

                if (cvc.Cpi != null && !string.IsNullOrEmpty(cvc.Cpi.Version))
                {
                    if (currentSettings.lastUpdateCheck.Date != DateTime.Now.Date)
                    {
                        this.Invoke(new Action(() =>
                        {
                            var nvForm = new NewVersionForm(currentVersion, cvc.Cpi.Version, cvc.Cpi.Description, ghUser, ghRepo, new Uri(string.Format(dlUrl, currentVersion)));
                            nvForm.ShowDialog(this);
                        }));
                    }
                }
                currentSettings.lastUpdateCheck = DateTime.Now;
            });
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
                //else
                //{
                //    foreach (var label in entity.DisplayName.LocalizedLabels)
                //    {
                //        if (label.LanguageCode == userLCID)
                //        {
                //            result = label.Label;
                //            break;
                //        }
                //    }
                //}
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
                //else
                //{
                //    foreach (var label in attribute.DisplayName.LocalizedLabels)
                //    {
                //        if (label.LanguageCode == userLCID)
                //        {
                //            attributeName = label.Label;
                //            break;
                //        }
                //    }
                //}
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
                        }
                        result.Add(attribute);
                    }
                }
            }
            return result.ToArray();
        }
        
        #endregion Static methods

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
    }
}
