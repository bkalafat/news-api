# Azure Deployment Script - 100% FREE TIER
# Static Web Apps (Frontend) + Container Apps (Backend)

$ErrorActionPreference = "Stop"

Write-Host "🚀 Starting Azure deployment..." -ForegroundColor Cyan

# Variables
$subscriptionId = "19acb272-bb61-400b-acc4-8b7a2f7aa0cc"
$resourceGroup = "newsportal-rg"
$location = "westeurope"
$projectName = "newsportal"

# Secrets (will be prompted or from environment)
$mongoConnectionString = "mongodb+srv://bkalafat:dbuserpassword123@cluster0.xwbfl1o.mongodb.net/NewsDb"
$unsplashKey = "ATK1ZDXPJKfdlYpm10AY1QNTvjzN4GymqQdH3JRP3qdU"

Write-Host "📋 Step 1: Verify Azure login" -ForegroundColor Yellow
az account show

Write-Host "`n📋 Step 2: Set subscription" -ForegroundColor Yellow
az account set --subscription $subscriptionId

Write-Host "`n📋 Step 3: Create resource group" -ForegroundColor Yellow
az group create `
    --name $resourceGroup `
    --location $location

Write-Host "`n📋 Step 4: Create Azure Container Registry (FREE tier - Basic)" -ForegroundColor Yellow
# Check if ACR exists
$acrExists = az acr show --name $projectName --resource-group $resourceGroup 2>$null
if (-not $acrExists) {
    az acr create `
        --name $projectName `
        --resource-group $resourceGroup `
        --sku Basic `
        --admin-enabled true
    Write-Host "✅ ACR created" -ForegroundColor Green
} else {
    Write-Host "✅ ACR already exists" -ForegroundColor Green
}

Write-Host "`n📋 Step 5: Build and push Docker image to ACR" -ForegroundColor Yellow
# Get ACR login server
$acrLoginServer = az acr show --name $projectName --resource-group $resourceGroup --query loginServer -o tsv

# Login to ACR
az acr login --name $projectName

# Build and push
docker build -t "${acrLoginServer}/newsapi:latest" -f Dockerfile .
docker push "${acrLoginServer}/newsapi:latest"

Write-Host "`n📋 Step 6: Create Container Apps Environment (FREE 2M requests/month)" -ForegroundColor Yellow
$envExists = az containerapp env show --name "${projectName}-env" --resource-group $resourceGroup 2>$null
if (-not $envExists) {
    az containerapp env create `
        --name "${projectName}-env" `
        --resource-group $resourceGroup `
        --location $location
    Write-Host "✅ Container Apps Environment created" -ForegroundColor Green
} else {
    Write-Host "✅ Environment already exists" -ForegroundColor Green
}

Write-Host "`n📋 Step 7: Get Cloudflare R2 Secret Key (MANUAL INPUT REQUIRED)" -ForegroundColor Yellow
Write-Host "Go to: https://dash.cloudflare.com/ → R2 → API Tokens" -ForegroundColor Cyan
Write-Host "Find Access Key ID: cc61a30e775100a198836d97dbce0d79" -ForegroundColor Cyan
$minioSecretKey = Read-Host "Enter Cloudflare R2 Secret Key"

Write-Host "`n📋 Step 8: Generate JWT Secret Key" -ForegroundColor Yellow
$jwtSecret = [Convert]::ToBase64String([System.Text.Encoding]::UTF8.GetBytes([System.Guid]::NewGuid().ToString() + [System.Guid]::NewGuid().ToString()))
Write-Host "Generated JWT Secret: $jwtSecret" -ForegroundColor Green

Write-Host "`n📋 Step 9: Create Container App (Backend) - FREE TIER" -ForegroundColor Yellow
az containerapp create `
    --name "${projectName}-backend" `
    --resource-group $resourceGroup `
    --environment "${projectName}-env" `
    --image "${acrLoginServer}/newsapi:latest" `
    --target-port 8080 `
    --ingress external `
    --registry-server $acrLoginServer `
    --registry-username $projectName `
    --registry-password (az acr credential show --name $projectName --query "passwords[0].value" -o tsv) `
    --cpu 0.25 `
    --memory 0.5Gi `
    --min-replicas 0 `
    --max-replicas 1 `
    --secrets `
        mongodb-connection="$mongoConnectionString" `
        minio-secret-key="$minioSecretKey" `
        unsplash-key="$unsplashKey" `
        jwt-secret="$jwtSecret" `
    --env-vars `
        ASPNETCORE_ENVIRONMENT=Production `
        MongoDbSettings__ConnectionString=secretref:mongodb-connection `
        MongoDbSettings__DatabaseName=NewsDb `
        MongoDbSettings__NewsCollectionName=News `
        MinioSettings__Endpoint=7ac015923324a4d426c1f7782c3f41e1.r2.cloudflarestorage.com `
        MinioSettings__AccessKey=cc61a30e775100a198836d97dbce0d79 `
        MinioSettings__SecretKey=secretref:minio-secret-key `
        MinioSettings__BucketName=news-images `
        MinioSettings__UseSSL=true `
        UnsplashSettings__AccessKey=secretref:unsplash-key `
        JwtSettings__SecretKey=secretref:jwt-secret `
        JwtSettings__Issuer=NewsApi `
        JwtSettings__Audience=NewsApiUsers `
        JwtSettings__ExpirationMinutes=60

Write-Host "`n📋 Step 10: Get backend URL" -ForegroundColor Yellow
$backendUrl = az containerapp show `
    --name "${projectName}-backend" `
    --resource-group $resourceGroup `
    --query properties.configuration.ingress.fqdn `
    -o tsv

Write-Host "`n✅ Backend deployed at: https://$backendUrl" -ForegroundColor Green

Write-Host "`n📋 Step 11: Test backend health" -ForegroundColor Yellow
Start-Sleep -Seconds 10
$healthCheck = curl -s "https://$backendUrl/health"
Write-Host "Health check response: $healthCheck" -ForegroundColor Cyan

Write-Host "`n📋 Step 12: Create Static Web App (Frontend) - FREE TIER" -ForegroundColor Yellow
Write-Host "⚠️  MANUAL STEP REQUIRED:" -ForegroundColor Red
Write-Host "1. Go to Azure Portal: https://portal.azure.com/" -ForegroundColor Cyan
Write-Host "2. Create a new Static Web App:" -ForegroundColor Cyan
Write-Host "   - Resource Group: $resourceGroup" -ForegroundColor Cyan
Write-Host "   - Name: ${projectName}-frontend" -ForegroundColor Cyan
Write-Host "   - Region: West Europe" -ForegroundColor Cyan
Write-Host "   - Source: GitHub" -ForegroundColor Cyan
Write-Host "   - Repository: bkalafat/newsportal" -ForegroundColor Cyan
Write-Host "   - Branch: master" -ForegroundColor Cyan
Write-Host "   - Build Presets: Next.js" -ForegroundColor Cyan
Write-Host "   - App location: /frontend" -ForegroundColor Cyan
Write-Host "   - Output location: .next" -ForegroundColor Cyan
Write-Host "3. After creation, add environment variable:" -ForegroundColor Cyan
Write-Host "   NEXT_PUBLIC_API_URL = https://$backendUrl" -ForegroundColor Cyan

Write-Host "`n📋 Step 13: Setup GitHub Actions secrets" -ForegroundColor Yellow
Write-Host "Add these secrets to: https://github.com/bkalafat/newsportal/settings/secrets/actions" -ForegroundColor Cyan
Write-Host "API_BASE_URL = https://$backendUrl" -ForegroundColor Green
Write-Host "ADMIN_USERNAME = admin" -ForegroundColor Green
Write-Host "ADMIN_PASSWORD = admin123" -ForegroundColor Green
Write-Host "UNSPLASH_ACCESS_KEY = $unsplashKey" -ForegroundColor Green
Write-Host "AZURE_BACKEND_URL = $backendUrl" -ForegroundColor Green

Write-Host "`n🎉 Deployment complete!" -ForegroundColor Green
Write-Host "`n📊 Cost Summary:" -ForegroundColor Yellow
Write-Host "✅ Static Web Apps: FREE (100GB bandwidth/month)" -ForegroundColor Green
Write-Host "✅ Container Apps: FREE (2M requests/month, scale-to-zero)" -ForegroundColor Green
Write-Host "✅ Container Registry: FREE (Basic SKU, 10GB storage)" -ForegroundColor Green
Write-Host "✅ MongoDB Atlas: FREE M0 (512MB, external)" -ForegroundColor Green
Write-Host "✅ Cloudflare R2: FREE (10GB, external)" -ForegroundColor Green
Write-Host "✅ GitHub Actions: FREE (public repo)" -ForegroundColor Green
Write-Host "`n💰 Total Monthly Cost: $0.00" -ForegroundColor Cyan

Write-Host "`n📝 Next steps:" -ForegroundColor Yellow
Write-Host "1. Complete Static Web App creation in Azure Portal" -ForegroundColor White
Write-Host "2. Add GitHub secrets" -ForegroundColor White
Write-Host "3. Test frontend + backend integration" -ForegroundColor White
Write-Host "4. Trigger weekly news aggregation manually to verify" -ForegroundColor White
