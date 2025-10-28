# Azure CLI Installation Guide for Windows

## Option 1: MSI Installer (Recommended)
# Download from: https://aka.ms/installazurecliwindows
# Or run this PowerShell command:

Write-Host "Downloading Azure CLI installer..." -ForegroundColor Cyan
$ProgressPreference = 'SilentlyContinue'
Invoke-WebRequest -Uri https://aka.ms/installazurecliwindows -OutFile .\AzureCLI.msi

Write-Host "Installing Azure CLI..." -ForegroundColor Green
Start-Process msiexec.exe -Wait -ArgumentList '/I AzureCLI.msi /quiet'

Write-Host "✅ Azure CLI installed!" -ForegroundColor Green
Write-Host "Please restart PowerShell and run: az --version" -ForegroundColor Yellow

## Option 2: Winget (Windows Package Manager)
# winget install -e --id Microsoft.AzureCLI

## Option 3: PowerShell (Advanced)
# Invoke-WebRequest -Uri https://aka.ms/installazurecliwindows -OutFile .\AzureCLI.msi; Start-Process msiexec.exe -Wait -ArgumentList '/I AzureCLI.msi /quiet'

## Verify Installation
# Open new PowerShell window and run:
# az --version
# az login
