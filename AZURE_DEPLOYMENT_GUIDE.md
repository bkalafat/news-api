# Azure Deployment Guide - NewsPortal API

Complete guide to deploying NewsPortal .NET API to Azure App Service.

## 📋 Table of Contents
1. [Prerequisites](#prerequisites)
2. [Azure Free Tier](#azure-free-tier)
3. [Quick Deployment (Automated)](#quick-deployment)
4. [Manual Deployment (Step-by-Step)](#manual-deployment)
5. [Cost Breakdown](#cost-breakdown)
6. [Troubleshooting](#troubleshooting)

---

## 🎯 Prerequisites

### 1. Azure Account
- Sign up: https://azure.microsoft.com/free/
- **Free credits**: $200 for 30 days
- **Free services**: Many services free for 12 months
- No credit card required for trial (but recommended)

### 2. Install Azure CLI
Run in PowerShell:
```powershell
.\scripts\install-azure-cli.ps1
```
Or download: https://aka.ms/installazurecliwindows

### 3. Verify Installation
```powershell
az --version
az login
```

---

## 💰 Azure Free Tier

### What You Get Free:
- **App Service**: 10 web apps (F1 tier - 1GB RAM, 1GB storage)
- **Container Registry**: Basic tier (10GB storage)
- **Bandwidth**: 15GB outbound/month
- **Azure Monitor**: Basic metrics and logs
- **SSL Certificate**: Free via App Service

### Free for 12 Months:
- **Virtual Machine**: B1S (750 hours/month)
- **Managed Disks**: 64GB x 2
- **Blob Storage**: 5GB
- **Database**: Various options

### Cost Estimate:
- **Free tier**: $0/month
- **Basic tier (B1)**: ~₺200-300/month (~$6-10)
- **Standard tier (S1)**: ~₺600-800/month (~$20-25)

**Recommended**: Start with Free (F1), upgrade to B1 if needed

---

## 🚀 Quick Deployment (Automated)

### Option 1: Run Deployment Script (Easiest!)

```powershell
# 1. Navigate to project
cd C:\dev\newsportal

# 2. Run deployment script
.\scripts\deploy-to-azure.ps1
```

**Total time**: 15-20 minutes

**What it does**:
1. Creates Azure Resource Group
2. Creates Container Registry
3. Builds Docker image
4. Creates App Service Plan
5. Creates Web App
6. Configures environment variables
7. Deploys your API

**Result**: Your API running at `https://newsportal-api-XXXX.azurewebsites.net`

---

## 📝 Manual Deployment (Step-by-Step)

### Step 1: Login to Azure
```powershell
az login
```
Browser will open - sign in with your Microsoft account.

### Step 2: Set Variables
```powershell
$RESOURCE_GROUP = "newsportal-rg"
$LOCATION = "westeurope"  # Or: northeurope, uksouth
$APP_NAME = "newsportal-api-$(Get-Random -Maximum 9999)"
$PLAN_NAME = "newsportal-plan"
```

### Step 3: Create Resource Group
```powershell
az group create `
    --name $RESOURCE_GROUP `
    --location $LOCATION
```

### Step 4: Create App Service Plan

**Free Tier (F1)**:
```powershell
az appservice plan create `
    --name $PLAN_NAME `
    --resource-group $RESOURCE_GROUP `
    --sku F1 `
    --is-linux
```

**Basic Tier (B1)** - Recommended:
```powershell
az appservice plan create `
    --name $PLAN_NAME `
    --resource-group $RESOURCE_GROUP `
    --sku B1 `
    --is-linux
```

### Step 5: Create Web App with Docker
```powershell
az webapp create `
    --resource-group $RESOURCE_GROUP `
    --plan $PLAN_NAME `
    --name $APP_NAME `
    --deployment-container-image-name mcr.microsoft.com/dotnet/samples:aspnetapp
```

### Step 6: Configure for Custom Docker Image

**Option A: Use Azure Container Registry (Recommended)**
```powershell
# Create ACR
$ACR_NAME = "newsportalacr$(Get-Random -Maximum 9999)"
az acr create --resource-group $RESOURCE_GROUP --name $ACR_NAME --sku Basic --admin-enabled true

# Build and push
az acr login --name $ACR_NAME
cd C:\dev\newsportal
az acr build --registry $ACR_NAME --image newsportal-api:latest .

# Get credentials
$ACR_SERVER = az acr show --name $ACR_NAME --query loginServer --output tsv
$ACR_USER = az acr credential show --name $ACR_NAME --query username --output tsv
$ACR_PASS = az acr credential show --name $ACR_NAME --query passwords[0].value --output tsv

# Configure Web App
az webapp config container set `
    --name $APP_NAME `
    --resource-group $RESOURCE_GROUP `
    --docker-custom-image-name "$ACR_SERVER/newsportal-api:latest" `
    --docker-registry-server-url "https://$ACR_SERVER" `
    --docker-registry-server-user $ACR_USER `
    --docker-registry-server-password $ACR_PASS
```

**Option B: Use Docker Hub** (Simpler but public)
```powershell
# Build locally
cd C:\dev\newsportal
docker build -t YOUR_DOCKERHUB_USERNAME/newsportal-api:latest .

# Push to Docker Hub
docker login
docker push YOUR_DOCKERHUB_USERNAME/newsportal-api:latest

# Configure Web App
az webapp config container set `
    --name $APP_NAME `
    --resource-group $RESOURCE_GROUP `
    --docker-custom-image-name YOUR_DOCKERHUB_USERNAME/newsportal-api:latest
```

### Step 7: Configure Environment Variables

```powershell
# MongoDB Atlas
az webapp config appsettings set `
    --resource-group $RESOURCE_GROUP `
    --name $APP_NAME `
    --settings `
    "MongoDbSettings__ConnectionString=mongodb+srv://bkalafat:dbuserpassword123@cluster0.xwbfl1o.mongodb.net/NewsDb?retryWrites=true&w=majority" `
    "MongoDbSettings__DatabaseName=NewsDb" `
    "MongoDbSettings__NewsCollectionName=News" `
    "ASPNETCORE_ENVIRONMENT=Production" `
    "ASPNETCORE_URLS=http://+:8080" `
    "WEBSITES_PORT=8080" `
    "JwtSettings__SecretKey=your-super-secret-jwt-key-minimum-32-characters-long" `
    "JwtSettings__Issuer=NewsApi" `
    "JwtSettings__Audience=NewsApiClients" `
    "JwtSettings__ExpirationMinutes=1440" `
    "MinioSettings__Endpoint=7ac015923324a4d426c1f7782c3f41e1.r2.cloudflarestorage.com" `
    "MinioSettings__AccessKey=cc61a30e775100a198836d97dbce0d79" `
    "MinioSettings__SecretKey=YOUR_R2_SECRET_KEY" `
    "MinioSettings__BucketName=news-images" `
    "MinioSettings__UseSSL=true" `
    "AllowedOrigins=http://localhost:3000,https://newsportal.vercel.app"
```

### Step 8: Restart Web App
```powershell
az webapp restart --name $APP_NAME --resource-group $RESOURCE_GROUP
```

### Step 9: Get URL and Test
```powershell
$URL = "https://$APP_NAME.azurewebsites.net"
Write-Host "Your API: $URL"

# Test
curl "$URL/health"
curl "$URL/api/NewsArticle"
```

---

## 🔧 Configure via Azure Portal

### Alternative: Use Azure Portal GUI

1. **Go to**: https://portal.azure.com
2. **Create Resource** → **Web App**
3. **Configure**:
   - Subscription: Free Trial
   - Resource Group: newsportal-rg (create new)
   - Name: newsportal-api-XXXX
   - Publish: **Docker Container**
   - Operating System: **Linux**
   - Region: West Europe
   - App Service Plan: **B1** (or F1 for free)
4. **Docker Tab**:
   - Options: Single Container
   - Image Source: Azure Container Registry (or Docker Hub)
   - Registry: (your ACR)
   - Image: newsportal-api
   - Tag: latest
5. **Create** → Wait 2-3 minutes
6. **Configuration** → **Application Settings** → Add all environment variables
7. **Save** and **Restart**

---

## 📊 Cost Breakdown

### Free Tier (F1):
- Cost: **$0/month**
- RAM: 1GB
- Storage: 1GB
- CPU: 60 min/day
- Limitations: Shared infrastructure, slower
- Best for: Testing, low traffic

### Basic Tier (B1):
- Cost: **~₺200-300/month** (~$6-10)
- RAM: 1.75GB
- Storage: 10GB
- CPU: 100 ACU
- Features: Always on, auto-scale
- Best for: Production, medium traffic

### Standard Tier (S1):
- Cost: **~₺600-800/month** (~$20-25)
- RAM: 1.75GB
- Storage: 50GB
- CPU: 100 ACU
- Features: Staging slots, backups, custom domains
- Best for: Production, high availability

### Additional Costs:
- Container Registry (Basic): ~₺15-20/month (~$0.50)
- Bandwidth: Free for first 15GB, then ~₺25/GB
- MongoDB Atlas: **Free** (M0 tier)
- Cloudflare R2: **Free** (10GB)

**Total Recommended**: ~₺200-350/month for B1 tier

---

## 🐛 Troubleshooting

### 1. Container Won't Start
```powershell
# View logs
az webapp log tail --name $APP_NAME --resource-group $RESOURCE_GROUP

# Or in portal: App Service → Log stream
```

### 2. Wrong Port
Ensure these are set:
```powershell
ASPNETCORE_URLS=http://+:8080
WEBSITES_PORT=8080
```

### 3. MongoDB Connection Failed
- Check connection string has correct username/password
- Verify MongoDB Atlas allows Azure IPs (use 0.0.0.0/0 for testing)
- Check firewall rules in Atlas

### 4. 404 on All Endpoints
- Container might not be running
- Check Docker logs: Portal → Monitoring → Log stream
- Verify Dockerfile is correct

### 5. Slow Performance
- Upgrade from F1 to B1 tier
- Enable "Always On" in Configuration → General Settings

### 6. Update Docker Image
```powershell
# Rebuild and push
az acr build --registry $ACR_NAME --image newsportal-api:latest .

# Restart app
az webapp restart --name $APP_NAME --resource-group $RESOURCE_GROUP
```

---

## 🔄 Continuous Deployment

### Option 1: GitHub Actions (Recommended)

Create `.github/workflows/azure-deploy.yml`:
```yaml
name: Deploy to Azure

on:
  push:
    branches: [ master, main ]

jobs:
  deploy:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      
      - name: Login to Azure
        uses: azure/login@v1
        with:
          creds: ${{ secrets.AZURE_CREDENTIALS }}
      
      - name: Build and push image
        uses: azure/docker-login@v1
        with:
          login-server: ${{ secrets.ACR_LOGIN_SERVER }}
          username: ${{ secrets.ACR_USERNAME }}
          password: ${{ secrets.ACR_PASSWORD }}
      
      - run: |
          docker build -t ${{ secrets.ACR_LOGIN_SERVER }}/newsportal-api:latest .
          docker push ${{ secrets.ACR_LOGIN_SERVER }}/newsportal-api:latest
      
      - name: Deploy to App Service
        uses: azure/webapps-deploy@v2
        with:
          app-name: ${{ secrets.AZURE_WEBAPP_NAME }}
          images: ${{ secrets.ACR_LOGIN_SERVER }}/newsportal-api:latest
```

### Option 2: Azure DevOps Pipeline

### Option 3: Enable Webhook
```powershell
az webapp deployment container config --enable-cd true `
    --name $APP_NAME `
    --resource-group $RESOURCE_GROUP
```

---

## 📚 Additional Resources

- **Azure Portal**: https://portal.azure.com
- **Azure CLI Docs**: https://docs.microsoft.com/cli/azure/
- **App Service Pricing**: https://azure.microsoft.com/pricing/details/app-service/
- **Azure Free Account**: https://azure.microsoft.com/free/
- **Azure Status**: https://status.azure.com/

---

## ✅ Next Steps After Deployment

1. **Update GitHub Actions secrets**:
   ```
   API_BASE_URL=https://newsportal-api-XXXX.azurewebsites.net
   ```

2. **Deploy Frontend to Vercel**:
   ```
   NEXT_PUBLIC_API_URL=https://newsportal-api-XXXX.azurewebsites.net
   ```

3. **Setup Custom Domain** (optional):
   - Buy domain from Namecheap/GoDaddy
   - Configure in Azure Portal → Custom Domains
   - Free SSL certificate included

4. **Enable Application Insights** (monitoring):
   - Azure Portal → App Service → Application Insights
   - Free tier: 5GB data/month

5. **Configure Alerts**:
   - Azure Portal → Alerts → New Alert Rule
   - Get notified of downtime or errors

---

**Ready to deploy?** Run: `.\scripts\deploy-to-azure.ps1`
