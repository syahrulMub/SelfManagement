using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyPrivateManager.Data;
using MyPrivateManager.IDatabaseServices;
using MyPrivateManager.Models;
using MyPrivateManager.DTOs;

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
    public async Task<IEnumerable<int>> GetMonthlyExpenseForYearChar(string userId, int? year = null)
    {
        var selectedYear = year ?? DateTime.Now.Year;
        var monthly = await _dbContext.Expenses
                            .Where(i => i.Category.UserId == userId && i.Date.Year == selectedYear)
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

        var categoryIds = _dbContext.Categories
                            .Where(i => i.UserId == userId)
                            .Select(i => i.CategoryId)
                            .ToList();

        if (!categoryIds.Any())
        {
            return Enumerable.Repeat(0m, 7).ToList();
        }

        var amounts = Enumerable.Range(0, 7)
            .Select(offset => firstDay.AddDays(offset))
            .Select(date => CountExpenseDaily(date, categoryIds))
            .ToList();

        return amounts;
    }

    private decimal CountExpenseDaily(DateTime date, List<int> categoryIds)
    {
        var count = _dbContext.Expenses
            .Where(i => categoryIds.Contains(i.CategoryId) && i.Date == date)
            .Sum(i => i.Amount);
        return (decimal)count;
    }

    public IEnumerable<decimal> CountByCurrentMonth(string userId)
    {
        var today = DateTime.Now;
        var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
        var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

        var categoryIds = _dbContext.Categories
                            .Where(i => i.UserId == userId)
                            .Select(i => i.CategoryId)
                            .ToList();

        if (!categoryIds.Any())
        {
            return Enumerable.Repeat(0m, 5).ToList();
        }

        var amounts = Enumerable.Range(0, (int)(lastDayOfMonth - firstDayOfMonth).TotalDays + 1)
            .Select(offset => firstDayOfMonth.AddDays(offset))
            .GroupBy(date => (int)Math.Ceiling(date.Day / 7.0))
            .Select(group => group.Sum(date => CountExpenseDaily(date, categoryIds)))
            .Take(5)
            .ToList();

        return amounts;
    }
    public async Task<IEnumerable<DTOTotalExpenseByCategory>> GetExpenseTotalByCategory(string userId, string filter, int? year = null)
    {
        var query = _dbContext.Expenses.Where(i => i.Category.UserId == userId);
        var selectedYear = year ?? DateTime.Now.Year;
        var today = DateTime.Now;

        // Apply year filter first
        query = query.Where(e => e.Date.Year == selectedYear);

        switch (filter.ToLower())
        {
            case "daily":
                query = query.Where(e => e.Date.Date == today.Date);
                break;
            case "weekly":
                var startOfWeek = today.Date.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday);
                var endOfWeek = startOfWeek.AddDays(7);
                query = query.Where(e => e.Date >= startOfWeek && e.Date < endOfWeek);
                break;
            case "monthly":
                var startOfMonth = new DateTime(selectedYear, today.Month, 1);
                var endOfMonth = startOfMonth.AddMonths(1);
                query = query.Where(e => e.Date >= startOfMonth && e.Date < endOfMonth);
                break;
            case "yearly":
                var startOfYear = new DateTime(selectedYear, 1, 1);
                var endOfYear = startOfYear.AddYears(1);
                query = query.Where(e => e.Date >= startOfYear && e.Date < endOfYear);
                break;
        }

        var categoryData = await query
                            .GroupBy(i => i.CategoryId)
                            .Select(i => new
                            {
                                category = i.Key,
                                total = i.Sum(i => i.Amount),
                                expenseIds = i.Select(e => e.ExpenseId).ToList()
                            })
                            .ToDictionaryAsync(i => i.category, i => new { i.total, i.expenseIds });

        var totalCategory = await _dbContext.Categories
                            .Where(c => c.UserId == userId)
                            .ToListAsync();

        var result = totalCategory
                    .Select(category => new DTOTotalExpenseByCategory
                    {
                        CategoryName = category.CategoryName,
                        Total = categoryData.TryGetValue(category.CategoryId, out var data) ? data.total : 0,
                        MaxSum = categoryData.Values.Any() ? categoryData.Values.Max(v => v.total) : 0,
                        ExpenseIds = categoryData.TryGetValue(category.CategoryId, out var ids) ? ids.expenseIds : new List<int>()
                    })
                    .OrderBy(i => i.CategoryName)
                    .ToList();
        return result;
    }

    public async Task<IEnumerable<DTOExpenseDetail>> GetExpenseDetailsByIds(List<int> expenseIds)
    {
        if (expenseIds == null || !expenseIds.Any())
        {
            return new List<DTOExpenseDetail>();
        }

        var expenses = await _dbContext.Expenses
                            .Where(e => expenseIds.Contains(e.ExpenseId))
                            .OrderByDescending(e => e.Amount)
                            .Select(e => new DTOExpenseDetail
                            {
                                ExpenseId = e.ExpenseId,
                                Description = e.Description,
                                Amount = e.Amount,
                                Date = e.Date
                            })
                            .ToListAsync();

        return expenses;
    }

    public async Task<DTOTotalCompareWithPrevious> GetTotalExpensesThisMonthAsync(string userId)
    {
        var today = DateTime.Now;
        var startOfMonth = new DateTime(today.Year, today.Month, 1);
        var previousMonth = startOfMonth.AddMonths(-1);
        var endOfMonth = startOfMonth.AddMonths(1);
        decimal amount = await _dbContext.Expenses.Where(e => e.Category.UserId == userId && e.Date >= startOfMonth && e.Date < endOfMonth)
                                        .SumAsync(e => e.Amount);
        decimal previousAmount = await _dbContext.Expenses.Where(e => e.Category.UserId == userId && e.Date >= previousMonth && e.Date < startOfMonth)
                                        .SumAsync(e => e.Amount);

        var percentageChange = previousAmount != 0 ? ((amount - previousAmount) / previousAmount) * 100 : 0;

        return new DTOTotalCompareWithPrevious
        {
            CurrentTotal = amount,
            PercentageChange = percentageChange
        };
    }
}


