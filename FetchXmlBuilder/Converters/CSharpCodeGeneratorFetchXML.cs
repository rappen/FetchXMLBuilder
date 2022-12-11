using Rappen.XTB.FetchXmlBuilder.Settings;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Xml;

namespace Rappen.XTB.FetchXmlBuilder.Converters
{
    public class CSharpCodeGeneratorFetchXML
    {
        public static string GetCSharpFetchXMLCode(string fetchXml, CodeGenerators codesettings)
        {
            var data = new Dictionary<string, string>();
            var xml = new XmlDocument();
            xml.LoadXml(fetchXml);

            if (codesettings.FilterVariables)
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
            var cs = "";

            if (data.Count > 0)
            {
                cs = "var fetchData = new {\r\n  " + string.Join(",\r\n  ", data.Select(kvp => $"{kvp.Key} = \"{kvp.Value.Replace("\\", "\\\\").Replace("\"", "\\\"")}\"")) + "\r\n};\r\n";
            }

            var sb = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "  ",
                NewLineChars = "\r\n",
                NewLineHandling = NewLineHandling.Replace
            };
            using (var writer = XmlWriter.Create(sb, settings))
            {
                xml.Save(writer);
            }

            cs += "var fetchXml = $@\"" + sb.Replace("\"", "\"\"").ToString() + "\";";

            return cs;
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