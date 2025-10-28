# Quick Start: Self-Host NewsPortal with Cloudflare Tunnel
# Total time: 5 minutes
# Cost: FREE

# Step 1: Download cloudflared
Write-Host "Downloading Cloudflare Tunnel..." -ForegroundColor Cyan
New-Item -ItemType Directory -Force -Path C:\cloudflared | Out-Null
Invoke-WebRequest -Uri "https://github.com/cloudflare/cloudflared/releases/latest/download/cloudflared-windows-amd64.exe" -OutFile "C:\cloudflared\cloudflared.exe"

Write-Host "✅ Downloaded!" -ForegroundColor Green

# Step 2: Start your Docker containers
Write-Host "`nStarting Docker containers..." -ForegroundColor Cyan
Set-Location C:\dev\newsportal
docker-compose up -d

Write-Host "✅ Docker running!" -ForegroundColor Green

# Step 3: Create instant public tunnel (no account needed!)
Write-Host "`nCreating public tunnel..." -ForegroundColor Cyan
Write-Host "⚠️ Keep this window open!" -ForegroundColor Yellow
Write-Host "`nYour public URL will appear below:" -ForegroundColor Green
Write-Host "================================================" -ForegroundColor Cyan

C:\cloudflared\cloudflared.exe tunnel --url http://localhost:5000

# URL will be shown like: https://random-words-123.trycloudflare.com
# Share this URL for testing!
# When you close this window, the tunnel stops.
