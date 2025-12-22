using System.ComponentModel.DataAnnotations;

namespace MyPrivateManager.Models;

public class Order
{
    public int OrderId { get; set; }
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = default!;
    public int TechnicianId { get; set; } 
    public Technician? Technician { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ScheduledAt { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Created;
    public decimal Price { get; set; }
    public string? PaymentStatus { get; set; }
    public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
}
