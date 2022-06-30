using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Rappen.XTB.FetchXmlBuilder.AppCode
{
    internal class QueryInfo
    {
        public QueryBase Query;
        public string AttributesSignature;
        public EntityCollection Results;
    }
}