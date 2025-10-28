# Cloudflare Tunnel - Free Self-Hosting Guide
# Exposes your local Docker containers to the internet for FREE
# No port forwarding needed, no dynamic DNS

## What is Cloudflare Tunnel?
- Free service from Cloudflare
- Creates secure tunnel from your PC to Cloudflare network
- No need to open ports on router
- Free SSL certificate
- No static IP required
- Works behind NAT/firewall

## Prerequisites:
- Cloudflare account (free)
- Docker running on your PC
- Domain name (optional, can use *.trycloudflare.com)

## Step 1: Install Cloudflare Tunnel

### Windows (PowerShell):
```powershell
# Download cloudflared
Invoke-WebRequest -Uri "https://github.com/cloudflare/cloudflared/releases/latest/download/cloudflared-windows-amd64.exe" -OutFile "cloudflared.exe"

# Move to a permanent location
Move-Item cloudflared.exe C:\cloudflared\cloudflared.exe -Force

# Add to PATH
$env:Path += ";C:\cloudflared"
```

## Step 2: Authenticate with Cloudflare
```powershell
cloudflared tunnel login
```
This will open browser - login with your Cloudflare account

## Step 3: Create a Tunnel
```powershell
# Create tunnel
cloudflared tunnel create newsportal

# This creates a tunnel and gives you a UUID (save this!)
# Example: Created tunnel newsportal with id: abc123-def456-ghi789
```

## Step 4: Configure Tunnel

Create config file: `C:\cloudflared\config.yml`

```yaml
tunnel: newsportal  # Your tunnel name
credentials-file: C:\Users\YOUR_USERNAME\.cloudflared\abc123-def456-ghi789.json

ingress:
  # Route for backend API
  - hostname: api.newsportal.com  # Your domain (or use *.trycloudflare.com)
    service: http://localhost:5000
    originRequest:
      noTLSVerify: true
  
  # Route for frontend (optional)
  - hostname: newsportal.com
    service: http://localhost:3000
    originRequest:
      noTLSVerify: true
  
  # Catch-all rule (required)
  - service: http_status:404
```

## Step 5: Route DNS (If using custom domain)
```powershell
# Point your domain to the tunnel
cloudflared tunnel route dns newsportal api.newsportal.com
cloudflared tunnel route dns newsportal newsportal.com
```

## Step 6: Run the Tunnel
```powershell
# Test run
cloudflared tunnel run newsportal

# Or run in background
cloudflared tunnel --config C:\cloudflared\config.yml run newsportal
```

## Step 7: Install as Windows Service (Auto-start on boot)
```powershell
# Install service
cloudflared service install

# Start service
net start cloudflared
```

## Quick Start (No Domain)

For testing without domain:

```powershell
# Quick tunnel (no account needed!)
cloudflared tunnel --url http://localhost:5000

# You'll get a temporary URL like:
# https://random-words-123.trycloudflare.com
```

## With Your Docker Setup

Your current setup with docker-compose.yml:

```powershell
# Start your Docker containers
docker-compose up -d

# Start Cloudflare tunnel pointing to backend
cloudflared tunnel --url http://localhost:5000
```

## Cost:
- Cloudflare Tunnel: **FREE** ✅
- Domain (optional): ~$10-15/year
- Your electricity: ~₺50-100/month

## Advantages:
✅ Completely free (except domain if you want one)
✅ No port forwarding
✅ Works behind NAT/firewall
✅ Free SSL certificate
✅ DDoS protection from Cloudflare
✅ Can turn off PC without breaking URL (tunnel reconnects)

## Disadvantages:
⚠️ Your PC must be running 24/7
⚠️ Need stable internet connection
⚠️ Electricity costs
⚠️ If PC crashes, website goes down

## Recommended Setup:

1. Use Cloudflare Tunnel on your home PC
2. Keep Docker containers running 24/7
3. Use MongoDB Atlas (free - already setup)
4. Use Cloudflare R2 (free - already setup)
5. Get cheap domain from Namecheap (~₺150/year)

Total cost: ~₺150/year (just domain) + electricity

## Alternative: Quick Tunnel (No Setup)

Just run this right now:

```powershell
cd C:\dev\newsportal
docker-compose up -d
cloudflared tunnel --url http://localhost:5000
```

You'll get instant public URL! 🚀
