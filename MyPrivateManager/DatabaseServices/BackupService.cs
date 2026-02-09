using MyPrivateManager.IDatabaseServices;
using MyPrivateManager.Models;
using Serilog;

namespace MyPrivateManager.DatabaseServices;

public class BackupService : IBackupService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<BackupService> _logger;
    private const string BACKUP_FOLDER = "DatabaseBackups";
    private const string LAST_BACKUP_FILE = "LastBackupDate.txt";

    public BackupService(IConfiguration configuration, ILogger<BackupService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public string GetBackupDirectory()
    {
        var backupDir = Path.Combine(Directory.GetCurrentDirectory(), BACKUP_FOLDER);
        if (!Directory.Exists(backupDir))
        {
            Directory.CreateDirectory(backupDir);
        }
        return backupDir;
    }

    public async Task<string> BackupDatabaseAsync()
    {
        try
        {
            var backupDir = GetBackupDirectory();
            var dbPath = GetDatabasePath();

            if (!File.Exists(dbPath))
            {
                _logger.LogWarning("Database file not found at: {DatabasePath}", dbPath);
                throw new FileNotFoundException($"Database file not found: {dbPath}");
            }

            var timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss-fff");
            var backupFileName = $"SelfManagement_Backup_{timestamp}.db";
            var backupPath = Path.Combine(backupDir, backupFileName);

            // Close any connections before backing up (for SQLite)
            File.Copy(dbPath, backupPath, overwrite: false);

            // Update last backup date
            await UpdateLastBackupDateAsync();

            _logger.LogInformation("Database backed up successfully to: {BackupPath}", backupPath);
            Log.Information($"Database backup created: {backupFileName}");

            return backupFileName;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while backing up database");
            Log.Error($"Database backup failed: {ex.Message}");
            throw;
        }
    }

    public async Task<bool> RestoreDatabaseAsync(string backupFileName)
    {
        try
        {
            var backupDir = GetBackupDirectory();
            var backupPath = Path.Combine(backupDir, backupFileName);
            var dbPath = GetDatabasePath();

            if (!File.Exists(backupPath))
            {
                _logger.LogWarning("Backup file not found: {BackupPath}", backupPath);
                throw new FileNotFoundException($"Backup file not found: {backupPath}");
            }

            // Create a safety copy of current database
            if (File.Exists(dbPath))
            {
                var safetyFileName = $"SelfManagement_PreRestore_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.db";
                var safetyPath = Path.Combine(backupDir, safetyFileName);
                File.Copy(dbPath, safetyPath, overwrite: false);
                _logger.LogInformation("Safety copy created at: {SafetyPath}", safetyPath);
            }

            // Restore from backup
            File.Copy(backupPath, dbPath, overwrite: true);

            _logger.LogInformation("Database restored successfully from: {BackupFile}", backupFileName);
            Log.Information($"Database restored from backup: {backupFileName}");

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while restoring database from: {BackupFile}", backupFileName);
            Log.Error($"Database restore failed: {ex.Message}");
            throw;
        }
    }

    public async Task<List<BackupInfo>> GetBackupListAsync()
    {
        try
        {
            var backupDir = GetBackupDirectory();
            var backupList = new List<BackupInfo>();

            if (!Directory.Exists(backupDir))
            {
                return backupList;
            }

            var backupFiles = Directory.GetFiles(backupDir, "SelfManagement_Backup_*.db")
                .OrderByDescending(f => File.GetCreationTime(f))
                .ToList();

            foreach (var file in backupFiles)
            {
                var fileInfo = new FileInfo(file);
                backupList.Add(new BackupInfo
                {
                    FileName = Path.GetFileName(file),
                    CreatedDate = fileInfo.CreationTime,
                    SizeInBytes = fileInfo.Length
                });
            }

            return backupList;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving backup list");
            return new List<BackupInfo>();
        }
    }

    public async Task<bool> DeleteBackupAsync(string backupFileName)
    {
        try
        {
            var backupDir = GetBackupDirectory();
            var backupPath = Path.Combine(backupDir, backupFileName);

            if (!File.Exists(backupPath))
            {
                _logger.LogWarning("Backup file not found for deletion: {BackupPath}", backupPath);
                return false;
            }

            File.Delete(backupPath);
            _logger.LogInformation("Backup file deleted: {BackupFile}", backupFileName);
            Log.Information($"Backup deleted: {backupFileName}");

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting backup: {BackupFile}", backupFileName);
            return false;
        }
    }

    public async Task<bool> PerformAutomaticBackupAsync()
    {
        try
        {
            var lastBackupPath = Path.Combine(GetBackupDirectory(), LAST_BACKUP_FILE);
            var today = DateTime.Now.Date;

            // Check if backup already exists for today
            if (File.Exists(lastBackupPath))
            {
                if (DateTime.TryParse(File.ReadAllText(lastBackupPath).Trim(), out var lastBackupDate))
                {
                    if (lastBackupDate.Date == today)
                    {
                        _logger.LogInformation("Backup already performed today");
                        return false;
                    }
                }
            }

            // Perform backup
            await BackupDatabaseAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during automatic backup");
            return false;
        }
    }

    private async Task UpdateLastBackupDateAsync()
    {
        try
        {
            var backupDir = GetBackupDirectory();
            var lastBackupPath = Path.Combine(backupDir, LAST_BACKUP_FILE);
            File.WriteAllText(lastBackupPath, DateTime.Now.ToString("yyyy-MM-dd"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating last backup date");
        }
    }

    private string GetDatabasePath()
    {
        var connectionString = _configuration.GetConnectionString("DatabaseContext");
        // Parse the connection string to get the database path
        // Format: "Data Source = SelfManagement.db" or "Data Source=SelfManagement.db"
        var dataSource = connectionString?.Split('=').LastOrDefault()?.Trim();

        if (string.IsNullOrEmpty(dataSource))
        {
            throw new InvalidOperationException("Could not parse database path from connection string");
        }

        // If it's a relative path, combine with current directory
        if (!Path.IsPathRooted(dataSource))
        {
            dataSource = Path.Combine(Directory.GetCurrentDirectory(), dataSource);
        }

        return dataSource;
    }
}
