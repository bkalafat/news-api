using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.ServiceModel.Syndication;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NewsApi.Application.DTOs;

namespace NewsApi.Infrastructure.Services;

/// <summary>
/// Service for aggregating news from multiple free sources
/// </summary>
public sealed class NewsAggregatorService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<NewsAggregatorService> _logger;
    private readonly IConfiguration _configuration;

    public NewsAggregatorService(
        HttpClient httpClient,
        ILogger<NewsAggregatorService> logger,
        IConfiguration configuration)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    /// <summary>
    /// Fetch AI and tech news from all available sources
    /// </summary>
    public async Task<List<AggregatedNewsItem>> FetchAllNewsAsync()
    {
        var allNews = new List<AggregatedNewsItem>();

        // Fetch from all sources in parallel
        var tasks = new[]
        {
            FetchRedditNewsAsync(),
            FetchGitHubTrendingAsync(),
            FetchHackerNewsAsync(),
            FetchDevToArticlesAsync(),
            FetchMediumAIArticlesAsync(),
            FetchArsTechnicaAsync(),
            FetchTechCrunchAsync(),
        };

        var results = await Task.WhenAll(tasks);

        foreach (var items in results)
        {
            allNews.AddRange(items);
        }

        // Sort by score/relevance and remove duplicates
        var uniqueNews = allNews
            .GroupBy(n => n.Title.ToLowerInvariant())
            .Select(g => g.OrderByDescending(n => n.Score).First())
            .OrderByDescending(n => n.Score)
            .ThenByDescending(n => n.PublishedDate)
            .ToList();

        _logger.LogInformation("Fetched {Count} unique news items from {Sources} sources", 
            uniqueNews.Count, tasks.Length);

        return uniqueNews;
    }

    /// <summary>
    /// Fetch from Reddit AI/tech subreddits
    /// </summary>
    private async Task<List<AggregatedNewsItem>> FetchRedditNewsAsync()
    {
        var items = new List<AggregatedNewsItem>();

        var subreddits = new[]
        {
            ("artificial", "AI discussions"),
            ("MachineLearning", "Machine Learning"),
            ("OpenAI", "OpenAI and ChatGPT"),
            ("ClaudeAI", "Anthropic Claude"),
            ("github", "GitHub"),
            ("programming", "Programming"),
            ("technology", "Technology"),
        };

        foreach (var (subreddit, description) in subreddits)
        {
            try
            {
                var url = $"https://www.reddit.com/r/{subreddit}/hot.json?limit=15";
                var response = await _httpClient.GetStringAsync(url);
                var json = JsonDocument.Parse(response);

                var posts = json.RootElement
                    .GetProperty("data")
                    .GetProperty("children");

                foreach (var post in posts.EnumerateArray())
                {
                    var data = post.GetProperty("data");
                    
                    // Skip stickied posts
                    if (data.GetProperty("stickied").GetBoolean())
                    {
                        continue;
                    }

                    var title = data.GetProperty("title").GetString() ?? string.Empty;
                    var selftext = data.TryGetProperty("selftext", out var st) ? st.GetString() : string.Empty;
                    var url_value = data.GetProperty("url").GetString() ?? string.Empty;
                    var permalink = data.GetProperty("permalink").GetString() ?? string.Empty;
                    var score = data.GetProperty("ups").GetInt32();
                    var created = data.GetProperty("created_utc").GetDouble();
                    var author = data.GetProperty("author").GetString() ?? "Unknown";

                    items.Add(new AggregatedNewsItem
                    {
                        Title = title,
                        Content = selftext,
                        SourceUrl = $"https://reddit.com{permalink}",
                        ExternalUrl = url_value,
                        Source = $"Reddit - r/{subreddit}",
                        Category = GetCategoryFromSubreddit(subreddit),
                        Author = author,
                        Score = score,
                        PublishedDate = DateTimeOffset.FromUnixTimeSeconds((long)created).DateTime,
                        Tags = new List<string> { subreddit, "reddit", "community" },
                    });
                }

                _logger.LogInformation("Fetched {Count} posts from r/{Subreddit}", 
                    items.Count(i => i.Source.Contains(subreddit)), subreddit);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching from r/{Subreddit}", subreddit);
            }
        }

        return items;
    }

    /// <summary>
    /// Fetch GitHub trending repositories
    /// </summary>
    private async Task<List<AggregatedNewsItem>> FetchGitHubTrendingAsync()
    {
        var items = new List<AggregatedNewsItem>();

        try
        {
            // GitHub trending repos (unofficial API)
            var url = "https://api.gitterapp.com/repositories?since=daily&language=";
            var response = await _httpClient.GetStringAsync(url);
            var repos = JsonSerializer.Deserialize<List<GitHubTrendingRepo>>(response);

            if (repos != null)
            {
                foreach (var repo in repos.Take(20))
                {
                    items.Add(new AggregatedNewsItem
                    {
                        Title = $"{repo.Author}/{repo.Name}: {repo.Description}",
                        Content = repo.Description ?? string.Empty,
                        SourceUrl = repo.Url ?? string.Empty,
                        ExternalUrl = repo.Url ?? string.Empty,
                        Source = "GitHub Trending",
                        Category = "Technology",
                        Author = repo.Author ?? "Unknown",
                        Score = repo.Stars ?? 0,
                        PublishedDate = DateTime.UtcNow,
                        Tags = new List<string> { "github", "open-source", "trending" },
                    });
                }

                _logger.LogInformation("Fetched {Count} trending GitHub repositories", items.Count);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching GitHub trending");
        }

        return items;
    }

    /// <summary>
    /// Fetch from Hacker News
    /// </summary>
    private async Task<List<AggregatedNewsItem>> FetchHackerNewsAsync()
    {
        var items = new List<AggregatedNewsItem>();

        try
        {
            // Get top stories
            var topStoriesUrl = "https://hacker-news.firebaseio.com/v0/topstories.json";
            var topStoriesJson = await _httpClient.GetStringAsync(topStoriesUrl);
            var storyIds = JsonSerializer.Deserialize<List<int>>(topStoriesJson);

            if (storyIds != null)
            {
                // Fetch first 30 stories
                var storyTasks = storyIds.Take(30).Select(async id =>
                {
                    try
                    {
                        var storyUrl = $"https://hacker-news.firebaseio.com/v0/item/{id}.json";
                        var storyJson = await _httpClient.GetStringAsync(storyUrl);
                        return JsonSerializer.Deserialize<HackerNewsItem>(storyJson);
                    }
                    catch
                    {
                        return null;
                    }
                });

                var stories = await Task.WhenAll(storyTasks);

                foreach (var story in stories.Where(s => s != null && !string.IsNullOrEmpty(s.Title)))
                {
                    items.Add(new AggregatedNewsItem
                    {
                        Title = story!.Title!,
                        Content = story.Text ?? string.Empty,
                        SourceUrl = $"https://news.ycombinator.com/item?id={story.Id}",
                        ExternalUrl = story.Url ?? string.Empty,
                        Source = "Hacker News",
                        Category = "Technology",
                        Author = story.By ?? "Unknown",
                        Score = story.Score ?? 0,
                        PublishedDate = DateTimeOffset.FromUnixTimeSeconds(story.Time ?? 0).DateTime,
                        Tags = new List<string> { "hackernews", "tech", "startup" },
                    });
                }

                _logger.LogInformation("Fetched {Count} stories from Hacker News", items.Count);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching from Hacker News");
        }

        return items;
    }

    /// <summary>
    /// Fetch from Dev.to
    /// </summary>
    private async Task<List<AggregatedNewsItem>> FetchDevToArticlesAsync()
    {
        var items = new List<AggregatedNewsItem>();

        try
        {
            var tags = new[] { "ai", "machinelearning", "chatgpt", "github", "copilot" };

            foreach (var tag in tags)
            {
                var url = $"https://dev.to/api/articles?tag={tag}&per_page=10&top=7";
                var response = await _httpClient.GetStringAsync(url);
                var articles = JsonSerializer.Deserialize<List<DevToArticle>>(response);

                if (articles != null)
                {
                    foreach (var article in articles)
                    {
                        items.Add(new AggregatedNewsItem
                        {
                            Title = article.Title ?? string.Empty,
                            Content = article.Description ?? string.Empty,
                            SourceUrl = article.Url ?? string.Empty,
                            ExternalUrl = article.Url ?? string.Empty,
                            Source = "Dev.to",
                            Category = "Technology",
                            Author = article.User?.Name ?? "Unknown",
                            Score = article.PublicReactionsCount ?? 0,
                            PublishedDate = article.PublishedAt ?? DateTime.UtcNow,
                            Tags = article.TagList ?? new List<string> { tag },
                        });
                    }
                }
            }

            _logger.LogInformation("Fetched {Count} articles from Dev.to", items.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching from Dev.to");
        }

        return items;
    }

    /// <summary>
    /// Fetch Medium articles via RSS
    /// </summary>
    private async Task<List<AggregatedNewsItem>> FetchMediumAIArticlesAsync()
    {
        var items = new List<AggregatedNewsItem>();

        try
        {
            var tags = new[] { "artificial-intelligence", "machine-learning", "chatgpt", "github-copilot" };

            foreach (var tag in tags)
            {
                var url = $"https://medium.com/feed/tag/{tag}";
                var rssItems = await FetchRssFeedAsync(url);

                foreach (var item in rssItems.Take(10))
                {
                    items.Add(new AggregatedNewsItem
                    {
                        Title = item.Title,
                        Content = item.Summary,
                        SourceUrl = item.Link,
                        ExternalUrl = item.Link,
                        Source = "Medium",
                        Category = "Technology",
                        Author = item.Author,
                        Score = 0,
                        PublishedDate = item.PublishedDate,
                        Tags = new List<string> { tag, "medium", "article" },
                    });
                }
            }

            _logger.LogInformation("Fetched {Count} articles from Medium", items.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching from Medium");
        }

        return items;
    }

    /// <summary>
    /// Fetch from Ars Technica RSS
    /// </summary>
    private async Task<List<AggregatedNewsItem>> FetchArsTechnicaAsync()
    {
        var items = new List<AggregatedNewsItem>();

        try
        {
            var url = "https://feeds.arstechnica.com/arstechnica/technology-lab";
            var rssItems = await FetchRssFeedAsync(url);

            foreach (var item in rssItems.Take(15))
            {
                items.Add(new AggregatedNewsItem
                {
                    Title = item.Title,
                    Content = item.Summary,
                    SourceUrl = item.Link,
                    ExternalUrl = item.Link,
                    Source = "Ars Technica",
                    Category = "Technology",
                    Author = item.Author,
                    Score = 100, // High quality source
                    PublishedDate = item.PublishedDate,
                    Tags = new List<string> { "arstechnica", "tech-news", "journalism" },
                });
            }

            _logger.LogInformation("Fetched {Count} articles from Ars Technica", items.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching from Ars Technica");
        }

        return items;
    }

    /// <summary>
    /// Fetch from TechCrunch RSS
    /// </summary>
    private async Task<List<AggregatedNewsItem>> FetchTechCrunchAsync()
    {
        var items = new List<AggregatedNewsItem>();

        try
        {
            var url = "https://techcrunch.com/feed/";
            var rssItems = await FetchRssFeedAsync(url);

            foreach (var item in rssItems.Take(15))
            {
                items.Add(new AggregatedNewsItem
                {
                    Title = item.Title,
                    Content = item.Summary,
                    SourceUrl = item.Link,
                    ExternalUrl = item.Link,
                    Source = "TechCrunch",
                    Category = "Technology",
                    Author = item.Author,
                    Score = 100, // High quality source
                    PublishedDate = item.PublishedDate,
                    Tags = new List<string> { "techcrunch", "startup", "tech-news" },
                });
            }

            _logger.LogInformation("Fetched {Count} articles from TechCrunch", items.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching from TechCrunch");
        }

        return items;
    }

    /// <summary>
    /// Generic RSS feed parser
    /// </summary>
    private async Task<List<RssItem>> FetchRssFeedAsync(string url)
    {
        var items = new List<RssItem>();

        try
        {
            var response = await _httpClient.GetStreamAsync(url);
            using var xmlReader = XmlReader.Create(response);
            var feed = SyndicationFeed.Load(xmlReader);

            foreach (var item in feed.Items)
            {
                items.Add(new RssItem
                {
                    Title = item.Title?.Text ?? string.Empty,
                    Summary = item.Summary?.Text ?? string.Empty,
                    Link = item.Links.FirstOrDefault()?.Uri.ToString() ?? string.Empty,
                    Author = item.Authors.FirstOrDefault()?.Name ?? "Unknown",
                    PublishedDate = item.PublishDate.DateTime,
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing RSS feed: {Url}", url);
        }

        return items;
    }

    private static string GetCategoryFromSubreddit(string subreddit)
    {
        return subreddit.ToLowerInvariant() switch
        {
            "artificial" or "machinelearning" or "openai" or "claudeai" => "Science",
            "github" or "programming" => "Technology",
            "technology" => "Technology",
            _ => "Technology",
        };
    }

    #region DTO Classes

    private class GitHubTrendingRepo
    {
        public string? Author { get; set; }
        public string? Name { get; set; }
        public string? Url { get; set; }
        public string? Description { get; set; }
        public int? Stars { get; set; }
    }

    private class HackerNewsItem
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Url { get; set; }
        public string? Text { get; set; }
        public string? By { get; set; }
        public int? Score { get; set; }
        public long? Time { get; set; }
    }

    private class DevToArticle
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Url { get; set; }
        public DateTime? PublishedAt { get; set; }
        public int? PublicReactionsCount { get; set; }
        public DevToUser? User { get; set; }
        public List<string>? TagList { get; set; }
    }

    private class DevToUser
    {
        public string? Name { get; set; }
    }

    private class RssItem
    {
        public string Title { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public DateTime PublishedDate { get; set; }
    }

    #endregion
}

/// <summary>
/// Aggregated news item from any source
/// </summary>
public class AggregatedNewsItem
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string SourceUrl { get; set; } = string.Empty;
    public string ExternalUrl { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public int Score { get; set; }
    public DateTime PublishedDate { get; set; }
    public List<string> Tags { get; set; } = new();
}
