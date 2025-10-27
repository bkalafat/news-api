# Gelişmiş Haber Toplama Sistemi - Kurulum Rehberi

## 🎯 Özellikler

### Haber Kaynakları (15+ Kaynak)

#### **Reddit Kaynakları**
- ✅ **r/technology** - Genel teknoloji haberleri (20 haber/hafta)
- ✅ **r/programming** - Programlama haberleri (15 haber/hafta)
- ✅ **r/artificial** - Yapay zeka haberleri (15 haber/hafta)
- ✅ **r/LocalLLaMA** - Local LLM'ler (15 haber/hafta)
- ✅ **r/robotics** - Robotik haberleri (15 haber/hafta)
- ✅ **r/github** - GitHub Copilot ve GitHub haberleri (10 haber/hafta)
- ✅ **r/Android** - Android ve cep telefonu haberleri (10 haber/hafta)
- ✅ **r/iphone** - iPhone haberleri (10 haber/hafta)
- ✅ **r/Turkey** - Türkiye teknoloji haberleri (10 haber/hafta)

#### **Diğer Kaynaklar**
- ✅ **GitHub Trending** - Trend olan repository'ler (15 repo/hafta)
- ✅ **HackerNews** - En popüler teknoloji haberleri (20 haber/hafta)
- ✅ **Webrazzi** - Türk teknoloji haberleri (RSS)
- ✅ **ShiftDelete.Net** - Türk teknoloji haberleri (RSS)

### Akıllı Özellikler

- 🔄 **Otomatik Çeviri**: İngilizce → Türkçe (akıllı dil algılama)
- 🖼️ **Akıllı Resim Bulma**: Unsplash API ile başlığa uygun resim arama
- 🚫 **Tekrar Önleme**: Slug tabanlı benzersiz haber kontrolü
- ⏰ **Otomatik Zamanlama**: Her sabah saat 5:00 (Türkiye saati)
- 📊 **Detaylı Loglama**: Dosya + konsol logları
- 🌍 **Türkiye Saati Desteği**: pytz ile saat dilimi yönetimi

## 📦 Kurulum

### 1. Bağımlılıkları Yükle

```bash
pip install requests schedule pytz
```

### 2. Unsplash API Key Al (Opsiyonel ama Önerilen)

1. https://unsplash.com/developers adresine git
2. Ücretsiz geliştirici hesabı oluştur
3. Yeni uygulama oluştur
4. Access Key'ini kopyala
5. `news_aggregator.py` dosyasını düzenle:
   ```python
   UNSPLASH_ACCESS_KEY = "senin_key_in_buraya"
   ```

**Not**: API key olmadan demo modda çalışır (sınırlı) veya placeholder resimleri kullanır.

## 🚀 Kullanım

### Tek Seferlik Çalıştırma (Manuel)

```bash
cd C:\dev\newsportal\scripts
python news_aggregator.py --once
```

veya batch dosyasını kullan:

```bash
run_aggregation_now.bat
```

### Otomatik Zamanlama (Her Gün Sabah 5:00)

```bash
cd C:\dev\newsportal\scripts
python news_aggregator.py --schedule
```

veya batch dosyasını kullan:

```bash
run_daily_aggregation.bat
```

Bu komut:
- Hemen bir kez çalışır
- Sonra her gün sabah 5:00'te (Türkiye saati) otomatik çalışır
- Ctrl+C ile durdurana kadar çalışmaya devam eder

### Windows Servisi Olarak Çalıştırma (Production)

#### **Yöntem 1: Windows Task Scheduler (Önerilen)**

1. **Task Scheduler**'ı aç
2. **Create Basic Task** → "Günlük Haber Toplama"
3. **Trigger**: Daily (Her gün)
4. **Start time**: 05:00 (Sabah 5)
5. **Action**: Start a program
   - Program: `C:\dev\newsportal\scripts\run_aggregation_now.bat`
   - Start in: `C:\dev\newsportal\scripts`
6. **Finish** → Task'ı kaydet

#### **Yöntem 2: NSSM (Windows Service)**

```bash
# NSSM kur (Chocolatey ile)
choco install nssm

# Servisi oluştur
nssm install NewsAggregator python.exe
nssm set NewsAggregator AppDirectory C:\dev\newsportal\scripts
nssm set NewsAggregator AppParameters "C:\dev\newsportal\scripts\news_aggregator.py --schedule"

# Servisi başlat
nssm start NewsAggregator

# Servisi kontrol et
nssm status NewsAggregator

# Servisi durdur
nssm stop NewsAggregator
```

## ⚙️ Yapılandırma

### Kaynak Ekleme/Çıkarma

`news_aggregator.py` dosyasındaki `NEWS_SOURCES` sözlüğünü düzenle:

```python
# Bir kaynağı devre dışı bırak
'reddit_science': {
    'enabled': False,  # Bu kaynak atlanacak
    ...
}

# Yeni Reddit kaynağı ekle
'reddit_machinelearning': {
    'enabled': True,
    'url': 'https://www.reddit.com/r/MachineLearning/top.json',
    'params': {'t': 'week', 'limit': 15},
    'category': 'Yapay Zeka',
    'parser': 'reddit'
}

# Yeni RSS kaynağı ekle
'teknoblog': {
    'enabled': True,
    'url': 'https://www.teknoblog.com/feed/',
    'category': 'Teknoloji',
    'parser': 'rss'
}
```

### Zamanlama Değiştirme

`run_scheduler()` fonksiyonunda zamanlama satırını değiştir:

```python
# Her gün sabah 5:00 (varsayılan)
schedule.every().day.at("05:00").do(aggregate_news)

# Günde 2 kez (sabah 6, akşam 18)
schedule.every().day.at("06:00").do(aggregate_news)
schedule.every().day.at("18:00").do(aggregate_news)

# Her 6 saatte bir
schedule.every(6).hours.do(aggregate_news)

# Sadece hafta içi sabah 7:00
schedule.every().monday.at("07:00").do(aggregate_news)
schedule.every().tuesday.at("07:00").do(aggregate_news)
# ... diğer günler
```

### Haber Limitlerini Ayarlama

Her kaynağın `params` veya `limit` değerini değiştir:

```python
'reddit_technology': {
    'params': {'t': 'week', 'limit': 50},  # 20'den 50'ye çıkar
    ...
}
```

## 📊 İzleme ve Kontrol

### Logları Görüntüleme

```bash
# Canlı log takibi (tail)
Get-Content news_aggregator.log -Wait -Tail 50

# Son 100 satırı göster
Get-Content news_aggregator.log -Tail 100

# Sadece hataları göster
Get-Content news_aggregator.log | Select-String "ERROR"

# Oluşturulan haber sayısını say
Get-Content news_aggregator.log | Select-String "Created article" | Measure-Object
```

### Servis Durumunu Kontrol Etme

```bash
# NSSM servisi
nssm status NewsAggregator

# Task Scheduler görevi
Get-ScheduledTask -TaskName "Günlük Haber Toplama"

# Backend API'yi kontrol et
curl http://localhost:5000/health

# Docker konteynerlerini kontrol et
docker-compose ps
```

### Veritabanını Kontrol Etme

```bash
# Mongo Express UI
http://localhost:8081

# Haber sayısını kontrol et (PowerShell)
docker exec -it newsportal-mongodb mongosh -u admin -p password123 --authenticationDatabase admin NewsDb --eval "db.News.countDocuments({})"

# Son eklenen 10 haberi göster
docker exec -it newsportal-mongodb mongosh -u admin -p password123 --authenticationDatabase admin NewsDb --eval "db.News.find().sort({CreatedAt: -1}).limit(10).pretty()"
```

## 🔧 Sorun Giderme

### 1. Resimler Yüklenmiyor

**Neden**: Unsplash API key yok veya kota dolmuş

**Çözüm**:
```python
# API key'i kontrol et
UNSPLASH_ACCESS_KEY = "senin_key_in_buraya"  # Boş veya "YOUR_UNSPLASH_KEY" olmamalı

# Kota kontrolü (ücretsiz: 50 istek/saat)
# Daha fazla için: https://unsplash.com/developers
```

### 2. Aynı Haber Tekrar Ekleniyor

**Neden**: Slug üretimi veya API kontrolü çalışmıyor

**Çözüm**:
```bash
# Logları kontrol et
Get-Content news_aggregator.log | Select-String "already exists"

# Manuel test
python -c "from news_aggregator import article_exists, get_auth_token; token = get_auth_token(); print(article_exists('test-slug', token))"
```

### 3. Çeviri Başarısız Oluyor

**Neden**: Google Translate API rate limit

**Çözüm**:
```python
# Bekleme sürelerini artır (news_aggregator.py içinde)
time.sleep(1.0)  # 0.5'ten 1.0'a çıkar
```

### 4. Backend Bağlantı Hatası

**Neden**: Docker konteynerleri çalışmıyor

**Çözüm**:
```bash
# Konteyner durumunu kontrol et
docker-compose ps

# Logları kontrol et
docker-compose logs newsapi

# Yeniden başlat
docker-compose restart newsapi

# Sağlık kontrolü
curl http://localhost:5000/health
```

### 5. RSS Feed Parse Hatası

**Neden**: Beklenmeyen XML formatı

**Çözüm**:
```python
# O kaynağı geçici olarak devre dışı bırak
'webrazzi': {
    'enabled': False,  # Sorun giderilene kadar kapat
    ...
}
```

## 📈 Performans

### Beklenen Çalışma Süresi

- **Reddit (9 kaynak)**: ~90 haber → ~3-4 dakika
- **GitHub Trending**: ~15 repo → ~30-45 saniye
- **HackerNews**: ~20 haber → ~1-2 dakika
- **RSS (2 kaynak)**: ~30 haber → ~1-2 dakika

**Toplam**: ~150-170 haber → **~7-10 dakika**

### Rate Limiting

Script otomatik olarak beklemeler ekler:
- Çeviri arası: 0.5 saniye
- Haber oluşturma arası: 1 saniye
- HackerNews story arası: 0.2 saniye

### Optimizasyon İpuçları

```python
# Eşzamanlı çalışma için (gelişmiş)
# from concurrent.futures import ThreadPoolExecutor

# Çeviriyi atla (zaten Türkçe kaynaklar için)
# is_turkish kontrolü otomatik yapılıyor

# Daha az kaynak kullan (gerekirse)
# 'enabled': False yap
```

## 📋 Örnek Çıktı

```
============================================================
🚀 Starting News Aggregation
============================================================
🔐 Authenticating...
✓ Authenticated successfully

📡 Processing: reddit_technology
------------------------------------------------------------
📰 Fetching from reddit_technology...
✓ Fetched 20 posts from reddit_technology

[1/20] OpenAI releases GPT-5 with breakthrough capabilities...
  🌐 Translating: OpenAI releases GPT-5...
  🔍 Searching for image...
  📷 Found Unsplash image for: OpenAI GPT-5
  🌐 Translating content...
  ✓ Created article (ID: 68ff29764868af71e7d4321c)

[2/20] Yeni yapay zeka yasası Türkiye'de onaylandı...
  ⏭️  Already Turkish, skipping translation
  ✓ Created article (ID: 68ff29764868af71e7d4321d)

...

✓ reddit_technology: Created 18/20 articles

📡 Processing: github_trending_daily
------------------------------------------------------------
📰 Fetching from github_trending_daily...
✓ Fetched 15 posts from github_trending_daily

[1/15] microsoft/autogen - Python projesi (45,234 yıldız)
  🌐 Translating: microsoft/autogen...
  🔍 Searching for image...
  ✓ Created article (ID: 68ff29764868af71e7d4321e)

...

============================================================
✓ Aggregation Complete!
  Total Fetched: 165
  Total Created: 142
  Duplicates Skipped: 23
  Runtime: 8m 34s
============================================================
```

## 🎯 Kategoriler

Script tarafından oluşturulan haber kategorileri:

| Kategori | Kaynaklar | Haber/Hafta |
|----------|-----------|-------------|
| **Teknoloji** | r/technology, HackerNews, ShiftDelete | ~50 |
| **Programlama** | r/programming | ~15 |
| **Yapay Zeka** | r/artificial, r/LocalLLaMA | ~30 |
| **Robotik** | r/robotics | ~15 |
| **Yazılım** | r/github, GitHub Trending | ~25 |
| **Mobil** | r/Android, r/iphone | ~20 |
| **Türkiye** | r/Turkey, Webrazzi | ~20 |

**Toplam**: ~175 haber/hafta (tekrarlar çıkarıldıktan sonra ~140-150)

## 🚀 Gelecek Geliştirmeler

- [ ] Resimleri indir ve MinIO'ya yükle (harici URL bağımlılığını kaldır)
- [ ] Twitter/X trending topics ekle
- [ ] Dev.to, Medium, TechCrunch ekle
- [ ] AI ile içerik özetleme
- [ ] Kategori otomatik sınıflandırma (ML)
- [ ] E-posta bildirimleri
- [ ] Web dashboard (monitoring UI)
- [ ] Eski haberleri otomatik temizleme
- [ ] Çoklu dil desteği (sadece Türkçe değil)
- [ ] Sentiment analizi
- [ ] Haber önceliklendirme (AI tabanlı)

## 📝 Lisans

NewsPortal projesinin bir parçasıdır. Aynı lisans geçerlidir.
