# MinIO Setup Guide for Windows 10 (Without Docker)

## 🚀 Quick Start - MinIO Native Installation

### **Option 1: Download MinIO Executable (Recommended)**

#### **Step 1: Download MinIO**
```powershell
# Create MinIO directory
New-Item -ItemType Directory -Path "C:\minio" -Force
cd C:\minio

# Download MinIO server (latest version)
Invoke-WebRequest -Uri "https://dl.min.io/server/minio/release/windows-amd64/minio.exe" -OutFile "minio.exe"
```

#### **Step 2: Create Data Directory**
```powershell
# Create directory for storing images
New-Item -ItemType Directory -Path "C:\minio\data" -Force
```

#### **Step 3: Start MinIO Server**
```powershell
# Set environment variables
$env:MINIO_ROOT_USER = "minioadmin"
$env:MINIO_ROOT_PASSWORD = "minioadmin123"

# Start MinIO server
.\minio.exe server C:\minio\data --console-address ":9001"
```

**Or create a startup script:**

Create file: `C:\minio\start-minio.bat`
```bat
@echo off
set MINIO_ROOT_USER=minioadmin
set MINIO_ROOT_PASSWORD=minioadmin123
C:\minio\minio.exe server C:\minio\data --console-address ":9001"
pause
```

Then just double-click `start-minio.bat` to start MinIO!

#### **Step 4: Access MinIO**
- **Console UI**: http://localhost:9001
- **API Endpoint**: http://localhost:9000
- **Username**: minioadmin
- **Password**: minioadmin123

---

### **Option 2: MongoDB Atlas for Images (Cloud-based Alternative)**

If you don't want to run MinIO locally, you can store images as Base64 in MongoDB or use MongoDB GridFS.

**Not recommended** for production, but works for development:
- Store small images directly in NewsArticle
- Use MongoDB GridFS for larger files
- Or use a free cloud storage service

---

### **Option 3: Use Cloudflare R2 / AWS S3 Free Tier**

**Cloudflare R2** (10GB free forever):
- Compatible with MinIO S3 API
- No egress fees
- Just change endpoint in appsettings.json

**AWS S3 Free Tier** (5GB for 12 months):
- Change endpoint to S3
- Update credentials

---

## 🔧 Configure Your News API

### **Update appsettings.Development.json**

```json
{
  "MinioSettings": {
    "Endpoint": "localhost:9000",
    "AccessKey": "minioadmin",
    "SecretKey": "minioadmin123",
    "BucketName": "news-images",
    "UseSSL": false,
    "MaxFileSizeBytes": 10485760,
    "ThumbnailWidth": 400,
    "ThumbnailHeight": 300
  }
}
```

---

## ✅ Verify MinIO is Running

```powershell
# Test if MinIO is accessible
Invoke-WebRequest -Uri "http://localhost:9000/minio/health/live"

# Should return: HTTP 200 OK
```

---

## 🎯 Start Development Workflow

### **Terminal 1: Start MinIO**
```powershell
cd C:\minio
.\start-minio.bat
```

### **Terminal 2: Start MongoDB**
```powershell
# If MongoDB is installed as Windows Service
net start MongoDB

# Or run mongod manually
mongod --dbpath C:\data\db
```

### **Terminal 3: Start News API**
```powershell
cd c:\dev\news-api\backend
dotnet run
```

---

## 📦 MongoDB Installation (If Not Installed)

### **Option 1: MongoDB Community Server**
```powershell
# Download from: https://www.mongodb.com/try/download/community
# Run installer
# Choose "Complete" installation
# Install as Windows Service

# Verify installation
mongo --version
```

### **Option 2: Use MongoDB Atlas (Cloud - Free)**
- Sign up at https://www.mongodb.com/cloud/atlas
- Create free M0 cluster (512MB)
- Get connection string
- Update appsettings.json:
  ```json
  "MongoDbSettings": {
    "ConnectionString": "mongodb+srv://username:password@cluster.mongodb.net/NewsDb",
    "DatabaseName": "NewsDb"
  }
  ```

---

## 🚨 Troubleshooting

### **MinIO won't start?**
```powershell
# Check if port 9000 or 9001 is already in use
netstat -ano | findstr :9000
netstat -ano | findstr :9001

# If in use, kill the process or change MinIO ports
.\minio.exe server C:\minio\data --address ":9002" --console-address ":9003"
```

### **"Access Denied" errors?**
```powershell
# Run PowerShell as Administrator
# Or move MinIO to user directory
New-Item -ItemType Directory -Path "$env:USERPROFILE\minio" -Force
```

### **MongoDB connection fails?**
```powershell
# Check if MongoDB is running
Get-Service -Name MongoDB

# Start MongoDB service
net start MongoDB

# Or use MongoDB Atlas (cloud) instead
```

---

## 🎉 Quick Test

Once MinIO and MongoDB are running, test your setup:

```powershell
# Start your API
cd c:\dev\news-api\backend
dotnet run

# Open Swagger
start http://localhost:5000/swagger

# Try uploading an image via Swagger UI:
# POST /api/newsarticle/{id}/image
```

---

## 💡 Recommended for Windows 10 Without Docker

**Best Setup:**
1. ✅ **MinIO** - Download .exe, run with batch file
2. ✅ **MongoDB Atlas** - Free cloud database (no local install needed)
3. ✅ **News API** - dotnet run

**Advantages:**
- No Docker required
- Fast startup
- Easy to debug
- Works on all Windows 10 versions

---

## 📝 Create Permanent MinIO Service (Optional)

If you want MinIO to always run in background:

### **Using NSSM (Non-Sucking Service Manager)**

```powershell
# Download NSSM
Invoke-WebRequest -Uri "https://nssm.cc/release/nssm-2.24.zip" -OutFile "nssm.zip"
Expand-Archive -Path "nssm.zip" -DestinationPath "C:\nssm"

# Install MinIO as Windows Service
C:\nssm\nssm-2.24\win64\nssm.exe install MinIO "C:\minio\minio.exe" "server C:\minio\data --console-address :9001"

# Set environment variables for the service
C:\nssm\nssm-2.24\win64\nssm.exe set MinIO AppEnvironmentExtra MINIO_ROOT_USER=minioadmin MINIO_ROOT_PASSWORD=minioadmin123

# Start the service
net start MinIO
```

---

**Ready to proceed?** Start MinIO with the batch file and let me know when it's running! 🚀
