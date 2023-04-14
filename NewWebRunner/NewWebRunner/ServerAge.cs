using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NewWebRunner
{
    internal class ServerAge
    {
        public async Task<(string stringResult, double averageAge, double standardDeviation)> CalculateAverageAndStandardDeviationAsync(List<string> webServerAddresses)
        {
            StringBuilder stringResult = new StringBuilder();

            List<double> ages = new List<double>();

            using (HttpClient client = new HttpClient())
            {
                var tasks = webServerAddresses.Select(async address =>
                {
                    double? ageInDays = null;

                    try
                    {
                        HttpResponseMessage response = await client.GetAsync(address);

                        if (response.IsSuccessStatusCode)
                        {
                            if (response.Content.Headers.LastModified.HasValue)
                            {
                                DateTimeOffset currentDate = DateTime.Now;
                                DateTimeOffset lastModifiedDate = response.Content.Headers.LastModified.Value;

                                ageInDays = (currentDate - lastModifiedDate).TotalDays;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error for URL {address}: {ex.Message}");
                    }

                    return (address, ageInDays);
                });

                var results = await Task.WhenAll(tasks);

                foreach (var (address, ageInDays) in results)
                {
                    if (ageInDays.HasValue)
                    {
                        ages.Add(ageInDays.Value);
                    }
                }
            }

            double averageAge = ages.Average();
            double standardDeviation = Math.Sqrt(ages.Select(age => Math.Pow(age - averageAge, 2)).Average());

            //display urls
            stringResult.AppendLine("List of URLS:\n");
            foreach (string address in webServerAddresses)
            {
                stringResult.AppendLine(address);
            }

            stringResult.AppendLine("\nAverage Age and Standard Deviation of Web pages (when was the page modified for the last time):\n");
            stringResult.AppendLine($"Average Age (in days): {averageAge}");
            stringResult.AppendLine($"Standard Deviation (in days): {standardDeviation}");

            return (stringResult.ToString(), averageAge, standardDeviation);
        }
    }
}
