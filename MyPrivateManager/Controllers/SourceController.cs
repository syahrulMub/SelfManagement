using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyPrivateManager.IDatabaseServices;
using MyPrivateManager.Models;

namespace MyPrivateManager.Controllers;

public class SourceController : Controller
{
    private readonly ISourceServices _services;
    private readonly ILogger<SourceController> _logger;
    private readonly UserManager<User> _userManager;
    public SourceController(ISourceServices services, ILogger<SourceController> logger, UserManager<User> userManager)
    {
        _services = services;
        _logger = logger;
        _userManager = userManager;
    }

    [HttpGet("/Source/GetSources")]
    public async Task<IActionResult> GetSources()
    {
        try
        {
            var sources = await _services.GetSourcesAsync();
            _logger.LogInformation("success get list of source");
            return Ok(sources);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving sources");
            return View("Error");
        }
    }

    [HttpGet("Source/GetSource/{sourceId}")]
    public async Task<IActionResult> GetSource(int sourceId)
    {
        try
        {
            var source = await _services.GetSourceByIdAsync(sourceId);

            if (source == null)
            {
                _logger.LogError("item not found");
                return NotFound();
            }
            _logger.LogInformation("success get source");
            return Ok(source);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving source");
            return View("Error");
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateSource(Source source)
    {
        try
        {
            var userId = _userManager.GetUserId(User);
            if (userId != null)
            {
                source.UserId = userId;
                await _services.CreateSourceAsync(source);
                return Ok();
            }
            else
            {
                return View("Error");
            }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating source");
            return View("Internal Server Error");
        }
    }

    [HttpPost("/Source/UpdateSource/{sourceId}")]
    public async Task<IActionResult> UpdateSource(int sourceId, Source source)
    {
        try
        {

            var result = await _services.UpdateSourceAsync(sourceId, source);
            if (result)
            {
                _logger.LogInformation("success update source");
                return Ok();
            }
            else
            {
                _logger.LogError("error while update source");
                return View("Error");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating source");
            return View("Error");
        }
    }

    [HttpDelete("/Source/DeleteSource/{sourceId}")]
    public async Task<IActionResult> DeleteSource(int sourceId)
    {
        try
        {
            await _services.DeleteSourceAsync(sourceId);
            _logger.LogInformation("success delete source");
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting source");
            return View("Error");
        }
    }
}