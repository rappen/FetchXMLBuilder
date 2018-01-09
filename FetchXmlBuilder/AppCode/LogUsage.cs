using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;

namespace Cinteros.Xrm.FetchXmlBuilder.AppCode
{
    public static class LogUsage
    {
        public static async Task DoLog(string action)
        {
            try
            {
                var ass = Assembly.GetExecutingAssembly().GetName();
                var version = ass.Version.ToString();
                var name = ass.Name.Replace(" ", "");
                action = "FXB-" + action;

                var query = "t.php" +
                    "?sc_project=10396418" +
                    "&security=95f631d9" +
                    "&java=1" +
                    "&invisible=1" +
                    "&u={2}" +
                    "&camefrom={0} {1}";

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(name, version));
                    client.BaseAddress = new Uri("http://c.statcounter.com/");
                    var response = await client.GetAsync(string.Format(query, name, version, action)).ConfigureAwait(continueOnCapturedContext: false);
                    response.EnsureSuccessStatusCode();
                    if (response.IsSuccessStatusCode)
                    {
                        response.Content.ReadAsStringAsync();
                    }
                }
            }
            catch { }
        }
    }
}
