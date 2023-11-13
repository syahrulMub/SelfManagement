using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyPrivateManager.Models;

public class Income
{
    [Key]
    public int IncomeId { get; set; }

    [Required]
    public decimal Amount { get; set; }

    [Required]
    public int SourceId { get; set; }

    [ForeignKey("SourceId")]
    public virtual Source Source { get; set; } = null!;

    [Required]
    public DateTime Date { get; set; }

    [MaxLength(255)]
    public string? Description { get; set; }

    [Required]
    public string UserId { get; set; } = null!;

    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;
}
