# 🚀 News Portal Deployment Guide

## Quick Start (5 minutes)

### Prerequisites
- **Docker Desktop** installed and running ([Download here](https://www.docker.com/products/docker-desktop/))
- That's it! No .NET SDK, MongoDB, or MinIO installation needed.

### One-Command Deployment

```powershell
# Windows PowerShell
.\deploy.ps1
```

```bash
# Linux/Mac
docker compose up -d --build
```

That's all! The script will:
1. ✅ Check if Docker is running
2. ✅ Create `.env` file if missing
3. ✅ Build the .NET API
4. ✅ Start MongoDB database
5. ✅ Start MinIO object storage
6. ✅ Start the backend API
7. ✅ Wait for health checks to pass

## 🌐 Access Your Services

After deployment completes:

| Service | URL | Credentials |
|---------|-----|-------------|
| **Backend API** | http://localhost:5000 | - |
| **Swagger Docs** | http://localhost:5000/swagger | - |
| **MongoDB Admin** | http://localhost:8081 | admin / admin123 |
| **MinIO Console** | http://localhost:9001 | minioadmin / minioadmin123 |

## 📋 Common Commands

### Deploy/Update
```powershell
.\deploy.ps1
```
Builds and starts all services. If already running, updates them.

### Clean Rebuild
```powershell
.\deploy.ps1 -Clean
```
Removes all containers and volumes, then rebuilds from scratch. **⚠️ This deletes all data!**

### Stop Services
```powershell
.\deploy.ps1 -Stop
```
Stops all running containers (data is preserved).

### Check Status
```powershell
.\deploy.ps1 -Status
```
Shows which services are running and their health status.

### View Logs
```powershell
.\deploy.ps1 -Logs
```
Shows real-time logs from all services (Ctrl+C to exit).

### View Backend Logs Only
```powershell
docker compose logs -f newsportal-backend
```

## 🔧 Manual Docker Commands

If you prefer manual control:

```bash
# Build and start
docker compose up -d --build

# Stop (keep data)
docker compose down

# Stop and delete data
docker compose down -v

# View logs
docker compose logs -f

# Check status
docker compose ps

# Restart a service
docker compose restart newsportal-backend
```

## 🐛 Troubleshooting

### "Docker is not running"
**Solution**: Start Docker Desktop and wait for it to fully start.

### Port Already in Use
**Problem**: Port 5000, 27017, 9000, 9001, or 8081 is already in use.

**Solution**:
```powershell
# Find what's using the port (example for port 5000)
netstat -ano | findstr :5000

# Kill the process by PID
taskkill /PID <PID> /F
```

Or edit `docker-compose.yml` to change the ports.

### Backend Not Starting
```powershell
# Check backend logs
docker compose logs newsportal-backend

# Common issues:
# - MongoDB not ready: Wait 30 more seconds
# - Port conflict: See above
# - Build error: Check logs for compilation errors
```

### Cannot Connect to Database
```powershell
# Restart MongoDB
docker compose restart mongodb

# Or rebuild everything
.\deploy.ps1 -Clean
```

### Image Upload Fails
```powershell
# Check MinIO is running
docker compose ps minio

# Restart MinIO
docker compose restart minio minio-init

# Verify bucket exists (should show 'news-images')
docker exec newsportal-minio mc ls myminio/
```

## 🏗️ Architecture

The deployment includes:

```
┌─────────────────────────────────────┐
│   Backend API (.NET 9)              │
│   Port: 5000                        │
│   Health: /health                   │
└─────────────────┬───────────────────┘
                  │
         ┌────────┴────────┐
         │                 │
┌────────▼──────┐  ┌──────▼─────────┐
│   MongoDB     │  │     MinIO      │
│   Port: 27017 │  │   Port: 9000   │
│   (Database)  │  │   (Images)     │
└───────────────┘  └────────────────┘
```

## 🔐 Environment Variables

Edit `.env` file to customize:

```env
# MongoDB
MONGO_ROOT_USER=admin
MONGO_ROOT_PASSWORD=password123
MONGO_DATABASE=NewsDb

# MinIO
MINIO_ROOT_USER=minioadmin
MINIO_ROOT_PASSWORD=minioadmin123

# JWT (Change in production!)
JWT_SECRET_KEY=your-super-secret-jwt-key-at-least-32-characters-long

# Environment
ASPNETCORE_ENVIRONMENT=Production
```

**⚠️ Security**: Change all passwords before deploying to production!

## 📊 Health Checks

The deployment includes automatic health checks:

```powershell
# Check if backend is healthy
curl http://localhost:5000/health

# Expected response: HTTP 200 OK
```

Health checks verify:
- ✅ API is responding
- ✅ MongoDB connection
- ✅ MinIO connection

## 🚀 Production Deployment

For production, see [Azure Deployment](../docs/DEPLOY.md) or use the same Docker Compose setup on your server:

1. Install Docker on your server
2. Clone the repository
3. Update `.env` with secure passwords
4. Run `docker compose up -d --build`
5. Configure reverse proxy (nginx/traefik) for HTTPS
6. Set up automatic backups for MongoDB

## 📚 Additional Resources

- **API Documentation**: Once running, visit http://localhost:5000/swagger
- **Architecture Guide**: [docs/ARCHITECTURE.md](../docs/ARCHITECTURE.md)
- **Build Instructions**: [docs/BUILD.md](../docs/BUILD.md)
- **Full Deployment Guide**: [docs/DEPLOY.md](../docs/DEPLOY.md)

## 🆘 Getting Help

If you encounter issues:

1. Check logs: `.\deploy.ps1 -Logs`
2. Verify services: `.\deploy.ps1 -Status`
3. Try clean rebuild: `.\deploy.ps1 -Clean`
4. Check GitHub Issues: https://github.com/bkalafat/newsportal/issues

---

**Last Updated**: October 31, 2025  
**Deployment Method**: Docker Compose  
**Complexity**: ⭐ Simple (One command!)
