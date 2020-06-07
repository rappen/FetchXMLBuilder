using Cinteros.Xrm.FetchXmlBuilder.DockControls;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace Cinteros.Xrm.FetchXmlBuilder.AppCode
{
    internal class TreeNodeHelper
    {
        /// <summary>
        /// Adds a new TreeNode to the parent object from the XmlNode information
        /// </summary>
        /// <param name="parentObject">Object (TreeNode or TreeView) where to add a new TreeNode</param>
        /// <param name="xmlNode">Xml node from the sitemap</param>
        /// <param name="tree">Current application form</param>
        /// <param name="isDisabled"> </param>
        public static TreeNode AddTreeViewNode(object parentObject, XmlNode xmlNode, TreeBuilderControl tree, FetchXmlBuilder fxb, int index = -1)
        {
            TreeNode node = null;
            if (xmlNode is XmlElement || xmlNode is XmlComment)
            {
                node = new TreeNode(xmlNode.Name);
                node.Name = xmlNode.Name;
                Dictionary<string, string> attributes = new Dictionary<string, string>();

                if (xmlNode.NodeType == XmlNodeType.Comment)
                {
                    attributes.Add("#comment", xmlNode.Value);
                    node.ForeColor = System.Drawing.Color.Gray;
                }
                else if (xmlNode.Attributes != null)
                {
                    foreach (XmlAttribute attr in xmlNode.Attributes)
                    {
                        attributes.Add(attr.Name, attr.Value);
                    }
                }
                if (parentObject is TreeView)
                {
                    ((TreeView)parentObject).Nodes.Add(node);
                }
                else if (parentObject is TreeNode)
                {
                    if (index == -1)
                    {
                        ((TreeNode)parentObject).Nodes.Add(node);
                    }
                    else
                    {
                        ((TreeNode)parentObject).Nodes.Insert(index, node);
                    }
                }
                else
                {
                    throw new Exception("AddTreeViewNode: Unsupported control type");
                }
                node.Tag = attributes;
                AddContextMenu(node, tree);
                foreach (XmlNode childNode in xmlNode.ChildNodes)
                {
                    AddTreeViewNode(node, childNode, tree, fxb);
                }
                SetNodeText(node, fxb);
            }
            else if (xmlNode is XmlText && parentObject is TreeNode)
            {
                var treeNode = (TreeNode)parentObject;
                if (treeNode.Tag is Dictionary<string, string>)
                {
                    var attributes = (Dictionary<string, string>)treeNode.Tag;
                    attributes.Add("#text", ((XmlText)xmlNode).Value);
                }
            }
            return node;
        }

        public static void SetNodeText(TreeNode node, FetchXmlBuilder fxb)
        {
            if (node == null)
            {
                return;
            }
            var text = node.Name;
            Dictionary<string, string> attributes =
                node.Tag is Dictionary<string, string> ?
                    (Dictionary<string, string>)node.Tag :
                    new Dictionary<string, string>();
            var agg = GetAttributeFromNode(node, "aggregate");
            var name = GetAttributeFromNode(node, "name");
            var alias = GetAttributeFromNode(node, "alias");
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
                    break;
                case "entity":
                case "link-entity":
                    text += " " + fxb.GetEntityDisplayName(name);
                    if (!string.IsNullOrEmpty(alias))
                    {
                        text += " (" + alias + ")";
                    }
                    if (GetAttributeFromNode(node, "intersect") == "true")
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
                            var parent = GetAttributeFromNode(node.Parent, "name");
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
                        var grp = GetAttributeFromNode(node, "groupby");
                        if (grp == "true")
                        {
                            text += " GRP";
                        }
                    }
                    break;
                case "filter":
                    var type = GetAttributeFromNode(node, "type");
                    if (!string.IsNullOrEmpty(type))
                    {
                        text += " (" + type + ")";
                    }
                    break;
                case "condition":
                    {
                        var ent = GetAttributeFromNode(node, "entityname");
                        var attr = GetAttributeFromNode(node, "attribute");
                        var oper = GetAttributeFromNode(node, "operator");
                        var val = GetAttributeFromNode(node, "value");
                        var uiname = GetAttributeFromNode(node, "uiname");
                        if (node.Parent != null && node.Parent.Parent != null)
                        {
                            var parent = GetAttributeFromNode(node.Parent.Parent, "name");
                            attr = fxb.GetAttributeDisplayName(parent, attr);
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
                        text += (" " + attr + " " + oper + " " + val).TrimEnd();
                    }
                    break;
                case "value":
                    var value = GetAttributeFromNode(node, "#text");
                    text += " " + value;
                    break;
                case "order":
                    {
                        var attr = GetAttributeFromNode(node, "attribute");
                        var desc = GetAttributeFromNode(node, "descending");
                        if (!string.IsNullOrEmpty(alias))
                        {
                            text += " " + alias;
                        }
                        else if (!string.IsNullOrEmpty(attr))
                        {
                            if (!string.IsNullOrEmpty(attr) && node.Parent != null)
                            {
                                var parent = GetAttributeFromNode(node.Parent, "name");
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
                    text = GetAttributeFromNode(node, "#comment")
                        .Trim()
                        .Replace("\r\n", "  ")
                        .Replace("&apos;", "'");
                    if (string.IsNullOrWhiteSpace(text))
                    {
                        text = " - comment - ";
                    }
                    break;
            }
            if (FetchXmlBuilder.friendlyNames && !string.IsNullOrEmpty(text))
            {
                text = text.Substring(0, 1).ToUpper() + text.Substring(1);
            }
            node.Text = text;
            SetNodeTooltip(node);
        }

        internal static void SetNodeTooltip(TreeNode node)
        {
            if (node != null)
            {
                var doc = new XmlDocument();
                XmlNode rootNode = doc.CreateElement("root");
                doc.AppendChild(rootNode);
                TreeNodeHelper.AddXmlNode(node, rootNode);
                var tooltip = "";
                try
                {
                    XDocument xdoc = XDocument.Parse(rootNode.InnerXml);
                    tooltip = xdoc.ToString();
                }
                catch
                {
                    tooltip = rootNode.InnerXml;
                }
                node.ToolTipText = tooltip;
                if (node.Parent != null)
                {
                    SetNodeTooltip(node.Parent);
                }
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
                var nodecapabilities = new FetchNodeCapabilities(node);

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

        internal static TreeNode AddChildNode(TreeNode parentNode, string name)
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
                var parentCap = new FetchNodeCapabilities(parentNode);
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
            return childNode;
        }

        internal static string GetAttributeFromNode(TreeNode treeNode, string attribute)
        {
            var result = "";
            if (treeNode != null && treeNode.Tag != null && treeNode.Tag is Dictionary<string, string>)
            {
                var collection = (Dictionary<string, string>)treeNode.Tag;
                if (collection.ContainsKey(attribute))
                {
                    result = collection[attribute];
                }
            }
            return result;
        }
    }
}