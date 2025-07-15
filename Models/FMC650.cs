using System.Text.Json.Serialization;

namespace TryConsoleApp.Models
{
    public class FMC650Data
    {
        [JsonPropertyName("deviceId")]
        public string DeviceId { get; set; } = "FMC650_001";
        
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }
        
        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }
        
        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }
        
        [JsonPropertyName("speed")]
        public int Speed { get; set; }
        
        [JsonPropertyName("heading")]
        public int Heading { get; set; }
        
        [JsonPropertyName("fuel")]
        public double Fuel { get; set; }
        
        [JsonPropertyName("engineHours")]
        public int EngineHours { get; set; }
        
        [JsonPropertyName("ignitionStatus")]
        public bool IgnitionStatus { get; set; }
    }
}