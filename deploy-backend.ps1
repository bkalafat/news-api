#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Deploy backend to Azure Container Apps

.DESCRIPTION
    Simple deployment script for .NET backend to Azure Container Apps.
    Uses Docker to build and push image, then updates Azure Container App.

.PARAMETER ResourceGroup
    Azure resource group name (default: newsportal-rg)

.PARAMETER ContainerApp
    Container app name (default: newsportal-backend)

.PARAMETER Registry
    Azure Container Registry name (default: newsportal)

.EXAMPLE
    .\deploy-backend.ps1
    Deploy with default settings

.EXAMPLE
    .\deploy-backend.ps1 -ResourceGroup "my-rg" -ContainerApp "my-app"
    Deploy with custom resource group and app name
#>

param(
    [string]$ResourceGroup = "newsportal-rg",
    [string]$ContainerApp = "newsportal-backend",
    [string]$Registry = "newsportal"
)

Write-Host "🚀 Backend Deployment to Azure" -ForegroundColor Cyan
Write-Host "===============================" -ForegroundColor Cyan
Write-Host ""
Write-Host "📋 Configuration:" -ForegroundColor Yellow
Write-Host "   Resource Group: $ResourceGroup"
Write-Host "   Container App: $ContainerApp"
Write-Host "   Registry: $Registry"
Write-Host ""

# Check if Azure CLI is installed
if (-not (Get-Command az -ErrorAction SilentlyContinue)) {
    Write-Host "❌ Azure CLI not found!" -ForegroundColor Red
    Write-Host "   Install from: https://aka.ms/azure-cli" -ForegroundColor Yellow
    exit 1
}

# Check if Docker is installed
if (-not (Get-Command docker -ErrorAction SilentlyContinue)) {
    Write-Host "❌ Docker not found!" -ForegroundColor Red
    Write-Host "   Install Docker Desktop from: https://docker.com" -ForegroundColor Yellow
    exit 1
}

# Login to Azure
Write-Host "🔐 Checking Azure login..." -ForegroundColor Yellow
$azAccount = az account show 2>$null | ConvertFrom-Json
if (-not $azAccount) {
    Write-Host "   Please login to Azure..." -ForegroundColor Yellow
    az login
    if ($LASTEXITCODE -ne 0) {
        Write-Host "❌ Azure login failed!" -ForegroundColor Red
        exit 1
    }
}
Write-Host "✅ Logged in as: $($azAccount.user.name)" -ForegroundColor Green
Write-Host ""

# Login to Azure Container Registry
Write-Host "🔐 Logging into Azure Container Registry..." -ForegroundColor Yellow
az acr login --name $Registry
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ ACR login failed!" -ForegroundColor Red
    exit 1
}
Write-Host "✅ ACR login successful" -ForegroundColor Green
Write-Host ""

# Build Docker image
$imageName = "$Registry.azurecr.io/newsapi"
$gitSha = (git rev-parse --short HEAD 2>$null) ?? "latest"
$imageTag = "${imageName}:${gitSha}"

Write-Host "🔨 Building Docker image..." -ForegroundColor Yellow
Write-Host "   Image: $imageTag" -ForegroundColor Gray

docker build -t $imageTag -t "${imageName}:latest" -f Dockerfile .

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Docker build failed!" -ForegroundColor Red
    exit 1
}
Write-Host "✅ Docker build completed" -ForegroundColor Green
Write-Host ""

# Push to Azure Container Registry
Write-Host "📤 Pushing image to ACR..." -ForegroundColor Yellow
docker push $imageTag
docker push "${imageName}:latest"

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Docker push failed!" -ForegroundColor Red
    exit 1
}
Write-Host "✅ Image pushed successfully" -ForegroundColor Green
Write-Host ""

# Update Container App
Write-Host "☁️  Updating Azure Container App..." -ForegroundColor Yellow
az containerapp update `
    --name $ContainerApp `
    --resource-group $ResourceGroup `
    --image $imageTag

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Container App update failed!" -ForegroundColor Red
    exit 1
}
Write-Host "✅ Container App updated" -ForegroundColor Green
Write-Host ""

# Get the URL
Write-Host "🌐 Getting application URL..." -ForegroundColor Yellow
$appUrl = az containerapp show `
    --name $ContainerApp `
    --resource-group $ResourceGroup `
    --query "properties.configuration.ingress.fqdn" `
    --output tsv

if ($appUrl) {
    Write-Host ""
    Write-Host "✅ Deployment completed successfully!" -ForegroundColor Green
    Write-Host ""
    Write-Host "📝 Application URLs:" -ForegroundColor Cyan
    Write-Host "   API: https://$appUrl" -ForegroundColor White
    Write-Host "   Health: https://$appUrl/health" -ForegroundColor White
    Write-Host "   Swagger: https://$appUrl/swagger" -ForegroundColor White
    Write-Host ""
    Write-Host "💡 Useful commands:" -ForegroundColor Yellow
    Write-Host "   View logs: az containerapp logs show --name $ContainerApp --resource-group $ResourceGroup --follow"
    Write-Host "   Check status: az containerapp show --name $ContainerApp --resource-group $ResourceGroup"
} else {
    Write-Host "⚠️  Could not retrieve application URL" -ForegroundColor Yellow
}
