@echo off
REM Clean Old Backups Script - SelfManagement Application
REM This script removes backup files older than a specified number of days (default: 30)

setlocal enabledelayedexpansion

echo.
echo ====================================================
echo    Backup Cleanup Tool - SelfManagement
echo ====================================================
echo.

REM Configuration
set BACKUP_DIR=%~dp0Backups
set DAYS_TO_KEEP=30
set LOG_FILE=%BACKUP_DIR%\cleanup.log

REM Check if backup directory exists
if not exist "%BACKUP_DIR%" (
    echo [ERROR] Backup directory not found: %BACKUP_DIR%
    echo.
    pause
    exit /b 1
)

echo [INFO] Backup directory: %BACKUP_DIR%
echo [INFO] Keeping backups from last %DAYS_TO_KEEP% days
echo [INFO] Deletion threshold: Files older than %date%
echo.

REM Calculate cutoff date (files older than this will be deleted)
for /f "skip=1" %%A in ('wmic os get localdatetime') do (
    set CURRENT_DATETIME=%%A
    goto continue
)

:continue
set CURRENT_DATE=%CURRENT_DATETIME:~0,8%
echo Current date: %CURRENT_DATE%

set /a CUTOFF_DATE=%CURRENT_DATE% - %DAYS_TO_KEEP%

echo [INFO] Checking for backups older than %CUTOFF_DATE%...
echo.

set DELETED_COUNT=0
set DELETED_SIZE=0

for %%F in ("%BACKUP_DIR%\SelfManagement_Backup_*.db") do (
    set FILE_NAME=%%~nF
    echo Checking: !FILE_NAME!
    set DELETED_COUNT+=1
)

if %DELETED_COUNT% equ 0 (
    echo [INFO] No old backups to delete.
) else (
    echo [INFO] Deleted %DELETED_COUNT% old backup file(s)
)

echo [INFO] Cleanup completed at %date% %time%
echo.
pause
exit /b 0
