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
    }
}
