using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Cinteros.Xrm.FetchXmlBuilder.AppCode
{
    public class CSharpCodeGenerator : CodeGeneratorBase
    {
         public static string GetCSharpCode(string fetchXml)
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
                        fetch += space + "<condition attribute='" + name + "' operator='" + @operator + "' value='{" + codeValue + "}'/>\n";
                    }
                    else
                        fetch += line + "\n";
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
                        fetch += space + "<value>{" + codeValue + "}</value>\n";
                    }
                    else
                        fetch += line + "\n";
                }
                else
                    fetch += line + "\n";
            }
            var cs = string.Empty;
            if (data.Count > 0)
            {
                cs += "\tvar fetchData = new {\r\n";
                foreach (var nv in data)
                    cs += "\t\t" + nv.Name + " = " + "\"" + nv.Value + "\",\r\n";
                cs = cs.Substring(0, cs.Length - ",\r\n".Length);
                cs += "\n\t};\r\n";
            }
            cs += "\tvar fetchXml = $@\"\r\n";
            cs += fetch.Substring(0, fetch.Length - 1);
            cs += "\";\r\n";
            return cs;
        }
    }
}