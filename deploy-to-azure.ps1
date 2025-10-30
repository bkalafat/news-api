# Complete Azure Deployment Script for News Portal
# Run this after ensuring Docker Desktop is running

$ErrorActionPreference = "Stop"

Write-Host "🚀 News Portal - Complete Azure Deployment" -ForegroundColor Cyan
Write-Host "============================================`n" -ForegroundColor Cyan

# Configuration
$resourceGroup = "newsportal-rg"
$location = "westeurope"
$acrName = "newsportal"
$projectName = "newsportal"

# Secrets (from parameters.json and environment)
$mongoConnectionString = "mongodb+srv://bkalafat:dbuserpassword123@cluster0.xwbfl1o.mongodb.net/NewsDb"
$minioEndpoint = "7ac015923324a4d426c1f7782c3f41e1.r2.cloudflarestorage.com"
$minioAccessKey = "cc61a30e775100a198836d97dbce0d79"
$unsplashKey = "ATK1ZDXPJKfdlYpm10AY1QNTvjzN4GymqQdH3JRP3qdU"

# Prompt for secrets that need to be secure
Write-Host "📋 Please provide the following secrets:" -ForegroundColor Yellow
$minioSecretKey = Read-Host "Enter Cloudflare R2 Secret Key" -AsSecureString
$minioSecretKeyPlain = [Runtime.InteropServices.Marshal]::PtrToStringAuto([Runtime.InteropServices.Marshal]::SecureStringToBSTR($minioSecretKey))

# Generate JWT secret
$jwtSecret = [Convert]::ToBase64String([System.Text.Encoding]::UTF8.GetBytes([System.Guid]::NewGuid().ToString() + [System.Guid]::NewGuid().ToString()))
Write-Host "Generated JWT Secret: $jwtSecret" -ForegroundColor Green

Write-Host "`n✅ Step 1: Verify Docker is running" -ForegroundColor Yellow
$dockerRunning = docker info 2>$null
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Docker is not running. Please start Docker Desktop and try again." -ForegroundColor Red
    exit 1
}
Write-Host "✅ Docker is running`n" -ForegroundColor Green

Write-Host "✅ Step 2: Login to Azure Container Registry" -ForegroundColor Yellow
az acr login --name $acrName
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ ACR login failed" -ForegroundColor Red
    exit 1
}
Write-Host "✅ ACR login successful`n" -ForegroundColor Green

Write-Host "✅ Step 3: Build Docker image" -ForegroundColor Yellow
$acrLoginServer = az acr show --name $acrName --resource-group $resourceGroup --query loginServer -o tsv
docker build -t "${acrLoginServer}/newsportal-backend:latest" -f Dockerfile .
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Docker build failed" -ForegroundColor Red
    exit 1
}
Write-Host "✅ Docker image built`n" -ForegroundColor Green

Write-Host "✅ Step 4: Push image to ACR" -ForegroundColor Yellow
docker push "${acrLoginServer}/newsportal-backend:latest"
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Docker push failed" -ForegroundColor Red
    exit 1
}
Write-Host "✅ Image pushed to ACR`n" -ForegroundColor Green

Write-Host "✅ Step 5: Create Container Apps Environment" -ForegroundColor Yellow
$envExists = az containerapp env show --name "${projectName}-env" --resource-group $resourceGroup 2>$null
if (-not $envExists) {
    az containerapp env create `
        --name "${projectName}-env" `
        --resource-group $resourceGroup `
        --location $location
    Write-Host "✅ Container Apps Environment created`n" -ForegroundColor Green
} else {
    Write-Host "✅ Environment already exists`n" -ForegroundColor Green
}

Write-Host "✅ Step 6: Deploy Backend Container App" -ForegroundColor Yellow
# Get ACR credentials
$acrPassword = az acr credential show --name $acrName --query "passwords[0].value" -o tsv

az containerapp create `
    --name "${projectName}-backend" `
    --resource-group $resourceGroup `
    --environment "${projectName}-env" `
    --image "${acrLoginServer}/newsportal-backend:latest" `
    --target-port 8080 `
    --ingress external `
    --registry-server $acrLoginServer `
    --registry-username $acrName `
    --registry-password $acrPassword `
    --cpu 0.5 `
    --memory 1Gi `
    --min-replicas 0 `
    --max-replicas 2 `
    --secrets `
        mongodb-connection="$mongoConnectionString" `
        minio-secret-key="$minioSecretKeyPlain" `
        unsplash-key="$unsplashKey" `
        jwt-secret="$jwtSecret" `
    --env-vars `
        ASPNETCORE_ENVIRONMENT=Production `
        "MongoDbSettings__ConnectionString=secretref:mongodb-connection" `
        "MongoDbSettings__DatabaseName=NewsDb" `
        "MongoDbSettings__NewsCollectionName=News" `
        "MinioSettings__Endpoint=$minioEndpoint" `
        "MinioSettings__AccessKey=$minioAccessKey" `
        "MinioSettings__SecretKey=secretref:minio-secret-key" `
        "MinioSettings__BucketName=news-images" `
        "MinioSettings__UseSSL=true" `
        "UnsplashSettings__AccessKey=secretref:unsplash-key" `
        "JwtSettings__SecretKey=secretref:jwt-secret" `
        "JwtSettings__Issuer=NewsPortal" `
        "JwtSettings__Audience=NewsPortalUsers" `
        "JwtSettings__ExpirationMinutes=60"

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Container App creation failed" -ForegroundColor Red
    exit 1
}
Write-Host "✅ Backend deployed`n" -ForegroundColor Green

Write-Host "✅ Step 7: Get Backend URL" -ForegroundColor Yellow
$backendUrl = az containerapp show `
    --name "${projectName}-backend" `
    --resource-group $resourceGroup `
    --query properties.configuration.ingress.fqdn `
    -o tsv

Write-Host "`n✅ Backend deployed at: https://$backendUrl" -ForegroundColor Green

Write-Host "`n✅ Step 8: Test Backend Health" -ForegroundColor Yellow
Write-Host "Waiting 30 seconds for container to start..." -ForegroundColor Cyan
Start-Sleep -Seconds 30

try {
    $health = Invoke-WebRequest -Uri "https://$backendUrl/health" -UseBasicParsing
    Write-Host "✅ Health check passed! Status: $($health.StatusCode)" -ForegroundColor Green
} catch {
    Write-Host "⚠️ Health check failed. The container may still be starting. Try again in a minute." -ForegroundColor Yellow
}

Write-Host "`n🎉 BACKEND DEPLOYMENT COMPLETE!" -ForegroundColor Green
Write-Host "============================================`n" -ForegroundColor Cyan

Write-Host "📊 Backend Information:" -ForegroundColor Yellow
Write-Host "  Backend URL: https://$backendUrl" -ForegroundColor White
Write-Host "  Health Check: https://$backendUrl/health" -ForegroundColor White
Write-Host "  Swagger UI: https://$backendUrl/swagger" -ForegroundColor White
Write-Host "  API Docs: https://$backendUrl/swagger/v1/swagger.json`n" -ForegroundColor White

Write-Host "🌐 NEXT STEP: Deploy Frontend to Azure Static Web Apps" -ForegroundColor Cyan
Write-Host "============================================`n" -ForegroundColor Cyan
Write-Host "Option 1: Via Azure Portal (Recommended)" -ForegroundColor Yellow
Write-Host "  1. Go to: https://portal.azure.com" -ForegroundColor White
Write-Host "  2. Click 'Create a resource' → 'Static Web App'" -ForegroundColor White
Write-Host "  3. Configuration:" -ForegroundColor White
Write-Host "     - Resource Group: $resourceGroup" -ForegroundColor Cyan
Write-Host "     - Name: ${projectName}-frontend" -ForegroundColor Cyan
Write-Host "     - Region: West Europe" -ForegroundColor Cyan
Write-Host "     - Source: GitHub" -ForegroundColor Cyan
Write-Host "     - Organization: bkalafat" -ForegroundColor Cyan
Write-Host "     - Repository: newsportal" -ForegroundColor Cyan
Write-Host "     - Branch: master" -ForegroundColor Cyan
Write-Host "     - Build Presets: Next.js" -ForegroundColor Cyan
Write-Host "     - App location: /frontend" -ForegroundColor Cyan
Write-Host "     - Output location: .next" -ForegroundColor Cyan
Write-Host "  4. After creation, add Configuration → Environment variables:" -ForegroundColor White
Write-Host "     - NEXT_PUBLIC_API_URL = https://$backendUrl`n" -ForegroundColor Cyan

Write-Host "Option 2: Via Azure CLI (Advanced)" -ForegroundColor Yellow
Write-Host "  Run the following command:" -ForegroundColor White
Write-Host "  az staticwebapp create --name ${projectName}-frontend --resource-group $resourceGroup --source https://github.com/bkalafat/newsportal --location westeurope --branch master --app-location /frontend --output-location .next --sku Free`n" -ForegroundColor Cyan

Write-Host "📝 GitHub Secrets to Add:" -ForegroundColor Yellow
Write-Host "  Go to: https://github.com/bkalafat/newsportal/settings/secrets/actions" -ForegroundColor White
Write-Host "  Add the following secrets:" -ForegroundColor White
Write-Host "  - NEXT_PUBLIC_API_URL = https://$backendUrl" -ForegroundColor Cyan
Write-Host "  - ADMIN_USERNAME = admin" -ForegroundColor Cyan
Write-Host "  - ADMIN_PASSWORD = admin123" -ForegroundColor Cyan
Write-Host "  - UNSPLASH_ACCESS_KEY = $unsplashKey`n" -ForegroundColor Cyan

Write-Host "💰 Cost Summary:" -ForegroundColor Yellow
Write-Host "  ✅ Container Apps: ~$0-20/month (free tier: 2M requests, 180k vCPU-s)" -ForegroundColor Green
Write-Host "  ✅ Container Registry: ~$5/month (Basic SKU)" -ForegroundColor Green
Write-Host "  ✅ Static Web Apps: FREE (100GB bandwidth/month)" -ForegroundColor Green
Write-Host "  ✅ MongoDB Atlas: FREE (M0 cluster, external)" -ForegroundColor Green
Write-Host "  ✅ Cloudflare R2: FREE (10GB storage, external)" -ForegroundColor Green
Write-Host "  💰 Total Estimated: $5-25/month`n" -ForegroundColor Cyan

Write-Host "🔧 Useful Commands:" -ForegroundColor Yellow
Write-Host "  View logs: az containerapp logs show --name ${projectName}-backend --resource-group $resourceGroup --follow" -ForegroundColor White
Write-Host "  Update image: az containerapp update --name ${projectName}-backend --resource-group $resourceGroup --image ${acrLoginServer}/newsportal-backend:latest" -ForegroundColor White
Write-Host "  Scale up: az containerapp update --name ${projectName}-backend --resource-group $resourceGroup --min-replicas 1 --max-replicas 5" -ForegroundColor White
Write-Host "  Delete all: az group delete --name $resourceGroup --yes`n" -ForegroundColor White

Write-Host "✅ Deployment script completed!" -ForegroundColor Green
