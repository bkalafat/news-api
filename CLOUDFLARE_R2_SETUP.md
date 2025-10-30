# Cloudflare R2 Storage Setup Guide

## Overview

Cloudflare R2 is an S3-compatible object storage service that's fully compatible with your existing MinIO implementation. This guide will help you migrate from MinIO to R2 for production deployments.

## Benefits of R2 over MinIO

- **Zero egress fees** - No charges for data transfer out
- **Global edge network** - Low latency worldwide via Cloudflare CDN
- **S3 compatibility** - Works with existing MinIO SDK
- **No server management** - Fully managed service
- **Cost-effective** - Pay only for storage and operations

## Prerequisites

1. Cloudflare account (free or paid)
2. Access to Cloudflare Dashboard: https://dash.cloudflare.com/

## Step 1: Create R2 Bucket

1. Navigate to **R2 Object Storage** in your Cloudflare Dashboard:
   ```
   https://dash.cloudflare.com/[YOUR_ACCOUNT_ID]/r2/overview
   ```

2. Click **"Create bucket"**

3. Enter bucket details:
   - **Bucket name**: `news-images` (or your preferred name)
   - **Location**: Choose automatic or specific location

4. Click **"Create bucket"**

## Step 2: Generate API Tokens

### Option A: R2 API Tokens (Recommended)

1. Go to **R2** → **Overview** → **Manage R2 API Tokens**
   ```
   https://dash.cloudflare.com/[YOUR_ACCOUNT_ID]/r2/api-tokens
   ```

2. Click **"Create API Token"**

3. Configure token:
   - **Token name**: `newsportal-backend-token`
   - **Permissions**: 
     - ✅ Object Read & Write
     - ✅ Optional: Edit Permissions (if you need bucket management)
   - **Bucket access**: 
     - Apply to specific buckets only
     - Select: `news-images`
   - **TTL**: No expiration (or set custom duration)

4. Click **"Create API Token"**

5. **IMPORTANT**: Copy these values immediately (shown only once):
   - **Access Key ID**: `xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx`
   - **Secret Access Key**: `yyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyy`

6. **Save these credentials securely** - you won't be able to see them again!

### Option B: Global API Key (Not Recommended)

- Less secure, provides full account access
- Use R2 API Tokens instead for better security

## Step 3: Get Your R2 Endpoint

1. In your R2 bucket dashboard, find your **Account ID** (in the URL or sidebar)

2. Your R2 endpoint will be in the format:
   ```
   [ACCOUNT_ID].r2.cloudflarestorage.com
   ```

3. Example:
   ```
   7ac015923324a4d426c1f7782c3f41e1.r2.cloudflarestorage.com
   ```

## Step 4: Configure Your Application

### Development (Local Docker)

Update `.env` file:

```env
# Cloudflare R2 Settings
MINIO_ROOT_USER=your_r2_access_key_id_here
MINIO_ROOT_PASSWORD=your_r2_secret_access_key_here
```

Update `appsettings.Development.json`:

```json
{
  "MinioSettings": {
    "Endpoint": "7ac015923324a4d426c1f7782c3f41e1.r2.cloudflarestorage.com",
    "AccessKey": "your_r2_access_key_id_here",
    "SecretKey": "your_r2_secret_access_key_here",
    "BucketName": "news-images",
    "UseSSL": true,
    "Region": "auto",
    "MaxFileSizeBytes": 10485760,
    "ThumbnailWidth": 400,
    "ThumbnailHeight": 300
  }
}
```

### Production (Azure Container Apps)

#### Method 1: Using Azure CLI (Current Setup)

```powershell
# Set R2 credentials as secrets
az containerapp secret set `
  --name newsportal-backend `
  --resource-group newsportal-rg `
  --secrets `
    minio-access-key="your_r2_access_key_id_here" `
    minio-secret-key="your_r2_secret_access_key_here"

# Update environment variables
az containerapp update `
  --name newsportal-backend `
  --resource-group newsportal-rg `
  --set-env-vars `
    MinioSettings__Endpoint="7ac015923324a4d426c1f7782c3f41e1.r2.cloudflarestorage.com" `
    MinioSettings__UseSSL="true" `
    MinioSettings__Region="auto" `
  --replace-env-vars `
    MinioSettings__AccessKey=secretref:minio-access-key `
    MinioSettings__SecretKey=secretref:minio-secret-key
```

#### Method 2: Using Azure Portal

1. Open **Azure Container Apps** → `newsportal-backend`
2. Go to **Settings** → **Secrets**
3. Add secrets:
   - `minio-access-key`: Your R2 Access Key ID
   - `minio-secret-key`: Your R2 Secret Access Key
4. Go to **Settings** → **Environment Variables**
5. Update variables:
   - `MinioSettings__Endpoint` = `7ac015923324a4d426c1f7782c3f41e1.r2.cloudflarestorage.com`
   - `MinioSettings__AccessKey` = Reference secret `minio-access-key`
   - `MinioSettings__SecretKey` = Reference secret `minio-secret-key`
   - `MinioSettings__UseSSL` = `true`
   - `MinioSettings__Region` = `auto`
6. Save and restart container

## Step 5: Update Docker Compose (Optional for Local Testing)

If you want to test R2 locally instead of using MinIO:

```yaml
services:
  newsapi:
    environment:
      - MinioSettings__Endpoint=7ac015923324a4d426c1f7782c3f41e1.r2.cloudflarestorage.com
      - MinioSettings__AccessKey=${R2_ACCESS_KEY_ID}
      - MinioSettings__SecretKey=${R2_SECRET_ACCESS_KEY}
      - MinioSettings__BucketName=news-images
      - MinioSettings__UseSSL=true
      - MinioSettings__Region=auto
    # Remove dependency on minio container
    depends_on:
      mongodb:
        condition: service_healthy
      # minio:
      #   condition: service_healthy
```

Then update `.env`:

```env
R2_ACCESS_KEY_ID=your_r2_access_key_here
R2_SECRET_ACCESS_KEY=your_r2_secret_key_here
```

## Step 6: Enable Public Access (Optional)

If you want images to be publicly accessible without presigned URLs:

### Option 1: R2.dev Domain (Simple)

1. Go to your bucket settings
2. Enable **"R2.dev subdomain"**
3. Your public URL will be: `https://pub-[hash].r2.dev/[object-key]`

### Option 2: Custom Domain (Recommended for Production)

1. Go to bucket **Settings** → **Custom Domains**
2. Add your domain: `images.yourdomain.com`
3. Update DNS:
   - Add CNAME: `images.yourdomain.com` → `[your-bucket].r2.cloudflarestorage.com`
4. Configure CORS if needed

### Option 3: Public Bucket Policy

Set bucket policy to allow public read access:

```json
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Effect": "Allow",
      "Principal": "*",
      "Action": "s3:GetObject",
      "Resource": "arn:aws:s3:::news-images/*"
    }
  ]
}
```

## Step 7: Migrate Existing Images (If Any)

### Using rclone (Recommended)

1. Install rclone: https://rclone.org/downloads/

2. Configure MinIO source:
   ```bash
   rclone config create minio s3 \
     --s3-provider Minio \
     --s3-endpoint http://localhost:9000 \
     --s3-access-key-id minioadmin \
     --s3-secret-access-key minioadmin123
   ```

3. Configure R2 destination:
   ```bash
   rclone config create r2 s3 \
     --s3-provider Cloudflare \
     --s3-endpoint https://7ac015923324a4d426c1f7782c3f41e1.r2.cloudflarestorage.com \
     --s3-access-key-id YOUR_R2_ACCESS_KEY \
     --s3-secret-access-key YOUR_R2_SECRET_KEY
   ```

4. Copy data:
   ```bash
   rclone copy minio:news-images r2:news-images --progress
   ```

### Using MinIO Client (mc)

```bash
# Configure MinIO source
mc alias set myminio http://localhost:9000 minioadmin minioadmin123

# Configure R2 destination
mc alias set r2 https://7ac015923324a4d426c1f7782c3f41e1.r2.cloudflarestorage.com YOUR_ACCESS_KEY YOUR_SECRET_KEY

# Copy bucket
mc cp --recursive myminio/news-images r2/news-images
```

## Step 8: Test the Integration

1. Restart your application:
   ```bash
   docker-compose restart newsapi
   # OR for Azure
   az containerapp revision restart --name newsportal-backend --resource-group newsportal-rg
   ```

2. Test image upload via Swagger:
   - Navigate to: http://localhost:5000/swagger (local) or your Azure URL
   - Authenticate with JWT token
   - Use `POST /api/News` to create a news article with an image
   - Verify image is uploaded to R2 bucket

3. Check R2 dashboard:
   - Go to your bucket in Cloudflare Dashboard
   - Verify uploaded objects appear

4. Test image retrieval:
   - Get presigned URL or public URL
   - Open in browser to verify image loads

## Troubleshooting

### Connection Issues

**Error**: "The request signature we calculated does not match"
- **Solution**: Verify Access Key ID and Secret Access Key are correct
- Check for extra spaces or newlines in credentials

**Error**: "NoSuchBucket"
- **Solution**: Ensure bucket name matches exactly (case-sensitive)
- Verify bucket exists in R2 dashboard

**Error**: "SSL connection failed"
- **Solution**: Ensure `UseSSL: true` in configuration
- Verify endpoint doesn't include `http://` or `https://` prefix

### Permission Issues

**Error**: "Access Denied"
- **Solution**: Check API token has Object Read & Write permissions
- Verify token is scoped to correct bucket

### Code Compatibility

The existing MinIO SDK (`Minio` NuGet package) is S3-compatible and works with R2 out of the box. No code changes needed!

## Cost Estimation

Cloudflare R2 Pricing (as of 2025):

- **Storage**: $0.015 per GB/month
- **Class A Operations** (PUT, POST, DELETE): $4.50 per million
- **Class B Operations** (GET, LIST): $0.36 per million
- **Egress**: **$0.00** (FREE!)

Example for 1000 news articles with images:
- 1000 images × 500KB avg = ~500MB storage = **$0.0075/month**
- 1000 uploads (Class A) = **$0.0045**
- 100,000 views (Class B) = **$0.036**
- **Total**: ~$0.05/month for 100k image views 🎉

## Security Best Practices

1. ✅ Use R2 API Tokens (not Global API Key)
2. ✅ Scope tokens to specific buckets only
3. ✅ Store credentials in Azure Key Vault or Container Apps secrets
4. ✅ Never commit credentials to Git
5. ✅ Use HTTPS/SSL for all connections
6. ✅ Rotate API tokens periodically
7. ✅ Enable Cloudflare Access for admin operations
8. ✅ Set token expiration dates

## Rollback Plan

If you need to switch back to MinIO:

1. Update environment variables to MinIO settings
2. Restart application
3. Optionally copy data back from R2 to MinIO using rclone

## Next Steps

1. **Enable CDN**: Configure Cloudflare CDN for faster image delivery
2. **Image Transformations**: Use Cloudflare Images for on-the-fly resizing
3. **Monitoring**: Set up R2 analytics to track usage
4. **Backup**: Configure R2 lifecycle policies for archival

## Support & Resources

- **R2 Documentation**: https://developers.cloudflare.com/r2/
- **S3 API Compatibility**: https://developers.cloudflare.com/r2/api/s3/
- **Pricing**: https://developers.cloudflare.com/r2/pricing/
- **Community**: https://community.cloudflare.com/

---

**Setup Script Available**: See `scripts/setup-r2.ps1` for automated configuration
