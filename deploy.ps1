#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Deploy News Portal Backend with Docker Compose

.DESCRIPTION
    One-command deployment script that builds and deploys the entire News Portal
    backend stack including MongoDB, MinIO, and the .NET API.

.PARAMETER Clean
    Clean rebuild - removes all containers and volumes before deploying

.PARAMETER Stop
    Stop all running containers

.PARAMETER Logs
    Show logs from all services

.PARAMETER Status
    Show status of all services

.EXAMPLE
    .\deploy.ps1
    Deploys the backend (builds if needed, updates if running)

.EXAMPLE
    .\deploy.ps1 -Clean
    Clean deployment - removes everything and redeploys from scratch

.EXAMPLE
    .\deploy.ps1 -Stop
    Stops all services

.EXAMPLE
    .\deploy.ps1 -Logs
    Shows logs from all services

.EXAMPLE
    .\deploy.ps1 -Status
    Shows status of all services
#>

param(
    [switch]$Clean,
    [switch]$Stop,
    [switch]$Logs,
    [switch]$Status
)

$ErrorActionPreference = "Stop"

# Colors for output
function Write-Success { Write-Host $args -ForegroundColor Green }
function Write-Info { Write-Host $args -ForegroundColor Cyan }
function Write-Warning { Write-Host $args -ForegroundColor Yellow }
function Write-Error { param($msg) Write-Host $msg -ForegroundColor Red }

# Banner
Write-Host ""
Write-Host "╔════════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "║    📰 News Portal Deployment Script   ║" -ForegroundColor Cyan
Write-Host "╚════════════════════════════════════════╝" -ForegroundColor Cyan
Write-Host ""

# Check if Docker is running
try {
    docker info | Out-Null
} catch {
    Write-Error "❌ Docker is not running. Please start Docker Desktop and try again."
    exit 1
}

# Handle different operations
if ($Stop) {
    Write-Info "⏹️  Stopping all services..."
    docker compose down
    Write-Success "✅ All services stopped"
    exit 0
}

if ($Status) {
    Write-Info "📊 Service Status:"
    Write-Host ""
    docker compose ps
    Write-Host ""
    Write-Info "🔍 Health Check:"
    try {
        $response = Invoke-WebRequest -Uri "http://localhost:5000/health" -UseBasicParsing -TimeoutSec 5
        Write-Success "✅ Backend API is healthy (Status: $($response.StatusCode))"
    } catch {
        Write-Warning "⚠️  Backend API is not responding"
    }
    exit 0
}

if ($Logs) {
    Write-Info "📋 Showing logs (Ctrl+C to exit)..."
    docker compose logs -f
    exit 0
}

if ($Clean) {
    Write-Warning "🧹 Clean deployment requested - this will remove ALL data!"
    $confirm = Read-Host "Are you sure? Type 'yes' to continue"
    if ($confirm -ne "yes") {
        Write-Info "Cancelled."
        exit 0
    }
    
    Write-Info "🗑️  Removing all containers and volumes..."
    docker compose down -v
    Write-Success "✅ Cleanup complete"
    Write-Host ""
}

# Main deployment
Write-Info "🚀 Starting deployment..."
Write-Host ""

# Create .env file if it doesn't exist
if (-not (Test-Path ".env")) {
    Write-Info "📝 Creating .env file from template..."
    if (Test-Path ".env.example") {
        Copy-Item ".env.example" ".env"
        Write-Success "✅ .env file created"
    } else {
        Write-Warning "⚠️  .env.example not found - using default values"
    }
}

# Build and start services
Write-Info "🔨 Building and starting services..."
Write-Host ""
docker compose up -d --build

if ($LASTEXITCODE -ne 0) {
    Write-Error "❌ Deployment failed! Check the error messages above."
    exit 1
}

Write-Host ""
Write-Success "✅ Deployment successful!"
Write-Host ""

# Wait for services to be healthy
Write-Info "⏳ Waiting for services to be healthy (this may take 30-60 seconds)..."
$maxAttempts = 20
$attempt = 0
$healthy = $false

while ($attempt -lt $maxAttempts) {
    Start-Sleep -Seconds 3
    $attempt++
    
    try {
        $response = Invoke-WebRequest -Uri "http://localhost:5000/health" -UseBasicParsing -TimeoutSec 2
        if ($response.StatusCode -eq 200) {
            $healthy = $true
            break
        }
    } catch {
        Write-Host "." -NoNewline
    }
}

Write-Host ""
Write-Host ""

if ($healthy) {
    Write-Success "✅ Backend is healthy and ready!"
} else {
    Write-Warning "⚠️  Backend is starting but not responding yet. Check logs with: .\deploy.ps1 -Logs"
}

Write-Host ""
Write-Host "╔════════════════════════════════════════════════════════════════╗" -ForegroundColor Green
Write-Host "║                    🎉 SERVICES RUNNING                         ║" -ForegroundColor Green
Write-Host "╚════════════════════════════════════════════════════════════════╝" -ForegroundColor Green
Write-Host ""
Write-Host "  🔥 Backend API:          " -NoNewline; Write-Host "http://localhost:5000" -ForegroundColor Cyan
Write-Host "  📖 API Documentation:    " -NoNewline; Write-Host "http://localhost:5000/swagger" -ForegroundColor Cyan
Write-Host "  🗄️  MongoDB Admin:        " -NoNewline; Write-Host "http://localhost:8081" -ForegroundColor Cyan
Write-Host "     └─ Credentials:       " -NoNewline; Write-Host "admin / admin123" -ForegroundColor Yellow
Write-Host "  📦 MinIO Console:        " -NoNewline; Write-Host "http://localhost:9001" -ForegroundColor Cyan
Write-Host "     └─ Credentials:       " -NoNewline; Write-Host "minioadmin / minioadmin123" -ForegroundColor Yellow
Write-Host ""
Write-Host "╔════════════════════════════════════════════════════════════════╗" -ForegroundColor Blue
Write-Host "║                    QUICK COMMANDS                              ║" -ForegroundColor Blue
Write-Host "╚════════════════════════════════════════════════════════════════╝" -ForegroundColor Blue
Write-Host ""
Write-Host "  .\deploy.ps1          " -NoNewline; Write-Host "- Deploy/update services" -ForegroundColor Gray
Write-Host "  .\deploy.ps1 -Clean   " -NoNewline; Write-Host "- Clean rebuild (removes all data)" -ForegroundColor Gray
Write-Host "  .\deploy.ps1 -Stop    " -NoNewline; Write-Host "- Stop all services" -ForegroundColor Gray
Write-Host "  .\deploy.ps1 -Status  " -NoNewline; Write-Host "- Check service status" -ForegroundColor Gray
Write-Host "  .\deploy.ps1 -Logs    " -NoNewline; Write-Host "- View logs" -ForegroundColor Gray
Write-Host ""
Write-Host "  docker compose ps     " -NoNewline; Write-Host "- List containers" -ForegroundColor Gray
Write-Host "  docker compose logs -f newsportal-backend" -NoNewline; Write-Host " - Follow backend logs" -ForegroundColor Gray
Write-Host ""
