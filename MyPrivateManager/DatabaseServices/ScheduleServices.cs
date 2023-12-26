using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyPrivateManager.Data;
using MyPrivateManager.IDatabaseServices;
using MyPrivateManager.Models;

namespace DatabaseServices
{
    public class ScheduleServices : IScheduleServices
    {
        Task<bool> IScheduleServices.CreateScheduleAsync(Schedule schedule)
        {
            throw new NotImplementedException();
        }

        Task<bool> IScheduleServices.DeleteScheduleAsync(int scheduleId)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<Schedule>> IScheduleServices.GetScheduleAsync()
        {
            throw new NotImplementedException();
        }

        Task<Schedule?> IScheduleServices.GetScheduleByIdAsync(int? scheduleId)
        {
            throw new NotImplementedException();
        }

        Task<bool> IScheduleServices.UpdateScheduleAsync(int scheduleId, Schedule schedule)
        {
            throw new NotImplementedException();
        }
    }
}
