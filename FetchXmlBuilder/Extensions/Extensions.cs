using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Rappen.XTB.FetchXmlBuilder.Extensions
{
    public static class Extensions
    {
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