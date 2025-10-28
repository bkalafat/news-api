# ngrok - Instant Public URL for Your Local Server
# Free tier: 1 online process, temporary URLs

## Step 1: Download ngrok
# Go to: https://ngrok.com/download
# Or use PowerShell:
Invoke-WebRequest -Uri "https://bin.equinox.io/c/bNyj1mQVY4c/ngrok-v3-stable-windows-amd64.zip" -OutFile "ngrok.zip"
Expand-Archive ngrok.zip -DestinationPath C:\ngrok

## Step 2: Sign Up (Free)
# Go to: https://dashboard.ngrok.com/signup
# Get your auth token

## Step 3: Authenticate
C:\ngrok\ngrok config add-authtoken YOUR_TOKEN_HERE

## Step 4: Start Your Backend
docker-compose up -d

## Step 5: Expose to Internet
C:\ngrok\ngrok http 5000

## You'll get URL like:
# https://abc123.ngrok-free.app

## Free Tier Limits:
# ✅ Unlimited requests
# ✅ 1 online process
# ⚠️ URL changes every restart
# ⚠️ ngrok banner on web pages

## Paid: $8/month for static domain
