using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Xml;

namespace Cinteros.Xrm.FetchXmlBuilder.Extensions
{
    public static class ServiceExtensions
    {
        /// <summary>
        /// Converts from QueryBase to FetchXML, removing invalid attribute useraworderby to validate with fetch.xsd
        /// Issue reported here: https://github.com/MicrosoftDocs/dynamics-365-customer-engagement/issues/233
        /// </summary>
        /// <param name="organizationService"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public static string QueryExpressionToFetchXml(this IOrganizationService organizationService, QueryBase query)
        {
            QueryExpressionToFetchXmlRequest request = new QueryExpressionToFetchXmlRequest()
            {
                Query = query
            };
            QueryExpressionToFetchXmlResponse response = (QueryExpressionToFetchXmlResponse)organizationService.Execute(request);
            var doc = new XmlDocument();
            doc.LoadXml(response.FetchXml);
            var fetchnode = doc.SelectSingleNode("fetch");
            if (fetchnode != null && fetchnode.Attributes["useraworderby"] != null)
            {
                fetchnode.Attributes.RemoveNamedItem("useraworderby");
            }
            return doc.OuterXml;
        }
    }
}