using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyPrivateManager.Data;
using MyPrivateManager.IDatabaseServices;
using MyPrivateManager.Models;

namespace DatabaseServices
{
    public class TaskCategoryServices : ITaskCategoryServices
    {
        Task<bool> ITaskCategoryServices.CreateTaskCategoryAsync(TaskCategory taskCategory)
        {
            throw new NotImplementedException();
        }

        Task<bool> ITaskCategoryServices.DeleteTaskCategoryAsync(int taskCategoryId)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<TaskCategory>> ITaskCategoryServices.GetTaskCategoriesAsync()
        {
            throw new NotImplementedException();
        }

        Task<TaskCategory?> ITaskCategoryServices.GetTaskCategoryByIdAsync(int? taskCategoryId)
        {
            throw new NotImplementedException();
        }

        Task<bool> ITaskCategoryServices.UpdateTaskCategoryAsync(int taskCategoryId, TaskCategory taskCategory)
        {
            throw new NotImplementedException();
        }
    }
}
