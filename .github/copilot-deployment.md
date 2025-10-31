# 💡 GitHub Copilot - Deployment Instructions

## Quick Deployment Reference

When a user asks to **build**, **deploy**, or **run the backend**, use this simple approach:

### Single Command Deployment

```powershell
# Windows
.\deploy.ps1

# Linux/Mac  
docker compose up -d --build
```

That's it! Everything runs in Docker containers.

### What It Does

1. ✅ Builds .NET 9 API
2. ✅ Starts MongoDB (port 27017)
3. ✅ Starts MinIO (ports 9000, 9001)
4. ✅ Starts Backend API (port 5000)
5. ✅ Waits for health checks
6. ✅ Shows service URLs

### Service URLs

- **API**: http://localhost:5000
- **Swagger**: http://localhost:5000/swagger
- **MongoDB Admin**: http://localhost:8081 (admin/admin123)
- **MinIO Console**: http://localhost:9001 (minioadmin/minioadmin123)

## Common Operations

### Deploy Script Options

```powershell
.\deploy.ps1           # Deploy/update
.\deploy.ps1 -Clean    # Clean rebuild (deletes data)
.\deploy.ps1 -Stop     # Stop all services
.\deploy.ps1 -Status   # Check service status
.\deploy.ps1 -Logs     # View logs
```

### Docker Compose Commands

```bash
docker compose up -d --build    # Build and start
docker compose down             # Stop (keep data)
docker compose down -v          # Stop and delete data
docker compose ps               # List containers
docker compose logs -f          # Follow logs
docker compose restart <service> # Restart service
```

## Architecture

All services run in Docker containers:

```
newsportal-backend    → .NET 9 API (port 5000)
newsportal-mongodb    → MongoDB 7.0 (port 27017)
newsportal-minio      → MinIO S3 storage (ports 9000/9001)
newsportal-mongo-express → DB admin UI (port 8081)
```

## Troubleshooting Guide

### Docker Not Running
**Error**: "Docker is not running"
**Fix**: Start Docker Desktop

### Port Conflicts
**Error**: "Port already in use"
**Fix**: 
```powershell
# Find process using port
netstat -ano | findstr :5000

# Kill process
taskkill /PID <PID> /F
```

### Backend Not Healthy
**Error**: "Backend not responding"
**Fix**:
```powershell
# Check logs
docker compose logs newsportal-backend

# Restart
docker compose restart newsportal-backend
```

### Database Connection Failed
**Fix**:
```powershell
# Restart MongoDB
docker compose restart mongodb

# Or clean rebuild
.\deploy.ps1 -Clean
```

### Image Upload Fails
**Fix**:
```powershell
# Restart MinIO
docker compose restart minio minio-init

# Check bucket exists
docker exec newsportal-minio mc ls myminio/
```

## File Structure

```
newsportal/
├── deploy.ps1              ← Main deployment script
├── docker-compose.yml      ← Docker services definition
├── Dockerfile             ← Backend API build instructions
├── .env                   ← Environment variables (gitignored)
├── .env.example           ← Environment template
└── .github/
    └── DEPLOYMENT.md      ← This file
```

## Environment Variables

Default values in `.env`:

```env
# MongoDB
MONGO_ROOT_USER=admin
MONGO_ROOT_PASSWORD=password123
MONGO_DATABASE=NewsDb

# MinIO
MINIO_ROOT_USER=minioadmin
MINIO_ROOT_PASSWORD=minioadmin123

# JWT
JWT_SECRET_KEY=your-super-secret-jwt-key-at-least-32-characters-long

# Backend
ASPNETCORE_ENVIRONMENT=Production
```

## Configuration Files

- **`docker-compose.yml`**: Service definitions (MongoDB, MinIO, API)
- **`Dockerfile`**: Multi-stage build for .NET API
- **`.env`**: Environment variables (created from `.env.example`)
- **`deploy.ps1`**: PowerShell deployment script
- **`backend/appsettings.json`**: API configuration

## Health Checks

All services have health checks:

```yaml
Backend API:  curl http://localhost:5000/health
MongoDB:      mongosh ping
MinIO:        mc ready local
```

## Deployment Workflow

1. User runs `.\deploy.ps1`
2. Script checks Docker is running
3. Creates `.env` if missing
4. Runs `docker compose up -d --build`
5. Waits for health checks (30-60s)
6. Shows service URLs
7. Done! ✅

## Production Notes

For production deployment:
- Change all passwords in `.env`
- Use Azure Container Apps (see `docs/DEPLOY.md`)
- Or use same Docker Compose on server with:
  - Reverse proxy for HTTPS
  - Automatic MongoDB backups
  - Monitoring/logging

## User Prompts to Watch For

When user says:
- "deploy backend" → `.\deploy.ps1`
- "build backend" → `.\deploy.ps1`
- "start backend" → `.\deploy.ps1`
- "run backend" → `.\deploy.ps1`
- "stop backend" → `.\deploy.ps1 -Stop`
- "backend not working" → `.\deploy.ps1 -Logs`
- "clean rebuild" → `.\deploy.ps1 -Clean`

**Always recommend Docker Compose!** It's the simplest and most reliable way.

## Why Docker Compose?

✅ **Simple**: One command deploys everything
✅ **Consistent**: Same environment everywhere
✅ **Fast**: Cached builds, instant restarts
✅ **Complete**: Includes DB, storage, API
✅ **Isolated**: No local installations needed
✅ **Reliable**: Health checks ensure everything works

## Alternative: Local Run (Not Recommended)

If user insists on running without Docker:

```powershell
# Start MongoDB locally (must be installed)
mongod --dbpath C:\data\db

# Start MinIO locally (must be installed)
minio server C:\minio-data

# Run backend
cd backend
dotnet run
```

**But Docker is way easier!** 🐳

---

**Last Updated**: October 31, 2025  
**Primary Method**: Docker Compose (`.\deploy.ps1`)  
**Complexity**: ⭐ Very Simple
