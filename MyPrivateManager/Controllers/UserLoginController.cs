using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyPrivateManager.IDatabaseServices;
using MyPrivateManager.Models;

namespace MyPrivateManager.Controllers;

public class UserLoginController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly IUserManager _myUserManager;
    private readonly ILogger<UserLoginController> _logger;
    public UserLoginController(IUserManager myUserManager, ILogger<UserLoginController> logger, UserManager<User> userManager)
    {
        _userManager = userManager;
        _myUserManager = myUserManager;
        _logger = logger;
    }
    [AllowAnonymous]
    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
    {
        try
        {
            var user = new User
            {
                Email = model.Email,
                UserName = model.UserName,
                UserJob = model.UserJob,

            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "user");
                return Ok(new { message = "Registration successful" });
            }
            else
            {
                var errors = result.Errors.Select(e => e.Description);
                return BadRequest(new { message = "Registration failed", errors = errors });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("error while process registered : " + ex.Message);
            return View("error while process registered : " + ex.Message);
        }
    }
    [AllowAnonymous]
    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginViewModel model)
    {
        try
        {
            var result = await _myUserManager.LoginAsync(model);
            if (result != null)
            {
                return Ok();
            }
            else
            {
                return Unauthorized();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("error while processing LogIn : " + ex.Message);
            return View("error while processing LogIn : " + ex.Message);
        }
    }
}
