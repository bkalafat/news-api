# Build Guide

## Prerequisites

- **Docker Desktop** (recommended) OR
- **.NET 9 SDK** + **MongoDB** + **MinIO** (for local development)

## Docker Build (Recommended)

### 1. Build and Start All Services

```bash
# Start all services (MongoDB, MinIO, Backend API, Mongo Express)
docker compose up -d --build

# View logs
docker compose logs -f newsportal-backend

# Check service status
docker compose ps
```

### 2. Verify Services

| Service | URL | Credentials |
|---------|-----|-------------|
| **Backend API** | http://localhost:5000 | N/A |
| **Swagger UI** | http://localhost:5000/swagger | N/A |
| **Mongo Express** | http://localhost:8081 | admin / admin123 |
| **MinIO Console** | http://localhost:9001 | minioadmin / minioadmin123 |

### 3. Health Check

```bash
curl http://localhost:5000/health
```

Expected response: `HTTP 200 OK`

### 4. Stop Services

```bash
# Stop all containers (preserves data)
docker compose down

# Stop and remove ALL data (reset to clean state)
docker compose down -v
```

## Local Build (Without Docker)

### 1. Install Dependencies

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [MongoDB Community Server](https://www.mongodb.com/try/download/community)
- [MinIO](https://min.io/download) (optional, for image storage)

### 2. Start MongoDB

```bash
# Default connection: mongodb://localhost:27017
mongod --dbpath /path/to/data
```

### 3. Configure Secrets

```bash
cd backend

# Set MongoDB connection string
dotnet user-secrets set "MongoDbSettings:ConnectionString" "mongodb://localhost:27017"
dotnet user-secrets set "MongoDbSettings:DatabaseName" "NewsDb"

# Set JWT secret (min 32 characters)
dotnet user-secrets set "JwtSettings:SecretKey" "your-super-secret-jwt-key-at-least-32-characters-long"

# Optional: MinIO configuration
dotnet user-secrets set "MinioSettings:Endpoint" "localhost:9000"
dotnet user-secrets set "MinioSettings:AccessKey" "minioadmin"
dotnet user-secrets set "MinioSettings:SecretKey" "minioadmin123"
```

### 4. Restore and Build

```bash
cd backend
dotnet restore
dotnet build newsApi.csproj
```

### 5. Run Application

```bash
dotnet run --project newsApi.csproj
```

Application will start on: **http://localhost:5000**

## Testing

### Run All Tests

```bash
dotnet test newsApi.sln
```

### Run Specific Test Categories

```bash
# Unit tests only
dotnet test --filter "FullyQualifiedName~Unit"

# Integration tests only
dotnet test --filter "FullyQualifiedName~Integration"
```

### Expected Results

- **Total Tests**: 178+
- **Expected**: All passing
- **Duration**: ~2-3 seconds

## Troubleshooting

### Issue: Docker Build Fails

**Solution**: Ensure Docker Desktop is running and you have an internet connection for NuGet packages.

### Issue: Port 5000 Already in Use

```bash
# Find process using port 5000
netstat -ano | findstr :5000  # Windows
lsof -i :5000                 # Linux/Mac

# Kill the process or change the port in docker-compose.yml
```

### Issue: MongoDB Connection Failed

**Solution**: 
- Verify MongoDB is running: `docker compose ps mongodb`
- Check connection string in `.env` file
- Restart services: `docker compose restart`

### Issue: Tests Fail

**Solution**:
- Clean and rebuild: `dotnet clean && dotnet build`
- Check test output for specific failures
- Ensure no background services are interfering

## Build Artifacts

After successful build:

```
backend/
├── bin/Debug/net9.0/
│   ├── newsApi.dll        # Main application
│   └── newsApi.exe        # Executable (Windows)
└── obj/
    └── Debug/net9.0/      # Intermediate build files
```

## CI/CD Integration

For GitHub Actions or Azure DevOps pipelines, see:
- `.github/workflows/azure-backend-deploy.yml` - GitHub Actions
- `azure/README.md` - Azure DevOps setup

## Next Steps

- [Run locally or in Docker](./RUN.md)
- [Deploy to production](./DEPLOY.md)
- [Architecture overview](../README.md#project-structure)
