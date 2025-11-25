using MyPrivateManager.Models;
namespace MyPrivateManager.IDatabaseServices;

public interface ITaskWorkServices
{
    Task<IEnumerable<TaskWork>> GetTaskWorkAsync();
    Task<IEnumerable<TaskWork>> GetTaskWorkByTaskCategory(int taskCategoryId);
    Task<TaskWork?> GetTaskWorkByIdAsync(int? taskWorkId);
    Task<List<DTOTaskWorkForIndex>> GetTaskWorksGroupedByCategory(string userId);
    Task<bool> CreateTaskWorkAsync(TaskWork taskWork);
    Task<bool> UpdateTaskWorkAsync(int taskWorkId, TaskWork taskWork);
    Task<bool> DeleteTaskWorkAsync(int taskWorkId);
    Task<bool> BulkDeleteTaskWorkByCategory(int taskCategoryId);
}

public class DTOTaskWorkForIndex
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public List<TaskWork>? TaskWorks { get; set; }
}