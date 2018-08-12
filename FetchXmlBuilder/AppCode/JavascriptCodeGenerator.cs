using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Cinteros.Xrm.FetchXmlBuilder.AppCode
{
    public class JavascriptCodeGenerator
    {
        private class NameValue
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }

        public static string GetJavascriptCode(string fetchXml)
        {
            var data = new List<NameValue>();
            var fetch = string.Empty;
            var name = string.Empty;
            fetchXml = fetchXml.Replace("\"", "'");
            var lines = fetchXml.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                var space = line.Substring(0, line.IndexOf("<"));
                if (line.Trim().StartsWith("<condition"))
                {
                    var pattern = "('(.*?)' |'(.*?)'/>)";
                    var matches = new Regex(pattern).Matches(line);
                    name = matches[0].Value.Substring(1, matches[0].Value.Length - 3);
                    if (matches.Count == 3 || matches.Count == 5)
                    {
                        var @operator = matches[1].Value.Substring(1, matches[1].Value.Length - 3);
                        var value = matches[matches.Count - 1].Value.Substring(1, matches[matches.Count - 1].Value.Length - 3);
                        var fetchData = GetFetchData(data, name, value);
                        var codeValue = "fetchData." + fetchData.Name + "/*" + fetchData.Value + "*/";
                        data.Add(new NameValue { Name = fetchData.Name, Value = fetchData.Value });
                        fetch += "\"" + space + "<condition attribute='" + name + "' operator='" + @operator + "' value='" + "\", " + codeValue + ", \"'/>\",\n";
                    }
                    else
                        fetch += "\"" + line + "\",\n";
                }
                else if (line.Trim().StartsWith("<value"))
                {
                    var pattern = ">.*<";
                    var matches = new Regex(pattern).Matches(line);
                    if (matches.Count == 1)
                    {
                        var value = matches[0].Value.Substring(1, matches[0].Value.Length - 2);
                        var fetchData = GetFetchData(data, name, value);
                        var codeValue = "fetchData." + fetchData.Name + "/*" + fetchData.Value + "*/";
                        data.Add(new NameValue { Name = fetchData.Name, Value = fetchData.Value });
                        fetch += "\"" + space + "<value>\", " + codeValue + ", \"</value>\",\n";
                    }
                    else
                        fetch += "\"" + line + "\",\n";
                }
                else
                    fetch += "\"" + line + "\",\n";
            }
            var js = string.Empty;
            if (data.Count > 0)
            {
                js += "\tvar fetchData = {\r\n";
                foreach (var nv in data)
                    js += "\t\t" + nv.Name + ": " + "\"" + nv.Value + "\",\r\n";
                js = js.Substring(0, js.Length - ",\r\n".Length);
                js += "\n\t};\r\n";
            }
            js += "\tvar fetchXml = [\r\n";
            js += fetch.Substring(0, fetch.Length - 1);
            js += "\r\n\t].join(\"\");";
            return js;
        }

        private static NameValue GetFetchData(List<NameValue> data, string name, string value)
        {
            var index = data.Where(r => r.Name == name).Count();
            if (index == 0) return new NameValue { Name = name, Value = value };
            return new NameValue { Name = name + (index + 1).ToString(), Value = value };
        }
    }
}