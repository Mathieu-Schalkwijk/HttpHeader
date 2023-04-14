using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NewWebRunner
{
    internal class ServerFrequency
    {
        public async Task<(string, Dictionary<string, int>)> GetServerStatisticsAsync(List<string> webServerAddresses)
        {
            StringBuilder stringResult = new StringBuilder();

            Dictionary<string, int> serverStats = new Dictionary<string, int>();
            serverStats["NotFound"] = 0; //when response does not include "Server"

            using (HttpClient client = new HttpClient())
            {
                var tasks = webServerAddresses.Select(async address =>
                {
                    string serverType = "NotFound";

                    try
                    {
                        HttpResponseMessage response = await client.GetAsync(address);

                        if (response.IsSuccessStatusCode)
                        {
                            if (response.Headers.Contains("Server"))
                            {
                                serverType = response.Headers.GetValues("Server").FirstOrDefault();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error for URL {address}: {ex.Message}");
                    }

                    return (address, serverType);
                });

                var results = await Task.WhenAll(tasks);

                foreach (var (address, serverType) in results)
                {
                    if (serverStats.ContainsKey(serverType))
                    {
                        serverStats[serverType]++;
                    }
                    else
                    {
                        serverStats[serverType] = 1;
                    }
                }
            }

            stringResult.AppendLine("Server Popularity Statistics:\n\n");

            //display results
            stringResult.AppendLine("List of URLS:\n");
            foreach (string address in webServerAddresses)
            {
                stringResult.AppendLine(address);
            }

            stringResult.AppendLine("\nFrequences of servers (NotFound for response headers which do not contains a server):\n");

            foreach (KeyValuePair<string, int> stat in serverStats)
            {
                stringResult.AppendLine($"{stat.Key}: {stat.Value}");
            }

            return (stringResult.ToString(), serverStats);
        }
    }
}