namespace NewsApi.Infrastructure.Services;

/// <summary>
/// Configuration settings for NewsAPI.org integration
/// </summary>
public class NewsApiSettings
{
    /// <summary>
    /// API key for NewsAPI.org (get from https://newsapi.org/register)
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Base URL for NewsAPI.org
    /// </summary>
    public string BaseUrl { get; set; } = "https://newsapi.org/v2";

    /// <summary>
    /// Countries to fetch news from (e.g., "tr" for Turkey, "us" for USA)
    /// </summary>
    public string[] Countries { get; set; } = ["tr", "us"];

    /// <summary>
    /// Categories to fetch (business, entertainment, general, health, science, sports, technology)
    /// </summary>
    public string[] Categories { get; set; } = ["technology", "business", "sports", "science", "health", "entertainment"];

    /// <summary>
    /// Maximum articles to fetch per category per run
    /// </summary>
    public int MaxArticlesPerCategory { get; set; } = 10;

    /// <summary>
    /// Language filter (e.g., "tr" for Turkish, "en" for English)
    /// </summary>
    public string Language { get; set; } = "tr";
}
