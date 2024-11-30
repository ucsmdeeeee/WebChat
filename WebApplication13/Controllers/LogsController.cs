using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication13.Services;

namespace WebApplication13.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class LogsController : ControllerBase
    {
        private readonly ILogProcessor _logProcessor;

        public LogsController(ILogProcessor logProcessor)
        {
            _logProcessor = logProcessor;
        }

        [HttpPost("process")]
        public IActionResult ProcessLogs([FromBody] LogProcessRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.FilePath))
            {
                return BadRequest(new { message = "The filePath field is required." });
            }

            try
            {
                _logProcessor.ProcessLogFile(request.FilePath);
                return Ok(new { message = "Log processing completed successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", details = ex.Message });
            }
        }

        public class LogProcessRequest
        {
            public string FilePath { get; set; }
        }
    }

}
