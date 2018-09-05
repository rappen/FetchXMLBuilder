using System.Collections.Generic;
using System.Linq;

namespace Cinteros.Xrm.FetchXmlBuilder.AppCode
{
    public class CodeGeneratorBase
    {
        internal class NameValue
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }


        internal static NameValue GetFetchData(List<NameValue> data, string name, string value)
        {
            var index = data.Where(r => r.Name == name || (r.Name.StartsWith(name) && int.TryParse(r.Name.Substring(name.Length), out int i))).Count();
            if (index == 0) return new NameValue { Name = name, Value = value };
            return new NameValue { Name = name + (index + 1).ToString(), Value = value };
        }
    }
}
