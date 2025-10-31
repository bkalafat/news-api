using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NewsApi.Application.DTOs;
using NewsApi.Application.Services;
using NewsApi.Infrastructure.Services;

namespace NewsApi.Presentation.Controllers;

/// <summary>
/// Manages social media posts from various platforms
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
internal sealed class SocialMediaController : ControllerBase
{
    private readonly ISocialMediaPostService _service;
    private readonly ILogger<SocialMediaController> _logger;

    private readonly RedditService _redditService;

    /// <summary>
    /// Initializes a new instance of the <see cref="SocialMediaController"/> class.
    /// Initializes a new instance of SocialMediaController
    /// </summary>
    public SocialMediaController(
        ISocialMediaPostService service,
        RedditService redditService,
        ILogger<SocialMediaController> logger)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _redditService = redditService ?? throw new ArgumentNullException(nameof(redditService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Retrieves all social media posts
    /// </summary>
    /// <returns>A collection of social media posts</returns>
    /// <response code="200">Returns the list of social media posts</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<SocialMediaPostDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<SocialMediaPostDto>>> GetAll()
    {
        _logger.LogInformation("GetAll social media posts called");
        var posts = await _service.GetAllPostsAsync();
        return Ok(posts);
    }

    /// <summary>
    /// Retrieves a social media post by its unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the post</param>
    /// <returns>The social media post if found</returns>
    /// <response code="200">Returns the requested post</response>
    /// <response code="404">If the post is not found</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(SocialMediaPostDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SocialMediaPostDto>> GetById(string id)
    {
        _logger.LogInformation("GetById called with id: {Id}", id);

        var post = await _service.GetByIdAsync(id);

        if (post == null)
        {
            _logger.LogWarning("Post not found: {Id}", id);
            return NotFound();
        }

        return Ok(post);
    }

    /// <summary>
    /// Retrieves posts from a specific platform
    /// </summary>
    /// <param name="platform">The platform name (Reddit, Twitter, LinkedIn, Facebook, Instagram, YouTube)</param>
    /// <returns>A collection of posts from the specified platform</returns>
    /// <response code="200">Returns the list of posts</response>
    [HttpGet("platform/{platform}")]
    [ProducesResponseType(typeof(IEnumerable<SocialMediaPostDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<SocialMediaPostDto>>> GetByPlatform(string platform)
    {
        _logger.LogInformation("GetByPlatform called with platform: {Platform}", platform);
        var posts = await _service.GetByPlatformAsync(platform);
        return Ok(posts);
    }

    /// <summary>
    /// Retrieves posts from a specific category or subreddit
    /// </summary>
    /// <param name="category">The category, subreddit, or hashtag</param>
    /// <returns>A collection of posts from the specified category</returns>
    /// <response code="200">Returns the list of posts</response>
    [HttpGet("category/{category}")]
    [ProducesResponseType(typeof(IEnumerable<SocialMediaPostDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<SocialMediaPostDto>>> GetByCategory(string category)
    {
        _logger.LogInformation("GetByCategory called with category: {Category}", category);
        var posts = await _service.GetByCategoryAsync(category);
        return Ok(posts);
    }

    /// <summary>
    /// Retrieves top posts by upvotes/likes
    /// </summary>
    /// <param name="limit">Maximum number of posts to return</param>
    /// <param name="platform">Optional platform filter</param>
    /// <returns>A collection of top posts</returns>
    /// <response code="200">Returns the list of top posts</response>
    [HttpGet("top")]
    [ProducesResponseType(typeof(IEnumerable<SocialMediaPostDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<SocialMediaPostDto>>> GetTopPosts(
        [FromQuery] int limit = 10,
        [FromQuery] string? platform = null)
    {
        _logger.LogInformation("GetTopPosts called with limit: {Limit}, platform: {Platform}", limit, platform);
        var posts = await _service.GetTopPostsAsync(limit, platform);
        return Ok(posts);
    }

    /// <summary>
    /// Creates a new social media post
    /// </summary>
    /// <param name="dto">The post data</param>
    /// <returns>The created post</returns>
    /// <response code="201">Returns the newly created post</response>
    /// <response code="400">If the request data is invalid</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <remarks>
    /// Sample request:
    ///     POST /api/socialmedia
    ///     {
    ///        "platform": "Reddit",
    ///        "externalId": "abc123",
    ///        "title": "GitHub Copilot Tips",
    ///        "content": "Check out these amazing tips...",
    ///        "author": "user123",
    ///        "authorUrl": "https://reddit.com/u/user123",
    ///        "postUrl": "https://reddit.com/r/github/comments/abc123",
    ///        "imageUrls": ["https://example.com/image.jpg"],
    ///        "upvotes": 150,
    ///        "downvotes": 5,
    ///        "commentCount": 25,
    ///        "category": "github",
    ///        "postedAt": "2025-10-01T12:00:00Z",
    ///        "priority": 50
    ///     }
    /// </remarks>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(SocialMediaPostDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<SocialMediaPostDto>> Create([FromBody] CreateSocialMediaPostDto dto)
    {
        _logger.LogInformation("Create called for platform: {Platform}, category: {Category}", dto.Platform, dto.Category);

        var post = await _service.CreatePostAsync(dto);

        _logger.LogInformation("Post created successfully: {Id}", post.Id);

        return CreatedAtAction(nameof(GetById), new { id = post.Id }, post);
    }

    /// <summary>
    /// Updates an existing social media post
    /// </summary>
    /// <param name="id">The unique identifier of the post</param>
    /// <param name="dto">The updated post data</param>
    /// <returns>No content</returns>
    /// <response code="204">If the update was successful</response>
    /// <response code="400">If the request data is invalid</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="404">If the post is not found</response>
    [HttpPut("{id}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateSocialMediaPostDto dto)
    {
        _logger.LogInformation("Update called for id: {Id}", id);

        await _service.UpdatePostAsync(id, dto);

        _logger.LogInformation("Post updated successfully: {Id}", id);

        return NoContent();
    }

    /// <summary>
    /// Deletes a social media post
    /// </summary>
    /// <param name="id">The unique identifier of the post</param>
    /// <returns>No content</returns>
    /// <response code="204">If the deletion was successful</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="404">If the post is not found</response>
    [HttpDelete("{id}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string id)
    {
        _logger.LogInformation("Delete called for id: {Id}", id);

        await _service.DeletePostAsync(id);

        _logger.LogInformation("Post deleted successfully: {Id}", id);

        return NoContent();
    }

    /// <summary>
    /// Updates post metrics (likes, comments, shares)
    /// </summary>
    /// <param name="id">The unique identifier of the post</param>
    /// <param name="upvotes">Updated upvote count</param>
    /// <param name="downvotes">Updated downvote count</param>
    /// <param name="commentCount">Updated comment count</param>
    /// <param name="shareCount">Updated share count</param>
    /// <returns>No content</returns>
    /// <response code="204">If the update was successful</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="404">If the post is not found</response>
    [HttpPatch("{id}/metrics")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateMetrics(
        string id,
        [FromQuery] int upvotes,
        [FromQuery] int downvotes,
        [FromQuery] int commentCount,
        [FromQuery] int shareCount)
    {
        _logger.LogInformation("UpdateMetrics called for id: {Id}", id);

        await _service.UpdateMetricsAsync(id, upvotes, downvotes, commentCount, shareCount);

        _logger.LogInformation("Metrics updated successfully: {Id}", id);

        return NoContent();
    }

    /// <summary>
    /// Fetches and imports posts from Reddit with flexible search
    /// </summary>
    /// <param name="subreddit">Subreddit name (without r/)</param>
    /// <param name="query">Search query (optional, fetches top posts if empty)</param>
    /// <param name="timeframe">Time period (hour, day, week, month, year)</param>
    /// <param name="limit">Maximum posts to fetch (max 100)</param>
    /// <returns>Number of posts imported</returns>
    /// <response code="200">Returns the count of imported posts</response>
    /// <response code="401">If the user is not authenticated</response>
    [HttpPost("import/reddit")]
    [Authorize]
    [ProducesResponseType(typeof(ImportResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ImportResultDto>> ImportRedditPosts(
        [FromQuery] string subreddit,
        [FromQuery] string? query = null,
        [FromQuery] string timeframe = "week",
        [FromQuery] int limit = 25)
    {
        _logger.LogInformation(
            "Importing Reddit posts from r/{Subreddit}, query: {Query}, timeframe: {Timeframe}, limit: {Limit}",
            subreddit, query ?? "(top posts)", timeframe, limit);

        try
        {
            List<CreateSocialMediaPostDto> redditPosts;

            if (string.IsNullOrEmpty(query))
            {
                // Fetch top posts
                redditPosts = await _redditService.GetTopPostsAsync(subreddit, timeframe, limit);
            }
            else
            {
                // Search with query
                redditPosts = await _redditService.SearchPostsAsync(subreddit, query, "top", timeframe, limit);
            }

            int imported = 0;
            int skipped = 0;
            var errors = new List<string>();

            foreach (var redditPost in redditPosts)
            {
                try
                {
                    await _service.CreatePostAsync(redditPost);
                    imported++;
                    _logger.LogInformation("Imported post: {Title}", redditPost.Title);
                }
                catch (InvalidOperationException ex) when (ex.Message.Contains("already exists", StringComparison.OrdinalIgnoreCase))
                {
                    skipped++;
                    _logger.LogDebug("Skipped duplicate post: {ExternalId}", redditPost.ExternalId);
                }
                catch (Exception ex)
                {
                    errors.Add($"Failed to import post {redditPost.ExternalId}: {ex.Message}");
                    _logger.LogError(ex, "Error importing post: {ExternalId}", redditPost.ExternalId);
                }
            }

            var result = new ImportResultDto
            {
                TotalFetched = redditPosts.Count,
                Imported = imported,
                Skipped = skipped,
                Errors = errors.ToArray(),
            };

            _logger.LogInformation(
                "Import completed: {Imported} imported, {Skipped} skipped, {Errors} errors",
                imported, skipped, errors.Count);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during Reddit import");
            return StatusCode(500, new { error = "Failed to import posts from Reddit", details = ex.Message });
        }
    }

    /// <summary>
    /// Fetches and imports top posts from Reddit r/github about "copilot"
    /// </summary>
    /// <param name="timeframe">Time period (hour, day, week, month, year)</param>
    /// <param name="limit">Maximum posts to fetch (max 100)</param>
    /// <returns>Number of posts imported</returns>
    /// <response code="200">Returns the count of imported posts</response>
    /// <response code="401">If the user is not authenticated</response>
    [HttpPost("import/reddit/github-copilot")]
    [Authorize]
    [ProducesResponseType(typeof(ImportResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public Task<ActionResult<ImportResultDto>> ImportGitHubCopilotPosts(
        [FromQuery] string timeframe = "month",
        [FromQuery] int limit = 25)
    {
        return ImportRedditPosts("github", "copilot", timeframe, limit);
    }
}

/// <summary>
/// Result of a social media import operation
/// </summary>
internal record ImportResultDto
{
    public int TotalFetched { get; init; }

    public int Imported { get; init; }

    public int Skipped { get; init; }

    public string[] Errors { get; init; } = [];
}
