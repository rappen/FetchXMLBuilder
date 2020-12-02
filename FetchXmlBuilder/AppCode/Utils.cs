using System;
using System.Web;

namespace Cinteros.Xrm.FetchXmlBuilder.AppCode
{
    public static class Utils
    {
        private const string DocsAndLearnToken = "WT.mc_id=BA-MVP-5002475";
        private const string UTMTokens = "utm_source=FXB&utm_medium=XrmToolBox";

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
                qry["WT.mc_id"] = "BA-MVP-5002475";
            }
            qry["utm_source"] = "FetchXMLBuilder";
            qry["utm_medium"] = "XrmToolBox";

            urib.Query = qry.ToString();
            return urib.Uri.ToString();
        }
    }
}
