using System.ComponentModel.DataAnnotations;

namespace MyPrivateManager.Models;

public class Source
{
    [Key]
    public int SourceId { get; set; }

    [Required]
    [MaxLength(50)]
    public string SourceName { get; set; } = null!;
}
