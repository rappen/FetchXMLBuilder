using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;

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

        internal static bool IsDecimalable(this AttributeTypeCode? attributetype)
        {
            if (attributetype == null)
            {
                return false;
            }
            return
                attributetype == AttributeTypeCode.Decimal ||
                attributetype == AttributeTypeCode.Double ||
                attributetype == AttributeTypeCode.Money;
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

                    list.Insert(Math.Min(newIndex, list.Count), item);
                }
            }
        }
    }
}