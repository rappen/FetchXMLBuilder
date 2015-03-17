using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinteros.Xrm.FetchXmlBuilder.AppCode
{
    class FetchIsAggregateException : Exception
    {
        public FetchIsAggregateException(string message) : base(message) { }
    }
}
