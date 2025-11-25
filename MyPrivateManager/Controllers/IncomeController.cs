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
            var userIncomes = incomes.Where(i => i.Source.UserId == userId);
            var sources = await _sourceService.GetSourcesAsync();
            var userSources = sources.Where(i => i.UserId == userId);
            ViewBag.UserIncomes = userIncomes;
            ViewBag.Sources = userSources;
            return View();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving incomes");
            return View("Error");
        }
    }
    [HttpGet("/Income/GetIncome/{incomeId}")]
    public async Task<IActionResult> Details(int incomeId)
    {
        try
        {

            var income = await _incomeService.GetIncomeByIdAsync(incomeId);

            if (income == null)
            {
                _logger.LogError("item not found");
                return NotFound();
            }
            else
            {
                _logger.LogInformation("success get income");
                return Ok(income);
            }
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

    [HttpPost("/Income/CreateIncome")]
    public async Task<IActionResult> Create(Income income)
    {
        try
        {
            await _incomeService.CreateIncomeAsync(income);
            _logger.LogInformation("success create income");
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating income");
            return View("Error");
        }
    }


    [HttpPost("/Income/UpdateIncome/{incomeId}")]
    public async Task<IActionResult> Edit(int incomeId, Income income)
    {
        try
        {

            var success = await _incomeService.UpdateIncomeAsync(incomeId, income);
            _logger.LogInformation("Success delete income");
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating income");
            return View("Error");
        }
    }
    [HttpPost("/Source/MigrateIncome")]
    public async Task<IActionResult> MigrateIncome(int sourceIdFrom, int sourceIdTo)
    {
        try
        {
            var success = await _incomeService.MigrateIncomeAsync(sourceIdFrom, sourceIdTo);
            if (success)
            {
                _logger.LogInformation("success migrate income");
                return Ok();
            }
            else
            {
                _logger.LogError("error while migrate income");
                return Ok();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("error while prossesing data : " + ex);
            return View("Error");
        }

    }
    [HttpGet("/Income/IncomeByCategory")]
    public async Task<IActionResult> IncomeByCategoryEchart()
    {
        try
        {
            var userId = _userManager.GetUserId(User);
            if (userId != null)
            {
                var incomeData = await _incomeService.GetIncomeTotalByCategory(userId);
                _logger.LogInformation("Success get income for echart");
                return Ok(incomeData);
            }
            else
            {
                return View("Error");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Error getting income for echart" + ex.Message);
            return View("Error");
        }
    }

    [HttpDelete("/Income/DeleteIncome/{incomeId}")]
    public async Task<IActionResult> DeleteConfirmed(int incomeId)
    {
        try
        {
            await _incomeService.DeleteIncomeAsync(incomeId);
            _logger.LogInformation("success delete income");
            return Ok();

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
            var userIncome = monthlyIncomeStatistics.Where(i => i.Source.UserId == userId);
            return Ok(userIncome);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving monthly income statistics");
            return View("Error");
        }
    }
    [HttpGet("/Income/MonthlyChart")]
    public async Task<IActionResult> GetMonthlyForChartIncome()
    {
        try
        {
            var userId = _userManager.GetUserId(User);
            if (userId != null)
            {
                var monthlyIncome = await _incomeService.GetMonthlyIncomeForYearChar(userId);
                return Ok(monthlyIncome);
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

    [HttpGet("/Income/IncomeDailyChart")]
    public IActionResult GetDailyDataChart()
    {
        try
        {
            var userId = _userManager.GetUserId(User);
            if (userId != null)
            {
                var result = _incomeService.CountByCurrentWeek(userId);
                _logger.LogInformation("Success Get Daily Chart");
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
            _logger.LogError("Error get daily Chart" + ex.Message);
            return View("Error");
        }
    }

    [HttpGet("/Income/IncomeWeeklyChart")]
    public IActionResult GetWeeklyDataChart()
    {
        try
        {
            var userId = _userManager.GetUserId(User);
            if (userId != null)
            {
                var result = _incomeService.CountByCurrentMonth(userId);
                _logger.LogInformation("Success Get Weekly Chart");
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
