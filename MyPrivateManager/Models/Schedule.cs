using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyPrivateManager.Models;

public class Schedule
{
    [Key]
    public int ScheduleId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    [ForeignKey("TaskWokId")]
    public int TaskWorkId { get; set; }
    public virtual TaskWork TaskWork { get; set; } = null!;
}
