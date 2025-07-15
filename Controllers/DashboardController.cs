using Microsoft.AspNetCore.Mvc;
using TryConsoleApp.Services;
using TryConsoleApp.Models;

namespace TryConsoleApp.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApiService _apiService;

        public DashboardController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> GetData()
        {
            try
            {
                var data = await _apiService.GetLatestDataAsync();
                
                if (data != null)
                {
                    return Json(data);
                }
                else
                {
                    return Json(new { 
                        message = "No data available from API", 
                        status = "no_data",
                        timestamp = DateTime.UtcNow 
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new { 
                    message = $"API error: {ex.Message}", 
                    status = "error",
                    timestamp = DateTime.UtcNow 
                });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetMultipleData(int limit = 5)
        {
            try
            {
                var data = await _apiService.GetLatestDataAsync(limit);
                
                if (data != null && data.Length > 0)
                {
                    return Json(data);
                }
                else
                {
                    return Json(new { 
                        message = "No data available from API", 
                        status = "no_data",
                        timestamp = DateTime.UtcNow 
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new { 
                    message = $"API error: {ex.Message}", 
                    status = "error",
                    timestamp = DateTime.UtcNow 
                });
            }
        }
    }
}
