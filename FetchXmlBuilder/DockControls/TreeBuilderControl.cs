using Cinteros.Xrm.FetchXmlBuilder.AppCode;
using Cinteros.Xrm.FetchXmlBuilder.Controls;
using Cinteros.Xrm.FetchXmlBuilder.Forms;
using Cinteros.Xrm.XmlEditorUtils;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Cinteros.Xrm.FetchXmlBuilder.DockControls
{
    public partial class TreeBuilderControl : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        #region Private Fields

        private static string fetchTemplate = "<fetch top=\"50\"><entity name=\"\"/></fetch>";
        private bool fetchChanged = false;
        private FetchXmlBuilder fxb;
        private HistoryManager historyMgr = new HistoryManager();
        private string treeChecksum = "";

        #endregion Private Fields

        #region Public Constructors

        public TreeBuilderControl(FetchXmlBuilder owner)
        {
            fxb = owner;
            InitializeComponent();
        }

        #endregion Public Constructors

        #region Internal Properties

        internal bool FetchChanged
        {
            get { return fetchChanged; }
            set
            {
                fetchChanged = value;
                fxb.EnableControls();
                //toolStripButtonSave.Enabled = value;
            }
        }

        internal int SplitterPos
        {
            get => splitContainer1.SplitterDistance;
            set
            {
                if (value > -1)
                {
                    splitContainer1.SplitterDistance = value;
                }
            }
        }

        #endregion Internal Properties

        #region Internal Methods

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

        internal void ApplyCurrentSettings()
        {
            BuildAndValidateXml(false);
            DisplayDefinition(GetFetchDocument());
            HandleNodeSelection(tvFetch.SelectedNode);
            fxb.UpdateLiveXML();
            ShowQuickActions(fxb.settings.QueryOptions.ShowQuickActions);
        }

        internal void ClearChanged()
        {
            treeChecksum = GetTreeChecksum(null);
            FetchChanged = false;
        }

        /// <summary>When SiteMap component properties are saved, they arecopied in the current selected TreeNode</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void CtrlSaved(object sender, SaveEventArgs e)
        {
            if (tvFetch?.SelectedNode == null)
            {
                return;
            }
            tvFetch.SelectedNode.Tag = e.AttributeCollection;
            TreeNodeHelper.SetNodeText(tvFetch.SelectedNode, FetchXmlBuilder.friendlyNames);
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
            fxb.UpdateLiveXML();
        }

        internal void EnableControls(bool enabled)
        {
            selectAttributesToolStripMenuItem.Enabled = enabled && fxb.Service != null;
            tvFetch.Enabled = enabled;
            gbProperties.Enabled = enabled;
        }

        internal string GetAttributesSignature(XmlNode entity)
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

        internal string GetFetchString(bool format, bool validate)
        {
            var xml = string.Empty;
            if (BuildAndValidateXml(validate))
            {
                if (tvFetch.Nodes.Count > 0)
                {
                    var doc = GetFetchDocument();
                    xml = doc.OuterXml;
                    if (fxb.settings.QueryOptions.UseSingleQuotation)
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
            }
            return xml;
        }

        internal FetchType GetFetchType()
        {
            var fetchstr = GetFetchString(false, false);
            var serializer = new XmlSerializer(typeof(FetchType));
            object result;
            using (TextReader reader = new StringReader(fetchstr))
            {
                result = serializer.Deserialize(reader);
            }
            return result as FetchType;
        }

        internal QueryExpression GetQueryExpression(string fetch = null, bool validate = true)
        {
            if (fxb.Service == null)
            {
                throw new Exception("Must be connected to CRM to convert to QueryExpression.");
            }
            if (string.IsNullOrWhiteSpace(fetch))
            {
                if (IsFetchAggregate(tvFetch.Nodes.Count > 0 ? tvFetch.Nodes[0] : null))
                {
                    throw new FetchIsAggregateException("QueryExpression does not support aggregate queries.");
                }
                fetch = GetFetchString(false, validate);
            }
            var convert = (FetchXmlToQueryExpressionResponse)fxb.Service.Execute(new FetchXmlToQueryExpressionRequest() { FetchXml = fetch });
            return convert.Query;
        }

        internal void Init(string fetchStr, string action, bool validate)
        {
            ParseXML(fetchStr, validate);
            fxb.UpdateLiveXML();
            ClearChanged();
            fxb.EnableControls(true);
            if (!string.IsNullOrWhiteSpace(action))
            {
                RecordHistory(action);
            }
        }

        internal void ParseXML(string xml, bool validate)
        {
            if (string.IsNullOrWhiteSpace(xml))
            {
                xml = fetchTemplate;
            }
            var fetchDoc = new XmlDocument();
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
                DisplayDefinition(fetchDoc);
                FetchChanged = true;
                fxb.EnableControls(true);
                BuildAndValidateXml(validate);
            }
        }

        internal void RecordHistory(string action)
        {
            var fetch = GetFetchString(false, false);
            historyMgr.RecordHistory(action, fetch);
            fxb.EnableDisableHistoryButtons(historyMgr);
        }

        internal void RestoreHistoryPosition(int delta)
        {
            fxb.LogUse(delta < 0 ? "Undo" : "Redo");
            var fetch = historyMgr.RestoreHistoryPosition(delta) as string;
            if (fetch != null)
            {
                ParseXML(fetch, false);
                RefreshSelectedNode();
            }
            fxb.EnableDisableHistoryButtons(historyMgr);
        }

        internal void Save(string fileName)
        {
            BuildAndValidateXml();
            var fetchDoc = GetFetchDocument();
            fetchDoc.Save(fileName);
            ClearChanged();
        }

        internal void SetFetchName(string name)
        {
            TabText = "Query Builder" + (string.IsNullOrWhiteSpace(name) ? "" : " - ") + name;
        }

        internal void ShowQuickActions(bool show)
        {
            panQuickActions.Visible = show;
        }

        internal void UpdateCurrentNode()
        {
            TreeNodeHelper.SetNodeText(tvFetch.SelectedNode, FetchXmlBuilder.friendlyNames);
        }

        #endregion Internal Methods

        #region Private Methods

        private bool BuildAndValidateXml(bool validate = true)
        {
            if (tvFetch.Nodes.Count == 0)
            {
                return false;
            }
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
                        var fetchDoc = GetFetchDocument();
                        fetchDoc.Schemas.Add(null, XmlReader.Create(stream));
                        fetchDoc.Validate(null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "FetchXML Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    result = ex.Message;
                }
            }
            return string.IsNullOrEmpty(result);
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

        private TreeNode DeleteNode()
        {
            var node = tvFetch.SelectedNode;
            var updateNode = node.Parent;
            node.Remove();
            RecordHistory("delete " + node.Name);
            return updateNode;
        }

        private void DisplayDefinition(XmlDocument fetchDoc)
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

        private void HandleNodeMenuClick(string ClickedTag)
        {
            if (ClickedTag == null || ClickedTag == "Add")
                return;
            TreeNode updateNode = null;
            if (ClickedTag == "Delete")
            {
                updateNode = DeleteNode();
            }
            else if (ClickedTag == "Comment")
            {
                CommentNode();
            }
            else if (ClickedTag == "Uncomment")
            {
                UncommentNode();
            }
            else if (ClickedTag == "SelectAttributes")
            {
                SelectAttributes();
            }
            else
            {
                string nodeText = ClickedTag;
                updateNode = TreeNodeHelper.AddChildNode(tvFetch.SelectedNode, nodeText);
                RecordHistory("add " + updateNode.Name);
                HandleNodeSelection(updateNode);
            }
            if (updateNode != null)
            {
                TreeNodeHelper.SetNodeTooltip(updateNode);
            }
            FetchChanged = treeChecksum != GetTreeChecksum(null);
            fxb.UpdateLiveXML();
        }

        private void HandleNodeSelection(TreeNode node)
        {
            if (!fxb.working)
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
                            ctrl = new entityControl(collec, fxb, this);
                            break;

                        case "link-entity":
                            if (node.Parent != null)
                            {
                                switch (node.Parent.Name)
                                {
                                    case "entity":
                                    case "link-entity":
                                        var entityName = TreeNodeHelper.GetAttributeFromNode(node.Parent, "name");
                                        if (fxb.NeedToLoadEntity(entityName))
                                        {
                                            if (!fxb.working)
                                            {
                                                fxb.LoadEntityDetails(entityName, RefreshSelectedNode);
                                            }
                                            break;
                                        }
                                        break;
                                }
                            }
                            var linkEntityName = TreeNodeHelper.GetAttributeFromNode(node, "name");
                            if (fxb.NeedToLoadEntity(linkEntityName))
                            {
                                if (!fxb.working)
                                {
                                    fxb.LoadEntityDetails(linkEntityName, RefreshSelectedNode);
                                }
                                break;
                            }
                            ctrl = new linkEntityControl(node, fxb, this);
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
                                        if (fxb.NeedToLoadEntity(entityName))
                                        {
                                            if (!fxb.working)
                                            {
                                                fxb.LoadEntityDetails(entityName, RefreshSelectedNode);
                                            }
                                            break;
                                        }
                                        AttributeMetadata[] attributes = fxb.GetDisplayAttributes(entityName);
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
                            ctrl = new conditionControl(node, fxb, this);
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

        private void HandleTVKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (deleteToolStripMenuItem.Enabled)
                {
                    if (MessageBox.Show(deleteToolStripMenuItem.Text + " ?", "Confirm", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                    {
                        HandleNodeMenuClick(deleteToolStripMenuItem.Tag?.ToString());
                    }
                }
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Insert)
            {
                addMenu.Show(tvFetch.PointToScreen(tvFetch.Location));
            }
            else if (e.Control && e.KeyCode == Keys.K && commentToolStripMenuItem.Enabled)
            {
                HandleNodeMenuClick(commentToolStripMenuItem.Tag?.ToString());
            }
            else if (e.Control && e.KeyCode == Keys.U && uncommentToolStripMenuItem.Enabled)
            {
                HandleNodeMenuClick(uncommentToolStripMenuItem.Tag?.ToString());
            }
            else if (e.Control && e.KeyCode == Keys.Up && moveUpToolStripMenuItem.Enabled)
            {
                toolStripButtonMoveUp_Click(null, null);
            }
            else if (e.Control && e.KeyCode == Keys.Down && moveDownToolStripMenuItem.Enabled)
            {
                toolStripButtonMoveDown_Click(null, null);
            }
        }

        private void ManageMenuDisplay()
        {
            TreeNode selectedNode = tvFetch.SelectedNode;
            moveUpToolStripMenuItem.Enabled = selectedNode != null && selectedNode.Parent != null &&
                                            selectedNode.Index != 0;
            moveDownToolStripMenuItem.Enabled = selectedNode != null && selectedNode.Parent != null &&
                                              selectedNode.Index != selectedNode.Parent.Nodes.Count - 1;
        }

        private void RefreshSelectedNode()
        {
            HandleNodeSelection(tvFetch.SelectedNode);
        }

        private void SelectAttributes()
        {
            if (fxb.Service == null)
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
            if (fxb.NeedToLoadEntity(entityName))
            {
                fxb.LoadEntityDetails(entityName, SelectAttributes);
                return;
            }
            var attributes = new List<AttributeMetadata>(fxb.GetDisplayAttributes(entityName));
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
                    TreeNodeHelper.SetNodeText(attrNode, fxb.settings.UseFriendlyNames);
                }
                FetchChanged = treeChecksum != GetTreeChecksum(null);
                fxb.UpdateLiveXML();
                RecordHistory("select attributes");
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

        #endregion Private Methods

        #region Control Event Handlers

        internal void QuickActionLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            HandleNodeMenuClick((sender as LinkLabel)?.Tag?.ToString());
        }

        private void nodeMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            HandleNodeMenuClick(e.ClickedItem.Tag?.ToString());
        }

        private void toolStripButtonMoveDown_Click(object sender, EventArgs e)
        {
            moveDownToolStripMenuItem.Enabled = false;
            fxb.working = true;
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
                    fxb.UpdateLiveXML();
                    RecordHistory("move down " + tnmNode.Name);
                }
            }
            fxb.working = false;
            moveDownToolStripMenuItem.Enabled = true;
        }

        private void toolStripButtonMoveUp_Click(object sender, EventArgs e)
        {
            moveUpToolStripMenuItem.Enabled = false;
            fxb.working = true;
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
                    fxb.UpdateLiveXML();
                    RecordHistory("move up " + tnmNode.Name);
                }
            }
            fxb.working = false;
            moveUpToolStripMenuItem.Enabled = true;
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

        #endregion Control Event Handlers
    }
}