using System.ComponentModel.DataAnnotations;

namespace MyPrivateManager.Models;

public class Customer
{
    [Key]
    public int CustomerId { get; set; }

    public string UserId { get; set; }
    public User User { get; set; }
    public string? Latitude { get; set; }
    public string? Longitude { get; set; }
    public string Address { get; set; }
    public string City { get; set; }

    public ICollection<Order> Orders { get; set; }
}
