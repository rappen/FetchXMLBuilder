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
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using XrmToolBox.Attributes;
using Clipboard = Cinteros.Xrm.FetchXmlBuilder.AppCode.Clipboard;

[assembly: BackgroundColor("#000000")]

namespace Cinteros.Xrm.FetchXmlBuilder
{
    public partial class FetchXmlBuilder : XrmToolBox.PluginBase
    {
        #region Declarations
        const string settingfile = "Cinteros.Xrm.FetchXmlBuilder.Settings.xml";
        internal Clipboard clipboard = new Clipboard();
        private bool Initializing = true;
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
                }
                else
                {
                    tsmiSaveView.Text = "Save View";
                }
            }
        }
        internal bool working = false;
        internal static bool useFriendlyNames = false;
        private static bool showEntitiesAll = true;
        private static bool showEntitiesManaged = true;
        private static bool showEntitiesUnmanaged = true;
        private static bool showEntitiesCustomizable = true;
        private static bool showEntitiesUncustomizable = true;
        private static bool showEntitiesCustom = true;
        private static bool showEntitiesStandard = true;
        private static bool showEntitiesIntersect = true;
        private static bool showEntitiesOnlyValidAF = true;
        private static bool showAttributesAll = true;
        private static bool showAttributesManaged = true;
        private static bool showAttributesUnmanaged = true;
        private static bool showAttributesCustomizable = true;
        private static bool showAttributesUncustomizable = true;
        private static bool showAttributesCustom = true;
        private static bool showAttributesStandard = true;
        private static bool showAttributesOnlyValidAF = true;
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
        internal static Size xmlWinSize;
        #endregion Declarations

        public FetchXmlBuilder()
        {
            InitializeComponent();
        }

        public override Image PluginLogo
        {
            get
            {
                return imageList1.Images[0];
            }
        }

        #region Event handlers

        public override void ClosingPlugin(XrmToolBox.PluginCloseInfo info)
        {
            if (!SaveIfChanged())
            {
                info.Cancel = true;
            }
            else
            {
                SaveSetting();
            }
        }

        private void FetchXmlBuilder_Load(object sender, EventArgs e)
        {
            LoadSetting();
            Initializing = false;
        }

        private void FetchXmlBuilder_ConnectionUpdated(object sender, ConnectionUpdatedEventArgs e)
        {
            View = null;
            if (!working)
            {
                LoadEntities();
            }
            EnableControls(buttonsEnabled);
        }

        /// <summary>When SiteMap component properties are saved, they arecopied in the current selected TreeNode</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void CtrlSaved(object sender, SaveEventArgs e)
        {
            tvFetch.SelectedNode.Tag = e.AttributeCollection;
            TreeNodeHelper.SetNodeText(tvFetch.SelectedNode);
            FetchChanged = treeChecksum != GetTreeChecksum(null);
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

        private void tsbEdit_Click(object sender, EventArgs e)
        {
            var xml = GetFetchString(false);
            var xcdDialog = new XmlContentDisplayDialog(xml, "FetchXML", true);
            xcdDialog.StartPosition = FormStartPosition.CenterParent;
            if (xcdDialog.ShowDialog() == DialogResult.OK)
            {
                EnableControls(false);
                XmlNode resultNode = xcdDialog.result;
                fetchDoc = new XmlDocument();
                fetchDoc.LoadXml(resultNode.OuterXml);
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
                    BuildAndValidateXml(true);
                }
            }
        }

        private void toolStripButtonExecute_Click(object sender, EventArgs e)
        {
            FetchResults(((ToolStripItem)sender).Tag.ToString());
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
            useFriendlyNames = tsmiFriendly.Checked;
            BuildAndValidateXml(false);
            DisplayDefinition();
            HandleNodeSelection(tvFetch.SelectedNode);
        }

        private void tsmiJSONresult_CheckedChanged(object sender, EventArgs e)
        {
            tsmiXMLresult.Checked = !tsmiJSONresult.Checked;
        }

        private void tsmiXMLresult_CheckedChanged(object sender, EventArgs e)
        {
            tsmiJSONresult.Checked = !tsmiXMLresult.Checked;
        }

        private void toolStripMain_Click(object sender, EventArgs e)
        {
            tvFetch.Focus();
        }

        private void tsbAbout_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "FetchXML Builder for XrmToolbox\n" +
                "Version: " + Assembly.GetExecutingAssembly().GetName().Version + "\n\n" +
                "Developed by Jonas Rapp at Cinteros AB.\n\n" +
                "Serialization to XML and JSON are custom developed to\n" +
                "be compact transports of CRM entity information.\n" +
                "There are deserialization methods as well...",
                "About FetchXML Builder", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void tsmiSaveCWPNew_Click(object sender, EventArgs e)
        {
            SaveCWPFeed();
        }

        private void tsmiEntities_Click(object sender, EventArgs e)
        {
            if (sender != tsmiEntitiesAll)
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
            showEntitiesAll = tsmiEntitiesAll.Checked;
            showEntitiesManaged = tsmiEntitiesManaged.Checked;
            showEntitiesUnmanaged = tsmiEntitiesUnmanaged.Checked;
            showEntitiesCustomizable = tsmiEntitiesCustomizable.Checked;
            showEntitiesUncustomizable = tsmiEntitiesUncustomizable.Checked;
            showEntitiesCustom = tsmiEntitiesCustom.Checked;
            showEntitiesStandard = tsmiEntitiesStandard.Checked;
            showEntitiesIntersect = tsmiEntitiesIntersect.Checked;
            showEntitiesOnlyValidAF = tsmiEntitiesOnlyValidAF.Checked;
            HandleNodeSelection(tvFetch.SelectedNode);
        }

        private void tsmiAttributes_Click(object sender, EventArgs e)
        {
            if (sender != tsmiAttributesAll)
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
            showAttributesAll = tsmiAttributesAll.Checked;
            showAttributesManaged = tsmiAttributesManaged.Checked;
            showAttributesUnmanaged = tsmiAttributesUnmanaged.Checked;
            showAttributesCustomizable = tsmiAttributesCustomizable.Checked;
            showAttributesUncustomizable = tsmiAttributesUncustomizable.Checked;
            showAttributesCustom = tsmiAttributesCustom.Checked;
            showAttributesStandard = tsmiAttributesStandard.Checked;
            showAttributesOnlyValidAF = tsmiAttributesOnlyValidAF.Checked;
            HandleNodeSelection(tvFetch.SelectedNode);
        }

        #endregion Event handlers

        #region Instance methods

        /// <summary>Saves various configurations to file for next session</summary>
        private void SaveSetting()
        {
            var xml = GetFetchString(false);
            var map = new ExeConfigurationFileMap { ExeConfigFilename = settingfile };
            System.Configuration.Configuration config = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
            config.AppSettings.Settings.Clear();
            if (!string.IsNullOrEmpty(FileName))
            {
                config.AppSettings.Settings.Add("Filename", FileName);
            }
            if (!string.IsNullOrWhiteSpace(xml))
            {
                config.AppSettings.Settings.Add("FetchXML", xml);
            }
            if (xmlWinSize != null)
            {
                config.AppSettings.Settings.Add("XmlWinWidth", xmlWinSize.Width.ToString());
                config.AppSettings.Settings.Add("XmlWinHeight", xmlWinSize.Height.ToString());
            }
            SaveControlValue(config, tsmiJSONresult);
            SaveControlValue(config, tsmiEntitiesManaged);
            SaveControlValue(config, tsmiEntitiesUnmanaged);
            SaveControlValue(config, tsmiEntitiesCustomizable);
            SaveControlValue(config, tsmiEntitiesUncustomizable);
            SaveControlValue(config, tsmiEntitiesCustom);
            SaveControlValue(config, tsmiEntitiesStandard);
            SaveControlValue(config, tsmiEntitiesIntersect);
            SaveControlValue(config, tsmiEntitiesOnlyValidAF);
            SaveControlValue(config, tsmiAttributesManaged);
            SaveControlValue(config, tsmiAttributesUnmanaged);
            SaveControlValue(config, tsmiAttributesCustomizable);
            SaveControlValue(config, tsmiAttributesUncustomizable);
            SaveControlValue(config, tsmiAttributesCustom);
            SaveControlValue(config, tsmiAttributesStandard);
            SaveControlValue(config, tsmiAttributesOnlyValidAF);
            config.Save();
        }

        private void SaveControlValue(Configuration config, object control)
        {
            if (control is ToolStripMenuItem)
            {
                config.AppSettings.Settings.Add(((ToolStripMenuItem)control).Name, ((ToolStripMenuItem)control).Checked ? "1" : "0");
            }
        }

        private void LoadControlValue(Configuration config, object control)
        {
            if (control is ToolStripMenuItem)
            {
                var name = ((ToolStripMenuItem)control).Name;
                if (config.AppSettings.Settings[name] != null)
                {
                    ((ToolStripMenuItem)control).Checked = config.AppSettings.Settings[name].Value == "1";
                }
            }
        }

        /// <summary>Loads configurations from file</summary>
        private void LoadSetting()
        {
            var map = new ExeConfigurationFileMap { ExeConfigFilename = settingfile };
            System.Configuration.Configuration config = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
            if (config.AppSettings.Settings["Filename"] != null)
            {
                FileName = config.AppSettings.Settings["Filename"].Value;
            }
            if (config.AppSettings.Settings["FetchXML"] != null)
            {
                var xml = config.AppSettings.Settings["FetchXML"].Value;
                fetchDoc = new XmlDocument();
                fetchDoc.LoadXml(xml);
                DisplayDefinition();
            }
            if (config.AppSettings.Settings["XmlWinWidth"] != null && config.AppSettings.Settings["XmlWinHeight"] != null)
            {
                var w = 0;
                var h = 0;
                if (int.TryParse(config.AppSettings.Settings["XmlWinWidth"].Value, out w) &&
                    int.TryParse(config.AppSettings.Settings["XmlWinHeight"].Value, out h))
                {
                    xmlWinSize = new Size(w, h);
                }
            }
            LoadControlValue(config, tsmiJSONresult);
            LoadControlValue(config, tsmiEntitiesManaged);
            LoadControlValue(config, tsmiEntitiesUnmanaged);
            LoadControlValue(config, tsmiEntitiesCustomizable);
            LoadControlValue(config, tsmiEntitiesUncustomizable);
            LoadControlValue(config, tsmiEntitiesCustom);
            LoadControlValue(config, tsmiEntitiesStandard);
            LoadControlValue(config, tsmiEntitiesIntersect);
            LoadControlValue(config, tsmiEntitiesOnlyValidAF);
            LoadControlValue(config, tsmiAttributesManaged);
            LoadControlValue(config, tsmiAttributesUnmanaged);
            LoadControlValue(config, tsmiAttributesCustomizable);
            LoadControlValue(config, tsmiAttributesUncustomizable);
            LoadControlValue(config, tsmiAttributesCustom);
            LoadControlValue(config, tsmiAttributesStandard);
            LoadControlValue(config, tsmiAttributesOnlyValidAF);
            tsmiEntities_Click(null, null);
            tsmiAttributes_Click(null, null);
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
                    tsmiOpenView.Enabled = enabled;
                    tsbSave.Enabled = enabled;
                    tsmiSaveFile.Enabled = enabled && FetchChanged && !string.IsNullOrEmpty(FileName);
                    tsmiSaveFileAs.Enabled = enabled && tvFetch.Nodes.Count > 0;
                    tsmiSaveView.Enabled = enabled && FetchChanged && View != null;
                    tsmiSaveCWPNew.Visible = enabled && Service != null && entities != null && entities.ContainsKey("cint_feed");
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
                    FileName = sfd.FileName;
                }
            }
            if (!string.IsNullOrEmpty(FileName))
            {
                EnableControls(false);
                BuildAndValidateXml();
                {
                    fetchDoc.Save(FileName);
                    treeChecksum = GetTreeChecksum(null);
                    FetchChanged = false;
                    if (!silent)
                    {
                        MessageBox.Show(this, "FetchXML saved!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    result = true;
                }
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
                        TreeNodeHelper.SetNodeText(tvFetch.SelectedNode);
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

        private void FetchResults(string fetchType)
        {
            if (!BuildAndValidateXml(true))
            {
                return;
            }
            if (working)
            {
                MessageBox.Show("Busy doing something...\n\nPlease wait until current transaction is done.");
                return;
            }

            switch (fetchType)
            {
                case "FetchRequest":
                    ExecuteFetch();
                    break;
                case "RetrieveMultiple":
                    RetrieveMultiple(tsmiJSONresult.Checked);
                    break;
                default:
                    MessageBox.Show("Invalid fetch method: " + fetchType, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                        var xcdDialog = new XmlContentDisplayDialog(doc.OuterXml, "FetchXML result", false);
                        xcdDialog.StartPosition = FormStartPosition.CenterParent;
                        xcdDialog.ShowDialog();
                    }
                });
        }

        private void RetrieveMultiple(bool ToJSON)
        {
            working = true;
            WorkAsync("Executing FetchXML...",
                (eventargs) =>
                {
                    var fetchxml = GetFetchDocument().OuterXml;
                    var resp = Service.RetrieveMultiple(new FetchExpression(fetchxml));
                    if (ToJSON)
                    {
                        var json = EntityCollectionSerializer.ToJSON(resp, Formatting.Indented);
                        eventargs.Result = json;
                    }
                    else
                    {
                        var serialized = EntityCollectionSerializer.Serialize(resp);
                        eventargs.Result = serialized;
                    }
                },
                (completedargs) =>
                {
                    working = false;
                    if (completedargs.Error != null)
                    {
                        MessageBox.Show(completedargs.Error.Message);
                    }
                    else if (completedargs.Result is XmlDocument)
                    {
                        var result = ((XmlDocument)completedargs.Result).OuterXml;
                        var xcdDialog = new XmlContentDisplayDialog(result, "XML Serialized RetrieveMultiple result", false);
                        xcdDialog.StartPosition = FormStartPosition.CenterParent;
                        xcdDialog.ShowDialog();
                    }
                    else if (completedargs.Result is string)
                    {
                        var result = completedargs.Result.ToString();
                        var xcdDialog = new XmlContentDisplayDialog(result, "JSON Serialized RetrieveMultiple result", false);
                        xcdDialog.StartPosition = FormStartPosition.CenterParent;
                        xcdDialog.ShowDialog();
                    }
                });
        }

        private void OpenView()
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
                    var viewselector = new SelectViewDialog(this);
                    viewselector.StartPosition = FormStartPosition.CenterParent;
                    if (viewselector.ShowDialog() == DialogResult.OK)
                    {
                        View = viewselector.View;
                        fetchDoc = new XmlDocument();
                        fetchDoc.LoadXml(View["fetchxml"].ToString());
                        DisplayDefinition();
                        attributesChecksum = GetAttributesSignature(null);
                    }
                }
            });
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

        private void SaveCWPFeed()
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
            var qeFeed = new QueryExpression("cint_feed");
            qeFeed.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            qeFeed.Criteria.AddCondition("cint_id", ConditionOperator.Equal, feedid);
            var feeds = Service.RetrieveMultiple(qeFeed);
            Entity feed = feeds.Entities.Count > 0 ? feeds.Entities[0] : new Entity("cint_feed");
            feed.Attributes.Add("cint_fetchxml", GetFetchString(true));
            var verb = feed.Id.Equals(Guid.Empty) ? "created" : "updated";
            if (feed.Id.Equals(Guid.Empty))
            {
                feed.Attributes.Add("cint_id", feedid);
                feed.Attributes.Add("cint_description", "Created by FetchXml Builder for XrmToolbox");
                Service.Create(feed);
            }
            else
            {
                Service.Update(feed);
            }
            MessageBox.Show("CWP Feed " + feedid + " has been " + verb + "!", "Save CWP Feed", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                    TreeNodeHelper.SetNodeText(attrNode);
                }
                FetchChanged = treeChecksum != GetTreeChecksum(null);
            }
        }

        #endregion Instance methods

        #region Static methods

        internal static string GetEntityDisplayName(string entityName)
        {
            if (!useFriendlyNames)
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
            if (useFriendlyNames)
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
            if (!useFriendlyNames)
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
            if (useFriendlyNames)
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
            var parent = node.Parent;
            while (parent != null && parent.Name != "fetch")
            {
                parent = parent.Parent;
            }
            if (parent != null && parent.Name == "fetch")
            {
                aggregate = TreeNodeHelper.GetAttributeFromNode(parent, "aggregate") == "true";
            }
            return aggregate;
        }

        internal static Dictionary<string, EntityMetadata> GetDisplayEntities()
        {
            var result = new Dictionary<string, EntityMetadata>();
            if (entities != null)
            {
                foreach (var entity in entities)
                {
                    if (!showEntitiesAll)
                    {
                        if (!showEntitiesManaged && entity.Value.IsManaged == true) { continue; }
                        if (!showEntitiesUnmanaged && entity.Value.IsManaged == false) { continue; }
                        if (!showEntitiesCustomizable && entity.Value.IsCustomizable.Value) { continue; }
                        if (!showEntitiesUncustomizable && !entity.Value.IsCustomizable.Value) { continue; }
                        if (!showEntitiesStandard && entity.Value.IsCustomEntity == false) { continue; }
                        if (!showEntitiesCustom && entity.Value.IsCustomEntity == true) { continue; }
                        if (!showEntitiesIntersect && entity.Value.IsIntersect == true) { continue; }
                        if (showEntitiesOnlyValidAF && entity.Value.IsValidForAdvancedFind == false) { continue; }
                    }
                    result.Add(entity.Key, entity.Value);
                }
            }
            return result;
        }

        internal static AttributeMetadata[] GetDisplayAttributes(string entityName)
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
                        if (!showAttributesAll)
                        {
                            if (!string.IsNullOrEmpty(attribute.AttributeOf)) { continue; }
                            if (!showAttributesManaged && attribute.IsManaged == true) { continue; }
                            if (!showAttributesUnmanaged && attribute.IsManaged == false) { continue; }
                            if (!showAttributesCustomizable && attribute.IsCustomizable.Value) { continue; }
                            if (!showAttributesUncustomizable && !attribute.IsCustomizable.Value) { continue; }
                            if (!showAttributesStandard && attribute.IsCustomAttribute == false) { continue; }
                            if (!showAttributesCustom && attribute.IsCustomAttribute == true) { continue; }
                            if (showAttributesOnlyValidAF && attribute.IsValidForAdvancedFind.Value == false) { continue; }
                        }
                        result.Add(attribute);
                    }
                }
            }
            return result.ToArray();
        }

        #endregion Static methods
    }
}
