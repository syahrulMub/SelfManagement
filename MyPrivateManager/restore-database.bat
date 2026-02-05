@echo off
REM Database Restore Script for SelfManagement Application
REM This script provides an interactive restore from backup

setlocal enabledelayedexpansion

echo.
echo ====================================================
echo    Database Restore Tool - SelfManagement
echo ====================================================
echo.

REM Get the directory where this script is located
set SCRIPT_DIR=%~dp0
set DB_FILE=%SCRIPT_DIR%SelfManagement.db
set BACKUP_DIR=%SCRIPT_DIR%Backups

REM Check if backup directory exists
if not exist "%BACKUP_DIR%" (
    echo [ERROR] Backup directory not found: %BACKUP_DIR%
    echo [INFO] Please create a backup first using backup-database.bat
    echo.
    pause
    exit /b 1
)

REM List available backups
echo [INFO] Available backups:
echo.

set COUNT=0
for %%F in ("%BACKUP_DIR%\SelfManagement_Backup_*.db") do (
    set /a COUNT+=1
    echo !COUNT!. %%~nF
)

if %COUNT% equ 0 (
    echo [ERROR] No backup files found in: %BACKUP_DIR%
    echo.
    pause
    exit /b 1
)

echo.
echo [WARNING] This will restore your database from a backup.
echo [WARNING] Your current database will be overwritten!
echo.
set /p CHOICE="Enter the backup number to restore (or press Enter to cancel): "

if "%CHOICE%"=="" (
    echo [INFO] Restore cancelled.
    pause
    exit /b 0
)

REM Validate input
if not "%CHOICE%" geq "1" goto invalid_choice
if not "%CHOICE%" leq "%COUNT%" goto invalid_choice

REM Find the selected backup file
set CURRENT=0
for %%F in ("%BACKUP_DIR%\SelfManagement_Backup_*.db") do (
    set /a CURRENT+=1
    if !CURRENT! equ %CHOICE% (
        set SELECTED_BACKUP=%%~nF
        set SELECTED_PATH=%%F
    )
)

echo.
echo [INFO] Selected backup: %SELECTED_BACKUP%
echo.

REM Create safety copy of current database
if exist "%DB_FILE%" (
    set SAFETY_BACKUP=%BACKUP_DIR%\SelfManagement_PreRestore_%date:~10,4%-%date:~4,2%-%date:~7,2%_%time:~0,2%-%time:~3,2%-%time:~6,2%.db
    set SAFETY_BACKUP=%SAFETY_BACKUP: =0%
    echo [INFO] Creating safety backup of current database...
    copy "%DB_FILE%" "%SAFETY_BACKUP%"
    echo [INFO] Safety backup created: %SAFETY_BACKUP%
    echo.
)

REM Confirm restore
set /p CONFIRM="Are you absolutely sure you want to restore? (yes/no): "

if /i not "%CONFIRM%"=="yes" (
    echo [INFO] Restore cancelled.
    pause
    exit /b 0
)

REM Perform restore
echo [INFO] Restoring database...
copy "%SELECTED_PATH%" "%DB_FILE%"

if %errorlevel% equ 0 (
    echo.
    echo [SUCCESS] Database restored successfully!
    echo [INFO] Database file: %DB_FILE%
    echo [INFO] Restored from: %SELECTED_BACKUP%
    echo [INFO] Please restart the application for changes to take effect.
) else (
    echo.
    echo [ERROR] Restore failed! Error code: %errorlevel%
)

echo.
pause
exit /b %errorlevel%

:invalid_choice
echo [ERROR] Invalid choice. Please enter a number between 1 and %COUNT%.
echo.
pause
exit /b 1
