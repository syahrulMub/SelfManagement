using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MyPrivateManager.Controllers;

/// <summary>
/// Developer Tools Controller - Provides easy access to development/admin tools
/// </summary>
[Authorize]
[Route("[controller]")]
public class DevToolsController : Controller
{
    private readonly ILogger<DevToolsController> _logger;

    public DevToolsController(ILogger<DevToolsController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Dev Tools Dashboard - Main entry point for developer tools
    /// </summary>
    public IActionResult Index()
    {
        if (!User.IsInRole("admin"))
        {
            _logger.LogWarning("Non-admin user attempted to access Dev Tools");
            return Forbid();
        }

        _logger.LogInformation("Dev Tools Dashboard accessed by user: {UserName}", User.Identity?.Name);
        return View();
    }

    /// <summary>
    /// Quick redirect to Log Viewer
    /// </summary>
    [Route("logs")]
    public IActionResult Logs()
    {
        if (!User.IsInRole("admin"))
        {
            return Forbid();
        }

        return RedirectToAction("Index", "LogViewer");
    }

    /// <summary>
    /// Quick redirect to Backup & Restore
    /// </summary>
    [Route("backup")]
    public IActionResult Backup()
    {
        if (!User.IsInRole("admin"))
        {
            return Forbid();
        }

        return RedirectToAction("Index", "Backup");
    }
}
