using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyPrivateManager.Models;

namespace MyPrivateManager.Controllers;

[Authorize(Roles = "admin")]
public class UserManagementController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ILogger<UserManagementController> _logger;

    public UserManagementController(
        UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager,
        ILogger<UserManagementController> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
    }

    // ===== VIEW =====

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    // ===== DATA ENDPOINTS =====

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

    // ===== CRUD OPERATIONS =====

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
}

public class UpdateUserRoleRequest
{
    public string UserId { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new List<string>();
}
