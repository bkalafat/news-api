using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace NewsApi.Application.DTOs;

/// <summary>
/// DTO for uploading an image to a news article
/// </summary>
public class ImageUploadDto
{
    /// <summary>
    /// The image file to upload
    /// </summary>
    [Required]
    public IFormFile Image { get; set; } = null!;

    /// <summary>
    /// Whether to generate a thumbnail (default: true)
    /// </summary>
    public bool GenerateThumbnail { get; set; } = true;

    /// <summary>
    /// Alternative text for the image (accessibility)
    /// </summary>
    [StringLength(200)]
    public string? AltText { get; set; }
}
