using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyPrivateManager.IDatabaseServices;
using MyPrivateManager.Models;

namespace MyPrivateManager.Controllers;

public class HomeController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly IExpenseServices _expenseServices;
    private readonly IIncomeServices _incomeServices;
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger, UserManager<User> userManager, IExpenseServices expenseServices, IIncomeServices incomeServices)
    {
        _userManager = userManager;
        _expenseServices = expenseServices;
        _incomeServices = incomeServices;
        _logger = logger;
    }
    [Authorize]
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    public async Task<IActionResult> UserInfoDashboard()
    {
        try
        {
            var userId = _userManager.GetUserId(User);
            var expenses = await _expenseServices.GetExpenses();
            if (userId != null)
            {
                var incomes = await _incomeServices.GetIncomesAsync();
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    decimal totalExpenses = expenses.Where(i => i.Category.UserId == userId).Sum(e => e.Amount);
                    decimal totalIncomes = incomes.Where(i => i.Source.UserId == userId).Sum(i => i.Amount);
                    decimal netBalance = user.Balance + totalIncomes - totalExpenses;

                    var financialSummaryViewModel = new FinancialSummaryViewModel
                    {
                        TotalExpenses = totalExpenses,
                        TotalIncomes = totalIncomes,
                        NetBalance = netBalance
                    };
                    return View(financialSummaryViewModel);
                }
                else
                {
                    decimal totalExpenses = expenses.Where(i => i.Category.UserId == userId).Sum(e => e.Amount);
                    decimal totalIncomes = incomes.Where(i => i.Source.UserId == userId).Sum(i => i.Amount);
                    decimal netBalance = totalIncomes - totalExpenses;

                    var financialSummaryViewModel = new FinancialSummaryViewModel
                    {
                        TotalExpenses = totalExpenses,
                        TotalIncomes = totalIncomes,
                        NetBalance = netBalance
                    };
                    return View(financialSummaryViewModel);
                }
            }
            else
            {
                _logger.LogWarning("User not found");
                return View("User not found");
            }


        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving financial summary");
            return View("Error");
        }
    }
}

internal class FinancialSummaryViewModel
{
    public decimal TotalExpenses { get; set; }
    public decimal TotalIncomes { get; set; }
    public decimal NetBalance { get; set; }
}
