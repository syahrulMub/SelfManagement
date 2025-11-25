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
        return await _dbContext.Incomes
                        .OrderByDescending(i => i)
                        .ToListAsync();
    }

    public async Task<Income?> GetIncomeByIdAsync(int? incomeId)
    {
        return await _dbContext.Incomes
                .Include(i => i.Source)
                .FirstOrDefaultAsync(i => i.IncomeId == incomeId);
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
        var existingIncome = await _dbContext.Incomes.FirstOrDefaultAsync(i => i.IncomeId == incomeId);

        if (existingIncome != null)
        {
            existingIncome.Amount = income.Amount;
            existingIncome.SourceId = income.SourceId;
            existingIncome.Date = income.Date;
            existingIncome.Description = income.Description;

            _dbContext.Incomes.Update(existingIncome);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        return false;
    }

    public async Task<bool> DeleteIncomeAsync(int incomeId)
    {
        var income = await _dbContext.Incomes.FirstOrDefaultAsync(i => i.IncomeId == incomeId);

        if (income != null)
        {
            _dbContext.Incomes.Remove(income);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        return false;
    }
    public async Task<bool> MigrateIncomeAsync(int sourceIdFrom, int sourceIdTo)
    {
        var dataIncome = await _dbContext.Incomes
                            .Where(i => i.SourceId == sourceIdFrom)
                            .ToListAsync();
        if (dataIncome == null)
        {
            return true;
        }
        else
        {
            foreach (var data in dataIncome)
            {
                data.SourceId = sourceIdTo;
                _dbContext.Incomes.Update(data);
            }
            await _dbContext.SaveChangesAsync();
            return true;
        }
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
    public async Task<IEnumerable<DTOTotalIncomeByCategory>> GetIncomeTotalByCategory(string userId)
    {
        var categoryTotal = _dbContext.Incomes
                            .Where(i => i.Source.UserId == userId)
                            .GroupBy(i => i.SourceId)
                            .Select(i => new
                            {
                                source = i.Key,
                                total = i.Sum(i => i.Amount)
                            })
                            .ToDictionary(i => i.source, i => i.total);
        var totalSource = await _dbContext.Sources
                            .ToListAsync();
        var result = totalSource
                        .Select(source => new DTOTotalIncomeByCategory
                        {
                            SourceName = source.SourceName,
                            Total = categoryTotal.TryGetValue(source.SourceId, out var total) ? total : 0,
                            MaxSum = categoryTotal.Values.Max()
                        })
                        .OrderBy(i => i.SourceName)
                        .ToList();
        return result;
    }

    public async Task<IEnumerable<int>> GetMonthlyIncomeForYearChar(string userId)
    {
        var monthly = await _dbContext.Incomes
                            .Where(i => i.Source.UserId == userId)
                            .GroupBy(i => i.Date.Month)
                            .Select(i => new
                            {
                                Month = i.Key,
                                TotalIncome = i.Sum(i => i.Amount)
                            })
                            .ToListAsync();
        
        var result = Enumerable.Range(1, 12)
                            .Select(month => new
                            {
                                Month = month,
                                TotalIncome = monthly.FirstOrDefault(i => i.Month == month)?.TotalIncome ?? 0
                            })
                            .OrderBy(i => i.Month)
                            .Select(i => i.TotalIncome)
                            .ToList();
        return result;
    }

    public IEnumerable<decimal> CountByCurrentWeek(string userId)
    {
        var today = DateTime.Now;
        var firstDay = today.Date.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday);

        var sourceIds = _dbContext.Sources
                            .Where(i => i.UserId == userId)
                            .Select(i => i.SourceId)
                            .ToList();

        if (!sourceIds.Any())
        {
            return Enumerable.Repeat(0m, 7).ToList();
        }

        var amounts = Enumerable.Range(0, 7)
            .Select(offset => firstDay.AddDays(offset))
            .Select(date => CountIncomeDaily(date, sourceIds))
            .ToList();

        return amounts;
    }

    private decimal CountIncomeDaily(DateTime date, List<int> sourceIds)
    {
        var count = _dbContext.Incomes
            .Where(i => sourceIds.Contains(i.SourceId) && i.Date == date)
            .Sum(i => i.Amount);
        return (decimal)count;
    }

    public IEnumerable<decimal> CountByCurrentMonth(string userId)
    {
        var today = DateTime.Now;
        var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
        var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

        var sourceIds = _dbContext.Sources
                            .Where(i => i.UserId == userId)
                            .Select(i => i.SourceId)
                            .ToList();

        if (!sourceIds.Any())
        {
             return Enumerable.Repeat(0m, 5).ToList();
        }

        var amounts = Enumerable.Range(0, (int)(lastDayOfMonth - firstDayOfMonth).TotalDays + 1)
            .Select(offset => firstDayOfMonth.AddDays(offset))
            .GroupBy(date => (int)Math.Ceiling(date.Day / 7.0))
            .Select(group => group.Sum(date => CountIncomeDaily(date, sourceIds)))
            .Take(5)
            .ToList();

        return amounts;
    }
}