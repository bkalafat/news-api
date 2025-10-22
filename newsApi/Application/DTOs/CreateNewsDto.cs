using System;
using System.ComponentModel.DataAnnotations;

namespace NewsApi.Application.DTOs;

public class CreateNewsDto
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
    /// Full-size image URL (can be external URL or MinIO URL)
    /// </summary>
    [StringLength(1000)]
    public string ImageUrl { get; set; } = string.Empty;
    
    /// <summary>
    /// Thumbnail image URL (can be external URL or MinIO URL)
    /// </summary>
    [StringLength(1000)]
    public string ThumbnailUrl { get; set; } = string.Empty;
    
    [Required]
    public string Content { get; set; } = string.Empty;
    
    public string[] Subjects { get; set; } = Array.Empty<string>();
    
    public string[] Authors { get; set; } = Array.Empty<string>();
    
    [Required]
    public DateTime ExpressDate { get; set; }
    
    [Range(1, 100)]
    public int Priority { get; set; } = 1;
    
    public bool IsActive { get; set; } = true;
    
    [StringLength(500)]
    public string Url { get; set; } = string.Empty;
    
    public bool IsSecondPageNews { get; set; } = false;
}