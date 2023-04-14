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
            string stringResult = string.Empty;

            Dictionary<string, int> serverStats = new Dictionary<string, int>();
            serverStats["NotFound"] = 0; //when response does not include "Server"

            using (HttpClient client = new HttpClient())
            {
                foreach (string address in webServerAddresses)
                {
                    try
                    {
                        HttpResponseMessage response = await client.GetAsync(address);

                        if (response.IsSuccessStatusCode)
                        {
                            if (response.Headers.Contains("Server"))
                            {
                                string serverType = response.Headers.GetValues("Server").FirstOrDefault();

                                if (serverStats.ContainsKey(serverType))
                                {
                                    serverStats[serverType]++;
                                }
                                else
                                {
                                    serverStats[serverType] = 1;
                                }
                            }
                            else
                            {
                                serverStats["NotFound"]++;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error for URL {address}: {ex.Message}");
                    }
                }
            }

            stringResult += "Server Popularity Statistics:\n\n";


            //display results
            stringResult += "List of URLS:\n\n";
            foreach (string address in webServerAddresses)
            {
                stringResult += address + "\n";
            }

            stringResult += "\nFrequences of servers (NotFound for response headers which do not contains a server):";

            foreach (KeyValuePair<string, int> stat in serverStats)
            {
                stringResult += $"{stat.Key}: {stat.Value}\n";
            }

            return (stringResult, serverStats);
        }
    }
}
