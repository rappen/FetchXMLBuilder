using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace Rappen.XTB.FetchXmlBuilder.Builder
{
    public static class TreeNodeExtensions
    {
        internal static TreeNode LocalEntityNode(this TreeNode node)
        {
            if (node == null)
            {
                return null;
            }
            while (!node.Name.Equals("entity") && !node.Name.Equals("link-entity"))
            {
                node = node.Parent;
                if (node == null)
                {
                    return null;
                }
            }
            return node.Name.Equals("entity") || node.Name.Equals("link-entity") ? node : null;
        }

        internal static bool LocalEntityIsRoot(this TreeNode node)
        {
            return node.LocalEntityNode()?.Name == "entity";
        }

        internal static string LocalEntityName(this TreeNode node)
        {
            return node.LocalEntityNode().Value("name");
        }

        internal static string Value(this TreeNode node, string key)
        {
            if (node != null && node.Tag != null && node.Tag is Dictionary<string, string> tag && tag.ContainsKey(key))
            {
                return tag[key];
            }
            return string.Empty;
        }

        internal static bool IsFetchAggregate(this TreeNode node)
        {
            var aggregate = false;
            while (node != null && node.Name != "fetch")
            {
                node = node.Parent;
            }
            if (node != null && node.Name == "fetch")
            {
                aggregate = node.Value("aggregate") == "true";
            }
            return aggregate;
        }

        internal static bool IsFetchDistinct(this TreeNode node)
        {
            var distinct = false;
            while (node != null && node.Name != "fetch")
            {
                node = node.Parent;
            }
            if (node != null && node.Name == "fetch")
            {
                distinct = node.Value("distinct") == "true";
            }
            return distinct;
        }

        internal static string GetTooltip(this TreeNode node)
        {
            if (node == null)
            {
                return null;
            }
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

            return tooltip;
        }
    }
}