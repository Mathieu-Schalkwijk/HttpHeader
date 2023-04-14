using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NewWebRunner
{
    internal class ResponseTimeStatistics
    {
        public async Task<(string stringResult, double averageResponseTime)> GetResponseTimeStatisticsAsync(List<string> webServerAddresses)
        {
            StringBuilder stringResult = new StringBuilder();

            List<double> responseTimes = new List<double>();

            using (HttpClient client = new HttpClient())
            {
                var tasks = webServerAddresses.Select(async address =>
                {
                    double responseTime = -1;

                    try
                    {
                        Stopwatch stopwatch = new Stopwatch();
                        stopwatch.Start();

                        HttpResponseMessage response = await client.GetAsync(address);
                        stopwatch.Stop();

                        responseTime = stopwatch.Elapsed.TotalMilliseconds;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error for URL {address}: {ex.Message}");
                    }

                    return (address, responseTime);
                });

                var results = await Task.WhenAll(tasks);

                foreach (var (address, responseTime) in results)
                {
                    if (responseTime >= 0)
                    {
                        responseTimes.Add(responseTime);
                    }
                }
            }

            double averageResponseTime = responseTimes.Average();

            //display urls
            stringResult.AppendLine("List of URLS and their response times (in milliseconds):\n");
            foreach (string address in webServerAddresses)
            {
                stringResult.AppendLine($"{address}: {responseTimes[webServerAddresses.IndexOf(address)]}");
            }

            stringResult.AppendLine("\nAverage response time (in milliseconds):\n");
            stringResult.AppendLine($"{averageResponseTime}");

            return (stringResult.ToString(), averageResponseTime);
        }
    }
}
