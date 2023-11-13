using MyPrivateManager.Models;

namespace MyPrivateManager.IDatabaseServices;

public interface ISourceServices
{
    Task<IEnumerable<Source>> GetSourcesAsync();
    Task<Source?> GetSourceByIdAsync(int? sourceId);
    Task<bool> CreateSourceAsync(Source source);
    Task<bool> UpdateSourceAsync(int sourceId, Source source);
    Task<bool> DeleteSourceAsync(int sourceId);
}
