using Microsoft.EntityFrameworkCore;
using MyPrivateManager.Data;
using MyPrivateManager.IDatabaseServices;
using MyPrivateManager.Models;

namespace DatabaseServices
{
    public class TaskCategoryServices : ITaskCategoryServices
    {
        private readonly DatabaseContext _dbContext;
        public TaskCategoryServices(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<IEnumerable<TaskCategory>> GetTaskCategoriesAsync()
        {
            var taskCategories = await _dbContext.TaskCategories
                                .ToListAsync();
            return taskCategories;
        }
        public async Task<TaskCategory?> GetTaskCategoryByIdAsync(int? taskCategoryId)
        {
            var taskCategory = await _dbContext.TaskCategories
                                .Where(i => i.TaskCategoryId == taskCategoryId)
                                .FirstOrDefaultAsync();
            return taskCategory;
        }
        public async Task<bool> CreateTaskCategoryAsync(TaskCategory taskCategory)
        {
            if (taskCategory != null)
            {
                _dbContext.TaskCategories.Add(taskCategory);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<bool> UpdateTaskCategoryAsync(int taskCategoryId, TaskCategory taskCategory)
        {
            var currentTaskCategory = await _dbContext.TaskCategories
                                    .Where(i => i.TaskCategoryId == taskCategoryId)
                                    .FirstOrDefaultAsync();
            if (currentTaskCategory != null)
            {
                currentTaskCategory.TaskCategoryName = taskCategory.TaskCategoryName;

                _dbContext.TaskCategories.Update(currentTaskCategory);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<bool> DeleteTaskCategoryAsync(int taskCategoryId)
        {
            var currentTaskCategory = await _dbContext.TaskCategories
                                    .Where(i => i.TaskCategoryId == taskCategoryId)
                                    .FirstOrDefaultAsync();
            if (currentTaskCategory != null)
            {
                _dbContext.TaskCategories.Remove(currentTaskCategory);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
