namespace NewsApi.Infrastructure.Services;

/// <summary>
/// Configuration settings for MinIO object storage
/// </summary>
public class MinioSettings
{
    /// <summary>
    /// MinIO server endpoint (e.g., localhost:9000)
    /// </summary>
    public string Endpoint { get; set; } = "localhost:9000";
    
    /// <summary>
    /// MinIO access key (username)
    /// </summary>
    public string AccessKey { get; set; } = "minioadmin";
    
    /// <summary>
    /// MinIO secret key (password)
    /// </summary>
    public string SecretKey { get; set; } = "minioadmin123";
    
    /// <summary>
    /// Default bucket name for storing images
    /// </summary>
    public string BucketName { get; set; } = "news-images";
    
    /// <summary>
    /// Whether to use SSL/TLS for connections
    /// </summary>
    public bool UseSSL { get; set; } = false;
    
    /// <summary>
    /// AWS region (for S3 compatibility)
    /// </summary>
    public string Region { get; set; } = "us-east-1";
    
    /// <summary>
    /// Maximum allowed file size in bytes (default: 5MB)
    /// </summary>
    public long MaxFileSizeBytes { get; set; } = 5 * 1024 * 1024;
    
    /// <summary>
    /// Thumbnail width in pixels
    /// </summary>
    public int ThumbnailWidth { get; set; } = 400;
    
    /// <summary>
    /// Thumbnail height in pixels
    /// </summary>
    public int ThumbnailHeight { get; set; } = 300;
}
