using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace NewsApi.Domain.Entities;

/// <summary>
/// Represents a social media post aggregated from various platforms
/// </summary>
public class SocialMediaPost
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    /// <summary>
    /// Platform name: Reddit, Twitter, LinkedIn, Facebook, Instagram, YouTube
    /// </summary>
    public string Platform { get; set; } = string.Empty;

    /// <summary>
    /// Original post ID from the platform
    /// </summary>
    public string ExternalId { get; set; } = string.Empty;

    /// <summary>
    /// Post title or tweet text
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Full content/body of the post
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Author username or display name
    /// </summary>
    public string Author { get; set; } = string.Empty;

    /// <summary>
    /// Author's profile URL
    /// </summary>
    public string AuthorUrl { get; set; } = string.Empty;

    /// <summary>
    /// Author's avatar/profile picture URL
    /// </summary>
    public string AuthorAvatar { get; set; } = string.Empty;

    /// <summary>
    /// Direct URL to the post on the platform
    /// </summary>
    public string PostUrl { get; set; } = string.Empty;

    /// <summary>
    /// Image URLs associated with the post
    /// </summary>
    public string[] ImageUrls { get; set; } = [];

    /// <summary>
    /// Video URL if applicable
    /// </summary>
    public string? VideoUrl { get; set; }

    /// <summary>
    /// Number of upvotes/likes
    /// </summary>
    public int Upvotes { get; set; }

    /// <summary>
    /// Number of downvotes (Reddit specific)
    /// </summary>
    public int Downvotes { get; set; }

    /// <summary>
    /// Number of comments/replies
    /// </summary>
    public int CommentCount { get; set; }

    /// <summary>
    /// Number of shares/retweets
    /// </summary>
    public int ShareCount { get; set; }

    /// <summary>
    /// Tags or hashtags from the post
    /// </summary>
    public string[] Tags { get; set; } = [];

    /// <summary>
    /// Subreddit, hashtag category, or group name
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// When the post was created on the platform
    /// </summary>
    public DateTime PostedAt { get; set; }

    /// <summary>
    /// When we fetched this post
    /// </summary>
    public DateTime FetchedAt { get; set; }

    /// <summary>
    /// Last time we updated the metrics (likes, comments, etc.)
    /// </summary>
    public DateTime? LastUpdated { get; set; }

    /// <summary>
    /// Whether this post is currently active/visible
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Priority for display (higher = more important)
    /// </summary>
    public int Priority { get; set; }

    /// <summary>
    /// Language of the post content
    /// </summary>
    public string Language { get; set; } = "en";

    /// <summary>
    /// Additional metadata specific to the platform
    /// </summary>
    [BsonExtraElements]
    public BsonDocument? Metadata { get; set; }
}
