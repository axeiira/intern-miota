using TryConsoleApp.Models;
using System.Text.Json;

namespace TryConsoleApp.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        public ApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiBaseUrl = configuration["Api:BaseUrl"] ?? "http://192.168.17.211:5000";
        }

        public async Task<FMC650Data?> GetLatestDataAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/telemetry/latest?limit=1");
                
                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    var dataArray = JsonSerializer.Deserialize<FMC650Data[]>(jsonContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    
                    return dataArray?.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"API error: {ex.Message}");
            }

            return null;
        }

        public async Task<FMC650Data[]?> GetLatestDataAsync(int limit = 5)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/telemetry/latest?limit={limit}");
                
                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    var dataArray = JsonSerializer.Deserialize<FMC650Data[]>(jsonContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    
                    return dataArray;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"API error: {ex.Message}");
            }

            return null;
        }
        
        public async Task<FMC650Data[]?> GetTelemetryDataByTimeRangeAsync(DateTime startTime, DateTime endTime)
        {
            try
            {
                // Ensure we're working with the correct timezone - treat input as local time
                var startTimeFormatted = DateTime.SpecifyKind(startTime, DateTimeKind.Local).ToString("yyyy-MM-ddTHH:mm:ss");
                var endTimeFormatted = DateTime.SpecifyKind(endTime, DateTimeKind.Local).ToString("yyyy-MM-ddTHH:mm:ss");
                
                // URL encode the time strings to handle special characters
                var encodedStartTime = Uri.EscapeDataString(startTimeFormatted);
                var encodedEndTime = Uri.EscapeDataString(endTimeFormatted);
                
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/telemetry/timerange?startTime={encodedStartTime}&endTime={encodedEndTime}");
                
                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    var dataArray = JsonSerializer.Deserialize<FMC650Data[]>(jsonContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    
                    return dataArray;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"API error: {ex.Message}");
            }

            return null;
        }
    }
}
