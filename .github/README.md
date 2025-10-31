# GitHub Actions CI/CD Setup Guide

> Complete guide for setting up automated deployments and news aggregation

## 📋 Table of Contents

- [Overview](#overview)
- [GitHub Secrets Configuration](#github-secrets-configuration)
- [Available Workflows](#available-workflows)
- [Manual Deployment](#manual-deployment)
- [Troubleshooting](#troubleshooting)

## 🎯 Overview

This repository includes automated workflows for:

1. **Daily News Aggregation** - Automatically fetches tech news from Reddit daily at 05:00 AM
2. **Backend Deployment** - Deploys .NET backend to Azure Container Apps on push
3. **Frontend Deployment** - Vercel auto-deploys frontend from master branch
4. **Code Quality Checks** - Runs on pull requests

## 🔐 GitHub Secrets Configuration

### Required Secrets

Go to **Settings → Secrets and variables → Actions → New repository secret**

#### For Backend Deployment (`azure-backend-deploy.yml`)

| Secret Name | Description | Example |
|-------------|-------------|---------|
| `AZURE_CREDENTIALS` | Azure service principal JSON | See [Azure Setup](#azure-credentials) below |

#### For Daily News Aggregation (`daily-news-aggregation.yml`)

| Secret Name | Description | Example |
|-------------|-------------|---------|
| `API_BASE_URL` | Backend API base URL | `https://newsportal-backend.azurecontainerapps.io/api` |
| `ADMIN_USERNAME` | Admin username for API | `admin` |
| `ADMIN_PASSWORD` | Admin password for API | `your_secure_password` |

### Azure Credentials

To create the `AZURE_CREDENTIALS` secret:

```bash
# Login to Azure
az login

# Create service principal (replace with your subscription ID and resource group)
az ad sp create-for-rbac \
  --name "newsportal-github-actions" \
  --role contributor \
  --scopes /subscriptions/{SUBSCRIPTION_ID}/resourceGroups/newsportal-rg \
  --sdk-auth

# Copy the entire JSON output and add as AZURE_CREDENTIALS secret
```

The JSON should look like:
```json
{
  "clientId": "...",
  "clientSecret": "...",
  "subscriptionId": "...",
  "tenantId": "...",
  "activeDirectoryEndpointUrl": "https://login.microsoftonline.com",
  "resourceManagerEndpointUrl": "https://management.azure.com/",
  "activeDirectoryGraphResourceId": "https://graph.windows.net/",
  "sqlManagementEndpointUrl": "https://management.core.windows.net:8443/",
  "galleryEndpointUrl": "https://gallery.azure.com/",
  "managementEndpointUrl": "https://management.core.windows.net/"
}
```

## 🤖 Available Workflows

### 1. Daily News Aggregation

**File:** `.github/workflows/daily-news-aggregation.yml`

**Schedule:** Every day at 05:00 AM UTC (08:00 AM Turkey time)

**What it does:**
- Fetches latest posts from technology subreddits
- Imports GitHub Copilot and AI-related discussions
- Aggregates developer tools, web dev, and security news
- Clears cache to refresh frontend

**Manual trigger:**
```bash
# Go to: Actions → Daily News Aggregation → Run workflow
# Or use GitHub CLI:
gh workflow run daily-news-aggregation.yml
```

**Subreddits monitored:**
- Technology: `r/technology`, `r/programming`, `r/coding`, `r/webdev`
- AI & ML: `r/MachineLearning`, `r/artificial`, `r/ArtificialInteligence`
- GitHub: `r/github` (filtered for "copilot"), `r/opensource`
- DevOps: `r/devops`, `r/docker`, `r/kubernetes`
- Web Dev: `r/reactjs`, `r/nextjs`, `r/node`, `r/typescript`
- Security: `r/cybersecurity`, `r/netsec`
- Gaming: `r/gamedev`, `r/Unity3D`

### 2. Backend Deployment

**File:** `.github/workflows/azure-backend-deploy.yml`

**Trigger:** Push to `master` branch (changes in `backend/` folder)

**What it does:**
- Builds Docker image from `Dockerfile`
- Pushes to Azure Container Registry
- Updates Azure Container App
- Runs health check verification

**Manual trigger:**
```bash
gh workflow run azure-backend-deploy.yml
```

### 3. Frontend Code Quality

**File:** `.github/workflows/frontend-code-quality.yml`

**Trigger:** Pull requests affecting `frontend/` folder

**What it does:**
- Runs ESLint checks
- Runs TypeScript type checking
- Runs Prettier format checks

## 🚀 Manual Deployment

### Frontend Deployment (Vercel)

**Automated:**
- Vercel auto-deploys on push to `master`
- No manual action needed

**Manual (from local machine):**
```powershell
# Preview deployment
.\deploy-frontend.ps1

# Production deployment
.\deploy-frontend.ps1 -Production
```

### Backend Deployment (Azure)

**Automated:**
- GitHub Actions deploys on push to `master`

**Manual (from local machine):**
```powershell
# Deploy with defaults
.\deploy-backend.ps1

# Deploy with custom settings
.\deploy-backend.ps1 -ResourceGroup "my-rg" -ContainerApp "my-app"
```

## 🔄 Manual News Refresh

### Option 1: Trigger GitHub Action

Go to **Actions → Daily News Aggregation → Run workflow**

### Option 2: API Call

```bash
# Get JWT token
curl -X POST "https://your-api-url/api/Auth/login" \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"your_password"}'

# Import from specific subreddit
curl -X POST "https://your-api-url/api/SocialMedia/import/reddit?subreddit=technology&limit=25" \
  -H "Authorization: Bearer YOUR_TOKEN"

# Clear cache
curl -X POST "https://your-api-url/api/NewsArticle/refresh" \
  -H "Authorization: Bearer YOUR_TOKEN"
```

### Option 3: Swagger UI

1. Go to `https://your-api-url/swagger`
2. Click **Authorize** and login
3. Use **POST /api/SocialMedia/import/reddit** endpoint
4. Use **POST /api/NewsArticle/refresh** to clear cache

## 🐛 Troubleshooting

### News Aggregation Fails

**Check:**
1. API secrets are correct (`API_BASE_URL`, `ADMIN_USERNAME`, `ADMIN_PASSWORD`)
2. Backend API is running and accessible
3. Admin credentials are valid

**View logs:**
- Go to **Actions → Daily News Aggregation → Select run → View logs**

### Backend Deployment Fails

**Check:**
1. `AZURE_CREDENTIALS` secret is correctly formatted JSON
2. Service principal has correct permissions
3. Azure Container Registry and Container App exist

**View logs:**
- Go to **Actions → Azure Deployment - Backend → Select run**

**Debug locally:**
```powershell
# Test Docker build
docker build -t test-image -f Dockerfile .

# Test Azure login
az login
az account show
```

### Cache Not Clearing

**Solution:**
```bash
# Manual cache clear via API
curl -X POST "https://your-api-url/api/NewsArticle/refresh" \
  -H "Authorization: Bearer YOUR_TOKEN"
```

### Reddit API Rate Limiting

**Symptoms:**
- Fewer posts imported than expected
- Import errors in logs

**Solution:**
- Reddit allows ~60 requests per minute
- Workflow is configured with delays
- If rate limited, wait 1 hour and retry

## 📊 Monitoring Workflow Runs

### View All Runs
```bash
# Using GitHub CLI
gh run list --workflow=daily-news-aggregation.yml --limit=10

# View specific run
gh run view <RUN_ID>

# View logs
gh run view <RUN_ID> --log
```

### Check Workflow Status Badge

Add to README.md:
```markdown
![Daily News](https://github.com/bkalafat/newsportal/actions/workflows/daily-news-aggregation.yml/badge.svg)
![Backend Deploy](https://github.com/bkalafat/newsportal/actions/workflows/azure-backend-deploy.yml/badge.svg)
```

## 💡 Tips

1. **Test workflows locally before committing:**
   ```bash
   # Install act (GitHub Actions local runner)
   # Then test workflow
   act -j fetch-news
   ```

2. **Enable notifications:**
   - Settings → Notifications → Actions
   - Get email alerts on workflow failures

3. **Schedule adjustments:**
   - Edit cron schedule in workflow file
   - Use https://crontab.guru/ to test cron expressions

4. **Cost optimization:**
   - Workflows run on GitHub's free tier (2000 minutes/month)
   - Each run takes ~5 minutes
   - Daily runs = ~150 minutes/month (well within free tier)

## 📝 Workflow Syntax Reference

```yaml
on:
  schedule:
    # ┌───────────── minute (0 - 59)
    # │ ┌───────────── hour (0 - 23)
    # │ │ ┌───────────── day of month (1 - 31)
    # │ │ │ ┌───────────── month (1 - 12)
    # │ │ │ │ ┌───────────── day of week (0 - 6, Sunday = 0)
    # │ │ │ │ │
    - cron: '0 5 * * *'  # 05:00 AM UTC daily
```

**Common schedules:**
- `0 5 * * *` - Daily at 05:00 AM
- `0 */6 * * *` - Every 6 hours
- `0 9 * * 1` - Every Monday at 09:00 AM
- `0 0 1 * *` - First day of every month

## 🔗 Useful Links

- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [Azure Container Apps Documentation](https://learn.microsoft.com/en-us/azure/container-apps/)
- [Vercel Deployment Documentation](https://vercel.com/docs)
- [Reddit API Documentation](https://www.reddit.com/dev/api/)

---

**Need Help?**
- Check existing workflow runs for examples
- Review logs in Actions tab
- Open an issue if you encounter problems
