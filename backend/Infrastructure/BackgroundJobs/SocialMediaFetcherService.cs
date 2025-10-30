using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NewsApi.Application.Services;
using NewsApi.Infrastructure.Services;

namespace NewsApi.Infrastructure.BackgroundJobs;

/// <summary>
/// Background service that fetches social media posts daily
/// </summary>
public class SocialMediaFetcherService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SocialMediaFetcherService> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromHours(24); // Run daily
    private readonly TimeSpan _startDelay = TimeSpan.FromMinutes(2); // Initial delay after startup

    public SocialMediaFetcherService(
        IServiceProvider serviceProvider,
        ILogger<SocialMediaFetcherService> logger)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Social Media Fetcher Service is starting");

        // Wait before first execution to avoid startup load
        await Task.Delay(_startDelay, stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Social Media Fetcher Service is running at: {Time}", DateTimeOffset.UtcNow);

                await FetchAndStoreSocialMediaPostsAsync(stoppingToken);

                _logger.LogInformation("Social Media Fetcher Service completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching social media posts");
            }

            // Wait for next interval
            _logger.LogInformation("Next run scheduled in {Hours} hours", _interval.TotalHours);
            await Task.Delay(_interval, stoppingToken);
        }

        _logger.LogInformation("Social Media Fetcher Service is stopping");
    }

    private async Task FetchAndStoreSocialMediaPostsAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();

        var redditService = scope.ServiceProvider.GetRequiredService<RedditService>();
        var socialMediaService = scope.ServiceProvider.GetRequiredService<ISocialMediaPostService>();

        int totalImported = 0;
        int totalSkipped = 0;

        // Fetch from multiple subreddits
        var subreddits = new[]
        {
            ("github", "copilot", "GitHub Copilot discussions"),
            ("programming", "copilot OR \"github copilot\"", "Programming community"),
            ("webdev", "copilot", "Web development"),
            ("MachineLearning", "copilot", "AI/ML community")
        };

        foreach (var (subreddit, query, description) in subreddits)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            try
            {
                _logger.LogInformation("Fetching posts from r/{Subreddit} about '{Query}'", subreddit, query);

                var posts = await redditService.SearchPostsAsync(
                    subreddit,
                    query,
                    "top",
                    "day", // Last 24 hours
                    25);

                _logger.LogInformation("Fetched {Count} posts from r/{Subreddit}", posts.Count, subreddit);

                foreach (var post in posts)
                {
                    if (cancellationToken.IsCancellationRequested)
                        break;

                    try
                    {
                        await socialMediaService.CreatePostAsync(post);
                        totalImported++;
                        _logger.LogDebug("Imported post: {Title} from r/{Subreddit}", post.Title, subreddit);
                    }
                    catch (InvalidOperationException ex) when (ex.Message.Contains("already exists"))
                    {
                        totalSkipped++;
                        _logger.LogDebug("Skipped duplicate post: {ExternalId}", post.ExternalId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to import post {ExternalId} from r/{Subreddit}",
                            post.ExternalId, subreddit);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching posts from r/{Subreddit}", subreddit);
            }
        }

        _logger.LogInformation(
            "Social media fetch completed: {Imported} posts imported, {Skipped} skipped",
            totalImported, totalSkipped);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Social Media Fetcher Service is stopping gracefully");
        return base.StopAsync(cancellationToken);
    }
}
