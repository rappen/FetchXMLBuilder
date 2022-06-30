using System;

namespace Cinteros.Xrm.FetchXmlBuilder.AppCode
{
    internal class FetchIsAggregateException : Exception
    {
        public FetchIsAggregateException(string message) : base(message)
        {
        }
    }
}