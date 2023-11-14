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
            _logger.LogError(ex, "Error retrieving expenses");
            return View("Error");
        }
    }
    [HttpGet]
    public async Task<IActionResult> Details(int expenseId)
    {
        try
        {
            var expense = await _services.GetExpenseByIdAsync(expenseId);

            if (expense == null)
            {
                return NotFound();
            }
            return View(expense);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving expense");
            return View("Error");
        }
    }
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Amount,CategoryId,Date,Description")] Expense expense)
    {
        try
        {
            // Validate input
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid input for CreateExpense");
                return View(expense);
            }
            var userId = _userManager.GetUserId(User);
            if (userId != null)
            {
                expense.UserId = userId;
                expense.Date = DateTime.Now;
                await _services.CreateExpenseAsync(expense);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                _logger.LogError("user not found");
                return View("User not found");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating expense");
            return View("Error");
        }
    }
    public async Task<IActionResult> GetTotalExpensesByCategory(int categoryId)
    {
        try
        {
            var userId = _userManager.GetUserId(User);
            if (userId != null)
            {
                var totalExpenses = await _services.GetTotalExpensesByCategoryAsync(categoryId, userId);
                return Ok(totalExpenses);
            }
            else
            {
                _logger.LogWarning("User ID is null when retrieving total expenses by category");
                return BadRequest("User ID is null");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving total expenses by category");
            return View("Error");
        }
    }
    public async Task<IActionResult> GetExpensesByDateRange(DateTime startDate, DateTime endDate)
    {
        try
        {
            var userId = _userManager.GetUserId(User);
            if (userId != null)
            {
                var expenses = await _services.GetExpensesByDateRangeAsync(startDate, endDate);
                expenses = expenses.Where(e => e.UserId == userId);
                return Ok(expenses);
            }
            else
            {
                _logger.LogWarning("User ID is null when retrieving expenses by date range");
                return BadRequest("User ID is null");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving expenses by date range");
            return View("Error");
        }
    }
}
