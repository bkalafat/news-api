# ============================================
# News API - Rebuild Docker Services
# ============================================

param(
    [switch]$Production,
    [switch]$NoCache,
    [string]$Service = ""
)

Write-Host "======================================" -ForegroundColor Cyan
Write-Host "   News API - Docker Rebuild" -ForegroundColor Cyan
Write-Host "======================================" -ForegroundColor Cyan
Write-Host ""

# Determine environment
$envFile = if ($Production) { "docker-compose.prod.yml" } else { "docker-compose.dev.yml" }
$envName = if ($Production) { "Production" } else { "Development" }

Write-Host "Environment: $envName" -ForegroundColor Yellow
if ($Service -ne "") {
    Write-Host "Service: $Service" -ForegroundColor Yellow
}
Write-Host ""

# Stop services
Write-Host "Stopping services..." -ForegroundColor Yellow
docker-compose down
Write-Host "✓ Services stopped" -ForegroundColor Green
Write-Host ""

# Build
Write-Host "Building Docker images..." -ForegroundColor Yellow

$buildArgs = @("docker-compose", "-f", "docker-compose.yml", "-f", $envFile, "build", "--pull")

if ($NoCache) {
    $buildArgs += "--no-cache"
    Write-Host "  Using --no-cache (fresh build)" -ForegroundColor Cyan
}

if ($Service -ne "") {
    $buildArgs += $Service
}

& $buildArgs[0] $buildArgs[1..($buildArgs.Length - 1)]

if ($LASTEXITCODE -ne 0) {
    Write-Host ""
    Write-Host "ERROR: Build failed!" -ForegroundColor Red
    exit 1
}

Write-Host "✓ Build complete" -ForegroundColor Green
Write-Host ""

# Start services
Write-Host "Starting services..." -ForegroundColor Yellow
docker-compose -f docker-compose.yml -f $envFile up -d

if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ Services started successfully" -ForegroundColor Green
    Write-Host ""
    Write-Host "Run '.\docker-logs.ps1 newsportal-backend -Follow' to view logs" -ForegroundColor Cyan
} else {
    Write-Host ""
    Write-Host "ERROR: Failed to start services!" -ForegroundColor Red
    exit 1
}
