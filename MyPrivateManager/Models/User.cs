using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;


namespace MyPrivateManager.Models;

public class User : IdentityUser
{
    [Required]
    [MaxLength(50)]
    public string? UserJob { get; set; }

    [Required]
    public decimal Balance { get; set; }
}
