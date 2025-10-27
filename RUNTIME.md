# Runtime Details

## Platform Architecture

### Backend (.NET 10)
- **Runtime**: Docker container (`newsportal-backend`)
- **Port**: `5000:8080`
- **Role**: REST API, business logic, data access
- **Stack**: ASP.NET Core 10, MongoDB Driver, JWT Auth
- **Environment**: Production mode in Docker

### Frontend (Next.js 16)
- **Runtime**: Node.js (local development)
- **Port**: `3000` (default)
- **Role**: User interface, SSR/SSG rendering
- **Stack**: React 19, Next.js 16, TailwindCSS, TypeScript
- **Environment**: Development mode locally

### Database (MongoDB 7.0)
- **Runtime**: Docker container (`newsportal-mongodb`)
- **Port**: `27017`
- **Role**: Primary data storage (NoSQL)
- **Access**: Admin UI via Mongo Express on `8081`
- **Credentials**: admin/password123

### Storage (MinIO)
- **Runtime**: Docker container (`newsportal-minio`)
- **Ports**: API `9000`, Console `9001`
- **Role**: S3-compatible object storage for images
- **Bucket**: `news-images`
- **Credentials**: minioadmin/minioadmin123

## Deployment Strategy

| Service | Where | Why |
|---------|-------|-----|
| Backend | Docker | Consistent environment, easy deployment, isolated dependencies |
| Frontend | Local/Node | Hot reload, fast development, framework tooling |
| MongoDB | Docker | Data persistence, no local installation needed |
| MinIO | Docker | S3-compatible storage, no AWS dependency |

## Quick Start

```bash
# Start backend services (MongoDB, MinIO, API)
docker-compose up -d

# Start frontend
cd frontend
npm run dev
```

## Service URLs

- Backend API: http://localhost:5000
- Swagger Docs: http://localhost:5000/swagger
- Frontend: http://localhost:3000
- MongoDB Admin: http://localhost:8081
- MinIO Console: http://localhost:9001
