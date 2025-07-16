using Microsoft.AspNetCore.Mvc;
using TryConsoleApp.Services;

namespace TryConsoleApp.Controllers
{
    public class FeaturesController : Controller
    {
        private readonly ApiService _apiService;

        public FeaturesController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
    
    [ApiController]
    [Route("api/telemetry")]
    public class TelemetryApiController : ControllerBase
    {
        private readonly ApiService _apiService;

        public TelemetryApiController(ApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet("timerange")]
        public async Task<IActionResult> GetTimeRangeData([FromQuery] string startTime, [FromQuery] string endTime)
        {
            try
            {
                // Parse datetime strings as local time to avoid timezone conversion issues
                if (!DateTime.TryParse(startTime, out DateTime parsedStartTime) || 
                    !DateTime.TryParse(endTime, out DateTime parsedEndTime))
                {
                    return BadRequest(new { message = "Invalid datetime format. Use yyyy-MM-ddTHH:mm:ss format." });
                }
                
                // Ensure the parsed times are treated as local time
                parsedStartTime = DateTime.SpecifyKind(parsedStartTime, DateTimeKind.Local);
                parsedEndTime = DateTime.SpecifyKind(parsedEndTime, DateTimeKind.Local);
                
                var data = await _apiService.GetTelemetryDataByTimeRangeAsync(parsedStartTime, parsedEndTime);
                
                if (data == null)
                {
                    return StatusCode(500, new { message = "Failed to fetch data from external API" });
                }
                
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching data", error = ex.Message });
            }
        }
    }
}
