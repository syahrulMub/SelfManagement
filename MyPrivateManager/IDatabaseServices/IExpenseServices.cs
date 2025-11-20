using MyPrivateManager.Models;

namespace MyPrivateManager.IDatabaseServices;

public interface IExpenseServices
{
    Task<IEnumerable<Expense>> GetExpenses();
    Task<Expense?> GetExpenseByIdAsync(int expenseId);
    Task<bool> CreateExpenseAsync(Expense expense);
    Task<bool> UpdateExpenseAsync(int expenseId, Expense expense);
    Task<bool> DeleteExpenseAsync(int expenseId);
    Task<decimal> GetTotalExpensesByCategoryAsync(int categoryId, string userId);
    Task<IEnumerable<Expense>> GetExpensesByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<int>> GetMonthlyExpenseForYearChar(string userId);
    Task<bool> MigrateExpenseData(int categoryFrom, int categoryTo);
    IEnumerable<decimal> CountByCurrentWeek(string userId);
    IEnumerable<decimal> CountByCurrentMonth(string userId);
    Task<IEnumerable<DTOTotalExpenseByCategory>> GetExpenseTotalByCategory(string userId, string filter);

}

public class DTOTotalExpenseByCategory
{
    public string? CategoryName { get; set; }
    public decimal Total { get; set; }
    public decimal MaxSum { get; set; }
}
