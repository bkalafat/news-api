#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Build frontend for Netlify deployment

.DESCRIPTION
    Build Next.js frontend. Deploy happens automatically via Git push to master.
    Netlify is configured to auto-deploy from the master branch.

.EXAMPLE
    .\deploy-frontend.ps1
    Build frontend and show deployment instructions
#>

Write-Host "🚀 Frontend Build for Netlify" -ForegroundColor Cyan
Write-Host "=============================" -ForegroundColor Cyan
Write-Host ""

# Change to frontend directory
Set-Location -Path "$PSScriptRoot\frontend"

# Build the project
Write-Host "🔨 Building Next.js application..." -ForegroundColor Yellow
npm run build

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Build failed!" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "✅ Build completed successfully!" -ForegroundColor Green
Write-Host ""
Write-Host "📝 Deployment Instructions:" -ForegroundColor Cyan
Write-Host "   1. Commit your changes: git add . && git commit -m 'Update frontend'" -ForegroundColor White
Write-Host "   2. Push to master: git push origin master" -ForegroundColor White
Write-Host "   3. Netlify will auto-deploy (usually takes 2-3 minutes)" -ForegroundColor White
Write-Host ""
Write-Host "🌐 Live Site: https://clever-speculoos-aacb3a.netlify.app" -ForegroundColor Green
Write-Host "📊 Netlify Dashboard: https://app.netlify.com" -ForegroundColor Cyan
Write-Host ""
