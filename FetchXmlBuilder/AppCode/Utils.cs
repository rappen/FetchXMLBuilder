namespace Cinteros.Xrm.FetchXmlBuilder.AppCode
{
    public static class Utils
    {
        private const string DocsAndLearnToken = "WT.mc_id=BA-MVP-5002475";

        public static string ProcessURL(string url)
        {
            if (url.ToLowerInvariant().Contains("microsoft.com") && !url.ToLowerInvariant().Contains(DocsAndLearnToken))
            {
                var param = url.Contains("?") ? "&" : "?";
                param += DocsAndLearnToken;
                if (url.Contains("#"))
                {
                    url = url.Split('#')[0] + param + "#" + url.Split('#')[1];
                }
                else
                {
                    url += param;
                }
            }
            return url;
        }
    }
}
