using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication13.Models;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class LogController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public LogController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("filter")]
    public IActionResult GetFilteredData(
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        [FromQuery] string? groupBy)
    {
        try
        {
            var query = _context.Logs.AsQueryable();

            if (startDate.HasValue)
            {
                var utcStartDate = DateTime.SpecifyKind(startDate.Value, DateTimeKind.Utc);
                query = query.Where(log => log.Date >= utcStartDate);
            }

            if (endDate.HasValue)
            {
                var utcEndDate = DateTime.SpecifyKind(endDate.Value, DateTimeKind.Utc);
                query = query.Where(log => log.Date <= utcEndDate);
            }

            if (!string.IsNullOrEmpty(groupBy))
            {
                if (groupBy.ToLower() == "ip")
                {
                    var groupedByIp = query
                        .GroupBy(log => log.IP)
                        .Select(g => new { IP = g.Key, Count = g.Count() })
                        .ToList();
                    return Ok(groupedByIp);
                }
                else if (groupBy.ToLower() == "date")
                {
                    var groupedByDate = query
                        .GroupBy(log => log.Date.Date)
                        .Select(g => new { Date = g.Key, Count = g.Count() })
                        .ToList();
                    return Ok(groupedByDate);
                }
                else
                {
                    return BadRequest(new { message = "Invalid groupBy value. Use 'ip' or 'date'." });
                }
            }

            var result = query.ToList();
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
        }
    }
}
