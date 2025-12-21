using System.ComponentModel.DataAnnotations;

namespace MyPrivateManager.Models;

public class Rating
{

    public int Id { get; set; }
    public Guid OrderId { get; set; }
    public Order Order { get; set; } = default!;
    public int Score { get; set; } // 1..5
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

}


