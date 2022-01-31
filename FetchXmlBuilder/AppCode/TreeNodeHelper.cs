using Cinteros.Xrm.FetchXmlBuilder.Controls;
using Cinteros.Xrm.FetchXmlBuilder.DockControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace Cinteros.Xrm.FetchXmlBuilder.AppCode
{
    internal class TreeNodeHelper
    {
        /// <summary>
        /// Reuses an existing node in a tree to represent a new FetchXML item
        /// </summary>
        /// <param name="parentnode">TreeNode where to add a new TreeNode</param>
        /// <param name="node">The node in the tree to reuse. Set to null to create a new node</param>
        /// <param name="xmlNode">Xml node from the sitemap</param>
        /// <param name="fxb"></param>
        /// <param name="validate">Indicates whether to re-validate the tree</param>
        /// <returns>The node that was added or updated</returns>
        public static TreeNode AddTreeViewNode(TreeNode parentnode, TreeNode node, XmlNode xmlNode, FetchXmlBuilder fxb, bool validate)
        {
            if (parentnode == null && node == null && xmlNode == null)
            {   // Don't add anything!
                return null;
            }

            if (xmlNode is XmlElement || xmlNode is XmlComment)
            {
                if (node == null)
                {
                    node = parentnode != null ? parentnode.Nodes.Add("") : fxb.dockControlBuilder.tvFetch.Nodes.Add("");
                }

                // Store the current state of this node
                var originalAttributes = node.Tag as Dictionary<string, string>;
                var originalText = node.Text;
                var originalName = node.Name;

                // Copy the name and attributes from the XML element to this tree view node
                node.Name = xmlNode.Name;
                var attributes = new Dictionary<string, string>();

                if (xmlNode.NodeType == XmlNodeType.Comment)
                {
                    attributes.Add("#comment", xmlNode.Value);
                    node.ForeColor = System.Drawing.Color.Gray;
                }
                else
                {
                    node.ForeColor = node.TreeView.ForeColor;
                    foreach (XmlAttribute attr in xmlNode.Attributes)
                    {
                        attributes.Add(attr.Name, attr.Value);
                    }
                }
                node.Tag = attributes;

                // Copy any children as well. Reuse the existing children of this tree view node where possible.
                var i = 0;
                foreach (XmlNode childNode in xmlNode.ChildNodes)
                {
                    AddTreeViewNode(node, i < node.Nodes.Count ? node.Nodes[i] : null, childNode, fxb, false);
                    i++;
                }

                // If we've got more child tree view nodes left that we don't need any longer, remove them
                while (i < node.Nodes.Count)
                {
                    node.Nodes.RemoveAt(i);
                }

                // Set the text of the node based on the attribute values. We don't need to validate here, we'll
                // do that as a single batch at the end if necessary
                SetNodeText(node, fxb, validate: false);

                // If the node is the currently selected node, check if any of the values have changed.
                // If so, deselect it and reselect it again to refresh the rest of the UI.
                if (node.IsSelected)
                {
                    if (node.Text != originalText ||
                        node.Name != originalName ||
                        originalAttributes == null ||
                        String.Join(";", attributes.OrderBy(kvp => kvp.Key).Select(kvp => kvp.Key + "=" + kvp.Value)) != String.Join(";", originalAttributes.OrderBy(kvp => kvp.Key).Select(kvp => kvp.Key + "=" + kvp.Value)))
                    {
                        node.TreeView.SelectedNode = null;
                        node.TreeView.SelectedNode = node;
                    }
                }
            }
            else if (xmlNode is XmlText text && parentnode?.Tag is Dictionary<string, string> attributes)
            {
                node = parentnode;
                attributes.Add("#text", text.Value);
            }

            if (validate)
            {
                Validate(node, fxb);
            }
            return node;
        }

        public static void SetNodeText(TreeNode node, FetchXmlBuilder fxb, bool validate = true)
        {
            if (node == null)
            {
                return;
            }
            var text = fxb.settings.ShowNodeType ? node.Name : "";
            var attributes = node.Tag is Dictionary<string, string> tag ? tag : new Dictionary<string, string>();
            var agg = node.Value("aggregate");
            var name = node.Value("name");
            var alias = node.Value("alias");
            switch (node.Name)
            {
                case "fetch":
                    if (attributes.ContainsKey("top"))
                    {
                        text += " top:" + attributes["top"];
                    }
                    if (attributes.ContainsKey("count"))
                    {
                        text += " cnt:" + attributes["count"];
                    }
                    if (attributes.ContainsKey("returntotalrecordcount") && attributes["returntotalrecordcount"] == "true")
                    {
                        text += " reccnt";
                    }
                    if (attributes.ContainsKey("aggregate") && attributes["aggregate"] == "true")
                    {
                        text += " aggr";
                    }
                    if (attributes.ContainsKey("distinct") && attributes["distinct"] == "true")
                    {
                        text += " dist";
                    }
                    if (attributes.ContainsKey("page"))
                    {
                        text += " pg:" + attributes["page"];
                    }
                    if (string.IsNullOrWhiteSpace(text))
                    {
                        text = "fetch";
                    }
                    break;
                case "entity":
                case "link-entity":
                    text += " " + fxb.GetEntityDisplayName(name);
                    if (!string.IsNullOrEmpty(alias))
                    {
                        text += " (" + alias + ")";
                    }
                    if (node.Value("intersect") == "true")
                    {
                        text += " M:M";
                    }
                    break;
                case "attribute":
                    if (!string.IsNullOrEmpty(name))
                    {
                        text += " ";
                        if (node.Parent != null)
                        {
                            var parent = node.Parent.Value("name");
                            name = fxb.GetAttributeDisplayName(parent, name);
                        }
                        if (!string.IsNullOrEmpty(agg) && !string.IsNullOrEmpty(name))
                        {
                            if (!string.IsNullOrEmpty(alias))
                            {
                                text += alias + "=";
                            }
                            text += agg + "(" + name + ")";
                        }
                        else if (!string.IsNullOrEmpty(alias))
                        {
                            text += alias + " (" + name + ")";
                        }
                        else
                        {
                            text += name;
                        }
                        var grp = node.Value("groupby");
                        if (grp == "true")
                        {
                            text += " GRP";
                        }
                    }
                    break;
                case "filter":
                    var type = node.Value("type");
                    if (string.IsNullOrEmpty(type))
                    {
                        type = "(and)";
                    }
                    text += " " + type;
                    break;
                case "condition":
                    {
                        var ent = node.Value("entityname");
                        var attr = node.Value("attribute");
                        var oper = node.Value("operator");
                        var val = node.Value("value");
                        var valueOf = node.Value("valueof");
                        var uiname = node.Value("uiname");
                        if (node.Parent != null && node.Parent.Parent != null)
                        {
                            var parent = node.Parent.Parent.Value("name");
                            attr = fxb.GetAttributeDisplayName(parent, attr);
                            valueOf = fxb.GetAttributeDisplayName(parent, valueOf);
                        }
                        if (!string.IsNullOrEmpty(ent))
                        {
                            attr = ent + "." + attr;
                        }
                        if (oper.Contains("-x-"))
                        {
                            oper = oper.Replace("-x-", " " + val + " ");
                            val = "";
                        }
                        if (!string.IsNullOrWhiteSpace(uiname) && fxb.settings.UseFriendlyNames)
                        {
                            val = uiname;
                        }
                        if (!string.IsNullOrWhiteSpace(valueOf))
                        {
                            val = valueOf;
                        }
                        text += (" " + attr + " " + oper + " " + val).TrimEnd();
                    }
                    break;
                case "value":
                    var value = node.Value("#text");
                    text += " " + value;
                    break;
                case "order":
                    {
                        var attr = node.Value("attribute");
                        var desc = node.Value("descending");
                        if (!string.IsNullOrEmpty(alias))
                        {
                            text += " " + alias;
                        }
                        else if (!string.IsNullOrEmpty(attr))
                        {
                            if (!string.IsNullOrEmpty(attr) && node.Parent != null)
                            {
                                var parent = node.Parent.Value("name");
                                attr = fxb.GetAttributeDisplayName(parent, attr);
                            }
                            {
                                text += " " + attr;
                            }
                        }
                        if (desc == "true")
                        {
                            text += " Desc";
                        }
                    }
                    break;
                case "#comment":
                    text = node.Value("#comment")
                        .Trim()
                        .Replace("\r\n", "  ")
                        .Replace("&apos;", "'");
                    if (string.IsNullOrWhiteSpace(text))
                    {
                        text = " - comment - ";
                    }
                    break;
            }
            text = text.Trim();
            if (FetchXmlBuilder.friendlyNames && !string.IsNullOrEmpty(text))
            {
                text = text.Substring(0, 1).ToUpper() + text.Substring(1);
            }
            if (string.IsNullOrWhiteSpace(text))
            {
                text = $"({node.Name})";
            }
            node.Text = text;
            if (validate)
            {
                Validate(node, fxb);
            }
        }

        public static void Validate(TreeNode node, FetchXmlBuilder fxb)
        {
            var root = node;
            while (root.Parent != null)
            {
                root = root.Parent;
            }
            SetWarnings(root, fxb);
        }

        private static void SetWarnings(TreeNode node, FetchXmlBuilder fxb)
        {
            if (node == null)
            {
                return;
            }
            var warning = Validations.GetWarning(node, fxb);
            if (warning != null)
            {
                node.ToolTipText = warning.Message;
                switch (warning.Level)
                {
                    case ControlValidationLevel.Error:
                        node.ImageKey = "error";
                        break;
                    case ControlValidationLevel.Warning:
                        node.ImageKey = "warning";
                        break;
                    case ControlValidationLevel.Info:
                        node.ImageKey = "info";
                        break;
                }
            }
            else
            {
                node.ImageKey = node.Name;
                SetNodeTooltip(node);
            }
            node.SelectedImageKey = node.ImageKey;

            foreach (var child in node.Nodes.OfType<TreeNode>())
            {
                SetWarnings(child, fxb);
            }
        }

        internal static void SetNodeTooltip(TreeNode node)
        {
            node.ToolTipText = node.GetTooltip();
            if (node.Parent != null)
            {
                SetNodeTooltip(node.Parent);
            }
        }

        /// <summary>Adds a context menu to a TreeNode control</summary>
        /// <param name="node">TreeNode where to add the context menu</param>
        /// <param name="tree">Current application form</param>
        public static void AddContextMenu(TreeNode node, TreeBuilderControl tree)
        {
            tree.addMenu.Items.Clear();
            var tmplbl = tree.lblQAExpander;
            tree.gbQuickActions.Controls.Clear();
            tree.gbQuickActions.Controls.Add(tmplbl);
            if (node == null && tree.tvFetch.Nodes.Count > 0)
            {
                node = tree.tvFetch.Nodes[0];
            }
            if (node != null)
            {
                var nodecapabilities = new FetchNodeCapabilities(node.Name, true);

                if (nodecapabilities.Multiple)
                {
                    tree.addOneMoreToolStripMenuItem.Text = "More " + nodecapabilities.Name;
                    tree.addOneMoreToolStripMenuItem.Tag = "MORE-" + nodecapabilities.Name;
                    AddLinkFromCapability(tree, "+" + nodecapabilities.Name, "MORE-" + nodecapabilities.Name);
                }
                if (nodecapabilities.Attributes && tree.selectAttributesToolStripMenuItem.Enabled)
                {
                    AddLinkFromCapability(tree, "Select Attributes", "SelectAttributes");
                }
                foreach (var childcapability in nodecapabilities.ChildTypes)
                {
                    if (childcapability.Name == "-")
                    {
                        tree.addMenu.Items.Add(new ToolStripSeparator());
                    }
                    else if (childcapability.Multiple || !node.Nodes.ContainsKey(childcapability.Name))
                    {
                        AddMenuFromCapability(tree.addMenu, childcapability.Name);
                        AddLinkFromCapability(tree, childcapability.Name, null, childcapability.Name == "#comment");
                    }
                }
                if (tree.addMenu.Items.Count == 0)
                {
                    var dummy = tree.addMenu.Items.Add("nothing to add");
                    dummy.Enabled = false;
                }

                tree.addOneMoreToolStripMenuItem.Visible = nodecapabilities.Multiple;
                tree.selectAttributesToolStripMenuItem.Visible = nodecapabilities.Attributes;
                tree.deleteToolStripMenuItem.Enabled = nodecapabilities.Delete;
                tree.commentToolStripMenuItem.Enabled = nodecapabilities.Comment;
                tree.uncommentToolStripMenuItem.Enabled = nodecapabilities.Uncomment;

                node.ContextMenuStrip = tree.nodeMenu;
            }
        }

        private static void AddLinkFromCapability(TreeBuilderControl tree, string name, string tag = null, bool alignright = false)
        {
            var link = new LinkLabel();
            link.AutoSize = true;
            link.Dock = alignright ? DockStyle.Right : DockStyle.Left;
            link.TabIndex = tree.gbQuickActions.Controls.Count;
            link.TabStop = true;
            link.Text = name;
            link.Tag = tag ?? name;
            link.LinkBehavior = LinkBehavior.HoverUnderline;
            link.LinkClicked += tree.QuickActionLink_LinkClicked;
            tree.gbQuickActions.Controls.Add(link);
            if (!alignright)
            {
                link.BringToFront();
            }
        }

        private static void AddMenuFromCapability(ToolStrip owner, string name, bool alignright = false, string prefix = "")
        {
            var additem = owner.Items.Add(prefix + name);
            additem.Tag = name;
            if (alignright)
            {
                additem.Alignment = ToolStripItemAlignment.Right;
            }
        }

        /// <summary>
        /// Hides all items from a context menu
        /// </summary>
        /// <param name="cm">Context menu to clean</param>
        public static void HideAllContextMenuItems(ContextMenuStrip cm)
        {
            foreach (ToolStripItem o in cm.Items)
            {
                if (o.Text == "Cut" || o.Text == "Copy" || o.Text == "Paste")
                {
                    o.Enabled = false;
                }
                else if (o.Name == "toolStripSeparatorBeginOfEdition" || o is ToolStripSeparator)
                {
                    o.Visible = true;
                }
                else
                {
                    o.Visible = false;
                }
            }
        }

        /// <summary>Creates xml from given treenode and adds it as child to given xml node</summary>
        /// <param name="currentNode">Tree node from which to build xml</param>
        /// <param name="parentXmlNode">Parent xml node</param>
        internal static void AddXmlNode(TreeNode currentNode, XmlNode parentXmlNode)
        {
            var collec = (Dictionary<string, string>)currentNode.Tag;
            XmlNode newNode;
            if (currentNode.Name == "#comment")
            {
                newNode = parentXmlNode.OwnerDocument.CreateComment(collec.ContainsKey("#comment") ? collec["#comment"] : "");
            }
            else
            {
                newNode = parentXmlNode.OwnerDocument.CreateElement(currentNode.Name);
                foreach (string key in collec.Keys)
                {
                    if (key == "#text")
                    {
                        XmlText newText = parentXmlNode.OwnerDocument.CreateTextNode(collec[key]);
                        newNode.AppendChild(newText);
                    }
                    else
                    {
                        XmlAttribute attr = parentXmlNode.OwnerDocument.CreateAttribute(key);
                        attr.Value = collec[key];
                        newNode.Attributes.Append(attr);
                    }
                }

                var others = new List<TreeNode>();

                foreach (TreeNode childNode in currentNode.Nodes)
                {
                    others.Add(childNode);
                }

                foreach (TreeNode otherNode in others)
                {
                    AddXmlNode(otherNode, newNode);
                }
            }

            parentXmlNode.AppendChild(newNode);
        }

        internal static TreeNode AddChildNode(TreeNode parentNode, string name, FetchXmlBuilder fxb, TreeNode sisterNode = null)
        {
            var childNode = new TreeNode(name);
            childNode.Tag = new Dictionary<string, string>();
            childNode.Name = childNode.Text.Replace(" ", "");
            if (name == "#comment")
            {
                childNode.ForeColor = System.Drawing.Color.Gray;
            }
            if (parentNode != null)
            {
                if (sisterNode != null)
                {
                    parentNode.Nodes.Insert(sisterNode.Index + 1, childNode);
                }
                else
                {
                    var parentCap = new FetchNodeCapabilities(parentNode.Name, true);
                    var nodeIndex = parentCap.IndexOfChild(name);
                    var pos = 0;
                    while (pos < parentNode.Nodes.Count && nodeIndex >= parentCap.IndexOfChild(parentNode.Nodes[pos].Name))
                    {
                        pos++;
                    }
                    if (pos == parentNode.Nodes.Count)
                    {
                        parentNode.Nodes.Add(childNode);
                    }
                    else
                    {
                        parentNode.Nodes.Insert(pos, childNode);
                    }
                }
            }
            Validate(childNode, fxb);
            return childNode;
        }

        internal static bool IsFetchAggregate(string fetch)
        {
            var xml = new XmlDocument();
            xml.LoadXml(fetch);
            return IsFetchAggregate(xml);
        }

        private static bool IsFetchAggregate(XmlDocument xml)
        {
            var fetchnode = xml.SelectSingleNode("fetch");
            return fetchnode.Attributes["aggregate"]?.Value == "true";
        }

        internal static List<string> GetEntitysForFetch(XmlDocument fetchDoc)
        {
            var result = new List<string>();
            if (!(fetchDoc.SelectSingleNode("fetch/entity") is XmlNode ent))
            {
                return null;
            }
            if (ent.Attributes.GetNamedItem("name") != null)
            {
                result.Add(ent.Attributes.GetNamedItem("name").Value);
                result.AddRange(GetEntitysChilds(ent));
            }
            result = result.Distinct().ToList();
            return result;
        }

        private static List<string> GetEntitysChilds(XmlNode ent)
        {
            var result = new List<string>();
            if (!(ent.SelectNodes("link-entity") is XmlNodeList childent))
            {
                return null;
            }
            foreach (XmlNode child in childent)
            {
                if (child.Attributes.GetNamedItem("name") != null)
                {
                    result.Add(child.Attributes.GetNamedItem("name").Value);
                    result.AddRange(GetEntitysChilds(child));
                }
            }
            return result;
        }
    }
}