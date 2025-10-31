using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NewsApi.Domain.Entities;
using NewsApi.Domain.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace NewsApi.Infrastructure.Services;

/// <summary>
/// Service for downloading images from URLs and storing them in MinIO
/// </summary>
internal sealed class ImageDownloadService
{
    private readonly HttpClient _httpClient;
    private readonly IImageStorageService _imageStorageService;
    private readonly MinioSettings _settings;
    private readonly ILogger<ImageDownloadService> _logger;

    public ImageDownloadService(
        HttpClient httpClient,
        IImageStorageService imageStorageService,
        MinioSettings settings,
        ILogger<ImageDownloadService> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _imageStorageService = imageStorageService ?? throw new ArgumentNullException(nameof(imageStorageService));
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Downloads an image from a URL and uploads it to MinIO storage
    /// </summary>
    /// <param name="newsId">The news article ID (used as object key)</param>
    /// <param name="imageUrl">The source URL of the image</param>
    /// <param name="altText">Alt text for the image</param>
    /// <returns>Image metadata with MinIO URLs, or null if download/upload failed</returns>
    public async Task<ImageMetadata?> DownloadAndUploadImageAsync(
        string newsId,
        string imageUrl,
        string? altText = null)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
        {
            return null;
        }

        try
        {
            _logger.LogInformation("Downloading image from: {Url}", imageUrl);

            // Download image from URL
            var imageBytes = await _httpClient.GetByteArrayAsync(imageUrl);

            if (imageBytes == null || imageBytes.Length == 0)
            {
                _logger.LogWarning("Downloaded image is empty: {Url}", imageUrl);
                return null;
            }

            // Validate image size
            if (imageBytes.Length > _settings.MaxFileSizeBytes)
            {
                _logger.LogWarning(
                    "Downloaded image too large ({Size} bytes), max is {Max} bytes: {Url}",
                    imageBytes.Length,
                    _settings.MaxFileSizeBytes,
                    imageUrl);
                return null;
            }

            // Validate and process image
            using var imageStream = new MemoryStream(imageBytes);
            using var image = await Image.LoadAsync(imageStream);

            var width = image.Width;
            var height = image.Height;

            // Determine file extension and content type
            var extension = ".jpg";
            var contentType = "image/jpeg";

            // Upload original image to MinIO
            var objectKey = $"{newsId}{extension}";
            var thumbnailKey = $"{newsId}-thumb{extension}";

            using var uploadStream = new MemoryStream(imageBytes);
            await _imageStorageService.UploadImageStreamAsync(
                objectKey,
                uploadStream,
                contentType,
                imageBytes.Length);

            _logger.LogInformation("Uploaded image to MinIO: {ObjectKey}", objectKey);

            // Generate and upload thumbnail
            var thumbnailBytes = await GenerateThumbnailAsync(
                imageBytes,
                _settings.ThumbnailWidth,
                _settings.ThumbnailHeight);

            using var thumbnailStream = new MemoryStream(thumbnailBytes);
            await _imageStorageService.UploadImageStreamAsync(
                thumbnailKey,
                thumbnailStream,
                contentType,
                thumbnailBytes.Length);

            _logger.LogInformation("Uploaded thumbnail to MinIO: {ThumbnailKey}", thumbnailKey);

            // Build image metadata
            var metadata = new ImageMetadata
            {
                FileName = Path.GetFileName(imageUrl),
                ContentType = contentType,
                FileSize = imageBytes.Length,
                Width = width,
                Height = height,
                UploadedAt = DateTime.UtcNow,
                MinioObjectKey = objectKey,
                AltText = altText ?? string.Empty,
            };

            return metadata;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning(ex, "Failed to download image from URL: {Url}", imageUrl);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading/uploading image from URL: {Url}", imageUrl);
            return null;
        }
    }

    /// <summary>
    /// Try to extract an image URL from various sources in aggregated news
    /// </summary>
    public string? ExtractImageUrl(string? externalUrl, string? sourceUrl, string source)
    {
        // For Reddit posts, we might have image URLs
        // For other sources, check if externalUrl is an image
        if (!string.IsNullOrEmpty(externalUrl))
        {
            var lower = externalUrl.ToLowerInvariant();
            if (lower.EndsWith(".jpg") || lower.EndsWith(".jpeg") ||
                lower.EndsWith(".png") || lower.EndsWith(".webp") ||
                lower.EndsWith(".gif"))
            {
                return externalUrl;
            }
        }

        // For sources like Unsplash (could be added in future)
        if (source.Contains("Unsplash", StringComparison.OrdinalIgnoreCase))
        {
            return externalUrl;
        }

        return null;
    }

    /// <summary>
    /// Generate a thumbnail from image bytes
    /// </summary>
    private static async Task<byte[]> GenerateThumbnailAsync(byte[] imageBytes, int width, int height)
    {
        using var image = await Image.LoadAsync(new MemoryStream(imageBytes));

        // Resize image maintaining aspect ratio
        image.Mutate(transform => transform.Resize(new ResizeOptions
        {
            Size = new Size(width, height),
            Mode = ResizeMode.Max
        }));

        // Save to memory stream
        using var outputStream = new MemoryStream();
        await image.SaveAsync(outputStream, new JpegEncoder { Quality = 80 });

        return outputStream.ToArray();
    }
}
