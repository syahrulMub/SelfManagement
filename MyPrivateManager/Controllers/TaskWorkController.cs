using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyPrivateManager.IDatabaseServices;
using MyPrivateManager.Models;

namespace MyPrivateManager.Controllers;
public class TaskWorkController : Controller
{
    private readonly ITaskWorkServices _taskWorkServices;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<TaskWorkController> _logger;
    public TaskWorkController(ITaskWorkServices taskWorkServices, UserManager<User> userManager, ILogger<TaskWorkController> logger)
    {
        _taskWorkServices = taskWorkServices;
        _logger = logger;
        _userManager = userManager;
    }
    public async Task<IActionResult> Index()
    {
        try
        {
            var userId = _userManager.GetUserId(User);
            if (userId != null)
            {
                var userTask = await _taskWorkServices.GetTaskWorksGroupedByCategory(userId);
                ViewBag.UserTask = userTask;
                return View();
            }
            else
            {
                _logger.LogError("failed to get userId");
                return View("Error");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Error getting task Work " + ex.Message);
            return View("Error");
        }
    }
    [HttpPost("/TaskWork/Create")]
    public async Task<IActionResult> CreateTaskWork(TaskWork taskwork)
    {
        try
        {
            await _taskWorkServices.CreateTaskWorkAsync(taskwork);
            _logger.LogInformation("Success create task work");
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError("error create task work" + ex.Message);
            return View("Error");
        }
    }
    [HttpPost("/TaskWork/Update/{taskWorkId}")]
    public async Task<IActionResult> UpdateTaskWork(int taskWorkId, TaskWork taskWork)
    {
        try
        {
            await _taskWorkServices.UpdateTaskWorkAsync(taskWorkId, taskWork);
            _logger.LogInformation("success update task work");
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError("error update task work" + ex.Message);
            return View("Error");
        }
    }
    [HttpDelete("/TaskWork/bulkDeleteByTaskCategory/{taskCategoryId}")]
    public async Task<IActionResult> BulkDeleteByTaskCategory(int taskCategoryId)
    {
        try
        {
            await _taskWorkServices.BulkDeleteTaskWorkByCategory(taskCategoryId);
            _logger.LogInformation("success bulk delete");
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError("Error bulk delete" + ex.Message);
            return View("Error");
        }
    }
    [HttpDelete("/TaskWork/Delete/{taskWorkId}")]
    public async Task<IActionResult> DeleteTaskWork(int taskWorkId)
    {
        try
        {
            await _taskWorkServices.DeleteTaskWorkAsync(taskWorkId);
            _logger.LogInformation("Success delete taskwork");
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError("Error while deleting taskwork", ex.Message);
            return View("Error");
        }
    }
}