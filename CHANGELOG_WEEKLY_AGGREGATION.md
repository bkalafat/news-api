# NewsPortal Değişiklik Özeti

## ✅ Yapılan Değişiklikler

### 1. 🔄 GitHub Actions - Haftada 1 Kez Çalışma (Kredi Tasarrufu)

**Değişiklik:** `.github/workflows/news-aggregator.yml`

**Önceki:** Her gün saat 05:00'te çalışıyordu (günde 1 kez = ayda 30 kez)
```yaml
cron: '0 2 * * *'  # Her gün
```

**Yeni:** Her Pazartesi saat 05:00'te çalışıyor (haftada 1 kez = ayda 4 kez)
```yaml
cron: '0 2 * * 1'  # Her Pazartesi
```

**Tasarruf:**
- Önceki kullanım: ~30 çalışma/ay
- Yeni kullanım: ~4 çalışma/ay
- **%87 tasarruf!** 🎉

**Manuel çalıştırma:** Hala mevcut
```bash
# GitHub'da: Actions → Weekly News Aggregation → Run workflow
```

---

### 2. 📂 Kategori Uyumu - Backend ile Aggregator Eşleşmesi

**Değişiklik:** `scripts/news_aggregator.py`

**Önceki:** Türkçe kategoriler
```python
'category': 'Teknoloji'
'category': 'Programlama'
'category': 'Yapay Zeka'
'category': 'Mobil'
'category': 'Yazılım'
'category': 'Robotik'
'category': 'Türkiye'
'category': 'Türkiye Teknoloji'
```

**Yeni:** Backend ile uyumlu İngilizce kategoriler
```python
'category': 'technology'    # Teknoloji, Programlama, AI, Mobil, Yazılım
'category': 'science'       # Robotik
'category': 'world'         # Türkiye
'category': 'business'      # Türkiye Teknoloji
```

**Kategori Eşleştirmeleri:**

| Kaynak | Eski Kategori | Yeni Kategori | Sebep |
|--------|---------------|---------------|-------|
| Reddit (technology, programming, AI, LocalLLM, github, Android, iPhone) | Teknoloji, Programlama, Yapay Zeka, Mobil, Yazılım | **technology** | Backend'de genel teknoloji kategorisi |
| Reddit (robotics) | Robotik | **science** | Bilim ve robotik |
| Reddit (Turkey) | Türkiye | **world** | Dünya haberleri |
| Webrazzi | Türkiye Teknoloji | **business** | Türk startup ekosistemi |
| GitHub Trending | Yazılım | **technology** | Teknoloji haberleri |
| HackerNews | Teknoloji | **technology** | Teknoloji haberleri |
| ShiftDelete | Teknoloji | **technology** | Teknoloji haberleri |

---

### 3. 📊 MongoDB Kategori Güncelleme

**Oluşturulan:** `scripts/migrate_categories.py`

Mevcut 193 haberin kategorilerini otomatik olarak günceller:

```python
# Örnekler:
'Teknoloji' → 'technology'
'Programlama' → 'technology'
'Yapay Zeka' → 'technology'
'Mobil' → 'technology'
'Yazılım' → 'technology'
'Robotik' → 'science'
'Türkiye Teknoloji' → 'business'
'Türkiye' → 'world'
```

**Çalıştırma:**
```powershell
cd C:\dev\newsportal\scripts
python migrate_categories.py
```

---

### 4. 📚 Dokümantasyon

**Oluşturulan:** `CATEGORY_REFERENCE.md`

İçerik:
- ✅ Desteklenen tüm kategoriler
- ✅ Kategori-kaynak eşleştirmeleri
- ✅ Kategori güncelleme komutları
- ✅ Troubleshooting
- ✅ En iyi pratikler

---

## 🎯 Sonuç

### Kredi Tasarrufu
- **Önceki:** Günlük çalışma (30/ay)
- **Yeni:** Haftalık çalışma (4/ay)
- **Tasarruf:** %87 ⭐

### Kategori Tutarlılığı
- **Önceki:** 8 farklı Türkçe kategori (Backend'le uyumsuz)
- **Yeni:** 4 İngilizce kategori (Backend ile %100 uyumlu)
- **Avantaj:** Frontend filtreleme kolaylaşacak

### Veri Kalitesi
- **193 mevcut haber** kategorileri güncellenecek
- **Yeni haberler** doğru kategorilerle eklenecek
- **Backend** daha tutarlı veri sunacak

---

## 📝 Yapılacaklar (Sırayla)

### 1. Kategori Migrasyonunu Çalıştır
```powershell
cd C:\dev\newsportal\scripts
python migrate_categories.py
```

### 2. GitHub'a Push Et
```powershell
cd C:\dev\newsportal
git add .
git commit -m "feat: weekly news aggregation + category alignment"
git push origin master
```

### 3. GitHub Secrets Kontrol Et
GitHub'da şu secrets'ların olduğundan emin ol:
- `API_BASE_URL`: https://newsportal.onrender.com/api (veya Railway/Azure URL'i)
- `ADMIN_USERNAME`: admin
- `ADMIN_PASSWORD`: admin123 (veya güvenli şifren)
- `UNSPLASH_ACCESS_KEY`: ATK1ZDXPJKfdlYpm10AY1QNTvjzN4GymqQdH3JRP3qdU

### 4. Manuel Test
GitHub'da Actions sekmesine git:
- "Weekly News Aggregation" workflow'u bul
- "Run workflow" butonuna tıkla
- Test modunda çalıştır

### 5. Backend Deploy Et
Render/Railway/Azure'da backend'in çalıştığından emin ol:
```powershell
curl https://YOUR-BACKEND-URL/api/NewsArticle
```

---

## 🔍 Kontrol Listesi

- [x] GitHub Actions haftalığa güncellendi
- [x] News aggregator kategorileri güncellendi
- [x] MongoDB migration scripti oluşturuldu
- [x] Kategori dokümantasyonu oluşturuldu
- [ ] MongoDB kategorileri güncellendi (çalıştır: `python migrate_categories.py`)
- [ ] GitHub'a push edildi
- [ ] GitHub secrets kontrol edildi
- [ ] Manuel test yapıldı
- [ ] Backend deploy edildi

---

## 💡 Ekstra İpuçları

### Manuel Haber Ekleme (Test için)
```powershell
cd C:\dev\newsportal\scripts
python news_aggregator.py --once
```

### Kategori İstatistiklerini Görüntüle
```powershell
python -c "from pymongo import MongoClient; client = MongoClient('mongodb+srv://bkalafat:dbuserpassword123@cluster0.xwbfl1o.mongodb.net/'); db = client['NewsDb']; pipeline = [{'$group': {'_id': '$Category', 'count': {'$sum': 1}}}, {'$sort': {'count': -1}}]; [print(f'{c[\"_id\"]}: {c[\"count\"]}') for c in db['News'].aggregate(pipeline)]"
```

### Backend Kategorilere Göre Filtreleme
```powershell
# Technology haberleri
curl "https://YOUR-BACKEND-URL/api/NewsArticle?category=technology"

# Business haberleri
curl "https://YOUR-BACKEND-URL/api/NewsArticle?category=business"
```

---

**Tüm değişiklikler tamamlandı!** 🎉

Sıradaki adım: MongoDB kategori migrasyonunu çalıştır.
