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
        const string settingfile = "Cinteros.Xrm.FetchXmlBuilder.Settings.xml";
        internal Clipboard clipboard = new Clipboard();
        private bool Initializing = true;
        private XmlDocument fetchDoc;
        internal static Dictionary<string, EntityMetadata> entities;
        internal static List<string> entityShitList = new List<string>();
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
        private static int userLCID = 0;

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

        private void FetchXmlBuilder_Load(object sender, EventArgs e)
        {
            LoadSetting();
            Initializing = false;
        }

        private void FetchXmlBuilder_OnCloseTool(object sender, EventArgs e)
        {
            // Do things when the tool is closing
            SaveSetting();
        }

        private void FetchXmlBuilder_ConnectionUpdated(object sender, ConnectionUpdatedEventArgs e)
        {
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
            if (SaveIfChanged())
            {
                CloseToolPrompt();
            }
        }

        private void FetchXmlBuilder_Leave(object sender, EventArgs e)
        {
            //SaveSetting();
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
                Title = "Select an XML file containing Fetch XML",
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
            var xml = GetFetchString();
            var xcdDialog = new XmlContentDisplayDialog(xml, "Fetch XML", true);
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

        private void tvFetch_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            HandleNodeClick(e.Node);
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

        private void chkTechNames_CheckedChanged(object sender, EventArgs e)
        {
            useFriendlyNames = chkFriendlyNames.Checked;
            BuildAndValidateXml(false);
            DisplayDefinition();
        }

        #endregion Event handlers

        /// <summary>Saves various configurations to file for next session</summary>
        private void SaveSetting()
        {
            var xml = GetFetchString();
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
            config.Save();
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
        }

        private void DoThingsAsync()
        {
            if (Initializing)
            {
                return;
            }
            WorkAsync("Doing something...",
                (eventargs) =>
                {
                    //var crmsvc = new CrmServiceProxy(Service);
                    //var log = new PluginLogger("FetchXmlBuilder", true, "");
                    // Do more things
                    DoThingsThreadSafe();
                },
                (completedeventargs) =>
                {
                    if (completedeventargs.Error != null)
                    {
                        // Something went wrong
                    }
                });
        }

        private void DoThingsThreadSafe()
        {
            MethodInvoker mi = delegate
            {
                // Access controls on the form
                // And do things
            };
            if (InvokeRequired) Invoke(mi); else mi();
        }

        /// <summary>Enables or disables all buttons on the form</summary>
        /// <param name="enabled"></param>
        private void EnableControls(bool enabled)
        {
            MethodInvoker mi = delegate
            {
                tsbNew.Enabled = enabled;
                tsbEdit.Enabled = enabled;
                tsbExecute.Enabled = enabled && tvFetch.Nodes.Count > 0 && Service != null;
                tsbOpen.Enabled = enabled;
                tsmiOpenFile.Enabled = enabled;
                tsmiOpenView.Enabled = enabled;
                tsbSave.Enabled = enabled;
                tsmiSaveFile.Enabled = enabled && FetchChanged && !string.IsNullOrEmpty(FileName);
                tsmiSaveFileAs.Enabled = enabled && tvFetch.Nodes.Count > 0;
                tsmiSaveView.Enabled = enabled && FetchChanged && View != null;
                chkFriendlyNames.Enabled = enabled && tvFetch.Nodes.Count > 0 && Service != null;
                gbFetchTree.Enabled = enabled;
                gbProperties.Enabled = enabled;
                buttonsEnabled = enabled;
            };
            if (InvokeRequired) { Invoke(mi); } else { mi(); }
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

        private string GetFetchString()
        {
            var xml = "";
            if (tvFetch.Nodes.Count > 0)
            {
                var doc = GetFetchDocument();
                xml = doc.OuterXml;
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
                var result = MessageBox.Show("Fetch XML has changed.\nSave changes?", "Confirm", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
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
            else if (ClickedItem.Tag.ToString() == "Cut")
                clipboard.Cut(tvFetch.SelectedNode);
            else if (ClickedItem.Tag.ToString() == "Copy")
                clipboard.Copy(tvFetch.SelectedNode);
            else if (ClickedItem.Tag.ToString() == "Paste")
                clipboard.Paste(tvFetch.SelectedNode);
            else
            {
                string nodeText = ClickedItem.Tag.ToString();
                var newNode = TreeNodeHelper.AddChildNode(tvFetch.SelectedNode, nodeText);
                HandleNodeClick(newNode);
            }
            FetchChanged = treeChecksum != GetTreeChecksum(null);
        }

        private void HandleNodeClick(TreeNode node)
        {
            node.TreeView.SelectedNode = node;
            var collec = (Dictionary<string, string>)node.Tag;

            TreeNodeHelper.AddContextMenu(node, this);
            Control existingControl = panelContainer.Controls.Count > 0 ? panelContainer.Controls[0] : null;
            this.deleteToolStripMenuItem.Text = "Delete " + node.Name;

            UserControl ctrl = null;
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
                                AttributeMetadata[] attributes = null;
                                if (entities != null && entities.ContainsKey(entityName))
                                {
                                    attributes = entities[entityName].Attributes;
                                }
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
            HandleNodeClick(tvFetch.SelectedNode);
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
                    var whoamireq = (WhoAmIResponse)Service.Execute(new WhoAmIRequest());
                    var queryuser = new QueryByAttribute("usersettings");
                    queryuser.ColumnSet = new ColumnSet("uilanguageid");
                    queryuser.AddAttributeValue("systemuserid", whoamireq.UserId);
                    var result = Service.RetrieveMultiple(queryuser);
                    if (result.Entities.Count > 0 && result.Entities[0].Attributes.Contains("uilanguageid"))
                    {
                        userLCID = result.Entities[0].GetAttributeValue<int>("uilanguageid");
                    }

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

        //private void LoadUsedEntities()
        //{
        //    var usedEntities = new List<string>();
        //    foreach (TreeNode node in tvFetch.Nodes)
        //    {
        //        if (node.Name == "entity" || node.Name == "link-entity")
        //        {
        //            var entityname = TreeNodeHelper.GetAttributeFromNode(node, "name");
        //            if (NeedToLoadEntity(entityname))
        //            {
        //                LoadEntityDetails(entityname, LoadUsedEntities);
        //                return;
        //            }
        //        }
        //    }
        //}

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
                        detailsLoaded();
                    }
                    working = false;
                });
        }

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
                foreach (var label in entity.DisplayName.LocalizedLabels)
                {
                    if (label.LanguageCode == userLCID)
                    {
                        result = label.Label;
                        break;
                    }
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
                foreach (var label in attribute.DisplayName.LocalizedLabels)
                {
                    if (label.LanguageCode == userLCID)
                    {
                        attributeName = label.Label;
                        break;
                    }
                }
                if (attributeName == attribute.LogicalName && attribute.DisplayName.LocalizedLabels.Count > 0)
                {
                    attributeName = attribute.DisplayName.LocalizedLabels[0].Label;
                }
            }
            return attributeName;
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

        private void FetchResults(string fetchType)
        {
            if (!BuildAndValidateXml(true))
            {
                return;
            }
            if (working)
            {
                MessageBox.Show("Please wait until current transaction is done.");
                return;
            }

            switch (fetchType)
            {
                case "FetchRequest":
                    ExecuteFetch();
                    break;
                case "RetrieveMultiple":
                    RetrieveMultiple();
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
                        var xcdDialog = new XmlContentDisplayDialog(doc.OuterXml, "Fetch XML result", false);
                        xcdDialog.StartPosition = FormStartPosition.CenterParent;
                        xcdDialog.ShowDialog();
                    }
                });
        }

        private void RetrieveMultiple()
        {
            working = true;
            WorkAsync("Executing FetchXML...",
                (eventargs) =>
                {
                    var fetchxml = GetFetchDocument().OuterXml;
                    var resp = Service.RetrieveMultiple(new FetchExpression(fetchxml));
                    var serialized = EntityCollectionSerializer.Serialize(resp);
                    eventargs.Result = serialized.OuterXml;
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
                        var result = completedargs.Result.ToString();
                        var xcdDialog = new XmlContentDisplayDialog(result, "Serialized FetchExpression result", false);
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
                    var xml = GetFetchString();
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
    }
}
