# Quick Azure Deployment Script for Windows
# Run with: .\deploy.ps1

# ============================================
# Configuration
# ============================================
Write-Host "🚀 News API - Azure Quick Deploy" -ForegroundColor Cyan
Write-Host "==================================" -ForegroundColor Cyan
Write-Host ""

# Prompt for required values
$Environment = Read-Host "Enter environment (dev/staging/prod) [dev]"
if ([string]::IsNullOrWhiteSpace($Environment)) { $Environment = "dev" }

$Location = Read-Host "Enter Azure region [eastus]"
if ([string]::IsNullOrWhiteSpace($Location)) { $Location = "eastus" }

$AppName = Read-Host "Enter app name [newsapi]"
if ([string]::IsNullOrWhiteSpace($AppName)) { $AppName = "newsapi" }

$AcrName = Read-Host "Enter Azure Container Registry name (lowercase, no hyphens)"
if ([string]::IsNullOrWhiteSpace($AcrName)) {
    Write-Host "❌ ACR name is required" -ForegroundColor Red
    exit 1
}

$JwtSecret = Read-Host "Enter JWT Secret Key (min 32 chars)" -AsSecureString
$JwtSecretPlain = [System.Runtime.InteropServices.Marshal]::PtrToStringAuto(
    [System.Runtime.InteropServices.Marshal]::SecureStringToBSTR($JwtSecret)
)
if ($JwtSecretPlain.Length -lt 32) {
    Write-Host "❌ JWT Secret must be at least 32 characters" -ForegroundColor Red
    exit 1
}

$MongoPassword = Read-Host "Enter MongoDB Admin Password" -AsSecureString
$MongoPasswordPlain = [System.Runtime.InteropServices.Marshal]::PtrToStringAuto(
    [System.Runtime.InteropServices.Marshal]::SecureStringToBSTR($MongoPassword)
)
if ([string]::IsNullOrWhiteSpace($MongoPasswordPlain)) {
    Write-Host "❌ MongoDB password is required" -ForegroundColor Red
    exit 1
}

# Set derived variables
$ResourceGroup = "$AppName-rg-$Environment"
$ContainerImage = "$AcrName.azurecr.io/${AppName}:latest"

Write-Host ""
Write-Host "📋 Deployment Configuration:" -ForegroundColor Yellow
Write-Host "   Environment:      $Environment"
Write-Host "   Location:  $Location"
Write-Host "   Resource Group:   $ResourceGroup"
Write-Host "   ACR Name:         $AcrName"
Write-Host "   Container Image:  $ContainerImage"
Write-Host ""

$Confirm = Read-Host "Continue with deployment? (yes/no)"
if ($Confirm -ne "yes") {
    Write-Host "❌ Deployment cancelled" -ForegroundColor Red
    exit 0
}

# ============================================
# Azure Login
# ============================================
Write-Host ""
Write-Host "🔐 Step 1/7: Logging in to Azure..." -ForegroundColor Cyan

try {
    $account = az account show | ConvertFrom-Json
    $subscription = $account.name
    Write-Host "✅ Logged in to subscription: $subscription" -ForegroundColor Green
} catch {
    az login
}

# ============================================
# Create Resource Group
# ============================================
Write-Host ""
Write-Host "📦 Step 2/7: Creating Resource Group..." -ForegroundColor Cyan

az group create `
  --name $ResourceGroup `
    --location $Location `
    --output none

Write-Host "✅ Resource group created: $ResourceGroup" -ForegroundColor Green

# ============================================
# Create Azure Container Registry
# ============================================
Write-Host ""
Write-Host "🐳 Step 3/7: Creating Azure Container Registry..." -ForegroundColor Cyan

try {
    az acr show --name $AcrName --resource-group $ResourceGroup --output none
    Write-Host "ℹ️  ACR already exists, skipping creation" -ForegroundColor Yellow
} catch {
    az acr create `
  --resource-group $ResourceGroup `
        --name $AcrName `
        --sku Basic `
  --admin-enabled true `
        --output none
    
    Write-Host "✅ Container Registry created: $AcrName" -ForegroundColor Green
}

# ============================================
# Build and Push Docker Image
# ============================================
Write-Host ""
Write-Host "🔨 Step 4/7: Building and pushing Docker image..." -ForegroundColor Cyan

# Login to ACR
az acr login --name $AcrName

# Build image
Write-Host "   Building Docker image..."
docker build -t $ContainerImage -f ..\Dockerfile ..

# Push image
Write-Host "   Pushing Docker image to ACR..."
docker push $ContainerImage

Write-Host "✅ Docker image pushed: $ContainerImage" -ForegroundColor Green

# ============================================
# Update Bicep Parameters
# ============================================
Write-Host ""
Write-Host "📝 Step 5/7: Preparing deployment parameters..." -ForegroundColor Cyan

$MinReplicas = if ($Environment -eq "prod") { 2 } else { 1 }
$MaxReplicas = if ($Environment -eq "prod") { 10 } else { 3 }

$ParamsFile = "parameters.$Environment.temp.json"
$ParamsContent = @"
{
  "`$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#",
  "contentVersion": "1.0.0.0",
  "parameters": {
    "environmentName": {
      "value": "$Environment"
    },
    "appName": {
      "value": "$AppName"
    },
    "containerImage": {
      "value": "$ContainerImage"
    },
    "jwtSecretKey": {
      "value": "$JwtSecretPlain"
    },
    "mongoAdminUsername": {
      "value": "newsapiadmin"
    },
    "mongoAdminPassword": {
      "value": "$MongoPasswordPlain"
    },
    "minReplicas": {
      "value": $MinReplicas
    },
    "maxReplicas": {
      "value": $MaxReplicas
    }
  }
}
"@

$ParamsContent | Out-File -FilePath $ParamsFile -Encoding UTF8
Write-Host "✅ Parameters file created" -ForegroundColor Green

# ============================================
# Deploy Infrastructure
# ============================================
Write-Host ""
Write-Host "☁️  Step 6/7: Deploying Azure infrastructure (this may take 5-10 minutes)..." -ForegroundColor Cyan

az deployment group create `
    --resource-group $ResourceGroup `
    --template-file main.bicep `
    --parameters $ParamsFile `
    --output none

# Clean up temporary parameters file
Remove-Item $ParamsFile

Write-Host "✅ Infrastructure deployed successfully" -ForegroundColor Green

# ============================================
# Get Deployment Outputs
# ============================================
Write-Host ""
Write-Host "📊 Step 7/7: Retrieving deployment information..." -ForegroundColor Cyan

$AppUrl = az deployment group show `
    --resource-group $ResourceGroup `
    --name main `
    --query properties.outputs.containerAppUrl.value `
    --output tsv

$AcrServer = az deployment group show `
    --resource-group $ResourceGroup `
  --name main `
    --query properties.outputs.containerRegistryLoginServer.value `
    --output tsv

$StorageAccount = az deployment group show `
    --resource-group $ResourceGroup `
    --name main `
    --query properties.outputs.storageAccountName.value `
    --output tsv

# ============================================
# Health Check
# ============================================
Write-Host ""
Write-Host "🏥 Running health check..." -ForegroundColor Cyan
Start-Sleep -Seconds 30

try {
    $response = Invoke-WebRequest -Uri "$AppUrl/health" -UseBasicParsing
    Write-Host "✅ Health check passed!" -ForegroundColor Green
} catch {
    Write-Host "⚠️  Health check failed - application may still be starting" -ForegroundColor Yellow
}

# ============================================
# Summary
# ============================================
Write-Host ""
Write-Host "============================================" -ForegroundColor Green
Write-Host "✅ Deployment Complete!" -ForegroundColor Green
Write-Host "============================================" -ForegroundColor Green
Write-Host ""
Write-Host "📍 Application URL:" -ForegroundColor Cyan
Write-Host "   $AppUrl"
Write-Host ""
Write-Host "📚 Swagger Documentation:" -ForegroundColor Cyan
Write-Host "   $AppUrl/swagger"
Write-Host ""
Write-Host "🏥 Health Endpoint:" -ForegroundColor Cyan
Write-Host "   $AppUrl/health"
Write-Host ""
Write-Host "🐳 Container Registry:" -ForegroundColor Cyan
Write-Host "   $AcrServer"
Write-Host ""
Write-Host "📦 Storage Account:" -ForegroundColor Cyan
Write-Host "   $StorageAccount"
Write-Host ""
Write-Host "🔧 Resource Group:" -ForegroundColor Cyan
Write-Host "   $ResourceGroup"
Write-Host ""
Write-Host "============================================" -ForegroundColor Green
Write-Host ""
Write-Host "📖 Next Steps:" -ForegroundColor Yellow
Write-Host "   1. Test API endpoints using Swagger UI"
Write-Host "   2. Configure CORS for your frontend domain"
Write-Host "   3. Set up custom domain (optional)"
Write-Host "   4. Configure monitoring alerts"
Write-Host "   5. Set up GitHub Actions for CI/CD"
Write-Host ""
Write-Host "📚 For more information, see:" -ForegroundColor Yellow
Write-Host "   azure\DEPLOYMENT_GUIDE.md"
Write-Host ""
Write-Host "🆘 View logs:" -ForegroundColor Yellow
Write-Host "   az containerapp logs show --name $AppName-$Environment-app --resource-group $ResourceGroup --follow"
Write-Host ""
