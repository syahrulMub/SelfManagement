using MyPrivateManager.Models;
namespace MyPrivateManager.IDatabaseServices;

public interface ITaskCategoryServices
{
    Task<IEnumerable<TaskCategory>> GetTaskCategoriesAsync();
    Task<TaskCategory?> GetTaskCategoryByIdAsync(int? taskCategoryId);
    Task<bool> CreateTaskCategoryAsync(TaskCategory taskCategory);
    Task<bool> UpdateTaskCategoryAsync(int taskCategoryId, TaskCategory taskCategory);
    Task<bool> DeleteTaskCategoryAsync(int taskCategoryId);
}