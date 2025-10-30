# 🚀 Quick Start: Cloudflare R2 Setup

## Option 1: Automated Setup (Recommended)

Run the setup script:

```powershell
cd c:\dev\newsportal
.\scripts\setup-r2.ps1
```

Follow the prompts to configure R2 for Azure and/or local development.

---

## Option 2: Manual Setup

### Step 1: Get R2 Credentials

1. **Go to Cloudflare Dashboard**: 
   - URL: https://dash.cloudflare.com/[YOUR_ACCOUNT_ID]/r2/api-tokens
   - Replace `[YOUR_ACCOUNT_ID]` with your actual account ID

2. **Create API Token**:
   - Click "Create API Token"
   - Token name: `newsportal-backend-token`
   - Permissions: ✅ Object Read & Write
   - Bucket access: Apply to `news-images` only
   - Click "Create API Token"

3. **Copy Credentials** (shown only once):
   ```
   Access Key ID: xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
   Secret Access Key: yyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyy
   ```

### Step 2: Configure Azure Container Apps

```powershell
# Set your Account ID
$accountId = "7ac015923324a4d426c1f7782c3f41e1"

# Set R2 credentials as secrets
az containerapp secret set `
  --name newsportal-backend `
  --resource-group newsportal-rg `
  --secrets `
    minio-access-key="YOUR_R2_ACCESS_KEY_ID" `
    minio-secret-key="YOUR_R2_SECRET_ACCESS_KEY"

# Update environment variables
az containerapp update `
  --name newsportal-backend `
  --resource-group newsportal-rg `
  --set-env-vars `
    MinioSettings__Endpoint="$accountId.r2.cloudflarestorage.com" `
    MinioSettings__BucketName="news-images" `
    MinioSettings__UseSSL="true" `
    MinioSettings__Region="auto" `
  --replace-env-vars `
    MinioSettings__AccessKey=secretref:minio-access-key `
    MinioSettings__SecretKey=secretref:minio-secret-key

# Restart container
az containerapp revision restart `
  --name newsportal-backend `
  --resource-group newsportal-rg
```

### Step 3: Test Deployment

```powershell
# Wait for restart
Start-Sleep -Seconds 30

# Check health
Invoke-WebRequest -Uri "https://newsportal-backend.happyglacier-db0dd319.westeurope.azurecontainerapps.io/health" -UseBasicParsing
```

---

## Quick Reference

### Your R2 Configuration

Replace these values with your actual credentials:

| Setting | Value |
|---------|-------|
| **Account ID** | `7ac015923324a4d426c1f7782c3f41e1` (from URL) |
| **R2 Endpoint** | `7ac015923324a4d426c1f7782c3f41e1.r2.cloudflarestorage.com` |
| **Access Key ID** | Get from Cloudflare Dashboard |
| **Secret Access Key** | Get from Cloudflare Dashboard |
| **Bucket Name** | `news-images` |
| **Use SSL** | `true` |
| **Region** | `auto` |

### API Token Permissions

✅ **Required:**
- Object Read
- Object Write

❌ **Not Required:**
- Bucket Management (unless you need to create buckets)
- Worker Scripts
- Account Settings

### Security Best Practices

1. ✅ Use R2 API Tokens (not Global API Key)
2. ✅ Scope tokens to specific buckets
3. ✅ Store in Azure Container Apps secrets
4. ✅ Never commit to Git
5. ✅ Rotate tokens periodically

---

## Testing

### Test 1: Health Check

```powershell
curl https://newsportal-backend.happyglacier-db0dd319.westeurope.azurecontainerapps.io/health
```

Expected: `200 OK` with status "Healthy"

### Test 2: Image Upload

1. Go to Swagger: `https://newsportal-backend.happyglacier-db0dd319.westeurope.azurecontainerapps.io/swagger`
2. Authenticate: Click "Authorize" → `Bearer YOUR_TOKEN`
3. POST `/api/News` with form-data including image file
4. Check R2 bucket for uploaded image

### Test 3: Verify in R2 Dashboard

1. Go to: `https://dash.cloudflare.com/7ac015923324a4d426c1f7782c3f41e1/r2/default/buckets/news-images`
2. Verify uploaded objects appear in `news-images` bucket

---

## Troubleshooting

| Problem | Solution |
|---------|----------|
| "Access Denied" | Verify API token has correct permissions |
| "NoSuchBucket" | Ensure bucket `news-images` exists in R2 |
| "Invalid credentials" | Check Access Key & Secret Key are correct |
| "SSL error" | Ensure `UseSSL: true` in config |
| Container won't start | Check logs: `az containerapp logs show --name newsportal-backend --resource-group newsportal-rg` |

---

## Cost Estimation

**Cloudflare R2 Pricing:**

- Storage: $0.015 per GB/month
- Class A Operations (PUT): $4.50 per million
- Class B Operations (GET): $0.36 per million
- **Egress: FREE!** 🎉

**Example (1000 news articles):**
- Storage: 500MB = **$0.0075/month**
- Uploads: 1000 = **$0.0045**
- Views: 100k = **$0.036**
- **Total: ~$0.05/month**

---

## Support

📚 **Full Documentation**: `CLOUDFLARE_R2_SETUP.md`

🔗 **Useful Links**:
- [R2 Dashboard](https://dash.cloudflare.com/7ac015923324a4d426c1f7782c3f41e1/r2)
- [R2 Documentation](https://developers.cloudflare.com/r2/)
- [API Tokens](https://dash.cloudflare.com/7ac015923324a4d426c1f7782c3f41e1/r2/api-tokens)

🐛 **Issues?** Check container logs:
```powershell
az containerapp logs show --name newsportal-backend --resource-group newsportal-rg --follow
```
