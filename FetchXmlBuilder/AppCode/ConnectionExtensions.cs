using McTools.Xrm.Connection;
using System;

namespace Cinteros.Xrm.FetchXmlBuilder.AppCode
{
    public static class ConnectionExtensions
    {
        public static string GetFullWebApplicationUrl(this ConnectionDetail connectiondetail)
        {
            var url = connectiondetail.WebApplicationUrl;
            if (string.IsNullOrEmpty(url))
            {
                url = connectiondetail.ServerName;
            }
            if (!url.ToLower().StartsWith("http"))
            {
                url = string.Concat("http://", url);
            }
            var uri = new Uri(url);
            if (!uri.Host.EndsWith(".dynamics.com"))
            {
                if (string.IsNullOrEmpty(uri.AbsolutePath.Trim('/')))
                {
                    uri = new Uri(uri, connectiondetail.Organization);
                }
            }
            return uri.ToString();
        }

        public static string GetWebApiServiceUrl(this ConnectionDetail connectiondetail)
        {
            var url = new Uri(new Uri(connectiondetail.GetFullWebApplicationUrl()), $"api/data/v{connectiondetail.OrganizationMajorVersion}.{connectiondetail.OrganizationMinorVersion}");
            return url.ToString();
        }
    }
}
