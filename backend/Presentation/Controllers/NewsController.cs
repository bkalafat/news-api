using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsApi.Application.DTOs;
using NewsApi.Application.Services;
using NewsApi.Domain.Entities;
using NewsApi.Domain.Interfaces;

namespace NewsApi.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class NewsController : ControllerBase
{
    private readonly INewsService _newsService;
    private readonly IImageStorageService _imageStorageService;

    public NewsController(INewsService newsService, IImageStorageService imageStorageService)
    {
        _newsService = newsService;
        _imageStorageService = imageStorageService;
    }

    /// <summary>
    /// Get all news articles (public endpoint)
    /// </summary>
    /// <param name="category">Filter by category</param>
    /// <param name="type">Filter by type</param>
    /// <returns>List of news articles</returns>
    [HttpGet]
    [AllowAnonymous] // Public endpoint
    public async Task<ActionResult<List<News>>> GetAllNews(
        [FromQuery] string? category = null,
        [FromQuery] string? type = null
    )
    {
        try
        {
            Console.WriteLine("GetAllNews called");
            var news = await _newsService.GetAllNewsAsync();
            Console.WriteLine($"Retrieved {news.Count} news items");

            // Apply filtering if parameters provided
            if (!string.IsNullOrEmpty(category))
            {
                news = news.FindAll(n => n.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
                Console.WriteLine($"After category filter: {news.Count} items");
            }

            if (!string.IsNullOrEmpty(type))
            {
                news = news.FindAll(n => n.Type.Equals(type, StringComparison.OrdinalIgnoreCase));
                Console.WriteLine($"After type filter: {news.Count} items");
            }

            return Ok(news);
        }
        catch (Exception ex)
        {
            // Log more detailed error information
            Console.WriteLine($"Error in GetAllNews: {ex}");
            return StatusCode(
                500,
                new
                {
                    message = "An error occurred while retrieving news",
                    error = ex.Message,
                    stackTrace = ex.StackTrace,
                }
            );
        }
    }

    /// <summary>
    /// Get news article by ID (public endpoint)
    /// </summary>
    /// <param name="id">News article ID</param>
    /// <returns>News article</returns>
    [HttpGet("{id}")]
    [AllowAnonymous] // Public endpoint
    public async Task<ActionResult<News>> GetNewsById(string id)
    {
        try
        {
            var news = await _newsService.GetNewsByIdAsync(id);

            if (news == null)
            {
                return NotFound(new { message = "News article not found" });
            }

            return Ok(news);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetNewsById: {ex}");
            return StatusCode(
                500,
                new { message = "An error occurred while retrieving the news article", error = ex.Message }
            );
        }
    }

    /// <summary>
    /// Get news article by URL (public endpoint)
    /// </summary>
    /// <param name="url">News article URL</param>
    /// <returns>News article</returns>
    [HttpGet("by-url")]
    [AllowAnonymous] // Public endpoint
    public async Task<ActionResult<News>> GetNewsByUrl([FromQuery] string url)
    {
        try
        {
            if (string.IsNullOrEmpty(url))
            {
                return BadRequest(new { message = "URL parameter is required" });
            }

            var news = await _newsService.GetNewsByUrlAsync(url);

            if (news == null)
            {
                return NotFound(new { message = "News article not found" });
            }

            return Ok(news);
        }
        catch (Exception ex)
        {
            return StatusCode(
                500,
                new { message = "An error occurred while retrieving the news article", error = ex.Message }
            );
        }
    }

    /// <summary>
    /// Create new news article (requires authentication)
    /// </summary>
    /// <param name="createNewsDto">News article data</param>
    /// <returns>Created news article</returns>
    [HttpPost]
    [Authorize] // Protected endpoint
    public async Task<ActionResult<News>> CreateNews([FromBody] CreateNewsDto createNewsDto)
    {
        try
        {
            // Map DTO to entity
            var news = new News
            {
                Category = createNewsDto.Category,
                Type = createNewsDto.Type,
                Caption = createNewsDto.Caption,
                Keywords = createNewsDto.Keywords,
                SocialTags = createNewsDto.SocialTags,
                Summary = createNewsDto.Summary,
                ImgPath = createNewsDto.ImgPath,
                ImgAlt = createNewsDto.ImgAlt,
                Content = createNewsDto.Content,
                Subjects = createNewsDto.Subjects,
                Authors = createNewsDto.Authors,
                ExpressDate = createNewsDto.ExpressDate,
                Priority = createNewsDto.Priority,
                IsActive = createNewsDto.IsActive,
                Url = createNewsDto.Url,
                IsSecondPageNews = createNewsDto.IsSecondPageNews,
            };

            var createdNews = await _newsService.CreateNewsAsync(news);
            return CreatedAtAction(nameof(GetNewsById), new { id = createdNews.Id }, createdNews);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in CreateNews: {ex}");
            return StatusCode(
                500,
                new { message = "An error occurred while creating the news article", error = ex.Message }
            );
        }
    }

    /// <summary>
    /// Update existing news article (requires authentication)
    /// </summary>
    /// <param name="id">News article ID</param>
    /// <param name="updateNewsDto">Updated news article data</param>
    /// <returns>No content</returns>
    [HttpPut("{id}")]
    [Authorize] // Protected endpoint
    public async Task<ActionResult> UpdateNews(string id, [FromBody] UpdateNewsDto updateNewsDto)
    {
        try
        {
            var existingNews = await _newsService.GetNewsByIdAsync(id);
            if (existingNews == null)
            {
                return NotFound(new { message = "News article not found" });
            }

            // Update only provided fields
            if (updateNewsDto.Category != null)
                existingNews.Category = updateNewsDto.Category;
            if (updateNewsDto.Type != null)
                existingNews.Type = updateNewsDto.Type;
            if (updateNewsDto.Caption != null)
                existingNews.Caption = updateNewsDto.Caption;
            if (updateNewsDto.Keywords != null)
                existingNews.Keywords = updateNewsDto.Keywords;
            if (updateNewsDto.SocialTags != null)
                existingNews.SocialTags = updateNewsDto.SocialTags;
            if (updateNewsDto.Summary != null)
                existingNews.Summary = updateNewsDto.Summary;
            if (updateNewsDto.ImgPath != null)
                existingNews.ImgPath = updateNewsDto.ImgPath;
            if (updateNewsDto.ImgAlt != null)
                existingNews.ImgAlt = updateNewsDto.ImgAlt;
            if (updateNewsDto.Content != null)
                existingNews.Content = updateNewsDto.Content;
            if (updateNewsDto.Subjects != null)
                existingNews.Subjects = updateNewsDto.Subjects;
            if (updateNewsDto.Authors != null)
                existingNews.Authors = updateNewsDto.Authors;
            if (updateNewsDto.ExpressDate.HasValue)
                existingNews.ExpressDate = updateNewsDto.ExpressDate.Value;
            if (updateNewsDto.Priority.HasValue)
                existingNews.Priority = updateNewsDto.Priority.Value;
            if (updateNewsDto.IsActive.HasValue)
                existingNews.IsActive = updateNewsDto.IsActive.Value;
            if (updateNewsDto.Url != null)
                existingNews.Url = updateNewsDto.Url;
            if (updateNewsDto.IsSecondPageNews.HasValue)
                existingNews.IsSecondPageNews = updateNewsDto.IsSecondPageNews.Value;
            if (updateNewsDto.ImageUrl != null)
                existingNews.ImageUrl = updateNewsDto.ImageUrl;
            if (updateNewsDto.ThumbnailUrl != null)
                existingNews.ThumbnailUrl = updateNewsDto.ThumbnailUrl;

            await _newsService.UpdateNewsAsync(id, existingNews);
            return NoContent();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in UpdateNews: {ex}");
            return StatusCode(
                500,
                new { message = "An error occurred while updating the news article", error = ex.Message }
            );
        }
    }

    /// <summary>
    /// Delete news article (requires authentication)
    /// </summary>
    /// <param name="id">News article ID</param>
    /// <returns>No content</returns>
    [HttpDelete("{id}")]
    [Authorize] // Protected endpoint
    public async Task<ActionResult> DeleteNews(string id)
    {
        try
        {
            var existingNews = await _newsService.GetNewsByIdAsync(id);
            if (existingNews == null)
            {
                return NotFound(new { message = "News article not found" });
            }

            await _newsService.DeleteNewsAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in DeleteNews: {ex}");
            return StatusCode(
                500,
                new { message = "An error occurred while deleting the news article", error = ex.Message }
            );
        }
    }

    /// <summary>
    /// Upload an image for a news article (requires authentication)
    /// </summary>
    /// <param name="id">News article ID</param>
    /// <param name="imageUpload">Image upload DTO</param>
    /// <returns>Updated news article with image URLs</returns>
    [HttpPost("{id}/image")]
    [Authorize]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(News), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<News>> UploadImage(string id, [FromForm] ImageUploadDto imageUpload)
    {
        try
        {
            // Check if news article exists
            var existingNews = await _newsService.GetNewsByIdAsync(id);
            if (existingNews == null)
            {
                return NotFound(new { message = "News article not found" });
            }

            // Delete old image if exists
            if (existingNews.ImageMetadata != null)
            {
                await _imageStorageService.DeleteImageWithThumbnailAsync(existingNews.ImageMetadata);
            }

            // Upload new image
            var imageMetadata = await _imageStorageService.UploadImageAsync(
                id,
                imageUpload.Image,
                imageUpload.GenerateThumbnail,
                imageUpload.AltText
            );

            // Update news article with image URLs
            existingNews.ImageUrl = GetImageUrl(imageMetadata.MinioObjectKey);
            existingNews.ThumbnailUrl = GetThumbnailUrl(id, Path.GetExtension(imageMetadata.FileName));
            existingNews.ImageMetadata = imageMetadata;
            existingNews.ImgAlt = imageUpload.AltText ?? imageMetadata.AltText;

            await _newsService.UpdateNewsAsync(id, existingNews);

            return Ok(existingNews);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in UploadImage: {ex}");
            return StatusCode(500, new { message = "An error occurred while uploading the image", error = ex.Message });
        }
    }

    /// <summary>
    /// Delete the image from a news article (requires authentication)
    /// </summary>
    /// <param name="id">News article ID</param>
    /// <returns>No content</returns>
    [HttpDelete("{id}/image")]
    [Authorize]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult> DeleteImage(string id)
    {
        try
        {
            var existingNews = await _newsService.GetNewsByIdAsync(id);
            if (existingNews == null)
            {
                return NotFound(new { message = "News article not found" });
            }

            if (existingNews.ImageMetadata == null)
            {
                return NotFound(new { message = "News article has no image" });
            }

            // Delete image from MinIO
            await _imageStorageService.DeleteImageWithThumbnailAsync(existingNews.ImageMetadata);

            // Update news article
            existingNews.ImageUrl = string.Empty;
            existingNews.ThumbnailUrl = string.Empty;
            existingNews.ImageMetadata = null;

            await _newsService.UpdateNewsAsync(id, existingNews);

            return NoContent();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in DeleteImage: {ex}");
            return StatusCode(500, new { message = "An error occurred while deleting the image", error = ex.Message });
        }
    }

    private string GetImageUrl(string objectKey)
    {
        // In production, this should be a CDN URL
        return $"http://localhost:9000/news-images/{objectKey}";
    }

    private string GetThumbnailUrl(string newsId, string extension)
    {
        // In production, this should be a CDN URL
        return $"http://localhost:9000/news-images/{newsId}-thumb{extension}";
    }
}
