using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyPrivateManager.IDatabaseServices;
using MyPrivateManager.Models;

namespace MyPrivateManager.Controllers;

public class ExpenseController : Controller
{
    private readonly IExpenseServices _services;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<ExpenseController> _logger;

    public ExpenseController(IExpenseServices services, UserManager<User> userManager, ILogger<ExpenseController> logger)
    {
        _services = services;
        _userManager = userManager;
        _logger = logger;
    }
    [HttpGet]
    public ActionResult<IEnumerable<Expense>> GetExpensesAsync()
    {
        try
        {
            var userId = _userManager.GetUserId(User);
            var result = _services.GetExpenses().Where(i => i.UserId == userId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return StatusCode(500, "Expenses not found");
        }
    }
}
