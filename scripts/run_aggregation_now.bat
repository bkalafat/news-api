@echo off
REM One-time News Aggregation Runner
REM Run this script to fetch news once manually

echo ============================================
echo News Portal - Manual News Aggregation
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

echo Running news aggregation from all sources...
echo - Reddit (technology, programming, AI, robotics, phones, etc.)
echo - GitHub Trending repositories
echo - HackerNews top stories
echo - Turkish tech news (Webrazzi, ShiftDelete)
echo.

REM Run once
python news_aggregator.py --once

echo.
echo ============================================
echo Aggregation complete! Check logs above.
echo ============================================
pause
