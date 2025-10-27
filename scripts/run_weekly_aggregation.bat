@echo off
REM Daily News Aggregation Runner (5 AM Turkey Time)
REM Run this script to start the automatic daily news aggregator

echo ============================================
echo News Portal - Daily Aggregation Scheduler
echo ============================================
echo.

cd /d "%~dp0"

REM Check if Python is installed
python --version >nul 2>&1
if errorlevel 1 (
    echo ERROR: Python is not installed or not in PATH
    echo Please install Python 3.x and try again
    pause
    exit /b 1
)

REM Check if required packages are installed
python -c "import requests, schedule, pytz" >nul 2>&1
if errorlevel 1 (
    echo Installing required packages...
    pip install requests schedule pytz
    echo.
)

echo Starting scheduler...
echo This will run every day at 5:00 AM (Turkey Time)
echo Press Ctrl+C to stop
echo.

REM Run the scheduler
python news_aggregator.py --schedule

pause
