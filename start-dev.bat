@echo off
echo ========================================
echo Starting News API Platform
echo ========================================
echo.

echo [1/3] Starting Backend API...
start "News API Backend" cmd /k "cd newsApi && dotnet run"

timeout /t 5 /nobreak > nul

echo [2/3] Starting Frontend...
start "News Frontend" cmd /k "cd web && npm run dev"

echo.
echo ========================================
echo Both servers are starting...
echo ========================================
echo.
echo Backend:  http://localhost:5000/swagger
echo Frontend: http://localhost:3000
echo.
echo Press Ctrl+C in each window to stop
echo ========================================
