using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace NewsApi.Application.DTOs;

/// <summary>
/// DTO for uploading an image to a news article
/// </summary>
internal sealed class ImageUploadDto
{
    /// <summary>
    /// Gets or sets the image file to upload
    /// </summary>
    [Required]
    public IFormFile Image { get; set; } = null!;

    /// <summary>
    /// Gets or sets a value indicating whether whether to generate a thumbnail (default: true)
    /// </summary>
    public bool GenerateThumbnail { get; set; } = true;

    /// <summary>
    /// Gets or sets alternative text for the image (accessibility)
    /// </summary>
    [StringLength(200)]
    public string? AltText { get; set; }
}
