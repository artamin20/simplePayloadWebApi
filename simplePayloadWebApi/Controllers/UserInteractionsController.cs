using Microsoft.AspNetCore.Mvc;
using simplePayloadWebApi.Data;
using simplePayloadWebApi.Models;
using Microsoft.Extensions.Logging;

namespace simplePayloadWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserTrackController : Controller
    {
        private readonly simplePayloadContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<UserTrackController> _logger;
        private const int BatchSize = 250;

        public UserTrackController(
            simplePayloadContext context,
            IHttpContextAccessor httpContextAccessor,
            ILogger<UserTrackController> logger)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        [HttpPost("bulk")]
        public async Task<IActionResult> CreateBulk([FromBody] List<UserInteraction> interactions)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state in CreateBulk.");
                return BadRequest(ModelState);
            }

            if (interactions == null || interactions.Count == 0)
            {
                _logger.LogWarning("No interactions received in CreateBulk.");
                return BadRequest("No data received.");
            }

            _logger.LogInformation("Received {Count} interactions.", interactions.Count);

            // enrich records
            var enriched = interactions.Select(i =>
            {
                i.UserAgent ??= _httpContextAccessor.HttpContext?.Request.Headers["User-Agent"];
                i.Device = ParseDevice(i.UserAgent!);
                i.OS = ParseOS(i.UserAgent!);
                i.Timestamp = DateTime.UtcNow;
                return i;
            }).ToList();

            _logger.LogInformation("Enriched {Count} interactions.", enriched.Count);

            try
            {
                for (int i = 0; i < enriched.Count; i += BatchSize)
                {
                    var batch = enriched.Skip(i).Take(BatchSize).ToList();
                    await _context.UserInteraction.AddRangeAsync(batch);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Saved batch of {BatchSize} interactions.", batch.Count);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while saving interactions.");
                return StatusCode(500, "An error occurred while saving data.");
            }

            _logger.LogInformation("Successfully processed {Count} interactions.", enriched.Count);
            return Ok(new { Count = enriched.Count });
        }

        private string ParseDevice(string ua)
        {
            if (ua?.Contains("Mobile") == true) return "Mobile";
            if (ua?.Contains("Tablet") == true) return "Tablet";
            return "Desktop";
        }

        private string ParseOS(string ua)
        {
            if (ua?.Contains("Windows") == true) return "Windows";
            if (ua?.Contains("Mac OS") == true) return "Mac";
            if (ua?.Contains("Linux") == true) return "Linux";
            if (ua?.Contains("Android") == true) return "Android";
            if (ua?.Contains("iPhone") == true) return "iOS";
            return "Unknown";
        }

        [HttpGet("/UserTrack/SendInteractions")]
        public IActionResult SendInteractions()
        {
            _logger.LogInformation("Rendering SendInteractions view.");
            return View();
        }
    }
}
