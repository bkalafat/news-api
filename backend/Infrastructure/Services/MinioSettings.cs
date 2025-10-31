namespace NewsApi.Infrastructure.Services;

/// <summary>
/// Configuration settings for MinIO object storage
/// </summary>
internal sealed class MinioSettings
{
    /// <summary>
    /// Gets or sets minIO server endpoint (e.g., localhost:9000)
    /// </summary>
    public string Endpoint { get; set; } = "localhost:9000";

    /// <summary>
    /// Gets or sets minIO access key (username)
    /// </summary>
    public string AccessKey { get; set; } = "minioadmin";

    /// <summary>
    /// Gets or sets minIO secret key (password)
    /// </summary>
    public string SecretKey { get; set; } = "minioadmin123";

    /// <summary>
    /// Gets or sets default bucket name for storing images
    /// </summary>
    public string BucketName { get; set; } = "news-images";

    /// <summary>
    /// Gets or sets a value indicating whether whether to use SSL/TLS for connections
    /// </summary>
    public bool UseSSL { get; set; }

    /// <summary>
    /// Gets or sets aWS region (for S3 compatibility)
    /// </summary>
    public string Region { get; set; } = "us-east-1";

    /// <summary>
    /// Gets or sets maximum allowed file size in bytes (default: 5MB)
    /// </summary>
    public long MaxFileSizeBytes { get; set; } = 5 * 1024 * 1024;

    /// <summary>
    /// Gets or sets thumbnail width in pixels
    /// </summary>
    public int ThumbnailWidth { get; set; } = 400;

    /// <summary>
    /// Gets or sets thumbnail height in pixels
    /// </summary>
    public int ThumbnailHeight { get; set; } = 300;
}
