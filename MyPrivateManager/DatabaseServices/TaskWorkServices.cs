using Microsoft.EntityFrameworkCore;
using MyPrivateManager.Data;
using MyPrivateManager.IDatabaseServices;
using MyPrivateManager.Models;

namespace DatabaseServices
{
    public class TaskWorkServices : ITaskWorkServices
    {
        private readonly DatabaseContext _dbContext;
        public TaskWorkServices(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<IEnumerable<TaskWork>> GetTaskWorkAsync()
        {
            var taskWork = await _dbContext.TaskWorks
                            .Include(i => i.TaskCategory)
                            .ToListAsync();
            return taskWork;
        }
        public async Task<IEnumerable<TaskWork>> GetTaskWorkByTaskCategory(int taskCategoryId)
        {
            var taskWork = await _dbContext.TaskWorks
                            .Where(i => i.TaskCategoryId == taskCategoryId)
                            .Include(i => i.TaskCategory)
                            .ToListAsync();
            return taskWork;
        }
        public async Task<TaskWork?> GetTaskWorkByIdAsync(int? taskWorkId)
        {
            var taskWork = await _dbContext.TaskWorks
                            .Where(i => i.TaskWorkId == taskWorkId)
                            .Include(i => i.TaskCategory)
                            .FirstOrDefaultAsync();
            return taskWork;
        }
        public async Task<List<DTOTaskWorkForIndex>> GetTaskWorksGroupedByCategory(string userId)
        {
            var taskCategoriesWithTaskWorks = await _dbContext.TaskWorks
                .Where(tw => tw.TaskCategory.UserId == userId)
                .GroupBy(tw => tw.TaskCategory)
                .Select(group => new DTOTaskWorkForIndex
                {
                    Id = group.Key.TaskCategoryId,
                    Name = group.Key.TaskCategoryName,
                    TaskWorks = group.ToList()
                })
                .ToListAsync();

            return taskCategoriesWithTaskWorks;
        }
        public async Task<bool> CreateTaskWorkAsync(TaskWork taskWork)
        {
            if (taskWork != null)
            {
                _dbContext.TaskWorks.Add(taskWork);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<bool> UpdateTaskWorkAsync(int taskWorkId, TaskWork taskWork)
        {
            var currentTaskWork = await _dbContext.TaskWorks
                                .Where(i => i.TaskWorkId == taskWorkId)
                                .FirstOrDefaultAsync();
            if (currentTaskWork != null)
            {
                currentTaskWork.TaskPriority = taskWork.TaskPriority;
                currentTaskWork.TaskStage = taskWork.TaskStage;
                currentTaskWork.DueDate = taskWork.DueDate;
                currentTaskWork.Description = taskWork.Description;

                _dbContext.TaskWorks.Update(currentTaskWork);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<bool> DeleteTaskWorkAsync(int taskWorkId)
        {
            var taskWork = await _dbContext.TaskWorks
                            .Where(i => i.TaskWorkId == taskWorkId)
                            .FirstOrDefaultAsync();
            if (taskWork != null)
            {
                _dbContext.TaskWorks.Remove(taskWork);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<bool> BulkDeleteTaskWorkByCategory(int taskCategoryId)
        {
            var workTask = _dbContext.TaskWorks
                        .Where(i => i.TaskCategoryId == taskCategoryId)
                        .AsEnumerable();
            if (workTask != null)
            {
                foreach (var task in workTask)
                {
                    _dbContext.TaskWorks.Remove(task);
                }
                await _dbContext.SaveChangesAsync();
                return true;
            }
            else
            {
                return true;
            }
        }

    }
}
