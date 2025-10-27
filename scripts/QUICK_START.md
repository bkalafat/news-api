# Gelişmiş Otomatik Haber Sistemi - Hızlı Başlangıç

## 🎯 Ne Değişti?

Artık **15+ farklı kaynaktan** otomatik haber toplanıyor:

### 📰 Haber Kaynakları

| Kaynak | Konu | Adet/Hafta |
|--------|------|-----------|
| **Reddit r/technology** | Genel teknoloji | 20 |
| **Reddit r/programming** | Programlama | 15 |
| **Reddit r/artificial** | Yapay zeka | 15 |
| **Reddit r/LocalLLaMA** | Local LLM'ler | 15 |
| **Reddit r/robotics** | Robotik | 15 |
| **Reddit r/github** | GitHub & Copilot | 10 |
| **Reddit r/Android** | Android telefonlar | 10 |
| **Reddit r/iphone** | iPhone | 10 |
| **Reddit r/Turkey** | Türkiye teknoloji | 10 |
| **GitHub Trending** | Trend repository'ler | 15 |
| **HackerNews** | Top teknoloji haberleri | 20 |
| **Webrazzi** | Türk tech haberleri | ~10 |
| **ShiftDelete.Net** | Türk tech haberleri | ~10 |

**Toplam**: ~165 haber/hafta → Tekrarlar çıkarıldıktan sonra ~140-150 benzersiz haber

## ⏰ Otomatik Çalışma

- **Zamanlama**: Her sabah saat **5:00** (Türkiye saati)
- **Tekrar Önleme**: Slug kontrolü ile aynı haber 2 kez eklenmez
- **Akıllı Çeviri**: Türkçe haberler çevrilmez, sadece İngilizce olanlar
- **Akıllı Resim**: Unsplash'tan başlığa uygun resim bulur

## 🚀 Hızlı Kullanım

### 1. Şimdi Çalıştır (Tüm Kaynaklar)

```bash
cd C:\dev\newsportal\scripts
run_aggregation_now.bat
```

Süre: ~7-10 dakika, ~140-150 haber oluşturur

### 2. Otomatik Zamanlayıcıyı Başlat

```bash
cd C:\dev\newsportal\scripts
run_weekly_aggregation.bat
```

Her gün sabah 5:00'te otomatik çalışır (Türkiye saati)

### 3. Test Et (Hızlı)

```bash
cd C:\dev\newsportal\scripts
test_aggregator.bat
```

Sadece 3 kaynak, toplam 13 haber (test için hızlı)

## 📋 Örnekler

### Hangi Konular Var?

✅ **GitHub Copilot** → r/github kaynağından son hafta top haberleri
✅ **Robotik** → r/robotics kaynağından son hafta top haberleri
✅ **Local LLM'ler** → r/LocalLLaMA kaynağından son hafta top haberleri
✅ **GitHub Trending** → Son 7 günde en çok yıldız alan repolar
✅ **Cep Telefonları** → r/Android + r/iphone'dan son hafta top haberleri
✅ **Türkiye Teknoloji** → r/Turkey + Webrazzi + ShiftDelete'den haberler
✅ **Sosyal Medya Trendleri** → (Şu an Reddit, gelecekte Twitter/X eklenecek)

### Örnek Çıktı

```
============================================================
🚀 Starting News Aggregation
============================================================
🔐 Authenticating...
✓ Authenticated successfully

📡 Processing: reddit_localllama
------------------------------------------------------------
📰 Fetching from reddit_localllama...
✓ Fetched 15 posts from reddit_localllama

[1/15] Running Llama 3 70B on a single GPU...
  🌐 Translating: Running Llama 3 70B on a single GPU...
  🔍 Searching for image...
  📷 Found Unsplash image for: Llama GPU
  ✓ Created article (ID: 68ff29764868af71e7d4321c)

[2/15] Ollama güncellemesi ile yeni özellikler...
  ⏭️  Already Turkish, skipping translation
  ✓ Created article (ID: 68ff29764868af71e7d4321d)

...

✓ reddit_localllama: Created 14/15 articles

📡 Processing: github_trending_daily
------------------------------------------------------------
📰 Fetching from github_trending_daily...
✓ Fetched 15 repos from github_trending_daily

[1/15] microsoft/autogen - Python projesi (45,234 yıldız)
  ✓ Created article (ID: 68ff29764868af71e7d4321e)

...

============================================================
✓ Aggregation Complete!
  Total Fetched: 165
  Total Created: 142
  Duplicates Skipped: 23
============================================================
```

## ⚙️ Yapılandırma

### Bir Kaynağı Kapat

`news_aggregator.py` dosyasını aç ve `enabled: False` yap:

```python
'reddit_science': {
    'enabled': False,  # Bu kaynak artık çalışmaz
    ...
}
```

### Yeni Kaynak Ekle

```python
# Reddit kaynağı ekle
'reddit_yenisubreddit': {
    'enabled': True,
    'url': 'https://www.reddit.com/r/yenisubreddit/top.json',
    'params': {'t': 'week', 'limit': 15},
    'category': 'YeniKategori',
    'parser': 'reddit'
}

# RSS feed ekle
'yenisayt': {
    'enabled': True,
    'url': 'https://yenisayt.com/feed/',
    'category': 'Teknoloji',
    'parser': 'rss'
}
```

### Zamanlamayı Değiştir

`news_aggregator.py` içinde `run_scheduler()` fonksiyonunu düzenle:

```python
# Her gün sabah 5:00 (varsayılan)
schedule.every().day.at("05:00").do(aggregate_news)

# Günde 3 kez (sabah 6, öğlen 12, akşam 18)
schedule.every().day.at("06:00").do(aggregate_news)
schedule.every().day.at("12:00").do(aggregate_news)
schedule.every().day.at("18:00").do(aggregate_news)

# Her 4 saatte bir
schedule.every(4).hours.do(aggregate_news)
```

## 🔍 İzleme

### Logları Görüntüle

```bash
# Canlı takip
Get-Content news_aggregator.log -Wait -Tail 50

# Son 100 satır
Get-Content news_aggregator.log -Tail 100

# Kaç haber oluşturuldu?
Get-Content news_aggregator.log | Select-String "Created article" | Measure-Object
```

### Veritabanını Kontrol Et

```bash
# Toplam haber sayısı
docker exec -it newsportal-mongodb mongosh -u admin -p password123 --authenticationDatabase admin NewsDb --eval "db.News.countDocuments({})"

# Son 10 haber
docker exec -it newsportal-mongodb mongosh -u admin -p password123 --authenticationDatabase admin NewsDb --eval "db.News.find().sort({CreatedAt: -1}).limit(10).pretty()"

# Kategorilere göre dağılım
docker exec -it newsportal-mongodb mongosh -u admin -p password123 --authenticationDatabase admin NewsDb --eval "db.News.aggregate([{'\$group': {_id: '\$Category', count: {'\$sum': 1}}}])"
```

## 🎯 Windows Task Scheduler ile Otomatik Çalıştırma

1. **Task Scheduler**'ı aç (Win+R → `taskschd.msc`)
2. **Create Basic Task** → "Günlük Haber Toplama"
3. **Trigger**: Daily
4. **Start time**: 05:00
5. **Action**: Start a program
   - Program: `C:\dev\newsportal\scripts\run_aggregation_now.bat`
   - Start in: `C:\dev\newsportal\scripts`
6. **Finish**

Artık her sabah 5'te otomatik çalışacak!

## 🆘 Sorunlar?

### Backend bağlantı hatası
```bash
docker-compose ps  # Konteynerler çalışıyor mu?
docker-compose restart newsapi
```

### Aynı haber tekrar ekleniyor
```bash
# Slug kontrolünü test et
python -c "from news_aggregator import article_exists, get_auth_token; print(article_exists('test-slug', get_auth_token()))"
```

### Resimler yüklenmiyor
```python
# news_aggregator.py içinde Unsplash key'ini ekle
UNSPLASH_ACCESS_KEY = "senin_key_buraya"
```

### RSS feed hatası
```python
# O kaynağı geçici kapat
'webrazzi': {
    'enabled': False,
    ...
}
```

## 📚 Detaylı Dokümantasyon

- **Tam Rehber**: `ADVANCED_AGGREGATOR_README.md`
- **API Dokümantasyonu**: `NEWS_AGGREGATOR_README.md`
- **Eski Script**: `fetch_reddit_news.py` (legacy)

## ✨ Özellikler

✅ 15+ farklı kaynak
✅ Otomatik Türkçe çeviri (akıllı dil algılama)
✅ Unsplash resim araması
✅ Tekrar önleme (slug tabanlı)
✅ Türkiye saat dilimi desteği
✅ Günlük otomatik çalışma
✅ Detaylı loglama
✅ Kolay yapılandırma
✅ Kategorilere göre düzenleme

## 🚀 Sonraki Adımlar

1. Test et: `test_aggregator.bat`
2. Tüm kaynakları çalıştır: `run_aggregation_now.bat`
3. Otomatik zamanlayıcıyı başlat: Task Scheduler ile kur
4. İzle: `Get-Content news_aggregator.log -Wait -Tail 50`
5. Frontend'de gör: `http://localhost:3000`

Hazır! 🎉
