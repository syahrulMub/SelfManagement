using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyPrivateManager.IDatabaseServices;
using MyPrivateManager.Models;

namespace MyPrivateManager.Controllers;

public class IncomeController : Controller
{

    private readonly IIncomeServices _incomeService;
    private readonly UserManager<User> _userManager;
    private readonly ISourceServices _sourceService;
    private readonly ILogger<IncomeController> _logger;

    public IncomeController(IIncomeServices incomeService, UserManager<User> userManager, ILogger<IncomeController> logger, ISourceServices sourceService)
    {
        _incomeService = incomeService;
        _sourceService = sourceService;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var userId = _userManager.GetUserId(User);
            var incomes = await _incomeService.GetIncomesAsync();
            var userIncomes = incomes.Where(i => i.UserId == userId);
            var sources = await _sourceService.GetSourcesAsync();
            ViewBag.UserIncome = userIncomes;
            ViewBag.Sources = sources;
            return View();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving incomes");
            return View("Error");
        }
    }

    public async Task<IActionResult> Details(int? id)
    {
        try
        {
            if (id == null)
            {
                return NotFound();
            }

            var income = await _incomeService.GetIncomeByIdAsync(id.Value);

            if (income == null)
            {
                return NotFound();
            }

            return View(income);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving income details");
            return View("Error");
        }
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Amount,Date,Description")] Income income)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);
                if (userId != null)
                {
                    income.UserId = userId;
                    await _incomeService.CreateIncomeAsync(income);
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    _logger.LogError("Error creating income becouse user not found");
                    return BadRequest("user not found");
                }
            }

            return View(income);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating income");
            return View("Error");
        }
    }

    public async Task<IActionResult> Edit(int? id)
    {
        try
        {
            if (id == null)
            {
                return NotFound();
            }

            var income = await _incomeService.GetIncomeByIdAsync(id.Value);

            if (income == null)
            {
                return NotFound();
            }

            return View(income);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving income for edit");
            return View("Error");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Amount,Description")] Income income)
    {
        try
        {
            if (id != income.IncomeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var success = await _incomeService.UpdateIncomeAsync(id, income);

                if (!success)
                {
                    return NotFound();
                }

                return RedirectToAction(nameof(Index));
            }

            return View(income);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating income");
            return View("Error");
        }
    }

    public async Task<IActionResult> Delete(int? id)
    {
        try
        {
            if (id == null)
            {
                return NotFound();
            }

            var income = await _incomeService.GetIncomeByIdAsync(id.Value);

            if (income == null)
            {
                return NotFound();
            }

            return View(income);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving income for delete");
            return View("Error");
        }
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var success = await _incomeService.DeleteIncomeAsync(id);

            if (!success)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting income");
            return View("Error");
        }
    }
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Income>>> GetMonthlyIncomeStatistics(DateTime startDate, DateTime endDate)
    {
        try
        {
            var userId = _userManager.GetUserId(User);
            var monthlyIncomeStatistics = await _incomeService.GetMonthlyIncomeStatisticsAsync(startDate, endDate);
            var userIncome = monthlyIncomeStatistics.Where(i => i.UserId == userId);
            return Ok(userIncome);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving monthly income statistics");
            return View("Error");
        }
    }
}
