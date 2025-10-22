@echo off
REM Development startup script for News API

echo ========================================
echo Starting News API Development Environment
echo ========================================
echo.

REM Check if MongoDB is running
echo Checking MongoDB...
tasklist /FI "IMAGENAME eq mongod.exe" 2>NUL | find /I /N "mongod.exe">NUL
if "%ERRORLEVEL%"=="0" (
    echo [OK] MongoDB is running
) else (
    echo [WARNING] MongoDB is not running. Start it manually or use Docker.
)
echo.

REM Start Backend API
echo Starting Backend API...
start "Backend API" cmd /k "cd /d %~dp0..\newsApi && dotnet run"
timeout /t 3 /nobreak >nul

REM Start Frontend
echo Starting Frontend...
start "Frontend" cmd /k "cd /d %~dp0..\frontend && npm run dev"

echo.
echo ========================================
echo Development servers starting...
echo Backend:  http://localhost:5000
echo Frontend: http://localhost:3000
echo Swagger:  http://localhost:5000/swagger
echo ========================================
