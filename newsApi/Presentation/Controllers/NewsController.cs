using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsApi.Application.DTOs;
using NewsApi.Application.Services;
using NewsApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NewsApi.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class NewsController : ControllerBase
{
    private readonly INewsService _newsService;

    public NewsController(INewsService newsService)
    {
        _newsService = newsService;
    }

    /// <summary>
    /// Get all news articles (public endpoint)
    /// </summary>
    /// <param name="category">Filter by category</param>
    /// <param name="type">Filter by type</param>
    /// <returns>List of news articles</returns>
    [HttpGet]
    [AllowAnonymous] // Public endpoint
    public async Task<ActionResult<List<News>>> GetAllNews([FromQuery] string? category = null, [FromQuery] string? type = null)
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
            return StatusCode(500, new { message = "An error occurred while retrieving news", error = ex.Message, stackTrace = ex.StackTrace });
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
            return StatusCode(500, new { message = "An error occurred while retrieving the news article", error = ex.Message });
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
            return StatusCode(500, new { message = "An error occurred while retrieving the news article", error = ex.Message });
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
                IsSecondPageNews = createNewsDto.IsSecondPageNews
            };

            var createdNews = await _newsService.CreateNewsAsync(news);
            return CreatedAtAction(nameof(GetNewsById), new { id = createdNews.Id }, createdNews);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in CreateNews: {ex}");
            return StatusCode(500, new { message = "An error occurred while creating the news article", error = ex.Message });
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
            if (updateNewsDto.Category != null) existingNews.Category = updateNewsDto.Category;
            if (updateNewsDto.Type != null) existingNews.Type = updateNewsDto.Type;
            if (updateNewsDto.Caption != null) existingNews.Caption = updateNewsDto.Caption;
            if (updateNewsDto.Keywords != null) existingNews.Keywords = updateNewsDto.Keywords;
            if (updateNewsDto.SocialTags != null) existingNews.SocialTags = updateNewsDto.SocialTags;
            if (updateNewsDto.Summary != null) existingNews.Summary = updateNewsDto.Summary;
            if (updateNewsDto.ImgPath != null) existingNews.ImgPath = updateNewsDto.ImgPath;
            if (updateNewsDto.ImgAlt != null) existingNews.ImgAlt = updateNewsDto.ImgAlt;
            if (updateNewsDto.Content != null) existingNews.Content = updateNewsDto.Content;
            if (updateNewsDto.Subjects != null) existingNews.Subjects = updateNewsDto.Subjects;
            if (updateNewsDto.Authors != null) existingNews.Authors = updateNewsDto.Authors;
            if (updateNewsDto.ExpressDate.HasValue) existingNews.ExpressDate = updateNewsDto.ExpressDate.Value;
            if (updateNewsDto.Priority.HasValue) existingNews.Priority = updateNewsDto.Priority.Value;
            if (updateNewsDto.IsActive.HasValue) existingNews.IsActive = updateNewsDto.IsActive.Value;
            if (updateNewsDto.Url != null) existingNews.Url = updateNewsDto.Url;
            if (updateNewsDto.IsSecondPageNews.HasValue) existingNews.IsSecondPageNews = updateNewsDto.IsSecondPageNews.Value;

            await _newsService.UpdateNewsAsync(id, existingNews);
            return NoContent();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in UpdateNews: {ex}");
            return StatusCode(500, new { message = "An error occurred while updating the news article", error = ex.Message });
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
            return StatusCode(500, new { message = "An error occurred while deleting the news article", error = ex.Message });
        }
    }
}