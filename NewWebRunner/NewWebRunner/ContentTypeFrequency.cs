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
            StringBuilder stringResult = new StringBuilder();

            Dictionary<string, int> contentTypeStats = new Dictionary<string, int>();

            using (HttpClient client = new HttpClient())
            {
                var tasks = webServerAddresses.Select(async address =>
                {
                    string contentType = string.Empty;

                    try
                    {
                        HttpResponseMessage response = await client.GetAsync(address);

                        if (response.IsSuccessStatusCode)
                        {
                            if (response.Content.Headers.Contains("Content-Type"))
                            {
                                contentType = response.Content.Headers.ContentType.MediaType;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error for URL {address}: {ex.Message}");
                    }

                    return (address, contentType);
                });

                var results = await Task.WhenAll(tasks);

                foreach (var (address, contentType) in results)
                {
                    if (!string.IsNullOrEmpty(contentType))
                    {
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

            stringResult.AppendLine("Content-Type Popularity Statistics:\n\n");

            //display results
            stringResult.AppendLine("List of URLS:\n");
            foreach (string address in webServerAddresses)
            {
                stringResult.AppendLine(address);
            }

            stringResult.AppendLine("\nFrequences of content-types\n");

            foreach (KeyValuePair<string, int> stat in contentTypeStats)
            {
                stringResult.AppendLine($"{stat.Key}: {stat.Value}");
            }

            return (stringResult.ToString(), contentTypeStats);
        }
    }
}
