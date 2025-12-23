using Microsoft.AspNetCore.Mvc;
using MyPrivateManager.IDatabaseServices;
using MyPrivateManager.Models;

namespace MyPrivateManager.Controllers;

public class TechnicianController : Controller
{
    private readonly ITechnicianServices _technicianServices;
    private readonly ILogger<TechnicianController> _logger;

    public TechnicianController(ITechnicianServices services, ILogger<TechnicianController> logger)
    {
        _technicianServices = services;
        _logger = logger;
    }

    [HttpGet("/Technician/Technicians")]
    public async Task<IActionResult> GetTechnicians()
    {
        try
        {
            var technicians = await _technicianServices.GetTechniciansAsync();
            return Ok(technicians);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving technicians");
            return View("Error");
        }
    }

    [HttpGet("/Technician/GetTechnician/{technicianId}")]
    public async Task<IActionResult> GetTechnician(int technicianId)
    {
        try
        {
            var technician = await _technicianServices.GetTechnicianByIdAsync(technicianId);
            if (technician == null)
            {
                _logger.LogError("Technician not found");
                return NotFound();
            }
            _logger.LogInformation("Successfully retrieved technician data");
            return Ok(technician);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving technician");
            return View("Error");
        }
    }

    [HttpPost("/Technician/CreateTechnician")]
    public async Task<IActionResult> CreateTechnician(Technician technician)
    {
        try
        {
            var success = await _technicianServices.CreateTechnicianAsync(technician);
            _logger.LogInformation("Successfully created technician");
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating technician");
            return View("Error");
        }
    }

    [HttpPost("/Technician/UpdateTechnician/{technicianId}")]
    public async Task<IActionResult> UpdateTechnician(int technicianId, Technician technician)
    {
        try
        {
            await _technicianServices.UpdateTechnicianAsync(technicianId, technician);
            _logger.LogInformation("Successfully updated technician");
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating technician");
            return View("Error");
        }
    }

    [HttpDelete("/Technician/DeleteTechnician/{technicianId}")]
    public async Task<ActionResult> DeleteTechnician(int technicianId)
    {
        try
        {
            await _technicianServices.DeleteTechnicianAsync(technicianId);
            _logger.LogInformation("Successfully deleted technician");
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting technician");
            return View("Error");
        }
    }
}
