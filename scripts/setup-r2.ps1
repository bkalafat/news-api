# Cloudflare R2 Setup Script for News Portal
# This script configures Cloudflare R2 storage for the News Portal application

param(
    [Parameter(Mandatory=$false)]
    [string]$Environment = "production",
    
    [Parameter(Mandatory=$false)]
    [string]$ResourceGroup = "newsportal-rg",
    
    [Parameter(Mandatory=$false)]
    [string]$ContainerAppName = "newsportal-backend"
)

# Set error action preference
$ErrorActionPreference = "Stop"

Write-Host "╔════════════════════════════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "║    Cloudflare R2 Storage Setup for News Portal            ║" -ForegroundColor Cyan
Write-Host "╚════════════════════════════════════════════════════════════╝" -ForegroundColor Cyan
Write-Host ""

# Function to validate R2 credentials format
function Test-R2Credentials {
    param(
        [string]$AccessKey,
        [string]$SecretKey
    )
    
    if ([string]::IsNullOrWhiteSpace($AccessKey) -or $AccessKey.Length -lt 32) {
        Write-Host "❌ Invalid Access Key format (should be at least 32 characters)" -ForegroundColor Red
        return $false
    }
    
    if ([string]::IsNullOrWhiteSpace($SecretKey) -or $SecretKey.Length -lt 32) {
        Write-Host "❌ Invalid Secret Key format (should be at least 32 characters)" -ForegroundColor Red
        return $false
    }
    
    return $true
}

# Step 1: Gather R2 Configuration
Write-Host "📋 Step 1: Cloudflare R2 Configuration" -ForegroundColor Yellow
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Yellow
Write-Host ""

Write-Host "To get your R2 credentials:" -ForegroundColor White
Write-Host "1. Go to: https://dash.cloudflare.com/ → R2 → Manage R2 API Tokens" -ForegroundColor Gray
Write-Host "2. Click 'Create API Token'" -ForegroundColor Gray
Write-Host "3. Configure: Object Read & Write permissions for 'news-images' bucket" -ForegroundColor Gray
Write-Host "4. Copy the Access Key ID and Secret Access Key" -ForegroundColor Gray
Write-Host ""

# Get Account ID
Write-Host "Enter your Cloudflare Account ID:" -ForegroundColor Cyan
Write-Host "(Found in R2 dashboard URL or sidebar)" -ForegroundColor Gray
$accountId = Read-Host "Account ID"

if ([string]::IsNullOrWhiteSpace($accountId)) {
    Write-Host "❌ Account ID is required!" -ForegroundColor Red
    exit 1
}

# Construct R2 endpoint
$r2Endpoint = "$accountId.r2.cloudflarestorage.com"
Write-Host "✅ R2 Endpoint: $r2Endpoint" -ForegroundColor Green
Write-Host ""

# Get Access Key
Write-Host "Enter your R2 Access Key ID:" -ForegroundColor Cyan
$accessKey = Read-Host "Access Key ID"

if ([string]::IsNullOrWhiteSpace($accessKey)) {
    Write-Host "❌ Access Key is required!" -ForegroundColor Red
    exit 1
}

# Get Secret Key (securely)
Write-Host ""
Write-Host "Enter your R2 Secret Access Key:" -ForegroundColor Cyan
$secretKeySecure = Read-Host "Secret Access Key" -AsSecureString
$secretKey = [Runtime.InteropServices.Marshal]::PtrToStringAuto([Runtime.InteropServices.Marshal]::SecureStringToBSTR($secretKeySecure))

if ([string]::IsNullOrWhiteSpace($secretKey)) {
    Write-Host "❌ Secret Key is required!" -ForegroundColor Red
    exit 1
}

# Validate credentials
Write-Host ""
Write-Host "🔍 Validating credentials format..." -ForegroundColor Yellow
if (-not (Test-R2Credentials -AccessKey $accessKey -SecretKey $secretKey)) {
    Write-Host "⚠️  Credentials may be invalid. Continue anyway? (y/n)" -ForegroundColor Yellow
    $continue = Read-Host
    if ($continue -ne 'y') {
        exit 1
    }
}

# Get bucket name
Write-Host ""
Write-Host "Enter your R2 bucket name (default: news-images):" -ForegroundColor Cyan
$bucketName = Read-Host "Bucket Name"
if ([string]::IsNullOrWhiteSpace($bucketName)) {
    $bucketName = "news-images"
}

Write-Host ""
Write-Host "✅ Configuration collected successfully!" -ForegroundColor Green
Write-Host ""

# Step 2: Choose deployment target
Write-Host "📦 Step 2: Select Deployment Target" -ForegroundColor Yellow
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Yellow
Write-Host ""
Write-Host "1. Azure Container Apps (Production)" -ForegroundColor White
Write-Host "2. Local Development (.env file)" -ForegroundColor White
Write-Host "3. Both" -ForegroundColor White
Write-Host ""
$deployTarget = Read-Host "Select option (1-3)"

# Step 3: Configure based on target
if ($deployTarget -eq "1" -or $deployTarget -eq "3") {
    Write-Host ""
    Write-Host "☁️  Step 3a: Configuring Azure Container Apps" -ForegroundColor Yellow
    Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Yellow
    Write-Host ""
    
    # Check if Azure CLI is installed
    try {
        $azVersion = az version | ConvertFrom-Json
        Write-Host "✅ Azure CLI detected: $($azVersion.'azure-cli')" -ForegroundColor Green
    }
    catch {
        Write-Host "❌ Azure CLI not found. Please install: https://aka.ms/installazurecli" -ForegroundColor Red
        exit 1
    }
    
    # Check if logged in
    Write-Host "🔍 Checking Azure login status..." -ForegroundColor Yellow
    try {
        $account = az account show | ConvertFrom-Json
        Write-Host "✅ Logged in as: $($account.user.name)" -ForegroundColor Green
    }
    catch {
        Write-Host "⚠️  Not logged in to Azure. Logging in..." -ForegroundColor Yellow
        az login
    }
    
    Write-Host ""
    Write-Host "📝 Setting Container App secrets..." -ForegroundColor Cyan
    
    # Set secrets
    try {
        az containerapp secret set `
            --name $ContainerAppName `
            --resource-group $ResourceGroup `
            --secrets `
                minio-access-key="$accessKey" `
                minio-secret-key="$secretKey" `
            --output none
        
        Write-Host "✅ Secrets configured successfully!" -ForegroundColor Green
    }
    catch {
        Write-Host "❌ Failed to set secrets: $_" -ForegroundColor Red
        Write-Host "You can set them manually in Azure Portal" -ForegroundColor Yellow
    }
    
    Write-Host ""
    Write-Host "🔧 Updating environment variables..." -ForegroundColor Cyan
    
    # Update environment variables
    try {
        az containerapp update `
            --name $ContainerAppName `
            --resource-group $ResourceGroup `
            --set-env-vars `
                "MinioSettings__Endpoint=$r2Endpoint" `
                "MinioSettings__BucketName=$bucketName" `
                "MinioSettings__UseSSL=true" `
                "MinioSettings__Region=auto" `
            --replace-env-vars `
                "MinioSettings__AccessKey=secretref:minio-access-key" `
                "MinioSettings__SecretKey=secretref:minio-secret-key" `
            --output none
        
        Write-Host "✅ Environment variables updated successfully!" -ForegroundColor Green
    }
    catch {
        Write-Host "❌ Failed to update environment variables: $_" -ForegroundColor Red
        Write-Host "You can update them manually in Azure Portal" -ForegroundColor Yellow
    }
    
    Write-Host ""
    Write-Host "🔄 Restarting container app..." -ForegroundColor Cyan
    try {
        az containerapp revision restart `
            --name $ContainerAppName `
            --resource-group $ResourceGroup `
            --output none
        
        Write-Host "✅ Container restarted successfully!" -ForegroundColor Green
    }
    catch {
        Write-Host "⚠️  Failed to restart automatically. Please restart manually." -ForegroundColor Yellow
    }
}

if ($deployTarget -eq "2" -or $deployTarget -eq "3") {
    Write-Host ""
    Write-Host "💻 Step 3b: Configuring Local Development" -ForegroundColor Yellow
    Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Yellow
    Write-Host ""
    
    $envPath = Join-Path $PSScriptRoot ".." ".env"
    
    # Check if .env exists
    if (Test-Path $envPath) {
        Write-Host "⚠️  .env file exists. Backup current file? (y/n)" -ForegroundColor Yellow
        $backup = Read-Host
        if ($backup -eq 'y') {
            $backupPath = "$envPath.backup.$(Get-Date -Format 'yyyyMMdd-HHmmss')"
            Copy-Item $envPath $backupPath
            Write-Host "✅ Backup created: $backupPath" -ForegroundColor Green
        }
    }
    
    # Create or update .env
    $envContent = @"
# Cloudflare R2 Configuration (Updated $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss'))
R2_ACCESS_KEY_ID=$accessKey
R2_SECRET_ACCESS_KEY=$secretKey
R2_ENDPOINT=$r2Endpoint
R2_BUCKET_NAME=$bucketName

# For Docker Compose (using R2 instead of MinIO)
MINIO_ROOT_USER=$accessKey
MINIO_ROOT_PASSWORD=$secretKey

# MongoDB (keep existing if you have it)
MONGO_ROOT_USER=admin
MONGO_ROOT_PASSWORD=password123
MONGO_DATABASE=NewsDb

# JWT (keep existing if you have it)
JWT_SECRET_KEY=your-super-secret-jwt-key-that-is-at-least-32-characters-long

# Mongo Express (keep existing if you have it)
MONGOEXPRESS_USER=admin
MONGOEXPRESS_PASSWORD=admin123

# ASP.NET Core
ASPNETCORE_ENVIRONMENT=Production
"@
    
    try {
        $envContent | Out-File -FilePath $envPath -Encoding UTF8 -Force
        Write-Host "✅ .env file updated successfully!" -ForegroundColor Green
        Write-Host "📁 Location: $envPath" -ForegroundColor Gray
    }
    catch {
        Write-Host "❌ Failed to write .env file: $_" -ForegroundColor Red
    }
    
    # Update appsettings.Development.json
    Write-Host ""
    Write-Host "📝 Updating appsettings.Development.json..." -ForegroundColor Cyan
    
    $appSettingsPath = Join-Path $PSScriptRoot ".." "backend" "appsettings.Development.json"
    
    if (Test-Path $appSettingsPath) {
        try {
            $appSettings = Get-Content $appSettingsPath -Raw | ConvertFrom-Json
            
            # Update MinioSettings
            if (-not $appSettings.MinioSettings) {
                $appSettings | Add-Member -MemberType NoteProperty -Name MinioSettings -Value @{}
            }
            
            $appSettings.MinioSettings.Endpoint = $r2Endpoint
            $appSettings.MinioSettings.AccessKey = $accessKey
            $appSettings.MinioSettings.SecretKey = $secretKey
            $appSettings.MinioSettings.BucketName = $bucketName
            $appSettings.MinioSettings.UseSSL = $true
            $appSettings.MinioSettings.Region = "auto"
            
            $appSettings | ConvertTo-Json -Depth 10 | Set-Content $appSettingsPath -Encoding UTF8
            
            Write-Host "✅ appsettings.Development.json updated successfully!" -ForegroundColor Green
        }
        catch {
            Write-Host "⚠️  Failed to update appsettings.Development.json: $_" -ForegroundColor Yellow
            Write-Host "Please update manually." -ForegroundColor Yellow
        }
    }
    else {
        Write-Host "⚠️  appsettings.Development.json not found at: $appSettingsPath" -ForegroundColor Yellow
    }
}

# Step 4: Test Connection
Write-Host ""
Write-Host "🧪 Step 4: Test R2 Connection (Optional)" -ForegroundColor Yellow
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Yellow
Write-Host ""
Write-Host "Would you like to test the R2 connection? (y/n)" -ForegroundColor Cyan
$testConnection = Read-Host

if ($testConnection -eq 'y') {
    Write-Host "🔍 Testing connection to R2..." -ForegroundColor Cyan
    Write-Host ""
    
    # Test using AWS CLI S3 commands (if available)
    try {
        $awsVersion = aws --version 2>$null
        Write-Host "✅ AWS CLI detected" -ForegroundColor Green
        
        # Configure AWS CLI temporarily
        $env:AWS_ACCESS_KEY_ID = $accessKey
        $env:AWS_SECRET_ACCESS_KEY = $secretKey
        
        Write-Host "📋 Listing buckets..." -ForegroundColor Cyan
        aws s3 ls --endpoint-url "https://$r2Endpoint"
        
        Write-Host ""
        Write-Host "✅ Connection test completed!" -ForegroundColor Green
    }
    catch {
        Write-Host "⚠️  AWS CLI not found. Skipping connection test." -ForegroundColor Yellow
        Write-Host "Install AWS CLI to test: https://aws.amazon.com/cli/" -ForegroundColor Gray
    }
}

# Step 5: Summary
Write-Host ""
Write-Host "╔════════════════════════════════════════════════════════════╗" -ForegroundColor Green
Write-Host "║                Setup Complete! ✅                          ║" -ForegroundColor Green
Write-Host "╚════════════════════════════════════════════════════════════╝" -ForegroundColor Green
Write-Host ""

Write-Host "📊 Configuration Summary:" -ForegroundColor Yellow
Write-Host "  • R2 Endpoint: $r2Endpoint" -ForegroundColor White
Write-Host "  • Bucket Name: $bucketName" -ForegroundColor White
Write-Host "  • SSL Enabled: Yes" -ForegroundColor White
Write-Host "  • Region: auto" -ForegroundColor White
Write-Host ""

if ($deployTarget -eq "1" -or $deployTarget -eq "3") {
    Write-Host "☁️  Azure Container Apps:" -ForegroundColor Cyan
    Write-Host "  • Resource Group: $ResourceGroup" -ForegroundColor White
    Write-Host "  • Container App: $ContainerAppName" -ForegroundColor White
    Write-Host "  • Secrets: ✅ Configured" -ForegroundColor Green
    Write-Host "  • Environment Variables: ✅ Updated" -ForegroundColor Green
    Write-Host ""
}

if ($deployTarget -eq "2" -or $deployTarget -eq "3") {
    Write-Host "💻 Local Development:" -ForegroundColor Cyan
    Write-Host "  • .env file: ✅ Updated" -ForegroundColor Green
    Write-Host "  • appsettings.Development.json: ✅ Updated" -ForegroundColor Green
    Write-Host ""
}

Write-Host "📚 Next Steps:" -ForegroundColor Yellow
Write-Host "  1. Test image upload via Swagger UI" -ForegroundColor White
Write-Host "  2. Verify images appear in R2 bucket" -ForegroundColor White
Write-Host "  3. Consider enabling R2.dev subdomain for public access" -ForegroundColor White
Write-Host "  4. Review CLOUDFLARE_R2_SETUP.md for advanced configuration" -ForegroundColor White
Write-Host ""

Write-Host "🔗 Useful Links:" -ForegroundColor Yellow
Write-Host "  • R2 Dashboard: https://dash.cloudflare.com/$accountId/r2" -ForegroundColor Cyan
Write-Host "  • Documentation: CLOUDFLARE_R2_SETUP.md" -ForegroundColor Cyan
Write-Host ""

Write-Host "Thank you for using News Portal! 🚀" -ForegroundColor Green
Write-Host ""

# Cleanup sensitive variables
$secretKey = $null
$accessKey = $null
[System.GC]::Collect()
