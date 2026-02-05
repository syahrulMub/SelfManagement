using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyPrivateManager.IDatabaseServices;

namespace MyPrivateManager.Controllers;

[Authorize]
public class BackupController : Controller
{
    private readonly IBackupService _backupService;
    private readonly ILogger<BackupController> _logger;

    public BackupController(IBackupService backupService, ILogger<BackupController> logger)
    {
        _backupService = backupService;
        _logger = logger;
    }

    // GET: Backup/Index - List all backups
    public async Task<IActionResult> Index()
    {
        try
        {
            var backups = await _backupService.GetBackupListAsync();
            return View(backups);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading backup list");
            TempData["Error"] = "Error loading backup list: " + ex.Message;
            return View(new List<BackupInfo>());
        }
    }

    // POST: Backup/CreateBackup - Create a new backup
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateBackup()
    {
        try
        {
            var backupFileName = await _backupService.BackupDatabaseAsync();
            TempData["Success"] = $"Database backed up successfully: {backupFileName}";
            _logger.LogInformation("Backup created by user: {BackupFile}", backupFileName);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating backup");
            TempData["Error"] = "Error creating backup: " + ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: Backup/Restore - Restore from a backup
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Restore(string backupFileName)
    {
        try
        {
            if (string.IsNullOrEmpty(backupFileName))
            {
                TempData["Error"] = "Backup file name is required";
                return RedirectToAction(nameof(Index));
            }

            // Show confirmation page
            ViewBag.BackupFileName = backupFileName;
            return View();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Restore GET");
            TempData["Error"] = "Error: " + ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: Backup/ConfirmRestore - Confirm and execute restore
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmRestore(string backupFileName)
    {
        try
        {
            if (string.IsNullOrEmpty(backupFileName))
            {
                TempData["Error"] = "Backup file name is required";
                return RedirectToAction(nameof(Index));
            }

            var result = await _backupService.RestoreDatabaseAsync(backupFileName);

            if (result)
            {
                TempData["Success"] = $"Database restored successfully from: {backupFileName}. Please refresh the application.";
                _logger.LogInformation("Database restored by user from: {BackupFile}", backupFileName);
            }
            else
            {
                TempData["Error"] = "Error restoring database";
            }

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error restoring from backup: {BackupFile}", backupFileName);
            TempData["Error"] = "Error restoring database: " + ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: Backup/Delete - Delete a backup
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string backupFileName)
    {
        try
        {
            if (string.IsNullOrEmpty(backupFileName))
            {
                TempData["Error"] = "Backup file name is required";
                return RedirectToAction(nameof(Index));
            }

            var result = await _backupService.DeleteBackupAsync(backupFileName);

            if (result)
            {
                TempData["Success"] = $"Backup deleted successfully: {backupFileName}";
                _logger.LogInformation("Backup deleted by user: {BackupFile}", backupFileName);
            }
            else
            {
                TempData["Error"] = "Error deleting backup";
            }

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting backup: {BackupFile}", backupFileName);
            TempData["Error"] = "Error deleting backup: " + ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }

    // GET: Backup/GetBackupListJson - For AJAX/API calls
    [HttpGet]
    public async Task<IActionResult> GetBackupListJson()
    {
        try
        {
            var backups = await _backupService.GetBackupListAsync();
            return Json(new { success = true, data = backups });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting backup list");
            return Json(new { success = false, message = ex.Message });
        }
    }

    // GET: Backup/DownloadBackup - Download a backup file
    [HttpGet]
    public async Task<IActionResult> DownloadBackup(string filename)
    {
        try
        {
            if (string.IsNullOrEmpty(filename))
            {
                return BadRequest("Filename is required");
            }

            var backupDir = _backupService.GetBackupDirectory();
            var backupPath = Path.Combine(backupDir, filename);

            // Security check: Ensure file is in backup directory
            var fullBackupDir = Path.GetFullPath(backupDir);
            var fullBackupPath = Path.GetFullPath(backupPath);

            if (!fullBackupPath.StartsWith(fullBackupDir))
            {
                _logger.LogWarning("Potential path traversal attack: {RequestedFile}", filename);
                return Forbid("Invalid file path");
            }

            if (!System.IO.File.Exists(backupPath))
            {
                _logger.LogWarning("Backup file not found: {BackupFile}", backupPath);
                return NotFound("Backup file not found");
            }

            var fileBytes = await System.IO.File.ReadAllBytesAsync(backupPath);
            _logger.LogInformation("Backup downloaded by user: {BackupFile}", filename);

            return File(fileBytes, "application/octet-stream", filename);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading backup: {Filename}", filename);
            return StatusCode(500, "Error downloading backup: " + ex.Message);
        }
    }
}
