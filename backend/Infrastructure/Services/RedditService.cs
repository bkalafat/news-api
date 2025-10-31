using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NewsApi.Application.DTOs;
using NewsApi.Infrastructure.Configuration;

namespace NewsApi.Infrastructure.Services;

/// <summary>
/// Service for fetching posts from Reddit API with OAuth authentication
/// </summary>
public sealed class RedditService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<RedditService> _logger;
    private readonly RedditSettings _settings;
    private const string RedditApiBase = "https://oauth.reddit.com";
    private const string RedditAuthBase = "https://www.reddit.com/api/v1";
    private string? _accessToken;
    private DateTime _tokenExpiration = DateTime.MinValue;

    public RedditService(
        HttpClient httpClient,
        ILogger<RedditService> logger,
        IOptions<RedditSettings> settings)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));

        // Reddit requires a User-Agent header
        _httpClient.DefaultRequestHeaders.Add("User-Agent", _settings.UserAgent);
    }

    /// <summary>
    /// Gets a valid OAuth access token, refreshing if necessary
    /// </summary>
    private async Task<string> GetAccessTokenAsync()
    {
        // Return cached token if still valid
        if (!string.IsNullOrEmpty(_accessToken) && DateTime.UtcNow < _tokenExpiration)
        {
            return _accessToken;
        }

        _logger.LogInformation("Requesting new Reddit OAuth access token...");

        try
        {
            // Check if credentials are configured
            if (string.IsNullOrEmpty(_settings.ClientId) ||
                string.IsNullOrEmpty(_settings.ClientSecret) ||
                string.IsNullOrEmpty(_settings.Username) ||
                string.IsNullOrEmpty(_settings.Password))
            {
                _logger.LogWarning("Reddit OAuth credentials not configured. Falling back to unauthenticated API.");
                return string.Empty;
            }

            // Create Basic Auth header for token request
            var authValue = Convert.ToBase64String(
                Encoding.UTF8.GetBytes($"{_settings.ClientId}:{_settings.ClientSecret}")
            );

            var tokenRequest = new HttpRequestMessage(HttpMethod.Post, $"{RedditAuthBase}/access_token");
            tokenRequest.Headers.Authorization = new AuthenticationHeaderValue("Basic", authValue);

            var formData = new Dictionary<string, string>
            {
                ["grant_type"] = "password",
                ["username"] = _settings.Username,
                ["password"] = _settings.Password,
            };

            tokenRequest.Content = new FormUrlEncodedContent(formData);

            var response = await _httpClient.SendAsync(tokenRequest).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            _logger.LogInformation("Reddit OAuth response: {Response}", responseContent);

            var tokenResponse = System.Text.Json.JsonSerializer.Deserialize<RedditOAuthTokenResponse>(
                responseContent,
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            if (tokenResponse?.AccessToken == null)
            {
                _logger.LogError("Token response was null or missing access_token. Response: {Response}", responseContent);
                throw new InvalidOperationException("Failed to obtain Reddit OAuth token");
            }

            _accessToken = tokenResponse.AccessToken;
            _tokenExpiration = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn - 60); // Refresh 1 min early

            _logger.LogInformation("Successfully obtained Reddit OAuth token. Expires in {Seconds}s", tokenResponse.ExpiresIn);

            return _accessToken;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to obtain Reddit OAuth token. Falling back to unauthenticated API.");
            return string.Empty;
        }
    }

    /// <summary>
    /// Fetches top posts from a specific subreddit
    /// </summary>
    /// <param name="subreddit">The subreddit name (without r/)</param>
    /// <param name="timeframe">Time period: hour, day, week, month, year, all</param>
    /// <param name="limit">Maximum number of posts to fetch (max 100)</param>
    /// <returns>List of social media post DTOs</returns>
    public async Task<List<CreateSocialMediaPostDto>> GetTopPostsAsync(
        string subreddit,
        string timeframe = "month",
        int limit = 25)
    {
        try
        {
            // Get OAuth token
            var accessToken = await GetAccessTokenAsync().ConfigureAwait(false);

            var url = $"{RedditApiBase}/r/{subreddit}/top?t={timeframe}&limit={limit}";
            _logger.LogInformation("Fetching Reddit posts from: {Url}", url);

            var request = new HttpRequestMessage(HttpMethod.Get, url);

            // Add OAuth token if available
            if (!string.IsNullOrEmpty(accessToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }

            var response = await _httpClient.SendAsync(request).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            var redditResponse = await response.Content
                .ReadFromJsonAsync<RedditResponse>()
                .ConfigureAwait(false);

            if (redditResponse?.Data?.Children == null || redditResponse.Data.Children.Count == 0)
            {
                _logger.LogWarning("No posts found for subreddit: {Subreddit}", subreddit);
                return new List<CreateSocialMediaPostDto>();
            }

            var posts = redditResponse.Data.Children
                .Where(child => child.Data != null)
                .Select(child => MapRedditPostToDto(child.Data!, subreddit))
                .ToList();

            _logger.LogInformation("Successfully fetched {Count} posts from r/{Subreddit}", posts.Count, subreddit);
            return posts;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching Reddit posts from r/{Subreddit}", subreddit);
            throw;
        }
    }

    /// <summary>
    /// Searches posts in a specific subreddit
    /// </summary>
    public async Task<List<CreateSocialMediaPostDto>> SearchPostsAsync(
        string subreddit,
        string query,
        string sort = "top",
        string timeframe = "month",
        int limit = 25)
    {
        try
        {
            // Get OAuth token
            var accessToken = await GetAccessTokenAsync().ConfigureAwait(false);

            var url = $"{RedditApiBase}/r/{subreddit}/search?q={Uri.EscapeDataString(query)}&restrict_sr=1&sort={sort}&t={timeframe}&limit={limit}";
            _logger.LogInformation("Searching Reddit: {Url}", url);

            var request = new HttpRequestMessage(HttpMethod.Get, url);

            // Add OAuth token if available
            if (!string.IsNullOrEmpty(accessToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }

            var response = await _httpClient.SendAsync(request, CancellationToken.None).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            var redditResponse = await response.Content
                .ReadFromJsonAsync<RedditResponse>()
                .ConfigureAwait(false);

            if (redditResponse?.Data?.Children == null || redditResponse.Data.Children.Count == 0)
            {
                _logger.LogWarning("No search results for query: {Query} in r/{Subreddit}", query, subreddit);
                return new List<CreateSocialMediaPostDto>();
            }

            var posts = redditResponse.Data.Children
                .Where(child => child.Data != null)
                .Select(child => MapRedditPostToDto(child.Data!, subreddit))
                .ToList();

            _logger.LogInformation("Found {Count} posts for query: {Query}", posts.Count, query);
            return posts;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching Reddit posts in r/{Subreddit}", subreddit);
            throw;
        }
    }

    private static CreateSocialMediaPostDto MapRedditPostToDto(RedditPostData data, string subreddit)
    {
        var imageUrls = new List<string>();

        // Extract image URLs
        if (!string.IsNullOrEmpty(data.Url))
        {
            // Direct image links
            if (data.Url.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                data.Url.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                data.Url.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                data.Url.EndsWith(".gif", StringComparison.OrdinalIgnoreCase))
            {
                imageUrls.Add(data.Url);
            }

            // Thumbnail
            else if (!string.IsNullOrEmpty(data.Thumbnail) &&
                     data.Thumbnail.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                imageUrls.Add(data.Thumbnail);
            }
        }

        // Preview images
        if (data.Preview?.Images != null && data.Preview.Images.Count > 0)
        {
            var previewUrls = data.Preview.Images
                .Where(img => !string.IsNullOrEmpty(img.Source?.Url))
                .Select(img => System.Net.WebUtility.HtmlDecode(img.Source!.Url)!)
                .Where(decodedUrl => !imageUrls.Contains(decodedUrl, StringComparer.Ordinal));

            imageUrls.AddRange(previewUrls);
        }

        return new CreateSocialMediaPostDto
        {
            Platform = "Reddit",
            ExternalId = data.Id ?? string.Empty,
            Title = data.Title ?? string.Empty,
            Content = data.Selftext ?? string.Empty,
            Author = data.Author ?? "unknown",
            AuthorUrl = $"https://reddit.com/u/{data.Author}",
            AuthorAvatar = string.Empty, // Reddit API doesn't provide author avatars in listing
            PostUrl = $"https://reddit.com{data.Permalink}",
            ImageUrls = imageUrls.ToArray(),
            VideoUrl = null,
            Upvotes = data.Ups ?? 0,
            Downvotes = data.Downs ?? 0,
            CommentCount = data.NumComments ?? 0,
            ShareCount = 0, // Reddit API doesn't provide share count
            Tags = new[] { subreddit },
            Category = subreddit,
            PostedAt = DateTimeOffset.FromUnixTimeSeconds((long)(data.CreatedUtc ?? 0)).UtcDateTime,
            Priority = CalculatePriority(data.Ups ?? 0, data.NumComments ?? 0),
            Language = "en",
        };
    }

    private static int CalculatePriority(int upvotes, int comments)
    {
        // Calculate priority based on engagement (0-100 scale)
        var score = upvotes + (comments * 2); // Comments are weighted more

        if (score >= 1000)
        {
            return 90;
        }

        if (score >= 500)
        {
            return 80;
        }

        if (score >= 200)
        {
            return 70;
        }

        if (score >= 100)
        {
            return 60;
        }

        if (score >= 50)
        {
            return 50;
        }

        if (score >= 20)
        {
            return 40;
        }

        if (score >= 10)
        {
            return 30;
        }

        return 20;
    }
}

internal sealed class RedditResponse
{
    public string? Kind { get; set; }

    public RedditData? Data { get; set; }
}

internal sealed class RedditData
{
    public List<RedditChild>? Children { get; set; }

    public string? After { get; set; }

    public string? Before { get; set; }
}

internal sealed class RedditChild
{
    public string? Kind { get; set; }

    public RedditPostData? Data { get; set; }
}

internal sealed class RedditPostData
{
    public string? Id { get; set; }

    public string? Title { get; set; }

    public string? Selftext { get; set; }

    public string? Author { get; set; }

    public string? Permalink { get; set; }

    public string? Url { get; set; }

    public string? Thumbnail { get; set; }

    public int? Ups { get; set; }

    public int? Downs { get; set; }

    public int? NumComments { get; set; }

    public double? CreatedUtc { get; set; }

    public RedditPreview? Preview { get; set; }
}

internal sealed class RedditPreview
{
    public List<RedditImage>? Images { get; set; }
}

internal sealed class RedditImage
{
    public RedditImageSource? Source { get; set; }
}

internal sealed class RedditImageSource
{
    public string? Url { get; set; }

    public int? Width { get; set; }

    public int? Height { get; set; }
}
