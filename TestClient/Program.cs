using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;

namespace TestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Test client for Ocelot, IdentityServer4, and an internal ASP.NET Core 2.1 API");
            Console.WriteLine(Environment.NewLine);

            MainAsync().GetAwaiter().GetResult();
        }

        private static async Task MainAsync()
        {
            try
            {
                Console.WriteLine("Calling IdentityServer4 discovery endpoint...");
                Console.WriteLine(Environment.NewLine);

                var discoveryUrl = "https://localhost:44351";
                var clientId = "WidgetClient";
                var clientSecret = "p@ssw0rd";
                var discoveryClient = new DiscoveryClient(discoveryUrl);
                var discoveryResponse = await discoveryClient.GetAsync();
                if (discoveryResponse.IsError)
                {
                    throw new Exception("Failed to get discovery response!");
                }

                Console.WriteLine("Calling IdentityServer4 authorize endpoint to get request token...");
                Console.WriteLine(Environment.NewLine);

                var scope = "widgetapi";
                var tokenClient = new TokenClient(discoveryResponse.TokenEndpoint, clientId, clientSecret);
                var tokenResponse = await tokenClient.RequestClientCredentialsAsync(scope);
                if (tokenResponse.IsError)
                {
                    throw new Exception("Authentication failed!");
                }

                Console.WriteLine("Calling Ocelot endpoint with bearer token to get widgets...");
                Console.WriteLine(Environment.NewLine);

                var uri = "https://localhost:44371/api/v1/widget";
                var gatewayClient = new HttpClient();
                gatewayClient.SetBearerToken(tokenResponse.AccessToken);
                var response = await gatewayClient.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Received HTTP 200 from API. Writing widgets to console...");
                    Console.WriteLine(Environment.NewLine);

                    var content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(content);
                }
                else
                {
                    Console.WriteLine("Response was unsuccessful with status code: " + response.StatusCode);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(Environment.NewLine);
            }

            Console.WriteLine("Press ENTER to quit.");
            Console.ReadLine();
        }
    }
}
