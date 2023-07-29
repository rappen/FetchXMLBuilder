using System;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

namespace Rappen.XTB.FetchXmlBuilder.AppCode
{
    public static class Utils
    {
        private static NameValueCollection commonparams = new NameValueCollection { { "utm_source", "FetchXMLBuilder" }, { "utm_medium", "XrmToolBox" } };
        private static NameValueCollection microsoftparams = new NameValueCollection { { "WT.mc_id", "BA-MVP-5002475" } };

        public static string ProcessURL(string url)
        {
            if (!Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out Uri _))
            {
                return url;
            }
            var urib = new UriBuilder(url);
            var qry = HttpUtility.ParseQueryString(urib.Query);
            if (urib.Host.ToLowerInvariant().Contains("microsoft.com"))
            {
                microsoftparams.AllKeys.ToList().ForEach(k => qry[k] = microsoftparams[k]);
                urib.Path = urib.Path.Replace("/en-us/", "/");
            }
            commonparams.AllKeys.ToList().ForEach(k => qry[k] = commonparams[k]);

            urib.Query = qry.ToString();
            return urib.Uri.ToString();
        }
    }
}