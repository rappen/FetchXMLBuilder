using McTools.Xrm.Connection;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;

namespace Cinteros.Xrm.FetchXmlBuilder.AppCode
{
    public class LogUsage
    {
        public static async Task DoLog(ConnectionDetail Connection, string action)
        {
            try
            {
                var ass = Assembly.GetExecutingAssembly().GetName();
                var version = ass.Version.ToString();
                var name = ass.Name.Replace(" ", "");
                var conn = Connection != null ? Connection.ConnectionName : "";

                var query = "t.php" +
                    "?sc_project=10396418" +
                    "&security=95f631d9" +
                    "&java=1" +
                    "&invisible=1" +
                    "&u={3}" +
                    "&camefrom={0} {1} {2}";

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(name, version));
                    client.BaseAddress = new Uri("http://c.statcounter.com/");
                    var response = await client.GetAsync(string.Format(query, name, version, conn, action)).ConfigureAwait(continueOnCapturedContext: false);
                    response.EnsureSuccessStatusCode();
                    if (response.IsSuccessStatusCode)
                    {
                        var data = response.Content.ReadAsStringAsync();
                    }

                    //client.BaseAddress = new Uri("http://gs.statcounter.com/");
                    //var response = await client.GetAsync("detect").ConfigureAwait(continueOnCapturedContext: false);
                    //response.EnsureSuccessStatusCode();
                    //if (response.IsSuccessStatusCode)
                    //{
                    //    var data = response.Content.ReadAsStringAsync();
                    //}

                }
            }
            catch { }
        }
    }
}
