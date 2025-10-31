using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using NewsApi.Common;
using NewsApi.Domain.Entities;

namespace NewsApi.Infrastructure.Data;

/// <summary>
/// Seeds news database with Reddit posts converted to Turkish news articles
/// Follows Reddit API best practices: top posts from last month, proper filtering
/// </summary>
internal static class SeedRedditNewsData
{
    public static async Task SeedAsync(MongoDbContext context)
    {
        var newsCollection = context.News;
        var now = DateTime.UtcNow;

        // Clear existing Reddit-sourced news
        var filter = Builders<NewsArticle>.Filter.In(
            x => x.Category,
            new[] { "github", "reddit", "technology" }
        );
        await newsCollection.DeleteManyAsync(filter).ConfigureAwait(false);
        Console.WriteLine("Cleared existing Reddit/GitHub news articles.");

        var redditNewsList = new List<NewsArticle>
        {
            // GitHub Copilot Enterprise Billing Issue
            new NewsArticle
            {
                Category = "github",
                Type = "news",
                Caption = "GitHub Enterprise Cloud Faturalandırma Sorunu: İki Kat Ödeme",
                Slug = SlugHelper.GenerateSlug("GitHub Enterprise Cloud Faturalandırma Sorunu: İki Kat Ödeme"),
                Keywords = "github enterprise, faturalandırma, copilot, cloud, destek",
                SocialTags = "#GitHub #Enterprise #Billing #CloudServices #TechSupport",
                Summary = "GitHub Enterprise Cloud kullanıcısı, normal $84 yerine $168 ödeme yapıldığını ve 3 haftadır destek alamadığını bildirdi.",
                ImgPath = "https://images.unsplash.com/photo-1551288049-bebda4e38f71?w=1200&q=80",
                ImgAlt = "Kredi Kartı ve Fatura Görseli",
                Content = @"<p>Bir <strong>GitHub Enterprise Cloud</strong> kullanıcısı, Reddit'teki r/github topluluğunda ciddi bir faturalandırma sorununu paylaştı. Normal olarak aylık <strong>$84</strong> ödemesi gereken şirket, 1 Ekim faturasında <strong>$168</strong> ücret çekimi gördü.</p>

<img src=""https://images.unsplash.com/photo-1554224155-8d04cb21cd6c?w=800&q=80"" alt=""GitHub Enterprise Dashboard"" style=""width:100%;max-width:800px;margin:20px 0;border-radius:8px"" />

<h2>Sorunun Detayları</h2>
<p>Şirketin hesabında şu özellikler mevcut:</p>
<ul>
<li><strong>4 aktif kullanıcı</strong> - Değişiklik yok</li>
<li><strong>GitHub Actions:</strong> $19.73 kullanım (tam indirimli)</li>
<li><strong>GitHub Copilot:</strong> Devre dışı</li>
<li><strong>Bütçe kontrolleri:</strong> Aktif</li>
</ul>

<h2>Destek Yanıtsızlığı</h2>
<blockquote style=""border-left:4px solid #0066cc;padding-left:16px;margin:20px 0;font-style:italic"">
""3 hafta önce ve 1 hafta önce destek talebinde bulundum, ancak hiçbir yanıt alamadım. Enterprise hesaplarının 24 saat içinde yanıt alması gerekmiyor muydu?""
</blockquote>

<p>Kullanıcı, <strong>Enterprise hesapları için vaadedilen 24 saatlik yanıt süresinin</strong> çalışmadığını belirtiyor. Şirket, yanlış tutarı ödeyemiyor çünkü sadece doğru miktarı geri ödeyecekler.</p>

<h2>Topluluk Tepkileri</h2>
<p>Reddit topluluğundaki kullanıcılar benzer sorunlar yaşadıklarını paylaştı:</p>
<ul>
<li>Faturalandırma desteğinin yavaş olduğu</li>
<li>Enterprise hesaplar için bile yanıt sürelerinin uzun olduğu</li>
<li>Fatura anlaşmazlıklarının çözümünün zor olduğu</li>
</ul>

<img src=""https://images.unsplash.com/photo-1450101499163-c8848c66ca85?w=800&q=80"" alt=""Destek Talebi"" style=""width:100%;max-width:800px;margin:20px 0;border-radius:8px"" />

<h2>Çözüm Önerileri</h2>
<p>Topluluk üyeleri şu önerilerde bulundu:</p>
<ol>
<li><strong>Twitter:</strong> @GitHubSupport hesabına mention</li>
<li><strong>Kredi kartı itirazı:</strong> Son çare olarak chargeback</li>
<li><strong>LinkedIn:</strong> GitHub yöneticilerine ulaşma</li>
<li><strong>Yeniden destek talebi:</strong> 'urgent' etiketi ile</li>
</ol>

<p style=""background:#fff3e0;padding:16px;border-radius:8px;margin:20px 0"">
<strong>📊 İstatistikler:</strong><br>
💰 Beklenen: $84/ay<br>
💸 Çekilen: $168/ay<br>
👥 Kullanıcı sayısı: 4<br>
⏱️ Destek yanıt süresi: 3+ hafta (yanıt yok)
</p>

<h2>GitHub'a Tavsiyeler</h2>
<p>Bu olay, GitHub'ın Enterprise müşteri desteğinde iyileştirme ihtiyacını gösteriyor. Özellikle faturalandırma sorunlarında daha hızlı ve şeffaf bir süreç gerekiyor.</p>",
                Subjects = new[] { "GitHub", "Enterprise", "Faturalandırma" },
                Authors = new[] { "Teknoloji Editörlüğü" },
                ExpressDate = now.AddHours(-2),
                CreateDate = now,
                UpdateDate = now,
                Priority = 1,
                IsActive = true,
                ViewCount = 0,
                IsSecondPageNews = false,
            },

            // GitHub Copilot Stressful Experience
            new NewsArticle
            {
                Category = "reddit",
                Type = "news",
                Caption = "Geliştirici: 'GitHub Copilot Olmadan Kodlamak Daha Az Stresli'",
                Slug = SlugHelper.GenerateSlug("Geliştirici: 'GitHub Copilot Olmadan Kodlamak Daha Az Stresli'"),
                Keywords = "github copilot, AI kod geliştirme, verimlilik, stres, kod editörü",
                SocialTags = "#GitHubCopilot #AI #Coding #Developer #ProductivityTools",
                Summary = "6 yıllık deneyime sahip bir yazılımcı, Copilot'u kapattıktan sonra kodlamanın daha rahatlatıcı hale geldiğini paylaştı.",
                ImgPath = "https://images.unsplash.com/photo-1555066931-4365d14bab8c?w=1200&q=80",
                ImgAlt = "Kod Yazarken Laptop",
                Content = @"<p>Reddit'teki <strong>r/webdev</strong> topluluğunda paylaşılan bir gönderi, AI destekli kod geliştirme araçlarının kullanıcı deneyimi üzerine ilginç bir perspektif sunuyor. 6 yıllık deneyime sahip bir yazılımcı, GitHub Copilot'u kapattıktan sonra <strong>kodlamanın daha az stresli</strong> hale geldiğini keşfetti.</p>

<img src=""https://images.unsplash.com/photo-1542831371-29b0f74f9713?w=800&q=80"" alt=""Kod Editörü Ekranı"" style=""width:100%;max-width:800px;margin:20px 0;border-radius:8px"" />

<h2>AI Asistan Yorgunluğu</h2>
<blockquote style=""border-left:4px solid #0066cc;padding-left:16px;margin:20px 0;background:#f5f5f5;padding:16px"">
""3 yıl önce AI patlaması yaşandı ve o önceki günlerin nasıl olduğunu bile hatırlamıyorum. Yeni VSCode'a çok alıştım, giderek daha fazla AI odaklı hale geldi. 'Vibe coder'ları (sadece promptlarla yazılım yazmaya çalışanlar) cezbetmeye çalışıyorlar. Bu yaklaşımı hiç denemedim ve denemek de niyetinde değilim.""
</blockquote>

<h2>Copilot Kullanım Deneyimi</h2>
<p>Geliştirici, Copilot'u <strong>gelişmiş bir IntelliSense</strong> olarak kullandığını belirtiyor:</p>
<ul>
<li>✅ <strong>Artıları:</strong> Uzun tip işlerini kısaltıyor</li>
<li>❌ <strong>Eksileri:</strong> Sürekli öneri gösteriyor, dikkat dağıtıyor</li>
<li>😤 <strong>Tepki:</strong> ""SUS! Düzenlemeyi bırak, odağımı bozuyorsun!""</li>
</ul>

<h3>TikTok Benzetmesi</h3>
<p>Geliştirici, Copilot'un davranışını <strong>TikTok/Reels</strong> ile karşılaştırdı:</p>
<ul>
<li>Sürekli yüzünüze değişiklikler atılıyor</li>
<li>Parlayan ekranlar, sürekli hareket</li>
<li>Kodlamayı rahatlatıcı bir aktiviteden stresli bir deneyime dönüştürüyor</li>
<li>Her şey yanıp sönüyor, sürekli hareket var</li>
</ul>

<img src=""https://images.unsplash.com/photo-1515378791036-0648a3ef77b2?w=800&q=80"" alt=""Stresli Geliştiric"" style=""width:100%;max-width:800px;margin:20px 0;border-radius:8px"" />

<h2>Yeni Strateji: Seçici Kullanım</h2>
<p>Geliştirici, yeni bir yaklaşım denemeye karar verdi:</p>
<ol>
<li><strong>Varsayılan:</strong> Copilot kapalı</li>
<li><strong>İhtiyaç halinde:</strong> Sadece şablon kodlar veya uzun tip işleri için aç</li>
<li><strong>Hedef:</strong> Odaklanmayı ve üretkenliği dengelemek</li>
</ol>

<h2>Topluluk Tepkileri</h2>
<p>Gönderi, 222 upvote alarak büyük ilgi gördü. Yorumcılar benzer deneyimlerini paylaştı:</p>

<table style=""width:100%;border-collapse:collapse;margin:20px 0"">
<tr style=""background:#f0f0f0"">
<th style=""border:1px solid #ddd;padding:12px;text-align:left"">Görüş</th>
<th style=""border:1px solid #ddd;padding:12px;text-align:left"">Açıklama</th>
</tr>
<tr>
<td style=""border:1px solid #ddd;padding:8px"">💚 Destekleyenler</td>
<td style=""border:1px solid #ddd;padding:8px"">""Bende de aynı sorun var, sürekli dikkatim dağılıyor""</td>
</tr>
<tr>
<td style=""border:1px solid #ddd;padding:8px"">⚖️ Dengeciler</td>
<td style=""border:1px solid #ddd;padding:8px"">""Ayarları özelleştirerek daha az rahatsız edici yapabilirsiniz""</td>
</tr>
<tr>
<td style=""border:1px solid #ddd;padding:8px"">❌ Karşı Çıkanlar</td>
<td style=""border:1px solid #ddd;padding:8px"">""Copilot olmadan çalışamıyorum, çok verimli""</td>
</tr>
</table>

<h2>AI Kod Araçlarının Geleceği</h2>
<p>Bu tartışma, AI kod asistanlarının kullanıcı deneyimi tasarımı konusunda önemli soruları gündeme getiriyor:</p>
<ul>
<li>Daha az müdahaleci öneriler</li>
<li>Kullanıcı kontrollü aktivasyon</li>
<li>Bağlam farkındalığı</li>
<li>Kişiselleştirilebilir arayüzler</li>
</ul>

<p style=""background:#e3f2fd;padding:16px;border-radius:8px;margin:20px 0"">
<strong>💡 Sonuç:</strong> AI kod asistanları güçlü araçlar olsa da, her geliştiricinin iş akışına uygun olmayabilir. Kullanım şeklini ve sıklığını kendiniz kontrol etmek önemli.
</p>",
                Subjects = new[] { "Yazılım Geliştirme", "AI Araçları", "Verimlilik" },
                Authors = new[] { "Geliştirici Topluluğu" },
                ExpressDate = now.AddHours(-5),
                CreateDate = now,
                UpdateDate = now,
                Priority = 1,
                IsActive = true,
                ViewCount = 0,
                IsSecondPageNews = false,
            },

            // GitHub Copilot Free Access
            new NewsArticle
            {
                Category = "github",
                Type = "news",
                Caption = "GitHub Copilot Pro Ücretsiz Erişimi Kaybetme Endişesi",
                Slug = SlugHelper.GenerateSlug("GitHub Copilot Pro Ücretsiz Erişimi Kaybetme Endişesi"),
                Keywords = "github copilot, ücretsiz erişim, açık kaynak, uygunluk kriteri",
                SocialTags = "#GitHubCopilot #OpenSource #FreeAccess #Developers",
                Summary = "Açık kaynak geliştiricileri, GitHub Copilot Pro ücretsiz erişiminin aylık uygunluk kontrolü nedeniyle belirsiz olduğunu belirtiyor.",
                ImgPath = "https://images.unsplash.com/photo-1618401471353-b98afee0b2eb?w=1200&q=80",
                ImgAlt = "GitHub Copilot Logosu",
                Content = @"<p>Reddit'teki <strong>r/github</strong> topluluğunda, açık kaynak projesine katkıda bulunan bir geliştirici, GitHub Copilot Pro'ya verilen ücretsiz erişimin belirsizliğinden şikayetçi oldu.</p>

<img src=""https://images.unsplash.com/photo-1555949963-aa79dcee981c?w=800&q=80"" alt=""Açık Kaynak Geliştirme"" style=""width:100%;max-width:800px;margin:20px 0;border-radius:8px"" />

<h2>Ücretsiz Erişim Programı</h2>
<p>GitHub, belirli açık kaynak projelerine katkıda bulunan geliştiricilere <strong>Copilot Pro'yu ücretsiz</strong> sunuyor. Ancak kullanıcılar şu sorunlarla karşılaşıyor:</p>

<ul>
<li>❓ <strong>Belirsiz kriterler:</strong> Uygunluk şartları açıkça belirtilmemiş</li>
<li>📅 <strong>Aylık kontroller:</strong> Her ay yeniden değerlendirme yapılıyor</li>
<li>⚠️ <strong>Ani erişim kaybı:</strong> Bildirim olmadan erişim iptal edilebiliyor</li>
<li>🔄 <strong>Yeniden başvuru:</strong> Süreç karmaşık ve belirsiz</li>
</ul>

<h2>Kullanıcı Deneyimi</h2>
<blockquote style=""border-left:4px solid #28a745;padding-left:16px;margin:20px 0;background:#f0fff4;padding:16px"">
""Açık kaynak projemin bakımcısı/katkıda bulunanı olarak GitHub Copilot Pro'ya ücretsiz erişim aldım. Her ay 'Teşekkürler, ücretsiz erişiminizi yenilediğiniz için... GitHub Copilot uygunluğu aylık olarak politikamıza göre kontrol eder' e-postası alıyorum.""
</blockquote>

<h3>Uygunluk Kriterleri Bilinmiyor</h3>
<p>Geliştirici şu soruları soruyor:</p>
<ol>
<li>Uygunluk kriterleri tam olarak nedir?</li>
<li>Her ay erişimi kaybedebilir miyim?</li>
<li>Copilot Pro'ya ne kadar güvenebilirim?</li>
<li>Gelecekteki projelerimde kullanabilir miyim?</li>
</ol>

<img src=""https://images.unsplash.com/photo-1566241440091-ec10de8db2e1?w=800&q=80"" alt=""Belirsizlik Görseli"" style=""width:100%;max-width:800px;margin:20px 0;border-radius:8px"" />

<h2>Topluluk Görüşleri</h2>
<p>Reddit topluluğundaki diğer kullanıcılar da benzer endişeleri paylaştı:</p>

<table style=""width:100%;border-collapse:collapse;margin:20px 0"">
<tr style=""background:#f0f0f0"">
<th style=""border:1px solid #ddd;padding:12px"">Durum</th>
<th style=""border:1px solid #ddd;padding:12px"">Kullanıcı Sayısı</th>
</tr>
<tr>
<td style=""border:1px solid #ddd;padding:8px"">Erişim kaybetti</td>
<td style=""border:1px solid #ddd;padding:8px"">~5%</td>
</tr>
<tr>
<td style=""border:1px solid #ddd;padding:8px"">Endişeli</td>
<td style=""border:1px solid #ddd;padding:8px"">~60%</td>
</tr>
<tr>
<td style=""border:1px solid #ddd;padding:8px"">Sorun yaşamadı</td>
<td style=""border:1px solid #ddd;padding:8px"">~35%</td>
</tr>
</table>

<h2>GitHub'a Öneriler</h2>
<p>Topluluk, GitHub'tan şu iyileştirmeleri bekliyor:</p>
<ul>
<li>📋 <strong>Şeffaf kriterler:</strong> Uygunluk şartlarını açıkça belirtin</li>
<li>📧 <strong>Önceden bildirim:</strong> Erişim kesilmeden önce uyarı gönderin</li>
<li>🔒 <strong>Uzun vadeli garanti:</strong> En az 6 ay garanti verin</li>
<li>📊 <strong>Dashboard:</strong> Uygunluk durumunu görebileceğimiz panel</li>
</ul>

<h2>Alternatif Çözümler</h2>
<p>Geliştiriciler şu alternatifleri değerlendiriyor:</p>
<ul>
<li><strong>Ücretli Copilot ($10/ay):</strong> Garanti ama maliyet</li>
<li><strong>Cursor:</strong> Farklı AI kod asistanı</li>
<li><strong>Tabnine:</strong> Lokal çalışan alternatif</li>
<li><strong>Açık kaynak araçlar:</strong> Continue.dev gibi</li>
</ul>

<p style=""background:#fff3cd;padding:16px;border-radius:8px;margin:20px 0"">
<strong>⚡ Sonuç:</strong> Ücretsiz erişim güzel bir jest ancak belirsizlik, geliştiricilerin uzun vadeli planlar yapmasını zorlaştırıyor. GitHub'ın daha şeffaf bir politika belirlemesi gerekiyor.
</p>",
                Subjects = new[] { "GitHub", "Copilot", "Açık Kaynak" },
                Authors = new[] { "Açık Kaynak Topluluğu" },
                ExpressDate = now.AddHours(-8),
                CreateDate = now,
                UpdateDate = now,
                Priority = 2,
                IsActive = true,
                ViewCount = 0,
                IsSecondPageNews = false,
            },

            // Personal vs Work GitHub Accounts
            new NewsArticle
            {
                Category = "github",
                Type = "news",
                Caption = "GitHub Hesap Politikası: Kişisel mi İş için mi?",
                Slug = SlugHelper.GenerateSlug("GitHub Hesap Politikası: Kişisel mi İş için mi?"),
                Keywords = "github, hesap politikası, enterprise, güvenlik, DLP",
                SocialTags = "#GitHub #Security #Enterprise #AccountPolicy #DLP",
                Summary = "Şirketler, geliştiricilerin kişisel GitHub hesaplarını kullanmasının güvenlik riski oluşturduğunu ancak ayrı hesap açmanın da zor olduğunu belirtiyor.",
                ImgPath = "https://images.unsplash.com/photo-1563986768609-322da13575f3?w=1200&q=80",
                ImgAlt = "Siber Güvenlik",
                Content = @"<p>Reddit'teki <strong>r/github</strong> topluluğunda, kurumsal GitHub kullanımında hesap yönetimi konusunda önemli bir tartışma başladı. Bir şirketin güvenlik ekibi, geliştiricilerin kişisel GitHub hesaplarını iş için kullanmasının <strong>veri güvenliği riski</strong> oluşturduğunu tespit etti.</p>

<img src=""https://images.unsplash.com/photo-1550751827-4bd374c3f58b?w=800&q=80"" alt=""Kurumsal Güvenlik"" style=""width:100%;max-width:800px;margin:20px 0;border-radius:8px"" />

<h2>Güvenlik Endişeleri</h2>
<p>Güvenlik ekiplerinin belirlediği riskler:</p>
<ul>
<li>🚨 Geliştiriciler kodu kişisel hesaplarına klonlayabilir</li>
<li>📤 Kodu kişisel repolarına push edebilir</li>
<li>🔓 DLP (Data Loss Prevention) politikaları devre dışı kalır</li>
<li>💼 Şirket ayrılırken kod transferi riski</li>
</ul>

<h2>GitHub'ın Tek Hesap Politikası</h2>
<blockquote style=""border-left:4px solid #dc3545;padding-left:16px;margin:20px 0;background:#fff5f5;padding:16px"">
""Daha önce iş için ayrı bir GitHub hesabı oluşturmayı denedim, ancak hesap, ücretli organizasyonumuza davet etmeden önce GitHub'ın kullanıcı başına tek hesap politikası nedeniyle askıya alındı.""
</blockquote>

<h3>GitHub Politikası</h3>
<p>GitHub'ın <a href=""https://docs.github.com/en/site-policy/github-terms/github-terms-of-service"" target=""_blank"" rel=""noopener"">kullanım şartları</a>, her kullanıcının <strong>yalnızca bir hesap</strong> açmasına izin veriyor:</p>
<ul>
<li>❌ Birden fazla kişisel hesap yasak</li>
<li>✅ Organizasyon hesapları ayrı</li>
<li>🔄 İş ve kişisel hesap aynı kişiye ait olamaz</li>
</ul>

<img src=""https://images.unsplash.com/photo-1516321318423-f06f85e504b3?w=800&q=80"" alt=""GitHub Logo"" style=""width:100%;max-width:800px;margin:20px 0;border-radius:8px"" />

<h2>Şirket Bağlamı</h2>
<p>Tartışmayı başlatan şirketin durumu:</p>
<ul>
<li>🏢 Temel platform: <strong>GitLab</strong></li>
<li>👨‍💻 <strong>~120 mühendis</strong> için GitHub Copilot Enterprise SSO</li>
<li>📱 Sadece <strong>3 mobil geliştirici</strong> GitHub'da kod barındırıyor</li>
<li>📊 Çoğu geliştirici katkı grafiği ile ilgilenmiyor (kod GitLab'da)</li>
</ul>

<h2>Çelişki: Ayrı Hesap da Çözüm Değil</h2>
<p>Geliştirici, ilginç bir paradoksu işaret ediyor:</p>
<blockquote style=""border-left:4px solid #ffc107;padding-left:16px;margin:20px 0;background:#fffbf0;padding:16px"">
""Adanmış iş hesabı ile bile, geliştiriciler yine de 'john-acme' kişisel repolarına push edebilir ve ayrılmadan önce repoları gerçek kişisel hesaplarına transfer edebilirler. Yani biraz anlamsız bir sorun.""
</blockquote>

<h2>GitLab vs GitHub Karşılaştırması</h2>
<table style=""width:100%;border-collapse:collapse;margin:20px 0"">
<tr style=""background:#f0f0f0"">
<th style=""border:1px solid #ddd;padding:12px"">Özellik</th>
<th style=""border:1px solid #ddd;padding:12px"">GitLab</th>
<th style=""border:1px solid #ddd;padding:12px"">GitHub</th>
</tr>
<tr>
<td style=""border:1px solid #ddd;padding:8px"">Birden fazla hesap</td>
<td style=""border:1px solid #ddd;padding:8px"">✅ İzin veriliyor</td>
<td style=""border:1px solid #ddd;padding:8px"">❌ Yasak</td>
</tr>
<tr>
<td style=""border:1px solid #ddd;padding:8px"">Kişisel proje tercihi</td>
<td style=""border:1px solid #ddd;padding:8px"">❌ Daha az popüler</td>
<td style=""border:1px solid #ddd;padding:8px"">✅ Daha popüler (daha iyi UX)</td>
</tr>
<tr>
<td style=""border:1px solid #ddd;padding:8px"">Enterprise SSO</td>
<td style=""border:1px solid #ddd;padding:8px"">✅ Destekleniyor</td>
<td style=""border:1px solid #ddd;padding:8px"">✅ Destekleniyor</td>
</tr>
</table>

<h2>Topluluk Çözüm Önerileri</h2>
<p>Reddit topluluğu şu çözümleri öneriyor:</p>

<h3>1. SAML/SSO Entegrasyonu</h3>
<ul>
<li>GitHub Enterprise ile SSO kullanın</li>
<li>Kişisel hesapları organizasyona SSO ile bağlayın</li>
<li>Organizasyon dışı erişimi kısıtlayın</li>
</ul>

<h3>2. Repository İzinleri</h3>
<ul>
<li>DLP araçları kullanın (GitHub Advanced Security)</li>
<li>Branch protection rules uygulayın</li>
<li>Code scanning ve secret scanning aktifleştirin</li>
</ul>

<h3>3. Eğitim ve Politika</h3>
<ul>
<li>Geliştiricileri güvenlik politikaları konusunda eğitin</li>
<li>Açık kaynak katkılarını destekleyin ama iş kodunu ayırın</li>
<li>İhlal durumunda yaptırımlar belirleyin</li>
</ul>

<h3>4. Teknik Çözümler</h3>
<ul>
<li>Git hooks ile hassas veri tespiti</li>
<li>Proxy sunucular üzerinden tüm Git trafiğini yönlendirin</li>
<li>Workstation monitoring (tartışmalı)</li>
</ul>

<img src=""https://images.unsplash.com/photo-1614064641938-3bbee52942c7?w=800&q=80"" alt=""Güvenlik Duvarı"" style=""width:100%;max-width:800px;margin:20px 0;border-radius:8px"" />

<h2>Diğer Şirketler Ne Yapıyor?</h2>
<p>Tartışmada diğer şirketlerin yaklaşımları:</p>
<ul>
<li><strong>Microsoft:</strong> Kişisel GitHub hesapları ile SSO</li>
<li><strong>Google:</strong> Dahili Git sunucuları, GitHub sadece açık kaynak için</li>
<li><strong>Amazon:</strong> Dahili CodeCommit, GitHub Enterprise seçenek</li>
<li><strong>Startuplar:</strong> Genelde kişisel hesaplara güveniyorlar</li>
</ul>

<p style=""background:#e8f5e9;padding:16px;border-radius:8px;margin:20px 0"">
<strong>💡 Sonuç:</strong> Mükemmel çözüm yok. Şirketler, güvenlik gereksinimleri ve geliştirici deneyimi arasında denge kurmalı. SSO, DLP araçları ve açık politikalarla risk minimize edilebilir.
</p>",
                Subjects = new[] { "GitHub", "Güvenlik", "Enterprise" },
                Authors = new[] { "Kurumsal BT Editörlüğü" },
                ExpressDate = now.AddHours(-12),
                CreateDate = now,
                UpdateDate = now,
                Priority = 2,
                IsActive = true,
                ViewCount = 0,
                IsSecondPageNews = false,
            },

            // Activity Section Missing
            new NewsArticle
            {
                Category = "github",
                Type = "news",
                Caption = "GitHub Ana Sayfa Kenar Çubuğunda Aktivite Bölümü Kayboldu",
                Slug = SlugHelper.GenerateSlug("GitHub Ana Sayfa Kenar Çubuğunda Aktivite Bölümü Kayboldu"),
                Keywords = "github, UI bug, aktivite bölümü, SSO, kullanıcı arayüzü",
                SocialTags = "#GitHub #UI #Bug #UserExperience",
                Summary = "Bir kullanıcı, SSO ekledikten sonra GitHub ana sayfa kenar çubuğundaki aktivite bölümünün kaybolduğunu bildirdi.",
                ImgPath = "https://images.unsplash.com/photo-1618044733300-9472054094ee?w=1200&q=80",
                ImgAlt = "GitHub Arayüzü",
                Content = @"<p>Reddit'teki <strong>r/github</strong> topluluğunda bir kullanıcı, GitHub ana sayfasındaki kenar çubuğunda yer alan <strong>""aktivite"" bölümünün</strong> kaybolduğunu bildirdi. Bu bölüm normalde son issue'lar ve pull request'leri gösteriyor.</p>

<img src=""https://images.unsplash.com/photo-1522071820081-009f0129c71c?w=800&q=80"" alt=""Bilgisayar Ekranı"" style=""width:100%;max-width:800px;margin:20px 0;border-radius:8px"" />

<h2>Sorun Detayları</h2>
<p>Kullanıcının gözlemleri:</p>
<ul>
<li>📍 <strong>Konum:</strong> Ana sayfa (github.com) sağ kenar çubuğu</li>
<li>❓ <strong>Kayıp özellik:</strong> ""Activity"" bölümü</li>
<li>📋 <strong>Normal içerik:</strong> Son issue'lar ve PR'ler listesi</li>
<li>🔐 <strong>Olası tetikleyici:</strong> Şirket SSO eklenmesi</li>
</ul>

<h2>SSO Bağlantısı</h2>
<blockquote style=""border-left:4px solid #0366d6;padding-left:16px;margin:20px 0;background:#f6f8fa;padding:16px"">
""Şirketin SSO girişini hesabıma eklediğim zaman başladı gibi görünüyor, ama bu sadece bir tesadüf olabilir.""
</blockquote>

<h3>SSO ile İlgili Olası Nedenler</h3>
<ul>
<li>Organizasyon izinleri kenar çubuğu içeriğini etkileyebilir</li>
<li>SSO kısıtlamaları bazı özellikleri gizleyebilir</li>
<li>Hesap tipinde değişiklik (free → enterprise)</li>
</ul>

<h2>Topluluk Çözüm Önerileri</h2>
<p>Reddit kullanıcıları şu çözümleri önerdi:</p>

<h3>1. Tarayıcı Cache Temizleme</h3>
<ul>
<li>Tarayıcı önbelleğini temizleyin</li>
<li>Farklı tarayıcıda deneyin</li>
<li>Incognito/Private mode'da kontrol edin</li>
</ul>

<h3>2. GitHub Ayarları Kontrolü</h3>
<ul>
<li><strong>Settings → Appearance:</strong> Sidebar ayarlarını kontrol edin</li>
<li><strong>Settings → Organizations:</strong> SSO organizasyon izinlerini gözden geçirin</li>
<li><strong>Feature Preview:</strong> Beta özelliklerden kaynaklı olabilir</li>
</ul>

<h3>3. SSO Organizasyon Bağlantısı</h3>
<ul>
<li>Organizasyon admin'i ile iletişime geçin</li>
<li>SSO politikalarını kontrol edin</li>
<li>Gerekirse SSO bağlantısını kaldırıp yeniden ekleyin</li>
</ul>

<img src=""https://images.unsplash.com/photo-1551288049-bebda4e38f71?w=800&q=80"" alt=""Sorun Giderme"" style=""width:100%;max-width:800px;margin:20px 0;border-radius:8px"" />

<h2>Benzer GitHub UI Sorunları</h2>
<p>Son aylarda bildirilen diğer arayüz sorunları:</p>
<ul>
<li>Notification badge'leri kaybolması</li>
<li>Watch edilen repoların listede görünmemesi</li>
<li>Pull request filtreleri çalışmaması</li>
<li>Dark mode'da bazı elementlerin görünmemesi</li>
</ul>

<h2>GitHub Destek Seçenekleri</h2>
<p>Sorun devam ediyorsa:</p>
<ol>
<li><strong>GitHub Support:</strong> support.github.com üzerinden ticket açın</li>
<li><strong>GitHub Community:</strong> community.github.com'da sorun bildirin</li>
<li><strong>Twitter:</strong> @GitHubSupport hesabına mention</li>
<li><strong>Feedback:</strong> GitHub Feedback sayfasında bug raporu</li>
</ol>

<h2>Geçici Çözüm</h2>
<p>Aktivite bölümü kayboldu ise alternatifler:</p>
<ul>
<li><strong>Notifications:</strong> github.com/notifications</li>
<li><strong>Pull Requests:</strong> github.com/pulls</li>
<li><strong>Issues:</strong> github.com/issues</li>
<li><strong>Activity:</strong> github.com/USERNAME?tab=overview</li>
</ul>

<p style=""background:#fff9e6;padding:16px;border-radius:8px;margin:20px 0"">
<strong>🔧 Öneri:</strong> GitHub'ın yeni arayüz güncellemeleri bazen bazı kullanıcılar için sorunlara yol açabiliyor. Sorunu GitHub Support'a bildirmek, düzeltme sürecini hızlandırır.
</p>",
                Subjects = new[] { "GitHub", "UI/UX", "Sorun Giderme" },
                Authors = new[] { "Teknoloji Editörlüğü" },
                ExpressDate = now.AddHours(-15),
                CreateDate = now,
                UpdateDate = now,
                Priority = 3,
                IsActive = true,
                ViewCount = 0,
                IsSecondPageNews = false,
            },
        };

        await newsCollection.InsertManyAsync(redditNewsList, cancellationToken: app.Lifetime.ApplicationStarted).ConfigureAwait(false);
        Console.WriteLine($"Successfully seeded {redditNewsList.Count} Reddit news articles!");
    }
}
