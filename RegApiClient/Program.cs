using IdentityModel.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace RegApiClient
{
    class Program
    {
        public static string acc_token { get; set; }
        static async Task Main(string[] args)
        {
            // discover endpoints from metadata
            var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = "https://psl-webapps/eiauth",
                Policy =
                {
                    RequireHttps = false
                }
            });

            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                return;
            }

            // request token
            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {

                Address = disco.TokenEndpoint,
                ClientId = "integration-client1",
                ClientSecret = "secret1",

                Scope = "ei-api"
            });

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return;
            }

            Console.WriteLine(tokenResponse.Json);
            Console.WriteLine("\n\n");

            //Call API
            acc_token = tokenResponse.AccessToken;

            var brvTracker = new List<BrvTracker>()
            {
                new BrvTracker()
                {
                    AssetName = "CR8049-12",
                EventType = "Loading",
                Latitude = 5.70846,
                Longitude = 0.027332,
                OrderNumber = "ORD -123-125",
                ReportTime = "1650623718",
                Sensor1LevelPercentage = 25.2,
                Sensor2LevelPercentage = 25.3,
                Sensor3LevelPercentage = 25.4,
                Sensor4LevelPercentage = 25.5,
                Sensor5LevelPercentage = 25.6,
                Sensor6LevelPercentage = 25.7
                }
            };
            try
            {
                var response = await PostBrvTracker(brvTracker);
                if (response != null)
                {
                    
                    Console.WriteLine(JsonConvert.SerializeObject(response, Formatting.Indented));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }
        private static async Task<List<BrvTrackerResponse>> PostBrvTracker(List<BrvTracker> brvTracker)
        {
            try
            {
                var apiClient = new HttpClient();
                apiClient.SetBearerToken(acc_token);

                var json = JsonConvert.SerializeObject(brvTracker);
                HttpContent httpContent = new StringContent(json);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var result = await apiClient.PostAsync("https://psl-webapps/eiapi/api/BrvTrackers", httpContent);
                var stringContent = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<BrvTrackerResponse>>(stringContent);

            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    internal class BrvTracker
    {
        public string AssetName { get; set; }
        public string OrderNumber { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string ReportTime { get; set; }
        public string EventType { get; set; }
        public double Sensor1LevelPercentage { get; set; }
        public double Sensor2LevelPercentage { get; set; }
        public double Sensor3LevelPercentage { get; set; }
        public double Sensor4LevelPercentage { get; set; }
        public double? Sensor5LevelPercentage { get; set; }
        public double? Sensor6LevelPercentage { get; set; }
    }
    internal class BrvTrackerResponse
    {
        public Guid Id { get; set; }
        public string AssetName { get; set; }
        public string OrderNumber { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string ReportTime { get; set; }
        public string EventType { get; set; }
        public double Sensor1LevelPercentage { get; set; }
        public double Sensor2LevelPercentage { get; set; }
        public double Sensor3LevelPercentage { get; set; }
        public double Sensor4LevelPercentage { get; set; }
        public double? Sensor5LevelPercentage { get; set; }
        public double? Sensor6LevelPercentage { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
