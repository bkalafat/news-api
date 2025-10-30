# Azure Deployment - Quick Start Guide

This guide helps you deploy the entire News Portal application to Azure using the free tier and external services.

## 🎯 Current Deployment Status

✅ **Resource Group**: `newsportal-rg` (created)  
✅ **Container Registry**: `newsportal.azurecr.io` (created)  
⏳ **Backend Container App**: Not deployed yet  
⏳ **Frontend Static Web App**: Not deployed yet  

## 📋 Prerequisites Checklist

Before running the deployment scripts, ensure:

- [x] Azure CLI installed and logged in (`az login`)
- [ ] **Docker Desktop is running** (required for building images)
- [x] MongoDB Atlas account (free M0 cluster)
- [x] Cloudflare R2 account (free 10GB storage)
- [x] Unsplash API key
- [x] GitHub account

## 🚀 Deployment Steps

### Step 1: Ensure Docker Desktop is Running

**IMPORTANT**: Docker must be running before deploying the backend.

```powershell
# Check if Docker is running
docker info

# If not running, start Docker Desktop and wait ~1 minute
Start-Process "C:\Program Files\Docker\Docker\Docker Desktop.exe"
Start-Sleep -Seconds 60
```

### Step 2: Deploy Backend to Azure Container Apps

Run the backend deployment script:

```powershell
.\deploy-to-azure.ps1
```

This script will:
1. ✅ Verify Docker is running
2. ✅ Build the Docker image for the backend
3. ✅ Push the image to Azure Container Registry
4. ✅ Create Container Apps environment
5. ✅ Deploy the backend container with all configurations
6. ✅ Test the health endpoint

**Expected Duration**: 5-10 minutes

**What you'll need**:
- Cloudflare R2 Secret Key (you'll be prompted)

**Output**:
- Backend URL: `https://newsportal-backend.{region}.azurecontainerapps.io`
- Swagger UI: `https://newsportal-backend.{region}.azurecontainerapps.io/swagger`

### Step 3: Deploy Frontend to Azure Static Web Apps

**Option A: Via PowerShell Script**

```powershell
.\deploy-frontend-azure.ps1
```

**Option B: Via Azure Portal (Recommended for First-Time)**

1. Go to [Azure Portal](https://portal.azure.com)
2. Click **"Create a resource"** → Search **"Static Web App"**
3. Configure:
   - **Subscription**: Your Azure subscription
   - **Resource Group**: `newsportal-rg`
   - **Name**: `newsportal-frontend`
   - **Plan type**: Free
   - **Region**: West Europe
   - **Source**: GitHub
   - **Organization**: bkalafat
   - **Repository**: newsportal
   - **Branch**: master
   - **Build Presets**: Next.js
   - **App location**: `/frontend`
   - **Output location**: `.next`
4. Click **"Review + Create"** → **"Create"**
5. Wait for deployment (~2-3 minutes)

### Step 4: Configure Environment Variables

After Static Web App is created:

1. Go to the Static Web App in Azure Portal
2. Navigate to **Configuration** → **Environment variables**
3. Add:
   - `NEXT_PUBLIC_API_URL` = `https://newsportal-backend.{region}.azurecontainerapps.io`
   - `NEXT_PUBLIC_UNSPLASH_ACCESS_KEY` = `ATK1ZDXPJKfdlYpm10AY1QNTvjzN4GymqQdH3JRP3qdU`
4. Click **"Save"**
5. Wait for automatic redeployment (~2 minutes)

### Step 5: Update Backend CORS

To allow the frontend to communicate with the backend:

1. Edit `backend/Presentation/Extensions/ServiceCollectionExtensions.cs`
2. Find the CORS configuration
3. Add your frontend URL to the allowed origins:

```csharp
.WithOrigins(
    "https://newsportal-frontend.azurestaticapps.net",  // Add this
    "http://localhost:3000"
)
```

4. Rebuild and redeploy the backend:

```powershell
# Login to ACR
az acr login --name newsportal

# Build and push
docker build -t newsportal.azurecr.io/newsapi:latest -f Dockerfile .
docker push newsportal.azurecr.io/newsapi:latest

# Update container app
az containerapp update `
    --name newsportal-backend `
    --resource-group newsportal-rg `
    --image newsportal.azurecr.io/newsapi:latest
```

## 🔧 Troubleshooting

### Docker Not Running

**Error**: `error during connect: Get "http://%2F%2F.%2Fpipe%2FdockerDesktopLinuxEngine/v1.51/info"`

**Solution**:
```powershell
# Manually start Docker Desktop
Start-Process "C:\Program Files\Docker\Docker\Docker Desktop.exe"

# Wait for it to start (check system tray icon)
# Then run: docker info
```

### Backend Health Check Fails

**Error**: Health check returns 404 or timeout

**Solutions**:
1. Check container logs:
   ```powershell
   az containerapp logs show --name newsportal-backend --resource-group newsportal-rg --follow
   ```
2. Verify environment variables are set correctly
3. Check MongoDB connection string is valid
4. Wait 2-3 minutes for container to fully start

### Frontend Build Fails

**Error**: GitHub Action workflow fails during build

**Solutions**:
1. Check the workflow file created by Azure
2. Ensure `app-location` is `/frontend` and `output-location` is `.next`
3. Check GitHub Actions logs in your repository
4. Verify environment variables are set in Azure Portal

### ACR Login Issues

**Error**: Cannot login to Azure Container Registry

**Solution**:
```powershell
# Get ACR credentials
az acr credential show --name newsportal

# Manual Docker login
docker login newsportal.azurecr.io -u newsportal -p <password-from-above>
```

## 📊 Deployed Resources

After successful deployment, you'll have:

| Resource | Name | Type | Cost |
|----------|------|------|------|
| Resource Group | newsportal-rg | Resource Group | Free |
| Container Registry | newsportal | ACR Basic | ~$5/month |
| Container Environment | newsportal-env | Container Apps | Free tier |
| Backend API | newsportal-backend | Container App | $0-20/month |
| Frontend | newsportal-frontend | Static Web App | Free |
| **External** | MongoDB | Atlas M0 | Free |
| **External** | Object Storage | Cloudflare R2 | Free |

**Total Estimated Cost**: $5-25/month

## 🌐 Application URLs

After deployment:

- **Frontend**: `https://newsportal-frontend.azurestaticapps.net`
- **Backend API**: `https://newsportal-backend.<region>.azurecontainerapps.io`
- **Swagger Docs**: `https://newsportal-backend.<region>.azurecontainerapps.io/swagger`
- **Health Check**: `https://newsportal-backend.<region>.azurecontainerapps.io/health`

## 🔒 Security Considerations

1. **Secrets Management**:
   - JWT Secret is auto-generated and stored in Container App secrets
   - Database credentials are stored as Container App secrets
   - Never commit secrets to Git

2. **CORS Configuration**:
   - Only allow your frontend domain
   - Remove `localhost` in production

3. **HTTPS**:
   - All Azure resources use HTTPS by default
   - Container Apps automatically provision SSL certificates

## 📝 Next Steps

1. ✅ Test the deployed backend: Visit Swagger UI
2. ✅ Test the deployed frontend: Open the Static Web App URL
3. ✅ Configure custom domain (optional)
4. ✅ Set up Application Insights monitoring (optional)
5. ✅ Configure GitHub Actions for CI/CD
6. ✅ Set up staging environment (optional)

## 🔄 CI/CD Pipeline

A GitHub Action workflow is automatically created for the frontend. To set up backend CI/CD:

1. Add `AZURE_CREDENTIALS` secret to GitHub:
   ```powershell
   az ad sp create-for-rbac --name "newsportal-github" --role contributor --scopes /subscriptions/19acb272-bb61-400b-acc4-8b7a2f7aa0cc/resourceGroups/newsportal-rg --sdk-auth
   ```

   This command generates a JSON output with the following structure:
   ```json
   {
     "clientId": "<GUID>",
     "clientSecret": "<STRING>",
     "subscriptionId": "<GUID>",
     "tenantId": "<GUID>"
   }
   ```

2. Copy the entire JSON output and add it as a repository secret named `AZURE_CREDENTIALS`:
   - Go to GitHub repository → Settings → Secrets and variables → Actions
   - Click "New repository secret"
   - Name: `AZURE_CREDENTIALS`
   - Value: Paste the entire JSON output

3. The workflow in `.github/workflows/azure-backend-deploy.yml` will automatically deploy on push to master

## 📚 Additional Resources

- [Azure Container Apps Documentation](https://learn.microsoft.com/azure/container-apps/)
- [Azure Static Web Apps Documentation](https://learn.microsoft.com/azure/static-web-apps/)
- [News Portal Project Documentation](./README.md)
- [Backend API Documentation](./NEWS_API_DOCUMENTATION.md)

## 💡 Tips

- **Scale to Zero**: Backend scales to 0 replicas when idle (saves money)
- **Monitor Costs**: Check Azure Cost Management regularly
- **Logs**: Use `az containerapp logs show` for debugging
- **Updates**: Push to GitHub to auto-deploy frontend, rebuild Docker for backend

---

**Last Updated**: October 29, 2025  
**Status**: Backend and frontend deployment infrastructure ready
