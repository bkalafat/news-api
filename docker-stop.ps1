# ============================================
# News API - Stop Docker Services
# ============================================

param(
    [switch]$RemoveVolumes
)

Write-Host "======================================" -ForegroundColor Cyan
Write-Host "   News API - Docker Shutdown" -ForegroundColor Cyan
Write-Host "======================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "Stopping Docker Compose services..." -ForegroundColor Yellow

if ($RemoveVolumes) {
    Write-Host "WARNING: This will remove all volumes (database data will be lost)!" -ForegroundColor Red
    $confirmation = Read-Host "Are you sure? (yes/no)"
    
    if ($confirmation -eq "yes") {
        docker-compose down -v
        Write-Host ""
        Write-Host "✓ Services stopped and volumes removed" -ForegroundColor Green
    } else {
        Write-Host "Cancelled. No changes made." -ForegroundColor Yellow
        exit 0
    }
} else {
    docker-compose down
    Write-Host ""
    Write-Host "✓ Services stopped (volumes preserved)" -ForegroundColor Green
}

Write-Host ""
Write-Host "To remove volumes later, run:" -ForegroundColor Cyan
Write-Host "  .\docker-stop.ps1 -RemoveVolumes" -ForegroundColor White
Write-Host ""
