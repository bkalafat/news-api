using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NewsApi.Domain.Entities;

namespace NewsApi.Domain.Interfaces;

/// <summary>
/// Interface for image storage operations using MinIO/S3
/// </summary>
public interface IImageStorageService
{
    /// <summary>
    /// Upload an image to MinIO storage
    /// </summary>
    /// <param name="newsId">The news article ID</param>
    /// <param name="image">The image file to upload</param>
    /// <param name="generateThumbnail">Whether to generate a thumbnail</param>
    /// <param name="altText">Alternative text for accessibility</param>
    /// <returns>Image metadata including URLs</returns>
    Task<ImageMetadata> UploadImageAsync(
        string newsId,
        IFormFile image,
        bool generateThumbnail = true,
        string? altText = null
    );

    /// <summary>
    /// Delete an image from MinIO storage
    /// </summary>
    /// <param name="objectKey">The MinIO object key</param>
    Task DeleteImageAsync(string objectKey);

    /// <summary>
    /// Delete both the image and thumbnail
    /// </summary>
    /// <param name="imageMetadata">The image metadata containing object keys</param>
    Task DeleteImageWithThumbnailAsync(ImageMetadata imageMetadata);

    /// <summary>
    /// Generate a pre-signed URL for temporary access to an image
    /// </summary>
    /// <param name="objectKey">The MinIO object key</param>
    /// <param name="expirySeconds">URL expiry time in seconds (default: 3600)</param>
    /// <returns>Pre-signed URL</returns>
    Task<string> GetPresignedUrlAsync(string objectKey, int expirySeconds = 3600);

    /// <summary>
    /// Check if an image exists in storage
    /// </summary>
    /// <param name="objectKey">The MinIO object key</param>
    /// <returns>True if image exists</returns>
    Task<bool> ImageExistsAsync(string objectKey);

    /// <summary>
    /// Get the public URL for an image
    /// </summary>
    /// <param name="objectKey">The MinIO object key</param>
    /// <returns>Public URL for the image</returns>
    string GetImageUrl(string objectKey);

    /// <summary>
    /// Get the public URL for a thumbnail image
    /// </summary>
    /// <param name="newsId">The news article ID</param>
    /// <param name="extension">File extension (e.g., ".jpg")</param>
    /// <returns>Public URL for the thumbnail</returns>
    string GetThumbnailUrl(string newsId, string extension);
}
