using System;

namespace Rappen.XTB.FetchXmlBuilder.Builder
{
    internal class FetchIsAggregateException : Exception
    {
        public FetchIsAggregateException(string message) : base(message)
        {
        }
    }
}