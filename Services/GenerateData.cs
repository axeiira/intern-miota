using TryConsoleApp.Models;

namespace TryConsoleApp.Services
{
    public class FMC650Generator
    {
        private readonly Random _random = new();
        private double _currentLat = 40.7128; // NYC coordinates
        private double _currentLon = -74.0060;

        public FMC650Data GenerateData()
        {
            // Simulate movement
            _currentLat += (_random.NextDouble() - 0.5) * 0.001;
            _currentLon += (_random.NextDouble() - 0.5) * 0.001;

            return new FMC650Data
            {
                Timestamp = DateTime.UtcNow,
                Latitude = _currentLat,
                Longitude = _currentLon,
                Speed = _random.Next(0, 120),
                Heading = _random.Next(0, 360),
                Fuel = _random.NextDouble() * 100,
                EngineHours = _random.Next(1000, 5000),
                IgnitionStatus = _random.NextDouble() > 0.3
            };
        }
    }
}