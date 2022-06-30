using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace Rappen.XTB.FetchXmlBuilder.Builder
{
    internal static class TreeNodeExtensions
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
            return (node.Name.Equals("entity") || node.Name.Equals("link-entity")) ? node : null;
        }

        internal static bool LocalEntityIsRoot(this TreeNode node)
        {
            return LocalEntityNode(node)?.Name == "entity";
        }

        internal static string LocalEntityName(this TreeNode node)
        {
            return LocalEntityNode(node).Value("name");
        }

        internal static string Value(this TreeNode node, string key)
        {
            if (node != null && node.Tag != null && node.Tag is Dictionary<string, string> tag && tag.ContainsKey(key))
            {
                return tag[key];
            }
            return string.Empty;
        }

        internal static void SetValue(this TreeNode node, string key, object value)
        {
            if (node == null)
            {
                return;
            }
            if (node.Tag == null)
            {
                node.Tag = new Dictionary<string, string>();
            }
            if (node.Tag is Dictionary<string, string> tag)
            {
                tag[key] = value.ToString();
            }
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

        internal static bool IsAttributeValidForView(this TreeNode node)
        {
            var entity = node.Parent;
            return node.Name == "attribute" && (entity?.Name == "entity" || (entity?.Name == "link-entity" && !string.IsNullOrWhiteSpace(entity.Value("alias"))));
        }

        internal static string GetAttributeLayoutName(this TreeNode node)
        {
            var entity = node.LocalEntityNode();
            var entityalias = entity.Name == "link-entity" ? entity.Value("alias") : string.Empty;
            var attribute = node.Value("name");
            var alias = node.Value("alias");
            if (!string.IsNullOrWhiteSpace(alias))
            {
                attribute = alias;
            }
            else if (!string.IsNullOrWhiteSpace(entityalias))
            {
                attribute = entityalias + "." + attribute;
            }
            if (!string.IsNullOrWhiteSpace(attribute))
            {
                return attribute;
            }
            return null;
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