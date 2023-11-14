using MyPrivateManager.Models;

namespace MyPrivateManager.IDatabaseServices;

public interface IExpenseServices
{
    IEnumerable<Expense> GetExpenses();
    Task<Expense?> GetExpenseByIdAsync(int expenseId);
    Task<bool> CreateExpenseAsync(Expense expense);
    Task<bool> UpdateExpenseAsync(int expenseId, Expense expense);
    Task<bool> DeleteExpenseAsync(int expenseId);
    Task<decimal> GetTotalExpensesByCategoryAsync(int categoryId, string userId);
    Task<IEnumerable<Expense>> GetExpensesByDateRangeAsync(DateTime startDate, DateTime endDate);
}