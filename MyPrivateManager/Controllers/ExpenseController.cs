using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyPrivateManager.IDatabaseServices;
using MyPrivateManager.Models;

namespace MyPrivateManager.Controllers;

public class ExpenseController : Controller
{
    private readonly IExpenseServices _expenseServices;
    private readonly ICategoryServices _categoryServices;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<ExpenseController> _logger;

    public ExpenseController(IExpenseServices expenseServices, ICategoryServices categoryServices, UserManager<User> userManager, ILogger<ExpenseController> logger)
    {
        _expenseServices = expenseServices;
        _categoryServices = categoryServices;
        _userManager = userManager;
        _logger = logger;
    }
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        try
        {
            var userId = _userManager.GetUserId(User);
            var expense = await _expenseServices.GetExpenses();
            var userExpenses = expense.Where(i => i.UserId == userId);
            var category = await _categoryServices.GetCategoriesAsync();
            ViewBag.UserExpenses = userExpenses;
            ViewBag.Category = category;
            return View();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving expenses");
            return View("Error");
        }
    }
    [HttpGet("/Expense/GetExpense/{expenseId}")]
    public async Task<IActionResult> Details(int expenseId)
    {
        try
        {
            var expense = await _expenseServices.GetExpenseByIdAsync(expenseId);

            if (expense == null)
            {
                return NotFound();
            }
            return Ok(expense);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving expense");
            return View("Error");
        }
    }

    [HttpPost("/Expense/CreateExpense")]
    public async Task<IActionResult> Create(Expense expense)
    {
        try
        {
            var userId = _userManager.GetUserId(User);
            if (userId != null)
            {
                expense.UserId = userId;
                await _expenseServices.CreateExpenseAsync(expense);
                return Ok();
            }
            else
            {
                _logger.LogError("user not found");
                return View("Error");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating expense");
            return View("Error");
        }
    }
    [HttpPost("/Expense/UpdateExpense/{expenseId}")]
    public async Task<IActionResult> UpdateExpense(int expenseId, Expense expense)
    {
        try
        {
            await _expenseServices.UpdateExpenseAsync(expenseId, expense);
            _logger.LogInformation("success update expense");
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError("error while updating expense " + ex.Message);
            return View("Error");
        }
    }

    [HttpDelete("/Expense/DeleteExpense/{expenseId}")]
    public async Task<IActionResult> DeleteExpense(int expenseId)
    {
        try
        {
            await _expenseServices.DeleteExpenseAsync(expenseId);
            _logger.LogInformation("Success deleting expense");
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError("Error while deleting expense :" + ex.Message);
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
                var totalExpenses = await _expenseServices.GetTotalExpensesByCategoryAsync(categoryId, userId);
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
                var expenses = await _expenseServices.GetExpensesByDateRangeAsync(startDate, endDate);
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
    [HttpGet("/Expense/MonthlyChart")]
    public async Task<IActionResult> GetMonthlyForChartExpense()
    {
        try
        {
            var userId = _userManager.GetUserId(User);
            if (userId != null)
            {
                var monthlyExpense = await _expenseServices.GetMonthlyExpenseForYearChar(userId);
                return Ok(monthlyExpense);
            }
            else
            {
                _logger.LogError("userId not found");
                return Ok();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Error proseccing data : " + ex.Message);
            return View("Error");
        }
    }
    [HttpPost("/Expense/MigrateExpense")]
    public async Task<IActionResult> MigrateExpenseCaregory(int categoryIdFrom, int categoryIdTo)
    {
        try
        {
            await _expenseServices.MigrateExpenseData(categoryIdFrom, categoryIdTo);
            _logger.LogInformation("success migrate expense");
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError("error migrate expense : " + ex.Message);
            return View("Error");
        }
    }
    [HttpGet("/Expense/ExpenseDailyChart")]
    public IActionResult ExpenseDailyForChart()
    {
        try
        {
            var userId = _userManager.GetUserId(User);
            if (userId != null)
            {
                var result = _expenseServices.CountByCurrentWeek(userId);
                return Ok(result);
            }
            else
            {
                _logger.LogError("userId not Found");
                return View("Error");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("error get daily expense" + ex.Message);
            return View("Error");
        }
    }
    [HttpGet("/Expense/ExpenseWeeklyChart")]
    public IActionResult GetWeeklyDataChart()
    {
        try
        {
            var userId = _userManager.GetUserId(User);
            if (userId != null)
            {
                var result = _expenseServices.CountByCurrentMonth(userId);
                _logger.LogInformation("Success Get Wekkly Chart");
                return Ok(result);
            }
            else
            {
                _logger.LogError("UserId not found");
                return View("Error");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Error get weekly Chart" + ex.Message);
            return View("Error");
        }
    }

}
