using MyPrivateManager.Models;

namespace MyPrivateManager.DatabaseServices;

public interface IActivityServices
{
    Task<List<Activity>> GetActivitiesAsync();
    Task<Activity?> GetActivityByIdAsync(int id);
    Task CreateActivityAsync(Activity activity);
    Task UpdateActivityAsync(Activity activity);
    Task DeleteActivityAsync(int id);
    Task<List<Activity>> GetTopActivitiesAsync(int count);
}
