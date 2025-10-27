# 🚀 Ücretsiz Deployment Stratejisi - News Portal

## 📋 Sistem Gereksinimleri

| Servis | Teknoloji | İhtiyaç |
|--------|-----------|---------|
| **Backend API** | .NET 10 | Container desteği, scheduled tasks |
| **Frontend** | Next.js 16 | Serverless, edge functions |
| **Database** | MongoDB | Ücretsiz hosting, 512MB+ |
| **Object Storage** | MinIO/S3 | Resim depolama, CDN |
| **Cron Jobs** | Python Script | Scheduled execution (her sabah 5 AM) |

## 🎯 Önerilen Stack (100% Ücretsiz)

### ✅ **En İyi Çözüm: Hybrid Approach**

```
┌─────────────────────────────────────────────────────┐
│                   DEPLOYMENT STACK                   │
├─────────────────────────────────────────────────────┤
│                                                      │
│  Frontend (Next.js)  →  Vercel (Ücretsiz)           │
│  Backend (.NET 10)   →  Render/Railway (Ücretsiz)   │
│  MongoDB             →  MongoDB Atlas (Ücretsiz)     │
│  MinIO/S3            →  Cloudflare R2 (Ücretsiz)     │
│  Cron Jobs           →  GitHub Actions (Ücretsiz)    │
│  Domain              →  Cloudflare (Ücretsiz)        │
│                                                      │
└─────────────────────────────────────────────────────┘
```

---

## 🏆 Detaylı Platform Önerileri

### 1. **Frontend: Vercel** (⭐⭐⭐⭐⭐ Önerilen)

**Ücretsiz Limits:**
- ✅ Unlimited deployments
- ✅ 100 GB bandwidth/ay
- ✅ Automatic HTTPS
- ✅ Edge Functions (serverless)
- ✅ Git integration (auto deploy on push)
- ✅ Preview deployments (PR başına)

**Neden Vercel?**
- Next.js'in yaratıcıları
- En iyi Next.js performansı
- Otomatik CDN
- Zero config deployment

**Deployment:**
```bash
# 1. Vercel CLI kur
npm i -g vercel

# 2. Frontend'i deploy et
cd frontend
vercel

# 3. Production deploy
vercel --prod
```

**Alternatif:** Netlify (benzer özellikler)

---

### 2. **Backend: Render** (⭐⭐⭐⭐⭐ En İyi Ücretsiz)

**Ücretsiz Limits:**
- ✅ 750 saat/ay (7/24 çalışır)
- ✅ 512MB RAM
- ✅ Docker container desteği
- ✅ Auto-deploy from Git
- ✅ Free SSL
- ✅ Background workers (cron job desteği!)
- ⚠️ 15 dakika inactivity sonrası sleep (ilk istek 30 saniye sürer)

**Neden Render?**
- Docker desteği (newsportal-backend container'ınız çalışır)
- Background workers ile Python script'i scheduled çalıştırabilirsiniz
- Heroku benzeri ama ücretsiz
- .NET 10 desteği

**Deployment:**

1. `render.yaml` oluştur (proje root'unda):

```yaml
services:
  # Backend API
  - type: web
    name: newsportal-backend
    env: docker
    dockerfilePath: ./Dockerfile
    envVars:
      - key: ASPNETCORE_ENVIRONMENT
        value: Production
      - key: MONGODB_CONNECTION_STRING
        sync: false
      - key: JWT_SECRET_KEY
        generateValue: true
      - key: MINIO_ENDPOINT
        sync: false
    healthCheckPath: /health
    
  # Background Worker (News Aggregator)
  - type: cron
    name: news-aggregator
    env: python
    schedule: "0 5 * * *"  # Her gün 5:00 AM (UTC+3 için 0 2 * * *)
    buildCommand: "pip install -r scripts/requirements.txt"
    startCommand: "python scripts/news_aggregator.py --once"
    envVars:
      - key: API_BASE_URL
        value: https://newsportal-backend.onrender.com/api
```

2. Git'e push et → Render otomatik deploy eder

**Alternatifler:**
- **Railway** (⭐⭐⭐⭐): $5 ücretsiz credit/ay, daha iyi limits
- **Fly.io** (⭐⭐⭐⭐): 3 VM ücretsiz, edge deployment
- **Koyeb** (⭐⭐⭐): Ücretsiz tier, Avrupa sunucuları

---

### 3. **Database: MongoDB Atlas** (⭐⭐⭐⭐⭐ Standart)

**Ücretsiz Tier (M0):**
- ✅ 512 MB storage
- ✅ Shared RAM
- ✅ Ücretsiz forever
- ✅ AWS/GCP/Azure regions
- ✅ Automatic backups
- ✅ Database monitoring

**Kurulum:**

1. https://www.mongodb.com/cloud/atlas/register adresine git
2. M0 Free Tier seç
3. Region: Frankfurt veya Amsterdam (Avrupa)
4. Database oluştur → Connection string kopyala
5. Render'da environment variable olarak ekle:
   ```
   MONGODB_CONNECTION_STRING=mongodb+srv://user:pass@cluster.mongodb.net/NewsDb
   ```

**Alternatif:**
- **Railway PostgreSQL** (ücretsiz ama MongoDB değil)

---

### 4. **Object Storage: Cloudflare R2** (⭐⭐⭐⭐⭐ En İyi)

**Ücretsiz Limits:**
- ✅ 10 GB storage
- ✅ Unlimited egress (çıkış trafiği ücretsiz!)
- ✅ S3-compatible API (MinIO yerine)
- ✅ CDN dahil

**Neden R2?**
- MinIO yerine S3-compatible bulut storage
- Çıkış trafiği ücretli değil (AWS S3'te pahalı)
- Cloudflare CDN ile entegre

**Kurulum:**

1. https://dash.cloudflare.com → R2 → Create bucket
2. API token oluştur
3. Backend'de MinIO yerine S3 client kullan:

```csharp
// Infrastructure/Services/CloudflareR2Service.cs
public class CloudflareR2Service : IImageStorageService
{
    private readonly AmazonS3Client _s3Client;
    
    public CloudflareR2Service(IConfiguration config)
    {
        var s3Config = new AmazonS3Config
        {
            ServiceURL = "https://[account-id].r2.cloudflarestorage.com",
            SignatureVersion = "v4",
            ForcePathStyle = true
        };
        
        _s3Client = new AmazonS3Client(
            config["R2:AccessKey"],
            config["R2:SecretKey"],
            s3Config
        );
    }
    
    public async Task<string> UploadImageAsync(Stream imageStream, string fileName)
    {
        var request = new PutObjectRequest
        {
            BucketName = "news-images",
            Key = fileName,
            InputStream = imageStream,
            ContentType = "image/jpeg"
        };
        
        await _s3Client.PutObjectAsync(request);
        return $"https://news-images.[account].r2.dev/{fileName}";
    }
}
```

**Alternatifler:**
- **Cloudinary** (⭐⭐⭐⭐): 25 GB storage, image transforms
- **Backblaze B2** (⭐⭐⭐): 10 GB ücretsiz, S3-compatible
- **Supabase Storage** (⭐⭐⭐): 1 GB ücretsiz

---

### 5. **Cron Jobs: GitHub Actions** (⭐⭐⭐⭐⭐ En Güvenilir)

**Ücretsiz Limits:**
- ✅ 2000 dakika/ay (public repo'da unlimited)
- ✅ Scheduled workflows
- ✅ Secrets management

**Neden GitHub Actions?**
- Render'ın cron job'ı sleep moduna girebilir
- GitHub Actions her zaman çalışır
- Secrets güvenli şekilde saklanır

**Kurulum:**

`.github/workflows/news-aggregator.yml` oluştur:

```yaml
name: Daily News Aggregation

on:
  schedule:
    # Her gün saat 02:00 UTC (Türkiye 05:00)
    - cron: '0 2 * * *'
  workflow_dispatch: # Manuel çalıştırma için

jobs:
  aggregate-news:
    runs-on: ubuntu-latest
    
    steps:
      - name: Checkout code
        uses: actions/checkout@v4
      
      - name: Setup Python
        uses: actions/setup-python@v4
        with:
          python-version: '3.11'
      
      - name: Install dependencies
        run: |
          pip install requests schedule pytz
      
      - name: Run news aggregator
        env:
          API_BASE_URL: ${{ secrets.API_BASE_URL }}
          ADMIN_USERNAME: ${{ secrets.ADMIN_USERNAME }}
          ADMIN_PASSWORD: ${{ secrets.ADMIN_PASSWORD }}
          UNSPLASH_ACCESS_KEY: ${{ secrets.UNSPLASH_ACCESS_KEY }}
        run: |
          cd scripts
          python news_aggregator.py --once
      
      - name: Upload logs
        if: always()
        uses: actions/upload-artifact@v3
        with:
          name: aggregation-logs
          path: scripts/news_aggregator.log
```

**Secrets ayarla:**
1. GitHub repo → Settings → Secrets → Actions
2. Şu secret'ları ekle:
   - `API_BASE_URL`: https://newsportal-backend.onrender.com/api
   - `ADMIN_USERNAME`: admin
   - `ADMIN_PASSWORD`: güçlü_şifre
   - `UNSPLASH_ACCESS_KEY`: unsplash_key_buraya

**Manuel çalıştırma:** GitHub Actions tab → Run workflow

---

### 6. **Domain & CDN: Cloudflare** (⭐⭐⭐⭐⭐ Zorunlu)

**Ücretsiz:**
- ✅ DNS hosting
- ✅ CDN (global)
- ✅ DDoS protection
- ✅ SSL certificate
- ✅ Page Rules
- ✅ Analytics

**Kurulum:**

1. Domain satın al (GoDaddy, Namecheap) veya ücretsiz: Freenom
2. Cloudflare'e nameserver'ları değiştir
3. DNS Records ekle:
   ```
   newsportal.com          → CNAME → newsportal-frontend.vercel.app
   api.newsportal.com      → CNAME → newsportal-backend.onrender.com
   images.newsportal.com   → CNAME → news-images.[account].r2.dev
   ```

---

## 📦 Tam Deployment Senaryosu

### Senaryo 1: Tam Ücretsiz (Önerilen)

```
Frontend    → Vercel          (ücretsiz, unlimited)
Backend     → Render          (ücretsiz, 750h/ay)
Database    → MongoDB Atlas   (ücretsiz, 512 MB)
Storage     → Cloudflare R2   (ücretsiz, 10 GB)
Cron        → GitHub Actions  (ücretsiz, unlimited)
Domain      → Cloudflare DNS  (ücretsiz)
```

**Maliyet:** $0/ay
**Performans:** ⭐⭐⭐⭐ (iyi)
**Sorun:** Backend 15 dk sonra sleep mode (ilk istek yavaş)

---

### Senaryo 2: Hybrid (Backend Always On)

```
Frontend    → Vercel             (ücretsiz)
Backend     → Railway            ($5 credit/ay → ~200 saat always-on)
Database    → MongoDB Atlas      (ücretsiz)
Storage     → Cloudflare R2      (ücretsiz)
Cron        → GitHub Actions     (ücretsiz)
```

**Maliyet:** $0/ay (ilk ay credit), sonra ~$5/ay
**Performans:** ⭐⭐⭐⭐⭐ (mükemmel)
**Avantaj:** Backend hiç sleep olmaz

---

### Senaryo 3: Premium (Production Ready)

```
Frontend    → Vercel Pro         ($20/ay)
Backend     → Render Standard    ($7/ay)
Database    → MongoDB Atlas M10  ($9/ay)
Storage     → Cloudflare R2      ($0-1/ay)
Monitoring  → Better Uptime      (ücretsiz)
```

**Maliyet:** ~$36-40/ay
**Performans:** ⭐⭐⭐⭐⭐ (enterprise)
**Avantaj:** SLA garantisi, 7/24 support

---

## 🎯 Adım Adım Deployment Rehberi

### 1️⃣ MongoDB Atlas Kurulumu

```bash
# 1. MongoDB Atlas hesabı oluştur
https://www.mongodb.com/cloud/atlas/register

# 2. M0 Free Cluster oluştur
Region: Frankfurt (eu-central-1)
Cluster Name: newsportal-cluster

# 3. Database user oluştur
Username: newsportal_admin
Password: [güçlü_şifre]

# 4. Network Access → IP Whitelist
0.0.0.0/0 (her yerden erişim - production'da değiştir)

# 5. Connection string kopyala
mongodb+srv://newsportal_admin:[password]@newsportal-cluster.xxxxx.mongodb.net/NewsDb?retryWrites=true&w=majority
```

---

### 2️⃣ Cloudflare R2 Kurulumu

```bash
# 1. Cloudflare hesabı oluştur
https://dash.cloudflare.com

# 2. R2 → Create bucket
Bucket name: news-images
Region: Automatic (WEUR - Western Europe)

# 3. R2 API Token oluştur
Permissions: Object Read & Write
Copy: Access Key ID + Secret Access Key

# 4. Public access ayarla
Settings → Public Access → Allow

# 5. Custom domain (optional)
images.newsportal.com → bucket'a bağla
```

---

### 3️⃣ Backend Deployment (Render)

```bash
# 1. render.yaml dosyası zaten hazır (yukarıda)

# 2. GitHub'a push et
git add .
git commit -m "Add Render deployment config"
git push origin main

# 3. Render dashboard
https://dashboard.render.com → New → Blueprint

# 4. GitHub repo'yu bağla
Repository: bkalafat/newsportal
Branch: main

# 5. Environment variables ekle
MONGODB_CONNECTION_STRING=mongodb+srv://...
JWT_SECRET_KEY=[generate]
R2_ENDPOINT=https://[account-id].r2.cloudflarestorage.com
R2_ACCESS_KEY=[R2 access key]
R2_SECRET_KEY=[R2 secret key]
R2_BUCKET_NAME=news-images

# 6. Deploy et
Create Blueprint → Wait 5-10 minutes

# 7. Health check
curl https://newsportal-backend.onrender.com/health
```

---

### 4️⃣ Frontend Deployment (Vercel)

```bash
# 1. Vercel CLI kur
npm i -g vercel

# 2. Frontend environment variables (.env.production)
cd frontend
cat > .env.production << EOF
NEXT_PUBLIC_API_URL=https://newsportal-backend.onrender.com/api
NEXT_PUBLIC_SITE_URL=https://newsportal.vercel.app
EOF

# 3. Deploy et
vercel

# 4. Production deploy
vercel --prod

# 5. URL notu al
https://newsportal-frontend.vercel.app

# 6. Custom domain ekle (optional)
Vercel Dashboard → Settings → Domains
Add: newsportal.com
```

---

### 5️⃣ GitHub Actions Cron Setup

```bash
# 1. Workflow dosyası zaten oluşturuldu (.github/workflows/news-aggregator.yml)

# 2. Secrets ekle
GitHub Repo → Settings → Secrets → Actions → New repository secret

API_BASE_URL: https://newsportal-backend.onrender.com/api
ADMIN_USERNAME: admin
ADMIN_PASSWORD: [güçlü_şifre]
UNSPLASH_ACCESS_KEY: [unsplash_key]

# 3. Git push et
git add .github/workflows/news-aggregator.yml
git commit -m "Add daily news aggregation workflow"
git push origin main

# 4. İlk çalıştırma (test)
GitHub → Actions → Daily News Aggregation → Run workflow

# 5. Logları kontrol et
Actions → Son çalıştırma → aggregate-news → Logs

# 6. Otomatik çalışma
Artık her gün saat 05:00 (Türkiye saati) otomatik çalışacak
```

---

### 6️⃣ Cloudflare DNS

```bash
# 1. Domain satın al (optional)
Namecheap, GoDaddy, veya ücretsiz: Freenom

# 2. Cloudflare'e ekle
Cloudflare → Add Site → newsportal.com

# 3. Nameserver'ları değiştir (domain provider'da)
ns1.cloudflare.com
ns2.cloudflare.com

# 4. DNS Records ekle
Type   Name      Target                                TTL
CNAME  @         newsportal-frontend.vercel.app       Auto
CNAME  www       newsportal-frontend.vercel.app       Auto
CNAME  api       newsportal-backend.onrender.com      Auto
CNAME  images    news-images.[account].r2.dev         Auto

# 5. SSL/TLS → Full (strict)

# 6. Speed → Optimization
Auto Minify: JS, CSS, HTML
Brotli: On
```

---

## 🔍 Monitoring & Maintenance

### 1. **Uptime Monitoring** (Ücretsiz)

**Better Uptime** (Önerilen):
- https://betteruptime.com
- 10 monitor ücretsiz
- SMS/Email alerts

**Monitors:**
```
1. https://newsportal.com (Frontend)
2. https://newsportal-backend.onrender.com/health (Backend)
3. https://newsportal-backend.onrender.com/api/NewsArticle (API)
```

### 2. **Error Tracking**

**Sentry** (Ücretsiz):
- 5000 error/ay
- Frontend + Backend integration

```bash
# Frontend
npm install @sentry/nextjs
npx @sentry/wizard -i nextjs

# Backend (.NET)
dotnet add package Sentry.AspNetCore
```

### 3. **Analytics**

**Vercel Analytics** (Ücretsiz):
- Pageview tracking
- Web vitals
- Zero config

**Alternatif:** Google Analytics 4

---

## 📊 Maliyet Karşılaştırması

| Platform | Ücretsiz Tier | Upgrade Maliyet |
|----------|---------------|-----------------|
| **Vercel** | 100 GB/ay | $20/ay (Pro) |
| **Render** | 750h/ay (sleep) | $7/ay (always-on) |
| **Railway** | $5 credit/ay | $20/500h sonrası |
| **MongoDB Atlas** | 512 MB | $9/ay (2GB M10) |
| **Cloudflare R2** | 10 GB | $0.015/GB sonrası |
| **GitHub Actions** | 2000 dk/ay | Nadiren gerekir |

**Toplam Ücretsiz:** $0/ay (limitler dahilinde)
**Upgrade Edilirse:** ~$36-50/ay (production-ready)

---

## ⚡ Performans İyileştirmeleri

### 1. **Render Sleep Problem Çözümü**

Render ücretsiz tier 15 dk sonra sleep. Çözüm:

**A) Kendi Ping Servisi (GitHub Actions):**

`.github/workflows/keep-alive.yml`:

```yaml
name: Keep Backend Alive

on:
  schedule:
    - cron: '*/10 * * * *'  # Her 10 dakikada bir

jobs:
  ping:
    runs-on: ubuntu-latest
    steps:
      - name: Ping backend
        run: curl https://newsportal-backend.onrender.com/health
```

**B) UptimeRobot (Ücretsiz):**
- https://uptimerobot.com
- 50 monitor ücretsiz
- 5 dakika interval
- Otomatik backend'i ayakta tutar

### 2. **CDN Optimizasyonu**

Cloudflare settings:
```
Caching → Cache Level: Standard
Speed → Auto Minify: JS, CSS, HTML
Speed → Brotli: On
Speed → Rocket Loader: On
```

### 3. **Database Indexing**

MongoDB Atlas:
```javascript
// Performans için index'ler
db.News.createIndex({ Slug: 1 }, { unique: true });
db.News.createIndex({ Category: 1, CreatedAt: -1 });
db.News.createIndex({ IsActive: 1, ExpressDate: -1 });
```

---

## 🚨 Sık Sorunlar & Çözümler

### 1. **Render backend sleep oluyor**

**Çözüm:** UptimeRobot ile 5 dakikada bir ping at

### 2. **MongoDB connection timeout**

**Çözüm:** 
```csharp
// Connection string'e timeout ekle
mongodb+srv://...?connectTimeoutMS=10000&socketTimeoutMS=30000
```

### 3. **Vercel build timeout (Next.js)**

**Çözüm:**
```json
// vercel.json
{
  "builds": [
    {
      "src": "package.json",
      "use": "@vercel/next",
      "config": { "maxDuration": 300 }
    }
  ]
}
```

### 4. **GitHub Actions cron çalışmıyor**

**Çözüm:**
- Public repo olduğundan emin ol
- UTC saat dilimini kontrol et (Türkiye +3)
- Manuel run workflow ile test et

---

## ✅ Deployment Checklist

- [ ] MongoDB Atlas cluster oluşturuldu
- [ ] Cloudflare R2 bucket oluşturuldu
- [ ] Render backend deploy edildi
- [ ] Vercel frontend deploy edildi
- [ ] GitHub Actions workflow eklendi
- [ ] GitHub Secrets ayarlandı
- [ ] Cloudflare DNS yapılandırıldı
- [ ] UptimeRobot ping ayarlandı
- [ ] Sentry error tracking eklendi
- [ ] İlk manuel aggregation çalıştırıldı
- [ ] Frontend → Backend bağlantısı test edildi
- [ ] Cron job test edildi (GitHub Actions)
- [ ] SSL sertifikası aktif
- [ ] Custom domain bağlandı (optional)

---

## 🎯 Özet: En İyi Kombinasyon

```
✅ Frontend:    Vercel (ücretsiz, mükemmel Next.js desteği)
✅ Backend:     Render (ücretsiz, Docker + cron desteği)
✅ Database:    MongoDB Atlas (ücretsiz, 512MB yeterli)
✅ Storage:     Cloudflare R2 (ücretsiz, S3-compatible)
✅ Cron:        GitHub Actions (ücretsiz, güvenilir)
✅ Domain/CDN:  Cloudflare (ücretsiz, global CDN)
✅ Monitoring:  UptimeRobot + Better Uptime (ücretsiz)
✅ Errors:      Sentry (ücretsiz, 5k error/ay)

TOPLAM MALİYET: $0/ay (tüm limitler dahilinde)
```

**Her sabah 5:00'te GitHub Actions otomatik olarak:**
1. news_aggregator.py script'ini çalıştırır
2. 15+ kaynaktan ~150 haber toplar
3. Türkçe'ye çevirir
4. Render backend API'ye POST eder
5. MongoDB'ye kaydedilir
6. Frontend'de görünür

**Sistem 7/24 çalışır, sıfır maliyet!** 🎉
