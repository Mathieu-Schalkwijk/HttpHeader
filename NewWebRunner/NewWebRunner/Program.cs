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

            ServerFrequency serverFrequency = new ServerFrequency();
            ServerAge serverAge = new ServerAge();
            ContentTypeFrequency contentTypeFrequency = new ContentTypeFrequency();
            RedirectionCounter redirectionCounter = new RedirectionCounter();

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

            string port = "5000";

            //Start server
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
                    int id = int.Parse(request.QueryString["id"]);//get scenario number
                    string result = "";
                    switch (id)
                    {
                        case 1:
                            //case 1 : servers statistics
                            (string string1, Dictionary<string, int> serverStats) = await serverFrequency.GetServerStatisticsAsync(webServerAddresses);
                            result = string1;
                            break;
                        case 2:
                            //case 2 : average age and standard deviation
                            (string string2, double averageAge, double standardDeviation) = await serverAge.CalculateAverageAndStandardDeviationAsync(webServerAddresses);
                            result = string2;
                            break;
                        case 3:
                            (string string3, Dictionary<string, int> stats) = await contentTypeFrequency.GetContentTypeStatisticsAsync(webServerAddresses);
                            result = string3;
                            break;
                        case 4:
                            result = await redirectionCounter.GetRedirectionStatisticsAsync(webServerAddresses);
                            break;
                        default:
                            result = $"There is no scenario {id}";
                            break;
                    }

                    //return a string to the web interface
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

    }
}
