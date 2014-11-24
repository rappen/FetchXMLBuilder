using Cinteros.Xrm.FetchXmlBuilder.AppCode;
using Cinteros.Xrm.FetchXmlBuilder.Controls;
using Cinteros.Xrm.FetchXmlBuilder.Forms;
using Cinteros.Xrm.XmlEditorUtils;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
using XrmToolBox;
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
        private static string fetchTemplate = "<fetch><entity name=\"\"/></fetch>";
        private string fileName;
        internal bool working = false;
        internal static bool useFriendlyNames = false;
        private string treeChecksum = "";
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
        private bool buttonsEnabled = false;
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
            SaveSetting();
        }

        private void toolStripButtonNew_Click(object sender, EventArgs e)
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

        private void toolStripButtonOpen_Click(object sender, EventArgs e)
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

                    toolStripButtonOpen.Enabled = true;
                }
                else
                {
                    //LoadUsedEntities();
                    fileName = ofd.FileName;
                    DisplayDefinition();
                    treeChecksum = GetTreeChecksum(null);
                    FetchChanged = false;
                    EnableControls(true);
                }
            }
        }

        private void toolStripButtonPaste_Click(object sender, EventArgs e)
        {
            if (!SaveIfChanged())
            {
                return;
            }
            var axForm = new AddXmlForm();
            axForm.StartPosition = FormStartPosition.CenterParent;

            if (axForm.ShowDialog() == DialogResult.OK)
            {
                EnableControls(false);
                XmlNode resultNode = axForm.AddedXmlNode;
                fetchDoc = new XmlDocument();
                fetchDoc.LoadXml(resultNode.OuterXml);
                treeChecksum = "";
                if (fetchDoc.DocumentElement.Name != "fetch" ||
                    fetchDoc.DocumentElement.ChildNodes.Count > 0 &&
                    fetchDoc.DocumentElement.ChildNodes[0].Name == "fetch")
                {
                    MessageBox.Show(this, "Invalid Xml: Definition XML root must be fetch!", "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                    toolStripButtonOpen.Enabled = true;
                }
                else
                {
                    //LoadUsedEntities();
                    fileName = "";
                    DisplayDefinition();
                    FetchChanged = true;
                    EnableControls(true);
                }
            }
        }

        private void toolStripButtonView_Click(object sender, EventArgs e)
        {
            var doc = GetFetchDocument();
            var xcdDialog = new XmlContentDisplayDialog(doc.OuterXml, "Fetch XML");
            xcdDialog.StartPosition = FormStartPosition.CenterParent;
            xcdDialog.ShowDialog();
        }

        private void toolStripButtonValidate_Click(object sender, EventArgs e)
        {
            if (BuildAndValidateXml())
            {
                MessageBox.Show("Fetch validated ok!");
            }
        }

        private void toolStripButtonExecute_Click(object sender, EventArgs e)
        {
            ExecuteFetch();
        }

        private void toolStripButtonSave_Click(object sender, EventArgs e)
        {
            SaveFetchXML(false, false);
        }

        private void toolStripButtonSaveAs_Click(object sender, EventArgs e)
        {
            SaveFetchXML(true, false);
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
            toolStripButtonMoveDown.Enabled = false;
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
            toolStripButtonMoveDown.Enabled = true;
        }

        private void toolStripButtonMoveUp_Click(object sender, EventArgs e)
        {
            toolStripButtonMoveUp.Enabled = false;
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
            toolStripButtonMoveUp.Enabled = true;
        }

        private void tsbItemSave_Click(object sender, EventArgs e)
        {
            HandleControlSaved();
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
            var map = new ExeConfigurationFileMap { ExeConfigFilename = settingfile };
            System.Configuration.Configuration config = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
            config.AppSettings.Settings.Clear();
            config.AppSettings.Settings.Add("mySetting", "myValue");
            config.Save();
        }

        /// <summary>Loads configurations from file</summary>
        private void LoadSetting()
        {
            var map = new ExeConfigurationFileMap { ExeConfigFilename = settingfile };
            System.Configuration.Configuration config = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
            var myValue = config.AppSettings.Settings["mySetting"];
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
                toolStripButtonNew.Enabled = enabled;
                toolStripButtonOpen.Enabled = enabled;
                toolStripButtonPaste.Enabled = enabled;
                toolStripButtonView.Enabled = enabled && tvFetch.Nodes.Count > 0;
                toolStripButtonValidate.Enabled = enabled && tvFetch.Nodes.Count > 0;
                toolStripButtonExecute.Enabled = enabled && tvFetch.Nodes.Count > 0 && Service != null;
                toolStripButtonSave.Enabled = enabled && FetchChanged && !string.IsNullOrEmpty(fileName);
                toolStripButtonSaveAs.Enabled = enabled;
                chkFriendlyNames.Enabled = enabled && tvFetch.Nodes.Count > 0 && Service != null;
                gbSiteMap.Enabled = enabled;
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
                tvFetch.Nodes[0].Expand();
                if (tvFetch.Nodes[0].Nodes.Count > 0)
                {
                    tvFetch.Nodes[0].Nodes[0].Expand();
                }
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
            tsbItemSave.Enabled = selectedNode != null;
            toolStripButtonMoveUp.Enabled = selectedNode != null && selectedNode.Parent != null &&
                                            selectedNode.Index != 0;
            toolStripButtonMoveDown.Enabled = selectedNode != null && selectedNode.Parent != null &&
                                              selectedNode.Index != selectedNode.Parent.Nodes.Count - 1;
        }

        private XmlDocument GetFetchDocument()
        {
            var doc = new XmlDocument();
            //if (tvFetch.Nodes.Count > 0)
            {
                XmlNode rootNode = doc.CreateElement("root");
                doc.AppendChild(rootNode);
                TreeNodeHelper.AddXmlNode(tvFetch.Nodes[0], rootNode);
                var xmlbody = doc.SelectSingleNode("root/fetch").OuterXml;
                doc.LoadXml(xmlbody);
            }
            return doc;
        }

        private bool BuildAndValidateXml(bool validate = true)
        {
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
            if (prompt || string.IsNullOrEmpty(fileName))
            {
                var sfd = new SaveFileDialog
                {
                    Title = "Select a location to save the FetchXML",
                    Filter = "Xml file (*.xml)|*.xml",
                    FileName = System.IO.Path.GetFileName(fileName)
                };
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    fileName = sfd.FileName;
                }
            }
            if (!string.IsNullOrEmpty(fileName))
            {
                EnableControls(false);

                BuildAndValidateXml();
                {
                    fetchDoc.Save(fileName);
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
                    ctrl = new entityControl(collec, entities, this);
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
                        tsbItemSave.Visible = false;
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
                tsbItemSave.Visible = true;
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

        private void HandleControlSaved()
        {
            ((IDefinitionSavable)panelContainer.Controls[0]).Save();
            var nodeAttributesCollection = (Dictionary<string, string>)tvFetch.SelectedNode.Tag;
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

        private void ExecuteFetch()
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
                        var xcdDialog = new XmlContentDisplayDialog(doc.OuterXml, "Fetch XML result");
                        xcdDialog.StartPosition = FormStartPosition.CenterParent;
                        xcdDialog.ShowDialog();
                    }
                });
        }
    }
}
