using System;

namespace Cinteros.Xrm.FetchXmlBuilder.Builder
{
    internal class FetchIsAggregateException : Exception
    {
        public FetchIsAggregateException(string message) : base(message)
        {
        }
    }
}