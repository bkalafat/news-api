using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NewsApi.Application.DTOs;

namespace NewsApi.Infrastructure.Services;

/// <summary>
/// Service for fetching news data from external NewsAPI.org
/// </summary>
internal interface INewsDataFetcherService
{
    /// <summary>
    /// Fetches latest news articles from NewsAPI.org
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of news articles as DTOs ready to be saved</returns>
    Task<List<CreateNewsArticleDto>> FetchLatestNewsAsync(CancellationToken cancellationToken = default);
}

internal sealed class NewsDataFetcherService : INewsDataFetcherService
{
    private readonly HttpClient _httpClient;
    private readonly NewsApiSettings _settings;
    private readonly ILogger<NewsDataFetcherService> _logger;

    public NewsDataFetcherService(
        HttpClient httpClient,
        IOptions<NewsApiSettings> settings,
        ILogger<NewsDataFetcherService> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<List<CreateNewsArticleDto>> FetchLatestNewsAsync(CancellationToken cancellationToken = default)
    {
        var allArticles = new List<CreateNewsArticleDto>();

        if (string.IsNullOrWhiteSpace(_settings.ApiKey))
        {
            _logger.LogWarning("NewsAPI ApiKey is not configured. Skipping news fetch.");
            return allArticles;
        }

        foreach (var category in _settings.Categories)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            try
            {
                _logger.LogInformation("Fetching {Category} news from NewsAPI.org", category);

                var articles = await FetchNewsByCategoryAsync(category, cancellationToken);
                allArticles.AddRange(articles);

                _logger.LogInformation("Fetched {Count} articles for {Category}", articles.Count, category);

                // Rate limiting: Wait 1 second between requests to avoid API throttling
                await Task.Delay(1000, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching news for category {Category}", category);
            }
        }

        return allArticles;
    }

    private async Task<List<CreateNewsArticleDto>> FetchNewsByCategoryAsync(
        string category,
        CancellationToken cancellationToken)
    {
        var articles = new List<CreateNewsArticleDto>();

        try
        {
            // Build request URL for top headlines
            var country = _settings.Countries.FirstOrDefault() ?? "tr";
            var url = $"{_settings.BaseUrl}/top-headlines?country={country}&category={category}&pageSize={_settings.MaxArticlesPerCategory}&apiKey={_settings.ApiKey}";

            var response = await _httpClient.GetAsync(url, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "NewsAPI returned status {StatusCode} for category {Category}",
                    response.StatusCode, category);
                return articles;
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonSerializer.Deserialize<NewsApiResponse>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            });

            if (result?.Articles == null || result.Articles.Length == 0)
            {
                _logger.LogInformation("No articles found for category {Category}", category);
                return articles;
            }

            // Convert NewsAPI articles to our DTOs
            foreach (var article in result.Articles)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                try
                {
                    var dto = MapToCreateDto(article, category);
                    if (dto != null)
                    {
                        articles.Add(dto);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to map article: {Title}", article.Title);
                }
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error fetching news for category {Category}", category);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON parsing error for category {Category}", category);
        }

        return articles;
    }

    private CreateNewsArticleDto? MapToCreateDto(NewsApiArticle article, string category)
    {
        // Skip articles without title or content
        if (string.IsNullOrWhiteSpace(article.Title) ||
            string.IsNullOrWhiteSpace(article.Description) ||
string.Equals(article.Title, "[Removed]", StringComparison.Ordinal))
        {
            return null;
        }

        // Map NewsAPI category to our internal categories
        var mappedCategory = MapCategory(category);

        return new CreateNewsArticleDto
        {
            Category = mappedCategory,
            Type = "Genel",
            Caption = TruncateString(article.Title, 500),
            Keywords = string.Join(", ", ExtractKeywords(article.Title)),
            SocialTags = string.Empty,
            Summary = TruncateString(article.Description ?? article.Title, 2000),
            ImgPath = string.Empty,
            ImgAlt = TruncateString(article.Title, 200),
            ImageUrl = article.UrlToImage ?? string.Empty,
            ThumbnailUrl = article.UrlToImage ?? string.Empty,
            Content = BuildContent(article),
            Subjects = ExtractKeywords(article.Title),
            Authors = string.IsNullOrWhiteSpace(article.Author)
                ? ["NewsAPI"]
                : [TruncateString(article.Author, 100)],
            ExpressDate = article.PublishedAt != default
                ? article.PublishedAt
                : DateTime.UtcNow,
            Priority = 5,
            IsActive = true,
            IsSecondPageNews = false,
        };
    }

    private static string BuildContent(NewsApiArticle article)
    {
        var content = article.Content ?? article.Description ?? string.Empty;

        // NewsAPI often truncates content with [+X chars], so add source link
        var sourceLink = $"\n\n<p>Kaynak: <a href=\"{article.Url}\" target=\"_blank\" rel=\"noopener noreferrer\">{article.Source?.Name ?? "Haber Kaynağı"}</a></p>";

        return content + sourceLink;
    }

    private static string MapCategory(string newsApiCategory)
    {
        return newsApiCategory.ToLowerInvariant() switch
        {
            "technology" => "Teknoloji",
            "business" => "Ekonomi",
            "sports" => "Spor",
            "science" => "Bilim",
            "health" => "Sağlık",
            "entertainment" => "Magazin",
            "general" => "Genel",
            _ => "Genel",
        };
    }

    private static string[] ExtractKeywords(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return [];
        }

        // Simple keyword extraction: split by common separators
        return text.Split([' ', ',', '-', ':', '|'], StringSplitOptions.RemoveEmptyEntries)
            .Where(w => w.Length > 3)
            .Take(5)
            .ToArray();
    }

    private static string TruncateString(string text, int maxLength)
    {
        if (string.IsNullOrEmpty(text))
        {
            return string.Empty;
        }

        return text.Length <= maxLength
            ? text
            : string.Concat(text.AsSpan(0, maxLength - 3), "...");
    }
}

/// <summary>
/// Response model from NewsAPI.org
/// </summary>
internal sealed class NewsApiResponse
{
    public string Status { get; set; } = string.Empty;

    public int TotalResults { get; set; }

    public NewsApiArticle[] Articles { get; set; } = [];
}

/// <summary>
/// Article model from NewsAPI.org
/// </summary>
internal sealed class NewsApiArticle
{
    public NewsApiSource? Source { get; set; }

    public string Author { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Url { get; set; } = string.Empty;

    public string UrlToImage { get; set; } = string.Empty;

    public DateTime PublishedAt { get; set; }

    public string Content { get; set; } = string.Empty;
}

/// <summary>
/// Source model from NewsAPI.org
/// </summary>
internal sealed class NewsApiSource
{
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;
}
