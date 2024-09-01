using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using Rappen.XRM.Helpers.FetchXML;
using Rappen.XRM.Helpers.Interfaces;
using Rappen.XTB.FetchXmlBuilder.Builder;
using Rappen.XTB.FetchXmlBuilder.Controls;
using Rappen.XTB.FetchXmlBuilder.Extensions;
using Rappen.XTB.FetchXmlBuilder.Forms;
using Rappen.XTB.FetchXmlBuilder.Views;
using Rappen.XTB.Helpers;
using Rappen.XTB.XmlEditorUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Rappen.XTB.FetchXmlBuilder.DockControls
{
    public partial class TreeBuilderControl : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        #region Private Fields

        private bool fetchChanged = false;
        private FetchXmlBuilder fxb;
        private string treeChecksum = "";
        private FetchXmlElementControlBase ctrl;
        private LayoutXML layoutxml;
        private string layoutxmloriginal;

        #endregion Private Fields

        #region Public Constructors

        public TreeBuilderControl(FetchXmlBuilder owner)
        {
            fxb = owner;
            InitializeComponent();
            panQuickActions.PrepareGroupBoxExpanders();
            lblQAExpander.GroupBoxSetState(tt, fxb.settings.QueryOptions.ShowQuickActions);
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

        internal LayoutXML LayoutXML
        {
            get
            {
                return fxb.settings.Results.WorkWithLayout ? layoutxml : null;
            }
            set
            {
                layoutxml = fxb.settings.Results.WorkWithLayout ? value : null;
            }
        }

        #endregion Internal Properties

        #region Internal Methods

        internal bool IsFetchAggregate()
        {
            return tvFetch.Nodes.Cast<TreeNode>().FirstOrDefault().IsFetchAggregate();
        }

        internal void ApplyCurrentSettings()
        {
            if (IsDisposed)
            {
                return;
            }
            BuildAndValidateXml(false);
            DisplayDefinition(GetFetchDocument());
            HandleNodeSelection(tvFetch.SelectedNode);
            fxb.UpdateLiveXML();
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
            TreeNodeHelper.SetNodeText(tvFetch.SelectedNode, fxb);
            FetchChanged = treeChecksum != GetTreeChecksum(null);

            if (!e.KeyPress)
            {
                var origin = "";
                if (sender is IDefinitionSavable)
                {
                    origin = sender.ToString().Replace("Rappen.XTB.FetchXmlBuilder.Controls.", "").Replace("Control", "");
                    foreach (var attr in e.AttributeCollection)
                    {
                        origin += "\n  " + attr.Key + "=" + attr.Value;
                    }
                }
                RecordHistory(origin);
            }

            fxb.UpdateLiveXML();
        }

        internal void EnableControls(bool enabled)
        {
            selectAttributesToolStripMenuItem.Enabled = enabled && fxb.Service != null;
            tvFetch.Enabled = enabled;
            gbProperties.Enabled = enabled;
        }

        internal IEnumerable<TreeNode> GetAllLayoutValidAttributes(TreeNode entity = null)
        {
            var result = new List<TreeNode>();
            if (entity == null)
            {
                entity = RootEntityNode;
            }
            if (entity != null)
            {
                var entityalias = entity.Name == "link-entity" ? entity.Value("alias") : string.Empty;
                result.AddRange(entity.Nodes.Cast<TreeNode>().Where(a => a.IsAttributeValidForView()));
                var linkEntities = entity.Nodes.Cast<TreeNode>().Where(e => e.Name == "link-entity");
                foreach (var link in linkEntities)
                {
                    result.AddRange(GetAllLayoutValidAttributes(link));
                }
            }
            return result;
        }

        internal TreeNode GetAttributeNodeFromLayoutName(string attributelayoutname)
        {
            return GetAllLayoutValidAttributes().FirstOrDefault(a => a.GetAttributeLayoutName().Equals(attributelayoutname));
        }

        internal string GetAttributesSignature()
        {
            return string.Join("\n", GetAllLayoutValidAttributes().Select(a => a.GetAttributeLayoutName()));
        }

        internal FetchXmlElementControlBase GetCurrentControl()
        {
            return panelContainer.Controls.Cast<FetchXmlElementControlBase>().FirstOrDefault();
        }

        internal TreeNode RootEntityNode =>
            tvFetch.Nodes.Cast<TreeNode>()?
                .FirstOrDefault(n => n.Name == "fetch")?
                .Nodes.Cast<TreeNode>()?
                .FirstOrDefault(n => n.Name == "entity");

        internal EntityMetadata RootEntityMetadata => fxb.GetEntity(RootEntityName);

        internal string RootEntityName => RootEntityNode?.Value("name");

        internal string PrimaryIdName => RootEntityMetadata?.PrimaryIdAttribute;

        internal TreeNode PrimaryIdNode =>
            tvFetch.Nodes.Cast<TreeNode>()?
               .FirstOrDefault(n => n.Name == "fetch")?
                .Nodes.Cast<TreeNode>()?
                .FirstOrDefault(n => n.Name == "entity")?
                .Nodes.Cast<TreeNode>()?
                .FirstOrDefault(n => n.Name == "attribute" && n.Value("name") == PrimaryIdName);

        private int GetUniqueLinkEntitySuffix(TreeNode entity)
        {
            var root = tvFetch.Nodes.Cast<TreeNode>().FirstOrDefault(n => n.Name == "fetch");
            var links = 0;

            FindLinkElement(root, ref links, entity);

            return links;
        }

        private bool FindLinkElement(TreeNode node, ref int links, TreeNode find)
        {
            if (node != null && node.Name == "link-entity" && string.IsNullOrWhiteSpace(node.Value("alias")))
            {
                links++;
            }

            if (node == find)
            {
                return true;
            }

            foreach (TreeNode child in node.Nodes)
            {
                if (FindLinkElement(child, ref links, find))
                {
                    return true;
                }
            }

            return false;
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
                    {   // #122 Not sure why this is done... and it messes up commented xml using single quotation
                        xml = xml.Replace("'", "&apos;");
                        xml = xml.Replace("\"", "'");
                    }
                }
                if (format)
                {
                    xml = XDocument.Parse(xml).ToString();
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

        internal MetadataBase SelectedMetadata()
        {
            return ctrl?.Metadata();
        }

        internal QueryExpression GetQueryExpression(bool validate = true)
        {
            if (fxb.Service == null)
            {
                throw new Exception("Must be connected to Dataverse to convert to QueryExpression.");
            }
            var fetchdoc = GetFetchDocument();
            var fetch = fetchdoc.OuterXml;
            if (TreeNodeHelper.IsFetchAggregate(fetch))
            {
                throw new FetchIsAggregateException("QueryExpression does not support aggregate queries.");
            }
            var query = ((FetchXmlToQueryExpressionResponse)fxb.Execute(new FetchXmlToQueryExpressionRequest() { FetchXml = fetch })).Query;
            if (fetchdoc.SelectSingleNode("fetch").AttributeBool("no-lock") == true && !query.NoLock)
            {
                query.NoLock = true;
            }
            return query;
        }

        internal void Init(string fetchStr, string layoutStr, bool layoutisfromview, string action, bool validate)
        {
            ParseXML(fetchStr, validate);
            layoutxmloriginal = layoutisfromview ? layoutStr : string.Empty;
            ResetLayout(layoutStr);
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
                xml = fxb.settings.QueryOptions.NewQueryTemplate;
            }
            var fetchDoc = new XmlDocument();
            try
            {
                fetchDoc.LoadXml(xml);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Invalid XML: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            treeChecksum = "";
            if (fetchDoc.DocumentElement.Name != "fetch" ||
                fetchDoc.DocumentElement.ChildNodes.Count > 0 &&
                fetchDoc.DocumentElement.ChildNodes[0].Name == "fetch")
            {
                MessageBox.Show(this, "Invalid XML: Definition XML root must be fetch!", "Error",
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

        internal void ResetLayout(string layoutxml = null)
        {
            //           fxb.dockControlLayoutXml?.Close();
            if (string.IsNullOrWhiteSpace(layoutxml))
            {
                layoutxml = layoutxmloriginal;
            }
            LayoutXML = fxb.settings.Results.WorkWithLayout && !string.IsNullOrWhiteSpace(layoutxml)
                ? new LayoutXML(layoutxml, fxb) : null;
        }

        internal void SetLayoutFromXML(string layoutxml)
        {
            LayoutXML = new LayoutXML(layoutxml, fxb);
            if (GetCurrentControl() is attributeControl attrcontrol)
            {
                attrcontrol.UpdateUIFromCell();
            }
        }

        internal void RecordHistory(string action)
        {
            var fetch = GetFetchString(false, false);
            fxb.historyMgr.RecordHistory(action, fetch);
            fxb.EnableDisableHistoryButtons();
        }

        internal void RestoreHistoryPosition(int delta)
        {
            fxb.LogUse(delta < 0 ? "Undo" : "Redo");
            var fetch = fxb.historyMgr.RestoreHistoryPosition(delta) as string;
            if (fetch != null)
            {
                ParseXML(fetch, false);
                RefreshSelectedNode();
                fxb.UpdateLiveXML();
            }
            fxb.EnableDisableHistoryButtons();
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

        internal void UpdateCurrentNode()
        {
            TreeNodeHelper.SetNodeText(tvFetch.SelectedNode, fxb);
        }

        internal void UpdateChildNode(TreeNode node)
        {
            TreeNodeHelper.SetNodeText(node, fxb);
            node.Nodes.OfType<TreeNode>().ToList().ForEach(n => UpdateChildNode(n));
        }

        internal void UpdateAllNode()
        {
            tvFetch.Nodes.OfType<TreeNode>().ToList().ForEach(n => UpdateChildNode(n));
        }

        internal void UpdateLayoutXML()
        {
            if (LayoutXML == null)
            {
                return;
            }
            fxb.dockControlLayoutXml?.UpdateXML(LayoutXML?.ToXML());
            if (GetCurrentControl() is attributeControl attrcontrol)
            {
                attrcontrol.UpdateUIFromCell();
            }
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
                    var fetchDoc = GetFetchDocument();
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
                TreeNodeHelper.AddTreeViewNode(node.Parent, node, commentNode, fxb, true);
                RecordHistory("comment");
            }
        }

        private TreeNode DeleteNode()
        {
            var node = tvFetch.SelectedNode;
            var updateNode = node.Parent;
            node.Remove();
            TreeNodeHelper.Validate(updateNode, fxb);
            RecordHistory("delete " + node.Name);
            return updateNode;
        }

        private void DisplayDefinition(XmlDocument fetchDoc)
        {
            if (fetchDoc == null)
            {
                return;
            }
            if (fxb.entities != null)
            {
                var entitys = TreeNodeHelper.GetEntitysForFetch(fetchDoc);
                entitys = entitys?.Where(e => !string.IsNullOrEmpty(e) && fxb.GetEntity(e)?.Attributes == null)?.ToList();
                entitys?.ForEach(e => fxb.LoadEntityDetails(e, null, false, false));
            }
            XmlNode definitionXmlNode = fetchDoc.DocumentElement;
            var selected = tvFetch.SelectedNode;
            TreeNodeHelper.AddTreeViewNode(null, tvFetch.Nodes?.Count > 0 ? tvFetch.Nodes[0] : null, definitionXmlNode, fxb, true);
            tvFetch.ExpandAll();
            if (selected != null)
            {
                tvFetch.SelectedNode = selected;
            }
            else if (tvFetch.Nodes.Count > 0)
            {
                tvFetch.SelectedNode = tvFetch.Nodes[0];
            }
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

        internal string GetTreeChecksum(TreeNode node)
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
            else if (ClickedTag.StartsWith("MORE-"))
            {
                var nodename = ClickedTag.Substring(5);
                updateNode = TreeNodeHelper.AddChildNode(tvFetch.SelectedNode.Parent, nodename, fxb, tvFetch.SelectedNode);
                RecordHistory("add " + updateNode.Name);
                HandleNodeSelection(updateNode);
                ctrl.Focus();
            }
            else
            {
                updateNode = TreeNodeHelper.AddChildNode(tvFetch.SelectedNode, ClickedTag, fxb);
                RecordHistory("add " + updateNode.Name);
                HandleNodeSelection(updateNode);
                if (fxb.settings.AddConditionToFilter && ClickedTag.Equals("filter"))
                {
                    HandleNodeMenuClick("condition");
                    return;
                }
                ctrl?.Focus();
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

                ctrl = null;
                Control existingControl = panelContainer.Controls.Count > 0 ? panelContainer.Controls[0] : null;
                if (node != null)
                {
                    TreeNodeHelper.AddContextMenu(node, this, fxb.settings.QueryOptions);
                    this.deleteToolStripMenuItem.Text = "Delete " + node.Name;
                    var collec = (Dictionary<string, string>)node.Tag;

                    switch (node.Name)
                    {
                        case "fetch":
                            ctrl = new fetchControl(collec, fxb, this);
                            break;

                        case "entity":
                            ctrl = new entityControl(collec, fxb, this);
                            break;

                        case "link-entity":
                            if (node.Parent != null && node.Parent.LocalEntityName() is string parententity && fxb.NeedToLoadEntity(parententity))
                            {
                                if (!fxb.working)
                                {
                                    fxb.LoadEntityDetails(parententity, RefreshSelectedNode);
                                }
                                break;
                            }
                            var linkEntityName = node.Value("name");
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
                            if (node.LocalEntityName() is string entity && !string.IsNullOrWhiteSpace(entity))
                            {
                                if (fxb.NeedToLoadEntity(entity))
                                {
                                    if (!fxb.working)
                                    {
                                        fxb.LoadEntityDetails(entity, RefreshSelectedNode);
                                    }
                                    break;
                                }
                                AttributeMetadata[] attributes = fxb.GetDisplayAttributes(entity).ToArray();
                                if (node.Name == "attribute")
                                {
                                    ctrl = new attributeControl(node, attributes, fxb, this);
                                }
                                else if (node.Name == "order")
                                {
                                    ctrl = new orderControl(node, attributes, fxb, this);
                                }
                            }
                            break;

                        case "filter":
                            ctrl = new filterControl(collec, fxb, this);
                            break;

                        case "condition":
                            ctrl = new conditionControl(node, fxb, this);
                            break;

                        case "value":
                            ctrl = new valueControl(node, fxb, this);
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
                    ctrl.Dock = DockStyle.Top;
                }
                if (existingControl != null) panelContainer.Controls.Remove(existingControl);
                fxb.ShowMetadata(ctrl?.Metadata());
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
            var entityName = entityNode.Value("name");
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
            var selected = entityNode.Nodes.Cast<TreeNode>().Where(n => n.Name == "attribute").Select(n => n.Value("name")).Where(a => !string.IsNullOrEmpty(a)).ToList();
            var selectAttributesDlg = new SelectAttributesDialog(fxb, entityName, selected);
            selectAttributesDlg.StartPosition = FormStartPosition.CenterParent;
            if (selectAttributesDlg.ShowDialog() == DialogResult.OK)
            {
                var selectedAttributes = selectAttributesDlg.GetSelectedAttributes().Select(a => a.LogicalName);
                var i = 0;
                while (i < entityNode.Nodes.Count)
                {   // Remove unselected previously added attributes
                    TreeNode subnode = entityNode.Nodes[i];
                    var attributename = subnode.Value("name");
                    if (subnode.Name == "attribute" && !selectedAttributes.Contains(attributename))
                    {
                        entityNode.Nodes.Remove(subnode);
                    }
                    else
                    {
                        i++;
                    }
                }
                foreach (var attribute in selectedAttributes.Where(a => !selected.Contains(a)))
                {   // Add new attributes
                    var attrNode = TreeNodeHelper.AddChildNode(entityNode, "attribute", fxb);
                    var coll = new Dictionary<string, string>();
                    coll.Add("name", attribute);
                    attrNode.Tag = coll;
                    TreeNodeHelper.SetNodeText(attrNode, fxb);
                }
                FetchChanged = treeChecksum != GetTreeChecksum(null);
                fxb.UpdateLiveXML();
                RecordHistory("select attributes");
            }
        }

        private void SetWarning(TreeNode node)
        {
            var warning = fxb.GetWarning(node);
            if (warning != null)
            {
                var leadingSpaces = "      ";
                lblWarning.Text = leadingSpaces + warning.Message;
                switch (warning.Level)
                {
                    case ControlValidationLevel.Error:
                        lblWarning.ImageKey = "error";
                        break;

                    case ControlValidationLevel.Warning:
                        lblWarning.ImageKey = "warning";
                        break;

                    case ControlValidationLevel.Info:
                        lblWarning.ImageKey = "info";
                        break;
                }

                if (string.IsNullOrWhiteSpace(warning.Url))
                {
                    lblWarning.LinkArea = new LinkArea(0, 0);
                }
                else
                {
                    lblWarning.LinkArea = new LinkArea(leadingSpaces.Length, lblWarning.Text.Length - leadingSpaces.Length);
                    lblWarning.Tag = warning.Url;
                }
            }
            lblWarning.Visible = warning != null;
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
                    if (comment.Contains("&apos;"))
                    {
                        comment = comment.Replace("&apos;", "'");
                    }
                    var doc = new XmlDocument();
                    try
                    {
                        doc.LoadXml(comment);
                        TreeNodeHelper.AddTreeViewNode(node.Parent, node, doc.DocumentElement, fxb, true);
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

        private void lblQAExpander_Click(object sender, EventArgs e)
        {
            (sender as Label)?.GroupBoxSetState(tt);
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

        private void TreeBuilderControl_FormClosing(object sender, FormClosingEventArgs e)
        {
            fxb.settings.QueryOptions.ShowQuickActions = gbQuickActions.IsExpanded();
        }

        private void TreeBuilderControl_Load(object sender, EventArgs e)
        {
            splitContainer1.SplitterDistance = splitContainer1.Height / 2;
        }

        private void tvFetch_AfterSelect(object sender, TreeViewEventArgs e)
        {
            HandleNodeSelection(e.Node);
            SetWarning(e.Node);
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

        private void showMetadataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fxb.ShowMetadataControl();
        }

        #endregion Control Event Handlers

        private void TreeBuilderControl_Enter(object sender, EventArgs e)
        {
            fxb.historyisavailable = true;
            fxb.EnableDisableHistoryButtons();
        }

        private void lblWarning_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            fxb.OpenUrl(sender);
        }
    }
}