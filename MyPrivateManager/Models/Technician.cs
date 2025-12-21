using System.ComponentModel.DataAnnotations;

namespace MyPrivateManager.Models;

public class Technician
{
    public int Id { get; set; }
    public string FullName { get; set; } = default!;
    public string Phone { get; set; } = default!;
    public bool IsActive { get; set; } = true;
    public double AvgRating { get; set; } // denormalisasi
    public int CompletedJobs { get; set; }
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}


