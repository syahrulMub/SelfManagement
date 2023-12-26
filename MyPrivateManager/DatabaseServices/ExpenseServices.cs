using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyPrivateManager.Data;
using MyPrivateManager.IDatabaseServices;
using MyPrivateManager.Models;

namespace MyPrivateManager.DatabaseServices;

public class ExpenseServices : IExpenseServices
{
    private readonly DatabaseContext _dbContext;

    public ExpenseServices(DatabaseContext dbContext)
    {
        _dbContext = dbContext;

    }

    public async Task<IEnumerable<Expense>> GetExpenses()
    {
        return await _dbContext.Expenses
                    .Include(i => i.Category)
                    .OrderByDescending(i => i)
                    .ToListAsync();
    }

    public async Task<Expense?> GetExpenseByIdAsync(int expenseId)
    {
        return await _dbContext.Expenses
                    .Include(i => i.Category)
                    .FirstOrDefaultAsync(i => i.ExpenseId == expenseId);
    }

    public async Task<bool> CreateExpenseAsync(Expense expense)
    {
        _dbContext.Expenses.Add(expense);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateExpenseAsync(int expenseId, Expense expense)
    {
        var existingExpense = await _dbContext.Expenses.FirstOrDefaultAsync(i => i.ExpenseId == expenseId);

        if (existingExpense != null)
        {
            existingExpense.Amount = expense.Amount;
            existingExpense.CategoryId = expense.CategoryId;
            existingExpense.Date = expense.Date;
            existingExpense.Description = expense.Description;

            _dbContext.Expenses.Update(existingExpense);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        else
        {
            return false;
        }
    }

    public async Task<bool> DeleteExpenseAsync(int expenseId)
    {
        var expense = await _dbContext.Expenses.FirstOrDefaultAsync(i => i.ExpenseId == expenseId);

        if (expense != null)
        {
            _dbContext.Expenses.Remove(expense);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        else
        {
            return false;
        }
    }
    public async Task<decimal> GetTotalExpensesByCategoryAsync(int categoryId, string userId)
    {
        return await _dbContext.Expenses
            .Where(e => e.CategoryId == categoryId && e.Category.UserId == userId)
            .SumAsync(e => e.Amount);
    }
    public async Task<IEnumerable<Expense>> GetExpensesByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _dbContext.Expenses
            .Where(e => e.Date >= startDate && e.Date <= endDate)
            .ToListAsync();
    }
    public async Task<IEnumerable<decimal>> GetMonthlyExpenseForYearChar(string userId)
    {
        var monthly = await _dbContext.Expenses
                            .Where(i => i.Category.UserId == userId)
                            .GroupBy(i => i.Date.Month)
                            .Select(i => new
                            {
                                Month = i.Key,
                                totalExpense = i.Sum(i => i.Amount)
                            })
                            .ToListAsync();
        var result = Enumerable.Range(1, 12)
                            .Select(month => new
                            {
                                Month = month,
                                TotalExpense = monthly.FirstOrDefault(i => i.Month == month)?.totalExpense ?? 0
                            })
                            .OrderBy(i => i.Month)
                            .Select(i => i.TotalExpense)
                            .ToList();
        return result;
    }
    public async Task<bool> MigrateExpenseData(int categoryFrom, int categoryTo)
    {
        var currentExpense = await _dbContext.Expenses
                                .Where(i => i.CategoryId == categoryFrom)
                                .ToListAsync();
        if (currentExpense == null)
        {
            return true;
        }
        else
        {
            foreach (var expense in currentExpense)
            {
                expense.CategoryId = categoryTo;
                _dbContext.Update(expense);
            }
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
    public IEnumerable<decimal> CountByCurrentWeek(string userId)
    {
        var today = DateTime.Now;
        var firstDay = today.Date.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday);

        var amounts = Enumerable.Range(0, 7)
            .Select(offset => firstDay.AddDays(offset))
            .Select(date => CountExpenseDaily(date, userId))
            .ToList();

        return amounts;

        // List<decimal> amount = new();
        // var today = DateTime.Now;
        // var firstDay = today.Date.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday);
        // DateTime currentDate = firstDay;
        // for (int i = 1; i <= 7; i++)
        // {
        //     var countDaily = CountExpenseDaily(currentDate, userId);
        //     if (countDaily == 0)
        //     {
        //         amount.Add(0);
        //     }
        //     else
        //     {
        //         amount.Add(countDaily);
        //     }
        //     currentDate = currentDate.AddDays(1);
        // }
        // return amount;
    }
    private decimal CountExpenseDaily(DateTime date, string userId)
    {
        var count = _dbContext.Expenses
            .Where(i => i.Category.UserId == userId && i.Date == date)
            .Sum(i => i.Amount);
        return count;
        // var count = _dbContext.Expenses
        //             .Where(i => i.UserId == userId)
        //             .Where(i => i.Date == date)
        //             .Sum(i => i.Amount);
        // return count;
    }
    public IEnumerable<decimal> CountByCurrentMonth(string userId)
    {
        var today = DateTime.Now;
        var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
        var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

        var amounts = Enumerable.Range(0, (int)(lastDayOfMonth - firstDayOfMonth).TotalDays + 1)
            .Select(offset => firstDayOfMonth.AddDays(offset))
            .GroupBy(date => (int)Math.Ceiling(date.Day / 7.0))
            .Select(group => group.Sum(date => CountExpenseDaily(date, userId)))
            .Take(5)
            .ToList();

        return amounts;
    }
}
