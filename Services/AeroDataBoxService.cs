using FlightyBot.Models;
using Newtonsoft.Json;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace FlightyBot.Services
{
    public class AeroDataBoxService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private const string ApiBaseUrl = "https://prod.api.market/api/v1/aedbx/aerodatabox/flights/Number/";

        public AeroDataBoxService(string apiKey)
        {
            _apiKey = apiKey;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("x-magicapi-key", _apiKey);
        }

        public async Task<AeroDataBoxFlight> GetFlightDataAsync(string flightNumber)
        {
            var requestUrl = $"{ApiBaseUrl}{flightNumber}?withLocation=true";

            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(requestUrl);
                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    var flights = JsonConvert.DeserializeObject<List<AeroDataBoxFlight>>(jsonResponse);
                    return flights?.FirstOrDefault();
                }
                return null;
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }
    }
}