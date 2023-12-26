using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyPrivateManager.Data;
using MyPrivateManager.IDatabaseServices;
using MyPrivateManager.Models;

namespace DatabaseServices
{
    public class TaskWorkServices : ITaskWorkServices
    {
        Task<bool> ITaskWorkServices.CreateTaskWorkAsync(TaskWork taskWork)
        {
            throw new NotImplementedException();
        }

        Task<bool> ITaskWorkServices.DeleteTaskWorkAsync(int taskWorkId)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<TaskWork>> ITaskWorkServices.GetTaskWorkAsync()
        {
            throw new NotImplementedException();
        }

        Task<TaskWork?> ITaskWorkServices.GetTaskWorkByIdAsync(int? taskWorkId)
        {
            throw new NotImplementedException();
        }

        Task<bool> ITaskWorkServices.UpdateTaskWorkAsync(int taskWorkId, TaskWork taskWork)
        {
            throw new NotImplementedException();
        }
    }
}
