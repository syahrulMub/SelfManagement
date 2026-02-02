@echo off
REM Automated Daily Backup Scheduler - SelfManagement Application
REM This script can be scheduled to run daily via Windows Task Scheduler

setlocal enabledelayedexpansion

echo.
echo ====================================================
echo    Automated Backup Scheduler - SelfManagement
echo ====================================================
echo Started at: %date% %time%
echo.

REM Get the directory where this script is located
set SCRIPT_DIR=%~dp0
set DB_FILE=%SCRIPT_DIR%SelfManagement.db
set BACKUP_DIR=%SCRIPT_DIR%Backups
set LAST_BACKUP_FILE=%BACKUP_DIR%\LastBackupDate.txt
set TIMESTAMP=%date:~10,4%-%date:~4,2%-%date:~7,2%_%time:~0,2%-%time:~3,2%-%time:~6,2%
set TIMESTAMP=%TIMESTAMP: =0%
set BACKUP_FILE=%BACKUP_DIR%\SelfManagement_Backup_%TIMESTAMP%.db
set LOG_FILE=%BACKUP_DIR%\backup.log

REM Create backup directory if it doesn't exist
if not exist "%BACKUP_DIR%" (
    mkdir "%BACKUP_DIR%"
)

REM Check if database file exists
if not exist "%DB_FILE%" (
    echo [ERROR] Database file not found: %DB_FILE% >> "%LOG_FILE%"
    exit /b 1
)

REM Check if backup already done today
if exist "%LAST_BACKUP_FILE%" (
    for /f "delims=" %%A in ('type "%LAST_BACKUP_FILE%"') do (
        if "%%A"=="%date%" (
            echo [INFO] Backup already performed today. Skipping. >> "%LOG_FILE%"
            exit /b 0
        )
    )
)

REM Perform backup
echo [INFO] Performing automatic backup at %date% %time% >> "%LOG_FILE%"
copy "%DB_FILE%" "%BACKUP_FILE%"

if %errorlevel% equ 0 (
    echo [SUCCESS] Backup created: %BACKUP_FILE% >> "%LOG_FILE%"
    echo %date% > "%LAST_BACKUP_FILE%"
    
    REM Get file size
    for %%A in ("%BACKUP_FILE%") do (
        set FILE_SIZE=%%~zA
        echo [INFO] Backup size: !FILE_SIZE! bytes >> "%LOG_FILE%"
    )
) else (
    echo [ERROR] Backup failed! Error code: %errorlevel% >> "%LOG_FILE%"
)

echo [INFO] Backup process completed at %date% %time% >> "%LOG_FILE%"
echo. >> "%LOG_FILE%"

exit /b %errorlevel%
