using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinteros.Xrm.FetchXmlBuilder.AppCode
{
    internal class QueryInfo
    {
        public QueryBase Query;
        public string AttributesSignature;
        public EntityCollection Results;
    }
}
