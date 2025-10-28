# Koyeb Deployment Guide for NewsPortal API
# Free tier: 1 web service, 512MB RAM, auto-sleep after 5 min inactivity
# Docs: https://www.koyeb.com/docs

## Deployment Steps:

### 1. Sign Up
- Go to: https://www.koyeb.com/
- Sign up with GitHub (no credit card required)
- Free tier: 1 web service, 512MB RAM

### 2. Create New App
- Click "Create App"
- Select "GitHub" as source
- Connect repository: bkalafat/newsportal
- Branch: upgrade-to-NET10

### 3. Configure Build
- **Builder**: Docker
- **Dockerfile path**: ./Dockerfile
- **Docker build context**: .
- **Port**: 8080
- **Health check path**: /health

### 4. Environment Variables

Add these in the "Environment Variables" section:

```bash
# MongoDB Atlas
MongoDbSettings__ConnectionString=mongodb+srv://bkalafat:dbuserpassword123@cluster0.xwbfl1o.mongodb.net/NewsDb?retryWrites=true&w=majority
MongoDbSettings__DatabaseName=NewsDb
MongoDbSettings__NewsCollectionName=News

# ASP.NET Core
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:8080

# JWT Settings
JwtSettings__SecretKey=your-super-secret-jwt-key-minimum-32-characters-long
JwtSettings__Issuer=NewsApi
JwtSettings__Audience=NewsApiClients
JwtSettings__ExpirationMinutes=1440

# Cloudflare R2
MinioSettings__Endpoint=7ac015923324a4d426c1f7782c3f41e1.r2.cloudflarestorage.com
MinioSettings__AccessKey=cc61a30e775100a198836d97dbce0d79
MinioSettings__SecretKey=YOUR_R2_SECRET_KEY
MinioSettings__BucketName=news-images
MinioSettings__UseSSL=true

# CORS
AllowedOrigins=http://localhost:3000,https://newsportal.vercel.app
```

### 5. Deploy
- Click "Deploy"
- Wait ~5-10 minutes for build
- Get your public URL: https://newsportal-api-YOUR-ID.koyeb.app

### 6. Test
```bash
curl https://newsportal-api-YOUR-ID.koyeb.app/health
curl https://newsportal-api-YOUR-ID.koyeb.app/api/NewsArticle
```

## Free Tier Limits:
- ✅ 1 web service
- ✅ 512MB RAM
- ✅ 2GB storage
- ✅ Unlimited bandwidth
- ⚠️ Auto-sleep after 5 minutes (wakes on request)
- ⚠️ Slower cold starts (~10-30 seconds)

## Advantages:
- No credit card required
- Docker support
- Free SSL
- Custom domains (free)
- Health checks
- Auto-deploy on git push

## Keep-Alive Solution:
Use GitHub Actions to ping every 4 minutes to prevent sleep (already configured in .github/workflows/keep-alive.yml)
