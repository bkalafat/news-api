# ============================================
# News API - Docker Status Check
# ============================================

Write-Host "======================================" -ForegroundColor Cyan
Write-Host "   News API - Docker Status" -ForegroundColor Cyan
Write-Host "======================================" -ForegroundColor Cyan
Write-Host ""

# Check Docker
Write-Host "Docker Engine:" -ForegroundColor Yellow
docker info > $null 2>&1
if ($LASTEXITCODE -eq 0) {
    Write-Host "  ✓ Running" -ForegroundColor Green
} else {
    Write-Host "  ✗ Not running" -ForegroundColor Red
    Write-Host ""
    exit 1
}
Write-Host ""

# Container status
Write-Host "Container Status:" -ForegroundColor Yellow
docker-compose ps
Write-Host ""

# Service health
Write-Host "Service Health Checks:" -ForegroundColor Yellow
$containers = docker-compose ps --format json | ConvertFrom-Json

foreach ($container in $containers) {
    $health = docker inspect --format='{{.State.Health.Status}}' $container.Name 2>$null
    $status = docker inspect --format='{{.State.Status}}' $container.Name 2>$null
    
    Write-Host "  $($container.Service):" -NoNewline -ForegroundColor White
    
    if ($status -eq "running") {
        if ($health -eq "healthy") {
            Write-Host " ✓ Healthy" -ForegroundColor Green
        } elseif ($health -eq "unhealthy") {
            Write-Host " ✗ Unhealthy" -ForegroundColor Red
        } elseif ($health -eq "starting") {
            Write-Host " ⟳ Starting" -ForegroundColor Yellow
        } else {
            Write-Host " ● Running (no health check)" -ForegroundColor Cyan
        }
    } else {
        Write-Host " ✗ $status" -ForegroundColor Red
    }
}
Write-Host ""

# Disk usage
Write-Host "Disk Usage:" -ForegroundColor Yellow
docker system df
Write-Host ""

# Network info
Write-Host "Networks:" -ForegroundColor Yellow
docker network ls --filter "name=news-api" --format "table {{.Name}}\t{{.Driver}}\t{{.Scope}}"
Write-Host ""

# Volume info
Write-Host "Volumes:" -ForegroundColor Yellow
docker volume ls --filter "name=newsapi" --format "table {{.Name}}\t{{.Driver}}\t{{.Size}}"
Write-Host ""
