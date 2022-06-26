using Microsoft.Xrm.Sdk.Metadata;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace Cinteros.Xrm.FetchXmlBuilder.AppCode
{
    public static class Extensions
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

        internal static string ToTypeString(this AttributeMetadata attribute)
        {
            if (attribute == null)
            {
                return string.Empty;
            }
            if (attribute.AttributeTypeName != null)
            {
                return attribute.AttributeTypeName.Value.RemoveEnd("Type");
            }
            return attribute.AttributeType.ToString();
        }

        internal static string TriToString(this CheckState state, string uncheck_check_indterminate)
        {
            var splits = uncheck_check_indterminate.Split(';');
            if (splits.Length == 3)
            {
                return state.TriToString(splits[0], splits[1], splits[2]);
            }
            return string.Empty;
        }

        internal static string TriToString(this CheckState state, string uncheck, string check, string indeterminate)
        {
            switch (state)
            {
                case CheckState.Unchecked:
                    return uncheck;

                case CheckState.Checked:
                    return check;

                case CheckState.Indeterminate:
                    return indeterminate;
            }
            return string.Empty;
        }

        internal static string RemoveEnd(this string text, string remove)
        {
            if (text == null || string.IsNullOrEmpty(remove) || !text.EndsWith(remove))
            {
                return text;
            }
            return text.Substring(0, text.Length - remove.Length);
        }

        internal static bool KeyDown(this KeyEventArgs keyevent, Keys key, bool shift, bool control, bool alt)
        {
            return keyevent.KeyCode == key && keyevent.Shift == shift && keyevent.Control == control && keyevent.Alt == alt;
        }
    }
}