# Deployment Guide

## Production Architecture

### Current Production Setup

```
┌─────────────────────────────────────────────────┐
│              Azure Container Apps                │
│  ┌───────────────────────────────────────────┐  │
│  │     Backend API (.NET 9)                  │  │
│  │     - Auto-scaling: 0-10 replicas         │  │
│  │     - Region: East US                     │  │
│  │     - URL: newsportal-backend.*.azurecontainerapps.io  │
│  └───────────────────────────────────────────┘  │
└─────────────────────────────────────────────────┘
                    ↓              ↓
        ┌───────────────┐  ┌──────────────┐
        │  MongoDB      │  │   MinIO      │
        │  (External)   │  │  (External)  │
        └───────────────┘  └──────────────┘

┌─────────────────────────────────────────────────┐
│           Vercel / Azure Static Web Apps         │
│  ┌───────────────────────────────────────────┐  │
│  │     Frontend (Next.js 16)                 │  │
│  │     - Auto-deploy from master branch      │  │
│  │     - CDN-enabled globally                │  │
│  └───────────────────────────────────────────┘  │
└─────────────────────────────────────────────────┘
```

### Components

| Component | Platform | Purpose | Auto-Deploy |
|-----------|----------|---------|-------------|
| **Backend API** | Azure Container Apps | REST API | ✅ GitHub Actions |
| **Frontend** | Vercel | Web UI | ✅ Git push |
| **Database** | MongoDB Atlas / Self-hosted | Data storage | Manual |
| **Object Storage** | MinIO / Cloudflare R2 | Image storage | Manual |

## Backend Deployment (Azure Container Apps)

### Prerequisites

- Azure subscription ([Free account](https://azure.microsoft.com/free/))
- Azure CLI installed
- Docker installed
- GitHub repository access

### Option 1: Automatic Deployment (GitHub Actions)

**Already configured!** Just push to trigger deployment:

```bash
# Deployment workflow: .github/workflows/azure-backend-deploy.yml

# Deploy to production
git push origin master

# Monitor deployment
# Go to: GitHub → Actions → azure-backend-deploy
```

#### GitHub Secrets Required

Ensure these secrets are configured in GitHub → Settings → Secrets:

- `AZURE_CREDENTIALS` - Service principal JSON
- `ACR_NAME` - Azure Container Registry name
- `JWT_SECRET_KEY` - JWT secret (min 32 chars)
- `MONGO_CONNECTION_STRING` - MongoDB connection URL
- `MINIO_ENDPOINT` - MinIO endpoint
- `MINIO_ACCESS_KEY` - MinIO access key
- `MINIO_SECRET_KEY` - MinIO secret key

### Option 2: Manual Deployment

#### 1. Build and Push Docker Image

```bash
# Login to Azure
az login

# Login to ACR
az acr login --name YOUR_ACR_NAME

# Build image
docker build -t YOUR_ACR_NAME.azurecr.io/newsportal-backend:latest \
  -f Dockerfile .

# Push image
docker push YOUR_ACR_NAME.azurecr.io/newsportal-backend:latest
```

#### 2. Deploy to Container Apps

```bash
# Update container app
az containerapp update \
  --name newsportal-backend \
  --resource-group newsportal-rg \
  --image YOUR_ACR_NAME.azurecr.io/newsportal-backend:latest
```

#### 3. Verify Deployment

```bash
# Get app URL
az containerapp show \
  --name newsportal-backend \
  --resource-group newsportal-rg \
  --query properties.configuration.ingress.fqdn \
  --output tsv

# Test health endpoint
curl https://YOUR_APP_URL/health
```

### Environment Variables (Production)

Set these in Azure Container Apps:

```bash
# Required
ASPNETCORE_ENVIRONMENT=Production
MongoDbSettings__ConnectionString=mongodb+srv://user:pass@cluster.mongodb.net/NewsDb
MinioSettings__Endpoint=your-minio-server.com:9000
MinioSettings__AccessKey=YOUR_ACCESS_KEY
MinioSettings__SecretKey=YOUR_SECRET_KEY
JwtSettings__SecretKey=YOUR_SUPER_SECRET_JWT_KEY_MIN_32_CHARS

# Optional
MinioSettings__UseSSL=true
JwtSettings__ExpirationMinutes=60
```

## Frontend Deployment

### Vercel (Recommended)

**Already configured!** Frontend auto-deploys on push to master.

#### Manual Setup

1. Go to [vercel.com](https://vercel.com)
2. Import GitHub repository
3. Configure:
   - **Framework**: Next.js
   - **Root Directory**: `frontend`
   - **Build Command**: `npm run build`
   - **Output Directory**: `.next`

4. Set environment variables:
   ```env
   NEXT_PUBLIC_API_URL=https://newsportal-backend.*.azurecontainerapps.io
   NEXT_PUBLIC_SITE_URL=https://your-domain.com
   ```

5. Deploy!

### Azure Static Web Apps

#### Create Resource

```bash
az staticwebapp create \
  --name newsportal-frontend \
  --resource-group newsportal-rg \
  --source https://github.com/bkalafat/newsportal \
  --location "East US 2" \
  --branch master \
  --app-location "frontend" \
  --output-location ".next" \
  --login-with-github
```

#### Configure

1. Add environment variables in Azure Portal
2. Trigger deployment from GitHub Actions

## Database Setup (MongoDB)

### Option 1: MongoDB Atlas (Recommended)

1. Create free cluster at [mongodb.com/cloud/atlas](https://www.mongodb.com/cloud/atlas)
2. Create database: `NewsDb`
3. Create user with read/write permissions
4. Whitelist Azure Container Apps IP addresses
5. Get connection string
6. Update backend environment variables

### Option 2: Self-Hosted MongoDB

```bash
# Docker deployment
docker run -d \
  --name mongodb-prod \
  -p 27017:27017 \
  -e MONGO_INITDB_ROOT_USERNAME=admin \
  -e MONGO_INITDB_ROOT_PASSWORD=YOUR_SECURE_PASSWORD \
  -v mongodb_data:/data/db \
  mongo:7.0

# Connection string
mongodb://admin:YOUR_SECURE_PASSWORD@your-server.com:27017/NewsDb?authSource=admin
```

## Object Storage Setup

### Option 1: MinIO

```bash
# Docker deployment
docker run -d \
  --name minio-prod \
  -p 9000:9000 \
  -p 9001:9001 \
  -e MINIO_ROOT_USER=YOUR_ACCESS_KEY \
  -e MINIO_ROOT_PASSWORD=YOUR_SECRET_KEY \
  -v minio_data:/data \
  quay.io/minio/minio:latest \
  server /data --console-address ":9001"

# Create bucket
mc alias set myminio http://your-server:9000 YOUR_ACCESS_KEY YOUR_SECRET_KEY
mc mb myminio/news-images
mc anonymous set download myminio/news-images
```

### Option 2: Cloudflare R2

1. Create R2 bucket in Cloudflare dashboard
2. Generate API credentials
3. Configure backend with R2-compatible endpoints

### Option 3: Azure Blob Storage

Not currently implemented, but backend can be extended to support it.

## Monitoring & Logging

### Application Insights (Azure)

Already configured! View:
- Application map
- Performance metrics
- Error tracking
- Request analytics

```bash
# View in portal
az portal open --resource-group newsportal-rg --resource newsportal-backend
```

### Container Logs

```bash
# Stream live logs
az containerapp logs show \
  --name newsportal-backend \
  --resource-group newsportal-rg \
  --follow

# Last 100 lines
az containerapp logs show \
  --name newsportal-backend \
  --resource-group newsportal-rg \
  --tail 100
```

### Metrics

```bash
# CPU and memory usage
az monitor metrics list \
  --resource newsportal-backend \
  --resource-group newsportal-rg \
  --metric-names "CpuPercentage,MemoryWorkingSetPercentage"
```

## SSL/TLS Certificates

- **Azure Container Apps**: Automatic HTTPS with managed certificates
- **Vercel**: Automatic HTTPS with Let's Encrypt
- **Custom domains**: Configure in respective platforms

## Scaling Configuration

### Backend Auto-Scaling

```bash
# Configure scaling rules
az containerapp update \
  --name newsportal-backend \
  --resource-group newsportal-rg \
  --min-replicas 1 \
  --max-replicas 10 \
  --scale-rule-name http-rule \
  --scale-rule-type http \
  --scale-rule-http-concurrency 50
```

Current configuration:
- **Min replicas**: 0 (scales to zero when idle)
- **Max replicas**: 10
- **Scale trigger**: HTTP concurrency > 50 requests

## Cost Optimization

### Current Monthly Costs

| Service | Cost/Month |
|---------|------------|
| Azure Container Apps | $5-50 (scales to zero) |
| Azure Container Registry | $5 |
| MongoDB Atlas (M0) | Free |
| MinIO (self-hosted) | $0 (if you host) |
| Vercel (Hobby) | Free |
| **Total** | **$10-60** |

### Tips to Reduce Costs

1. Enable scale-to-zero for dev environments
2. Use free tiers where possible (MongoDB Atlas M0, Vercel Hobby)
3. Delete unused resources
4. Use shared Container Apps environment

## Rollback Procedure

### Rollback Backend

```bash
# List revisions
az containerapp revision list \
  --name newsportal-backend \
  --resource-group newsportal-rg

# Activate previous revision
az containerapp revision activate \
  --name newsportal-backend \
  --resource-group newsportal-rg \
  --revision REVISION_NAME
```

### Rollback Frontend (Vercel)

1. Go to Vercel dashboard
2. Select deployment
3. Click "Promote to Production"

## Disaster Recovery

### Backup MongoDB

```bash
# Backup
mongodump --uri="mongodb+srv://user:pass@cluster.mongodb.net/NewsDb" \
  --out=/backup/$(date +%Y%m%d)

# Restore
mongorestore --uri="mongodb+srv://user:pass@cluster.mongodb.net/NewsDb" \
  /backup/20251030
```

### Backup MinIO

```bash
# Sync to backup location
mc mirror myminio/news-images /backup/minio/news-images
```

## Security Checklist

- [ ] Use Azure Key Vault for secrets (not environment variables)
- [ ] Enable Azure Defender for Container Apps
- [ ] Implement rate limiting
- [ ] Configure CORS properly (only allow your frontend domain)
- [ ] Enable HTTPS only
- [ ] Rotate JWT secret regularly
- [ ] Use strong MongoDB passwords
- [ ] Restrict network access (firewall rules)
- [ ] Enable audit logging

## Troubleshooting

### Backend Not Starting

1. Check container logs: `az containerapp logs show`
2. Verify environment variables are set
3. Test MongoDB connection from local machine
4. Verify image was pushed successfully

### 502 Bad Gateway

- Container is starting (wait 30-60 seconds)
- Health check is failing (check `/health` endpoint)
- Resource limits exceeded (increase CPU/memory)

### Database Connection Timeout

- Check MongoDB firewall rules
- Whitelist Azure Container Apps outbound IPs
- Verify connection string format

### Images Not Loading

- Check MinIO is accessible from backend
- Verify MinIO credentials
- Check bucket permissions (should allow public read)

## Support

For deployment issues:
- Check [Azure documentation](https://learn.microsoft.com/azure/container-apps/)
- Open GitHub issue
- Contact Azure support for critical production issues

---

**Last Updated**: October 2025  
**Maintained By**: @bkalafat
