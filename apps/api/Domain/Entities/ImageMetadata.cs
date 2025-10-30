using System;

namespace NewsApi.Domain.Entities;

/// <summary>
/// Metadata for uploaded images stored in MinIO
/// </summary>
public class ImageMetadata
{
    /// <summary>
    /// Original file name
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// MIME content type (e.g., image/jpeg, image/png)
    /// </summary>
    public string ContentType { get; set; } = string.Empty;

    /// <summary>
    /// File size in bytes
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// Image width in pixels
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    /// Image height in pixels
    /// </summary>
    public int Height { get; set; }

    /// <summary>
    /// When the image was uploaded to MinIO
    /// </summary>
    public DateTime UploadedAt { get; set; }

    /// <summary>
    /// MinIO object key/path for the image
    /// </summary>
    public string MinioObjectKey { get; set; } = string.Empty;

    /// <summary>
    /// Alternative text for accessibility
    /// </summary>
    public string AltText { get; set; } = string.Empty;
}
