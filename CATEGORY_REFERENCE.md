# NewsPortal - Kategori Referansı

Backend API tarafından desteklenen kategoriler ve haber aggregator eşleşmeleri.

## 📋 Desteklenen Kategoriler

Backend (NewsPortal API) aşağıdaki İngilizce kategorileri kullanır:

| Kategori | Açıklama | Haber Kaynakları |
|----------|----------|------------------|
| **technology** | Teknoloji haberleri | Reddit (technology, programming, AI, LocalLLM, github), HackerNews, ShiftDelete, GitHub Trending |
| **world** | Dünya haberleri | Reddit (Turkey, world news) |
| **business** | İş dünyası haberleri | Webrazzi (Türk startup ekosistemi) |
| **science** | Bilim haberleri | Reddit (robotics, science) |
| **health** | Sağlık haberleri | - |
| **entertainment** | Eğlence haberleri | - |
| **sports** | Spor haberleri | - |

## 🔄 Haber Aggregator Kategori Eşleştirmeleri

`scripts/news_aggregator.py` dosyasındaki kaynak-kategori eşleştirmeleri:

### Technology (Teknoloji)
- Reddit: `/r/technology` ⭐ (haftalık top 20)
- Reddit: `/r/programming` (haftalık top 15)
- Reddit: `/r/artificial` (haftalık top 15)
- Reddit: `/r/LocalLLaMA` (haftalık top 15)
- Reddit: `/r/github` (haftalık top 10)
- Reddit: `/r/Android` (haftalık top 10)
- Reddit: `/r/iphone` (haftalık top 10)
- HackerNews: Top 20 stories
- ShiftDelete: RSS feed
- GitHub Trending: Son 7 gün içinde en çok yıldız alan repolar

### World (Dünya)
- Reddit: `/r/Turkey` (haftalık top 10)

### Business (İş Dünyası)
- Webrazzi: RSS feed (Türk tech startup haberleri)

### Science (Bilim)
- Reddit: `/r/robotics` (haftalık top 15)

## ⚙️ Kategori Değiştirme

Bir haberin kategorisini değiştirmek için:

### 1. Aggregator'da Kaynak Kategorisini Değiştir

`scripts/news_aggregator.py` dosyasında:

```python
NEWS_SOURCES = {
    'reddit_technology': {
        # ...
        'category': 'technology',  # Buradan değiştir
        # ...
    }
}
```

### 2. Backend'de Yeni Kategori Ekle

Eğer yeni bir kategori eklemek istiyorsanız:

1. Backend'de kategori validasyonu yok, doğrudan kullanabilirsiniz
2. Frontend'de kategoriye göre filtreleme varsa güncelleyin

## 📊 Kategori İstatistikleri

MongoDB Atlas'taki mevcut veri dağılımı (193 haber):

```
Mobil: ~20 haber
Programlama: ~25 haber
Robotik: ~15 haber
Teknoloji: ~80 haber
Türkiye: ~10 haber
Türkiye Teknoloji: ~25 haber
Yapay Zeka: ~18 haber
Yazılım: ~10 haber
```

**Yeni kategorilerle güncellenecek dağılım:**

```
technology: ~170 haber (Teknoloji, Programlama, Mobil, Yapay Zeka, Yazılım birleştirildi)
business: ~15 haber (Türkiye Teknoloji → Webrazzi)
science: ~15 haber (Robotik)
world: ~10 haber (Türkiye)
```

## 🔄 Eski Kategorileri Güncelleme

MongoDB Atlas'taki eski Türkçe kategorileri İngilizce'ye çevirmek için:

```javascript
// MongoDB shell veya Compass'te çalıştır

// Türkçe → İngilizce kategori dönüşümü
db.News.updateMany(
  { Category: { $in: ["Teknoloji", "Programlama", "Mobil", "Yapay Zeka", "Yazılım"] } },
  { $set: { Category: "technology" } }
);

db.News.updateMany(
  { Category: "Robotik" },
  { $set: { Category: "science" } }
);

db.News.updateMany(
  { Category: "Türkiye Teknoloji" },
  { $set: { Category: "business" } }
);

db.News.updateMany(
  { Category: "Türkiye" },
  { $set: { Category: "world" } }
);
```

## 🎯 En İyi Pratikler

1. **Tutarlılık**: Backend ve aggregator aynı kategori isimlerini kullanmalı
2. **İngilizce**: Kategoriler İngilizce olmalı (frontend'de Türkçe'ye çevrilebilir)
3. **Küçük harf**: Kategoriler küçük harf olmalı (`technology`, `business`)
4. **Tek kelime**: Mümkünse tek kelime kullanın
5. **Anlaşılır**: Kategori isimleri açık ve anlaşılır olmalı

## 🔧 Troubleshooting

### Problem: Haber yanlış kategoride görünüyor

**Çözüm 1**: Aggregator'daki kaynak kategorisini kontrol et
```python
# scripts/news_aggregator.py
'category': 'technology'  # Doğru kategoriyi kullandığından emin ol
```

**Çözüm 2**: MongoDB'deki mevcut haberleri güncelle
```javascript
db.News.updateOne(
  { Slug: "haber-slug" },
  { $set: { Category: "technology" } }
)
```

### Problem: Frontend kategorileri Türkçe göstermeli

**Çözüm**: Frontend'de kategori mapping ekle
```javascript
const categoryMap = {
  'technology': 'Teknoloji',
  'business': 'İş Dünyası',
  'science': 'Bilim',
  'world': 'Dünya',
  'health': 'Sağlık',
  'entertainment': 'Eğlence',
  'sports': 'Spor'
};
```

## 📝 Notlar

- Aggregator haftada 1 kez çalışıyor (her Pazartesi 05:00 Türkiye saati)
- Duplicate haberleri slug'a göre önlüyor
- Her kaynak için limit var (kredi tasarrufu için)
- Kategoriler büyük/küçük harfe duyarlı değil (backend lowercase'e çeviriyor)
