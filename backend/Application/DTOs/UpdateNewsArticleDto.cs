using System;
using System.ComponentModel.DataAnnotations;

namespace NewsApi.Application.DTOs;

/// <summary>
/// Data transfer object for updating an existing news article.
/// All properties are optional to support partial updates.
/// </summary>
internal sealed record UpdateNewsArticleDto
{
    [StringLength(100)]
    public string? Category { get; set; }

    [StringLength(50)]
    public string? Type { get; set; }

    [StringLength(500)]
    public string? Caption { get; set; }

    [StringLength(1000)]
    public string? Keywords { get; set; }

    [StringLength(500)]
    public string? SocialTags { get; set; }

    [StringLength(2000)]
    public string? Summary { get; set; }

    [StringLength(500)]
    public string? ImgPath { get; set; }

    [StringLength(200)]
    public string? ImgAlt { get; set; }

    /// <summary>
    /// Gets or sets full-size image URL (can be external URL or MinIO URL)
    /// </summary>
    [StringLength(1000)]
    public string? ImageUrl { get; set; }

    /// <summary>
    /// Gets or sets thumbnail image URL (can be external URL or MinIO URL)
    /// </summary>
    [StringLength(1000)]
    public string? ThumbnailUrl { get; set; }

    public string? Content { get; set; }

    public string[]? Subjects { get; set; }

    public string[]? Authors { get; set; }

    public DateTime? ExpressDate { get; set; }

    [Range(1, 100)]
    public int? Priority { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsSecondPageNews { get; set; }
}
