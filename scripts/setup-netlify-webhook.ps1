#!/usr/bin/env pwsh

<#
.SYNOPSIS
    Setup Netlify Build Hook for ISR Revalidation
.DESCRIPTION
    This script helps you create a Netlify build hook and configure it in the backend
#>

Write-Host "`n🔧 Netlify ISR Revalidation Setup" -ForegroundColor Cyan
Write-Host "═══════════════════════════════════════════════════════════`n" -ForegroundColor Gray

Write-Host "📋 STEP 1: Create Build Hook in Netlify" -ForegroundColor Yellow
Write-Host ""
Write-Host "1. Go to: https://app.netlify.com/sites/teknohaber/settings/deploys#build-hooks" -ForegroundColor White
Write-Host "2. Click 'Add build hook'" -ForegroundColor White
Write-Host "3. Name: 'ISR Revalidation (Auto)'" -ForegroundColor White
Write-Host "4. Branch: 'master'" -ForegroundColor White
Write-Host "5. Copy the generated webhook URL" -ForegroundColor White
Write-Host ""

# Open Netlify settings
Start-Process "https://app.netlify.com/sites/clever-speculoos-aacb3a/settings/deploys#build-hooks"

Write-Host "`n📋 STEP 2: Configure Backend" -ForegroundColor Yellow
Write-Host ""
$webhookUrl = Read-Host "Paste your Netlify build hook URL here"

if ([string]::IsNullOrEmpty($webhookUrl)) {
    Write-Host "`n❌ No URL provided. Exiting..." -ForegroundColor Red
    exit 1
}

# Update appsettings.json
$appsettingsPath = "$PSScriptRoot\..\backend\appsettings.json"
$appsettings = Get-Content $appsettingsPath -Raw | ConvertFrom-Json

if (-not $appsettings.NetlifySettings) {
    $appsettings | Add-Member -MemberType NoteProperty -Name "NetlifySettings" -Value @{}
}

$appsettings.NetlifySettings = @{
    RevalidateWebhookUrl = $webhookUrl
    EnableAutoRevalidation = $true
}

$appsettings | ConvertTo-Json -Depth 10 | Set-Content $appsettingsPath

Write-Host "`n✅ appsettings.json updated!" -ForegroundColor Green

Write-Host "`n📋 STEP 3: Update Azure Environment Variables" -ForegroundColor Yellow
Write-Host ""
Write-Host "Run this command to update Azure Container Apps:" -ForegroundColor White
Write-Host ""
Write-Host "az containerapp update \" -ForegroundColor Gray
Write-Host "  --name newsportal-backend \" -ForegroundColor Gray
Write-Host "  --resource-group newsportal-rg \" -ForegroundColor Gray
Write-Host "  --set-env-vars \\" -ForegroundColor Gray
Write-Host "    `"NetlifySettings__RevalidateWebhookUrl=$webhookUrl`" \\" -ForegroundColor Gray
Write-Host "    `"NetlifySettings__EnableAutoRevalidation=true`"" -ForegroundColor Gray
Write-Host ""

Write-Host "`n📋 STEP 4: Commit & Deploy" -ForegroundColor Yellow
Write-Host ""
Write-Host "git add backend/appsettings.json" -ForegroundColor Gray
Write-Host "git commit -m `"feat: Add Netlify ISR auto-revalidation after Reddit fetch`"" -ForegroundColor Gray
Write-Host "git push origin master" -ForegroundColor Gray
Write-Host ""

Write-Host "`n🎯 HOW IT WORKS:" -ForegroundColor Cyan
Write-Host "  1. Reddit news fetched (manual or scheduled at 5 AM)" -ForegroundColor White
Write-Host "  2. Backend automatically triggers Netlify build hook" -ForegroundColor White
Write-Host "  3. Netlify rebuilds and deploys (ISR cache cleared)" -ForegroundColor White
Write-Host "  4. Fresh content visible on frontend within 2-3 minutes" -ForegroundColor White
Write-Host ""

Write-Host "═══════════════════════════════════════════════════════════`n" -ForegroundColor Gray
Write-Host "✅ Setup Complete!" -ForegroundColor Green
Write-Host ""
