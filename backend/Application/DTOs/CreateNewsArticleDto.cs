using System;
using System.ComponentModel.DataAnnotations;

namespace NewsApi.Application.DTOs;

/// <summary>
/// Data transfer object for creating a new news article.
/// </summary>
public sealed record CreateNewsArticleDto
{
    [Required]
    [StringLength(100)]
    public string Category { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Type { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string Caption { get; set; } = string.Empty;

    [StringLength(1000)]
    public string Keywords { get; set; } = string.Empty;

    [StringLength(500)]
    public string SocialTags { get; set; } = string.Empty;

    [Required]
    [StringLength(2000)]
    public string Summary { get; set; } = string.Empty;

    [StringLength(500)]
    public string ImgPath { get; set; } = string.Empty;

    [StringLength(200)]
    public string ImgAlt { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets full-size image URL (can be external URL or MinIO URL)
    /// </summary>
    [StringLength(1000)]
    public string ImageUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets thumbnail image URL (can be external URL or MinIO URL)
    /// </summary>
    [StringLength(1000)]
    public string ThumbnailUrl { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;

    public string[] Subjects { get; set; } = [];

    public string[] Authors { get; set; } = [];

    [Required]
    public DateTime ExpressDate { get; set; }

    [Range(1, 100)]
    public int Priority { get; set; } = 1;

    public bool IsActive { get; set; } = true;

    public bool IsSecondPageNews { get; set; }
}
