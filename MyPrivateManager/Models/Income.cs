using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyPrivateManager.Models;

public class Income
{
    [Key]
    public int IncomeId { get; set; }

    [Required]
    [Column(TypeName = "decimal(18, 2)")]
    public decimal Amount { get; set; }

    [Required]
    [ForeignKey("SourceId")]
    public int SourceId { get; set; }

    public virtual Source Source { get; set; } = null!;

    [Required]
    public DateTime Date { get; set; }

    [MaxLength(255)]
    public string? Description { get; set; }
}
