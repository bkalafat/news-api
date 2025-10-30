# Reddit News Seed Generator Prompt

## Overview
This prompt is designed to help generate seed data for the news portal by scraping Reddit posts from technology and GitHub-related subreddits, converting them into properly formatted news articles.

## Objective
Convert top Reddit posts from the last month into Turkish news articles with proper formatting, images, and metadata suitable for the NewsApi database.

## Target Subreddits

### Primary Technology Subreddits
1. **r/github** - GitHub-specific discussions, Copilot news, enterprise issues
2. **r/technology** - General technology news and innovations
3. **r/programming** - Development tools, languages, best practices
4. **r/webdev** - Web development trends and tools
5. **r/MachineLearning** - AI/ML advances and applications
6. **r/artificial** - Artificial Intelligence discussions
7. **r/devops** - DevOps practices and tools
8. **r/coding** - General coding discussions

### GitHub Copilot Specific
- **r/github** - Filter posts containing "copilot", "AI", "code generation"
- **r/webdev** - AI coding tools discussions
- **r/programming** - Copilot experiences and comparisons

## Reddit API Parameters

### Time Filter
```bash
t=month  # Last 1 month (options: hour, day, week, month, year, all)
```

### Sorting
```bash
sort=top  # Top posts by upvotes
limit=25  # Number of posts to fetch
```

### API Endpoint Examples

**Get top posts from last month:**
```bash
curl -H "Authorization: Bearer YOUR_OAUTH_TOKEN" \
     "https://oauth.reddit.com/r/github/top?t=month&limit=25"
```

**Search GitHub Copilot posts:**
```bash
curl -H "Authorization: Bearer YOUR_OAUTH_TOKEN" \
     "https://oauth.reddit.com/r/github/search?q=copilot&restrict_sr=true&sort=top&t=month&limit=25"
```

**Get r/technology top posts:**
```bash
curl -H "Authorization: Bearer YOUR_OAUTH_TOKEN" \
     "https://oauth.reddit.com/r/technology/top?t=month&limit=25"
```

## Post Selection Criteria

### Priority Scoring (1-100)
- **70-100**: Highly upvoted (100+ upvotes), active discussion, trending topics
- **40-69**: Medium engagement (25-99 upvotes), relevant technical content
- **20-39**: Low engagement but quality content, niche topics

### Inclusion Criteria
- ✅ Minimum 10 upvotes
- ✅ At least 5 comments (indicates discussion value)
- ✅ Posted within last 30 days
- ✅ Original content (not cross-posts unless highly relevant)
- ✅ English language (will be translated to Turkish)
- ✅ Contains useful information, news, or insights

### Exclusion Criteria
- ❌ Spam or promotional posts
- ❌ Low-quality memes (unless highly relevant)
- ❌ Personal rants without constructive content
- ❌ Duplicate topics (combine similar posts)
- ❌ NSFW content
- ❌ Copyright violations

## Content Transformation Process

### Step 1: Extract Data from Reddit
From each Reddit post, extract:
- **Title**: Post title (will become Caption)
- **Content**: Post body/description
- **Author**: Reddit username
- **PostUrl**: Direct link to Reddit post
- **ImageUrls**: Attached images or preview images
- **Upvotes**: Reddit score
- **CommentCount**: Number of comments
- **PostedAt**: Original post timestamp
- **Category**: Subreddit name or relevant category

### Step 2: Translate to Turkish
- **Caption**: Translate Reddit title to Turkish, keep concise (max 100 chars)
- **Summary**: Create Turkish summary (150-200 chars)
- **Content**: Translate and expand post content to Turkish, add context
- **Keywords**: Generate Turkish keywords based on content

### Step 3: Generate Slug
Use `SlugHelper.GenerateSlug()` for Turkish-friendly URLs:
```csharp
Slug = SlugHelper.GenerateSlug("GitHub Copilot Kullanımı Artıyor");
// Output: "github-copilot-kullanimi-artiyor"
```

### Step 4: Find Appropriate Images

#### Image Sources Priority
1. **Reddit Post Images**: Use post's image URLs if available
2. **Unsplash**: Search for relevant free images
   - Keywords: "technology", "coding", "ai", "github", "programming"
   - Example: `https://images.unsplash.com/photo-{id}?w=1200&q=80`
3. **MinIO Storage**: Upload custom images to MinIO
   - Endpoint: `http://localhost:9000/news-images/{category}/{filename}`

#### Unsplash Search Examples
```
Technology: https://images.unsplash.com/photo-1677442136019-21780ecad995?w=1200&q=80
AI/ML: https://images.unsplash.com/photo-1555255707-c07966088b7b?w=1200&q=80
Coding: https://images.unsplash.com/photo-1461749280684-dccba630e2f6?w=1200&q=80
GitHub: https://images.unsplash.com/photo-1618401471353-b98afee0b2eb?w=1200&q=80
Quantum Computing: https://images.unsplash.com/photo-1635070041078-e363dbe005cb?w=1200&q=80
Cybersecurity: https://images.unsplash.com/photo-1563986768609-322da13575f3?w=1200&q=80
```

#### Image Requirements
- Format: JPG, PNG, WebP
- Resolution: Min 800x600, recommended 1200x800
- Max file size: 5MB
- Alt text: Turkish description of image content

### Step 5: Categorization

#### Category Mapping
- **r/github** → `"github"` or `"technology"`
- **r/programming** → `"technology"` or `"reddit"`
- **r/webdev** → `"technology"` or `"reddit"`
- **r/technology** → `"technology"`
- **r/MachineLearning** → `"technology"` or `"reddit"`

#### Type Classification
- `"news"` - Announcements, product launches, breaking news
- `"article"` - Opinion pieces, tutorials, deep dives
- `"discussion"` - Community discussions, debates

### Step 6: Metadata Generation

#### Keywords (Turkish)
Generate 5-7 relevant keywords in Turkish:
- GitHub Copilot post → "github, copilot, yapay zeka, kod geliştirme, AI"
- Web development → "web geliştirme, frontend, React, JavaScript, UI/UX"

#### Social Tags (Turkish + English)
Create hashtags for social media:
```
#GitHub #Copilot #YapayZeka #AI #KodGeliştirme
```

#### Subjects
Categorize with 2-4 subject tags:
```csharp
Subjects = new[] { "Yapay Zeka", "Yazılım Geliştirme", "GitHub" }
```

#### Authors
If identifiable, use Reddit username, otherwise use generic:
```csharp
Authors = new[] { "Teknoloji Editörlüğü" }
```

## Content Formatting Guidelines

### HTML Structure
Use rich HTML with proper formatting:

```html
<p>Opening paragraph with <strong>bold</strong> and <em>italic</em> emphasis.</p>

<img src="https://images.unsplash.com/photo-xxx?w=800&q=80" alt="Descriptive Alt Text" 
     style="width:100%;max-width:800px;margin:20px 0;border-radius:8px" />

<h2>Section Heading</h2>
<p>Section content...</p>

<ul>
<li>Bullet point 1</li>
<li>Bullet point 2</li>
</ul>

<blockquote style="border-left:4px solid #0066cc;padding-left:16px;margin:20px 0;font-style:italic">
"Quotable text or important statement"
</blockquote>

<h3>Subsection</h3>
<ol>
<li>Numbered item 1</li>
<li>Numbered item 2</li>
</ol>

<table style="width:100%;border-collapse:collapse;margin:20px 0">
<tr style="background:#f0f0f0">
<th style="border:1px solid #ddd;padding:12px">Header 1</th>
<th style="border:1px solid #ddd;padding:12px">Header 2</th>
</tr>
<tr>
<td style="border:1px solid #ddd;padding:8px">Data 1</td>
<td style="border:1px solid #ddd;padding:8px">Data 2</td>
</tr>
</table>
```

### Content Length
- **Minimum**: 500 words (for substantial posts)
- **Ideal**: 800-1200 words (comprehensive coverage)
- **Maximum**: 2000 words (very detailed posts)

### Turkish Writing Style
- Use formal but accessible Turkish
- Avoid jargon when possible, explain technical terms
- Use active voice
- Break long sentences into shorter ones
- Include context for international news

## Priority Calculation Formula

```csharp
Priority = CalculatePriority(upvotes, commentCount, recency);

// Formula:
// - Upvotes > 100: Priority 1 (High)
// - Upvotes 50-99: Priority 2 (Medium)
// - Upvotes 25-49: Priority 3 (Normal)
// - Upvotes < 25: Priority 4 (Low)

// Adjust based on:
// - Recency: Posts < 7 days get +1 priority boost
// - Engagement: CommentCount > 50 gets +1 priority boost
// - Keywords: Contains "breaking", "announcement", "launch" gets +1 boost
```

## Example News Article Structure

```csharp
new NewsArticle
{
    Category = "github",
    Type = "news",
    Caption = "GitHub Copilot Pro Ücretsiz Erişim Kaybı Sorunu",
    Slug = SlugHelper.GenerateSlug("GitHub Copilot Pro Ücretsiz Erişim Kaybı Sorunu"),
    Keywords = "github copilot, ücretsiz erişim, açık kaynak, AI kod geliştirme",
    SocialTags = "#GitHub #Copilot #OpenSource #YapayZeka",
    Summary = "Açık kaynak geliştiricileri, GitHub Copilot Pro'nun ücretsiz erişimini kaybetme konusunda endişelerini dile getiriyor.",
    ImgPath = "https://images.unsplash.com/photo-1618401471353-b98afee0b2eb?w=1200&q=80",
    ImgAlt = "GitHub Copilot Logosu",
    Content = @"<p>Reddit'teki <strong>r/github</strong> topluluğunda, GitHub Copilot Pro'ya ücretsiz erişim alan açık kaynak geliştiricileri arasında bu erişimin kaybedilmesi konusunda tartışmalar yükseldi...</p>
    
    <img src=""https://images.unsplash.com/photo-1618401471353-b98afee0b2eb?w=800&q=80"" alt=""GitHub Copilot Interface"" style=""width:100%;max-width:800px;margin:20px 0;border-radius:8px"" />
    
    <h2>Sorun Ne?</h2>
    <p>GitHub, belirli açık kaynak projeleri için Copilot Pro'yu ücretsiz sunuyor. Ancak kullanıcılar...</p>
    
    <ul>
    <li>Aylık uygunluk kontrollerinin belirsizliği</li>
    <li>Erişim kaybı bildirimleri</li>
    <li>Yeniden başvuru süreci</li>
    </ul>
    
    <blockquote style=""border-left:4px solid #0066cc;padding-left:16px;margin:20px 0"">
    ""GitHub Copilot Pro için ücretsiz erişime güvenemiyorum. Her ay erişimin devam edip etmeyeceğini bilmiyorum."" - Reddit kullanıcısı
    </blockquote>
    
    <h2>GitHub'ın Açıklaması</h2>
    <p>GitHub, uygunluk kriterlerini açıkça belirtmemekle eleştiriliyor...</p>",
    Subjects = new[] { "GitHub", "Copilot", "Açık Kaynak" },
    Authors = new[] { "Teknoloji Editörlüğü" },
    ExpressDate = DateTime.UtcNow.AddHours(-2),
    CreateDate = DateTime.UtcNow.AddHours(-2),
    UpdateDate = DateTime.UtcNow.AddHours(-2),
    Priority = 2,
    IsActive = true,
    ViewCount = 0,
    IsSecondPageNews = false,
}
```

## Batch Processing Strategy

### Daily Workflow
1. **Fetch**: Get top 25 posts from each target subreddit (t=day)
2. **Filter**: Apply selection criteria, remove low-quality posts
3. **Translate**: Convert to Turkish, maintain technical accuracy
4. **Enhance**: Add context, images, proper formatting
5. **Review**: Quality check for translation, relevance, formatting
6. **Import**: Insert into MongoDB via SeedController

### Weekly Review (Recommended)
- Fetch: Top posts from last week (t=week)
- Select: 10-15 best posts per subreddit
- Deep dive: More comprehensive articles with research

### Monthly Compilation
- Fetch: Top posts from last month (t=month)
- Select: Top 5 posts per subreddit
- Create: In-depth feature articles, trend analysis

## Implementation in C# (.NET)

### Create New Seed Method

```csharp
public static async Task SeedRedditNewsAsync(MongoDbContext context)
{
    var newsCollection = context.News;
    var redditNewsList = new List<NewsArticle>();

    // Example: Convert socialmedia_export.json Reddit posts to NewsArticle
    // This would typically fetch from Reddit API in production
    
    var redditPosts = await FetchRedditPostsAsync("github", "month", 25);
    
    foreach (var post in redditPosts)
    {
        if (post.Upvotes < 10) continue; // Skip low-engagement posts
        
        var newsArticle = new NewsArticle
        {
            Category = DetermineCategoryFromSubreddit(post.Subreddit),
            Type = "news",
            Caption = TranslateToTurkish(post.Title),
            Slug = SlugHelper.GenerateSlug(TranslateToTurkish(post.Title)),
            Keywords = GenerateKeywords(post),
            SocialTags = GenerateSocialTags(post),
            Summary = GenerateSummary(post),
            ImgPath = SelectBestImage(post) ?? GetUnsplashImage(post.Category),
            ImgAlt = GenerateAltText(post),
            Content = GenerateHTMLContent(post),
            Subjects = ExtractSubjects(post),
            Authors = new[] { "Reddit Editörlüğü" },
            ExpressDate = post.PostedAt,
            CreateDate = DateTime.UtcNow,
            UpdateDate = DateTime.UtcNow,
            Priority = CalculatePriority(post),
            IsActive = true,
            ViewCount = 0,
            IsSecondPageNews = false,
        };
        
        redditNewsList.Add(newsArticle);
    }

    await newsCollection.InsertManyAsync(redditNewsList);
    Console.WriteLine($"Seeded {redditNewsList.Count} Reddit news articles!");
}
```

## Quality Assurance Checklist

Before importing news articles:
- [ ] Turkish translation is accurate and natural
- [ ] All technical terms are correctly translated or explained
- [ ] Images are high-quality and relevant
- [ ] HTML formatting is correct and renders well
- [ ] Slug is Turkish-character safe (no ı, ğ, ü, ş, ö, ç)
- [ ] Keywords are relevant and in Turkish
- [ ] Social tags include both Turkish and English
- [ ] Content length is substantial (500+ words)
- [ ] Priority is correctly calculated
- [ ] Authors field is populated
- [ ] Category matches content
- [ ] ImgAlt provides good accessibility description

## Resources

### Reddit API Documentation
- Official API: https://www.reddit.com/dev/api/
- OAuth2 Guide: https://github.com/reddit-archive/reddit/wiki/OAuth2
- Rate Limits: 60 requests per minute (with OAuth)

### Translation Tools
- DeepL API: High-quality Turkish translation
- Google Translate API: Fallback option
- Manual review: Recommended for technical content

### Image Resources
- Unsplash API: https://unsplash.com/developers
- Pexels API: https://www.pexels.com/api/
- MinIO: Local object storage

### Turkish Text Processing
- SlugHelper: `newsportal/backend/Common/SlugHelper.cs`
- Converts: ı→i, ğ→g, ü→u, ş→s, ö→o, ç→c
- Removes special characters, max 100 chars

## Notes

- Always respect Reddit's API terms of service
- Attribute content to original Reddit post (include PostUrl)
- Update ViewCount based on actual engagement
- Monitor Priority accuracy and adjust formula as needed
- Regularly review and update Unsplash image pool
- Consider legal implications of content scraping and republishing
- Ensure GDPR compliance if storing user data (Reddit usernames)

## Version History

- **v1.0** (2025-10-31): Initial prompt creation
- Future: Add automated translation pipeline
- Future: Implement Reddit OAuth authentication
- Future: Add sentiment analysis for priority calculation
- Future: Create dashboard for manual review and approval
