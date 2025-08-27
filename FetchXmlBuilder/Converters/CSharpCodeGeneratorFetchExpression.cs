using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Rappen.XRM.Helpers.Extensions;
using Rappen.XTB.FetchXmlBuilder.Settings;

namespace Rappen.XTB.FetchXmlBuilder.Converters
{
    internal class CSharpCodeGeneratorFetchExpression
    {
        internal static string GetCSharpFetchExpression(string fetchXml, FXBSettings codesettings)
        {
            var data = new Dictionary<string, string>();
            var xml = fetchXml.ToXml();

            if (codesettings.CodeGenerators.FilterVariables)
            {
                var conditionAttributes = xml.SelectNodes("//condition/@value");

                foreach (XmlAttribute attribute in conditionAttributes)
                {
                    var value = AddData(attribute.OwnerElement.GetAttribute("attribute"), attribute.Value, data);
                    attribute.Value = $"{{{value}}}";
                }

                var conditionValues = xml.SelectNodes("//condition/value");

                foreach (XmlElement val in conditionValues)
                {
                    var value = AddData(((XmlElement)val.ParentNode).GetAttribute("attribute"), val.InnerText.Trim(), data);
                    val.InnerText = $"{{{value}}}";
                }
            }

            var csharpBuilder = new StringBuilder();

            if (data.Any())
            {
                csharpBuilder.AppendLine("var fetchData = new");
                csharpBuilder.AppendLine("{");

                foreach (var kvp in data)
                {
                    var key = kvp.Key;
                    var value = kvp.Value
                        .Replace("\\", "\\\\")
                        .Replace("\"", "\\\"");

                    csharpBuilder
                        .Append(new string(' ', 4))
                        .AppendLine($"{key} = \"{value}\","); // trailing comma is allowed
                }

                csharpBuilder.AppendLine("};");
                csharpBuilder.AppendLine();
            }

            var xmlBuilder = new StringBuilder();
            var xmlWriterSettings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = new string(' ', 2),
                NewLineChars = Environment.NewLine,
                NewLineHandling = NewLineHandling.Replace,
            };

            using var xmlWriter = XmlWriter.Create(xmlBuilder, xmlWriterSettings);
            xml.Save(xmlWriter);

            xmlBuilder.Replace('"', '\'');

            csharpBuilder.AppendLine($"var fetch = $@\"{xmlBuilder}\";");
            csharpBuilder.AppendLine();
            csharpBuilder.AppendLine("var query = new FetchExpression(fetch);");

            if (codesettings.CodeGenerators.Indents > 0)
            {
                csharpBuilder.Insert(0, new string(' ', 4 * codesettings.CodeGenerators.Indents));
                csharpBuilder.Replace(Environment.NewLine, Environment.NewLine + new string(' ', 4 * codesettings.CodeGenerators.Indents));
            }

            return csharpBuilder.ToString();
        }

        private static string AddData(string attribute, string value, Dictionary<string, string> data)
        {
            var key = attribute;

            var suffix = 1;
            while (data.ContainsKey(key))
            {
                suffix++;
                key = attribute + suffix;
            }

            data[key] = SecurityElement.Escape(value);

            return $"fetchData.{key}/*{value.Replace("*/", "")}*/";
        }
    }
}
