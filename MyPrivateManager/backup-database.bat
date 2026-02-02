@echo off
REM Database Backup Script for SelfManagement Application
REM This script creates a manual backup of the SQLite database

setlocal enabledelayedexpansion

echo.
echo ====================================================
echo    Database Backup Tool - SelfManagement
echo ====================================================
echo.

REM Get the directory where this script is located
set SCRIPT_DIR=%~dp0
set DB_FILE=%SCRIPT_DIR%SelfManagement.db
set BACKUP_DIR=%SCRIPT_DIR%Backups
set TIMESTAMP=%date:~10,4%-%date:~4,2%-%date:~7,2%_%time:~0,2%-%time:~3,2%-%time:~6,2%
set TIMESTAMP=%TIMESTAMP: =0%
set BACKUP_FILE=%BACKUP_DIR%\SelfManagement_Backup_%TIMESTAMP%.db

REM Create backup directory if it doesn't exist
if not exist "%BACKUP_DIR%" (
    mkdir "%BACKUP_DIR%"
    echo [INFO] Created backup directory: %BACKUP_DIR%
)

REM Check if database file exists
if not exist "%DB_FILE%" (
    echo [ERROR] Database file not found: %DB_FILE%
    echo.
    pause
    exit /b 1
)

REM Perform backup
echo [INFO] Database file: %DB_FILE%
echo [INFO] Backup location: %BACKUP_FILE%
echo [INFO] Creating backup...
echo.

copy "%DB_FILE%" "%BACKUP_FILE%"

if %errorlevel% equ 0 (
    echo.
    echo [SUCCESS] Database backup created successfully!
    echo [INFO] Backup file: %BACKUP_FILE%
    
    REM Get file size
    for %%A in ("%BACKUP_FILE%") do (
        set FILE_SIZE=%%~zA
        echo [INFO] Backup size: !FILE_SIZE! bytes
    )
    
    REM Update last backup date
    echo %date% > "%BACKUP_DIR%\LastBackupDate.txt"
    
) else (
    echo.
    echo [ERROR] Backup failed! Error code: %errorlevel%
)

echo.
pause
exit /b %errorlevel%
