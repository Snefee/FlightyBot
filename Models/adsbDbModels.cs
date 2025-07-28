using Newtonsoft.Json;

namespace FlightyBot.Models
{
    public class AdsbDbApiResponse
    {
        [JsonProperty("response")]
        public AdsbDbResponse Response { get; set; }
    }

    public class AdsbDbResponse
    {
        [JsonProperty("aircraft")]
        public AdsbDbAircraftData Aircraft { get; set; }
    }

    public class AdsbDbAircraftData
    {
        [JsonProperty("registration")]
        public string Registration { get; set; }

        [JsonProperty("manufacturer")]
        public string Manufacturer { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("icao_type")]
        public string IcaoType { get; set; }

        [JsonProperty("url_photo")]
        public string PhotoUrl { get; set; }

        [JsonProperty("registered_owner_country_name")]
        public string OwnerCountryName { get; set; }

        [JsonProperty("registered_owner")]
        public string RegisteredOwner { get; set; }

    }
}