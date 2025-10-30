# Simple Azure Deployment
$ErrorActionPreference = "Stop"

Write-Host "Starting Azure deployment..." -ForegroundColor Cyan

$subscriptionId = "19acb272-bb61-400b-acc4-8b7a2f7aa0cc"
$resourceGroup = "newsportal-rg"
$location = "westeurope"
$projectName = "newsportal"

Write-Host "Setting subscription..."
az account set --subscription $subscriptionId

Write-Host "Creating resource group..."
az group create --name $resourceGroup --location $location

Write-Host "Creating Azure Container Registry..."
try {
    az acr show --name $projectName --resource-group $resourceGroup 2>&1 | Out-Null
    Write-Host "ACR already exists"
} catch {
    Write-Host "ACR does not exist, creating..."
    az acr create --name $projectName --resource-group $resourceGroup --sku Basic --admin-enabled true
}

Write-Host "Building Docker image..."
$acrLoginServer = az acr show --name $projectName --resource-group $resourceGroup --query loginServer -o tsv
az acr login --name $projectName

Set-Location C:\dev\newsportal
docker build -t "${acrLoginServer}/newsportal-backend:latest" -f Dockerfile .
docker push "${acrLoginServer}/newsportal-backend:latest"

Write-Host "Creating Container Apps Environment..."
az containerapp env show --name "${projectName}-env" --resource-group $resourceGroup 2>&1 | Out-Null
if ($LASTEXITCODE -ne 0) {
    Write-Host "Environment does not exist, creating..."
    az containerapp env create --name "${projectName}-env" --resource-group $resourceGroup --location $location
} else {
    Write-Host "Environment already exists"
}

Write-Host "Enter Cloudflare R2 Secret Key:"
$minioSecretKey = Read-Host -AsSecureString
$minioSecretKeyPlain = [Runtime.InteropServices.Marshal]::PtrToStringAuto([Runtime.InteropServices.Marshal]::SecureStringToBSTR($minioSecretKey))

Write-Host "Generating JWT Secret..."
$jwtSecret = [Convert]::ToBase64String([System.Text.Encoding]::UTF8.GetBytes([System.Guid]::NewGuid().ToString() + [System.Guid]::NewGuid().ToString()))

Write-Host "Creating Container App..."
$registryPassword = az acr credential show --name $projectName --query "passwords[0].value" -o tsv

az containerapp create `
    --name "${projectName}-backend" `
    --resource-group $resourceGroup `
    --environment "${projectName}-env" `
    --image "${acrLoginServer}/newsportal-backend:latest" `
    --target-port 8080 `
    --ingress external `
    --registry-server $acrLoginServer `
    --registry-username $projectName `
    --registry-password $registryPassword `
    --cpu 0.25 `
    --memory 0.5Gi `
    --min-replicas 0 `
    --max-replicas 1 `
    --secrets mongodb-connection="mongodb+srv://bkalafat:dbuserpassword123@cluster0.xwbfl1o.mongodb.net/NewsDb" minio-secret-key="$minioSecretKeyPlain" unsplash-key="ATK1ZDXPJKfdlYpm10AY1QNTvjzN4GymqQdH3JRP3qdU" jwt-secret="$jwtSecret" `
    --env-vars ASPNETCORE_ENVIRONMENT=Production MongoDbSettings__ConnectionString=secretref:mongodb-connection MongoDbSettings__DatabaseName=NewsDb MongoDbSettings__NewsCollectionName=News MinioSettings__Endpoint=7ac015923324a4d426c1f7782c3f41e1.r2.cloudflarestorage.com MinioSettings__AccessKey=cc61a30e775100a198836d97dbce0d79 MinioSettings__SecretKey=secretref:minio-secret-key MinioSettings__BucketName=news-images MinioSettings__UseSSL=true UnsplashSettings__AccessKey=secretref:unsplash-key JwtSettings__SecretKey=secretref:jwt-secret JwtSettings__Issuer=NewsApi JwtSettings__Audience=NewsApiUsers JwtSettings__ExpirationMinutes=60

Write-Host "Getting backend URL..."
$backendUrl = az containerapp show --name "${projectName}-backend" --resource-group $resourceGroup --query properties.configuration.ingress.fqdn -o tsv

Write-Host "Backend URL: https://$backendUrl" -ForegroundColor Green
Write-Host "Testing health endpoint..."
Start-Sleep -Seconds 15
Invoke-WebRequest -Uri "https://$backendUrl/health" -UseBasicParsing

Write-Host "Deployment complete!" -ForegroundColor Green
Write-Host "Backend: https://$backendUrl"
Write-Host "Next: Create Static Web App in Azure Portal for frontend"
