using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using NewsApi.Common;
using NewsApi.Domain.Entities;
using NewsApi.Infrastructure.Data;

namespace NewsApi.Infrastructure.Data;

public static class SeedNewsData
{
    public static async Task SeedAsync(MongoDbContext context)
    {
        var newsCollection = context.News;

        // Check if we already have data and clear it
        var existingCount = await newsCollection.CountDocumentsAsync(FilterDefinition<News>.Empty);
        if (existingCount > 0)
        {
            Console.WriteLine($"Database already contains {existingCount} news articles. Clearing old data...");
            await newsCollection.DeleteManyAsync(FilterDefinition<News>.Empty);
            Console.WriteLine("Old data cleared successfully!");
        }

        var now = DateTime.UtcNow;
        var newsList = new List<News>
        {
            // Technology News
            new News
            {
                Category = "technology",
                Type = "news",
                Caption = "Yapay Zeka Çağında Yeni Gelişmeler",
                Slug = SlugHelper.GenerateSlug("Yapay Zeka Çağında Yeni Gelişmeler"),
                Keywords = "yapay zeka, AI, GPT-5, OpenAI, teknoloji",
                SocialTags = "#YapayZeka #AI #GPT5 #Teknoloji",
                Summary =
                    "OpenAI'nin yeni GPT-5 modeli, yapay zeka dünyasında devrim yaratmaya hazırlanıyor. Model, daha gelişmiş anlama ve üretim yetenekleriyle dikkat çekiyor.",
                ImgPath = "https://ichef.bbci.co.uk/news/976/cpsprodpb/1234/production/_123456789_ai.jpg",
                ImgAlt = "Yapay Zeka Görseli",
                Content =
                    "OpenAI, yapay zeka alanındaki en son hamlesiyle GPT-5 modelini tanıttı. Yeni model, önceki versiyonlara göre %300 daha hızlı ve daha doğru sonuçlar üretiyor. Dil işleme, görüntü analizi ve çok modlu öğrenme yetenekleri ile GPT-5, endüstri standardını yeniden belirliyor. Araştırmacılar, modelin etik kullanım için gelişmiş güvenlik önlemleriyle donatıldığını vurguluyor.",
                Subjects = new[] { "Yapay Zeka", "Teknoloji", "İnovasyon" },
                Authors = new[] { "Ahmet Yılmaz" },
                ExpressDate = now.AddHours(-2),
                CreateDate = now.AddHours(-2),
                UpdateDate = now.AddHours(-2),
                Priority = 1,
                IsActive = true,
                Url = "https://www.bbc.com/turkce/haberler-teknoloji-67890123",
                ViewCount = 1250,
                IsSecondPageNews = false,
            },
            new News
            {
                Category = "technology",
                Type = "news",
                Caption = "Quantum Bilgisayarlar Gerçek Oluyor",
                Keywords = "quantum, bilgisayar, Google, Willow, teknoloji",
                SocialTags = "#Quantum #QuantumComputing #Google #Teknoloji",
                Summary =
                    "Google'ın yeni quantum işlemcisi, klasik bilgisayarların yıllarca sürdüreceği hesaplamaları dakikalar içinde tamamlıyor.",
                ImgPath = "https://ichef.bbci.co.uk/news/976/cpsprodpb/1235/production/_123456790_quantum.jpg",
                ImgAlt = "Quantum Bilgisayar",
                Content =
                    "Google, Willow adlı yeni quantum işlemcisini duyurdu. Bu işlemci, quantum supremacy'yi kanıtlayan önemli bir adım olarak değerlendiriliyor. 100 qubit'lik sistemle gerçekleştirilen testler, klasik süper bilgisayarların 10,000 yıl süreceği hesaplamaları 200 saniyede tamamladı. Quantum bilgisayarlar, ilaç geliştirme, kriptografi ve finansal modelleme alanlarında devrim yaratacak.",
                Subjects = new[] { "Quantum Computing", "Teknoloji", "Bilim" },
                Authors = new[] { "Zeynep Kaya" },
                ExpressDate = now.AddHours(-5),
                CreateDate = now.AddHours(-5),
                UpdateDate = now.AddHours(-5),
                Priority = 1,
                IsActive = true,
                Url = "https://www.bbc.com/turkce/haberler-teknoloji-67890124",
                ViewCount = 980,
                IsSecondPageNews = false,
            },
            new News
            {
                Category = "technology",
                Type = "news",
                Caption = "5G Teknolojisi Türkiye'de Hızla Yayılıyor",
                Keywords = "5G, Türkiye, Türk Telekom, Turkcell, mobil internet",
                SocialTags = "#5G #Türkiye #MobilInternet #Teknoloji",
                Summary =
                    "Türk Telekom ve Turkcell, 5G altyapısını 81 ile yaymayı hedefliyor. 2025 sonuna kadar tüm büyük şehirlerde 5G erişimi sağlanacak.",
                ImgPath = "https://ichef.bbci.co.uk/news/976/cpsprodpb/1236/production/_123456791_5g.jpg",
                ImgAlt = "5G Baz İstasyonu",
                Content =
                    "Türkiye'de 5G teknolojisi hızla yaygınlaşıyor. Operatörler, yeni nesil mobil internet altyapısını genişletmek için büyük yatırımlar yapıyor. 5G ile birlikte indirme hızları 20 Gbps'ye, gecikme süreleri ise 1 milisaniyenin altına inecek. Akıllı şehir uygulamaları, otonom araçlar ve IoT cihazları için kritik altyapı sağlanacak.",
                Subjects = new[] { "5G", "Telekomünikasyon", "Altyapı" },
                Authors = new[] { "Mehmet Demir" },
                ExpressDate = now.AddHours(-8),
                CreateDate = now.AddHours(-8),
                UpdateDate = now.AddHours(-8),
                Priority = 2,
                IsActive = true,
                Url = "https://www.bbc.com/turkce/haberler-teknoloji-67890125",
                ViewCount = 750,
                IsSecondPageNews = false,
            },
            new News
            {
                Category = "technology",
                Type = "news",
                Caption = "Elektrikli Araç Satışları Rekor Kırdı",
                Keywords = "elektrikli araç, Tesla, BYD, otomotiv, yeşil enerji",
                SocialTags = "#ElektrikliAraç #Tesla #SürdürülebilirUlaşım",
                Summary =
                    "2024 yılında dünya genelinde 14 milyon elektrikli araç satıldı. Tesla ve BYD pazarda lider konumda.",
                ImgPath = "https://ichef.bbci.co.uk/news/976/cpsprodpb/1237/production/_123456792_ev.jpg",
                ImgAlt = "Elektrikli Araç Şarj İstasyonu",
                Content =
                    "Elektrikli araç pazarı 2024'te rekor kırdı. Dünya genelinde 14 milyon EV satışı gerçekleşti, bu bir önceki yıla göre %35 artışı temsil ediyor. Tesla Model Y en çok satan model olurken, Çinli BYD üretim hacmi ile liderliği ele geçirdi. Batarya teknolojisindeki ilerlemeler, menzil kaygısını ortadan kaldırıyor ve şarj süreleri 15 dakikaya kadar indi.",
                Subjects = new[] { "Otomotiv", "Elektrikli Araçlar", "Sürdürülebilirlik" },
                Authors = new[] { "Can Aydın" },
                ExpressDate = now.AddHours(-12),
                CreateDate = now.AddHours(-12),
                UpdateDate = now.AddHours(-12),
                Priority = 2,
                IsActive = true,
                Url = "https://www.bbc.com/turkce/haberler-teknoloji-67890126",
                ViewCount = 620,
                IsSecondPageNews = false,
            },
            new News
            {
                Category = "technology",
                Type = "news",
                Caption = "Metaverse Yatırımları Artıyor",
                Keywords = "metaverse, sanal gerçeklik, VR, AR, Meta, Apple Vision Pro",
                SocialTags = "#Metaverse #VR #AR #AppleVisionPro",
                Summary =
                    "Meta, Apple ve Microsoft gibi teknoloji devleri, metaverse teknolojilerine milyarlarca dolar yatırım yapıyor.",
                ImgPath = "https://ichef.bbci.co.uk/news/976/cpsprodpb/1238/production/_123456793_metaverse.jpg",
                ImgAlt = "Sanal Gerçeklik Gözlüğü",
                Content =
                    "Metaverse teknolojileri, Apple Vision Pro'nun piyasaya sürülmesiyle yeni bir ivme kazandı. Şirketler, sanal ofisler, toplantılar ve etkinlikler için metaverse platformlarına yatırım yapıyor. Eğitim, sağlık ve perakende sektörleri de VR/AR teknolojilerini hızla benimsiyor. Analistler, metaverse pazarının 2030'da 800 milyar dolara ulaşacağını öngörüyor.",
                Subjects = new[] { "Metaverse", "Sanal Gerçeklik", "Teknoloji Trendleri" },
                Authors = new[] { "Elif Özkan" },
                ExpressDate = now.AddDays(-1),
                CreateDate = now.AddDays(-1),
                UpdateDate = now.AddDays(-1),
                Priority = 3,
                IsActive = true,
                Url = "https://www.bbc.com/turkce/haberler-teknoloji-67890127",
                ViewCount = 450,
                IsSecondPageNews = false,
            },
            // World News
            new News
            {
                Category = "world",
                Type = "news",
                Caption = "İklim Zirvesi'nde Tarihi Anlaşma",
                Keywords = "iklim değişikliği, COP29, Paris Anlaşması, sürdürülebilirlik",
                SocialTags = "#İklimDeğişikliği #COP29 #Sürdürülebilirlik",
                Summary =
                    "Dubai'deki COP29 zirvesinde 195 ülke, karbon emisyonlarını 2030'a kadar %50 azaltma taahhüdünde bulundu.",
                ImgPath = "https://ichef.bbci.co.uk/news/976/cpsprodpb/1239/production/_123456794_climate.jpg",
                ImgAlt = "İklim Zirvesi",
                Content =
                    "Dubai'de düzenlenen COP29 İklim Zirvesi tarihi bir anlaşmayla sonuçlandı. 195 ülke, karbon emisyonlarını 2030'a kadar %50 oranında azaltmayı taahhüt etti. Gelişmiş ülkeler, gelişmekte olan ülkelere yıllık 100 milyar dolar iklim finansmanı sağlayacak. Fosil yakıt kullanımından çıkış için net bir yol haritası belirlendi.",
                Subjects = new[] { "İklim Değişikliği", "Çevre", "Uluslararası İşbirliği" },
                Authors = new[] { "Ayşe Yılmaz", "John Smith" },
                ExpressDate = now.AddDays(-1).AddHours(-6),
                CreateDate = now.AddDays(-1).AddHours(-6),
                UpdateDate = now.AddDays(-1).AddHours(-6),
                Priority = 1,
                IsActive = true,
                Url = "https://www.bbc.com/turkce/haberler-dunya-67890128",
                ViewCount = 2100,
                IsSecondPageNews = false,
            },
            new News
            {
                Category = "world",
                Type = "news",
                Caption = "AB'den Göç Politikası Reformu",
                Keywords = "Avrupa Birliği, göç, mülteci, sınır güvenliği",
                SocialTags = "#AB #Göç #Mülteci #AvrupaBirliği",
                Summary = "Avrupa Birliği, ortak göç ve iltica politikası için kapsamlı bir reform paketini onayladı.",
                ImgPath = "https://ichef.bbci.co.uk/news/976/cpsprodpb/1240/production/_123456795_eu.jpg",
                ImgAlt = "Avrupa Birliği Bayrağı",
                Content =
                    "AB liderleri, uzun süren müzakerelerden sonra ortak göç politikası konusunda anlaşmaya vardı. Yeni sistem, sınır kontrollerini güçlendirirken, üye ülkeler arasında dayanışma mekanizması kuruyor. Mülteci başvurularının daha hızlı işlenmesi ve entegrasyon programlarının iyileştirilmesi öngörülüyor.",
                Subjects = new[] { "Avrupa Birliği", "Göç", "Uluslararası Politika" },
                Authors = new[] { "Maria Schmidt" },
                ExpressDate = now.AddDays(-2),
                CreateDate = now.AddDays(-2),
                UpdateDate = now.AddDays(-2),
                Priority = 2,
                IsActive = true,
                Url = "https://www.bbc.com/turkce/haberler-dunya-67890129",
                ViewCount = 1650,
                IsSecondPageNews = false,
            },
            new News
            {
                Category = "world",
                Type = "news",
                Caption = "Uzay Turizmi Yeni Döneme Giriyor",
                Keywords = "uzay turizmi, SpaceX, Blue Origin, Virgin Galactic",
                SocialTags = "#UzayTurizmi #SpaceX #UzayYolculuğu",
                Summary =
                    "SpaceX'in Starship aracı, ilk ticari uzay turizmini başlattı. 4 turistik yolcu, Ay'ın çevresinde 6 günlük yolculuğa çıktı.",
                ImgPath = "https://ichef.bbci.co.uk/news/976/cpsprodpb/1241/production/_123456796_space.jpg",
                ImgAlt = "Starship Uzay Aracı",
                Content =
                    "SpaceX'in Starship uzay aracı, tarihi ilk ticari ay turunu başarıyla tamamladı. 4 sivil yolcu, 6 gün boyunca Ay'ın etrafında dönme imkanı buldu. Bilet fiyatları 200 milyon dolar civarında. Blue Origin ve Virgin Galactic de sub-orbital uçuşlarını yoğunlaştırıyor. Uzay turizmi endüstrisi 2030'da 3 milyar dolara ulaşacak.",
                Subjects = new[] { "Uzay", "Turizm", "Teknoloji" },
                Authors = new[] { "Dr. Kemal Yıldız" },
                ExpressDate = now.AddDays(-3),
                CreateDate = now.AddDays(-3),
                UpdateDate = now.AddDays(-3),
                Priority = 2,
                IsActive = true,
                Url = "https://www.bbc.com/turkce/haberler-dunya-67890130",
                ViewCount = 3200,
                IsSecondPageNews = false,
            },
            // Business News
            new News
            {
                Category = "business",
                Type = "news",
                Caption = "Türkiye Ekonomisi İlk Çeyrekte %5.2 Büyüdü",
                Keywords = "Türkiye ekonomisi, büyüme, GSYH, ekonomi",
                SocialTags = "#TürkiyeEkonomisi #Büyüme #Ekonomi",
                Summary =
                    "Türkiye ekonomisi 2025 ilk çeyreğinde %5.2 büyüme kaydetti. İmalat sanayi ve ihracat büyümenin lokomotifi oldu.",
                ImgPath = "https://ichef.bbci.co.uk/news/976/cpsprodpb/1242/production/_123456797_turkey-economy.jpg",
                ImgAlt = "Türkiye Ekonomi Grafiği",
                Content =
                    "TÜİK verilerine göre, Türkiye ekonomisi 2025'in ilk çeyreğinde %5.2 büyüme kaydetti. İmalat sanayi %7.1, hizmetler sektörü %4.8 büyüdü. İhracat rekor seviyede arttı ve 70 milyar dolara ulaştı. Enflasyonla mücadele devam ederken, yatırımcı güveni artıyor. Ekonomistler yıl sonu büyüme tahminlerini %4.5'e yükseltti.",
                Subjects = new[] { "Ekonomi", "Türkiye", "Büyüme" },
                Authors = new[] { "Ekonomi Editörlüğü" },
                ExpressDate = now.AddDays(-1).AddHours(-3),
                CreateDate = now.AddDays(-1).AddHours(-3),
                UpdateDate = now.AddDays(-1).AddHours(-3),
                Priority = 1,
                IsActive = true,
                Url = "https://www.bbc.com/turkce/haberler-ekonomi-67890131",
                ViewCount = 2800,
                IsSecondPageNews = false,
            },
            new News
            {
                Category = "business",
                Type = "news",
                Caption = "Kripto Para Piyasasında Yükseliş",
                Keywords = "Bitcoin, Ethereum, kripto para, blockchain",
                SocialTags = "#Bitcoin #Kripto #Blockchain",
                Summary =
                    "Bitcoin 75,000 dolar seviyesini aştı. Ethereum ve diğer altcoinler de güçlü yükseliş gösteriyor.",
                ImgPath = "https://ichef.bbci.co.uk/news/976/cpsprodpb/1243/production/_123456798_crypto.jpg",
                ImgAlt = "Bitcoin Grafiği",
                Content =
                    "Kripto para piyasalarında güçlü yükseliş devam ediyor. Bitcoin, tarihinde ilk kez 75,000 doları aştı. Ethereum 4,500 dolar seviyesinde işlem görüyor. Kurumsal yatırımcıların artan ilgisi ve Bitcoin ETF'lerinin başarısı, rallinin arkasındaki temel faktörler. Analistler yıl sonuna kadar Bitcoin'in 100,000 dolara ulaşabileceğini öngörüyor.",
                Subjects = new[] { "Kripto Para", "Finans", "Yatırım" },
                Authors = new[] { "Finans Masası" },
                ExpressDate = now.AddDays(-2).AddHours(-8),
                CreateDate = now.AddDays(-2).AddHours(-8),
                UpdateDate = now.AddDays(-2).AddHours(-8),
                Priority = 2,
                IsActive = true,
                Url = "https://www.bbc.com/turkce/haberler-ekonomi-67890132",
                ViewCount = 1900,
                IsSecondPageNews = false,
            },
            new News
            {
                Category = "business",
                Type = "news",
                Caption = "Türk Startupları 2 Milyar Dolar Yatırım Aldı",
                Keywords = "startup, girişim, yatırım, teknoloji, Türkiye",
                SocialTags = "#Startup #Girişim #YatırımHaberleri",
                Summary =
                    "2024 yılında Türk startupları toplam 2 milyar dolar yatırım aldı. E-ticaret ve fintech sektörleri öne çıktı.",
                ImgPath = "https://ichef.bbci.co.uk/news/976/cpsprodpb/1244/production/_123456799_startup.jpg",
                ImgAlt = "Startup Ofisi",
                Content =
                    "Türkiye'nin girişim ekosistemi 2024'te rekor yatırım aldı. E-ticaret platformu Trendyol'un değerlemesi 20 milyar dolara ulaştı. Fintech girişimi Papara unicorn statüsü kazandı. Oyun, yapay zeka ve yeşil teknoloji alanlarındaki startuplar da önemli yatırımlar çekti. Türkiye, Avrupa'nın en hızlı büyüyen startup ekosistemlerinden biri haline geldi.",
                Subjects = new[] { "Startup", "Girişimcilik", "Yatırım" },
                Authors = new[] { "İş Dünyası Editörlüğü" },
                ExpressDate = now.AddDays(-3).AddHours(-5),
                CreateDate = now.AddDays(-3).AddHours(-5),
                UpdateDate = now.AddDays(-3).AddHours(-5),
                Priority = 2,
                IsActive = true,
                Url = "https://www.bbc.com/turkce/haberler-ekonomi-67890133",
                ViewCount = 1450,
                IsSecondPageNews = false,
            },
            // Science News
            new News
            {
                Category = "science",
                Type = "news",
                Caption = "Mars'ta Su Keşfi: Yaşam İzleri Bulundu",
                Keywords = "Mars, NASA, su, yaşam, uzay keşfi",
                SocialTags = "#Mars #NASA #UzayKeşfi #Bilim",
                Summary =
                    "NASA'nın Perseverance rover'ı, Mars'ın yüzeyinin altında sıvı su izleri ve olası mikrobiyal yaşam belirtileri tespit etti.",
                ImgPath = "https://ichef.bbci.co.uk/news/976/cpsprodpb/1245/production/_123456800_mars.jpg",
                ImgAlt = "Mars Yüzeyi",
                Content =
                    "NASA'nın Perseverance uzay aracı, Mars'ta devrim niteliğinde bir keşif yaptı. Jezero Krateri'nin altında sıvı su rezervleri tespit edildi. Daha da önemlisi, su örneklerinde organik moleküller ve olası mikrobiyal yaşam izleri bulundu. Bu keşif, Mars'ta yaşam olduğuna dair en güçlü kanıtları sunuyor. Bilim insanları, bu bulguların Mars misyonlarının yönünü değiştireceğini söylüyor.",
                Subjects = new[] { "Uzay Bilimi", "Astrobiyoloji", "Mars" },
                Authors = new[] { "Dr. Sarah Johnson", "Prof. Ali Toprak" },
                ExpressDate = now.AddDays(-1).AddHours(-10),
                CreateDate = now.AddDays(-1).AddHours(-10),
                UpdateDate = now.AddDays(-1).AddHours(-10),
                Priority = 1,
                IsActive = true,
                Url = "https://www.bbc.com/turkce/haberler-bilim-67890134",
                ViewCount = 4500,
                IsSecondPageNews = false,
            },
            new News
            {
                Category = "science",
                Type = "news",
                Caption = "Kanser Tedavisinde Çığır Açan Gelişme",
                Keywords = "kanser, tedavi, mRNA, aşı, tıp",
                SocialTags = "#Kanser #Tedavi #Bilim #Sağlık",
                Summary =
                    "mRNA teknolojisi kullanılarak geliştirilen kişiselleştirilmiş kanser aşıları, klinik deneylerde %90 başarı oranı gösterdi.",
                ImgPath = "https://ichef.bbci.co.uk/news/976/cpsprodpb/1246/production/_123456801_cancer.jpg",
                ImgAlt = "Kanser Hücreleri Mikroskopta",
                Content =
                    "BioNTech ve Moderna'nın ortaklaşa geliştirdiği mRNA bazlı kanser aşısı, Faz 3 denemelerinde çığır açan sonuçlar verdi. Hastanın kendi tümör hücrelerinden üretilen kişiselleştirilmiş aşı, %90 başarı oranı gösterdi. Melanom, akciğer ve pankreas kanseri hastalarında tümör küçülmesi gözlendi. FDA onayının 2026'da beklenmesi, milyonlarca hasta için umut ışığı.",
                Subjects = new[] { "Tıp", "Kanser", "Biyoteknoloji" },
                Authors = new[] { "Dr. Mehmet Öztürk" },
                ExpressDate = now.AddDays(-2).AddHours(-3),
                CreateDate = now.AddDays(-2).AddHours(-3),
                UpdateDate = now.AddDays(-2).AddHours(-3),
                Priority = 1,
                IsActive = true,
                Url = "https://www.bbc.com/turkce/haberler-bilim-67890135",
                ViewCount = 5200,
                IsSecondPageNews = false,
            },
            new News
            {
                Category = "science",
                Type = "news",
                Caption = "Biyoçeşitlilik Krizi: 1 Milyon Tür Tehlikede",
                Keywords = "biyoçeşitlilik, nesli tükenen türler, çevre, ekosistem",
                SocialTags = "#Biyoçeşitlilik #Çevre #DoğaKoruma",
                Summary =
                    "BM raporuna göre, 1 milyon hayvan ve bitki türü nesli tükenme tehlikesiyle karşı karşıya. İklim değişikliği ve habitat kaybı ana faktörler.",
                ImgPath = "https://ichef.bbci.co.uk/news/976/cpsprodpb/1247/production/_123456802_biodiversity.jpg",
                ImgAlt = "Tropikal Orman",
                Content =
                    "Birleşmiş Milletler'in Biyoçeşitlilik Raporu, doğa için alarm zili çalıyor. 1 milyon tür, önümüzdeki on yıllarda yok olma riski taşıyor. İklim değişikliği, habitat kaybı, aşırı avlanma ve kirlilik başlıca tehditler. Bilim insanları, acil eylem çağrısı yapıyor. Koruma alanlarının genişletilmesi, sürdürülebilir tarım ve fosil yakıt kullanımının azaltılması kritik önlemler.",
                Subjects = new[] { "Çevre Bilimi", "Ekoloji", "Koruma" },
                Authors = new[] { "Prof. Dr. Canan Yılmaz" },
                ExpressDate = now.AddDays(-4),
                CreateDate = now.AddDays(-4),
                UpdateDate = now.AddDays(-4),
                Priority = 2,
                IsActive = true,
                Url = "https://www.bbc.com/turkce/haberler-bilim-67890136",
                ViewCount = 1800,
                IsSecondPageNews = false,
            },
            // Health News
            new News
            {
                Category = "health",
                Type = "news",
                Caption = "DSÖ Yeni Beslenme Kılavuzu Yayınladı",
                Keywords = "DSÖ, beslenme, sağlık, diyet, WHO",
                SocialTags = "#Sağlık #Beslenme #DSÖ",
                Summary =
                    "Dünya Sağlık Örgütü, güncellenen beslenme kılavuzunda bitkisel protein kaynaklarını öneriyor ve işlenmiş et tüketiminin azaltılmasını tavsiye ediyor.",
                ImgPath = "https://ichef.bbci.co.uk/news/976/cpsprodpb/1248/production/_123456803_nutrition.jpg",
                ImgAlt = "Sağlıklı Yiyecekler",
                Content =
                    "DSÖ'nün yeni beslenme kılavuzu, bitkisel protein kaynaklarını (baklagiller, soya, fındık) öne çıkarıyor. İşlenmiş et ve kırmızı et tüketiminin haftada 2 porsiyon ile sınırlandırılması öneriliyor. Omega-3 açısından zengin balık, meyve ve sebze tüketiminin artırılması gerektiği vurgulanıyor. Yeni rehber, kalp hastalıkları, diyabet ve kanser riskini azaltmayı hedefliyor.",
                Subjects = new[] { "Sağlık", "Beslenme", "Önleyici Tıp" },
                Authors = new[] { "Sağlık Editörlüğü" },
                ExpressDate = now.AddDays(-2).AddHours(-12),
                CreateDate = now.AddDays(-2).AddHours(-12),
                UpdateDate = now.AddDays(-2).AddHours(-12),
                Priority = 2,
                IsActive = true,
                Url = "https://www.bbc.com/turkce/haberler-saglik-67890137",
                ViewCount = 2100,
                IsSecondPageNews = false,
            },
            new News
            {
                Category = "health",
                Type = "news",
                Caption = "Yapay Zeka Erken Teşhiste Doktorları Geçti",
                Keywords = "yapay zeka, erken teşhis, radyoloji, AI, sağlık teknolojisi",
                SocialTags = "#YapayZeka #Sağlık #Teşhis #AIteknoloji",
                Summary =
                    "Stanford Üniversitesi araştırması, AI sistemlerinin radyoloji görüntülerinden hastalık tespitinde %95 doğrulukla insan doktorları geçtiğini gösterdi.",
                ImgPath = "https://ichef.bbci.co.uk/news/976/cpsprodpb/1249/production/_123456804_ai-health.jpg",
                ImgAlt = "Tıbbi Görüntüleme ve Yapay Zeka",
                Content =
                    "Stanford Tıp Fakültesi'nden araştırmacılar, derin öğrenme algoritmaları kullanarak geliştirdikleri AI sisteminin, röntgen, MR ve CT görüntülerinden hastalık tespitinde %95 doğruluk oranına ulaştığını açıkladı. Sistem, akciğer kanseri, pnömoni ve kemik kırıklarını erken evrede tespit edebiliyor. Radyologların iş yükünü azaltırken, tanı sürecini hızlandırıyor.",
                Subjects = new[] { "Tıbbi Teknoloji", "Radyoloji", "Yapay Zeka" },
                Authors = new[] { "Dr. Emily Chen" },
                ExpressDate = now.AddDays(-3).AddHours(-8),
                CreateDate = now.AddDays(-3).AddHours(-8),
                UpdateDate = now.AddDays(-3).AddHours(-8),
                Priority = 2,
                IsActive = true,
                Url = "https://www.bbc.com/turkce/haberler-saglik-67890138",
                ViewCount = 1750,
                IsSecondPageNews = false,
            },
            // Entertainment News
            new News
            {
                Category = "entertainment",
                Type = "news",
                Caption = "2025 Oscar Ödülleri Sahiplerini Buldu",
                Keywords = "Oscar, akademi ödülleri, sinema, Hollywood",
                SocialTags = "#Oscar2025 #Oscarlar #Sinema",
                Summary =
                    "97. Akademi Ödülleri töreni Los Angeles'ta gerçekleştirildi. 'Oppenheimer' en iyi film ödülünü kazandı.",
                ImgPath = "https://ichef.bbci.co.uk/news/976/cpsprodpb/1250/production/_123456805_oscars.jpg",
                ImgAlt = "Oscar Heykeli",
                Content =
                    "97. Akademi Ödülleri töreni Dolby Theatre'da büyük bir coşkuyla gerçekleştirildi. Christopher Nolan'ın 'Oppenheimer' filmi, En İyi Film dahil 7 dalda Oscar kazandı. Cillian Murphy En İyi Erkek Oyuncu, Emma Stone En İyi Kadın Oyuncu ödülünün sahibi oldu. Törende yapay zekanın sinema endüstrisindeki geleceği konusu tartışıldı.",
                Subjects = new[] { "Sinema", "Oscar", "Ödüller" },
                Authors = new[] { "Magazin Masası" },
                ExpressDate = now.AddDays(-5),
                CreateDate = now.AddDays(-5),
                UpdateDate = now.AddDays(-5),
                Priority = 2,
                IsActive = true,
                Url = "https://www.bbc.com/turkce/haberler-eglence-67890139",
                ViewCount = 3500,
                IsSecondPageNews = false,
            },
            new News
            {
                Category = "entertainment",
                Type = "news",
                Caption = "Netflix'in Türk Dizisi Dünya Listelerinde Birinci",
                Keywords = "Netflix, Türk dizisi, streaming, dizi",
                SocialTags = "#Netflix #TürkDizisi #DizilerDünyası",
                Summary =
                    "'Gölge Oyunları' adlı Türk yapımı dizi, Netflix küresel izlenme listesinde birinci sıraya yükseldi.",
                ImgPath = "https://ichef.bbci.co.uk/news/976/cpsprodpb/1251/production/_123456806_netflix.jpg",
                ImgAlt = "Netflix Logosu",
                Content =
                    "Netflix'in Türk yapımı dizisi 'Gölge Oyunları', platformun küresel izlenme listesinde birinci sıraya yükseldi. 190'dan fazla ülkede yayınlanan dizi, ilk haftasında 50 milyon saatten fazla izlendi. Gerilim ve aksiyon dolu hikayesi ile uluslararası izleyiciyi büyüledi. Bu başarı, Türk dizilerinin global platformlardaki yükselişini gösteriyor.",
                Subjects = new[] { "Dizi", "Streaming", "Türk Yapımı" },
                Authors = new[] { "Kültür Sanat Editörlüğü" },
                ExpressDate = now.AddDays(-4).AddHours(-6),
                CreateDate = now.AddDays(-4).AddHours(-6),
                UpdateDate = now.AddDays(-4).AddHours(-6),
                Priority = 2,
                IsActive = true,
                Url = "https://www.bbc.com/turkce/haberler-eglence-67890140",
                ViewCount = 2900,
                IsSecondPageNews = false,
            },
            new News
            {
                Category = "entertainment",
                Type = "news",
                Caption = "Yapay Zeka ile Üretilen Müzik Grammy Aldı",
                Keywords = "Grammy, yapay zeka, müzik, AI müzik",
                SocialTags = "#Grammy #YapayZeka #Müzik",
                Summary =
                    "AI destekli müzik prodüksiyonu ile üretilen albüm, Grammy Ödüllerinde 'En İyi Elektronik Müzik Albümü' kategorisinde ödül kazandı.",
                ImgPath = "https://ichef.bbci.co.uk/news/976/cpsprodpb/1252/production/_123456807_grammy.jpg",
                ImgAlt = "Grammy Ödül Töreni",
                Content =
                    "Müzik endüstrisinde tarihi bir an yaşandı. Yapay zeka araçları kullanılarak üretilen 'Digital Dreams' albümü, Grammy Ödüllerinde 'En İyi Elektronik Müzik Albümü' kategorisinde ödül kazandı. Sanatçı ve yapay zeka arasındaki işbirliği, müziğin geleceği hakkında tartışmalar başlattı. Eleştirmenler yaratıcılığı sorgularken, destekçiler yeni bir sanat formunun doğduğunu söylüyor.",
                Subjects = new[] { "Müzik", "Yapay Zeka", "Grammy" },
                Authors = new[] { "Müzik Editörlüğü" },
                ExpressDate = now.AddDays(-6),
                CreateDate = now.AddDays(-6),
                UpdateDate = now.AddDays(-6),
                Priority = 3,
                IsActive = true,
                Url = "https://www.bbc.com/turkce/haberler-eglence-67890141",
                ViewCount = 1250,
                IsSecondPageNews = false,
            },
            // Sports News (Turkish Focus)
            new News
            {
                Category = "sports",
                Type = "news",
                Caption = "Trabzonspor Altyapı Müjdeli Haberler",
                Keywords = "Trabzonspor, altyapı, genç futbolcular, Süper Lig",
                SocialTags = "#Trabzonspor #Altyapı #GençYetenek",
                Summary =
                    "Trabzonspor'un genç yetenekleri, altyapı sisteminde göz dolduran performanslar sergiliyor. A takıma yükselme hazırlığındalar.",
                ImgPath = "https://ichef.bbci.co.uk/news/976/cpsprodpb/1253/production/_123456808_trabzonspor.jpg",
                ImgAlt = "Trabzonspor Altyapı Antrenmanı",
                Content =
                    "Trabzonspor'un altyapı sistemi, Türk futbolunun en başarılı örneklerinden biri olmaya devam ediyor. U19 takımı, Elit Lig'de liderliğini sürdürürken, U17 ve U15 takımları da kategorilerinde zirvede yer alıyor. Teknik direktör Abdullah Avcı, genç yetenekleri yakından takip ediyor ve A takıma entegrasyon planları yapıyor. Özellikle 17 yaşındaki orta saha oyuncusu Emre Öztürk ve 16 yaşındaki golcü Burak Yılmaz, dikkat çekiyor.",
                Subjects = new[] { "Futbol", "Trabzonspor", "Altyapı" },
                Authors = new[] { "Spor Editörlüğü" },
                ExpressDate = now.AddHours(-1),
                CreateDate = now.AddHours(-1),
                UpdateDate = now.AddHours(-1),
                Priority = 1,
                IsActive = true,
                Url = "https://www.bbc.com/turkce/spor/trabzonspor-altyapi-mujdeli-haberler",
                ViewCount = 850,
                IsSecondPageNews = false,
            },
            new News
            {
                Category = "sports",
                Type = "news",
                Caption = "TFF Önemli Karar: Liga Değişiklik Geliyor",
                Keywords = "TFF, Süper Lig, lig format değişikliği, Türk futbolu",
                SocialTags = "#TFF #SüperLig #TürkFutbolu",
                Summary =
                    "Türkiye Futbol Federasyonu, Süper Lig formatında önemli değişikliklere gidiyor. 2026-2027 sezonundan itibaren yeni sistem uygulanacak.",
                ImgPath = "https://ichef.bbci.co.uk/news/976/cpsprodpb/1254/production/_123456809_tff.jpg",
                ImgAlt = "TFF Logosu",
                Content =
                    "Türkiye Futbol Federasyonu Yönetim Kurulu, Süper Lig formatında radikal değişikliklere gidilmesine karar verdi. 2026-2027 sezonundan itibaren lig 20 takıma çıkarılacak. Playoff sisteminin getirilmesi, yabancı futbolcu kuralının revizyonu ve VAR sisteminin geliştirilmesi planlanıyor. Kulüpler ve futbol kamuoyu, değişiklikler hakkında görüşlerini bildiriyor.",
                Subjects = new[] { "Futbol", "TFF", "Lig Sistemi" },
                Authors = new[] { "Futbol Muhabirleri" },
                ExpressDate = now.AddHours(-3),
                CreateDate = now.AddHours(-3),
                UpdateDate = now.AddHours(-3),
                Priority = 1,
                IsActive = true,
                Url = "https://www.bbc.com/turkce/spor/tff-onemli-karar-liga-degisiklik",
                ViewCount = 1650,
                IsSecondPageNews = false,
            },
            new News
            {
                Category = "sports",
                Type = "news",
                Caption = "Galatasaray Avrupa'da Tarih Yazdı",
                Keywords = "Galatasaray, Şampiyonlar Ligi, Avrupa, futbol",
                SocialTags = "#Galatasaray #ŞampiyonlarLigi #Avrupa",
                Summary =
                    "Galatasaray, Şampiyonlar Ligi'nde çeyrek finale yükselen ilk Türk takımı oldu. Manchester City'yi eleyerek tarihi başarıya imza attı.",
                ImgPath = "https://ichef.bbci.co.uk/news/976/cpsprodpb/1255/production/_123456810_galatasaray.jpg",
                ImgAlt = "Galatasaray Taraftarları Kutlama",
                Content =
                    "Galatasaray, Manchester City'yi penaltılarla 5-4 yenerek Şampiyonlar Ligi çeyrek finaline yükseldi. Türk futbol tarihinde bir ilke imza atan sarı-kırmızılılar, Avrupa'nın en büyük kulüplerinden birini eleyerek dev bir başarı elde etti. Teknik direktör Okan Buruk'un taktik başarısı ve Mauro Icardi'nin golcü performansı, zaferin mimarları oldu. Türkiye, Avrupa futbolunda yeniden söz sahibi oluyor.",
                Subjects = new[] { "Futbol", "Şampiyonlar Ligi", "Galatasaray" },
                Authors = new[] { "Avrupa Futbolu Editörlüğü" },
                ExpressDate = now.AddHours(-6),
                CreateDate = now.AddHours(-6),
                UpdateDate = now.AddHours(-6),
                Priority = 1,
                IsActive = true,
                Url = "https://www.bbc.com/turkce/spor/galatasaray-avrupa-tarihi",
                ViewCount = 5800,
                IsSecondPageNews = false,
            },
            new News
            {
                Category = "sports",
                Type = "news",
                Caption = "Milli Takımda Yeni Dönem: Montella İmzayı Attı",
                Keywords = "milli takım, Vincenzo Montella, TFF, A Milli Takım",
                SocialTags = "#MilliTakım #Montella #TFF",
                Summary =
                    "İtalyan teknik adam Vincenzo Montella, Türkiye A Milli Futbol Takımı'nın başına geçti. 2026 Dünya Kupası elemanları hedefi var.",
                ImgPath = "https://ichef.bbci.co.uk/news/976/cpsprodpb/1256/production/_123456811_montella.jpg",
                ImgAlt = "Vincenzo Montella",
                Content =
                    "Türkiye Futbol Federasyonu, A Milli Takım için İtalyan teknik adam Vincenzo Montella ile anlaştı. Fiorentina ve Sevilla'da başarılı çalışmalar yapan Montella, 2026 Dünya Kupası elemeleri için hazırlıklara başlayacak. Genç oyuncuları takıma entegre etme planı yapan teknik adam, modern futbol anlayışını Türkiye'ye getirmeyi hedefliyor. İlk maçı Mart ayında Hollanda ile oynanacak.",
                Subjects = new[] { "Milli Takım", "Futbol", "Teknik Direktör" },
                Authors = new[] { "Milli Takım Muhabirleri" },
                ExpressDate = now.AddHours(-10),
                CreateDate = now.AddHours(-10),
                UpdateDate = now.AddHours(-10),
                Priority = 2,
                IsActive = true,
                Url = "https://www.bbc.com/turkce/spor/milli-takim-montella",
                ViewCount = 2400,
                IsSecondPageNews = false,
            },
        };

        // Generate slugs for all news articles before inserting
        foreach (var news in newsList)
        {
            if (string.IsNullOrEmpty(news.Slug))
            {
                news.Slug = SlugHelper.GenerateSlug(news.Caption);
            }
        }

        await newsCollection.InsertManyAsync(newsList);
        Console.WriteLine($"Successfully seeded {newsList.Count} news articles to the database!");
    }
}
