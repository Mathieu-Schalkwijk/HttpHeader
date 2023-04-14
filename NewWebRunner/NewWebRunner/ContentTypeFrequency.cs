using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NewWebRunner
{
    internal class ContentTypeFrequency
    {
        public async Task<(string, Dictionary<string, int>)> GetContentTypeStatisticsAsync(List<string> webServerAddresses)
        {
            string stringResult = string.Empty;

            Dictionary<string, int> contentTypeStats = new Dictionary<string, int>();

            using (HttpClient client = new HttpClient())
            {
                foreach (string address in webServerAddresses)
                {
                    try
                    {
                        HttpResponseMessage response = await client.GetAsync(address);

                        if (response.IsSuccessStatusCode)
                        {
                            if (response.Content.Headers.Contains("Content-Type"))
                            {
                                string contentType = response.Content.Headers.ContentType.MediaType;

                                if (contentTypeStats.ContainsKey(contentType))
                                {
                                    contentTypeStats[contentType]++;
                                }
                                else
                                {
                                    contentTypeStats[contentType] = 1;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error for URL {address}: {ex.Message}");
                    }
                }
            }

            stringResult += "Content-Type Popularity Statistics:\n\n";

            //display results
            stringResult += "List of URLS:\n\n";
            foreach (string address in webServerAddresses)
            {
                stringResult += address + "\n";
            }

            stringResult += "\nFrequences of content-types\n\n";

            foreach (KeyValuePair<string, int> stat in contentTypeStats)
            {
                stringResult += $"{stat.Key}: {stat.Value}\n";
            }

            return (stringResult, contentTypeStats);
        }
    }
}
