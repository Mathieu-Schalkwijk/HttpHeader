using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NewWebRunner
{
    internal class Program
    {
        static async Task Main(string[] args)
        {

            List<string> webServerAddresses = new List<string>
            {
                "https://google.com",
                "https://amazon.com",
                "https://apple.com",
                "https://wikipedia.com",
                "https://youtube.com",
                "https://facebook.com",
                "https://discord.com",
                "https://smee.io",
                "https://twitter.com",
                "https://dell.com",
            };

            /*(string string1, Dictionary<string, int> serverStats) = await GetServerStatisticsAsync(webServerAddresses);
            Console.WriteLine("Results from scenario 1:\n" + string1);

            (string string2, double averageAge, double standardDeviation) = await CalculateAverageAndStandardDeviationAsync(webServerAddresses);
            Console.WriteLine("Results from scenario 2:\n" + string2);

            Console.ReadLine();*/

            string port = "5000";

            string baseDirectory = Directory.GetCurrentDirectory();
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add($"http://localhost:{port}/");
            listener.Start();
            Console.WriteLine($"Listening on port {port}...");

            while (true)
            {
                HttpListenerContext context = await listener.GetContextAsync();
                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;

                Console.WriteLine($"Request received at {request.Url.AbsolutePath}");

                if (request.Url.AbsolutePath == "/")//give index.html from this server when connecting on localhost 5000
                {
                    //string filePath = "index.html";
                    string filePath = Path.Combine(baseDirectory, "wwwroot", "index.html");

                    if (File.Exists(filePath))
                    {
                        byte[] buffer = File.ReadAllBytes(filePath);
                        response.ContentType = "text/html";
                        response.ContentLength64 = buffer.Length;
                        response.OutputStream.Write(buffer, 0, buffer.Length);
                    }
                    else
                    {
                        Console.WriteLine($"File {filePath} not found");
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                    }
                }
                else if (request.Url.AbsolutePath == "/scenario")
                {
                    int id = int.Parse(request.QueryString["id"]);
                    string result = "";
                    switch (id)
                    {
                        case 1:
                            (string string1, Dictionary<string, int> serverStats) = await GetServerStatisticsAsync(webServerAddresses);
                            result= string1;
                            break;
                        case 2:
                            (string string2, double averageAge, double standardDeviation) = await CalculateAverageAndStandardDeviationAsync(webServerAddresses);
                            result = string2;
                            break;
                        default:
                            result = $"There is no scenario {id}";
                            break;
                    }

                    byte[] buffer = Encoding.UTF8.GetBytes(result);
                    response.ContentLength64 = buffer.Length;
                    response.ContentType = "text/plain";
                    response.OutputStream.Write(buffer, 0, buffer.Length);
                }
                else
                {
                    Console.WriteLine($"Path {request.Url.AbsolutePath} not found");
                }
                response.Close();
            }
        }

        private static async Task<(string, Dictionary<string, int>)> GetServerStatisticsAsync(List<string> webServerAddresses)
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

        private static async Task<(string stringResult, double averageAge, double standardDeviation)> CalculateAverageAndStandardDeviationAsync(List<string> webServerAddresses)
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
