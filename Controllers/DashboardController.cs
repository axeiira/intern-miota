using Microsoft.AspNetCore.Mvc;
using TryConsoleApp.Services;
using TryConsoleApp.Models;

namespace TryConsoleApp.Controllers
{
    public class DashboardController : Controller
    {
        private readonly DatabaseService _databaseService;

        public DashboardController(DatabaseService databaseService)
        {
            _databaseService = databaseService;
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
                var data = await _databaseService.GetLatestDataAsync();
                
                if (data != null)
                {
                    return Json(data);
                }
                else
                {
                    return Json(new { 
                        message = "No data available from database", 
                        status = "no_data",
                        timestamp = DateTime.UtcNow 
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new { 
                    message = $"Database error: {ex.Message}", 
                    status = "error",
                    timestamp = DateTime.UtcNow 
                });
            }
        }
    }
}
