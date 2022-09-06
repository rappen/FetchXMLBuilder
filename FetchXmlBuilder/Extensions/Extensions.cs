using Rappen.XTB.FetchXmlBuilder.Builder;
using Microsoft.Xrm.Sdk.Metadata;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace Rappen.XTB.FetchXmlBuilder.Extensions
{
    public static class Extensions
    {
        internal static string ToTypeString(this AttributeMetadata attribute)
        {
            if (attribute == null)
            {
                return string.Empty;
            }
            if (attribute.AttributeTypeName?.Value != null)
            {
                return attribute.AttributeTypeName.Value.RemoveEnd("Type");
            }
            if (attribute.AttributeType != null)
            {
                return attribute.AttributeType.ToString();
            }
            return string.Empty;
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

        internal static string AttributeValue(this XmlNode node, string key)
        {
            if (node != null && node.Attributes != null && node.Attributes[key] is XmlAttribute attr)
            {
                return attr.Value;
            }
            return string.Empty;
        }

        public static void Move<T>(this List<T> list, T item, int newIndex)
        {   // From this tip: https://stackoverflow.com/a/450250/2866704
            if (item != null)
            {
                var oldIndex = list.IndexOf(item);
                if (oldIndex > -1 && oldIndex != newIndex)
                {
                    list.RemoveAt(oldIndex);

                    if (newIndex > oldIndex) newIndex--;
                    // the actual index could have shifted due to the removal

                    list.Insert(newIndex, item);
                }
            }
        }
    }
}