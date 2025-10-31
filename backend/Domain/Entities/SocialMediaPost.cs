using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace NewsApi.Domain.Entities;

/// <summary>
/// Represents a social media post aggregated from various platforms
/// </summary>
public sealed class SocialMediaPost
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    /// <summary>
    /// Gets or sets platform name: Reddit, Twitter, LinkedIn, Facebook, Instagram, YouTube
    /// </summary>
    public string Platform { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets original post ID from the platform
    /// </summary>
    public string ExternalId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets post title or tweet text
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets full content/body of the post
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets author username or display name
    /// </summary>
    public string Author { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets author's profile URL
    /// </summary>
    public string AuthorUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets author's avatar/profile picture URL
    /// </summary>
    public string AuthorAvatar { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets direct URL to the post on the platform
    /// </summary>
    public string PostUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets image URLs associated with the post
    /// </summary>
    public string[] ImageUrls { get; set; } = [];

    /// <summary>
    /// Gets or sets video URL if applicable
    /// </summary>
    public string? VideoUrl { get; set; }

    /// <summary>
    /// Gets or sets number of upvotes/likes
    /// </summary>
    public int Upvotes { get; set; }

    /// <summary>
    /// Gets or sets number of downvotes (Reddit specific)
    /// </summary>
    public int Downvotes { get; set; }

    /// <summary>
    /// Gets or sets number of comments/replies
    /// </summary>
    public int CommentCount { get; set; }

    /// <summary>
    /// Gets or sets number of shares/retweets
    /// </summary>
    public int ShareCount { get; set; }

    /// <summary>
    /// Gets or sets tags or hashtags from the post
    /// </summary>
    public string[] Tags { get; set; } = [];

    /// <summary>
    /// Gets or sets subreddit, hashtag category, or group name
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets when the post was created on the platform
    /// </summary>
    public DateTime PostedAt { get; set; }

    /// <summary>
    /// Gets or sets when we fetched this post
    /// </summary>
    public DateTime FetchedAt { get; set; }

    /// <summary>
    /// Gets or sets last time we updated the metrics (likes, comments, etc.)
    /// </summary>
    public DateTime? LastUpdated { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether whether this post is currently active/visible
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets priority for display (higher = more important)
    /// </summary>
    public int Priority { get; set; }

    /// <summary>
    /// Gets or sets language of the post content
    /// </summary>
    public string Language { get; set; } = "en";

    /// <summary>
    /// Gets or sets additional metadata specific to the platform
    /// </summary>
    [BsonExtraElements]
    public BsonDocument? Metadata { get; set; }
}
