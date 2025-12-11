using System.Text.RegularExpressions;
using MyPrivateManager.IDatabaseServices;
using MyPrivateManager.Models;

namespace MyPrivateManager.DatabaseServices;

public class LogFileService : ILogFileService
{
    private readonly string _logDirectory;
    private readonly ILogger<LogFileService> _logger;

    public LogFileService(ILogger<LogFileService> logger, IWebHostEnvironment env)
    {
        _logger = logger;
        _logDirectory = Path.Combine(env.ContentRootPath, "Logs");
        
        // Ensure log directory exists
        if (!Directory.Exists(_logDirectory))
        {
            Directory.CreateDirectory(_logDirectory);
        }
    }

    public async Task<List<LogFileInfo>> GetLogFilesAsync()
    {
        try
        {
            var logFiles = new List<LogFileInfo>();
            var files = Directory.GetFiles(_logDirectory, "log-*.log")
                .OrderByDescending(f => f);

            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);
                var fileName = Path.GetFileName(file);
                
                // Extract date from filename (e.g., log-20251211.log)
                var dateMatch = Regex.Match(fileName, @"log-(\d{8})\.log");
                DateTime fileDate = DateTime.MinValue;
                
                if (dateMatch.Success)
                {
                    var dateStr = dateMatch.Groups[1].Value;
                    DateTime.TryParseExact(dateStr, "yyyyMMdd", null, 
                        System.Globalization.DateTimeStyles.None, out fileDate);
                }

                var lineCount = 0;
                try
                {
                    lineCount = File.ReadLines(file).Count();
                }
                catch
                {
                    // File might be locked, skip counting
                }

                logFiles.Add(new LogFileInfo
                {
                    FileName = fileName,
                    Date = fileDate,
                    SizeBytes = fileInfo.Length,
                    LineCount = lineCount
                });
            }

            return logFiles;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting log files");
            return new List<LogFileInfo>();
        }
    }

    public async Task<List<LogEntry>> ReadLogFileAsync(string fileName, string? filterLevel = null, string? searchText = null)
    {
        try
        {
            var filePath = Path.Combine(_logDirectory, fileName);
            
            if (!File.Exists(filePath))
            {
                _logger.LogWarning($"Log file not found: {fileName}");
                return new List<LogEntry>();
            }

            var entries = new List<LogEntry>();
            
            // Read file with shared access to avoid locking issues
            // Serilog is writing to this file, so we need FileShare.ReadWrite
            var lines = new List<string>();
            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var reader = new StreamReader(fileStream))
            {
                string? line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    lines.Add(line);
                }
            }
            
            LogEntry? currentEntry = null;
            
            foreach (var line in lines)
            {
                // Pattern: 2025-12-11 09:29:34.123 +07:00 [INF] Message
                var match = Regex.Match(line, @"^(\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}\.\d{3} [+-]\d{2}:\d{2}) \[(\w{3})\] (.+)$");
                
                if (match.Success)
                {
                    // Save previous entry
                    if (currentEntry != null)
                    {
                        if (ShouldIncludeEntry(currentEntry, filterLevel, searchText))
                        {
                            entries.Add(currentEntry);
                        }
                    }
                    
                    // Start new entry
                    currentEntry = new LogEntry
                    {
                        Timestamp = DateTime.Parse(match.Groups[1].Value),
                        Level = MapLogLevel(match.Groups[2].Value),
                        Message = match.Groups[3].Value,
                        FullText = line
                    };
                }
                else if (currentEntry != null)
                {
                    // This is a continuation line (likely exception stack trace)
                    currentEntry.Exception = (currentEntry.Exception ?? "") + line + "\n";
                    currentEntry.FullText += "\n" + line;
                }
            }
            
            // Add last entry
            if (currentEntry != null && ShouldIncludeEntry(currentEntry, filterLevel, searchText))
            {
                entries.Add(currentEntry);
            }
            
            return entries.OrderByDescending(e => e.Timestamp).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error reading log file: {fileName}");
            return new List<LogEntry>();
        }
    }

    public async Task<List<LogEntry>> GetLogsByDateRangeAsync(DateTime startDate, DateTime endDate, string? filterLevel = null)
    {
        try
        {
            var allEntries = new List<LogEntry>();
            var logFiles = await GetLogFilesAsync();
            
            var relevantFiles = logFiles
                .Where(f => f.Date >= startDate.Date && f.Date <= endDate.Date)
                .ToList();

            foreach (var file in relevantFiles)
            {
                var entries = await ReadLogFileAsync(file.FileName, filterLevel);
                allEntries.AddRange(entries.Where(e => 
                    e.Timestamp >= startDate && e.Timestamp <= endDate));
            }

            return allEntries.OrderByDescending(e => e.Timestamp).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting logs by date range");
            return new List<LogEntry>();
        }
    }

    private bool ShouldIncludeEntry(LogEntry entry, string? filterLevel, string? searchText)
    {
        if (!string.IsNullOrEmpty(filterLevel) && 
            !entry.Level.Equals(filterLevel, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (!string.IsNullOrEmpty(searchText) && 
            !entry.FullText.Contains(searchText, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return true;
    }

    private string MapLogLevel(string level)
    {
        return level switch
        {
            "INF" => "Information",
            "WRN" => "Warning",
            "ERR" => "Error",
            "DBG" => "Debug",
            "FTL" => "Fatal",
            _ => level
        };
    }
}
