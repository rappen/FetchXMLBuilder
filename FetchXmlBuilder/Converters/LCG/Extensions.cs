using Microsoft.Xrm.Sdk.Metadata;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rappen.XTB.LCG
{
    public static class Extensions
    {
        public static AttributeMetadata GetAttribute(this Dictionary<string, EntityMetadata> entities, string entity, string attribute)
        {
            if (entities == null
                || !entities.TryGetValue(entity, out var metadata)
                || metadata.Attributes == null)
            {
                return null;
            }

            return metadata.Attributes.FirstOrDefault(metaattribute => metaattribute.LogicalName == attribute);
        }

        private static string BeautifyContent(this string content, string indentstr)
        {
            var fixedcontent = new StringBuilder();
            var lines = content.Split('\n').ToList();
            var lastline = string.Empty;
            var indent = 0;
            foreach (var line in lines.Select(l => l.Trim()).Where(l => !string.IsNullOrEmpty(l)))
            {
                if (AddBlankLineBetween(lastline, line))
                {
                    fixedcontent.AppendLine();
                }
                if (lastline.EndsWith("{"))
                {
                    indent++;
                }
                if (line.Equals("}") && indent > 0)
                {
                    indent--;
                }
                fixedcontent.AppendLine(string.Concat(Enumerable.Repeat(indentstr, indent)) + line);
                lastline = line;
            }
            return fixedcontent.ToString();
        }

        private static bool AddBlankLineBetween(string lastline, string line)
        {
            if (string.IsNullOrWhiteSpace(lastline) || lastline.Equals("{"))
            {   // Never two empty lines after each other
                return false;
            }
            if (lastline.StartsWith("#region") || line.StartsWith("#region") || line.StartsWith("#endregion"))
            {   // Empty lines around region statements
                return true;
            }
            if (lastline.StartsWith("using ") && !line.StartsWith("using "))
            {   // Empty lines after usings
                return true;
            }
            if (line.StartsWith("namespace "))
            {   // Empty lines before namespace
                return true;
            }
            if (line.StartsWith("public enum"))
            {   // Never empty line before enums, we keep it compact
                return false;
            }
            if (lastline.Equals("}") && !line.Equals("}") && !string.IsNullOrWhiteSpace(line))
            {   // Never empty line between end blocks
                return true;
            }
            // Following rules are UML specific
            if (line.StartsWith("@startuml") || lastline.StartsWith("@startuml") || line.StartsWith("@enduml"))
            {
                return true;
            }
            if (line.StartsWith("title") || line.StartsWith("header") || line.StartsWith("footer "))
            {
                return true;
            }
            if (line.StartsWith("skinparam") && !lastline.StartsWith("skinparam"))
            {
                return true;
            }
            if (line.StartsWith("entity "))
            {
                return true;
            }
            return false;
        }

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

        public static string GetNonDisplayName(this Settings settings, string name)
        {
            if (settings.DoStripPrefix && !string.IsNullOrEmpty(settings.StripPrefix))
            {
                foreach (var prefix in settings.StripPrefix.Split(',')
                                               .Select(p => p.Trim())
                                               .Where(p => !string.IsNullOrWhiteSpace(p)
                                                      && name.ToLowerInvariant().StartsWith(p)))
                {
                    name = name.Substring(prefix.Length);
                }
            }
            if (settings.ConstantCamelCased)
            {
                name = name.CamelCaseIt(settings);
            }
            return name;
        }

        public static string ReplaceIfNotEmpty(this string template, string oldValue, string newValue)
        {
            return string.IsNullOrEmpty(template) ? newValue : template.Replace(oldValue, newValue);
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