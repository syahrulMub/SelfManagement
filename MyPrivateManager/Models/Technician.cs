using System.ComponentModel.DataAnnotations;

namespace MyPrivateManager.Models;

public class Technician
{
    public int TechnicianId { get; set; }
    public string UserId { get; set; }
    public User User { get; set; }
    public bool IsActive { get; set; } = true;
    public double AvgRating { get; set; } // denormalisasi
    public int CompletedJobs { get; set; }
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}


