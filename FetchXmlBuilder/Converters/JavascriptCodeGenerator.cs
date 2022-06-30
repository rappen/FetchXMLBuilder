using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Xml;

namespace Cinteros.Xrm.FetchXmlBuilder.Converters
{
    public class JavascriptCodeGenerator
    {
        public static string GetJavascriptCode(string fetchXml)
        {
            var data = new Dictionary<string, string>();
            var lines = new List<string>();
            var xml = new XmlDocument();
            xml.LoadXml(fetchXml);

            Convert(xml.DocumentElement, 0, lines, data);

            var js = "";

            if (data.Count > 0)
            {
                js = $"var fetchData = {JsonConvert.SerializeObject(data, Newtonsoft.Json.Formatting.Indented)};\r\n";
            }

            js += $"var fetchXml = [\r\n{string.Join(",\r\n", lines)}\r\n].join(\"\");";

            return js;
        }

        private static void Convert(XmlElement element, int depth, List<string> lines, Dictionary<string, string> data)
        {
            var lineComponents = new List<string>();
            var line = new string(' ', depth * 2) + $"<{element.Name}";

            foreach (XmlAttribute attribute in element.Attributes)
            {
                line += $" {attribute.Name}='";

                if (attribute.Name == "value" && element.Name == "condition")
                {
                    lineComponents.Add(JsonConvert.SerializeObject(line));
                    AddData(element.GetAttribute("attribute"), attribute.Value, data, lineComponents);
                    line = "'";
                }
                else
                {
                    line += $"{SecurityElement.Escape(attribute.Value)}'";
                }
            }

            if (element.IsEmpty)
            {
                line += "/>";
                lineComponents.Add(JsonConvert.SerializeObject(line));
                lines.Add(string.Join(", ", lineComponents));
            }
            else
            {
                line += ">";
                lineComponents.Add(JsonConvert.SerializeObject(line));

                if (element.Name == "value" && element.ParentNode is XmlElement parentElement && parentElement.Name == "condition")
                {
                    AddData(parentElement.GetAttribute("attribute"), element.InnerText.Trim(), data, lineComponents);
                    lineComponents.Add("\"</value>\"");
                    lines.Add(string.Join(", ", lineComponents));
                }
                else
                {
                    lines.Add(string.Join(", ", lineComponents));

                    foreach (var child in element.ChildNodes.OfType<XmlElement>())
                    {
                        Convert(child, depth + 1, lines, data);
                    }

                    lines.Add(JsonConvert.SerializeObject(new string(' ', depth * 2) + $"</{element.Name}>"));
                }
            }
        }

        private static void AddData(string attribute, string value, Dictionary<string, string> data, List<string> lineComponents)
        {
            var key = attribute;

            var suffix = 1;
            while (data.ContainsKey(key))
            {
                suffix++;
                key = attribute + suffix;
            }

            data[key] = SecurityElement.Escape(value);

            lineComponents.Add($"fetchData.{key}/*{value.Replace("*/", "")}*/");
        }
    }
}