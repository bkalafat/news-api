using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using NewsApi.Common;
using NewsApi.Domain.Entities;

namespace NewsApi.Infrastructure.Data;

internal static class SeedNewsData
{
    public static async Task SeedAsync(MongoDbContext context)
    {
        var newsCollection = context.News;

        // Check if we already have data and clear it
        var existingCount = await newsCollection
            .CountDocumentsAsync(FilterDefinition<NewsArticle>.Empty)
            .ConfigureAwait(false);
        if (existingCount > 0)
        {
            Console.WriteLine($"Database already contains {existingCount} news articles. Clearing old data...");
            await newsCollection.DeleteManyAsync(FilterDefinition<NewsArticle>.Empty).ConfigureAwait(false);
            Console.WriteLine("Old data cleared successfully!");
        }

        var now = DateTime.UtcNow;
        var newsArticles = new List<NewsArticle>
        {
            // GitHub News - Real from Reddit (Top rated)
            new NewsArticle
            {
                Category = "github",
                Type = "news",
                Caption = "GitHub Enterprise Cloud Çift Ücretlendirme Sorunu",
                Slug = SlugHelper.GenerateSlug("GitHub Enterprise Cloud Çift Ücretlendirme Sorunu"),
                Keywords = "github, enterprise, billing, support, cloud",
                SocialTags = "#GitHub #Enterprise #Billing",
                Summary = "Bir geliştirici, GitHub Enterprise Cloud hesabında $168 yerine $84 faturalaşma sorunu yaşıyor ve 3 haftadır destekten yanıt alamıyor.",
                ImgPath = "https://images.unsplash.com/photo-1618044733300-9472054094ee?w=1200&q=80",
                ImgAlt = "GitHub Enterprise Cloud",
                ImageUrl = "https://images.unsplash.com/photo-1618044733300-9472054094ee?w=1200&q=80",
                ThumbnailUrl = "https://images.unsplash.com/photo-1618044733300-9472054094ee?w=400&q=80",
                Content = @"<p>Reddit kullanıcısı stepanokdev, <strong>GitHub Enterprise Cloud</strong> hesabında yaşadığı faturalama sorununu paylaştı. Normalde 4 aktif kullanıcı için aylık $84 ödeyen şirket, Ekim ayı faturasında $168 ücretlendirilmiş.</p>

<h2>Destek Ekibi Yanıt Vermiyor</h2>
<p>Kullanıcı, 3 hafta önce açtığı destek talebine hala yanıt alamadığını belirtiyor. Enterprise hesapların 24 saat içinde yanıt alması beklenirken, bu durum hayal kırıklığı yarattı.</p>

<blockquote>""Enterprise hesapların 24 saat içinde yanıt alması gerekmiyor mu? Neredeyse bir aydır bekliyorum.""</blockquote>

<h2>Detaylar</h2>
<ul>
<li>Fatura No: INV102226125</li>
<li>Beklenen Ücret: ~$84</li>
<li>Çekilen Ücret: $168</li>
<li>GitHub Actions: $0 (tamamen indirimli)</li>
<li>Copilot: Devre dışı</li>
</ul>

<p>Kullanıcı, şirketin yalnızca doğru miktarı geri ödeyeceğini ve çift ücretlendirmeyi ödeyemeyeceğini belirtiyor. GitHub Enterprise ekibinden bir açıklama bekleniyor.</p>",
                Subjects = new[] { "GitHub", "Enterprise", "Billing" },
                Authors = new[] { "Reddit Community" },
                ExpressDate = now.AddDays(-2),
                CreateDate = now.AddDays(-2),
                UpdateDate = now.AddDays(-2),
                Priority = 1,
                IsActive = true,
                ViewCount = 2500,
                IsSecondPageNews = false,
            },
            new NewsArticle
            {
                Category = "github",
                Type = "news",
                Caption = "GitHub Ana Sayfasında Activity Bölümü Kayboldu",
                Slug = SlugHelper.GenerateSlug("GitHub Ana Sayfasında Activity Bölümü Kayboldu"),
                Keywords = "github, activity, sidebar, bug, SSO",
                SocialTags = "#GitHub #Bug #Activity",
                Summary = "Kullanıcılar GitHub ana sayfasındaki 'Activity' bölümünün kaybolduğunu bildiriyor. Sorun şirket SSO eklendiğinde başlamış olabilir.",
                ImgPath = "https://images.unsplash.com/photo-1556075798-4825dfaaf498?w=1200&q=80",
                ImgAlt = "GitHub Dashboard",
                ImageUrl = "https://images.unsplash.com/photo-1556075798-4825dfaaf498?w=1200&q=80",
                ThumbnailUrl = "https://images.unsplash.com/photo-1556075798-4825dfaaf498?w=400&q=80",
                Content = @"<p>Reddit kullanıcısı superl2, GitHub ana sayfasının sidebar'ında bulunan <strong>Activity</strong> bölümünün ortadan kaybolduğunu paylaştı.</p>

<h2>Sorunun Detayları</h2>
<p>Activity bölümü normalde kullanıcının son issue'ları ve pull request'lerini gösteriyor. Kullanıcı, sorununun şirket SSO login'i ekledikten sonra başladığını düşünüyor ancak bunun tesadüf olabileceğini belirtiyor.</p>

<h2>Topluluk Tepkileri</h2>
<p>Benzer sorunları yaşayan kullanıcılar, GitHub'ın son UI güncellemelerinden sonra çeşitli hataların ortaya çıktığını belirtiyor. Özellikle SSO entegrasyonundan sonra bazı özelliklerin kaybolması bilinen bir sorun.</p>

<p><strong>Geçici Çözüm:</strong> Kullanıcılar cache temizleme ve farklı tarayıcı kullanmayı öneriyorlar.</p>",
                Subjects = new[] { "GitHub", "Bug", "UI" },
                Authors = new[] { "Reddit Community" },
                ExpressDate = now.AddDays(-3),
                CreateDate = now.AddDays(-3),
                UpdateDate = now.AddDays(-3),
                Priority = 2,
                IsActive = true,
                ViewCount = 1000,
                IsSecondPageNews = false,
            },
            new NewsArticle
            {
                Category = "github",
                Type = "news",
                Caption = "GitHub Copilot Actions PR'larda Çöktü mü?",
                Slug = SlugHelper.GenerateSlug("GitHub Copilot Actions PR'larda Çöktü mü"),
                Keywords = "github, copilot, actions, billing, error",
                SocialTags = "#GitHubCopilot #Actions #Bug",
                Summary = "Kullanıcılar PR'larda @copilot etiketlendiğinde 'billing error' hatası alıyorlar. Hesaplar güncel ve limitler aşılmamış durumda.",
                ImgPath = "https://images.unsplash.com/photo-1677442136019-21780ecad995?w=1200&q=80",
                ImgAlt = "GitHub Copilot Error",
                ImageUrl = "https://images.unsplash.com/photo-1677442136019-21780ecad995?w=1200&q=80",
                ThumbnailUrl = "https://images.unsplash.com/photo-1677442136019-21780ecad995?w=400&q=80",
                Content = @"<p>Reddit kullanıcısı SoCalChrisW, Pull Request'lerde <strong>@copilot</strong> etiketlendiğinde hata aldığını bildirdi.</p>

<h2>Hata Mesajı</h2>
<blockquote>""Copilot has encountered an error. See logs for additional details.""</blockquote>

<p>Action log'larında ise şu hata görülüyor:</p>

<blockquote>""The job was not started because recent account payments have failed or your spending limit needs to be increased. Please check the 'Billing & plans' section in your settings""</blockquote>

<h2>Gerçek Durum</h2>
<ul>
<li>Kullanım limitlerin çok altında</li>
<li>Hesap güncel</li>
<li>Son ödeme denemesi yok</li>
<li>Billing cycle ortasında</li>
</ul>

<p>Birçok kullanıcı aynı hatayı alıyor. GitHub'ın Copilot Actions altyapısında genel bir sorun olduğu tahmin ediliyor.</p>",
                Subjects = new[] { "GitHub", "Copilot", "Actions" },
                Authors = new[] { "Reddit Community" },
                ExpressDate = now.AddDays(-1),
                CreateDate = now.AddDays(-1),
                UpdateDate = now.AddDays(-1),
                Priority = 1,
                IsActive = true,
                ViewCount = 750,
                IsSecondPageNews = false,
            },

            // Reddit News - Web Development
            new NewsArticle
            {
                Category = "reddit",
                Type = "discussion",
                Caption = "Copilot'u Kapattıktan Sonra Kodlamanın Ne Kadar Stresli Olduğunu Fark Ettim",
                Slug = SlugHelper.GenerateSlug("Copilot'u Kapattıktan Sonra Kodlamanın Ne Kadar Stresli Olduğunu Fark Ettim"),
                Keywords = "copilot, AI, coding, stress, productivity",
                SocialTags = "#Copilot #AI #Coding #WebDev",
                Summary = "6 yıllık bir geliştirici, Copilot'u kapattıktan sonra kodlamanın ne kadar rahatladığını paylaşıyor. AI'nın sürekli öneri yapması dikkat dağıtıcı olabiliyor.",
                ImgPath = "https://images.unsplash.com/photo-1555066931-4365d14bab8c?w=1200&q=80",
                ImgAlt = "Coding without AI",
                ImageUrl = "https://images.unsplash.com/photo-1555066931-4365d14bab8c?w=1200&q=80",
                ThumbnailUrl = "https://images.unsplash.com/photo-1555066931-4365d14bab8c?w=400&q=80",
                Content = @"<p>Reddit kullanıcısı xSypRo, 6 yıldır yazılım geliştirdiğini ve AI'nın son 3 yıldır hayatının bir parçası olduğunu belirtiyor. Ancak Copilot'u kapattıktan sonra şaşırtıcı bir keşif yaptı.</p>

<h2>AI ile Kodlama Stresi</h2>
<blockquote>""Copilot'u daha iyi bir IntelliSense gibi kullanıyordum. Ne yazmak istediğimi biliyordum ama bazen çok fazla yazı gerekliydi ve Copilot kısayol sağlıyordu. Ama bazen 'SUS!!! Düzenlemeyi bırak, odağımı dağıtıyorsun!!' diye düşünüyordum.""</blockquote>

<h2>TikTok/Reels Benzeri Davranış</h2>
<p>Kullanıcı, Copilot'un davranışını sosyal medya algoritmalarına benzetiyor:</p>
<ul>
<li>Sürekli ekranda değişiklikler</li>
<li>Yanıp sönen öneriler</li>
<li>Dikkat dağıtıcı görseller</li>
<li>Odaklanmayı zorlaştıran sürekli hareket</li>
</ul>

<blockquote>""Bu sadece bir text editör, bu şekilde davranmamalı.""</blockquote>

<h2>Yeni Yaklaşım</h2>
<p>Geliştirici şimdi varsayılan olarak kapalı tutup sadece template kod veya uzun yazı işlerinde açmayı deniyor. Topluluktan 222 upvote alan post, birçok geliştiricinin benzer hissettiğini gösteriyor.</p>

<p><strong>Sonuç:</strong> AI araçları üretkenliği artırabilir ama her zaman daha iyi değil. Kişisel tercih ve çalışma tarzı önemli.</p>",
                Subjects = new[] { "Web Development", "AI Tools", "Productivity" },
                Authors = new[] { "xSypRo" },
                ExpressDate = now.AddHours(-12),
                CreateDate = now.AddHours(-12),
                UpdateDate = now.AddHours(-12),
                Priority = 1,
                IsActive = true,
                ViewCount = 3200,
                IsSecondPageNews = false,
            },
            new NewsArticle
            {
                Category = "reddit",
                Type = "discussion",
                Caption = "Kişisel vs Kurumsal GitHub Hesapları Güvenlik Sorunu",
                Slug = SlugHelper.GenerateSlug("Kişisel vs Kurumsal GitHub Hesapları Güvenlik Sorunu"),
                Keywords = "github, security, DLP, enterprise, personal account",
                SocialTags = "#GitHub #Security #Enterprise",
                Summary = "Güvenlik ekipleri, geliştiricilerin kişisel GitHub hesaplarını iş için kullanmasını risk olarak işaretliyor. DLP politikaları atlanabilir.",
                ImgPath = "https://images.unsplash.com/photo-1563986768609-322da13575f3?w=1200&q=80",
                ImgAlt = "GitHub Security",
                ImageUrl = "https://images.unsplash.com/photo-1563986768609-322da13575f3?w=1200&q=80",
                ThumbnailUrl = "https://images.unsplash.com/photo-1563986768609-322da13575f3?w=400&q=80",
                Content = @"<p>Reddit kullanıcısı hashkent, şirketindeki güvenlik ekibinin tespit ettiği riski paylaştı: Geliştiriciler kişisel GitHub hesaplarıyla çalışırken şirket kodunu kendi hesaplarına push edebilir ve DLP politikalarını atlayabilir.</p>

<h2>Denenen Çözüm</h2>
<p>Kullanıcı iş için ayrı bir GitHub hesabı oluşturmaya çalışmış ancak GitHub'ın <strong>one-account-per-user</strong> politikası nedeniyle hesap suspend edilmiş.</p>

<h2>Şirket Durumu</h2>
<ul>
<li>Primarily GitLab shop</li>
<li>~120 mühendis için GitHub Copilot Enterprise SSO</li>
<li>Sadece 3 mobile developer GitHub'da kod tutuyor</li>
<li>Çoğu geliştirici katkı grafiği umursamıyor (kod GitLab'da)</li>
</ul>

<h2>Tartışma Noktası</h2>
<blockquote>""Özel iş hesabıyla bile, geliştiriciler 'john-acme' gibi kişisel repo'lara push edebilir ve ayrılmadan önce gerçek kişisel hesaplarına transfer edebilir. Bu biraz anlamsız bir sorun.""</blockquote>

<p>Topluluk, benzer kurulumda diğer şirketlerin nasıl yönettiğini tartışıyor.</p>",
                Subjects = new[] { "Security", "GitHub", "Enterprise" },
                Authors = new[] { "hashkent" },
                ExpressDate = now.AddDays(-4),
                CreateDate = now.AddDays(-4),
                UpdateDate = now.AddDays(-4),
                Priority = 2,
                IsActive = true,
                ViewCount = 900,
                IsSecondPageNews = false,
            },
            new NewsArticle
            {
                Category = "reddit",
                Type = "question",
                Caption = "GitHub Copilot Pro Ücretsiz Erişimi Kaybedilebilir mi?",
                Slug = SlugHelper.GenerateSlug("GitHub Copilot Pro Ücretsiz Erişimi Kaybedilebilir mi"),
                Keywords = "copilot, pro, free, open source, eligibility",
                SocialTags = "#Copilot #OpenSource #Free",
                Summary = "Açık kaynak projelere katkı yapanlara verilen ücretsiz Copilot Pro erişimi aylık kontrol ediliyor. Kullanıcılar erişimi kaybedip kaybetmeyeceklerini merak ediyor.",
                ImgPath = "https://images.unsplash.com/photo-1633356122544-f134324a6cee?w=1200&q=80",
                ImgAlt = "GitHub Copilot Pro",
                ImageUrl = "https://images.unsplash.com/photo-1633356122544-f134324a6cee?w=1200&q=80",
                ThumbnailUrl = "https://images.unsplash.com/photo-1633356122544-f134324a6cee?w=400&q=80",
                Content = @"<p>Reddit kullanıcısı a-curious-goose, açık kaynak projesine katkı sağladığı için ücretsiz <strong>GitHub Copilot Pro</strong> erişimi aldığını paylaştı.</p>

<h2>Aylık Kontrol</h2>
<p>Her ay şu mesajı alıyor:</p>
<blockquote>""Thank you for renewing your free access to GitHub Copilot... GitHub Copilot checks eligibility monthly per our policy.""</blockquote>

<h2>Belirsiz Kriterler</h2>
<p>Uygunluk kriterleri belirsiz:</p>
<ul>
<li>Hangi aktiviteler sayılıyor?</li>
<li>Ne kadar contribution gerekli?</li>
<li>Hangi projeler geçerli?</li>
<li>Aylık minimum var mı?</li>
</ul>

<h2>Soru</h2>
<blockquote>""Erişimi kaybeden birini biliyor musunuz? Copilot Pro'ya gelecekte ne kadar güvenebileceğimi bilmek istiyorum.""</blockquote>

<p>Topluluktan henüz net bir yanıt gelmemiş. GitHub'ın resmi açıklaması bekleniyor.</p>",
                Subjects = new[] { "GitHub", "Copilot", "Open Source" },
                Authors = new[] { "a-curious-goose" },
                ExpressDate = now.AddDays(-3),
                CreateDate = now.AddDays(-3),
                UpdateDate = now.AddDays(-3),
                Priority = 2,
                IsActive = true,
                ViewCount = 450,
                IsSecondPageNews = false,
            },

            // Technology News
            new NewsArticle
            {
                Category = "technology",
                Type = "news",
                Caption = "Yapay Zeka Kodlama Araçları: Copilot vs Cursor vs Cline",
                Slug = SlugHelper.GenerateSlug("Yapay Zeka Kodlama Araçları Copilot vs Cursor vs Cline"),
                Keywords = "AI, coding, copilot, cursor, cline, development tools",
                SocialTags = "#AI #Coding #DevTools",
                Summary = "2025 yılında geliştiricilerin en çok kullandığı AI kodlama araçları karşılaştırılıyor. Her birinin güçlü ve zayıf yönleri neler?",
                ImgPath = "https://images.unsplash.com/photo-1555949963-aa79dcee981c?w=1200&q=80",
                ImgAlt = "AI Coding Tools",
                ImageUrl = "https://images.unsplash.com/photo-1555949963-aa79dcee981c?w=1200&q=80",
                ThumbnailUrl = "https://images.unsplash.com/photo-1555949963-aa79dcee981c?w=400&q=80",
                Content = @"<p>2025'te geliştiriciler için AI kodlama araçları vazgeçilmez hale geldi. Ancak hangi araç hangi iş için en uygun?</p>

<h2>GitHub Copilot</h2>
<h3>Güçlü Yönleri:</h3>
<ul>
<li>VS Code'a native entegrasyon</li>
<li>Geniş dil desteği</li>
<li>Güçlü code completion</li>
<li>Enterprise SSO desteği</li>
</ul>

<h3>Zayıf Yönleri:</h3>
<ul>
<li>Bazen dikkat dağıtıcı</li>
<li>Context window sınırlı</li>
<li>Multi-file refactoring zayıf</li>
</ul>

<h2>Cursor</h2>
<h3>Güçlü Yönleri:</h3>
<ul>
<li>Mükemmel chat interface</li>
<li>Codebase-wide understanding</li>
<li>Multi-file editing</li>
<li>Custom prompts</li>
</ul>

<h3>Zayıf Yönleri:</h3>
<ul>
<li>Ayrı IDE gerekiyor</li>
<li>VS Code extension'larıyla uyumsuzluk</li>
<li>Ücretli model daha pahalı</li>
</ul>

<h2>Cline (eski Claude Dev)</h2>
<h3>Güçlü Yönleri:</h3>
<ul>
<li>Terminal komutlarını çalıştırabilir</li>
<li>Dosya sistemiyle etkileşim</li>
<li>Claude 3.5 Sonnet gücü</li>
<li>VS Code extension</li>
</ul>

<h3>Zayıf Yönleri:</h3>
<ul>
<li>Manuel onay gerektirir</li>
<li>API key maliyeti yüksek olabilir</li>
<li>Bazen fazla agresif</li>
</ul>

<h2>Sonuç</h2>
<p><strong>Yeni başlayanlar için:</strong> GitHub Copilot<br>
<strong>Büyük refactoring için:</strong> Cursor<br>
<strong>Automation için:</strong> Cline</p>",
                Subjects = new[] { "Technology", "AI", "Development Tools" },
                Authors = new[] { "Tech Review Team" },
                ExpressDate = now.AddHours(-6),
                CreateDate = now.AddHours(-6),
                UpdateDate = now.AddHours(-6),
                Priority = 1,
                IsActive = true,
                ViewCount = 1800,
                IsSecondPageNews = false,
            },

            // Twitter/X News
            new NewsArticle
            {
                Category = "twitter",
                Type = "news",
                Caption = "X (Twitter) Yeni Algoritma Güncellemesi: Uzun İçerikler Ön Planda",
                Slug = SlugHelper.GenerateSlug("X Twitter Yeni Algoritma Güncellemesi Uzun İçerikler Ön Planda"),
                Keywords = "twitter, X, algorithm, long form, content",
                SocialTags = "#Twitter #X #Algorithm",
                Summary = "X platformu, algoritmasını güncelledi. Artık uzun formlu içerikler ve thread'ler daha fazla görünürlük kazanacak.",
                ImgPath = "https://images.unsplash.com/photo-1611605698335-8b1569810432?w=1200&q=80",
                ImgAlt = "X Platform Algorithm",
                ImageUrl = "https://images.unsplash.com/photo-1611605698335-8b1569810432?w=1200&q=80",
                ThumbnailUrl = "https://images.unsplash.com/photo-1611605698335-8b1569810432?w=400&q=80",
                Content = @"<p>X (eski Twitter), içerik algoritmasında önemli değişiklikler yaptı. Yeni güncellemede <strong>uzun formlu içerikler</strong> ve detaylı thread'ler daha fazla boost alacak.</p>

<h2>Değişiklikler</h2>
<ul>
<li>280+ karakter içerikler artı puan</li>
<li>Thread'ler tek tweet'e göre 3x daha fazla reach</li>
<li>Dış link'ler artık cezalandırılmıyor</li>
<li>Video içerikler eskisi gibi öncelikli</li>
</ul>

<h2>Premium Aboneler</h2>
<p>X Premium (mavi tik) aboneleri için ek avantajlar:</p>
<ul>
<li>4000 karakter limiti</li>
<li>Algoritma puanında %40 boost</li>
<li>Reply'lerde öncelik</li>
<li>Edit özelliği</li>
</ul>

<h2>İçerik Üreticileri İçin Öneriler</h2>
<ol>
<li>Thread formatını kullanın</li>
<li>İlk tweet'i dikkat çekici yapın</li>
<li>Her thread'de en az 5 tweet olsun</li>
<li>Görsel/video ekleyin</li>
<li>Engagement için soru sorun</li>
</ol>

<p>Elon Musk, değişikliğin 'X'i gerçek bir tartışma platformu' haline getirmek için yapıldığını belirtti.</p>",
                Subjects = new[] { "Social Media", "Twitter", "Algorithm" },
                Authors = new[] { "Social Media News" },
                ExpressDate = now.AddHours(-8),
                CreateDate = now.AddHours(-8),
                UpdateDate = now.AddHours(-8),
                Priority = 1,
                IsActive = true,
                ViewCount = 2100,
                IsSecondPageNews = false,
            },
            new NewsArticle
            {
                Category = "twitter",
                Type = "news",
                Caption = "Twitter Spaces'te Yeni Özellik: Kayıt ve Tekrar İzleme",
                Slug = SlugHelper.GenerateSlug("Twitter Spaces'te Yeni Özellik Kayıt ve Tekrar İzleme"),
                Keywords = "twitter, spaces, recording, replay, audio",
                SocialTags = "#TwitterSpaces #Audio #SocialMedia",
                Summary = "X Spaces artık otomatik kaydedilebiliyor ve 30 gün boyunca tekrar dinlenebiliyor. Podcast alternatifi olma yolunda.",
                ImgPath = "https://images.unsplash.com/photo-1590602847861-f357a9332bbc?w=1200&q=80",
                ImgAlt = "Twitter Spaces",
                ImageUrl = "https://images.unsplash.com/photo-1590602847861-f357a9332bbc?w=1200&q=80",
                ThumbnailUrl = "https://images.unsplash.com/photo-1590602847861-f357a9332bbc?w=400&q=80",
                Content = @"<p>X (Twitter) Spaces için uzun zamandır beklenen <strong>kayıt özelliği</strong> aktif edildi. Kullanıcılar artık Space'lerini otomatik kaydedebilir ve 30 gün boyunca tekrar dinlenebilir hale getirebilir.</p>

<h2>Yeni Özellikler</h2>
<ul>
<li>Otomatik kayıt seçeneği</li>
<li>30 gün replay süresi</li>
<li>Timestamp'li bölümler</li>
<li>Hız ayarlama (0.5x - 2x)</li>
<li>İndirme seçeneği (Premium)</li>
</ul>

<h2>İçerik Üreticileri İçin Fırsatlar</h2>
<p>Bu özellik, Spaces'i podcast alternatifi haline getiriyor:</p>
<ol>
<li>Canlı yayın + replay avantajı</li>
<li>Anında publish (podcast upload'a gerek yok)</li>
<li>Native X audience'i</li>
<li>Monetization potansiyeli</li>
</ol>

<h2>Teknik Detaylar</h2>
<ul>
<li>Maksimum kayıt süresi: 12 saat</li>
<li>Audio quality: 128kbps</li>
<li>Storage: X Cloud</li>
<li>Format: M4A</li>
</ul>

<blockquote>""Bu özellik, Spaces'i sadece canlı bir deneyim olmaktan çıkarıp kalıcı içerik platformuna dönüştürüyor."" - X Product Team</blockquote>",
                Subjects = new[] { "Twitter", "Audio", "Content Creation" },
                Authors = new[] { "X News Team" },
                ExpressDate = now.AddHours(-10),
                CreateDate = now.AddHours(-10),
                UpdateDate = now.AddHours(-10),
                Priority = 2,
                IsActive = true,
                ViewCount = 1500,
                IsSecondPageNews = false,
            },

            // LinkedIn News
            new NewsArticle
            {
                Category = "linkedin",
                Type = "news",
                Caption = "LinkedIn'de AI Powered İş İlanları Dönemi Başladı",
                Slug = SlugHelper.GenerateSlug("LinkedIn'de AI Powered İş İlanları Dönemi Başladı"),
                Keywords = "linkedin, AI, job posting, recruitment, hiring",
                SocialTags = "#LinkedIn #AI #Recruitment",
                Summary = "LinkedIn, iş ilanları oluşturmak için AI destekli araçlar sunuyor. İş tanımları otomatik olarak optimize ediliyor.",
                ImgPath = "https://images.unsplash.com/photo-1586281380349-632531db7ed4?w=1200&q=80",
                ImgAlt = "LinkedIn AI Recruitment",
                ImageUrl = "https://images.unsplash.com/photo-1586281380349-632531db7ed4?w=1200&q=80",
                ThumbnailUrl = "https://images.unsplash.com/photo-1586281380349-632531db7ed4?w=400&q=80",
                Content = @"<p>LinkedIn, işe alım sürecini hızlandırmak için <strong>AI-powered iş ilanı oluşturma araçlarını</strong> kullanıma açtı.</p>

<h2>Yeni Özellikler</h2>
<ul>
<li>Otomatik iş tanımı yazma</li>
<li>Skill matching AI</li>
<li>Salary range önerileri</li>
<li>Competitor analysis</li>
<li>SEO optimizasyonu</li>
</ul>

<h2>Nasıl Çalışıyor?</h2>
<ol>
<li>Pozisyon başlığını girin</li>
<li>AI size şablon önerir</li>
<li>Gerekli skill'leri otomatik belirler</li>
<li>Market salary range'i gösterir</li>
<li>Tek tıkla yayınlayın</li>
</ol>

<h2>Recruiter'lar İçin Avantajlar</h2>
<blockquote>""AI sayesinde iş ilanı oluşturma süresi 30 dakikadan 3 dakikaya düştü."" - LinkedIn HR Analytics</blockquote>

<h3>Veri Odaklı Kararlar</h3>
<ul>
<li>Hangi skill'ler trend?</li>
<li>Rakipler ne kadar maaş veriyor?</li>
<li>Hangi keywords daha çok başvuru getiriyor?</li>
</ul>

<h2>Premium Özellikler</h2>
<p>LinkedIn Recruiter aboneleri için ek özellikler:</p>
<ul>
<li>Candidate matching AI</li>
<li>Automated screening questions</li>
<li>Interview scheduling assistant</li>
<li>Culture fit assessment</li>
</ul>

<p>Platform, 2025 sonuna kadar tüm iş ilanlarının %60'ının AI ile oluşturulacağını tahmin ediyor.</p>",
                Subjects = new[] { "LinkedIn", "AI", "Recruitment" },
                Authors = new[] { "LinkedIn Product Team" },
                ExpressDate = now.AddHours(-14),
                CreateDate = now.AddHours(-14),
                UpdateDate = now.AddHours(-14),
                Priority = 1,
                IsActive = true,
                ViewCount = 1200,
                IsSecondPageNews = false,
            },
            new NewsArticle
            {
                Category = "linkedin",
                Type = "news",
                Caption = "LinkedIn Learning: 2025'te En Çok Talep Gören 10 Skill",
                Slug = SlugHelper.GenerateSlug("LinkedIn Learning 2025'te En Çok Talep Gören 10 Skill"),
                Keywords = "linkedin, learning, skills, career, education",
                SocialTags = "#LinkedInLearning #Skills #Career",
                Summary = "LinkedIn, 2025 yılında iş dünyasında en çok aranan 10 yeteneği açıkladı. AI ve data science ilk sıralarda.",
                ImgPath = "https://images.unsplash.com/photo-1522202176988-66273c2fd55f?w=1200&q=80",
                ImgAlt = "LinkedIn Learning Skills",
                ImageUrl = "https://images.unsplash.com/photo-1522202176988-66273c2fd55f?w=1200&q=80",
                ThumbnailUrl = "https://images.unsplash.com/photo-1522202176988-66273c2fd55f?w=400&q=80",
                Content = @"<p>LinkedIn Learning, 1 milyar veri noktasını analiz ederek 2025'te en çok talep gören becerileri belirledi.</p>

<h2>Top 10 Skills</h2>
<ol>
<li><strong>AI & Machine Learning</strong> - %156 artış</li>
<li><strong>Data Science & Analytics</strong> - %142 artış</li>
<li><strong>Cloud Computing</strong> - %128 artış</li>
<li><strong>Cybersecurity</strong> - %118 artış</li>
<li><strong>Product Management</strong> - %95 artış</li>
<li><strong>UX/UI Design</strong> - %89 artış</li>
<li><strong>DevOps Engineering</strong> - %76 artış</li>
<li><strong>Digital Marketing</strong> - %72 artış</li>
<li><strong>Leadership & Management</strong> - %68 artış</li>
<li><strong>Sustainability</strong> - %64 artış</li>
</ol>

<h2>Sektör Bazında Değişim</h2>

<h3>Tech Sector</h3>
<ul>
<li>AI/ML en kritik skill</li>
<li>Full-stack developer talebi artıyor</li>
<li>Low-code/no-code platformlar popüler</li>
</ul>

<h3>Finance</h3>
<ul>
<li>Blockchain ve crypto knowledge</li>
<li>Regulatory compliance</li>
<li>Quantitative analysis</li>
</ul>

<h3>Healthcare</h3>
<ul>
<li>Healthcare AI</li>
<li>Telemedicine platforms</li>
<li>Patient data analytics</li>
</ul>

<h2>Kariyer Önerileri</h2>
<blockquote>""2025'te başarılı olmak isteyenler, en az 2-3 high-demand skill'e sahip olmalı."" - Ryan Roslansky, LinkedIn CEO</blockquote>

<h3>Nasıl Başlanır?</h3>
<ol>
<li>LinkedIn Learning'de ilginizi çeken bir kurs seçin</li>
<li>Haftalık en az 2 saat ayırın</li>
<li>Projeler yaparak pratik yapın</li>
<li>Sertifika alın ve profile ekleyin</li>
<li>Networking yaparak fırsatları değerlendirin</li>
</ol>

<p>Platform, bu skill'lere sahip profesyonellerin ortalama %34 daha yüksek maaş aldığını belirtiyor.</p>",
                Subjects = new[] { "LinkedIn", "Learning", "Career Development" },
                Authors = new[] { "LinkedIn Research Team" },
                ExpressDate = now.AddDays(-1),
                CreateDate = now.AddDays(-1),
                UpdateDate = now.AddDays(-1),
                Priority = 1,
                IsActive = true,
                ViewCount = 2800,
                IsSecondPageNews = false,
            },

            // Facebook News
            new NewsArticle
            {
                Category = "facebook",
                Type = "news",
                Caption = "Meta AI Artık WhatsApp, Instagram ve Facebook'ta Entegre",
                Slug = SlugHelper.GenerateSlug("Meta AI Artık WhatsApp Instagram ve Facebook'ta Entegre"),
                Keywords = "meta, AI, whatsapp, instagram, facebook, integration",
                SocialTags = "#MetaAI #WhatsApp #Instagram",
                Summary = "Meta'nın AI asistanı artık tüm platformlarda kullanılabiliyor. Sohbetlerde AI desteği ve görsel oluşturma özellikleri sunuluyor.",
                ImgPath = "https://images.unsplash.com/photo-1611162617474-5b21e879e113?w=1200&q=80",
                ImgAlt = "Meta AI Integration",
                ImageUrl = "https://images.unsplash.com/photo-1611162617474-5b21e879e113?w=1200&q=80",
                ThumbnailUrl = "https://images.unsplash.com/photo-1611162617474-5b21e879e113?w=400&q=80",
                Content = @"<p>Meta, kendi AI asistanını WhatsApp, Instagram ve Facebook'a tam entegre etti. Kullanıcılar artık sohbetlerde AI desteği alabiliyor.</p>

<h2>Yeni Özellikler</h2>

<h3>WhatsApp</h3>
<ul>
<li>Chat'lerde @meta ile AI çağırma</li>
<li>Mesaj önerileri</li>
<li>Çeviri desteği (100+ dil)</li>
<li>Voice message transkript</li>
</ul>

<h3>Instagram</h3>
<ul>
<li>DM'lerde AI asistan</li>
<li>Görsel oluşturma (text-to-image)</li>
<li>Caption önerileri</li>
<li>Hashtag optimizasyonu</li>
</ul>

<h3>Facebook</h3>
<ul>
<li>Post yazma yardımı</li>
<li>Görsel düzenleme</li>
<li>Event planning asistanı</li>
<li>Group management tools</li>
</ul>

<h2>Gizlilik</h2>
<p>Meta, AI'nın end-to-end şifreli sohbetlere erişemeyeceğini garanti ediyor:</p>
<blockquote>""Meta AI, yalnızca kullanıcının açıkça paylaştığı mesajları görebilir. E2E şifreli sohbetler tamamen özel kalır.""</blockquote>

<h2>İş Kullanımı</h2>
<p>Business hesaplar için ek özellikler:</p>
<ul>
<li>Customer service automation</li>
<li>Product recommendation</li>
<li>Order tracking</li>
<li>FAQ responses</li>
</ul>

<h2>Rekabet</h2>
<p>Bu hamle, Meta'yı ChatGPT ve Google Bard ile rekabette güçlendiriyor. 3 milyar+ kullanıcıya anında ulaşım, önemli bir avantaj sağlıyor.</p>",
                Subjects = new[] { "Meta", "AI", "Social Media" },
                Authors = new[] { "Meta Newsroom" },
                ExpressDate = now.AddHours(-5),
                CreateDate = now.AddHours(-5),
                UpdateDate = now.AddHours(-5),
                Priority = 1,
                IsActive = true,
                ViewCount = 3500,
                IsSecondPageNews = false,
            },

            // Instagram News
            new NewsArticle
            {
                Category = "instagram",
                Type = "news",
                Caption = "Instagram Reels Artık 10 Dakikaya Kadar Uzun Olabiliyor",
                Slug = SlugHelper.GenerateSlug("Instagram Reels Artık 10 Dakikaya Kadar Uzun Olabiliyor"),
                Keywords = "instagram, reels, video, content, creator",
                SocialTags = "#Instagram #Reels #ContentCreator",
                Summary = "Instagram, Reels için maksimum süreyi 90 saniyeden 10 dakikaya çıkardı. YouTube Shorts'a karşı hamle.",
                ImgPath = "https://images.unsplash.com/photo-1611162616305-c69b3fa7fbe0?w=1200&q=80",
                ImgAlt = "Instagram Reels",
                ImageUrl = "https://images.unsplash.com/photo-1611162616305-c69b3fa7fbe0?w=1200&q=80",
                ThumbnailUrl = "https://images.unsplash.com/photo-1611162616305-c69b3fa7fbe0?w=1200&q=80",
                Content = @"<p>Instagram, içerik üreticileri için önemli bir güncelleme yaptı: Reels artık <strong>10 dakikaya kadar</strong> uzun olabiliyor.</p>

<h2>Değişiklikler</h2>
<ul>
<li>Maksimum süre: 90 saniye → 10 dakika</li>
<li>Minimum süre değişmedi (3 saniye)</li>
<li>Tüm hesaplar için geçerli</li>
<li>Aşamalı rollout (2 hafta)</li>
</ul>

<h2>Neden Bu Değişiklik?</h2>
<p>Instagram, YouTube Shorts ve TikTok'un artan rekabetine yanıt veriyor:</p>

<blockquote>""Uzun formlu içerik, izleyicilerin daha fazla engagement göstermesini sağlıyor. Creator'lar daha derin hikayeler anlatabilecek."" - Adam Mosseri, Instagram Head</blockquote>

<h2>Content Creator'lar İçin Fırsatlar</h2>

<h3>Yeni İçerik Türleri</h3>
<ul>
<li>Tutorial ve how-to videoları</li>
<li>Mini vlog'lar</li>
<li>Product review'ları</li>
<li>Behind-the-scenes içerikler</li>
<li>Educational content</li>
</ul>

<h3>Monetization</h3>
<p>Uzun Reels için özel para kazanma seçenekleri:</p>
<ul>
<li>Mid-roll ads (5+ dakika videolarda)</li>
<li>Branded content integration</li>
<li>Bonus programı (view sayısına göre)</li>
</ul>

<h2>Algoritma Değişiklikleri</h2>
<p>Instagram, uzun Reels'leri boost ediyor:</p>
<ul>
<li>Watch time artık daha önemli metrik</li>
<li>Complete rate hesaplanıyor</li>
<li>Share ve save öncelikli</li>
</ul>

<h2>Best Practices</h2>
<ol>
<li><strong>Hook güçlü olsun:</strong> İlk 3 saniye kritik</li>
<li><strong>Bölümlere ayır:</strong> Uzun videoda chapter'lar kullan</li>
<li><strong>Caption detaylı:</strong> Timestamps ekle</li>
<li><strong>CTA koy:</strong> Like, comment, share isteyin</li>
</ol>

<p>Bu değişiklik, Instagram'ın 'kısa video platformu' imajını değiştirerek 'genel video platformu' haline gelme stratejisinin parçası.</p>",
                Subjects = new[] { "Instagram", "Reels", "Social Media" },
                Authors = new[] { "Instagram Creators Team" },
                ExpressDate = now.AddHours(-18),
                CreateDate = now.AddHours(-18),
                UpdateDate = now.AddHours(-18),
                Priority = 1,
                IsActive = true,
                ViewCount = 2900,
                IsSecondPageNews = false,
            },

            // TikTok News
            new NewsArticle
            {
                Category = "tiktok",
                Type = "news",
                Caption = "TikTok Shop Türkiye'de Açılıyor: E-Ticaretin Yeni Dönemi",
                Slug = SlugHelper.GenerateSlug("TikTok Shop Türkiye'de Açılıyor E-Ticaretin Yeni Dönemi"),
                Keywords = "tiktok, shop, e-commerce, turkey, shopping",
                SocialTags = "#TikTokShop #ETicaret #Shopping",
                Summary = "TikTok, Türkiye'de e-ticaret platformunu açıyor. Videolardan direkt alışveriş yapılabilecek.",
                ImgPath = "https://images.unsplash.com/photo-1607082348824-0a96f2a4b9da?w=1200&q=80",
                ImgAlt = "TikTok Shop",
                ImageUrl = "https://images.unsplash.com/photo-1607082348824-0a96f2a4b9da?w=1200&q=80",
                ThumbnailUrl = "https://images.unsplash.com/photo-1607082348824-0a96f2a4b9da?w=400&q=80",
                Content = @"<p>TikTok, Kasım 2025'te Türkiye'de <strong>TikTok Shop</strong> platformunu açıyor. Kullanıcılar videolardan direkt ürün satın alabilecek.</p>

<h2>Özellikler</h2>

<h3>Kullanıcılar İçin</h3>
<ul>
<li>Video izlerken tek tıkla alışveriş</li>
<li>Live stream satışları</li>
<li>Creator önerileri</li>
<li>Güvenli ödeme sistemi</li>
<li>Hızlı kargo takibi</li>
</ul>

<h3>Satıcılar İçin</h3>
<ul>
<li>Ücretsiz mağaza açma</li>
<li>%5 komisyon (ilk 6 ay %0)</li>
<li>Creator partnership programı</li>
<li>Analytics dashboard</li>
<li>Advertising tools</li>
</ul>

<h2>Nasıl Çalışıyor?</h2>
<ol>
<li><strong>Satıcı:</strong> Ürünü TikTok Shop'a ekler</li>
<li><strong>Creator:</strong> Ürünü videosunda tanıtır</li>
<li><strong>Link:</strong> Video'ya alışveriş linki eklenir</li>
<li><strong>Satış:</strong> Kullanıcı videonun içinden satın alır</li>
<li><strong>Commission:</strong> Creator ve TikTok pay alır</li>
</ol>

<h2>Türkiye Pazarı</h2>
<p>TikTok Türkiye'deki potansiyele inanıyor:</p>
<ul>
<li>30M+ aktif kullanıcı</li>
<li>Günlük 60 dakika ortalama kullanım</li>
<li>Genç demografik (18-34 yaş %65)</li>
<li>Yüksek engagement rate</li>
</ul>

<h2>Rakipler</h2>
<p>Bu hamle, Trendyol, Hepsiburada ve Amazon'a karşı önemli bir rekabet:</p>
<blockquote>""Social commerce, e-ticaretin geleceği. Türkiye'de bu trendi öncü olmak istiyoruz."" - TikTok EMEA Director</blockquote>

<h2>Creator Economy</h2>
<p>Türk influencer'lar için yeni gelir kapısı:</p>
<ul>
<li>Affiliate komisyonları (%10-20)</li>
<li>Sponsored content</li>
<li>Brand partnerships</li>
<li>Live shopping bonusu</li>
</ul>

<h2>Launch Plan</h2>
<ul>
<li><strong>Beta:</strong> Kasım 2025</li>
<li><strong>Public:</strong> Aralık 2025</li>
<li><strong>Hedef:</strong> İlk yıl $500M GMV</li>
</ul>",
                Subjects = new[] { "TikTok", "E-commerce", "Social Commerce" },
                Authors = new[] { "TikTok Business Team" },
                ExpressDate = now.AddDays(-1),
                CreateDate = now.AddDays(-1),
                UpdateDate = now.AddDays(-1),
                Priority = 1,
                IsActive = true,
                ViewCount = 4200,
                IsSecondPageNews = false,
            },

            // YouTube News
            new NewsArticle
            {
                Category = "youtube",
                Type = "news",
                Caption = "YouTube Premium Türkiye'de Fiyat Artışı: Yeni Tarifeler Açıklandı",
                Slug = SlugHelper.GenerateSlug("YouTube Premium Türkiye'de Fiyat Artışı Yeni Tarifeler Açıklandı"),
                Keywords = "youtube, premium, pricing, turkey, subscription",
                SocialTags = "#YouTube #Premium #Pricing",
                Summary = "YouTube Premium, Türkiye'de fiyatlarını güncelledi. Bireysel abonelik 59.99 TL'den 89.99 TL'ye çıktı.",
                ImgPath = "https://images.unsplash.com/photo-1611162618071-b39a2ec055fb?w=1200&q=80",
                ImgAlt = "YouTube Premium",
                ImageUrl = "https://images.unsplash.com/photo-1611162618071-b39a2ec055fb?w=1200&q=80",
                ThumbnailUrl = "https://images.unsplash.com/photo-1611162618071-b39a2ec055fb?w=400&q=80",
                Content = @"<p>YouTube, Türkiye'deki Premium abonelik fiyatlarını <strong>1 Kasım 2025</strong> tarihinden itibaren güncelliyor.</p>

<h2>Yeni Fiyatlar</h2>

<table style=""width:100%;border-collapse:collapse;margin:20px 0"">
<thead style=""background:#f0f0f0"">
<tr>
<th style=""padding:12px;text-align:left;border:1px solid #ddd"">Plan</th>
<th style=""padding:12px;text-align:right;border:1px solid #ddd"">Eski Fiyat</th>
<th style=""padding:12px;text-align:right;border:1px solid #ddd"">Yeni Fiyat</th>
<th style=""padding:12px;text-align:right;border:1px solid #ddd"">Artış</th>
</tr>
</thead>
<tbody>
<tr>
<td style=""padding:12px;border:1px solid #ddd"">Bireysel (Aylık)</td>
<td style=""padding:12px;text-align:right;border:1px solid #ddd"">59.99 TL</td>
<td style=""padding:12px;text-align:right;border:1px solid #ddd"">89.99 TL</td>
<td style=""padding:12px;text-align:right;border:1px solid #ddd;color:red"">+50%</td>
</tr>
<tr style=""background:#fafafa"">
<td style=""padding:12px;border:1px solid #ddd"">Öğrenci (Aylık)</td>
<td style=""padding:12px;text-align:right;border:1px solid #ddd"">29.99 TL</td>
<td style=""padding:12px;text-align:right;border:1px solid #ddd"">44.99 TL</td>
<td style=""padding:12px;text-align:right;border:1px solid #ddd;color:red"">+50%</td>
</tr>
<tr>
<td style=""padding:12px;border:1px solid #ddd"">Aile (Aylık)</td>
<td style=""padding:12px;text-align:right;border:1px solid #ddd"">89.99 TL</td>
<td style=""padding:12px;text-align:right;border:1px solid #ddd"">134.99 TL</td>
<td style=""padding:12px;text-align:right;border:1px solid #ddd;color:red"">+50%</td>
</tr>
</tbody>
</table>

<h2>Premium Özellikleri</h2>
<ul>
<li>Reklamsız izleme</li>
<li>Arka planda oynatma</li>
<li>Offline indirme</li>
<li>YouTube Music Premium dahil</li>
<li>Picture-in-picture mode</li>
<li>Queue management</li>
</ul>

<h2>Neden Artış?</h2>
<p>YouTube'un açıklamasına göre:</p>
<blockquote>""Türkiye'deki enflasyon ve döviz kurları, fiyat güncellemesini gerekli kıldı. Bölgesel ekonomik koşullara uyum sağlıyoruz.""</blockquote>

<h2>Mevcut Aboneler</h2>
<p>Şu anki aboneler için:</p>
<ul>
<li>3 ay boyunca eski fiyat geçerli</li>
<li>Şubat 2026'dan itibaren yeni fiyat</li>
<li>İptal hakkı saklı</li>
</ul>

<h2>Alternatifler</h2>
<p>Kullanıcılar şu alternatiflere bakıyor:</p>
<ol>
<li><strong>YouTube Music:</strong> 54.99 TL (sadece müzik)</li>
<li><strong>Ad blocker:</strong> Ücretsiz ama kurallara aykırı</li>
<li><strong>Aile planı:</strong> 6 kişiye kadar, kişi başı 22.50 TL</li>
</ol>

<h2>Sosyal Medya Tepkileri</h2>
<p>Twitter'da #YouTubePremium trending oldu. Kullanıcılar %50 artışın çok fazla olduğunu belirtiyor. Bazıları aboneliği iptal edeceğini söyledi.</p>

<p>YouTube Türkiye'den resmi açıklama bekleniyor.</p>",
                Subjects = new[] { "YouTube", "Premium", "Pricing" },
                Authors = new[] { "YouTube Turkey Team" },
                ExpressDate = now.AddHours(-20),
                CreateDate = now.AddHours(-20),
                UpdateDate = now.AddHours(-20),
                Priority = 1,
                IsActive = true,
                ViewCount = 5600,
                IsSecondPageNews = false,
            },
        };

        // Generate slugs for all news articles before inserting
        foreach (var article in newsArticles.Where(a => string.IsNullOrEmpty(a.Slug)))
        {
            article.Slug = SlugHelper.GenerateSlug(article.Caption);
        }

        await newsCollection.InsertManyAsync(newsArticles, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        Console.WriteLine($"Successfully seeded {newsArticles.Count} news articles to the database!");
    }
}
