using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NewWebRunner
{
    internal class RedirectionCounter
    {
        public async Task<string> GetRedirectionStatisticsAsync(List<string> webServerAddresses)
        {
            StringBuilder stringResult = new StringBuilder();

            Dictionary<string, int> redirectionStats = new Dictionary<string, int>();

            using (HttpClientHandler handler = new HttpClientHandler())
            {
                handler.AllowAutoRedirect = false;

                using (HttpClient client = new HttpClient(handler))
                {
                    //we parallelise the requests
                    var tasks = webServerAddresses.Select(async address =>
                    {
                        int redirectionCount = 0;

                        try
                        {
                            string currentAddress = address;
                            HttpResponseMessage response = await client.GetAsync(currentAddress);

                            while ((int)response.StatusCode >= 300 && (int)response.StatusCode < 400)
                            {
                                if (response.Headers.Location.IsAbsoluteUri)
                                {
                                    currentAddress = response.Headers.Location.AbsoluteUri;
                                }
                                else
                                {
                                    currentAddress = new Uri(new Uri(currentAddress), response.Headers.Location).AbsoluteUri;
                                }

                                response = await client.GetAsync(currentAddress);
                                redirectionCount++;
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error for URL {address}: {ex.Message}");
                        }

                        return (address, redirectionCount);
                    });

                    var results = await Task.WhenAll(tasks);

                    foreach (var (address, redirectionCount) in results)
                    {
                        redirectionStats[address] = redirectionCount;
                    }
                }
            }

            stringResult.AppendLine("Redirection Statistics:\n");

            foreach (var stat in redirectionStats.OrderByDescending(x => x.Value))
            {
                stringResult.AppendLine($"{stat.Value} redirections for request to {stat.Key}");
            }

            return stringResult.ToString();
        }
    }
}
