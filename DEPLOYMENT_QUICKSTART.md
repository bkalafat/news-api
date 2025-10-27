# 🚀 Deployment Hızlı Başlangıç

## 📋 Gerekli Hesaplar

Şu platformlarda hesap oluştur (hepsi ücretsiz):

1. ✅ **MongoDB Atlas** - https://www.mongodb.com/cloud/atlas/register
2. ✅ **Cloudflare** - https://dash.cloudflare.com/sign-up
3. ✅ **Render** - https://dashboard.render.com/register
4. ✅ **Vercel** - https://vercel.com/signup
5. ✅ **Unsplash** - https://unsplash.com/developers (opsiyonel)

## 🎯 5 Adımda Deployment

### 1️⃣ MongoDB Atlas Kurulumu (5 dakika)

```bash
1. https://www.mongodb.com/cloud/atlas/register → Hesap oluştur
2. "Create a Cluster" → M0 Free Tier seç
3. Region: Frankfurt (AWS)
4. Cluster Name: newsportal-cluster
5. Create Cluster

6. Security → Database Access → Add New Database User
   Username: newsportal_admin
   Password: [güçlü_şifre_oluştur]  # KAYDET!
   
7. Security → Network Access → Add IP Address
   Access List Entry: 0.0.0.0/0 (Allow access from anywhere)
   
8. Connect → Connect your application
   Driver: C#/.NET
   Connection String KOPYALA:
   mongodb+srv://bkalafat:dbuserpassword123@cluster0.xwbfl1o.mongodb.net/
   
   [password] kısmını şifrenle değiştir!
```

### 2️⃣ Cloudflare R2 Kurulumu (5 dakika)

```bash
1. https://dash.cloudflare.com → Login
2. R2 → Create bucket
   Bucket name: news-images
   Location: Automatic
   
3. Settings → R2 API Tokens → Create API Token
   Permission: Object Read & Write
   TTL: Forever
   
   KAYDET:
   - Access Key ID: xxxxxxxxxxxxx
   - Secret Access Key: yyyyyyyyyyyyyy
   
4. Bucket Settings → Public Access → Allow
5. Custom domain (opsiyonel): images.newsportal.com
```

### 3️⃣ Render Backend Deployment (10 dakika)

```bash
1. Git push yap (render.yaml ve workflows zaten hazır)
   git add .
   git commit -m "Add deployment configs"
   git push origin main

2. https://dashboard.render.com → New → Web Service
   Connect GitHub repository: bkalafat/newsportal
   
3. Settings:
   Name: newsportal-backend
   Region: Frankfurt
   Branch: main
   Root Directory: backend
   Runtime: Docker
   Dockerfile Path: ../Dockerfile
   
4. Environment Variables ekle:
   
   MongoDbSettings__ConnectionString = [MongoDB Atlas connection string]
   MongoDbSettings__DatabaseName = NewsDb
   
   R2Settings__Endpoint = https://[account-id].r2.cloudflarestorage.com
   R2Settings__AccessKey = [R2 Access Key ID]
   R2Settings__SecretKey = [R2 Secret Access Key]
   R2Settings__BucketName = news-images
   R2Settings__Region = auto
   
   JwtSettings__SecretKey = [Generate edilecek - Render otomatik üretir]
   JwtSettings__Issuer = NewsPortalAPI
   JwtSettings__Audience = NewsPortalClients
   
   AllowedOrigins = https://newsportal.vercel.app
   
   ASPNETCORE_ENVIRONMENT = Production
   ASPNETCORE_URLS = http://+:8080

5. Create Web Service
6. Bekle (5-10 dakika) → Deploy tamamlanınca URL'i kopyala:
   https://newsportal-backend.onrender.com
   
7. Test et:
   curl https://newsportal-backend.onrender.com/health
   Response: "Healthy"
```

### 4️⃣ Vercel Frontend Deployment (3 dakika)

```bash
1. Terminal'de:
   cd frontend
   
2. .env.production dosyası oluştur:
   NEXT_PUBLIC_API_URL=https://newsportal-backend.onrender.com/api
   NEXT_PUBLIC_SITE_URL=https://newsportal.vercel.app

3. Vercel CLI kur ve login:
   npm i -g vercel
   vercel login

4. Deploy et:
   vercel
   
   Sorular:
   - Set up and deploy? Y
   - Scope: [seç]
   - Link to existing project? N
   - Project name: newsportal-frontend
   - Directory: ./
   - Override settings? N
   
5. Production deploy:
   vercel --prod
   
6. URL'i kopyala:
   https://newsportal-frontend.vercel.app
   
7. Tarayıcıda test et:
   https://newsportal-frontend.vercel.app
```

### 5️⃣ GitHub Actions Kurulumu (5 dakika)

```bash
1. GitHub Repository → Settings → Secrets → Actions
2. New repository secret ekle:

   Name: API_BASE_URL
   Value: https://newsportal-backend.onrender.com/api
   
   Name: ADMIN_USERNAME
   Value: admin
   
   Name: ADMIN_PASSWORD
   Value: [Render backend admin şifresi]
   
   Name: UNSPLASH_ACCESS_KEY
   Value: [Unsplash API key - opsiyonel]

3. Git push yap (.github/workflows dosyaları zaten hazır):
   git add .github/workflows/
   git commit -m "Add GitHub Actions workflows"
   git push origin main

4. GitHub → Actions → Daily News Aggregation
5. Run workflow (test için)
6. Logs'u kontrol et → başarılı olmalı

7. Otomatik zamanlama:
   Artık her gün saat 05:00 (Türkiye saati) otomatik çalışacak!
```

## ✅ Deployment Checklist

Tamamlandıktan sonra kontrol et:

- [ ] MongoDB Atlas cluster çalışıyor
- [ ] Cloudflare R2 bucket oluşturuldu
- [ ] Render backend deploy edildi ve healthy
- [ ] Vercel frontend deploy edildi
- [ ] GitHub Actions secrets eklendi
- [ ] İlk manuel aggregation başarılı
- [ ] Frontend → Backend bağlantısı çalışıyor
- [ ] Her 10 dakikada keep-alive ping atıyor
- [ ] Backend sleep olmuyor

## 🧪 Test Etme

### Backend Test

```bash
# Health check
curl https://newsportal-backend.onrender.com/health

# Swagger UI
https://newsportal-backend.onrender.com/swagger

# API endpoint
curl https://newsportal-backend.onrender.com/api/NewsArticle
```

### Frontend Test

```bash
# Anasayfa
https://newsportal-frontend.vercel.app

# Kategoriler
https://newsportal-frontend.vercel.app/categories

# Admin
https://newsportal-frontend.vercel.app/admin
```

### GitHub Actions Test

```bash
# Manuel çalıştır
GitHub → Actions → Daily News Aggregation → Run workflow

# Logları kontrol et
Actions → Son çalıştırma → aggregate-news → View logs

# Artifact indir (loglar)
Actions → Son çalıştırma → Artifacts → aggregation-logs
```

### Cron Job Test

```bash
# MongoDB'de son haberleri kontrol et
MongoDB Atlas → Clusters → Browse Collections → News
Sort by: CreatedAt (descending)
Limit: 10

# Son aggregation'dan haberler görülmeli
```

## 🔍 Monitoring

### 1. Backend Uptime (UptimeRobot)

```bash
1. https://uptimerobot.com → Sign Up (ücretsiz)
2. Add New Monitor
   Monitor Type: HTTP(s)
   Friendly Name: NewsPortal Backend
   URL: https://newsportal-backend.onrender.com/health
   Monitoring Interval: 5 minutes
   
3. Alerts ekle (Email/SMS)
```

### 2. Frontend Analytics (Vercel)

```bash
Vercel Dashboard → newsportal-frontend → Analytics
- Pageviews
- Unique visitors
- Top pages
- Web vitals
```

### 3. Database Monitoring (MongoDB Atlas)

```bash
MongoDB Atlas → Clusters → Metrics
- Operations
- Connections
- Network
- Storage
```

### 4. Logs

```bash
# Backend logs (Render)
Render Dashboard → newsportal-backend → Logs

# Frontend logs (Vercel)
Vercel Dashboard → newsportal-frontend → Logs

# Aggregation logs (GitHub Actions)
GitHub → Actions → Latest run → Artifacts
```

## 🆘 Sorun Giderme

### Backend 503 hatası

```bash
# Render kontrol et
Render Dashboard → newsportal-backend → Events

# Sık sebep: Environment variables eksik
Settings → Environment → Tüm değişkenler ekli mi kontrol et

# Deploy yenile
Manual Deploy → Deploy latest commit
```

### Frontend backend'e bağlanamıyor

```bash
# CORS kontrolü
Render → Environment → AllowedOrigins doğru mu?
AllowedOrigins=https://newsportal-frontend.vercel.app

# Environment variable kontrolü (Vercel)
Vercel Dashboard → Settings → Environment Variables
NEXT_PUBLIC_API_URL doğru mu?
```

### GitHub Actions cron çalışmıyor

```bash
# Secrets kontrolü
GitHub → Settings → Secrets → Actions
Tüm secrets ekli mi?

# UTC saat kontrolü
Türkiye 05:00 = UTC 02:00
Cron: '0 2 * * *' doğru mu?

# Manuel test
Actions → Daily News Aggregation → Run workflow
```

### MongoDB bağlantı hatası

```bash
# IP whitelist kontrolü
MongoDB Atlas → Network Access
0.0.0.0/0 ekli mi?

# Connection string kontrolü
Render → Environment → MongoDbSettings__ConnectionString
mongodb+srv://... formatında mı?
Şifre doğru mu? (özel karakterler encode edilmeli)

# Test
Render → Shell
echo $MongoDbSettings__ConnectionString
```

## 💰 Maliyet Takibi

### MongoDB Atlas

```bash
Dashboard → Billing
Free Tier: 512 MB (kullanım: ~50-100 MB)
```

### Render

```bash
Dashboard → Billing
Free Tier: 750 saat/ay (kullanım: 720 saat = 7/24)
```

### Vercel

```bash
Dashboard → Usage
Free Tier: 100 GB bandwidth (kullanım: ~1-5 GB/ay)
```

### Cloudflare R2

```bash
Dashboard → R2 → Usage
Free Tier: 10 GB storage (kullanım: ~500 MB - 1 GB)
```

**Toplam Maliyet:** $0/ay (tüm limitler dahilinde) ✅

## 🎉 Tamamlandı!

Artık sisteminiz:
- ✅ 7/24 canlı
- ✅ Her sabah 5'te otomatik haber topluyor
- ✅ 15+ kaynaktan ~150 haber/hafta
- ✅ Ücretsiz çalışıyor
- ✅ Global CDN ile hızlı
- ✅ SSL güvenli
- ✅ Monitoring aktif

**Frontend URL:** https://newsportal-frontend.vercel.app
**Backend URL:** https://newsportal-backend.onrender.com
**Admin Panel:** https://newsportal-frontend.vercel.app/admin

Hayırlı olsun! 🚀
