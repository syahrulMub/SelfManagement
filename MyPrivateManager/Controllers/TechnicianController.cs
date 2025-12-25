using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyPrivateManager.DTOs;
using MyPrivateManager.IDatabaseServices;
using MyPrivateManager.Models;

namespace MyPrivateManager.Controllers;

public class TechnicianController : Controller
{
    private readonly ITechnicianServices _technicianServices;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<TechnicianController> _logger;

    public TechnicianController(
        ITechnicianServices services,
        UserManager<User> userManager,
        ILogger<TechnicianController> logger)
    {
        _technicianServices = services;
        _userManager = userManager;
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

    // ===== ADMIN MASTER DATA MANAGEMENT =====

    [Authorize(Roles = "admin")]
    [HttpGet("/Admin/Technicians")]
    public IActionResult MasterData()
    {
        return View("~/Views/MasterData/Technicians.cshtml");
    }

    [Authorize(Roles = "admin")]
    [HttpGet("/Admin/Technicians/GetData")]
    public async Task<IActionResult> GetTechniciansData()
    {
        try
        {
            var technicians = await _technicianServices.GetTechniciansAsync();
            var technicianDataList = technicians.Select(t => new
            {
                technicianId = t.TechnicianId,
                fullName = t.User?.UserName ?? "N/A",
                phone = t.User?.PhoneNumber ?? "N/A",
                isActive = t.IsActive,
                avgRating = t.AvgRating,
                completedJobs = t.CompletedJobs,
                orderCount = t.Orders?.Count ?? 0
            });

            return Json(new { data = technicianDataList });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving technicians data");
            return StatusCode(500, new { error = "Error retrieving technicians data" });
        }
    }

    [Authorize(Roles = "admin")]
    [HttpGet("/Admin/Technicians/Get/{technicianId}")]
    public async Task<IActionResult> GetTechnicianById(int technicianId)
    {
        try
        {
            var technician = await _technicianServices.GetTechnicianByIdAsync(technicianId);
            if (technician == null)
            {
                return NotFound(new { error = "Technician not found" });
            }

            return Json(new
            {
                technicianId = technician.TechnicianId,
                userId = technician.UserId,
                fullName = technician.User?.UserName ?? "N/A",
                phone = technician.User?.PhoneNumber ?? "N/A",
                isActive = technician.IsActive,
                avgRating = technician.AvgRating,
                completedJobs = technician.CompletedJobs
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving technician");
            return StatusCode(500, new { error = "Error retrieving technician" });
        }
    }

    [Authorize(Roles = "admin")]
    [HttpPost("/Admin/Technicians/Create")]
    public async Task<IActionResult> CreateTechnicianAdmin([FromBody] CreateTechnicianDto dto)
    {
        try
        {
            // Get the user
            var user = await _userManager.FindByIdAsync(dto.UserId);
            if (user == null)
            {
                return NotFound(new { error = "User not found" });
            }

            // Update user phone if provided
            if (!string.IsNullOrEmpty(dto.PhoneNumber))
            {
                user.PhoneNumber = dto.PhoneNumber;
                await _userManager.UpdateAsync(user);
            }

            // Check if technician already exists for this user
            var technicians = await _technicianServices.GetTechniciansAsync();
            if (technicians.Any(t => t.UserId == dto.UserId))
            {
                return BadRequest(new { error = "Technician profile already exists for this user" });
            }

            // Create technician
            var technician = new Technician
            {
                UserId = dto.UserId,
                IsActive = dto.IsActive,
                AvgRating = 0,
                CompletedJobs = 0
            };

            await _technicianServices.CreateTechnicianAsync(technician);
            _logger.LogInformation($"Successfully created technician for user {user.Email}");
            return Ok(new { message = "Technician created successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating technician");
            return StatusCode(500, new { error = "Error creating technician" });
        }
    }

    [Authorize(Roles = "admin")]
    [HttpPost("/Admin/Technicians/Update/{technicianId}")]
    public async Task<IActionResult> UpdateTechnicianAdmin(int technicianId, [FromBody] UpdateTechnicianDto dto)
    {
        try
        {
            // Get existing technician
            var technician = await _technicianServices.GetTechnicianByIdAsync(technicianId);
            if (technician == null)
            {
                return NotFound(new { error = "Technician not found" });
            }

            // Update user phone if provided
            if (!string.IsNullOrEmpty(dto.PhoneNumber))
            {
                var user = await _userManager.FindByIdAsync(technician.UserId);
                if (user != null)
                {
                    user.PhoneNumber = dto.PhoneNumber;
                    await _userManager.UpdateAsync(user);
                }
            }

            // Update technician profile
            technician.IsActive = dto.IsActive;

            var success = await _technicianServices.UpdateTechnicianAsync(technicianId, technician);
            if (!success)
            {
                return NotFound(new { error = "Technician not found" });
            }

            _logger.LogInformation($"Successfully updated technician {technicianId}");
            return Ok(new { message = "Technician updated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating technician");
            return StatusCode(500, new { error = "Error updating technician" });
        }
    }

    [Authorize(Roles = "admin")]
    [HttpDelete("/Admin/Technicians/Delete/{technicianId}")]
    public async Task<IActionResult> DeleteTechnicianAdmin(int technicianId)
    {
        try
        {
            var success = await _technicianServices.DeleteTechnicianAsync(technicianId);
            if (!success)
            {
                return NotFound(new { error = "Technician not found" });
            }
            _logger.LogInformation("Successfully deleted technician (admin)");
            return Ok(new { message = "Technician deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting technician");
            return StatusCode(500, new { error = "Error deleting technician" });
        }
    }

    [Authorize(Roles = "admin")]
    [HttpGet("/Admin/Technicians/GetAvailableUsers")]
    public async Task<IActionResult> GetAvailableUsers()
    {
        try
        {
            // Get all users
            var users = _userManager.Users.ToList();
            
            // Get users already assigned as technicians
            var technicians = await _technicianServices.GetTechniciansAsync();
            var assignedUserIds = technicians.Select(t => t.UserId).ToHashSet();
            
            // Filter out assigned users
            var availableUsers = users
                .Where(u => !assignedUserIds.Contains(u.Id))
                .Select(u => new
                {
                    id = u.Id,
                    email = u.Email,
                    userName = u.UserName,
                    phoneNumber = u.PhoneNumber
                })
                .ToList();
            
            return Json(new { data = availableUsers });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving available users");
            return StatusCode(500, new { error = "Error retrieving available users" });
        }
    }
}
