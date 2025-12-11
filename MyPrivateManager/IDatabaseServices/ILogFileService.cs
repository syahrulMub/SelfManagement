using MyPrivateManager.Models;

namespace MyPrivateManager.IDatabaseServices;

public interface ILogFileService
{
    /// <summary>
    /// Gets a list of all available log files
    /// </summary>
    /// <returns>List of log file information</returns>
    Task<List<LogFileInfo>> GetLogFilesAsync();

    /// <summary>
    /// Reads the content of a specific log file
    /// </summary>
    /// <param name="fileName">Name of the log file</param>
    /// <param name="filterLevel">Optional log level filter (Error, Warning, Information, Debug)</param>
    /// <param name="searchText">Optional search text</param>
    /// <returns>List of log entries</returns>
    Task<List<LogEntry>> ReadLogFileAsync(string fileName, string? filterLevel = null, string? searchText = null);

    /// <summary>
    /// Gets log entries for a specific date range
    /// </summary>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <param name="filterLevel">Optional log level filter</param>
    /// <returns>List of log entries</returns>
    Task<List<LogEntry>> GetLogsByDateRangeAsync(DateTime startDate, DateTime endDate, string? filterLevel = null);
}
