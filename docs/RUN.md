# Run Guide

## Quick Start with Docker (Recommended)

### Start All Services

```bash
# Create .env file from example (first time only)
cp .env.example .env

# Start all services
docker compose up -d

# View real-time logs
docker compose logs -f newsportal-backend
```

### Access the Application

| Service | URL | Purpose |
|---------|-----|---------|
| **API** | http://localhost:5000 | REST API endpoints |
| **Swagger** | http://localhost:5000/swagger | Interactive API documentation |
| **MongoDB Admin** | http://localhost:8081 | Database management (admin/admin123) |
| **MinIO Console** | http://localhost:9001 | Object storage admin (minioadmin/minioadmin123) |

### Test the API

```bash
# Health check
curl http://localhost:5000/health

# Get all news articles
curl http://localhost:5000/api/news

# Get news by ID
curl http://localhost:5000/api/news/YOUR_NEWS_ID
```

## Running Locally (Without Docker)

### Prerequisites Running

Ensure these services are running before starting the API:

1. **MongoDB** on `mongodb://localhost:27017`
2. **MinIO** on `localhost:9000` (optional)

### Start the Backend

```bash
cd backend
dotnet run
```

The API will start on http://localhost:5000

## Development Mode

### Hot Reload with Docker

```bash
# Rebuild and restart backend after code changes
docker compose up -d --build newsportal-backend

# View logs
docker compose logs -f newsportal-backend
```

### Local Development

```bash
cd backend
dotnet watch run
```

This will automatically rebuild and restart when you save code changes.

## Frontend (Next.js)

### Prerequisites

- Node.js 18+
- Backend API running

### Start Frontend

```bash
cd frontend

# Install dependencies (first time)
npm install

# Create environment file
cp .env.example .env.local

# Start development server
npm run dev
```

Frontend will be available at: http://localhost:3000

### Configure Frontend

Edit `frontend/.env.local`:

```env
NEXT_PUBLIC_API_URL=http://localhost:5000
NEXT_PUBLIC_SITE_URL=http://localhost:3000
```

## Working with the API

### Authentication

Most endpoints require JWT authentication. Get a token first:

```bash
# Login (via Swagger or curl)
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"admin123"}'
```

Response will include a `token` field. Use it in subsequent requests:

```bash
curl http://localhost:5000/api/news \
  -H "Authorization: Bearer YOUR_TOKEN_HERE"
```

### Using Swagger UI

1. Open http://localhost:5000/swagger
2. Click **Authorize** button (top right)
3. Get token from `/api/auth/login` endpoint
4. Enter: `Bearer YOUR_TOKEN`
5. Now you can test all protected endpoints

### Sample API Calls

**Get all news:**
```bash
GET http://localhost:5000/api/news
```

**Get news by category:**
```bash
GET http://localhost:5000/api/news?category=technology
```

**Create news (requires auth):**
```bash
POST http://localhost:5000/api/news
Authorization: Bearer YOUR_TOKEN
Content-Type: application/json

{
  "category": "technology",
  "type": "article",
  "caption": "Breaking News Title",
  "summary": "Short summary of the news",
  "content": "Full content here...",
  "expressDate": "2025-10-30T00:00:00Z",
  "priority": 50
}
```

**Upload image (requires auth):**
```bash
POST http://localhost:5000/api/news/upload-image
Authorization: Bearer YOUR_TOKEN
Content-Type: multipart/form-data
category=technology
file=@/path/to/image.jpg
```

## Database Management

### Access MongoDB

```bash
# Via Mongo Express (Web UI)
http://localhost:8081

# Via MongoDB shell
docker exec -it newsportal-mongodb mongosh \
  -u admin \
  -p password123 \
  --authenticationDatabase admin
```

### Common MongoDB Operations

```javascript
// Switch to NewsDb
use NewsDb

// Count news articles
db.News.countDocuments()

// Find all news
db.News.find().limit(5)

// Find by category
db.News.find({ Category: "technology" })

// Delete all news (careful!)
db.News.deleteMany({})
```

### Seed Sample Data

```bash
# Via API endpoint (requires auth)
POST http://localhost:5000/api/seed/news
Authorization: Bearer YOUR_TOKEN
```

## Object Storage (MinIO)

### Access MinIO Console

http://localhost:9001 (minioadmin / minioadmin123)

### Using MinIO Client

```bash
# Enter MinIO container
docker exec -it newsportal-minio sh

# List buckets
mc ls myminio/

# List files in news-images bucket
mc ls myminio/news-images/

# Download file
mc cp myminio/news-images/technology/image.jpg ./
```

## Monitoring & Logs

### View Logs

```bash
# All services
docker compose logs

# Specific service
docker compose logs newsportal-backend

# Follow logs (real-time)
docker compose logs -f newsportal-backend

# Last 100 lines
docker compose logs --tail=100 newsportal-backend
```

### Service Status

```bash
# Check container status
docker compose ps

# Check container resource usage
docker stats
```

## Stopping Services

```bash
# Stop all (keeps data)
docker compose down

# Stop all and remove volumes (deletes all data!)
docker compose down -v

# Stop specific service
docker compose stop newsportal-backend

# Restart specific service
docker compose restart newsportal-backend
```

## Helper Scripts (Windows)

The repository includes PowerShell helper scripts for Windows users:

```powershell
# Start services
.\docker-start.ps1

# Stop services  
.\docker-stop.ps1

# View logs
.\docker-logs.ps1

# Check status
.\docker-status.ps1

# Clean rebuild
.\docker-clean.ps1
.\docker-rebuild.ps1
```

## Troubleshooting

### Services Won't Start

```bash
# Check if ports are already in use
netstat -ano | findstr "5000 27017 9000 9001 8081"  # Windows
lsof -i :5000,:27017,:9000,:9001,:8081             # Linux/Mac

# Clean everything and restart
docker compose down -v
docker compose up -d --build
```

### Cannot Connect to API

1. Check if backend is running: `docker compose ps newsportal-backend`
2. Check logs: `docker compose logs newsportal-backend`
3. Verify health: `curl http://localhost:5000/health`
4. Check firewall settings

### Database Connection Issues

1. Ensure MongoDB is running: `docker compose ps newsportal-mongodb`
2. Check connection string in `.env` file
3. Verify credentials match between `.env` and `docker-compose.yml`

### Image Upload Fails

1. Check MinIO is running: `docker compose ps newsportal-minio`
2. Verify MinIO bucket exists: Access http://localhost:9001
3. Check MinIO credentials in `.env`
4. Ensure image file size < 5MB

## Performance Tips

- Use memory caching (enabled by default)
- Enable compression in production
- Use CDN for static assets
- Monitor MongoDB indexes: `db.News.getIndexes()`

## Next Steps

- [Build instructions](./BUILD.md)
- [Deploy to production](./DEPLOY.md)
- [API documentation](../README.md#api-endpoints)
