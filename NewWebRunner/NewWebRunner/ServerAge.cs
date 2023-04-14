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
            string stringResult = string.Empty;

            List<double> ages = new List<double>();

            using (HttpClient client = new HttpClient())
            {
                foreach (string address in webServerAddresses)
                {
                    try
                    {
                        HttpResponseMessage response = await client.GetAsync(address);

                        if (response.IsSuccessStatusCode)
                        {
                            if (response.Content.Headers.LastModified.HasValue)
                            {
                                //Console.WriteLine($"Last-Modified header found for {address}");

                                DateTimeOffset currentDate = DateTime.Now;
                                DateTimeOffset lastModifiedDate = response.Content.Headers.LastModified.Value;

                                double ageInDays = (currentDate - lastModifiedDate).TotalDays;
                                ages.Add(ageInDays);
                            }
                            else
                            {
                                //Console.WriteLine($"Error: Last-Modified header not found for {address}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error for URL {address}: {ex.Message}");
                    }
                }
            }

            double averageAge = ages.Average();
            double standardDeviation = Math.Sqrt(ages.Select(age => Math.Pow(age - averageAge, 2)).Average());

            //display urls
            stringResult += "List of URLS:\n\n";
            foreach (string address in webServerAddresses)
            {
                stringResult += address + "\n";
            }

            stringResult += "\nAverage Age and Standard Deviation of Web Servers (when was the page modified for the last time):\n\n";
            stringResult += $"Average Age (in days): {averageAge}\n";
            stringResult += $"Standard Deviation (in days): {standardDeviation}\n";

            return (stringResult, averageAge, standardDeviation);
        }
    }
}
