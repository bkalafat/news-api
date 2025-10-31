#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Deploy frontend to Netlify

.DESCRIPTION
    Deployment script for Next.js frontend to Netlify.
    Netlify CLI will be installed if not present.

.PARAMETER Production
    Deploy to production (default: preview deployment)

.EXAMPLE
    .\deploy-netlify.ps1
    Deploy to preview environment

.EXAMPLE
    .\deploy-netlify.ps1 -Production
    Deploy to production environment
#>

param(
    [switch]$Production
)

Write-Host "🚀 Frontend Deployment to Netlify" -ForegroundColor Cyan
Write-Host "==================================" -ForegroundColor Cyan
Write-Host ""

# Change to frontend directory
Push-Location -Path "$PSScriptRoot\frontend"

try {
    # Check if Netlify CLI is installed
    if (-not (Get-Command netlify -ErrorAction SilentlyContinue)) {
        Write-Host "📦 Installing Netlify CLI..." -ForegroundColor Yellow
        npm install -g netlify-cli
        if ($LASTEXITCODE -ne 0) {
            Write-Host "❌ Failed to install Netlify CLI!" -ForegroundColor Red
            exit 1
        }
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

    # Deploy to Netlify
    Write-Host "☁️  Deploying to Netlify..." -ForegroundColor Yellow

    if ($Production) {
        Write-Host "🎯 Production deployment" -ForegroundColor Magenta
        netlify deploy --prod --dir=.next
    } else {
        Write-Host "🔍 Preview deployment" -ForegroundColor Magenta
        netlify deploy --dir=.next
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
        Write-Host "   - For production: .\deploy-netlify.ps1 -Production"
        Write-Host "   - View deployments: netlify open"
        Write-Host "   - View logs: netlify logs"
    } else {
        Write-Host ""
        Write-Host "❌ Deployment failed!" -ForegroundColor Red
        exit 1
    }
}
finally {
    Pop-Location
}
