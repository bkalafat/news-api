#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Deploy frontend to Vercel

.DESCRIPTION
    Simple deployment script for Next.js frontend to Vercel.
    Vercel CLI will be installed if not present.

.PARAMETER Production
    Deploy to production (default: preview deployment)

.EXAMPLE
    .\deploy-frontend.ps1
    Deploy to preview environment

.EXAMPLE
    .\deploy-frontend.ps1 -Production
    Deploy to production environment
#>

param(
    [switch]$Production
)

Write-Host "🚀 Frontend Deployment to Vercel" -ForegroundColor Cyan
Write-Host "=================================" -ForegroundColor Cyan
Write-Host ""

# Change to frontend directory
Set-Location -Path "$PSScriptRoot\frontend"

# Check if Vercel CLI is installed
if (-not (Get-Command vercel -ErrorAction SilentlyContinue)) {
    Write-Host "📦 Installing Vercel CLI..." -ForegroundColor Yellow
    npm install -g vercel
}

# Build the project
Write-Host "🔨 Building Next.js application..." -ForegroundColor Yellow
npm run build

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Build failed!" -ForegroundColor Red
    exit 1
}

Write-Host "✅ Build completed successfully" -ForegroundColor Green
Write-Host ""

# Deploy to Vercel
Write-Host "☁️  Deploying to Vercel..." -ForegroundColor Yellow

if ($Production) {
    Write-Host "🎯 Production deployment" -ForegroundColor Magenta
    vercel --prod
} else {
    Write-Host "🔍 Preview deployment" -ForegroundColor Magenta
    vercel
}

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "✅ Deployment completed successfully!" -ForegroundColor Green
    Write-Host ""
    Write-Host "📝 Next steps:" -ForegroundColor Cyan
    Write-Host "   - Check deployment URL in the output above"
    Write-Host "   - Verify frontend is working correctly"
    Write-Host "   - Test API connectivity"
    Write-Host ""
    Write-Host "💡 Tips:" -ForegroundColor Yellow
    Write-Host "   - For production: .\deploy-frontend.ps1 -Production"
    Write-Host "   - View deployments: vercel ls"
    Write-Host "   - View logs: vercel logs"
} else {
    Write-Host ""
    Write-Host "❌ Deployment failed!" -ForegroundColor Red
    exit 1
}
