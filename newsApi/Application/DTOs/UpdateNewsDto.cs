using System;
using System.ComponentModel.DataAnnotations;

namespace NewsApi.Application.DTOs;

public class UpdateNewsDto
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
    
    public string? Content { get; set; }
    
    public string[]? Subjects { get; set; }
    
    public string[]? Authors { get; set; }
    
    public DateTime? ExpressDate { get; set; }
    
    [Range(1, 100)]
    public int? Priority { get; set; }
    
    public bool? IsActive { get; set; }
    
    [StringLength(500)]
    public string? Url { get; set; }
    
    public bool? IsSecondPageNews { get; set; }
}