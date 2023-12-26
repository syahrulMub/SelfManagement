using MyPrivateManager.Models;
namespace MyPrivateManager.IDatabaseServices;

public interface IScheduleServices
{
    Task<IEnumerable<Schedule>> GetScheduleAsync();
    Task<Schedule?> GetScheduleByIdAsync(int? scheduleId);
    Task<bool> CreateScheduleAsync(Schedule schedule);
    Task<bool> UpdateScheduleAsync(int scheduleId, Schedule schedule);
    Task<bool> DeleteScheduleAsync(int scheduleId);
}