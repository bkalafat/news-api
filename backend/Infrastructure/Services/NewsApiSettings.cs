namespace NewsApi.Infrastructure.Services;

/// <summary>
/// Configuration settings for NewsAPI.org integration
/// </summary>
internal sealed class NewsApiSettings
{
    /// <summary>
    /// Gets or sets aPI key for NewsAPI.org (get from https://newsapi.org/register)
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets base URL for NewsAPI.org
    /// </summary>
    public string BaseUrl { get; set; } = "https://newsapi.org/v2";

    /// <summary>
    /// Gets or sets countries to fetch news from (e.g., "tr" for Turkey, "us" for USA)
    /// </summary>
    public string[] Countries { get; set; } = ["tr", "us"];

    /// <summary>
    /// Gets or sets categories to fetch (business, entertainment, general, health, science, sports, technology)
    /// </summary>
    public string[] Categories { get; set; } = [];

    /// <summary>
    /// Gets or sets maximum articles to fetch per category per run
    /// </summary>
    public int MaxArticlesPerCategory { get; set; } = 10;

    /// <summary>
    /// Gets or sets language filter (e.g., "tr" for Turkish, "en" for English)
    /// </summary>
    public string Language { get; set; } = "tr";
}
