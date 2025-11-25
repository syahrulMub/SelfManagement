using Microsoft.EntityFrameworkCore;
using MyPrivateManager.Data;
using MyPrivateManager.Models;

namespace MyPrivateManager.DatabaseServices;

public class ActivityServices : IActivityServices
{
    private readonly DatabaseContext _context;

    public ActivityServices(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<List<Activity>> GetActivitiesAsync()
    {
        return await _context.Activities
            .OrderByDescending(a => a.Date)
            .ToListAsync();
    }

    public async Task<Activity?> GetActivityByIdAsync(int id)
    {
        return await _context.Activities.FindAsync(id);
    }

    public async Task CreateActivityAsync(Activity activity)
    {
        _context.Activities.Add(activity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateActivityAsync(Activity activity)
    {
        _context.Activities.Update(activity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteActivityAsync(int id)
    {
        var activity = await _context.Activities.FindAsync(id);
        if (activity != null)
        {
            _context.Activities.Remove(activity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<Activity>> GetTopActivitiesAsync(int count)
    {
        return await _context.Activities
            .OrderByDescending(a => a.Rating)
            .ThenByDescending(a => a.Date)
            .Take(count)
            .ToListAsync();
    }
}
