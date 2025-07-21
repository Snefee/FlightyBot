using FlightyBot.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace FlightyBot.Services
{
    public class AdsbDbService
    {
        private readonly HttpClient _httpClient;
        private const string ApiAircraftUrl = "https://api.adsbdb.com/v0/aircraft/";
        private const string ApiCallsignUrl = "https://api.adsbdb.com/v0/callsign/";

        public AdsbDbService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<AdsbDbAircraftData> GetAircraftDataAsync(string icao24)
        {
            if (string.IsNullOrEmpty(icao24)) return null;

            var requestUrl = $"{ApiAircraftUrl}{icao24}";
            return await FetchAndParseAircraftData(requestUrl);
        }

        public async Task<AdsbDbAircraftData> GetAircraftDataByCallsignAsync(string callsign)
        {
            if (string.IsNullOrEmpty(callsign)) return null;

            var requestUrl = $"{ApiCallsignUrl}{callsign}";
            return await FetchAndParseAircraftData(requestUrl);
        }

        public async Task<AdsbDbAircraftData> GetAircraftDataByRegistrationAsync(string registration)
        {
            if (string.IsNullOrEmpty(registration)) return null;

            var requestUrl = $"{ApiAircraftUrl}{registration}";
            return await FetchAndParseAircraftData(requestUrl);
        }

        private async Task<AdsbDbAircraftData> FetchAndParseAircraftData(string url)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    var adsbDbResponse = JsonConvert.DeserializeObject<AdsbDbResponse>(jsonResponse);
                    return adsbDbResponse?.Aircraft;
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