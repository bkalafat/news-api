using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace NewsApi.Domain.Entities;

/// <summary>
/// Represents a news article entity with full content and metadata.
/// </summary>
public sealed class NewsArticle
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    public string Category { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public string Caption { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets SEO-friendly URL slug generated from Caption
    /// </summary>
    public string Slug { get; set; } = string.Empty;

    public string Keywords { get; set; } = string.Empty;

    public string SocialTags { get; set; } = string.Empty;

    public string Summary { get; set; } = string.Empty;

    public string ImgPath { get; set; } = string.Empty;

    public string ImgAlt { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets full-size image URL from MinIO storage
    /// </summary>
    public string ImageUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets thumbnail image URL from MinIO storage (optimized for lists/previews)
    /// </summary>
    public string ThumbnailUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets metadata about the uploaded image
    /// </summary>
    public ImageMetadata? ImageMetadata { get; set; }

    public string Content { get; set; } = string.Empty;

    public string[] Subjects { get; set; } = [];

    public string[] Authors { get; set; } = [];

    public DateTime ExpressDate { get; set; }

    public DateTime CreateDate { get; set; }

    public DateTime UpdateDate { get; set; }

    public int Priority { get; set; }

    public bool IsActive { get; set; }

    public int ViewCount { get; set; }

    public bool IsSecondPageNews { get; set; }
}
