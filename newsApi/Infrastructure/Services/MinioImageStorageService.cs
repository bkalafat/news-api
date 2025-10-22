using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel.Args;
using NewsApi.Domain.Entities;
using NewsApi.Domain.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using System;
using System.IO;
using System.Threading.Tasks;

namespace NewsApi.Infrastructure.Services;

/// <summary>
/// Service for managing image storage in MinIO/S3
/// </summary>
public class MinioImageStorageService : IImageStorageService
{
    private readonly IMinioClient _minioClient;
    private readonly MinioSettings _settings;
    private readonly ILogger<MinioImageStorageService> _logger;

    public MinioImageStorageService(MinioSettings settings, ILogger<MinioImageStorageService> logger)
    {
        _settings = settings;
        _logger = logger;
        
        // Initialize MinIO client
        _minioClient = new MinioClient()
            .WithEndpoint(_settings.Endpoint)
            .WithCredentials(_settings.AccessKey, _settings.SecretKey)
            .WithSSL(_settings.UseSSL)
            .Build();
    }

    /// <inheritdoc />
    public async Task<ImageMetadata> UploadImageAsync(string newsId, IFormFile image, bool generateThumbnail = true, string? altText = null)
    {
        // Validate image
        ValidateImage(image);

        // Ensure bucket exists
        await EnsureBucketExistsAsync();

        // Generate object keys
        var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
        var objectKey = $"{newsId}{extension}";
        var thumbnailKey = $"{newsId}-thumb{extension}";

        // Upload original image
        using var imageStream = image.OpenReadStream();
        var imageBytes = await ReadStreamToBytes(imageStream);
        
        // Get image dimensions
        using var img = Image.Load(imageBytes);
        var width = img.Width;
        var height = img.Height;

        // Upload to MinIO
        await using var uploadStream = new MemoryStream(imageBytes);
        await _minioClient.PutObjectAsync(new PutObjectArgs()
            .WithBucket(_settings.BucketName)
            .WithObject(objectKey)
            .WithStreamData(uploadStream)
            .WithObjectSize(imageBytes.Length)
            .WithContentType(image.ContentType));

        _logger.LogInformation("Uploaded image {ObjectKey} to MinIO", objectKey);

        // Generate and upload thumbnail if requested
        if (generateThumbnail)
        {
            var thumbnailBytes = await GenerateThumbnailAsync(imageBytes, _settings.ThumbnailWidth, _settings.ThumbnailHeight);
            await using var thumbnailStream = new MemoryStream(thumbnailBytes);
            
            await _minioClient.PutObjectAsync(new PutObjectArgs()
                .WithBucket(_settings.BucketName)
                .WithObject(thumbnailKey)
                .WithStreamData(thumbnailStream)
                .WithObjectSize(thumbnailBytes.Length)
                .WithContentType(image.ContentType));

            _logger.LogInformation("Uploaded thumbnail {ThumbnailKey} to MinIO", thumbnailKey);
        }

        // Build image metadata
        var metadata = new ImageMetadata
        {
            FileName = image.FileName,
            ContentType = image.ContentType,
            FileSize = image.Length,
            Width = width,
            Height = height,
            UploadedAt = DateTime.UtcNow,
            MinioObjectKey = objectKey,
            AltText = altText ?? ""
        };

        return metadata;
    }

    /// <inheritdoc />
    public async Task DeleteImageAsync(string objectKey)
    {
        try
        {
            await _minioClient.RemoveObjectAsync(new RemoveObjectArgs()
                .WithBucket(_settings.BucketName)
                .WithObject(objectKey));

            _logger.LogInformation("Deleted image {ObjectKey} from MinIO", objectKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting image {ObjectKey} from MinIO", objectKey);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task DeleteImageWithThumbnailAsync(ImageMetadata imageMetadata)
    {
        // Delete original image
        await DeleteImageAsync(imageMetadata.MinioObjectKey);

        // Delete thumbnail
        var extension = Path.GetExtension(imageMetadata.FileName).ToLowerInvariant();
        var newsId = Path.GetFileNameWithoutExtension(imageMetadata.MinioObjectKey);
        var thumbnailKey = $"{newsId}-thumb{extension}";
        
        await DeleteImageAsync(thumbnailKey);
    }

    /// <inheritdoc />
    public async Task<string> GetPresignedUrlAsync(string objectKey, int expirySeconds = 3600)
    {
        var args = new PresignedGetObjectArgs()
            .WithBucket(_settings.BucketName)
            .WithObject(objectKey)
            .WithExpiry(expirySeconds);

        var url = await _minioClient.PresignedGetObjectAsync(args);
        return url;
    }

    /// <inheritdoc />
    public async Task<bool> ImageExistsAsync(string objectKey)
    {
        try
        {
            await _minioClient.StatObjectAsync(new StatObjectArgs()
                .WithBucket(_settings.BucketName)
                .WithObject(objectKey));
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Build the public URL for an object
    /// </summary>
    public string GetPublicUrl(string objectKey)
    {
        var protocol = _settings.UseSSL ? "https" : "http";
        return $"{protocol}://{_settings.Endpoint}/{_settings.BucketName}/{objectKey}";
    }

    /// <summary>
    /// Validate the uploaded image
    /// </summary>
    private void ValidateImage(IFormFile image)
    {
        if (image == null || image.Length == 0)
        {
            throw new ArgumentException("Image file is required");
        }

        if (image.Length > _settings.MaxFileSizeBytes)
        {
            throw new ArgumentException($"Image size exceeds maximum allowed size of {_settings.MaxFileSizeBytes / (1024 * 1024)}MB");
        }

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
        var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
        
        if (!Array.Exists(allowedExtensions, ext => ext == extension))
        {
            throw new ArgumentException($"Image format not supported. Allowed formats: {string.Join(", ", allowedExtensions)}");
        }

        var allowedContentTypes = new[] { "image/jpeg", "image/png", "image/webp", "image/gif" };
        if (!Array.Exists(allowedContentTypes, ct => ct == image.ContentType))
        {
            throw new ArgumentException($"Invalid content type. Allowed types: {string.Join(", ", allowedContentTypes)}");
        }
    }

    /// <summary>
    /// Ensure the MinIO bucket exists
    /// </summary>
    private async Task EnsureBucketExistsAsync()
    {
        var bucketExists = await _minioClient.BucketExistsAsync(new BucketExistsArgs()
            .WithBucket(_settings.BucketName));

        if (!bucketExists)
        {
            await _minioClient.MakeBucketAsync(new MakeBucketArgs()
                .WithBucket(_settings.BucketName));

            _logger.LogInformation("Created MinIO bucket: {BucketName}", _settings.BucketName);
        }
    }

    /// <summary>
    /// Generate a thumbnail from the original image
    /// </summary>
    private async Task<byte[]> GenerateThumbnailAsync(byte[] imageBytes, int width, int height)
    {
        using var image = Image.Load(imageBytes);
        
        // Resize image maintaining aspect ratio
        image.Mutate(x => x.Resize(new ResizeOptions
        {
            Size = new Size(width, height),
            Mode = ResizeMode.Max
        }));

        // Save to memory stream
        using var outputStream = new MemoryStream();
        await image.SaveAsync(outputStream, new JpegEncoder { Quality = 80 });
        
        return outputStream.ToArray();
    }

    /// <summary>
    /// Read stream to byte array
    /// </summary>
    private async Task<byte[]> ReadStreamToBytes(Stream stream)
    {
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream);
        return memoryStream.ToArray();
    }
}
