using Newtonsoft.Json;
using System;

namespace FlightyBot.Models
{
    public class AeroDataBoxFlight
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("departure")]
        public FlightEndpoint Departure { get; set; }

        [JsonProperty("arrival")]
        public FlightEndpoint Arrival { get; set; }

        [JsonProperty("airline")]
        public AirlineInfo Airline { get; set; }

        [JsonProperty("number")]
        public string Number { get; set; }

        [JsonProperty("aircraft")]
        public AircraftInfo Aircraft { get; set; }

        [JsonProperty("location")]
        public LocationInfo Location { get; set; }

        [JsonProperty("greatCircleDistance")]
        public GreatCircleDistanceInfo GreatCircleDistance { get; set; }
    }

    public class FlightEndpoint
    {
        [JsonProperty("airport")]
        public AirportInfo Airport { get; set; }

        [JsonProperty("scheduledTime")]
        public TimeInfo ScheduledTime { get; set; }

        [JsonProperty("predictedTime")]
        public TimeInfo PredictedTime { get; set; }
    }

    public class AirportInfo
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("iata")]
        public string Iata { get; set; }

        [JsonProperty("countryCode")]
        public string CountryCode { get; set; }

        [JsonProperty("location")]
        public LocationInfo Location { get; set; }
    }

    public class TimeInfo
    {
        [JsonProperty("local")]
        public DateTime? Local { get; set; }
    }

    public class AirlineInfo
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class AircraftInfo
    {
        [JsonProperty("reg")]
        public string Registration { get; set; }

        [JsonProperty("model")]
        public string Model { get; set; }
    }

    public class LocationInfo
    {
        [JsonProperty("lat")]
        public double Latitude { get; set; }
        [JsonProperty("lon")]
        public double Longitude { get; set; }
    }

    public class GreatCircleDistanceInfo
    {
        [JsonProperty("km")]
        public double Km { get; set; }
    }
}