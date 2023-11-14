using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;


namespace MyPrivateManager.Models;

public class User : IdentityUser
{
    [Required]
    [MaxLength(50)]
    public string? UserJob { get; set; }

    [Required]
    [Column(TypeName = "decimal(18, 2)")]
    public decimal Balance { get; set; }
}
