using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NewsApi.Infrastructure.Data;
using System;
using System.Threading.Tasks;

namespace NewsApi.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SeedController : ControllerBase
{
    private readonly MongoDbContext _context;
    private readonly ILogger<SeedController> _logger;

    public SeedController(MongoDbContext context, ILogger<SeedController> logger)
    {
        _context = context;
        _logger = logger;
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
}
