#!/bin/bash

# ============================================
# Quick Azure Deployment Script - API Only
# ============================================
# Deploys only the News API Container App to Azure
# Uses external MongoDB and MinIO services

set -e  # Exit on error

# ============================================
# Configuration
# ============================================
echo "🚀 News API - Azure Quick Deploy (API Only)"
echo "============================================"
echo ""

# Prompt for required values
read -p "Enter environment (dev/staging/prod) [dev]: " ENVIRONMENT
ENVIRONMENT=${ENVIRONMENT:-dev}

read -p "Enter Azure region [eastus]: " LOCATION
LOCATION=${LOCATION:-eastus}

read -p "Enter app name [newsapi]: " APP_NAME
APP_NAME=${APP_NAME:-newsapi}

read -p "Enter Azure Container Registry name (lowercase, no hyphens): " ACR_NAME
if [ -z "$ACR_NAME" ]; then
    echo "❌ ACR name is required"
    exit 1
fi

echo ""
echo "🔐 Security Configuration"
echo "========================"

read -sp "Enter JWT Secret Key (min 32 chars): " JWT_SECRET
echo ""
if [ ${#JWT_SECRET} -lt 32 ]; then
    echo "❌ JWT Secret must be at least 32 characters"
    exit 1
fi

echo ""
echo "🗄️  External MongoDB Configuration"
echo "==================================="

read -p "Enter MongoDB connection string: " MONGO_CONNECTION
if [ -z "$MONGO_CONNECTION" ]; then
    echo "❌ MongoDB connection string is required"
    exit 1
fi

read -p "Enter MongoDB database name [NewsDb]: " MONGO_DB
MONGO_DB=${MONGO_DB:-NewsDb}

echo ""
echo "📦 External MinIO Configuration"
echo "==============================="

read -p "Enter MinIO endpoint (e.g., minio.yourdomain.com:9000): " MINIO_ENDPOINT
if [ -z "$MINIO_ENDPOINT" ]; then
    echo "❌ MinIO endpoint is required"
    exit 1
fi

read -p "Enter MinIO access key: " MINIO_ACCESS_KEY
if [ -z "$MINIO_ACCESS_KEY" ]; then
    echo "❌ MinIO access key is required"
    exit 1
fi

read -sp "Enter MinIO secret key: " MINIO_SECRET_KEY
echo ""
if [ -z "$MINIO_SECRET_KEY" ]; then
 echo "❌ MinIO secret key is required"
    exit 1
fi

read -p "Enter MinIO bucket name [news-images]: " MINIO_BUCKET
MINIO_BUCKET=${MINIO_BUCKET:-news-images}

# Set derived variables
RESOURCE_GROUP="${APP_NAME}-rg-${ENVIRONMENT}"
CONTAINER_IMAGE="${ACR_NAME}.azurecr.io/${APP_NAME}:latest"

echo ""
echo "📋 Deployment Configuration:"
echo "   Environment: $ENVIRONMENT"
echo "   Location:            $LOCATION"
echo "   Resource Group:      $RESOURCE_GROUP"
echo "   ACR Name:            $ACR_NAME"
echo "   Container Image:     $CONTAINER_IMAGE"
echo "   MongoDB Database:    $MONGO_DB"
echo "   MinIO Endpoint:      $MINIO_ENDPOINT"
echo "   MinIO Bucket:        $MINIO_BUCKET"
echo ""

read -p "Continue with deployment? (yes/no): " CONFIRM
if [ "$CONFIRM" != "yes" ]; then
    echo "❌ Deployment cancelled"
    exit 0
fi

# ============================================
# Azure Login
# ============================================
echo ""
echo "🔐 Step 1/7: Logging in to Azure..."
if ! az account show &> /dev/null; then
    az login
fi

SUBSCRIPTION=$(az account show --query name -o tsv)
echo "✅ Logged in to subscription: $SUBSCRIPTION"

# ============================================
# Create Resource Group
# ============================================
echo ""
echo "📦 Step 2/7: Creating Resource Group..."
az group create \
    --name $RESOURCE_GROUP \
    --location $LOCATION \
    --output none

echo "✅ Resource group created: $RESOURCE_GROUP"

# ============================================
# Create Azure Container Registry
# ============================================
echo ""
echo "🐳 Step 3/7: Creating Azure Container Registry..."

if az acr show --name $ACR_NAME --resource-group $RESOURCE_GROUP &> /dev/null; then
    echo "ℹ️  ACR already exists, skipping creation"
else
    az acr create \
      --resource-group $RESOURCE_GROUP \
        --name $ACR_NAME \
        --sku Basic \
        --admin-enabled true \
        --output none
    
    echo "✅ Container Registry created: $ACR_NAME"
fi

# ============================================
# Build and Push Docker Image
# ============================================
echo ""
echo "🔨 Step 4/7: Building and pushing Docker image..."

# Login to ACR
az acr login --name $ACR_NAME

# Build image (from project root)
echo "   Building Docker image..."
cd ..
docker build -t $CONTAINER_IMAGE -f Dockerfile .
cd azure/scripts

# Push image
echo "   Pushing Docker image to ACR..."
docker push $CONTAINER_IMAGE

echo "✅ Docker image pushed: $CONTAINER_IMAGE"

# ============================================
# Create Parameters File
# ============================================
echo ""
echo "📝 Step 5/7: Preparing deployment parameters..."

PARAMS_FILE="parameters.${ENVIRONMENT}.temp.json"
cat > $PARAMS_FILE <<EOF
{
  "\$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#",
  "contentVersion": "1.0.0.0",
  "parameters": {
    "environmentName": {
  "value": "$ENVIRONMENT"
    },
    "appName": {
      "value": "$APP_NAME"
    },
    "containerImage": {
      "value": "$CONTAINER_IMAGE"
    },
    "jwtSecretKey": {
"value": "$JWT_SECRET"
    },
    "mongoConnectionString": {
      "value": "$MONGO_CONNECTION"
},
    "mongoDatabaseName": {
      "value": "$MONGO_DB"
    },
    "minioEndpoint": {
      "value": "$MINIO_ENDPOINT"
    },
    "minioAccessKey": {
      "value": "$MINIO_ACCESS_KEY"
    },
    "minioSecretKey": {
      "value": "$MINIO_SECRET_KEY"
 },
    "minioBucketName": {
      "value": "$MINIO_BUCKET"
 },
    "minReplicas": {
      "value": $([ "$ENVIRONMENT" == "prod" ] && echo "2" || echo "1")
    },
    "maxReplicas": {
  "value": $([ "$ENVIRONMENT" == "prod" ] && echo "10" || echo "3")
    },
    "containerCpu": {
      "value": "0.5"
    },
 "containerMemory": {
   "value": "1Gi"
    }
  }
}
EOF

echo "✅ Parameters file created"

# ============================================
# Deploy Infrastructure
# ============================================
echo ""
echo "☁️  Step 6/7: Deploying Azure infrastructure (this may take 5-10 minutes)..."

cd ../bicep
az deployment group create \
    --resource-group $RESOURCE_GROUP \
    --template-file main.bicep \
    --parameters ../../$PARAMS_FILE \
    --output none

# Clean up temporary parameters file
cd ../../
rm $PARAMS_FILE

echo "✅ Infrastructure deployed successfully"

# ============================================
# Get Deployment Outputs
# ============================================
echo ""
echo "📊 Step 7/7: Retrieving deployment information..."

APP_URL=$(az deployment group show \
    --resource-group $RESOURCE_GROUP \
    --name main \
    --query properties.outputs.containerAppUrl.value \
    --output tsv)

ACR_SERVER=$(az deployment group show \
    --resource-group $RESOURCE_GROUP \
    --name main \
    --query properties.outputs.containerRegistryLoginServer.value \
    --output tsv)

# ============================================
# Health Check
# ============================================
echo ""
echo "🏥 Running health check..."
sleep 30

if curl -f -s "$APP_URL/health" > /dev/null; then
    echo "✅ Health check passed!"
else
    echo "⚠️  Health check failed - application may still be starting"
fi

# ============================================
# Summary
# ============================================
echo ""
echo "============================================"
echo "✅ Deployment Complete!"
echo "============================================"
echo ""
echo "📍 Application URL:"
echo "   $APP_URL"
echo ""
echo "📚 Swagger Documentation:"
echo "   $APP_URL/swagger"
echo ""
echo "🏥 Health Endpoint:"
echo "   $APP_URL/health"
echo ""
echo "🐳 Container Registry:"
echo "   $ACR_SERVER"
echo ""
echo "🔧 Resource Group:"
echo "   $RESOURCE_GROUP"
echo ""
echo "============================================"
echo ""
echo "📖 Next Steps:"
echo "   1. Test API endpoints using Swagger UI"
echo "   2. Verify MongoDB connection"
echo "3. Test image upload to MinIO"
echo "   4. Configure CORS for your frontend"
echo "   5. Set up Azure DevOps pipeline"
echo ""
echo "🆘 View logs:"
echo "   az containerapp logs show --name ${APP_NAME}-${ENVIRONMENT}-app --resource-group $RESOURCE_GROUP --follow"
echo ""
