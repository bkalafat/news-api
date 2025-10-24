# ============================================
# News API - View Docker Logs
# ============================================

param(
    [string]$Service = "",
    [switch]$Follow,
    [int]$Tail = 100
)

Write-Host "======================================" -ForegroundColor Cyan
Write-Host "   News API - Docker Logs" -ForegroundColor Cyan
Write-Host "======================================" -ForegroundColor Cyan
Write-Host ""

# Available services
$services = @("newsapi", "mongodb", "minio", "mongo-express", "redis")

if ($Service -eq "") {
    Write-Host "Available services:" -ForegroundColor Yellow
    foreach ($svc in $services) {
        Write-Host "  - $svc" -ForegroundColor White
    }
    Write-Host ""
    Write-Host "Usage examples:" -ForegroundColor Cyan
    Write-Host "  .\docker-logs.ps1 newsapi          # View last 100 lines" -ForegroundColor White
    Write-Host "  .\docker-logs.ps1 newsapi -Follow  # Stream logs in real-time" -ForegroundColor White
    Write-Host "  .\docker-logs.ps1 newsapi -Tail 50 # View last 50 lines" -ForegroundColor White
    Write-Host "  .\docker-logs.ps1                  # View all service logs" -ForegroundColor White
    Write-Host ""
    
    # Show all logs by default
    Write-Host "Showing logs for all services (last $Tail lines):" -ForegroundColor Cyan
    docker-compose logs --tail=$Tail
} else {
    # Validate service name
    if ($services -contains $Service) {
        if ($Follow) {
            Write-Host "Streaming logs for '$Service' (Ctrl+C to stop)..." -ForegroundColor Yellow
            docker-compose logs -f --tail=$Tail $Service
        } else {
            Write-Host "Showing last $Tail lines for '$Service':" -ForegroundColor Yellow
            docker-compose logs --tail=$Tail $Service
        }
    } else {
        Write-Host "ERROR: Unknown service '$Service'" -ForegroundColor Red
        Write-Host "Available services: $($services -join ', ')" -ForegroundColor Yellow
        exit 1
    }
}
