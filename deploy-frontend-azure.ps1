# Azure Static Web Apps Deployment for News Portal Frontend
# This script creates a Static Web App connected to your GitHub repository

$ErrorActionPreference = "Stop"

Write-Host "🌐 News Portal Frontend - Azure Static Web Apps Deployment" -ForegroundColor Cyan
Write-Host "=========================================================`n" -ForegroundColor Cyan

# Configuration
$resourceGroup = "newsportal-rg"
$location = "westeurope"
$appName = "newsportal-frontend"
$repoUrl = "https://github.com/bkalafat/newsportal"
$branch = "master"

# Get backend URL
Write-Host "📋 Step 1: Get Backend URL" -ForegroundColor Yellow
$backendUrl = az containerapp show `
    --name newsportal-backend `
    --resource-group $resourceGroup `
    --query properties.configuration.ingress.fqdn `
    -o tsv 2>$null

if (-not $backendUrl) {
    Write-Host "⚠️ Warning: Backend not found. Please deploy backend first." -ForegroundColor Yellow
    $backendUrl = Read-Host "Enter Backend URL (or press Enter to skip)"
} else {
    Write-Host "✅ Backend URL: https://$backendUrl`n" -ForegroundColor Green
}

Write-Host "📋 Step 2: Create Static Web App" -ForegroundColor Yellow
Write-Host "Note: This requires GitHub authentication and will create a GitHub Action workflow`n" -ForegroundColor Cyan

# Create Static Web App
$swa = az staticwebapp create `
    --name $appName `
    --resource-group $resourceGroup `
    --source $repoUrl `
    --location $location `
    --branch $branch `
    --app-location "/frontend" `
    --output-location ".next" `
    --sku Free `
    --login-with-github 2>&1

if ($LASTEXITCODE -ne 0) {
    Write-Host "`n⚠️ Automated creation failed. Please create manually via Azure Portal:" -ForegroundColor Yellow
    Write-Host "`n📋 Manual Creation Steps:" -ForegroundColor Cyan
    Write-Host "1. Go to: https://portal.azure.com" -ForegroundColor White
    Write-Host "2. Click 'Create a resource' → Search 'Static Web App'" -ForegroundColor White
    Write-Host "3. Configuration:" -ForegroundColor White
    Write-Host "   Basics:" -ForegroundColor Yellow
    Write-Host "   - Subscription: Azure subscription 1" -ForegroundColor White
    Write-Host "   - Resource Group: $resourceGroup" -ForegroundColor White
    Write-Host "   - Name: $appName" -ForegroundColor White
    Write-Host "   - Plan type: Free" -ForegroundColor White
    Write-Host "   - Region: West Europe" -ForegroundColor White
    Write-Host "`n   Deployment:" -ForegroundColor Yellow
    Write-Host "   - Source: GitHub" -ForegroundColor White
    Write-Host "   - GitHub account: bkalafat" -ForegroundColor White
    Write-Host "   - Organization: bkalafat" -ForegroundColor White
    Write-Host "   - Repository: newsportal" -ForegroundColor White
    Write-Host "   - Branch: master" -ForegroundColor White
    Write-Host "`n   Build Details:" -ForegroundColor Yellow
    Write-Host "   - Build Presets: Next.js" -ForegroundColor White
    Write-Host "   - App location: /frontend" -ForegroundColor White
    Write-Host "   - Api location: (leave empty)" -ForegroundColor White
    Write-Host "   - Output location: .next" -ForegroundColor White
    Write-Host "`n4. Click 'Review + Create' → 'Create'" -ForegroundColor White
    Write-Host "5. Wait for deployment (~2 minutes)" -ForegroundColor White
    Write-Host "`n6. After creation, go to Configuration → Environment variables:" -ForegroundColor Yellow
    Write-Host "   Add the following:" -ForegroundColor White
    if ($backendUrl) {
        Write-Host "   - NEXT_PUBLIC_API_URL = https://$backendUrl" -ForegroundColor Cyan
    }
    Write-Host "   - NEXT_PUBLIC_UNSPLASH_ACCESS_KEY = ATK1ZDXPJKfdlYpm10AY1QNTvjzN4GymqQdH3JRP3qdU" -ForegroundColor Cyan
    Write-Host "`n7. Save and wait for redeployment" -ForegroundColor White
    
    exit 0
}

Write-Host "✅ Static Web App created!`n" -ForegroundColor Green

Write-Host "📋 Step 3: Configure Environment Variables" -ForegroundColor Yellow
if ($backendUrl) {
    Write-Host "Setting NEXT_PUBLIC_API_URL..." -ForegroundColor Cyan
    az staticwebapp appsettings set `
        --name $appName `
        --resource-group $resourceGroup `
        --setting-names "NEXT_PUBLIC_API_URL=https://$backendUrl" "NEXT_PUBLIC_UNSPLASH_ACCESS_KEY=ATK1ZDXPJKfdlYpm10AY1QNTvjzN4GymqQdH3JRP3qdU"
    Write-Host "✅ Environment variables set`n" -ForegroundColor Green
} else {
    Write-Host "⚠️ Skipped (no backend URL). Set manually in Azure Portal.`n" -ForegroundColor Yellow
}

Write-Host "📋 Step 4: Get Static Web App URL" -ForegroundColor Yellow
$frontendUrl = az staticwebapp show `
    --name $appName `
    --resource-group $resourceGroup `
    --query defaultHostname `
    -o tsv

Write-Host "`n🎉 FRONTEND DEPLOYMENT COMPLETE!" -ForegroundColor Green
Write-Host "=========================================================`n" -ForegroundColor Cyan

Write-Host "🌐 Application URLs:" -ForegroundColor Yellow
if ($frontendUrl) {
    Write-Host "  Frontend: https://$frontendUrl" -ForegroundColor White
}
if ($backendUrl) {
    Write-Host "  Backend: https://$backendUrl" -ForegroundColor White
    Write-Host "  Swagger: https://$backendUrl/swagger" -ForegroundColor White
}

Write-Host "`n📝 GitHub Actions Workflow:" -ForegroundColor Yellow
Write-Host "  A GitHub Action workflow has been automatically created in:" -ForegroundColor White
Write-Host "  .github/workflows/azure-static-web-apps-<random-name>.yml" -ForegroundColor Cyan
Write-Host "  This workflow will automatically deploy your frontend on every push to master." -ForegroundColor White

Write-Host "`n🔧 Update CORS Settings:" -ForegroundColor Yellow
Write-Host "  Don't forget to update CORS in your backend to allow the frontend URL:" -ForegroundColor White
if ($frontendUrl) {
    Write-Host "  Add to backend ServiceCollectionExtensions.cs:" -ForegroundColor Cyan
    Write-Host "  .WithOrigins(`"https://$frontendUrl`")`n" -ForegroundColor White
}

Write-Host "✅ Deployment complete!" -ForegroundColor Green
