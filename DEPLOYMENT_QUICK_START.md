# 🚀 Quick Deployment Guide

## Frontend Deployment (Vercel)

### Option 1: Automatic (Recommended)
Vercel automatically deploys when you push to master branch.

### Option 2: Manual Deployment
```powershell
# Preview deployment
.\deploy-frontend.ps1

# Production deployment
.\deploy-frontend.ps1 -Production
```

**First-time Setup:**
1. Install Vercel CLI: `npm install -g vercel`
2. Run `vercel` in the frontend directory
3. Link to your Vercel project
4. Deploy!

---

## Backend Deployment (Azure)

### Option 1: Automatic (GitHub Actions)
Push changes to `master` branch:
```bash
git add .
git commit -m "Update backend"
git push origin master
```

GitHub Actions will automatically deploy to Azure.

### Option 2: Manual Deployment
```powershell
# Deploy with default settings
.\deploy-backend.ps1

# Deploy with custom resource group
.\deploy-backend.ps1 -ResourceGroup "my-rg" -ContainerApp "my-app"
```

---

## 🔄 Manual News Refresh

### Option 1: API Call
```bash
# Login
curl -X POST "https://your-api-url/api/Auth/login" \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"your_password"}'

# Refresh cache
curl -X POST "https://your-api-url/api/NewsArticle/refresh" \
  -H "Authorization: Bearer YOUR_TOKEN"
```

### Option 2: Swagger UI
1. Go to `https://your-api-url/swagger`
2. Click **Authorize** button
3. Login with admin credentials
4. Find **POST /api/NewsArticle/refresh**
5. Click "Try it out" → "Execute"

### Option 3: Trigger GitHub Action
1. Go to GitHub: **Actions** → **Daily News Aggregation**
2. Click **Run workflow** button
3. Select branch and click **Run workflow**

---

## 🤖 Daily News Aggregation (Automated)

**Schedule:** Every day at 05:00 AM UTC (08:00 AM Turkey time)

**What it does:**
- Fetches latest posts from 20+ technology subreddits
- Imports AI, GitHub Copilot, and programming news
- Gets trending topics from web development communities
- Clears cache automatically

**Monitored Subreddits:**
- Technology: r/technology, r/programming, r/coding, r/webdev
- AI: r/MachineLearning, r/artificial, r/ArtificialInteligence
- GitHub: r/github, r/opensource
- DevOps: r/devops, r/docker, r/kubernetes
- Web: r/reactjs, r/nextjs, r/node, r/typescript
- Security: r/cybersecurity, r/netsec
- Gaming: r/gamedev, r/Unity3D

---

## 📋 Required GitHub Secrets

Configure these in **Settings → Secrets → Actions**:

### Backend Deployment
- `AZURE_CREDENTIALS` - Azure service principal JSON

### News Aggregation
- `API_BASE_URL` - Backend API URL (e.g., `https://newsportal-backend.azurecontainerapps.io/api`)
- `ADMIN_USERNAME` - Admin username (default: `admin`)
- `ADMIN_PASSWORD` - Admin password (secure!)

---

## 🔧 Local Development

### Start All Services (Docker)
```powershell
# Start MongoDB, MinIO, and Backend API
.\deploy.ps1

# Stop all services
.\deploy.ps1 -Stop

# View logs
.\deploy.ps1 -Logs

# Check status
.\deploy.ps1 -Status
```

### Frontend Development
```bash
cd frontend
npm install
npm run dev
# Opens at http://localhost:3000
```

---

## 📚 Documentation

- **Detailed CI/CD Guide:** [.github/README.md](.github/README.md)
- **Build Instructions:** [docs/BUILD.md](docs/BUILD.md)
- **Running Guide:** [docs/RUN.md](docs/RUN.md)
- **Deployment Guide:** [docs/DEPLOY.md](docs/DEPLOY.md)
- **Architecture:** [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md)

---

## ⚡ Quick Commands

```powershell
# Frontend
.\deploy-frontend.ps1              # Deploy frontend to Vercel
npm run build                      # Build Next.js app

# Backend
.\deploy-backend.ps1               # Deploy backend to Azure
.\deploy.ps1                       # Start local Docker environment

# Testing
dotnet test                        # Run backend tests
cd frontend; npm test              # Run frontend tests

# Code Quality
dotnet build                       # Build backend
cd frontend; npm run lint          # Lint frontend
```

---

## 🎯 Deployment Checklist

### First-Time Setup
- [ ] Configure GitHub secrets
- [ ] Link Vercel project
- [ ] Setup Azure Container Registry
- [ ] Create Azure Container App
- [ ] Test manual deployment locally

### Before Each Deployment
- [ ] Run tests: `dotnet test`
- [ ] Build frontend: `npm run build`
- [ ] Check for errors
- [ ] Commit and push changes

### After Deployment
- [ ] Check GitHub Actions status
- [ ] Verify health endpoint: `/health`
- [ ] Test API: `/swagger`
- [ ] Check frontend loading
- [ ] Monitor logs for errors

---

## 🆘 Troubleshooting

### Frontend build fails
- Check Node.js version (18+)
- Clear cache: `rm -rf .next node_modules; npm install`
- Check environment variables

### Backend deployment fails
- Verify Azure credentials
- Check Docker is running
- Ensure ACR access
- Review GitHub Actions logs

### News aggregation not working
- Verify API secrets are correct
- Check backend is accessible
- Review workflow logs
- Test Reddit API manually

### Cache not clearing
- Call `/api/NewsArticle/refresh` endpoint
- Restart backend container
- Check admin credentials

---

**Need Help?** Check the detailed guides in `.github/README.md` or open an issue.
