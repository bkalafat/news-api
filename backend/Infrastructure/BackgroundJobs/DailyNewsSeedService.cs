using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NewsApi.Application.Services;
using NewsApi.Domain.Entities;
using NewsApi.Infrastructure.Services;

namespace NewsApi.Infrastructure.BackgroundJobs;

/// <summary>
/// Background service that automatically fetches and seeds news articles daily at 5:00 AM
/// </summary>
internal sealed class DailyNewsSeedService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DailyNewsSeedService> _logger;
    private readonly TimeSpan _targetTime = new(5, 0, 0); // 05:00 AM

    public DailyNewsSeedService(
        IServiceProvider serviceProvider,
        ILogger<DailyNewsSeedService> logger)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Daily News Seed Service is starting. Scheduled for {Time} daily.", _targetTime);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var now = DateTime.Now;
                var nextRun = CalculateNextRunTime(now);
                var delay = nextRun - now;

                _logger.LogInformation(
                    "Next automatic news seed scheduled for {NextRun} (in {Hours:F1} hours)",
                    nextRun,
                    delay.TotalHours);

                // Wait until next scheduled time
                await Task.Delay(delay, stoppingToken);

                if (stoppingToken.IsCancellationRequested)
                {
                    break;
                }

                // Execute the news seeding
                await FetchAndSeedNewsAsync(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Daily News Seed Service is being cancelled");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in Daily News Seed Service");

                // Wait 1 hour before retrying on error
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }

        _logger.LogInformation("Daily News Seed Service has stopped");
    }

    private DateTime CalculateNextRunTime(DateTime currentTime)
    {
        var today = currentTime.Date;
        var todayTarget = today + _targetTime;

        // If target time today has passed, schedule for tomorrow
        if (currentTime >= todayTarget)
        {
            return todayTarget.AddDays(1);
        }

        return todayTarget;
    }

    private async Task FetchAndSeedNewsAsync(CancellationToken cancellationToken)
    {
        var startTime = DateTime.UtcNow;
        _logger.LogInformation("=== Starting Daily News Seed at {Time} ===", startTime);

        using var scope = _serviceProvider.CreateScope();

        var newsDataFetcher = scope.ServiceProvider.GetRequiredService<INewsDataFetcherService>();
        var newsService = scope.ServiceProvider.GetRequiredService<INewsArticleService>();

        int totalFetched = 0;
        int totalCreated = 0;
        int totalSkipped = 0;
        int totalErrors = 0;

        try
        {
            // Fetch news from external API
            _logger.LogInformation("Fetching latest news from external sources...");
            var newsArticles = await newsDataFetcher.FetchLatestNewsAsync(cancellationToken);
            totalFetched = newsArticles.Count;

            _logger.LogInformation("Fetched {Count} news articles from external sources", totalFetched);

            if (totalFetched == 0)
            {
                _logger.LogWarning("No news articles were fetched. Check API configuration and limits.");
                return;
            }

            // Save each article to database
            foreach (var article in newsArticles)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                try
                {
                    // Check if article with similar caption already exists (avoid duplicates)
                    var existingArticles = await newsService.GetAllNewsAsync();

                    var isDuplicate = existingArticles
                        .Where(e => e.Category == article.Category)
                        .Any(existing => existing.Caption.Equals(article.Caption, StringComparison.OrdinalIgnoreCase));

                    if (isDuplicate)
                    {
                        totalSkipped++;
                        _logger.LogDebug("Skipped duplicate article: {Caption}", article.Caption);
                        continue;
                    }

                    // Create the news article
                    // Convert DTO to entity
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
                        IsSecondPageNews = article.IsSecondPageNews
                    };
                    await newsService.CreateNewsAsync(newsEntity);
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
            var duration = endTime - startTime;

            _logger.LogInformation(
                "=== Daily News Seed Completed ===\n" +
                "Duration: {Duration:F1} seconds\n" +
                "Fetched: {Fetched} articles\n" +
                "Created: {Created} articles\n" +
                "Skipped: {Skipped} duplicates\n" +
                "Errors: {Errors}",
                duration.TotalSeconds,
                totalFetched,
                totalCreated,
                totalSkipped,
                totalErrors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Critical error during daily news seed operation");
        }
    }

    private static bool IsSimilarCaption(string caption1, string caption2)
    {
        // Simple similarity check: compare first 50 characters
        var sample1 = caption1.Length > 50 ? caption1.Substring(0, 50) : caption1;
        var sample2 = caption2.Length > 50 ? caption2.Substring(0, 50) : caption2;

        return sample1.Equals(sample2, StringComparison.OrdinalIgnoreCase);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Daily News Seed Service is stopping gracefully");
        return base.StopAsync(cancellationToken);
    }
}
