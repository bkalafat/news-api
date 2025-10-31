using System.Text.Json.Serialization;

namespace NewsApi.Infrastructure.Services;

/// <summary>
/// Reddit OAuth token response
/// </summary>
internal sealed class RedditOAuthTokenResponse
{
    [JsonPropertyName("access_token")]
    public string? AccessToken { get; set; }

    [JsonPropertyName("token_type")]
    public string? TokenType { get; set; }

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }

    [JsonPropertyName("scope")]
    public string? Scope { get; set; }
}
