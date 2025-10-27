# ============================================
# News API - Clean Docker Resources
# ============================================

param(
    [switch]$Force
)

Write-Host "======================================" -ForegroundColor Red
Write-Host "   News API - Docker Cleanup" -ForegroundColor Red
Write-Host "======================================" -ForegroundColor Red
Write-Host ""

Write-Host "WARNING: This will remove:" -ForegroundColor Yellow
Write-Host "  - All containers" -ForegroundColor White
Write-Host "  - All volumes (DATABASE DATA WILL BE LOST)" -ForegroundColor White
Write-Host "  - All networks" -ForegroundColor White
Write-Host "  - All images built by docker-compose" -ForegroundColor White
Write-Host ""

if (-not $Force) {
    $confirmation = Read-Host "Are you absolutely sure? Type 'DELETE' to confirm"
    
    if ($confirmation -ne "DELETE") {
        Write-Host ""
        Write-Host "Cancelled. No changes made." -ForegroundColor Green
        exit 0
    }
}

Write-Host ""
Write-Host "Cleaning up Docker resources..." -ForegroundColor Yellow
Write-Host ""

# Stop and remove containers, networks, volumes
Write-Host "Removing containers and volumes..." -ForegroundColor Yellow
docker-compose down -v --remove-orphans

# Remove images
Write-Host "Removing images..." -ForegroundColor Yellow
docker-compose down --rmi local

# Prune system
Write-Host "Pruning unused resources..." -ForegroundColor Yellow
docker system prune -f

Write-Host ""
Write-Host "======================================" -ForegroundColor Green
Write-Host "   Cleanup Complete!" -ForegroundColor Green
Write-Host "======================================" -ForegroundColor Green
Write-Host ""
Write-Host "To start fresh, run:" -ForegroundColor Cyan
Write-Host "  .\docker-start.ps1 -Build" -ForegroundColor White
Write-Host ""
