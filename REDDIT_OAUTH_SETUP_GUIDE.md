# Reddit OAuth Setup Guide

## 🎯 Step 1: Create Reddit Developer App

1. **Login to Reddit**: https://www.reddit.com/login
2. **Go to Apps Page**: https://www.reddit.com/prefs/apps
3. **Click "create another app..." button** (at the bottom)
4. **Fill in the form**:

   ```
   Name: News Portal Aggregator
   App type: ☑️ script (for personal use)
   Description: Automated news aggregation from technology subreddits
   About URL: https://clever-speculoos-aacb3a.netlify.app
   Redirect URI: https://newsportal-backend.happyglacier-db0dd319.westeurope.azurecontainerapps.io/auth/reddit/callback
   ```

   **Note**: For "script" type apps, redirect URI is not actually used (we use password grant flow), but it's a required field. You can add both production and local URLs if needed:
   - Production: `https://newsportal-backend.happyglacier-db0dd319.westeurope.azurecontainerapps.io/auth/reddit/callback`
   - Local dev: `http://localhost:5000/auth/reddit/callback`

5. **Click "create app"**

## 🔑 Step 2: Get Your Credentials

After creating the app, you'll see:

```
personal use script
[YOUR_CLIENT_ID] ← 14-character string under "personal use script"
secret: [YOUR_CLIENT_SECRET] ← longer string
```

**Example**:
```
personal use script
abc123XYZ456abc  ← This is your CLIENT_ID
secret: longSecretString123456789abcdefghijklmnop  ← This is your CLIENT_SECRET
```

## 🔐 Step 3: Add Credentials to Azure

### Option A: Azure Portal (Recommended for Production)

1. Go to Azure Portal: https://portal.azure.com
2. Navigate to: `newsportal-rg` → `newsportal-backend`
3. Click **"Secrets"** in left sidebar
4. Add these secrets:

   ```
   RedditSettings__ClientId = [YOUR_CLIENT_ID]
   RedditSettings__ClientSecret = [YOUR_CLIENT_SECRET]
   RedditSettings__Username = [YOUR_REDDIT_USERNAME]
   RedditSettings__Password = [YOUR_REDDIT_PASSWORD]
   ```

5. Click **"Save"**
6. Click **"Restart"** to apply changes

### Option B: Local Development (.NET User Secrets)

```powershell
cd backend

# Set Reddit credentials
dotnet user-secrets set "RedditSettings:ClientId" "YOUR_CLIENT_ID"
dotnet user-secrets set "RedditSettings:ClientSecret" "YOUR_CLIENT_SECRET"
dotnet user-secrets set "RedditSettings:Username" "YOUR_REDDIT_USERNAME"
dotnet user-secrets set "RedditSettings:Password" "YOUR_REDDIT_PASSWORD"
```

### Option C: Docker Environment Variables

Edit `.env` file in project root:

```env
# Reddit OAuth
REDDIT_CLIENT_ID=YOUR_CLIENT_ID
REDDIT_CLIENT_SECRET=YOUR_CLIENT_SECRET
REDDIT_USERNAME=YOUR_REDDIT_USERNAME
REDDIT_PASSWORD=YOUR_REDDIT_PASSWORD
```

Then update `docker-compose.yml`:

```yaml
environment:
  RedditSettings__ClientId: ${REDDIT_CLIENT_ID}
  RedditSettings__ClientSecret: ${REDDIT_CLIENT_SECRET}
  RedditSettings__Username: ${REDDIT_USERNAME}
  RedditSettings__Password: ${REDDIT_PASSWORD}
```

## 🧪 Step 4: Test the Integration

### Test Locally (Docker)

```powershell
# Rebuild and start
docker compose up -d --build newsportal-backend

# Test Reddit fetch
$token = (Invoke-RestMethod -Uri "http://localhost:5000/api/Auth/login" -Method Post -Body (@{username="admin";password="admin123"}|ConvertTo-Json) -ContentType "application/json").token

Invoke-RestMethod -Uri "http://localhost:5000/api/Seed/fetch-reddit-news" -Method Post -Headers @{Authorization="Bearer $token"}
```

### Test on Azure (Production)

```powershell
# Login and get token
$loginUrl = "https://newsportal-backend.happyglacier-db0dd319.westeurope.azurecontainerapps.io/api/Auth/login"
$token = (Invoke-RestMethod -Uri $loginUrl -Method Post -Body (@{username="admin";password="admin123"}|ConvertTo-Json) -ContentType "application/json").token

# Trigger Reddit fetch
$fetchUrl = "https://newsportal-backend.happyglacier-db0dd319.westeurope.azurecontainerapps.io/api/Seed/fetch-reddit-news"
Invoke-RestMethod -Uri $fetchUrl -Method Post -Headers @{Authorization="Bearer $token"}
```

## 📋 Expected Results

Successful response:
```json
{
  "message": "Reddit news aggregation completed successfully!",
  "fetched": 120,
  "saved": 95,
  "subreddits": [
    "popular",
    "ArtificialIntelligence",
    "GithubCopilot",
    "mcp",
    "OpenAI",
    "robotics",
    "DeepSeek",
    "dotnet",
    "ClaudeAI"
  ]
}
```

## 🔍 Troubleshooting

### Error: 401 Unauthorized
**Cause**: Invalid credentials or expired token  
**Fix**: Double-check CLIENT_ID, CLIENT_SECRET, USERNAME, PASSWORD

### Error: 403 Forbidden
**Cause**: Rate limiting or incorrect app type  
**Fix**: Ensure app type is "script", wait 1 minute and retry

### Error: 429 Too Many Requests
**Cause**: Reddit API rate limit (60 requests per minute)  
**Fix**: Background job automatically handles rate limiting

### No Posts Fetched
**Cause**: Subreddit doesn't exist or has no recent posts  
**Fix**: Check subreddit names are correct (case-sensitive)

## 📊 Rate Limits

**With OAuth Authentication**:
- 60 requests per minute
- 600 requests per 10 minutes
- Sufficient for our use case (9 subreddits × 1 request = 9 requests per job run)

**Background Job Schedule**:
- Runs daily at 06:00 UTC
- Fetches ~100-150 posts total
- Well within rate limits

## 🔒 Security Best Practices

1. **Never commit credentials to Git**
   - Use Azure Key Vault or User Secrets
   - Add `.env` to `.gitignore`

2. **Rotate credentials regularly**
   - Change Reddit password every 90 days
   - Regenerate app secret if compromised

3. **Use separate accounts for dev/prod**
   - Development: Your personal Reddit account
   - Production: Create dedicated service account

4. **Monitor API usage**
   - Check Azure logs for errors
   - Set up alerts for 401/403 errors

## 📚 Resources

- **Reddit API Documentation**: https://www.reddit.com/dev/api/
- **OAuth2 Flow**: https://github.com/reddit-archive/reddit/wiki/OAuth2
- **Rate Limiting**: https://www.reddit.com/r/redditdev/wiki/api

---

**Status**: ⏳ Waiting for credentials  
**Next Step**: Add credentials to Azure secrets and redeploy
