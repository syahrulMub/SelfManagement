using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyPrivateManager.IDatabaseServices;

namespace MyPrivateManager.Controllers;

[Authorize]
public class LogViewerController : Controller
{
    private readonly ILogFileService _logFileService;
    private readonly ILogger<LogViewerController> _logger;

    public LogViewerController(ILogFileService logFileService, ILogger<LogViewerController> logger)
    {
        _logFileService = logFileService;
        _logger = logger;
    }

    /// <summary>
    /// Main log viewer page
    /// </summary>
    public IActionResult Index()
    {
        _logger.LogInformation("Log viewer accessed");
        return View();
    }

    /// <summary>
    /// Get list of available log files
    /// </summary>
    [HttpGet("/logserver/files")]
    public async Task<IActionResult> GetLogFiles()
    {
        try
        {
            var files = await _logFileService.GetLogFilesAsync();
            return Ok(files);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving log files");
            return StatusCode(500, new { error = "Failed to retrieve log files" });
        }
    }

    /// <summary>
    /// Get content of a specific log file
    /// </summary>
    [HttpGet("/logserver/content")]
    public async Task<IActionResult> GetLogContent(
        [FromQuery] string fileName,
        [FromQuery] string? level = null,
        [FromQuery] string? search = null)
    {
        try
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return BadRequest(new { error = "File name is required" });
            }

            var entries = await _logFileService.ReadLogFileAsync(fileName, level, search);
            return Ok(entries);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading log content for file: {FileName}", fileName);
            return StatusCode(500, new { error = "Failed to read log content" });
        }
    }

    /// <summary>
    /// Get logs by date range
    /// </summary>
    [HttpGet("/logserver/daterange")]
    public async Task<IActionResult> GetLogsByDateRange(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        [FromQuery] string? level = null)
    {
        try
        {
            var entries = await _logFileService.GetLogsByDateRangeAsync(startDate, endDate, level);
            return Ok(entries);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving logs by date range");
            return StatusCode(500, new { error = "Failed to retrieve logs" });
        }
    }
}
