using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyPrivateManager.Models;

public class Category
{
    [Key]
    public int CategoryId { get; set; }

    [Required]
    [MaxLength(50)]
    public string CategoryName { get; set; } = null!;
    [Required]
    [ForeignKey("UserId")]
    public string UserId { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}