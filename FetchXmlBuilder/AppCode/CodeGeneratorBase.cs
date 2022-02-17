using System.Collections.Generic;

namespace Cinteros.Xrm.FetchXmlBuilder.AppCode
{
    public class CodeGeneratorBase
    {
        internal class NameValue
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }

        private static bool foundFetchData(List<NameValue> data, string name)
        {
            for (var i = 0; i < data.Count; i++)
            {
                if (data[i].Name == name) return true;
            }
            return false;
        }

        internal static NameValue GetFetchData(List<NameValue> data, string name, string value)
        {
            var nv = new NameValue { Name = name, Value = value };
            var index = 1;
            var checkName = name;
            while (foundFetchData(data, checkName))
            {
                index = index + 1;
                checkName = name + index;
            }
            nv.Name = checkName;
            return nv;
        }
    }
}
