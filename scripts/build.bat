@echo off
REM Build script for News API

echo ========================================
echo Building News API
echo ========================================
echo.

REM Build Backend
echo Building Backend...
cd /d %~dp0..\newsApi
dotnet build --configuration Release
if errorlevel 1 (
    echo [ERROR] Backend build failed!
    exit /b 1
)
echo [OK] Backend built successfully
echo.

REM Build Frontend
echo Building Frontend...
cd /d %~dp0..\frontend
call npm run build
if errorlevel 1 (
    echo [ERROR] Frontend build failed!
    exit /b 1
)
echo [OK] Frontend built successfully
echo.

echo ========================================
echo Build completed successfully!
echo ========================================
