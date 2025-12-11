namespace MyPrivateManager.Models;

public class LogFileInfo
{
    public string FileName { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public long SizeBytes { get; set; }
    public int LineCount { get; set; }
}

public class LogEntry
{
    public DateTime Timestamp { get; set; }
    public string Level { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Exception { get; set; }
    public string FullText { get; set; } = string.Empty;
}
