using System;

namespace NewsApi.Domain.Entities;

/// <summary>
/// Metadata for uploaded images stored in MinIO
/// </summary>
internal sealed class ImageMetadata
{
    /// <summary>
    /// Gets or sets original file name
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets mIME content type (e.g., image/jpeg, image/png)
    /// </summary>
    public string ContentType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets file size in bytes
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// Gets or sets image width in pixels
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    /// Gets or sets image height in pixels
    /// </summary>
    public int Height { get; set; }

    /// <summary>
    /// Gets or sets when the image was uploaded to MinIO
    /// </summary>
    public DateTime UploadedAt { get; set; }

    /// <summary>
    /// Gets or sets minIO object key/path for the image
    /// </summary>
    public string MinioObjectKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets alternative text for accessibility
    /// </summary>
    public string AltText { get; set; } = string.Empty;
}
