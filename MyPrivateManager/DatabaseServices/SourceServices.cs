using Microsoft.EntityFrameworkCore;
using MyPrivateManager.Data;
using MyPrivateManager.IDatabaseServices;
using MyPrivateManager.Models;

namespace MyPrivateManager.DatabaseServices;

public class SourceServices : ISourceServices
{
    private readonly DatabaseContext _dbContext;

    public SourceServices(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Source>> GetSourcesAsync()
    {
        return await _dbContext.Sources.ToListAsync();
    }

    public async Task<Source?> GetSourceByIdAsync(int? sourceId)
    {
        return await _dbContext.Sources.FirstOrDefaultAsync(i => i.SourceId == sourceId);
    }

    public async Task<bool> CreateSourceAsync(Source source)
    {
        _dbContext.Sources.Add(source);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateSourceAsync(int sourceId, Source source)
    {
        var existingSource = await _dbContext.Sources.FirstOrDefaultAsync(i => i.SourceId == sourceId);

        if (existingSource != null)
        {
            existingSource.SourceName = source.SourceName;
            _dbContext.Sources.Update(existingSource);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        else
        {
            return false;
        }
    }

    public async Task<bool> DeleteSourceAsync(int sourceId)
    {
        var source = await _dbContext.Sources.FindAsync(sourceId);

        if (source != null)
        {
            _dbContext.Sources.Remove(source);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        else
        {
            return false;
        }
    }
}
