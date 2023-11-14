using Microsoft.AspNetCore.Mvc;
using MyPrivateManager.IDatabaseServices;
using MyPrivateManager.Models;

namespace MyPrivateManager.Controllers;

public class SourceController : Controller
{
    private readonly ISourceServices _services;
    private readonly ILogger<SourceController> _logger;
    public SourceController(ISourceServices services, ILogger<SourceController> logger)
    {
        _services = services;
        _logger = logger;
    }

    [HttpGet("/sources")]
    public async Task<IActionResult> GetSources()
    {
        try
        {
            var sources = await _services.GetSourcesAsync();
            return Ok(sources);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving sources");
            return View("Error");
        }
    }

    [HttpGet("source/{sourceId}")]
    public async Task<IActionResult> GetSource(int sourceId)
    {
        try
        {
            var source = await _services.GetSourceByIdAsync(sourceId);

            if (source == null)
            {
                return NotFound();
            }

            return Ok(source);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving source");
            return View("Error");
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateSource([FromBody] Source source)
    {
        try
        {
            // Validate input
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid input for CreateSource");
                return BadRequest(ModelState);
            }

            if (await _services.CreateSourceAsync(source))
            {
                return Ok();
            }
            else
            {
                _logger.LogError("Failed to create source");
                return View("Failed to create source");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating source");
            return View("Internal Server Error");
        }
    }

    [HttpPut("{sourceId}")]
    public async Task<IActionResult> UpdateSource(int sourceId, [FromBody] Source source)
    {
        try
        {
            // Validate input
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid input for UpdateSource");
                return BadRequest(ModelState);
            }

            var success = await _services.UpdateSourceAsync(sourceId, source);

            if (success)
            {
                return Ok(source);
            }
            else
            {
                _logger.LogError($"Source with id {sourceId} not found");
                return NotFound($"Source with id {sourceId} not found");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating source");
            return View("Internal Server Error");
        }
    }

    [HttpDelete("{sourceId}")]
    public async Task<IActionResult> DeleteSource(int sourceId)
    {
        try
        {
            var success = await _services.DeleteSourceAsync(sourceId);

            if (success)
            {
                return NoContent();
            }
            else
            {
                _logger.LogError($"Source with id {sourceId} not found");
                return NotFound($"Source with id {sourceId} not found");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting source");
            return View("Internal Server Error");
        }
    }
}
