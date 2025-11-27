using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyPrivateManager.IDatabaseServices;
using MyPrivateManager.Models;

namespace MyPrivateManager.Controllers;

public class TaskCategoryController : Controller
{
    private readonly ITaskCategoryServices _taskCategoryServices;
    private readonly ILogger<TaskCategoryController> _logger;
    private readonly UserManager<User> _userManagaer;

    public TaskCategoryController(ITaskCategoryServices taskCategoryServices, ILogger<TaskCategoryController> logger, UserManager<User> userManager)
    {
        _taskCategoryServices = taskCategoryServices;
        _logger = logger;
        _userManagaer = userManager;
    }
    [HttpGet("/TaskCategory/GetCategories")]
    public async Task<IActionResult> GetTaskCategories()
    {
        try
        {
            var userId = _userManagaer.GetUserId(User);
            if (userId != null)
            {
                var taskCategories = await _taskCategoryServices.GetTaskCategoriesAsync();
                var userTask = taskCategories.Where(i => i.UserId == userId);
                _logger.LogInformation("success get categories");
                return PartialView("_TaskCategoryList", userTask);
            }
            else
            {
                _logger.LogError("user not found");
                return View("Error");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("error get categories" + ex.Message);
            return View("Error");
        }
    }

    [HttpGet("/TaskCategory/GetCategoriesJson")]
    public async Task<IActionResult> GetTaskCategoriesJson()
    {
        try
        {
            var userId = _userManagaer.GetUserId(User);
            if (userId != null)
            {
                var taskCategories = await _taskCategoryServices.GetTaskCategoriesAsync();
                var userTask = taskCategories.Where(i => i.UserId == userId)
                                             .Select(i => new { taskCategoryId = i.TaskCategoryId, taskCategoryName = i.TaskCategoryName });
                return Ok(userTask);
            }
            return BadRequest();
        }
        catch (Exception ex)
        {
            _logger.LogError("error get categories json" + ex.Message);
            return BadRequest();
        }
    }
    [HttpGet("/TaskCategory/GetTaskCategory/{taskCategoryId}")]
    public async Task<IActionResult> GetTaskCategory(int taskCategoryId)
    {
        try
        {
            var taskCategory = await _taskCategoryServices.GetTaskCategoryByIdAsync(taskCategoryId);
            _logger.LogInformation("success get category ");
            return Ok(taskCategory);
        }
        catch (Exception ex)
        {
            _logger.LogError("error getting task category " + ex.Message);
            return View("Error");
        }
    }
    [HttpPost("/TaskCategory/Create")]
    public async Task<IActionResult> CreateTaskCategory(TaskCategory taskCategory)
    {
        try
        {
            var userId = _userManagaer.GetUserId(User);
            if (userId != null)
            {
                taskCategory.UserId = userId;
                await _taskCategoryServices.CreateTaskCategoryAsync(taskCategory);
                _logger.LogInformation("Success Create Task Category");
                return Ok();
            }
            else
            {
                _logger.LogError("UserId not found");
                return View("Error");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Error creating task category" + ex.Message);
            return View("Error");
        }
    }
    [HttpPost("/TaskCategory/Update/{taskCategoryId}")]
    public async Task<IActionResult> UpdateTaskCategory(int taskCategoryId, TaskCategory taskCategory)
    {
        try
        {
            await _taskCategoryServices.UpdateTaskCategoryAsync(taskCategoryId, taskCategory);
            _logger.LogInformation("success update task category");
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError("Error updating task category" + ex.Message);
            return View("Error");
        }
    }
    [HttpDelete("/TaskCategory/Delete/{taskCategoryId}")]
    public async Task<IActionResult> DeleteTaskCategory(int taskCategoryId)
    {
        try
        {
            await _taskCategoryServices.DeleteTaskCategoryAsync(taskCategoryId);
            _logger.LogInformation("Success delete task Category");
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError("Error deleting task category" + ex.Message);
            return View("Error");
        }
    }
}
