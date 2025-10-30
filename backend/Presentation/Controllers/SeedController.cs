using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NewsApi.Application.Services;
using NewsApi.Infrastructure.Data;
using NewsApi.Infrastructure.Services;

namespace NewsApi.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SeedController : ControllerBase
{
    private readonly MongoDbContext _context;
    private readonly ILogger<SeedController> _logger;
    private readonly INewsDataFetcherService _newsDataFetcher;
    private readonly INewsService _newsService;

    public SeedController(
        MongoDbContext context,
        ILogger<SeedController> logger,
        INewsDataFetcherService newsDataFetcher,
        INewsService newsService)
    {
        _context = context;
        _logger = logger;
        _newsDataFetcher = newsDataFetcher;
        _newsService = newsService;
    }

    /// <summary>
    /// Seeds the database with sample news articles
    /// </summary>
    /// <returns>Result of seeding operation</returns>
    [HttpPost("news")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SeedNews()
    {
        try
        {
            _logger.LogInformation("Starting database seeding...");
            await SeedNewsData.SeedAsync(_context);
            _logger.LogInformation("Database seeding completed successfully!");

            return Ok(new { message = "Database seeded successfully with news articles!" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while seeding database");
            return StatusCode(500, new { message = "Error seeding database", error = ex.Message });
        }
    }

    /// <summary>
    /// Seeds the database with Reddit posts converted to Turkish news articles
    /// Focuses on technology subreddits and GitHub Copilot discussions
    /// </summary>
    /// <returns>Result of Reddit seeding operation</returns>
    [HttpPost("reddit")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SeedRedditNews()
    {
        try
        {
            _logger.LogInformation("Starting Reddit news seeding...");
            await SeedRedditNewsData.SeedAsync(_context);
            _logger.LogInformation("Reddit news seeding completed successfully!");

            return Ok(new { message = "Database seeded successfully with Reddit news articles!" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while seeding Reddit news");
            return StatusCode(500, new { message = "Error seeding Reddit news", error = ex.Message });
        }
    }

    /// <summary>
    /// Manually triggers the daily news fetching process from external NewsAPI.org
    /// This endpoint allows testing the automatic seed functionality without waiting for scheduled time
    /// </summary>
    /// <returns>Result of news fetching and seeding operation</returns>
    [HttpPost("fetch-external-news")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> FetchExternalNews()
    {
        try
        {
            _logger.LogInformation("Manually triggering external news fetch...");

            var startTime = DateTime.UtcNow;
            int totalFetched = 0;
            int totalCreated = 0;
            int totalSkipped = 0;
            int totalErrors = 0;

            // Fetch news from external API
            var newsArticles = await _newsDataFetcher.FetchLatestNewsAsync();
            totalFetched = newsArticles.Count;

            _logger.LogInformation("Fetched {Count} news articles from external sources", totalFetched);

            if (totalFetched == 0)
            {
                return Ok(new
                {
                    message = "No news articles were fetched. Check API configuration and limits.",
                    fetched = 0,
                    created = 0,
                    skipped = 0,
                    errors = 0
                });
            }

            // Save each article to database
            foreach (var article in newsArticles)
            {
                try
                {
                    // Check if article with similar caption already exists (avoid duplicates)
                    var existingArticles = await _newsService.GetAllNewsAsync(
                        category: article.Category,
                        type: null);

                    var isDuplicate = existingArticles.Any(existing =>
                        existing.Caption.Equals(article.Caption, StringComparison.OrdinalIgnoreCase));

                    if (isDuplicate)
                    {
                        totalSkipped++;
                        _logger.LogDebug("Skipped duplicate article: {Caption}", article.Caption);
                        continue;
                    }

                    // Create the news article
                    await _newsService.CreateNewsAsync(article);
                    totalCreated++;

                    _logger.LogDebug("Created article: {Caption} in {Category}",
                        article.Caption, article.Category);
                }
                catch (Exception ex)
                {
                    totalErrors++;
                    _logger.LogWarning(ex, "Failed to create article: {Caption}", article.Caption);
                }
            }

            var endTime = DateTime.UtcNow;
            var duration = (endTime - startTime).TotalSeconds;

            _logger.LogInformation(
                "External news fetch completed: {Created} created, {Skipped} skipped, {Errors} errors",
                totalCreated, totalSkipped, totalErrors);

            return Ok(new
            {
                message = "External news fetch completed successfully!",
                durationSeconds = Math.Round(duration, 2),
                fetched = totalFetched,
                created = totalCreated,
                skipped = totalSkipped,
                errors = totalErrors
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching external news");
            return StatusCode(500, new { message = "Error fetching external news", error = ex.Message });
        }
    }
}
