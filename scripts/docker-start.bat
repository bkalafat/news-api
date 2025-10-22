@echo off
REM Docker startup script

echo ========================================
echo Starting Docker Containers
echo ========================================
echo.

cd /d %~dp0..\docker

echo Starting MongoDB and MinIO...
docker-compose up -d

if errorlevel 1 (
    echo [ERROR] Failed to start containers!
    exit /b 1
)

echo.
echo Waiting for containers to be ready...
timeout /t 5 /nobreak >nul

echo.
echo ========================================
echo Containers started successfully!
echo MongoDB:     localhost:27017
echo MinIO API:   localhost:9000
echo MinIO UI:    localhost:9001
echo ========================================
echo.
echo Run 'docker-compose ps' to check status
