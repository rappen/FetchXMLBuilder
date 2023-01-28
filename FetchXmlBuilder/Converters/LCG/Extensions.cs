using Microsoft.Xrm.Sdk.Metadata;
using System.Linq;

namespace Rappen.XTB.LCG
{
    public static class Extensions
    {
        public static string CamelCaseIt(this string name, Settings settings)
        {
            if (!settings.ConstantCamelCased || settings.ConstantName == NameType.DisplayName)
            {
                return name;
            }
            bool WordBeginOrEnd(string text, int i)
            {
                var last = text.Substring(0, i).ToLowerInvariant();
                var next = text.Substring(i).ToLowerInvariant();
                foreach (var word in settings.commonsettings.CamelCaseWords.Where(word => last.EndsWith(word) || next.StartsWith(word)))
                {   // Found a "word" in the string (for example "count"
                    var isunbreakable = false;
                    foreach (var unbreak in settings.commonsettings.CamelCaseWords)
                    {   // Check that this word is not also part of a bigger word (for example "account"
                        var len = unbreak.Length;
                        var pos = text.ToLowerInvariant().IndexOf(unbreak);
                        if (pos >= 0 && pos < i & pos + len > i)
                        {   // Found word appears to split a bigger valid word, prevent that
                            isunbreakable = true;
                            break;
                        }
                    }
                    if (!isunbreakable)
                    {
                        return true;
                    }
                }
                return settings.commonsettings.CamelCaseWordEnds.Any(word => next.Equals(word));
            }

            var result = string.Empty;
            var nextCapital = true;
            for (var i = 0; i < name.Length; i++)
            {
                var chr = name[i];
                if ((chr < 'a') &&
                    (chr < 'A' || chr > 'Z') &&
                    (chr < '0' || chr > '9'))
                {   // Any non-letters/numbers are treated as word separators
                    nextCapital = true;
                }
                else if (chr > 'z')
                {   // Just ignore special character
                }
                else
                {
                    nextCapital = nextCapital || WordBeginOrEnd(name, i);
                    if (nextCapital)
                    {
                        result += chr.ToString().ToUpperInvariant();
                    }
                    else
                    {
                        result += chr;
                    }
                    nextCapital = false;
                }
            }
            return result;
        }

        public static string StripPrefix(this string name, Settings settings)
        {
            if (!settings.DoStripPrefix || string.IsNullOrEmpty(settings.StripPrefix) || settings.ConstantName == NameType.DisplayName)
            {
                return name;
            }
            foreach (var prefix in settings.StripPrefix.Split(',').Select(p => p.Trim()).Where(p => !string.IsNullOrEmpty(p)))
            {
                if (name.ToLowerInvariant().StartsWith(prefix))
                {
                    name = name.Substring(prefix.Length);
                }
            }
            return name;
        }

        internal static string StringToCSharpIdentifier(string name)
        {
            name = System.Text.Encoding.UTF8.GetString(System.Text.Encoding.GetEncoding("ISO-8859-8").GetBytes(name))
                .Replace(" ", "")
                .Replace("(", "")
                .Replace(")", "")
                .Replace("<", "")
                .Replace(">", "")
                .Replace(".", "")
                .Replace(",", "")
                .Replace(";", "")
                .Replace(":", "")
                .Replace("'", "")
                .Replace("*", "")
                .Replace("&", "")
                .Replace("%", "")
                .Replace("-", "_")
                .Replace("+", "_")
                .Replace("/", "_")
                .Replace("\\", "_")
                .Replace("[", "_")
                .Replace("]", "_");
            return UnicodeCharacterUtilities.MakeValidIdentifier(name, false);
        }

        internal static string GetEntityClass(this EntityMetadata entity, Settings lcgsettings)
        {
            var result = entity.LogicalName;
            switch (lcgsettings.ConstantName)
            {
                case NameType.DisplayName:
                    result = StringToCSharpIdentifier(entity.DisplayName?.UserLocalizedLabel?.Label ?? entity.LogicalName);
                    break;

                case NameType.SchemaName:
                    result = entity.SchemaName;
                    break;
            }
            result = StripPrefix(result, lcgsettings);
            result = CamelCaseIt(result, lcgsettings);
            return result;
        }

        internal static string GetAttributeProperty(this AttributeMetadata attribute, Settings lcgsettings)
        {
            if (attribute.IsPrimaryId == true)
            {
                return "PrimaryKey";
            }
            if (attribute.IsPrimaryName == true)
            {
                return "PrimaryName";
            }
            var result = attribute.LogicalName;
            switch (lcgsettings.ConstantName)
            {
                case NameType.DisplayName:
                    result = StringToCSharpIdentifier(attribute.DisplayName?.UserLocalizedLabel?.Label ?? attribute.LogicalName);
                    break;

                case NameType.SchemaName:
                    result = attribute.SchemaName;
                    break;
            }
            result = StripPrefix(result, lcgsettings);
            result = CamelCaseIt(result, lcgsettings);
            return result;
        }
    }
}