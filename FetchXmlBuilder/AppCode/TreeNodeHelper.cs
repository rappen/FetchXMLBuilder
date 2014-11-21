using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace Cinteros.Xrm.FetchXmlBuilder.AppCode
{
    class TreeNodeHelper
    {
        /// <summary>
        /// Adds a new TreeNode to the parent object from the XmlNode information
        /// </summary>
        /// <param name="parentObject">Object (TreeNode or TreeView) where to add a new TreeNode</param>
        /// <param name="xmlNode">Xml node from the sitemap</param>
        /// <param name="form">Current application form</param>
        /// <param name="isDisabled"> </param>
        public static void AddTreeViewNode(object parentObject, XmlNode xmlNode, FetchXmlBuilder form, bool isDisabled = false)
        {
            if (xmlNode is XmlElement)
            {
                TreeNode node = new TreeNode(xmlNode.Name);
                node.Name = xmlNode.Name;
                Dictionary<string, string> attributes = new Dictionary<string, string>();

                if (xmlNode.Attributes != null)
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
                    ((TreeNode)parentObject).Nodes.Add(node);
                }
                else
                {
                    throw new Exception("AddTreeViewNode: Unsupported control type");
                }
                node.Tag = attributes;
                AddContextMenu(node, form);
                foreach (XmlNode childNode in xmlNode.ChildNodes)
                {
                    if (childNode.NodeType != XmlNodeType.Comment)
                    {
                        AddTreeViewNode(node, childNode, form);
                    }
                }
                SetNodeText(node);
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
        }

        public static void SetNodeText(TreeNode node)
        {
            var text = node.Name;
            Dictionary<string, string> attributes =
                node.Tag is Dictionary<string, string> ?
                    (Dictionary<string, string>)node.Tag :
                    new Dictionary<string, string>();
            var agg = GetAttributeFromNode(node, "aggregate");
            var name = GetAttributeFromNode(node, "name");
            switch (node.Name)
            {
                case "fetch":
                    if (attributes.ContainsKey("count"))
                    {
                        text += " count: " + attributes["count"];
                    }
                    break;
                case "entity":
                case "link-entity":
                    text += " " + FetchXmlBuilder.GetEntityDisplayName(name);
                    var alias = GetAttributeFromNode(node, "alias");
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
                        if (node.Parent != null)
                        {
                            var parent = GetAttributeFromNode(node.Parent, "name");
                            name = FetchXmlBuilder.GetAttributeDisplayName(parent, name);
                        }
                        if (!string.IsNullOrEmpty(agg) && !string.IsNullOrEmpty(name))
                        {
                            text += " " + agg + "(" + name + ")";
                        }
                        else
                        {
                            text += " " + name;
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
                    var ent = GetAttributeFromNode(node, "entityname");
                    var attr = GetAttributeFromNode(node, "attribute");
                    var oper = GetAttributeFromNode(node, "operator");
                    var val = GetAttributeFromNode(node, "value");
                    if (node.Parent != null && node.Parent.Parent != null)
                    {
                        var parent = GetAttributeFromNode(node.Parent.Parent, "name");
                        attr = FetchXmlBuilder.GetAttributeDisplayName(parent, attr);
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
                    text += (" " + attr + " " + oper + " " + val).TrimEnd();
                    break;
                case "value":
                    var value = GetAttributeFromNode(node, "#text");
                    text += " " + value;
                    break;
            }
            if (FetchXmlBuilder.useFriendlyNames && !string.IsNullOrEmpty(text))
            {
                text = text.Substring(0, 1).ToUpper() + text.Substring(1);
            }
            node.Text = text;
        }

        /// <summary>Adds a context menu to a TreeNode control</summary>
        /// <param name="node">TreeNode where to add the context menu</param>
        /// <param name="form">Current application form</param>
        public static void AddContextMenu(TreeNode node, FetchXmlBuilder form)
        {
            var collec = (Dictionary<string, string>)node.Tag;

            //HideAllContextMenuItems(form.nodeMenu);
            //form.deleteToolStripMenuItem.Visible = true;

            form.addToolStripMenuItem.DropDown.Items.Clear();

            var nodecapabilities = new FetchNodeCapabilities(node);
            foreach (var childcapability in nodecapabilities.ChildTypes)
            {
                if (childcapability.Multiple || !node.Nodes.ContainsKey(childcapability.Name))
                {
                    var additem = form.addToolStripMenuItem.DropDown.Items.Add(childcapability.Name);
                    additem.Tag = childcapability.Name;
                }
            }
            //form.addToolStripMenuItem.Enabled = form.addToolStripMenuItem.DropDown.Items.Count > 0;
            if (form.addToolStripMenuItem.DropDown.Items.Count == 0)
            {
                var dummy = form.addToolStripMenuItem.DropDown.Items.Add("nothing to add");
                dummy.Enabled = false;
            }
            var cutcopy = true;

            form.deleteToolStripMenuItem.Enabled = nodecapabilities.Delete;
            form.cutToolStripMenuItem.Enabled = cutcopy;
            form.copyToolStripMenuItem.Enabled = cutcopy;
            form.pasteToolStripMenuItem.Enabled = form.clipboard.IsValidForPaste(node);

            node.ContextMenuStrip = form.nodeMenu;
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
            XmlNode newNode = parentXmlNode.OwnerDocument.CreateElement(currentNode.Name);

            var collec = (Dictionary<string, string>)currentNode.Tag;

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

            parentXmlNode.AppendChild(newNode);
        }

        internal static TreeNode AddChildNode(TreeNode parentNode, string name)
        {
            var childNode = new TreeNode(name);
            childNode.Tag = new Dictionary<string, string>();
            childNode.Name = childNode.Text.Replace(" ", "");
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
