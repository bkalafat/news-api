# Azure Deployment Guide for News API

This guide provides step-by-step instructions for deploying the News API to Azure using Azure Container Apps, Cosmos DB (MongoDB API), and Azure Blob Storage.

## 📋 Prerequisites

Before starting, ensure you have:

- **Azure Account** with an active subscription ([Create free account](https://azure.microsoft.com/free/))
- **Azure CLI** installed ([Installation guide](https://learn.microsoft.com/cli/azure/install-azure-cli))
- **Docker** installed and running
- **GitHub account** (for CI/CD with GitHub Actions)
- **.NET 9 SDK** installed

## 🏗️ Azure Architecture

The deployment creates the following Azure resources:

1. **Azure Container Apps** - Hosts the .NET API with auto-scaling
2. **Azure Cosmos DB (MongoDB API)** - Managed MongoDB database
3. **Azure Blob Storage** - Replaces MinIO for image storage
4. **Azure Container Registry** - Private Docker image registry
5. **Log Analytics Workspace** - Centralized logging
6. **Application Insights** - Application monitoring and telemetry

## 🚀 Deployment Options

### Option 1: Automated Deployment via GitHub Actions (Recommended)

#### Step 1: Configure GitHub Secrets

Navigate to your GitHub repository → Settings → Secrets and variables → Actions, and add the following secrets:

```bash
# Azure Service Principal (create with: az ad sp create-for-rbac)
AZURE_CREDENTIALS='{"clientId":"xxx","clientSecret":"xxx","subscriptionId":"xxx","tenantId":"xxx"}'

# Azure Container Registry name (without .azurecr.io)
ACR_NAME=newsapidevacr

# JWT Secret Key (min 32 characters)
JWT_SECRET_KEY=your-super-secret-jwt-key-that-is-at-least-32-characters-long

# MongoDB Admin Password
MONGO_ADMIN_PASSWORD=YourSecurePassword123!
```

#### Step 2: Create Azure Service Principal

```bash
# Login to Azure
az login

# Create service principal with contributor role
az ad sp create-for-rbac \
  --name "newsapi-github-actions" \
  --role contributor \
  --scopes /subscriptions/{subscription-id} \
  --sdk-auth

# Copy the JSON output and save it as AZURE_CREDENTIALS secret in GitHub
```

#### Step 3: Trigger Deployment

Push to `main` (production) or `develop` (dev) branch, or manually trigger via GitHub Actions:

1. Go to **Actions** tab in GitHub
2. Select **Deploy to Azure Container Apps**
3. Click **Run workflow**
4. Select environment (dev/staging/prod)
5. Click **Run workflow**

The workflow will:
- Build the Docker image
- Push to Azure Container Registry
- Deploy infrastructure using Bicep
- Deploy the container app
- Run health checks

---

### Option 2: Manual Deployment via Azure CLI

#### Step 1: Login to Azure

```bash
# Login to Azure
az login

# Set default subscription (if you have multiple)
az account set --subscription "Your Subscription Name"

# Verify selected subscription
az account show
```

#### Step 2: Create Azure Container Registry

```bash
# Set variables
RESOURCE_GROUP="newsapi-rg-dev"
LOCATION="eastus"
ACR_NAME="newsapidevacr"  # Must be globally unique, lowercase, no hyphens

# Create resource group
az group create \
  --name $RESOURCE_GROUP \
  --location $LOCATION

# Create Azure Container Registry
az acr create \
  --resource-group $RESOURCE_GROUP \
  --name $ACR_NAME \
  --sku Basic \
  --admin-enabled true

# Get ACR credentials
az acr credential show --name $ACR_NAME
```

#### Step 3: Build and Push Docker Image

```bash
# Login to ACR
az acr login --name $ACR_NAME

# Build Docker image (run from project root)
docker build -t ${ACR_NAME}.azurecr.io/newsapi:latest -f Dockerfile .

# Push to ACR
docker push ${ACR_NAME}.azurecr.io/newsapi:latest
```

#### Step 4: Deploy Infrastructure with Bicep

```bash
# Set deployment parameters
JWT_SECRET="your-super-secret-jwt-key-min-32-chars"
MONGO_PASSWORD="YourSecurePassword123!"
CONTAINER_IMAGE="${ACR_NAME}.azurecr.io/newsapi:latest"

# Deploy Bicep template
az deployment group create \
  --resource-group $RESOURCE_GROUP \
  --template-file azure/bicep/main.bicep \
  --parameters environmentName=dev \
  --parameters appName=newsapi \
  --parameters containerImage=$CONTAINER_IMAGE \
  --parameters jwtSecretKey=$JWT_SECRET \
  --parameters mongoAdminPassword=$MONGO_PASSWORD \
  --parameters minReplicas=1 \
  --parameters maxReplicas=3

# Get deployment outputs
az deployment group show \
  --resource-group $RESOURCE_GROUP \
  --name main \
  --query properties.outputs
```

#### Step 5: Verify Deployment

```bash
# Get Container App URL
APP_URL=$(az deployment group show \
  --resource-group $RESOURCE_GROUP \
  --name main \
  --query properties.outputs.containerAppUrl.value \
  --output tsv)

echo "Application URL: $APP_URL"

# Test health endpoint
curl $APP_URL/health

# Test Swagger UI
echo "Swagger UI: $APP_URL/swagger"
```

---

## 🔧 Post-Deployment Configuration

### 1. Configure CORS for Frontend

Update the CORS configuration in `backend/Presentation/Extensions/ServiceCollectionExtensions.cs`:

```csharp
services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", builder =>
    {
        builder
   .WithOrigins(
"https://your-frontend-domain.com",
        "https://{containerAppName}.{region}.azurecontainerapps.io"
            )
            .AllowAnyHeader()
   .AllowAnyMethod()
    .AllowCredentials();
    });
});
```

Redeploy after changes:

```bash
docker build -t ${ACR_NAME}.azurecr.io/newsapi:latest -f Dockerfile .
docker push ${ACR_NAME}.azurecr.io/newsapi:latest

az containerapp update \
  --name newsapi-dev-app \
  --resource-group $RESOURCE_GROUP \
  --image ${ACR_NAME}.azurecr.io/newsapi:latest
```

### 2. Configure Custom Domain (Optional)

```bash
# Add custom domain to Container App
az containerapp hostname add \
  --name newsapi-dev-app \
  --resource-group $RESOURCE_GROUP \
  --hostname api.yourdomain.com

# Bind SSL certificate
az containerapp hostname bind \
  --name newsapi-dev-app \
  --resource-group $RESOURCE_GROUP \
  --hostname api.yourdomain.com \
  --certificate {certificate-id} \
  --environment newsapi-dev-env
```

### 3. Enable Application Insights Monitoring

Application Insights is automatically configured. View telemetry:

```bash
# Open Application Insights in Azure Portal
az portal open \
  --resource-id /subscriptions/{subscription-id}/resourceGroups/$RESOURCE_GROUP/providers/Microsoft.Insights/components/newsapi-dev-insights
```

Or access via: [Azure Portal](https://portal.azure.com) → Application Insights → newsapi-dev-insights

---

## 🔒 Security Best Practices

### 1. Use Azure Key Vault for Secrets (Production)

```bash
# Create Key Vault
az keyvault create \
  --name newsapi-vault-prod \
  --resource-group newsapi-rg-prod \
  --location eastus

# Store secrets
az keyvault secret set \
  --vault-name newsapi-vault-prod \
  --name jwt-secret-key \
  --value "your-super-secret-jwt-key"

az keyvault secret set \
  --vault-name newsapi-vault-prod \
  --name mongo-admin-password \
  --value "YourSecurePassword123!"

# Update parameters.prod.json with Key Vault references (already configured)
```

### 2. Enable Managed Identity

```bash
# Enable system-assigned managed identity for Container App
az containerapp identity assign \
  --name newsapi-prod-app \
  --resource-group newsapi-rg-prod \
  --system-assigned

# Grant Key Vault access to managed identity
az keyvault set-policy \
  --name newsapi-vault-prod \
  --object-id {managed-identity-principal-id} \
  --secret-permissions get list
```

### 3. Restrict Network Access

```bash
# Configure Cosmos DB firewall
az cosmosdb update \
  --name newsapi-prod-cosmos \
  --resource-group newsapi-rg-prod \
  --ip-range-filter "0.0.0.0/0"  # Replace with specific IPs

# Configure Storage Account network rules
az storage account update \
  --name newsapiprodstorage \
  --resource-group newsapi-rg-prod \
  --default-action Deny

az storage account network-rule add \
  --account-name newsapiprodstorage \
  --resource-group newsapi-rg-prod \
  --ip-address {container-app-outbound-ip}
```

---

## 📊 Monitoring and Troubleshooting

### View Container App Logs

```bash
# Stream live logs
az containerapp logs show \
  --name newsapi-dev-app \
  --resource-group $RESOURCE_GROUP \
  --follow

# View recent logs
az containerapp logs show \
  --name newsapi-dev-app \
  --resource-group $RESOURCE_GROUP \
  --tail 100
```

### Check Application Health

```bash
# Get app status
az containerapp show \
  --name newsapi-dev-app \
  --resource-group $RESOURCE_GROUP \
  --query properties.runningStatus

# Get replica count
az containerapp replica list \
  --name newsapi-dev-app \
  --resource-group $RESOURCE_GROUP
```

### Access Cosmos DB

```bash
# Get connection string
az cosmosdb keys list \
  --name newsapi-dev-cosmos \
  --resource-group $RESOURCE_GROUP \
  --type connection-strings

# Connect with mongosh (if installed)
mongosh "mongodb://newsapi-dev-cosmos.mongo.cosmos.azure.com:10255/?ssl=true" \
  --username newsapiadmin \
  --password {password}
```

### Common Issues

**1. Container App not starting**
- Check logs: `az containerapp logs show`
- Verify image is pushed: `az acr repository show-tags --name $ACR_NAME --repository newsapi`
- Check environment variables: `az containerapp show --query properties.template.containers[0].env`

**2. MongoDB connection errors**
- Verify Cosmos DB is running: `az cosmosdb show`
- Check connection string in Container App secrets
- Ensure MongoDB API version compatibility (7.0)

**3. Image upload failures**
- Verify Storage Account exists: `az storage account show`
- Check container permissions: `az storage container show --name news-images`
- Review Blob Storage connection string in app config

---

## 💰 Cost Optimization

### Development Environment

```bash
# Scale down to 0 replicas when not in use
az containerapp update \
  --name newsapi-dev-app \
  --resource-group newsapi-rg-dev \
  --min-replicas 0 \
  --max-replicas 1

# Use Cosmos DB serverless for dev
# (Update bicep template with serverless capability)

# Delete dev environment when not needed
az group delete --name newsapi-rg-dev --yes --no-wait
```

### Estimated Monthly Costs (USD)

| Resource | Dev | Production |
|----------|-----|------------|
| Container Apps | $0-20 | $50-200 |
| Cosmos DB | $25 | $100-500 |
| Blob Storage | $1-5 | $10-50 |
| Container Registry | $5 | $5-20 |
| Application Insights | Free-$5 | $20-100 |
| **Total** | **$31-55** | **$185-870** |

---

## 🔄 CI/CD Pipeline Details

The GitHub Actions workflow (`.github/workflows/azure-deploy.yml`) includes:

1. **Build Job**
   - Checks out code
   - Builds Docker image
   - Pushes to ACR
   - Uploads Bicep templates

2. **Deploy Job**
   - Creates/updates Azure resources
   - Deploys container app
   - Runs health checks
   - Creates GitHub deployment

3. **Notify Job**
   - Reports deployment status
   - Can be extended with Slack/Teams notifications

**Trigger Conditions:**
- Push to `main` → Deploy to production
- Push to `develop` → Deploy to dev
- Manual workflow dispatch → Deploy to selected environment

---

## 📚 Additional Resources

- [Azure Container Apps Documentation](https://learn.microsoft.com/azure/container-apps/)
- [Azure Cosmos DB for MongoDB](https://learn.microsoft.com/azure/cosmos-db/mongodb/introduction)
- [Azure Bicep Documentation](https://learn.microsoft.com/azure/azure-resource-manager/bicep/)
- [GitHub Actions for Azure](https://github.com/Azure/actions)

---

## 🆘 Support

For issues or questions:
1. Check Azure Portal for resource status
2. Review Application Insights logs
3. Check GitHub Actions workflow logs
4. Review this documentation

**Need help?** Open an issue in the GitHub repository with:
- Azure resource group name
- Environment (dev/staging/prod)
- Error messages from logs
- Steps to reproduce
