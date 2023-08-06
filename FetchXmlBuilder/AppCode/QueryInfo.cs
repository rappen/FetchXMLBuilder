using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Rappen.XRM.Helpers.FetchXML;
using System;
using System.Xml;

namespace Rappen.XTB.FetchXmlBuilder.AppCode
{
    internal class QueryInfo
    {
        private QueryBase query;
        private EntityCollection result;
        internal int PageSize = 0;
        internal int PageNo = 1;
        internal int Pages = -1;
        internal int RecordFrom = -1;
        internal int RecordTo = -1;

        public string QuerySignature;
        public string AttributesSignature;

        public QueryBase Query
        {
            get { return query; }
            set
            {
                query = value;
                if (query is FetchExpression fetch)
                {
                    var fetchdoc = new XmlDocument();
                    fetchdoc.LoadXml(fetch.Query);
                    if (fetchdoc.SelectSingleNode("fetch") is XmlElement fetchnode)
                    {
                        if (fetchnode.AttributeInt("count") is int count)
                        {
                            PageSize = count;
                        }
                        if (fetchnode.AttributeInt("page") is int page)
                        {
                            PageNo = page;
                        }
                    }
                }
            }
        }

        public EntityCollection Results
        {
            get { return result; }
            set
            {
                result = value;
                if (!string.IsNullOrEmpty(result.PagingCookie))
                {
                    var cookdoc = new XmlDocument();
                    cookdoc.LoadXml(result.PagingCookie);
                    if (cookdoc.SelectSingleNode("cookie") is XmlElement cookie &&
                        cookie.AttributeInt("page") is int page)
                    {
                        PageNo = page;
                    }
                }
                if (result.TotalRecordCount > -1 && PageSize > 0)
                {
                    Pages = (int)Math.Ceiling((decimal)result.TotalRecordCount / PageSize);
                }
                RecordFrom = 1 + (PageNo - 1) * PageSize;
                RecordTo = RecordFrom - 1 + result.Entities.Count;
            }
        }
    }
}