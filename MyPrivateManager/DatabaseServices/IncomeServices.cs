using Microsoft.EntityFrameworkCore;
using MyPrivateManager.Data;
using MyPrivateManager.IDatabaseServices;
using MyPrivateManager.Models;

namespace MyPrivateManager.DatabaseServices;

public class IncomeServices : IIncomeServices
{
    private readonly DatabaseContext _dbContext;

    public IncomeServices(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Income>> GetIncomesAsync()
    {
        return await _dbContext.Incomes.ToListAsync();
    }

    public async Task<Income?> GetIncomeByIdAsync(int incomeId)
    {
        return await _dbContext.Incomes.FindAsync(incomeId);
    }

    public async Task<bool> CreateIncomeAsync(Income income)
    {
        try
        {
            _dbContext.Incomes.Add(income);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UpdateIncomeAsync(int incomeId, Income income)
    {
        var existingIncome = await _dbContext.Incomes.FindAsync(incomeId);

        if (existingIncome != null)
        {
            existingIncome.Amount = income.Amount;
            existingIncome.Date = DateTime.Now;
            existingIncome.Description = income.Description;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        return false;
    }

    public async Task<bool> DeleteIncomeAsync(int incomeId)
    {
        var income = await _dbContext.Incomes.FindAsync(incomeId);

        if (income != null)
        {
            _dbContext.Incomes.Remove(income);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        return false;
    }
    public async Task<IEnumerable<Income>> GetMonthlyIncomeStatisticsAsync(DateTime startDate, DateTime endDate)
    {
        var monthlyIncomeStatistics = await _dbContext.Incomes
            .Where(i => i.Date >= startDate && i.Date <= endDate)
            .GroupBy(i => new { Year = i.Date.Year, Month = i.Date.Month })
            .Select(group => new Income
            {
                Amount = group.Sum(i => i.Amount),
                Date = new DateTime(group.Key.Year, group.Key.Month, 1),
            })
            .ToListAsync();

        return monthlyIncomeStatistics;
    }
}