using Microsoft.AspNetCore.Mvc;
using GenericCalcLogViewer.Models;
using GenericCalcLogViewer.Services;

namespace GenericCalcLogViewer.Controllers;

/// <summary>
/// בקר Web API המטפל בבקשות HTTP לחיפוש לוגים
/// </summary>
[ApiController]
[Route("api/logs")]
public class LogsController : ControllerBase
{
    private readonly ILogSearchService _searchService;
    private readonly ILogger<LogsController> _logger;

    public LogsController(ILogSearchService searchService, ILogger<LogsController> logger)
    {
        _searchService = searchService;
        _logger = logger;
    }

    /// <summary>
    /// מבצע חיפוש לוגים לפי מספר תיק וסביבה
    /// </summary>
    /// <param name="request">בקשת חיפוש</param>
    /// <returns>תגובת חיפוש עם לוגים</returns>
    [HttpPost("search")]
    public async Task<ActionResult<SearchLogsResponse>> Search([FromBody] SearchLogsRequest request)
    {
        // Validation
        if (!ValidateRequest(request, out var validationError))
        {
            return BadRequest(new { error = validationError });
        }

        try
        {
            var response = await _searchService.SearchLogsAsync(request);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching logs for case {CaseNumber} in {Environment}", 
                request.CaseNumber, request.Environment);
            return StatusCode(500, new { error = "Internal server error occurred while searching logs" });
        }
    }

    private bool ValidateRequest(SearchLogsRequest request, out string? error)
    {
        error = null;

        if (request.CaseNumber <= 0)
        {
            error = "CaseNumber must be greater than zero";
            return false;
        }

        var validEnvironments = new[] { "DEV", "TEST", "INT", "PROD" };
        if (!validEnvironments.Contains(request.Environment?.ToUpper()))
        {
            error = "Environment must be one of: DEV, TEST, INT, PROD";
            return false;
        }

        if (request.FromDate.HasValue && request.ToDate.HasValue && 
            request.FromDate.Value >= request.ToDate.Value)
        {
            error = "FromDate must be earlier than ToDate";
            return false;
        }

        return true;
    }
}
