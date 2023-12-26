using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyPrivateManager.Models;

public class Source
{
    [Key]
    public int SourceId { get; set; }

    [Required]
    [MaxLength(50)]
    public string SourceName { get; set; } = null!;
    [Required]
    [ForeignKey("UserId")]
    public string UserId { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}
