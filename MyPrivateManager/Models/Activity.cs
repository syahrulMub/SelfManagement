using System.ComponentModel.DataAnnotations;

namespace MyPrivateManager.Models;

public class Activity
{
    [Key]
    public int ActivityId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = null!;

    [Required]
    public DateTime Date { get; set; }

    [Range(1, 5)]
    public int Rating { get; set; }

    public int? Duration { get; set; } // In minutes

    [MaxLength(500)]
    public string? Description { get; set; }
}
