using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NewsApi.Common;
using NewsApi.Domain.Entities;
using NewsApi.Domain.Interfaces;
using NewsApi.Infrastructure.Data.Repositories;
using NewsApi.Infrastructure.Services;

namespace NewsApi.Infrastructure.BackgroundJobs;

/// <summary>
/// Daily scheduled job that fetches news from Reddit subreddits
/// Runs every morning at 6 AM UTC (9 AM Turkey time)
/// </summary>
public sealed class RedditNewsAggregatorJob : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<RedditNewsAggregatorJob> _logger;
    private readonly TimeSpan _executionInterval = TimeSpan.FromHours(24);
    private DateTime? _lastExecutionTime;

    // Reddit subreddit to category mapping
    private static readonly Dictionary<string, string> SubredditCategories = new()
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

    public RedditNewsAggregatorJob(
        IServiceScopeFactory scopeFactory,
        ILogger<RedditNewsAggregatorJob> logger)
    {
        _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Reddit News Aggregator Job started");

        // Run immediately on startup, then every 24 hours
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var now = DateTime.UtcNow;

                // Check if we should execute (6 AM UTC or first run)
                if (!_lastExecutionTime.HasValue ||
                    (now.Hour >= 6 && now - _lastExecutionTime.Value >= _executionInterval))
                {
                    await FetchAndSeedRedditNewsAsync(stoppingToken).ConfigureAwait(false);
                    _lastExecutionTime = now;
                }

                // Wait 1 hour before checking again
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Reddit News Aggregator Job cancelled");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Reddit News Aggregator Job");
                await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken).ConfigureAwait(false);
            }
        }
    }

    private async Task FetchAndSeedRedditNewsAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting Reddit news aggregation...");

        using var scope = _scopeFactory.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<INewsArticleRepository>();
        var httpClientFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();
        var redditLogger = scope.ServiceProvider.GetRequiredService<ILogger<RedditService>>();
        var redditSettings = scope.ServiceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<Infrastructure.Configuration.RedditSettings>>();

        var httpClient = httpClientFactory.CreateClient("Reddit");
        var redditService = new RedditService(httpClient, redditLogger, redditSettings);
        var totalFetched = 0;
        var totalSaved = 0;

        foreach (var (subreddit, category) in SubredditCategories)
        {
            try
            {
                _logger.LogInformation("Fetching from r/{Subreddit}...", subreddit);

                // Fetch top posts from last week
                var posts = await redditService.GetTopPostsAsync(
                    subreddit,
                    timeframe: "week",
                    limit: subreddit == "popular" ? 10 : 15
                ).ConfigureAwait(false);

                totalFetched += posts.Count;

                // Convert Reddit posts to news articles
                var newsArticles = posts.Select(post => ConvertRedditPostToNews(post, category, subreddit)).ToList();

                // Save to database (skip if already exists by checking post URL)
                foreach (var article in newsArticles)
                {
                    var existing = await repository.GetBySlugAsync(article.Slug).ConfigureAwait(false);
                    if (existing == null)
                    {
                        await repository.CreateAsync(article).ConfigureAwait(false);
                        totalSaved++;
                    }
                }

                _logger.LogInformation("Processed {Count} posts from r/{Subreddit}", posts.Count, subreddit);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching from r/{Subreddit}", subreddit);
            }
        }

        _logger.LogInformation(
            "Reddit aggregation completed. Fetched: {Fetched}, Saved: {Saved}",
            totalFetched,
            totalSaved
        );
    }

    private static NewsArticle ConvertRedditPostToNews(
        Application.DTOs.CreateSocialMediaPostDto post,
        string category,
        string subreddit)
    {
        var now = DateTime.UtcNow;

        // Generate Turkish title and content
        var turkishTitle = GenerateTurkishTitle(post.Title, subreddit);
        var turkishContent = GenerateTurkishContent(post);

        // Extract image URL (prefer first image, fallback to thumbnail)
        var imageUrl = post.ImageUrls?.FirstOrDefault() ?? string.Empty;

        return new NewsArticle
        {
            Category = category,
            Type = "reddit",
            Caption = turkishTitle,
            Slug = SlugHelper.GenerateSlug(turkishTitle),
            Keywords = string.Join(", ", post.Tags ?? Array.Empty<string>()),
            SocialTags = $"#Reddit #{subreddit} {string.Join(" ", (post.Tags ?? Array.Empty<string>()).Select(t => $"#{t}"))}",
            Summary = GenerateTurkishSummary(post.Title, post.Content),
            ImgPath = imageUrl,
            ImgAlt = turkishTitle,
            Content = turkishContent,
            Subjects = new[] { subreddit, "Reddit", category },
            Authors = new[] { $"r/{subreddit}", post.Author ?? "Reddit" },
            ExpressDate = post.PostedAt == default ? now : post.PostedAt,
            CreateDate = now,
            UpdateDate = now,
            Priority = CalculatePriority(post.Upvotes, post.CommentCount),
            IsActive = true,
            ViewCount = 0,
            IsSecondPageNews = false,
        };
    }

    private static string GenerateTurkishTitle(string originalTitle, string subreddit)
    {
        // Simple title generation - can be enhanced with translation API
        var prefix = subreddit switch
        {
            "popular" => "Popüler: ",
            "ArtificialIntelligence" => "AI: ",
            "GithubCopilot" => "Copilot: ",
            "OpenAI" => "OpenAI: ",
            "ClaudeAI" => "Claude: ",
            _ => string.Empty,
        };

        return $"{prefix}{originalTitle}";
    }

    private static string GenerateTurkishSummary(string title, string content)
    {
        // Extract first sentence or first 200 chars
        var summary = !string.IsNullOrEmpty(content) ? content : title;

        if (summary.Length > 200)
        {
            var firstSentence = summary.Split('.').FirstOrDefault();
            summary = firstSentence?.Length <= 200 ? firstSentence : summary[..200];
        }

        return summary + "...";
    }

    private static string GenerateTurkishContent(Application.DTOs.CreateSocialMediaPostDto post)
    {
        var content = new System.Text.StringBuilder();

        content.AppendLine("<div class='reddit-post'>");
        content.AppendLine($"<h2>{post.Title}</h2>");

        if (!string.IsNullOrEmpty(post.Content))
        {
            // Convert Reddit markdown to HTML
            var htmlContent = ConvertMarkdownToHtml(post.Content);
            content.AppendLine($"<div class='post-content'>{htmlContent}</div>");
        }

        // Add images
        if (post.ImageUrls != null && post.ImageUrls.Length > 0)
        {
            foreach (var imageUrl in post.ImageUrls.Take(3))
            {
                content.AppendLine($"<img src='{imageUrl}' alt='{post.Title}' style='max-width:100%;height:auto;margin:20px 0;' />");
            }
        }

        // Add Reddit metadata
        content.AppendLine("<div class='post-metadata' style='background:#f5f5f5;padding:16px;margin:20px 0;border-radius:8px;'>");
        content.AppendLine($"<p><strong>📊 Reddit İstatistikleri:</strong></p>");
        content.AppendLine("<ul>");
        content.AppendLine($"<li>⬆️ Upvote: {post.Upvotes}</li>");
        content.AppendLine($"<li>💬 Yorum: {post.CommentCount}</li>");
        content.AppendLine($"<li>👤 Yazar: u/{post.Author}</li>");
        content.AppendLine($"<li>📅 Tarih: {post.PostedAt:dd MMMM yyyy}</li>");
        content.AppendLine("</ul>");
        content.AppendLine($"<p><a href='{post.PostUrl}' target='_blank' rel='noopener'>🔗 Reddit'te Görüntüle</a></p>");
        content.AppendLine("</div>");

        content.AppendLine("</div>");

        return content.ToString();
    }

    private static string ConvertMarkdownToHtml(string markdown)
    {
        if (string.IsNullOrEmpty(markdown))
        {
            return string.Empty;
        }

        // Basic markdown to HTML conversion
        var html = markdown;

        // Bold: **text** or __text__
        html = Regex.Replace(html, @"\*\*(.+?)\*\*", "<strong>$1</strong>");
        html = Regex.Replace(html, @"__(.+?)__", "<strong>$1</strong>");

        // Italic: *text* or _text_
        html = Regex.Replace(html, @"\*(.+?)\*", "<em>$1</em>");
        html = Regex.Replace(html, @"_(.+?)_", "<em>$1</em>");

        // Links: [text](url)
        html = Regex.Replace(html, @"\[(.+?)\]\((.+?)\)", "<a href='$2' target='_blank' rel='noopener'>$1</a>");

        // Line breaks
        html = html.Replace("\n\n", "</p><p>");
        html = html.Replace("\n", "<br />");

        return $"<p>{html}</p>";
    }

    private static int CalculatePriority(int upvotes, int comments)
    {
        // Priority based on engagement (1-100)
        var score = upvotes + (comments * 2);

        return score switch
        {
            >= 10000 => 95,
            >= 5000 => 90,
            >= 2000 => 85,
            >= 1000 => 80,
            >= 500 => 70,
            >= 200 => 60,
            >= 100 => 50,
            >= 50 => 40,
            _ => 30,
        };
    }
}
