using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using NewsApi.Common;
using NewsApi.Domain.Entities;
using NewsApi.Domain.Interfaces;
using NewsApi.Infrastructure.Services;

namespace NewsApi.Infrastructure.BackgroundJobs;

/// <summary>
/// Background service that aggregates, translates, and publishes news daily at 5 AM
/// </summary>
internal sealed class DailyNewsAggregatorJob : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DailyNewsAggregatorJob> _logger;
    private readonly IMemoryCache _cache;

    // Run at 5:00 AM every day
    private static readonly TimeSpan TargetTime = new(5, 0, 0);

    public DailyNewsAggregatorJob(
        IServiceProvider serviceProvider,
        ILogger<DailyNewsAggregatorJob> logger,
        IMemoryCache cache)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Daily News Aggregator Job started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var now = DateTime.Now;
                var nextRun = CalculateNextRunTime(now);
                var delay = nextRun - now;

                _logger.LogInformation(
                    "Next news aggregation scheduled at {NextRun} (in {Hours:F1} hours)",
                    nextRun.ToString("yyyy-MM-dd HH:mm:ss"),
                    delay.TotalHours);

                // Wait until 5 AM
                await Task.Delay(delay, stoppingToken);

                if (stoppingToken.IsCancellationRequested)
                {
                    break;
                }

                // Execute the aggregation
                _logger.LogInformation("Starting daily news aggregation at {Time}", DateTime.Now);
                await AggregateAndPublishNewsAsync(stoppingToken);
                _logger.LogInformation("Daily news aggregation completed at {Time}", DateTime.Now);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in daily news aggregation");

                // Wait 1 hour before retrying on error
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }

        _logger.LogInformation("Daily News Aggregator Job stopped");
    }

    private static DateTime CalculateNextRunTime(DateTime now)
    {
        var scheduledTime = now.Date + TargetTime;

        // If it's already past 5 AM today, schedule for 5 AM tomorrow
        if (now >= scheduledTime)
        {
            scheduledTime = scheduledTime.AddDays(1);
        }

        return scheduledTime;
    }

    private async Task AggregateAndPublishNewsAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();

        var aggregatorService = scope.ServiceProvider.GetRequiredService<NewsAggregatorService>();
        var translationService = scope.ServiceProvider.GetRequiredService<TranslationService>();
        var newsRepository = scope.ServiceProvider.GetRequiredService<INewsArticleRepository>();
        var imageDownloadService = scope.ServiceProvider.GetRequiredService<ImageDownloadService>();
        var categoryDetectionService = scope.ServiceProvider.GetRequiredService<CategoryDetectionService>();
        var imageStorageService = scope.ServiceProvider.GetRequiredService<IImageStorageService>();

        int successCount = 0;
        int failureCount = 0;
        int skippedCount = 0;

        try
        {
            // 1. Fetch all news from various sources
            _logger.LogInformation("Fetching news from all sources...");
            var aggregatedNews = await aggregatorService.FetchAllNewsAsync();
            _logger.LogInformation("Fetched {Count} news items", aggregatedNews.Count);

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
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                try
                {
                    // Check if already exists (by title)
                    var slug = SlugHelper.GenerateSlug(item.Title);
                    var existing = await newsRepository.GetBySlugAsync(slug);

                    if (existing != null)
                    {
                        skippedCount++;
                        _logger.LogDebug("Skipping duplicate: {Title}", item.Title);
                        continue;
                    }

                    // Detect language
                    var sourceLanguage = translationService.DetectLanguage(item.Title);

                    // Translate to Turkish if not already Turkish
                    string translatedTitle = item.Title;
                    string translatedContent = item.Content;
                    string translatedSummary = item.Content;

                    if (sourceLanguage != "tr")
                    {
                        _logger.LogInformation("Translating: {Title}", item.Title);

                        translatedTitle = await translationService.TranslateToTurkishAsync(item.Title, sourceLanguage);

                        // Create summary from content (first 200 chars)
                        var summary = item.Content.Length > 200
                            ? item.Content[..200] + "..."
                            : item.Content;

                        translatedSummary = await translationService.TranslateToTurkishAsync(summary, sourceLanguage);

                        // Only translate full content if it's not too long (to save API quota)
                        if (item.Content.Length > 0 && item.Content.Length < 2000)
                        {
                            translatedContent = await translationService.TranslateToTurkishAsync(item.Content, sourceLanguage);
                        }
                        else if (item.Content.Length >= 2000)
                        {
                            // For very long content, keep original and add translation note
                            translatedContent = $"[Orijinal içerik: {item.Source}]\n\n{item.Content}";
                        }

                        // Rate limiting - wait between translations
                        await Task.Delay(500, cancellationToken);
                    }

                    // 4. Detect category intelligently based on content and engagement
                    var detectedCategory = categoryDetectionService.DetectCategory(
                        item.Title,
                        item.Content,
                        item.Source,
                        item.Tags,
                        item.Score);

                    // 5. Download and upload image to MinIO (if available)
                    ImageMetadata? imageMetadata = null;
                    string imageUrl = string.Empty;
                    string thumbnailUrl = string.Empty;
                    string imgPath = string.Empty;

                    var imageSourceUrl = imageDownloadService.ExtractImageUrl(
                        item.ExternalUrl,
                        item.SourceUrl,
                        item.Source);

                    if (!string.IsNullOrEmpty(imageSourceUrl))
                    {
                        var newsId = ObjectId.GenerateNewId().ToString();
                        imageMetadata = await imageDownloadService.DownloadAndUploadImageAsync(
                            newsId,
                            imageSourceUrl,
                            $"Image from {item.Source}");

                        if (imageMetadata != null)
                        {
                            imageUrl = imageStorageService.GetImageUrl(imageMetadata.MinioObjectKey);
                            thumbnailUrl = imageStorageService.GetThumbnailUrl(newsId, ".jpg");
                            imgPath = imageMetadata.MinioObjectKey;
                            _logger.LogInformation("Downloaded and uploaded image for: {Title}", item.Title);
                        }
                    }

                    // 6. Create news entity
                    var news = new NewsArticle
                    {
                        Id = ObjectId.GenerateNewId().ToString(),
                        Category = detectedCategory,
                        Type = "article",
                        Caption = translatedTitle,
                        Slug = slug,
                        Keywords = string.Join(", ", item.Tags.Take(10)),
                        SocialTags = string.Join(" ", item.Tags.Select(t => $"#{t}").Take(5)),
                        Summary = translatedSummary,
                        ImgPath = imgPath,
                        ImgAlt = $"Image from {item.Source}",
                        ImageUrl = imageUrl,
                        ThumbnailUrl = thumbnailUrl,
                        Content = translatedContent,
                        ExpressDate = item.PublishedDate,
                        CreateDate = DateTime.UtcNow,
                        UpdateDate = DateTime.UtcNow,
                        Priority = CalculatePriority(item.Score, item.Source),
                        IsActive = true,
                        ViewCount = 0,
                        IsSecondPageNews = false,
                        ImageMetadata = imageMetadata,
                        Subjects = Array.Empty<string>(),
                        Authors = new[] { item.Author },
                    };

                    // 7. Save to database
                    await newsRepository.CreateAsync(news);
                    successCount++;

                    _logger.LogInformation(
                        "Published: [{Category}] {Title} (Source: {Source}, Score: {Score}, Image: {HasImage})",
                        news.Category,
                        news.Caption,
                        item.Source,
                        item.Score,
                        !string.IsNullOrEmpty(news.ImageUrl));
                }
                catch (Exception ex)
                {
                    failureCount++;
                    _logger.LogError(ex, "Failed to process news item: {Title}", item.Title);
                }
            }

            // 8. Log trending categories
            var trendingCategories = categoryDetectionService.GetTrendingCategories(aggregatedNews);
            _logger.LogInformation(
                "Trending categories: {Categories}",
                string.Join(", ", trendingCategories.Select(x => $"{x.Key}({x.Value})")));

            // 9. Clear all caches
            _logger.LogInformation("Clearing caches...");
            ClearAllCaches();

            _logger.LogInformation(
                "News aggregation complete: {Success} published, {Skipped} skipped, {Failed} failed",
                successCount,
                skippedCount,
                failureCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fatal error in news aggregation process");
            throw;
        }
    }

    private void ClearAllCaches()
    {
        try
        {
            // Clear all news-related caches
            _cache.Remove(CacheKeys.AllNews);

            // Clear category caches
            var categories = new[] { "Technology", "Science", "Business", "Sports", "Entertainment", "Health", "World" };
            foreach (var category in categories)
            {
                _cache.Remove(CacheKeys.GetNewsByCategory(category));
            }

            // Clear type caches
            var types = new[] { "article", "breaking", "analysis" };
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

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Daily News Aggregator Job stopping gracefully");
        return base.StopAsync(cancellationToken);
    }
}
