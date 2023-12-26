using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyPrivateManager.Models;

public class TaskCategory
{
    [Key]
    [Required]
    public int TaskCategoryId { get; set; }
    public string TaskCategoryName { get; set; } = null!;
    [Required]
    [ForeignKey("UserId")]
    public string? UserId { get; set; }
    public virtual User User { get; set; } = null!;
}
