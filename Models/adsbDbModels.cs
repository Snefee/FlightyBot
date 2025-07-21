using Newtonsoft.Json;

namespace FlightyBot.Models
{
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
    }
}