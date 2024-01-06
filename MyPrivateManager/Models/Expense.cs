using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyPrivateManager.Models;

public class Expense
{
    [Key]
    public int ExpenseId { get; set; }

    [Required]
    public int Amount { get; set; }

    [Required]
    [ForeignKey("CategoryId")]
    public int CategoryId { get; set; }

    public virtual Category Category { get; set; } = null!;

    [Required]
    public DateTime Date { get; set; }

    [MaxLength(255)]
    public string? Description { get; set; }
}
