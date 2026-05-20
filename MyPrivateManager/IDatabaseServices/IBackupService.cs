using MyPrivateManager.Models;

namespace MyPrivateManager.IDatabaseServices;

public interface IBackupService
{
    /// <summary>
    /// Creates a backup of the database
    /// </summary>
    /// <returns>Path to the backup file</returns>
    Task<string> BackupDatabaseAsync();

    /// <summary>
    /// Restores database from a specific backup file
    /// </summary>
    /// <param name="backupFileName">Name of the backup file to restore</param>
    /// <returns>True if restoration was successful</returns>
    Task<bool> RestoreDatabaseAsync(string backupFileName);

    /// <summary>
    /// Gets list of all available backups with details
    /// </summary>
    /// <returns>List of BackupInfo objects</returns>
    Task<List<BackupInfo>> GetBackupListAsync();

    /// <summary>
    /// Deletes a specific backup file
    /// </summary>
    /// <param name="backupFileName">Name of the backup file to delete</param>
    /// <returns>True if deletion was successful</returns>
    Task<bool> DeleteBackupAsync(string backupFileName);

    /// <summary>
    /// Performs automatic daily backup if needed
    /// </summary>
    Task<bool> PerformAutomaticBackupAsync();

    /// <summary>
    /// Gets the backup directory path
    /// </summary>
    string GetBackupDirectory();
}

public class BackupInfo
{
    public string FileName { get; set; }
    public DateTime CreatedDate { get; set; }
    public long SizeInBytes { get; set; }
    public string SizeFormatted => FormatBytes(SizeInBytes);

    private string FormatBytes(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB" };
        double len = bytes;
        int order = 0;
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len /= 1024;
        }
        return $"{len:0.##} {sizes[order]}";
    }
}
