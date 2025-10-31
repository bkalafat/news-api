using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using NewsApi.Application.Services;
using NewsApi.Common;
using NewsApi.Domain.Entities;
using NewsApi.Infrastructure.Data;
using NewsApi.Infrastructure.Services;

namespace NewsApi.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class SeedController : ControllerBase
{
    private readonly MongoDbContext _context;
    private readonly ILogger<SeedController> _logger;
    private readonly INewsDataFetcherService _newsDataFetcher;
    private readonly INewsArticleService _newsService;
    private readonly NewsAggregatorService _newsAggregator;
    private readonly TranslationService _translationService;
    private readonly IMemoryCache _cache;
    private readonly IConfiguration _configuration;

    public SeedController(
        MongoDbContext context,
        ILogger<SeedController> logger,
        INewsDataFetcherService newsDataFetcher,
        INewsArticleService newsService,
        NewsAggregatorService newsAggregator,
        TranslationService translationService,
        IMemoryCache cache,
        IConfiguration configuration)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _newsDataFetcher = newsDataFetcher ?? throw new ArgumentNullException(nameof(newsDataFetcher));
        _newsService = newsService ?? throw new ArgumentNullException(nameof(newsService));
        _newsAggregator = newsAggregator ?? throw new ArgumentNullException(nameof(newsAggregator));
        _translationService = translationService ?? throw new ArgumentNullException(nameof(translationService));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    /// <summary>
    /// Seeds the database with sample news articles
    /// </summary>
    /// <returns>Result of seeding operation</returns>
    [HttpPost("news")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SeedNews()
    {
        try
        {
            _logger.LogInformation("Starting database seeding...");
            await SeedNewsData.SeedAsync(_context);
            _logger.LogInformation("Database seeding completed successfully!");

            return Ok(new { message = "Database seeded successfully with news articles!" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while seeding database");
            return StatusCode(500, new { message = "Error seeding database", error = ex.Message });
        }
    }

    /// <summary>
    /// Seeds the database with Reddit posts converted to Turkish news articles
    /// Focuses on technology subreddits and GitHub Copilot discussions
    /// </summary>
    /// <returns>Result of Reddit seeding operation</returns>
    [HttpPost("reddit")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SeedRedditNews()
    {
        try
        {
            _logger.LogInformation("Starting Reddit news seeding...");
            await SeedRedditNewsData.SeedAsync(_context);
            _logger.LogInformation("Reddit news seeding completed successfully!");

            return Ok(new { message = "Database seeded successfully with Reddit news articles!" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while seeding Reddit news");
            return StatusCode(500, new { message = "Error seeding Reddit news", error = ex.Message });
        }
    }

    /// <summary>
    /// Manually triggers the daily news fetching process from external NewsAPI.org
    /// This endpoint allows testing the automatic seed functionality without waiting for scheduled time
    /// </summary>
    /// <returns>Result of news fetching and seeding operation</returns>
    [HttpPost("fetch-external-news")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> FetchExternalNews()
    {
        try
        {
            _logger.LogInformation("Manually triggering external news fetch...");

            var startTime = DateTime.UtcNow;
            int totalFetched = 0;
            int totalCreated = 0;
            int totalSkipped = 0;
            int totalErrors = 0;

            // Fetch news from external API
            var newsArticles = await _newsDataFetcher.FetchLatestNewsAsync(CancellationToken.None);
            totalFetched = newsArticles.Count;

            _logger.LogInformation("Fetched {Count} news articles from external sources", totalFetched);

            if (totalFetched == 0)
            {
                return Ok(new
                {
                    message = "No news articles were fetched. Check API configuration and limits.",
                    fetched = 0,
                    created = 0,
                    skipped = 0,
                    errors = 0,
                });
            }

            // Save each article to database
            foreach (var article in newsArticles)
            {
                try
                {
                    // Check if article with similar caption already exists (avoid duplicates)
                    var existingArticles = await _newsService.GetAllNewsAsync();

                    var isDuplicate = existingArticles
                        .Where(e => string.Equals(e.Category, article.Category, StringComparison.Ordinal))
                        .Any(existing => existing.Caption.Equals(article.Caption, StringComparison.OrdinalIgnoreCase));

                    if (isDuplicate)
                    {
                        totalSkipped++;
                        _logger.LogDebug("Skipped duplicate article: {Caption}", article.Caption);
                        continue;
                    }

                    // Create the news article (convert DTO to entity)
                    var newsEntity = new NewsArticle
                    {
                        Category = article.Category,
                        Type = article.Type,
                        Caption = article.Caption,
                        Keywords = article.Keywords,
                        SocialTags = article.SocialTags,
                        Summary = article.Summary,
                        ImgPath = article.ImgPath,
                        ImgAlt = article.ImgAlt,
                        ImageUrl = article.ImageUrl,
                        ThumbnailUrl = article.ThumbnailUrl,
                        Content = article.Content,
                        Subjects = article.Subjects,
                        Authors = article.Authors,
                        ExpressDate = article.ExpressDate,
                        Priority = article.Priority,
                        IsActive = article.IsActive,
                        IsSecondPageNews = article.IsSecondPageNews,
                    };
                    await _newsService.CreateNewsAsync(newsEntity);
                    totalCreated++;

                    _logger.LogDebug(
                        "Created article: {Caption} in {Category}",
                        article.Caption, article.Category);
                }
                catch (Exception ex)
                {
                    totalErrors++;
                    _logger.LogWarning(ex, "Failed to create article: {Caption}", article.Caption);
                }
            }

            var endTime = DateTime.UtcNow;
            var duration = (endTime - startTime).TotalSeconds;

            _logger.LogInformation(
                "External news fetch completed: {Created} created, {Skipped} skipped, {Errors} errors",
                totalCreated, totalSkipped, totalErrors);

            return Ok(new
            {
                message = "External news fetch completed successfully!",
                durationSeconds = Math.Round(duration, 2),
                fetched = totalFetched,
                created = totalCreated,
                skipped = totalSkipped,
                errors = totalErrors,
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching external news");
            return StatusCode(500, new { message = "Error fetching external news", error = ex.Message });
        }
    }

    /// <summary>
    /// Manually triggers the daily news aggregation from free sources (Reddit, GitHub, RSS feeds)
    /// Fetches AI/tech news, translates to Turkish, and publishes them
    /// This endpoint allows testing the automated daily job without waiting until 5 AM
    /// </summary>
    /// <returns>Result of news aggregation, translation, and publishing operation</returns>
    [HttpPost("aggregate-and-publish")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AggregateAndPublishNews()
    {
        try
        {
            _logger.LogInformation("Manually triggering news aggregation and publishing...");

            var startTime = DateTime.UtcNow;
            int totalFetched = 0;
            int totalCreated = 0;
            int totalSkipped = 0;
            int totalErrors = 0;

            // 1. Fetch all news from various sources
            _logger.LogInformation("Fetching news from all sources...");
            var aggregatedNews = await _newsAggregator.FetchAllNewsAsync();
            totalFetched = aggregatedNews.Count;

            _logger.LogInformation("Fetched {Count} news items from all sources", totalFetched);

            if (totalFetched == 0)
            {
                return Ok(new
                {
                    message = "No news articles were fetched from any source.",
                    fetched = 0,
                    created = 0,
                    skipped = 0,
                    errors = 0,
                });
            }

            // 2. Filter and prioritize (top 50 most relevant)
            var topNews = aggregatedNews
                .OrderByDescending(n => n.Score)
                .ThenByDescending(n => n.PublishedDate)
                .Take(50)
                .ToList();

            _logger.LogInformation("Processing top {Count} news items", topNews.Count);

            // 3. Translate and save each news item
            foreach (var item in topNews)
            {
                try
                {
                    // Check if already exists (by title slug)
                    var slug = SlugHelper.GenerateSlug(item.Title);
                    var existing = await _newsService.GetNewsBySlugAsync(slug);

                    if (existing != null)
                    {
                        totalSkipped++;
                        _logger.LogDebug("Skipping duplicate: {Title}", item.Title);
                        continue;
                    }

                    // Detect language
                    var sourceLanguage = _translationService.DetectLanguage(item.Title);

                    // Translate to Turkish if not already Turkish
                    string translatedTitle = item.Title;
                    string translatedContent = item.Content;
                    string translatedSummary = item.Content;

                    if (sourceLanguage != "tr")
                    {
                        _logger.LogInformation("Translating: {Title}", item.Title);

                        translatedTitle = await _translationService.TranslateToTurkishAsync(item.Title, sourceLanguage);

                        // Create summary from content (first 200 chars)
                        var summary = item.Content.Length > 200
                            ? item.Content[..200] + "..."
                            : item.Content;

                        translatedSummary = await _translationService.TranslateToTurkishAsync(summary, sourceLanguage);

                        // Only translate full content if it's not too long
                        if (item.Content.Length > 0 && item.Content.Length < 2000)
                        {
                            translatedContent = await _translationService.TranslateToTurkishAsync(item.Content, sourceLanguage);
                        }
                        else if (item.Content.Length >= 2000)
                        {
                            translatedContent = $"[Orijinal kaynak: {item.Source}]\n\n{item.Content}";
                        }

                        // Rate limiting - wait between translations
                        await Task.Delay(500);
                    }

                    // 4. Create news entity
                    var news = new NewsArticle
                    {
                        Id = ObjectId.GenerateNewId().ToString(),
                        Category = item.Category,
                        Type = "article",
                        Caption = translatedTitle,
                        Slug = slug,
                        Keywords = string.Join(", ", item.Tags.Take(10)),
                        SocialTags = string.Join(" ", item.Tags.Select(t => $"#{t}").Take(5)),
                        Summary = translatedSummary,
                        ImgPath = string.Empty,
                        ImgAlt = $"Image from {item.Source}",
                        ImageUrl = string.Empty,
                        ThumbnailUrl = string.Empty,
                        Content = translatedContent,
                        ExpressDate = item.PublishedDate,
                        CreateDate = DateTime.UtcNow,
                        UpdateDate = DateTime.UtcNow,
                        Priority = CalculatePriority(item.Score, item.Source),
                        IsActive = true,
                        ViewCount = 0,
                        IsSecondPageNews = false,
                        ImageMetadata = null,
                        Subjects = Array.Empty<string>(),
                        Authors = new[] { item.Author },
                    };

                    // 5. Save to database
                    await _newsService.CreateNewsAsync(news);
                    totalCreated++;

                    _logger.LogInformation(
                        "Published: [{Category}] {Title} (Source: {Source}, Score: {Score})",
                        news.Category,
                        news.Caption,
                        item.Source,
                        item.Score);
                }
                catch (Exception ex)
                {
                    totalErrors++;
                    _logger.LogError(ex, "Failed to process news item: {Title}", item.Title);
                }
            }

            // 6. Clear all caches
            _logger.LogInformation("Clearing caches...");
            ClearAllCaches();

            var endTime = DateTime.UtcNow;
            var duration = (endTime - startTime).TotalSeconds;

            _logger.LogInformation(
                "News aggregation complete: {Created} published, {Skipped} skipped, {Errors} failed",
                totalCreated,
                totalSkipped,
                totalErrors);

            return Ok(new
            {
                message = "News aggregation and publishing completed successfully!",
                durationSeconds = Math.Round(duration, 2),
                fetched = totalFetched,
                created = totalCreated,
                skipped = totalSkipped,
                errors = totalErrors,
                sources = new[]
                {
                    "Reddit (AI/Tech subreddits)",
                    "GitHub Trending",
                    "Hacker News",
                    "Dev.to",
                    "Medium",
                    "Ars Technica",
                    "TechCrunch"
                },
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while aggregating news");
            return StatusCode(500, new { message = "Error aggregating news", error = ex.Message });
        }
    }

    /// <summary>
    /// Manually triggers Reddit news aggregation from configured subreddits
    /// Fetches from 9 AI/Tech subreddits, translates to Turkish, and saves as news articles
    /// </summary>
    /// <returns>Result of Reddit aggregation operation</returns>
    [HttpPost("fetch-reddit-news")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> FetchRedditNews()
    {
        try
        {
            _logger.LogInformation("Manually triggering Reddit news aggregation with Turkish translation...");

            var redditService = HttpContext.RequestServices.GetRequiredService<RedditService>();
            var newsRepository = HttpContext.RequestServices.GetRequiredService<NewsApi.Domain.Interfaces.INewsArticleRepository>();

            var subreddits = new Dictionary<string, string>
            {
                ["popular"] = "popular",
                ["ArtificialIntelligence"] = "artificialintelligence",
                ["GithubCopilot"] = "githubcopilot",
                ["mcp"] = "mcp",
                ["OpenAI"] = "openai",
                ["robotics"] = "robotics",
                ["DeepSeek"] = "deepseek",
                ["dotnet"] = "dotnet",
                ["ClaudeAI"] = "claudeai",
            };

            int totalFetched = 0;
            int totalSaved = 0;

            foreach (var (subreddit, category) in subreddits)
            {
                try
                {
                    var posts = await redditService.GetTopPostsAsync(
                        subreddit,
                        timeframe: "week",
                        limit: subreddit == "popular" ? 10 : 15
                    );

                    totalFetched += posts.Count;

                    foreach (var post in posts)
                    {
                        var slug = SlugHelper.GenerateSlug($"{subreddit}: {post.Title}");
                        var existing = await newsRepository.GetBySlugAsync(slug);

                        if (existing == null)
                        {
                            // Detect language and translate to Turkish
                            var sourceLanguage = _translationService.DetectLanguage(post.Title);
                            string translatedTitle = post.Title;
                            string translatedSummary = post.Content?.Length > 200 ? post.Content[..200] : post.Content ?? post.Title;
                            string translatedContent = post.Content ?? post.Title;

                            // Translate if content is in English
                            if (sourceLanguage == "en")
                            {
                                try
                                {
                                    translatedTitle = await _translationService.TranslateToTurkishAsync(post.Title, sourceLanguage);
                                    _logger.LogDebug("Translated title: {Original} -> {Translated}", post.Title, translatedTitle);

                                    var summaryText = post.Content?.Length > 200 ? post.Content[..200] : post.Content ?? post.Title;
                                    translatedSummary = await _translationService.TranslateToTurkishAsync(summaryText, sourceLanguage);

                                    if (!string.IsNullOrEmpty(post.Content) && post.Content.Length > 50)
                                    {
                                        translatedContent = await _translationService.TranslateToTurkishAsync(post.Content, sourceLanguage);
                                    }
                                }
                                catch (Exception transEx)
                                {
                                    _logger.LogWarning(transEx, "Translation failed for post, using original text");
                                }
                            }

                            var article = new NewsArticle
                            {
                                Category = category,
                                Type = "reddit",
                                Caption = translatedTitle,
                                Slug = slug,
                                Summary = translatedSummary + "...",
                                Content = translatedContent,
                                ImgPath = post.ImageUrls?.FirstOrDefault() ?? string.Empty,
                                ImgAlt = translatedTitle,
                                ExpressDate = post.PostedAt,
                                CreateDate = DateTime.UtcNow,
                                UpdateDate = DateTime.UtcNow,
                                Priority = Math.Min(post.Upvotes / 10, 100),
                                IsActive = true,
                                ViewCount = 0,
                            };

                            await newsRepository.CreateAsync(article);
                            totalSaved++;
                            _logger.LogInformation("Saved Turkish article: {Title}", translatedTitle);
                        }
                    }

                    _logger.LogInformation("Processed {Count} posts from r/{Subreddit}", posts.Count, subreddit);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error fetching from r/{Subreddit}", subreddit);
                }
            }

            // Clear caches
            ClearAllCaches();

            // Trigger ISR revalidation on Netlify
            await TriggerNetlifyRevalidationAsync();

            return Ok(new
            {
                message = "Reddit news aggregation completed successfully with Turkish translation!",
                fetched = totalFetched,
                saved = totalSaved,
                subreddits = subreddits.Keys.ToArray(),
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching Reddit news");
            return StatusCode(500, new { message = "Error fetching Reddit news", error = ex.Message });
        }
    }

    private async Task TriggerNetlifyRevalidationAsync()
    {
        try
        {
            var webhookUrl = _configuration["NetlifySettings:RevalidateWebhookUrl"];
            var isEnabled = _configuration.GetValue<bool>("NetlifySettings:EnableAutoRevalidation", true);

            if (string.IsNullOrEmpty(webhookUrl) || !isEnabled || webhookUrl.Contains("YOUR_BUILD_HOOK_ID"))
            {
                _logger.LogInformation("Netlify revalidation webhook not configured or disabled");
                return;
            }

            using var httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(10);

            var response = await httpClient.PostAsync(webhookUrl, null);
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("✅ Netlify ISR revalidation triggered successfully");
            }
            else
            {
                _logger.LogWarning("⚠️ Netlify revalidation failed with status: {Status}", response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to trigger Netlify revalidation (non-critical)");
        }
    }

    private void ClearAllCaches()
    {
        try
        {
            // Clear all news-related caches
            _cache.Remove(CacheKeys.NewsList);
            _cache.Remove(CacheKeys.AllNews);

            // Clear old category caches
            var oldCategories = new[] { "Technology", "Science", "Business", "Sports", "Entertainment", "Health", "World" };
            foreach (var category in oldCategories)
            {
                _cache.Remove(CacheKeys.GetNewsByCategory(category));
            }

            // Clear Reddit category caches
            var redditCategories = new[] { "popular", "artificialintelligence", "githubcopilot", "mcp", "openai", "robotics", "deepseek", "dotnet", "claudeai" };
            foreach (var category in redditCategories)
            {
                _cache.Remove(CacheKeys.GetNewsByCategory(category));
            }

            // Clear type caches
            var types = new[] { "article", "breaking", "analysis", "reddit" };
            foreach (var type in types)
            {
                _cache.Remove(CacheKeys.GetNewsByType(type));
            }

            _logger.LogInformation("All news caches cleared");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing caches");
        }
    }

    /// <summary>
    /// Cleans up low-quality articles from the database
    /// Removes articles with:
    /// - Short content (less than 500 characters)
    /// - Non-Turkish content (detected by language)
    /// - Low engagement Reddit posts
    /// </summary>
    /// <returns>Result of cleanup operation with count of deleted articles</returns>
    [HttpPost("cleanup-low-quality")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CleanupLowQualityNews()
    {
        try
        {
            _logger.LogInformation("Starting low-quality news cleanup...");

            // Clear cache first to ensure we're working with fresh data from database
            ClearAllCaches();

            var allNews = await _newsService.GetAllNewsAsync();
            int totalChecked = allNews.Count;
            int totalDeleted = 0;
            var deletionReasons = new Dictionary<string, int>
            {
                ["ShortContent"] = 0,
                ["NonTurkish"] = 0,
                ["LowQuality"] = 0,
                ["NoImage"] = 0
            };

            foreach (var news in allNews)
            {
                bool shouldDelete = false;
                string reason = string.Empty;

                // 1. Check if article has an image (REQUIRED - highest priority)
                if (string.IsNullOrWhiteSpace(news.ImageUrl) && string.IsNullOrWhiteSpace(news.ImgPath))
                {
                    shouldDelete = true;
                    reason = "NoImage";
                    deletionReasons["NoImage"]++;
                }

                // 2. Check content length (minimum 500 chars)
                if (!shouldDelete)
                {
                    var contentLength = (news.Content ?? string.Empty).Length;
                    if (contentLength < 500)
                    {
                        shouldDelete = true;
                        reason = "ShortContent";
                        deletionReasons["ShortContent"]++;
                    }
                }

                // 3. Check if content appears to be English (no Turkish characters)
                if (!shouldDelete && !ContainsTurkishCharacters(news.Caption) && !ContainsTurkishCharacters(news.Content))
                {
                    // Additional check: look for common English-only patterns
                    if (IsLikelyEnglish(news.Content ?? string.Empty))
                    {
                        shouldDelete = true;
                        reason = "NonTurkish";
                        deletionReasons["NonTurkish"]++;
                    }
                }

                // 4. Check for low-quality indicators
                if (!shouldDelete && IsLowQualityContent(news))
                {
                    shouldDelete = true;
                    reason = "LowQuality";
                    deletionReasons["LowQuality"]++;
                }

                if (shouldDelete)
                {
                    await _newsService.DeleteNewsAsync(news.Id);
                    totalDeleted++;
                    _logger.LogInformation(
                        "Deleted article: [{Reason}] {Caption} (Content length: {Length})",
                        reason,
                        news.Caption.Length > 50 ? news.Caption[..50] + "..." : news.Caption,
                        news.Content?.Length ?? 0);
                }
            }

            // Clear all caches after cleanup
            ClearAllCaches();

            _logger.LogInformation(
                "Cleanup completed: {Deleted}/{Total} articles removed",
                totalDeleted,
                totalChecked);

            return Ok(new
            {
                message = $"Cleanup completed successfully! Removed {totalDeleted} low-quality articles.",
                totalChecked,
                totalDeleted,
                deletionReasons,
                summary = new
                {
                    shortContent = deletionReasons["ShortContent"],
                    noImage = deletionReasons["NoImage"],
                    nonTurkish = deletionReasons["NonTurkish"],
                    lowQuality = deletionReasons["LowQuality"]
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during cleanup");
            return StatusCode(500, new { message = "Error during cleanup", error = ex.Message });
        }
    }

    /// <summary>
    /// Check if text contains Turkish-specific characters
    /// </summary>
    private static bool ContainsTurkishCharacters(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return false;
        }

        var turkishChars = new[] { 'ı', 'ğ', 'ü', 'ş', 'ö', 'ç', 'İ', 'Ğ', 'Ü', 'Ş', 'Ö', 'Ç' };
        return turkishChars.Any(text.Contains);
    }

    /// <summary>
    /// Check if text is likely English based on common patterns
    /// </summary>
    private static bool IsLikelyEnglish(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return false;
        }

        var lowerText = text.ToLowerInvariant();

        // Common English words that are less common in Turkish
        var englishIndicators = new[]
        {
            " the ", " and ", " with ", " this ", " that ",
            " have ", " from ", " they ", " what ", " your ",
            " which ", " their ", " would ", " there ", " could "
        };

        int englishWordCount = englishIndicators.Count(indicator => lowerText.Contains(indicator));

        // If text contains 3+ common English words, it's likely English
        return englishWordCount >= 3;
    }

    /// <summary>
    /// Check if article has low-quality indicators
    /// </summary>
    private static bool IsLowQualityContent(NewsArticle news)
    {
        // Check for common low-quality patterns
        var caption = news.Caption?.ToLowerInvariant() ?? string.Empty;

        var lowQualityPatterns = new[]
        {
            "ama ", "ask me anything", "i made ", "check out",
            "eli5", "looking for", "need help", "please help"
        };

        return lowQualityPatterns.Any(caption.Contains);
    }

    private static int CalculatePriority(int score, string source)
    {
        // Base priority from score
        var priority = Math.Min(score / 10, 50);

        // Boost for high-quality sources
        priority += source switch
        {
            "Hacker News" => 20,
            "Ars Technica" => 25,
            "TechCrunch" => 25,
            "GitHub Trending" => 15,
            "Dev.to" => 10,
            "Medium" => 10,
            _ when source.StartsWith("Reddit") => 5,
            _ => 0,
        };

        return Math.Clamp(priority, 10, 100);
    }
}
