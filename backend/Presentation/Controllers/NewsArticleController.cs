using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsApi.Application.DTOs;
using NewsApi.Application.Services;
using NewsApi.Common;
using NewsApi.Domain.Entities;
using NewsApi.Domain.Interfaces;

namespace NewsApi.Presentation.Controllers;

/// <summary>
/// Controller for managing news articles with public read and authenticated write operations.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public sealed class NewsArticleController(INewsArticleService newsService, IImageStorageService imageStorageService)
    : ControllerBase
{
    /// <summary>
    /// Get all news articles (public endpoint)
    /// </summary>
    /// <param name="category">Filter by category</param>
    /// <param name="type">Filter by type</param>
    /// <returns>List of news articles</returns>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<NewsArticle>>> GetAllNews(
        [FromQuery] string? category = null,
        [FromQuery] string? type = null
    )
    {
        try
        {
            Console.WriteLine("GetAllNews called");
            var news = await newsService.GetAllNewsAsync();
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
    public async Task<ActionResult<NewsArticle>> GetNewsById(string id)
    {
        try
        {
            var news = await newsService.GetNewsByIdAsync(id);

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
    /// Get news article by slug (public endpoint)
    /// </summary>
    /// <param name="slug">News article slug</param>
    /// <returns>News article</returns>
    [HttpGet("by-slug")]
    [AllowAnonymous] // Public endpoint
    public async Task<ActionResult<NewsArticle>> GetNewsBySlug([FromQuery] string slug)
    {
        try
        {
            if (string.IsNullOrEmpty(slug))
            {
                return BadRequest(new { message = "Slug parameter is required" });
            }

            var news = await newsService.GetNewsBySlugAsync(slug);

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
    /// <param name="createNewsArticleDto">News article data</param>
    /// <returns>Created news article</returns>
    [HttpPost]
    [Authorize] // Protected endpoint
    public async Task<ActionResult<NewsArticle>> CreateNews([FromBody] CreateNewsArticleDto createNewsArticleDto)
    {
        try
        {
            // Map DTO to entity
            var news = new NewsArticle
            {
                Category = createNewsArticleDto.Category,
                Type = createNewsArticleDto.Type,
                Caption = createNewsArticleDto.Caption,
                Slug = SlugHelper.GenerateSlug(createNewsArticleDto.Caption),
                Keywords = createNewsArticleDto.Keywords,
                SocialTags = createNewsArticleDto.SocialTags,
                Summary = createNewsArticleDto.Summary,
                ImgPath = createNewsArticleDto.ImgPath,
                ImgAlt = createNewsArticleDto.ImgAlt,
                ImageUrl = createNewsArticleDto.ImageUrl,
                ThumbnailUrl = createNewsArticleDto.ThumbnailUrl,
                Content = createNewsArticleDto.Content,
                Subjects = createNewsArticleDto.Subjects,
                Authors = createNewsArticleDto.Authors,
                ExpressDate = createNewsArticleDto.ExpressDate,
                Priority = createNewsArticleDto.Priority,
                IsActive = createNewsArticleDto.IsActive,
                IsSecondPageNews = createNewsArticleDto.IsSecondPageNews,
            };

            var createdNews = await newsService.CreateNewsAsync(news);
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
    /// <param name="updateNewsArticleDto">Updated news article data</param>
    /// <returns>No content</returns>
    [HttpPut("{id}")]
    [Authorize] // Protected endpoint
    public async Task<ActionResult> UpdateNews(string id, [FromBody] UpdateNewsArticleDto updateNewsArticleDto)
    {
        try
        {
            var existingNews = await newsService.GetNewsByIdAsync(id);
            if (existingNews == null)
            {
                return NotFound(new { message = "News article not found" });
            }

            // Update only provided fields
            if (updateNewsArticleDto.Category != null)
                existingNews.Category = updateNewsArticleDto.Category;
            if (updateNewsArticleDto.Type != null)
                existingNews.Type = updateNewsArticleDto.Type;
            if (updateNewsArticleDto.Caption != null)
            {
                existingNews.Caption = updateNewsArticleDto.Caption;
                existingNews.Slug = SlugHelper.GenerateSlug(updateNewsArticleDto.Caption);
            }

            if (updateNewsArticleDto.Keywords != null)
                existingNews.Keywords = updateNewsArticleDto.Keywords;
            if (updateNewsArticleDto.SocialTags != null)
                existingNews.SocialTags = updateNewsArticleDto.SocialTags;
            if (updateNewsArticleDto.Summary != null)
                existingNews.Summary = updateNewsArticleDto.Summary;
            if (updateNewsArticleDto.ImgPath != null)
                existingNews.ImgPath = updateNewsArticleDto.ImgPath;
            if (updateNewsArticleDto.ImgAlt != null)
                existingNews.ImgAlt = updateNewsArticleDto.ImgAlt;
            if (updateNewsArticleDto.Content != null)
                existingNews.Content = updateNewsArticleDto.Content;
            if (updateNewsArticleDto.Subjects != null)
                existingNews.Subjects = updateNewsArticleDto.Subjects;
            if (updateNewsArticleDto.Authors != null)
                existingNews.Authors = updateNewsArticleDto.Authors;
            if (updateNewsArticleDto.ExpressDate.HasValue)
                existingNews.ExpressDate = updateNewsArticleDto.ExpressDate.Value;
            if (updateNewsArticleDto.Priority.HasValue)
                existingNews.Priority = updateNewsArticleDto.Priority.Value;
            if (updateNewsArticleDto.IsActive.HasValue)
                existingNews.IsActive = updateNewsArticleDto.IsActive.Value;
            if (updateNewsArticleDto.IsSecondPageNews.HasValue)
                existingNews.IsSecondPageNews = updateNewsArticleDto.IsSecondPageNews.Value;
            if (updateNewsArticleDto.ImageUrl != null)
                existingNews.ImageUrl = updateNewsArticleDto.ImageUrl;
            if (updateNewsArticleDto.ThumbnailUrl != null)
                existingNews.ThumbnailUrl = updateNewsArticleDto.ThumbnailUrl;

            await newsService.UpdateNewsAsync(id, existingNews);
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
            var existingNews = await newsService.GetNewsByIdAsync(id);
            if (existingNews == null)
            {
                return NotFound(new { message = "News article not found" });
            }

            await newsService.DeleteNewsAsync(id);
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
    [ProducesResponseType(typeof(NewsArticle), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<NewsArticle>> UploadImage(string id, [FromForm] ImageUploadDto imageUpload)
    {
        try
        {
            // Check if news article exists
            var existingNews = await newsService.GetNewsByIdAsync(id);
            if (existingNews == null)
            {
                return NotFound(new { message = "News article not found" });
            }

            // Delete old image if exists
            if (existingNews.ImageMetadata != null)
            {
                await imageStorageService.DeleteImageWithThumbnailAsync(existingNews.ImageMetadata);
            }

            // Upload new image
            var imageMetadata = await imageStorageService.UploadImageAsync(
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

            await newsService.UpdateNewsAsync(id, existingNews);

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
            var existingNews = await newsService.GetNewsByIdAsync(id);
            if (existingNews == null)
            {
                return NotFound(new { message = "News article not found" });
            }

            if (existingNews.ImageMetadata == null)
            {
                return NotFound(new { message = "News article has no image" });
            }

            // Delete image from MinIO
            await imageStorageService.DeleteImageWithThumbnailAsync(existingNews.ImageMetadata);

            // Update news article
            existingNews.ImageUrl = string.Empty;
            existingNews.ThumbnailUrl = string.Empty;
            existingNews.ImageMetadata = null;

            await newsService.UpdateNewsAsync(id, existingNews);

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
