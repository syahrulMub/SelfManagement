using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyPrivateManager.IDatabaseServices;
using MyPrivateManager.Models;

namespace MyPrivateManager.Controllers;

[Authorize(Roles = "admin")]
public class MasterDataController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ICustomerServices _customerServices;
    private readonly ITechnicianServices _technicianServices;
    private readonly ILogger<MasterDataController> _logger;

    public MasterDataController(
        UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager,
        ICustomerServices customerServices,
        ITechnicianServices technicianServices,
        ILogger<MasterDataController> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _customerServices = customerServices;
        _technicianServices = technicianServices;
        _logger = logger;
    }

    // ===== MASTER USER MANAGEMENT =====

    [HttpGet]
    public IActionResult Users()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetUsersData()
    {
        try
        {
            var users = await _userManager.Users.ToListAsync();
            var userDataList = new List<object>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userDataList.Add(new
                {
                    id = user.Id,
                    email = user.Email,
                    userName = user.UserName,
                    roles = roles
                });
            }

            return Json(new { data = userDataList });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users data");
            return StatusCode(500, new { error = "Error retrieving users data" });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetUser(string userId)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new { error = "User not found" });
            }

            var roles = await _userManager.GetRolesAsync(user);
            return Json(new
            {
                id = user.Id,
                email = user.Email,
                userName = user.UserName,
                roles = roles
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user");
            return StatusCode(500, new { error = "Error retrieving user" });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAvailableRoles()
    {
        try
        {
            var roles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
            return Json(new { data = roles });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving roles");
            return StatusCode(500, new { error = "Error retrieving roles" });
        }
    }

    [HttpPost]
    public async Task<IActionResult> UpdateUserRole([FromBody] UpdateUserRoleRequest request)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
            {
                return NotFound(new { error = "User not found" });
            }

            // Get current roles
            var currentRoles = await _userManager.GetRolesAsync(user);

            // Remove all current roles
            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
            {
                return BadRequest(new { error = "Failed to remove current roles" });
            }

            // Add new roles
            if (request.Roles != null && request.Roles.Any())
            {
                var addResult = await _userManager.AddToRolesAsync(user, request.Roles);
                if (!addResult.Succeeded)
                {
                    return BadRequest(new { error = "Failed to add new roles" });
                }
            }

            _logger.LogInformation($"Successfully updated roles for user {user.Email}");
            return Ok(new { message = "User roles updated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user roles");
            return StatusCode(500, new { error = "Error updating user roles" });
        }
    }

    // ===== MASTER CUSTOMER MANAGEMENT =====

    [HttpGet]
    public IActionResult Customers()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetCustomersData()
    {
        try
        {
            var customers = await _customerServices.GetCustomersAsync();
            var customerDataList = customers.Select(c => new
            {
                customerId = c.CustomerId,
                userId = c.UserId,
                userEmail = c.User?.Email ?? "N/A",
                address = c.Address,
                city = c.City,
                latitude = c.Latitude,
                longitude = c.Longitude,
                orderCount = c.Orders?.Count ?? 0
            });

            return Json(new { data = customerDataList });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving customers data");
            return StatusCode(500, new { error = "Error retrieving customers data" });
        }
    }

    // ===== MASTER TECHNICIAN MANAGEMENT =====

    [HttpGet]
    public IActionResult Technicians()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetTechniciansData()
    {
        try
        {
            var technicians = await _technicianServices.GetTechniciansAsync();
            var technicianDataList = technicians.Select(t => new
            {
                technicianId = t.TechnicianId,
                fullName = t.FullName,
                phone = t.Phone,
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
}

public class UpdateUserRoleRequest
{
    public string UserId { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new List<string>();
}
