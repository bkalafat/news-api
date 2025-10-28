# Azure App Service Deployment for NewsPortal
# Deploy .NET API to Azure using Docker

# Prerequisites:
# - Azure account with free credits
# - Azure CLI installed
# - Docker running locally

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "NewsPortal - Azure Deployment Script" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Configuration
$RESOURCE_GROUP = "newsportal-rg"
$LOCATION = "westeurope"  # Closest to Turkey
$APP_SERVICE_PLAN = "newsportal-plan"
$WEB_APP_NAME = "newsportal-api-$(Get-Random -Maximum 9999)"  # Must be globally unique
$ACR_NAME = "newsportalacr$(Get-Random -Maximum 9999)"  # Azure Container Registry

Write-Host "Configuration:" -ForegroundColor Yellow
Write-Host "  Resource Group: $RESOURCE_GROUP"
Write-Host "  Location: $LOCATION"
Write-Host "  Web App Name: $WEB_APP_NAME"
Write-Host "  Container Registry: $ACR_NAME"
Write-Host ""

# Step 1: Login to Azure
Write-Host "Step 1: Logging in to Azure..." -ForegroundColor Green
az login

# Step 2: Create Resource Group
Write-Host "`nStep 2: Creating Resource Group..." -ForegroundColor Green
az group create --name $RESOURCE_GROUP --location $LOCATION

# Step 3: Create Azure Container Registry
Write-Host "`nStep 3: Creating Container Registry..." -ForegroundColor Green
az acr create --resource-group $RESOURCE_GROUP --name $ACR_NAME --sku Basic --admin-enabled true

# Step 4: Build and Push Docker Image to ACR
Write-Host "`nStep 4: Building Docker image..." -ForegroundColor Green
Set-Location C:\dev\newsportal

# Login to ACR
az acr login --name $ACR_NAME

# Build and push
Write-Host "Building and pushing image (this may take 5-10 minutes)..." -ForegroundColor Yellow
az acr build --registry $ACR_NAME --image newsportal-api:latest --file Dockerfile .

# Step 5: Create App Service Plan
Write-Host "`nStep 5: Creating App Service Plan..." -ForegroundColor Green
az appservice plan create `
    --name $APP_SERVICE_PLAN `
    --resource-group $RESOURCE_GROUP `
    --sku B1 `
    --is-linux

# Step 6: Create Web App
Write-Host "`nStep 6: Creating Web App..." -ForegroundColor Green
$ACR_LOGIN_SERVER = az acr show --name $ACR_NAME --query loginServer --output tsv
$ACR_USERNAME = az acr credential show --name $ACR_NAME --query username --output tsv
$ACR_PASSWORD = az acr credential show --name $ACR_NAME --query passwords[0].value --output tsv

az webapp create `
    --resource-group $RESOURCE_GROUP `
    --plan $APP_SERVICE_PLAN `
    --name $WEB_APP_NAME `
    --deployment-container-image-name "$ACR_LOGIN_SERVER/newsportal-api:latest"

# Configure container registry credentials
az webapp config container set `
    --name $WEB_APP_NAME `
    --resource-group $RESOURCE_GROUP `
    --docker-custom-image-name "$ACR_LOGIN_SERVER/newsportal-api:latest" `
    --docker-registry-server-url "https://$ACR_LOGIN_SERVER" `
    --docker-registry-server-user $ACR_USERNAME `
    --docker-registry-server-password $ACR_PASSWORD

# Step 7: Configure Environment Variables
Write-Host "`nStep 7: Configuring Environment Variables..." -ForegroundColor Green

# MongoDB Atlas connection
az webapp config appsettings set --resource-group $RESOURCE_GROUP --name $WEB_APP_NAME --settings `
    "MongoDbSettings__ConnectionString=mongodb+srv://bkalafat:dbuserpassword123@cluster0.xwbfl1o.mongodb.net/NewsDb?retryWrites=true&w=majority" `
    "MongoDbSettings__DatabaseName=NewsDb" `
    "MongoDbSettings__NewsCollectionName=News"

# ASP.NET Core settings
az webapp config appsettings set --resource-group $RESOURCE_GROUP --name $WEB_APP_NAME --settings `
    "ASPNETCORE_ENVIRONMENT=Production" `
    "ASPNETCORE_URLS=http://+:8080" `
    "WEBSITES_PORT=8080"

# JWT Settings
az webapp config appsettings set --resource-group $RESOURCE_GROUP --name $WEB_APP_NAME --settings `
    "JwtSettings__SecretKey=newsportal-azure-super-secret-jwt-key-minimum-32-characters-2025" `
    "JwtSettings__Issuer=NewsApi" `
    "JwtSettings__Audience=NewsApiClients" `
    "JwtSettings__ExpirationMinutes=1440"

# Cloudflare R2 Settings
Write-Host "`n⚠️  Note: Add your R2 Secret Key manually in Azure Portal" -ForegroundColor Yellow
az webapp config appsettings set --resource-group $RESOURCE_GROUP --name $WEB_APP_NAME --settings `
    "MinioSettings__Endpoint=7ac015923324a4d426c1f7782c3f41e1.r2.cloudflarestorage.com" `
    "MinioSettings__AccessKey=cc61a30e775100a198836d97dbce0d79" `
    "MinioSettings__BucketName=news-images" `
    "MinioSettings__UseSSL=true" `
    "MinioSettings__MaxFileSizeBytes=10485760"

# CORS
az webapp config appsettings set --resource-group $RESOURCE_GROUP --name $WEB_APP_NAME --settings `
    "AllowedOrigins=http://localhost:3000,https://newsportal.vercel.app"

# Step 8: Enable Continuous Deployment (Optional)
Write-Host "`nStep 8: Enabling Continuous Deployment..." -ForegroundColor Green
az webapp deployment container config --enable-cd true --name $WEB_APP_NAME --resource-group $RESOURCE_GROUP

# Step 9: Restart Web App
Write-Host "`nStep 9: Restarting Web App..." -ForegroundColor Green
az webapp restart --name $WEB_APP_NAME --resource-group $RESOURCE_GROUP

# Get the URL
$WEB_APP_URL = "https://$WEB_APP_NAME.azurewebsites.net"

Write-Host "`n========================================" -ForegroundColor Green
Write-Host "✅ Deployment Complete!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "Your API is available at:" -ForegroundColor Yellow
Write-Host "  $WEB_APP_URL" -ForegroundColor Cyan
Write-Host ""
Write-Host "Test endpoints:" -ForegroundColor Yellow
Write-Host "  Health: $WEB_APP_URL/health" -ForegroundColor White
Write-Host "  API: $WEB_APP_URL/api/NewsArticle" -ForegroundColor White
Write-Host "  Swagger: $WEB_APP_URL/swagger" -ForegroundColor White
Write-Host ""
Write-Host "Important Next Steps:" -ForegroundColor Yellow
Write-Host "  1. Add MinioSettings__SecretKey in Azure Portal" -ForegroundColor White
Write-Host "  2. Update GitHub Actions with API_BASE_URL=$WEB_APP_URL" -ForegroundColor White
Write-Host "  3. Deploy frontend to Vercel with this backend URL" -ForegroundColor White
Write-Host ""
Write-Host "Azure Portal: https://portal.azure.com" -ForegroundColor Cyan
Write-Host "Resource Group: $RESOURCE_GROUP" -ForegroundColor White
Write-Host ""

# Save configuration
$config = @{
    ResourceGroup = $RESOURCE_GROUP
    Location = $LOCATION
    WebAppName = $WEB_APP_NAME
    WebAppUrl = $WEB_APP_URL
    ContainerRegistry = $ACR_NAME
    AppServicePlan = $APP_SERVICE_PLAN
} | ConvertTo-Json

$config | Out-File "azure-deployment-config.json"
Write-Host "Configuration saved to: azure-deployment-config.json" -ForegroundColor Green
