using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyPrivateManager.Data;
using MyPrivateManager.IDatabaseServices;
using MyPrivateManager.Models;

namespace MyPrivateManager.DatabaseServices;

public class ExpenseServices : IExpenseServices
{
    private readonly DatabaseContext _dbContext;
    private readonly UserManager<User> _userManager;

    public ExpenseServices(DatabaseContext dbContext, UserManager<User> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public IEnumerable<Expense> GetExpenses()
    {
        return _dbContext.Expenses
                    .Include(i => i.User)
                    .AsEnumerable();
    }

    public async Task<Expense?> GetExpenseByIdAsync(int expenseId)
    {
        return await _dbContext.Expenses.FindAsync(expenseId);
    }

    public async Task<bool> CreateExpenseAsync(Expense expense)
    {
        _dbContext.Expenses.Add(expense);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateExpenseAsync(int expenseId, Expense expense)
    {
        var existingExpense = await _dbContext.Expenses.FindAsync(expenseId);

        if (existingExpense != null)
        {
            existingExpense.Amount = expense.Amount;
            existingExpense.CategoryId = expense.CategoryId;
            existingExpense.Date = expense.Date;
            existingExpense.Description = expense.Description;
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
        var expense = await _dbContext.Expenses.FindAsync(expenseId);

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
            .Where(e => e.CategoryId == categoryId && e.UserId == userId)
            .SumAsync(e => e.Amount);
    }
    public async Task<IEnumerable<Expense>> GetExpensesByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _dbContext.Expenses
            .Where(e => e.Date >= startDate && e.Date <= endDate)
            .ToListAsync();
    }
}