using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyPrivateManager.Models;

public class TaskWork
{
    [Key]
    public int TaskWorkId { get; set; }
    public string? Description { get; set; }
    public DateTime DueDate { get; set; }
    public TaskPriority TaskPriority { get; set; }
    public TaskStage TaskStage { get; set; }
    [ForeignKey("TaskCategoryId")]
    public int TaskCategoryId { get; set; }
    public virtual TaskCategory TaskCategory { get; set; } = null!;
    public virtual ICollection<Schedule>? Schedules { get; set; }
}
