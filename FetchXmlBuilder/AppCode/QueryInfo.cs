using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Rappen.XRM.Helpers.Extensions;
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
        internal TimeSpan Elapsed = TimeSpan.Zero;

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
                    if (fetch.Query.ToXml().SelectSingleNode("fetch") is XmlElement fetchnode)
                    {
                        PageSize = Math.Min(fetchnode.AttributeInt("count") ?? 0, 5000);
                        PageNo = fetchnode.AttributeInt("page") ?? 1;
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
                    if (result.PagingCookie.ToXml().SelectSingleNode("cookie") is XmlElement cookie &&
                        cookie.AttributeInt("page") is int page)
                    {
                        PageNo = page;
                    }
                }
                if (PageSize == 0)
                {
                    if (result.MoreRecords)
                    {
                        PageSize = result.Entities.Count;
                    }
                    else
                    {
                        PageSize = 5000;
                    }
                }
                if (result.TotalRecordCount > -1 && result.TotalRecordCount < 5000 && PageSize > 0 && result.TotalRecordCount > PageSize)
                {
                    Pages = (int)Math.Ceiling((decimal)result.TotalRecordCount / PageSize);
                }
                RecordFrom = 1 + (PageNo - 1) * PageSize;
                if (result.Entities.Count > 0)
                {
                    RecordTo = RecordFrom - 1 + result.Entities.Count;
                }
            }
        }
    }
}