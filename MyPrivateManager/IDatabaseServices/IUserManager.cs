using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace MyPrivateManager.IDatabaseServices;

public interface IUserManager
{
    public Task<bool> RegisterUserAsync(RegisterViewModel model);
    public Task<SignInResult> LoginAsync(LoginViewModel model);
}

public class LoginViewModel
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}

public class RegisterViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;
    [Required]
    public string UserName { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string? UserJob { get; internal set; }

}