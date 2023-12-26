using MyPrivateManager.Models;
namespace MyPrivateManager.IDatabaseServices;

public interface ITaskWorkServices
{
    Task<IEnumerable<TaskWork>> GetTaskWorkAsync();
    Task<TaskWork?> GetTaskWorkByIdAsync(int? taskWorkId);
    Task<bool> CreateTaskWorkAsync(TaskWork taskWork);
    Task<bool> UpdateTaskWorkAsync(int taskWorkId, TaskWork taskWork);
    Task<bool> DeleteTaskWorkAsync(int taskWorkId);
}