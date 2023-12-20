using MyPrivateManager.Models;

namespace MyPrivateManager.IDatabaseServices;

public interface IIncomeServices
{
    Task<IEnumerable<Income>> GetIncomesAsync();
    Task<Income?> GetIncomeByIdAsync(int? incomeId);
    Task<bool> CreateIncomeAsync(Income income);
    Task<bool> UpdateIncomeAsync(int incomeId, Income income);
    Task<bool> DeleteIncomeAsync(int incomeId);
    Task<bool> MigrateIncomeAsync(int sourceIdFrom, int sourceIdTo);
    Task<IEnumerable<Income>> GetMonthlyIncomeStatisticsAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<DTOTotalIncomeByCategory>> GetIncomeTotalByCategory();
}

public class DTOTotalIncomeByCategory
{
    public string? SourceName { get; set; }
    public decimal Total { get; set; }
    public decimal MaxSum { get; set; }
}