using Microsoft.Owin.Hosting;
using System;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace HttpClientWriteStreamFailureOn401
{
    class Program
    {
        static void Main(string[] args)
        {
            var baseAddress = "http://localhost:9510";

            using (WebApp.Start<Startup>(url: baseAddress))
            {
                using (HttpClient httpClient = new HttpClient(new HttpClientHandler()
                {
                    UseDefaultCredentials = true
                }))
                {
                    httpClient.BaseAddress = new Uri(baseAddress);
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    SendRequest(httpClient, HttpMethod.Get, "default/getData").Wait();
                    SendRequest(httpClient, HttpMethod.Get, "default/getData/authorize/true").Wait();
                    SendRequest(httpClient, HttpMethod.Get, "default/getData/authorize/false").Wait();
                    SendRequest(httpClient, HttpMethod.Get, "default/getData/authorize/false/forbidden").Wait();
                    SendRequest(httpClient, HttpMethod.Post, "default/sendData", new Data { Integer = 1 }).Wait();
                    SendRequest(httpClient, HttpMethod.Post, "default/sendData/authorize/true", new Data { Integer = 2 }).Wait();
                    SendRequest(httpClient, HttpMethod.Post, "default/sendData/authorize/false", new Data { Integer = 3 }).Wait();
                    SendRequest(httpClient, HttpMethod.Post, "default/sendData/authorize/false/forbidden", new Data { Integer = 4 }).Wait();
                }

                Console.WriteLine("Press any key to quit.");
                Console.ReadKey();
            }
        }

        private async static Task SendRequest(HttpClient httpClient, HttpMethod httpMethod, string targetUri, object data = null)
        {
            using (HttpRequestMessage getDataRequest = new HttpRequestMessage(httpMethod, targetUri))
            {
                if (data != null)
                {
                    getDataRequest.Content = new ObjectContent(data.GetType(), data, new MediaTypeFormatterCollection().JsonFormatter);

                    Console.WriteLine($"Sending '{httpMethod}' with payload to: {targetUri}");
                }
                else
                {
                    Console.WriteLine($"Sending '{httpMethod}' to: {targetUri}");
                }

                try
                {
                    var getDataResponse = await httpClient.SendAsync(getDataRequest);

                    if (getDataResponse.IsSuccessStatusCode)
                    {
                        var serverResponse = await getDataResponse.Content.ReadAsAsync<ServerResponse>();

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Successfully received response with message: {serverResponse.Message}.");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"Failed with HttpStatusCode: {getDataResponse.StatusCode}.");
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Client throw an exception: {ex.GetBaseException().Message}");
                }
            }

            Console.ResetColor();
            Console.WriteLine();
        }
    }
}
