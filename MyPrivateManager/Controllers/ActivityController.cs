using Microsoft.AspNetCore.Mvc;
using MyPrivateManager.DatabaseServices;
using MyPrivateManager.Models;

namespace MyPrivateManager.Controllers;

public class ActivityController : Controller
{
    private readonly IActivityServices _activityServices;

    public ActivityController(IActivityServices activityServices)
    {
        _activityServices = activityServices;
    }

    public async Task<IActionResult> Index()
    {
        var activities = await _activityServices.GetActivitiesAsync();
        var topActivities = await _activityServices.GetTopActivitiesAsync(5);
        
        ViewBag.TopActivities = topActivities;
        
        return View(activities);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Activity activity)
    {
        if (ModelState.IsValid)
        {
            await _activityServices.CreateActivityAsync(activity);
            return RedirectToAction(nameof(Index));
        }
        
        // If invalid, reload the page with errors (and data)
        var activities = await _activityServices.GetActivitiesAsync();
        var topActivities = await _activityServices.GetTopActivitiesAsync(5);
        ViewBag.TopActivities = topActivities;
        return View("Index", activities);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        await _activityServices.DeleteActivityAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
