namespace NewsApi.Infrastructure.Configuration;

/// <summary>
/// Configuration settings for Reddit OAuth authentication
/// </summary>
public sealed class RedditSettings
{
    /// <summary>
    /// Reddit app client ID (found under "personal use script")
    /// </summary>
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// Reddit app client secret
    /// </summary>
    public string ClientSecret { get; set; } = string.Empty;

    /// <summary>
    /// Reddit account username for OAuth authentication
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Reddit account password for OAuth authentication
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// User agent string sent with Reddit API requests
    /// </summary>
    public string UserAgent { get; set; } = "NewsPortal/2.0 (OAuth Social Media Aggregator)";
}
