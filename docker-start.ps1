# ============================================
# News API - Start Docker Services
# ============================================

param(
    [switch]$Production,
    [switch]$Build
)

Write-Host "======================================" -ForegroundColor Cyan
Write-Host "   News API - Docker Startup" -ForegroundColor Cyan
Write-Host "======================================" -ForegroundColor Cyan
Write-Host ""

# Check if Docker is running
Write-Host "Checking Docker status..." -ForegroundColor Yellow
docker info > $null 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: Docker is not running!" -ForegroundColor Red
    Write-Host "Please start Docker Desktop and try again." -ForegroundColor Yellow
    exit 1
}
Write-Host "✓ Docker is running" -ForegroundColor Green
Write-Host ""

# Check if .env file exists
if (-not (Test-Path ".env")) {
    Write-Host "WARNING: .env file not found!" -ForegroundColor Yellow
    Write-Host "Creating .env from .env.example..." -ForegroundColor Yellow
    Copy-Item ".env.example" ".env"
    Write-Host "✓ Created .env file. Please review and update values." -ForegroundColor Green
    Write-Host ""
}

# Determine environment
$envFile = if ($Production) { "docker-compose.prod.yml" } else { "docker-compose.dev.yml" }
$envName = if ($Production) { "Production" } else { "Development" }

Write-Host "Starting services in $envName mode..." -ForegroundColor Cyan
Write-Host ""

# Build if requested
if ($Build) {
    Write-Host "Building Docker images..." -ForegroundColor Yellow
    docker-compose -f docker-compose.yml -f $envFile build --pull
    if ($LASTEXITCODE -ne 0) {
        Write-Host "ERROR: Build failed!" -ForegroundColor Red
        exit 1
    }
    Write-Host "✓ Build complete" -ForegroundColor Green
    Write-Host ""
}

# Start services
Write-Host "Starting Docker Compose services..." -ForegroundColor Yellow
docker-compose -f docker-compose.yml -f $envFile up -d

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "======================================" -ForegroundColor Green
    Write-Host "   Services Started Successfully!" -ForegroundColor Green
    Write-Host "======================================" -ForegroundColor Green
    Write-Host ""
    
    # Wait for services to initialize
    Write-Host "Waiting for services to initialize..." -ForegroundColor Yellow
    Start-Sleep -Seconds 5
    Write-Host ""
    
    # Display service URLs
    Write-Host "Service URLs:" -ForegroundColor Cyan
    Write-Host "  News API:        http://localhost:5000" -ForegroundColor White
    Write-Host "  Swagger UI:      http://localhost:5000/swagger" -ForegroundColor White
    Write-Host "  MinIO Console:   http://localhost:9001" -ForegroundColor White
    Write-Host "  Mongo Express:   http://localhost:8081" -ForegroundColor White
    if (-not $Production) {
        Write-Host "  Redis:           localhost:6379" -ForegroundColor White
    }
    Write-Host ""
    
    # Display credentials
    Write-Host "Default Credentials:" -ForegroundColor Cyan
    Write-Host "  MinIO:     minioadmin / minioadmin123" -ForegroundColor White
    Write-Host "  MongoDB:   admin / password123" -ForegroundColor White
    if (-not $Production) {
        Write-Host "  Mongo Exp: admin / admin123" -ForegroundColor White
    }
    Write-Host ""
    
    # Display useful commands
    Write-Host "Useful Commands:" -ForegroundColor Cyan
    Write-Host "  View logs:       .\docker-logs.ps1" -ForegroundColor White
    Write-Host "  Stop services:   .\docker-stop.ps1" -ForegroundColor White
    Write-Host "  Rebuild:         .\docker-rebuild.ps1" -ForegroundColor White
    Write-Host "  Clean all:       .\docker-clean.ps1" -ForegroundColor White
    Write-Host ""
    
} else {
    Write-Host ""
    Write-Host "ERROR: Failed to start services!" -ForegroundColor Red
    Write-Host "Run '.\docker-logs.ps1' to view error details." -ForegroundColor Yellow
    exit 1
}
