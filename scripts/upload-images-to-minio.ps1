# Upload Sample Images to MinIO for News Articles
# This script downloads sample images from Unsplash and uploads them to MinIO

Write-Host "=== MinIO Image Upload Script ===" -ForegroundColor Cyan
Write-Host ""

# MinIO Configuration
$minioEndpoint = "http://localhost:9000"
$minioBucket = "news-images"
$minioAccessKey = "minioadmin"
$minioSecretKey = "minioadmin"

# Install AWS CLI tools if not present (MinIO is S3-compatible)
if (-not (Get-Command aws -ErrorAction SilentlyContinue)) {
    Write-Host "AWS CLI not found. Installing..." -ForegroundColor Yellow
    # Using chocolatey or winget to install
    if (Get-Command choco -ErrorAction SilentlyContinue) {
        choco install awscli -y
    } elseif (Get-Command winget -ErrorAction SilentlyContinue) {
        winget install Amazon.AWSCLI
    } else {
        Write-Host "Please install AWS CLI manually: https://aws.amazon.com/cli/" -ForegroundColor Red
        exit 1
    }
}

# Configure AWS CLI for MinIO
Write-Host "Configuring AWS CLI for MinIO..." -ForegroundColor Green
$env:AWS_ACCESS_KEY_ID = $minioAccessKey
$env:AWS_SECRET_ACCESS_KEY = $minioSecretKey
$env:AWS_DEFAULT_REGION = "us-east-1"

# Sample images to download and upload
$images = @(
    @{
        Name = "galatasaray-celebration.jpg"
        Url = "https://images.unsplash.com/photo-1574629810360-7efbbe195018?w=1200&q=80&fm=jpg"
        Category = "sports"
    },
    @{
        Name = "ai-technology.jpg"
        Url = "https://images.unsplash.com/photo-1677442136019-21780ecad995?w=1200&q=80&fm=jpg"
        Category = "technology"
    },
    @{
        Name = "mars-surface.jpg"
        Url = "https://images.unsplash.com/photo-1614732414444-096e5f1122d5?w=1200&q=80&fm=jpg"
        Category = "science"
    },
    @{
        Name = "quantum-computer.jpg"
        Url = "https://images.unsplash.com/photo-1635070041078-e363dbe005cb?w=1200&q=80&fm=jpg"
        Category = "technology"
    }
)

# Create temp directory for downloads
$tempDir = "$env:TEMP\minio-uploads"
if (-not (Test-Path $tempDir)) {
    New-Item -ItemType Directory -Path $tempDir | Out-Null
}

Write-Host "Downloading images..." -ForegroundColor Green
foreach ($image in $images) {
    $localPath = Join-Path $tempDir $image.Name
    Write-Host "  - $($image.Name)..." -NoNewline
    
    try {
        Invoke-WebRequest -Uri $image.Url -OutFile $localPath -UseBasicParsing
        Write-Host " ✓" -ForegroundColor Green
    } catch {
        Write-Host " ✗ Failed: $_" -ForegroundColor Red
        continue
    }
}

Write-Host ""
Write-Host "Uploading images to MinIO bucket: $minioBucket" -ForegroundColor Green

foreach ($image in $images) {
    $localPath = Join-Path $tempDir $image.Name
    if (Test-Path $localPath) {
        $s3Path = "$($image.Category)/$($image.Name)"
        Write-Host "  - Uploading $s3Path..." -NoNewline
        
        try {
            aws --endpoint-url $minioEndpoint s3 cp $localPath "s3://$minioBucket/$s3Path" --acl public-read
            Write-Host " ✓" -ForegroundColor Green
            
            # Display public URL
            $publicUrl = "$minioEndpoint/$minioBucket/$s3Path"
            Write-Host "    URL: $publicUrl" -ForegroundColor Cyan
        } catch {
            Write-Host " ✗ Failed: $_" -ForegroundColor Red
        }
    }
}

Write-Host ""
Write-Host "=== Upload Complete ===" -ForegroundColor Green
Write-Host ""
Write-Host "You can access images at: $minioEndpoint/$minioBucket/" -ForegroundColor Cyan
Write-Host "MinIO Console: http://localhost:9001" -ForegroundColor Cyan

# Clean up temp files
Write-Host ""
Write-Host "Cleaning up temporary files..." -ForegroundColor Yellow
Remove-Item -Path $tempDir -Recurse -Force

Write-Host "Done!" -ForegroundColor Green
