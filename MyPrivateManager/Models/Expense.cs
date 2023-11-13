using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyPrivateManager.Models;

public class Expense
{
    [Key]
    public int ExpenseId { get; set; }

    [Required]
    public decimal Amount { get; set; }

    [Required]
    public int CategoryId { get; set; }

    [ForeignKey("CategoryId")]
    public virtual Category Category { get; set; } = null!;

    [Required]
    public DateTime Date { get; set; }

    [MaxLength(255)]
    public string? Description { get; set; }

    [Required]
    public string UserId { get; set; } = null!;

    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;
}
