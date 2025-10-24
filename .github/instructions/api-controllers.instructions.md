---
applyTo: '**/Controllers/**/*.cs'
---

# API Controller Instructions

These instructions apply to all controller files in the Presentation layer.

## 🎯 Controller Purpose

Controllers in Clean Architecture should be thin orchestrators that:
- Route HTTP requests to appropriate services
- Handle HTTP-specific concerns (status codes, headers)
- Transform service results to HTTP responses
- **NOT** contain business logic

## 🔧 Controller Structure

### Standard Controller Template

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsApi.Application.Services;
using NewsApi.Application.DTOs;

namespace NewsApi.Presentation.Controllers
{
    /// <summary>
    /// Manages news article operations
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class NewsController : ControllerBase
    {
        private readonly INewsService _newsService;
        private readonly ILogger<NewsController> _logger;

        /// <summary>
        /// Initializes a new instance of NewsController
        /// </summary>
        public NewsController(INewsService newsService, ILogger<NewsController> logger)
        {
            _newsService = newsService ?? throw new ArgumentNullException(nameof(newsService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // Action methods here
    }
}
```

## 📝 Action Method Patterns

### GET - Retrieve Single Resource

```csharp
/// <summary>
/// Retrieves a news article by its unique identifier
/// </summary>
/// <param name="id">The unique identifier of the news article</param>
/// <returns>The news article if found</returns>
/// <response code="200">Returns the requested news article</response>
/// <response code="404">If the news article is not found</response>
[HttpGet("{id}")]
[ProducesResponseType(typeof(News), StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public async Task<ActionResult<News>> GetById(string id)
{
    _logger.LogInformation("GetById called with id: {Id}", id);

    var news = await _newsService.GetByIdAsync(id);
    
    if (news == null)
    {
        _logger.LogWarning("News not found: {Id}", id);
        return NotFound();
    }

    return Ok(news);
}
```

### GET - Retrieve Collection

```csharp
/// <summary>
/// Retrieves all news articles with optional filtering
/// </summary>
/// <param name="category">Optional category filter</param>
/// <param name="type">Optional type filter</param>
/// <returns>A collection of news articles</returns>
/// <response code="200">Returns the list of news articles</response>
[HttpGet]
[ProducesResponseType(typeof(IEnumerable<News>), StatusCodes.Status200OK)]
public async Task<ActionResult<IEnumerable<News>>> GetAll(
    [FromQuery] string? category = null,
    [FromQuery] string? type = null)
{
    _logger.LogInformation("GetAll called with category: {Category}, type: {Type}", 
        category, type);

    var news = await _newsService.GetAllNewsAsync(category, type);
    return Ok(news);
}
```

### POST - Create Resource

```csharp
/// <summary>
/// Creates a new news article
/// </summary>
/// <param name="dto">The news article data</param>
/// <returns>The created news article</returns>
/// <response code="201">Returns the newly created news article</response>
/// <response code="400">If the request data is invalid</response>
/// <response code="401">If the user is not authenticated</response>
[HttpPost]
[Authorize]
[ProducesResponseType(typeof(News), StatusCodes.Status201Created)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public async Task<ActionResult<News>> Create([FromBody] CreateNewsDto dto)
{
    _logger.LogInformation("Create called for category: {Category}", dto.Category);

    var news = await _newsService.CreateNewsAsync(dto);

    _logger.LogInformation("News created successfully: {Id}", news.Id);
    
    return CreatedAtAction(
        nameof(GetById), 
        new { id = news.Id }, 
        news);
}
```

### PUT - Update Resource

```csharp
/// <summary>
/// Updates an existing news article
/// </summary>
/// <param name="id">The unique identifier of the news article</param>
/// <param name="dto">The updated news article data</param>
/// <returns>No content</returns>
/// <response code="204">If the update was successful</response>
/// <response code="400">If the request data is invalid</response>
/// <response code="401">If the user is not authenticated</response>
/// <response code="404">If the news article is not found</response>
[HttpPut("{id}")]
[Authorize]
[ProducesResponseType(StatusCodes.Status204NoContent)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public async Task<IActionResult> Update(string id, [FromBody] UpdateNewsDto dto)
{
    _logger.LogInformation("Update called for id: {Id}", id);

    await _newsService.UpdateNewsAsync(id, dto);

    _logger.LogInformation("News updated successfully: {Id}", id);
    
    return NoContent();
}
```

### DELETE - Remove Resource

```csharp
/// <summary>
/// Deletes a news article
/// </summary>
/// <param name="id">The unique identifier of the news article</param>
/// <returns>No content</returns>
/// <response code="204">If the deletion was successful</response>
/// <response code="401">If the user is not authenticated</response>
/// <response code="404">If the news article is not found</response>
[HttpDelete("{id}")]
[Authorize]
[ProducesResponseType(StatusCodes.Status204NoContent)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public async Task<IActionResult> Delete(string id)
{
    _logger.LogInformation("Delete called for id: {Id}", id);

    await _newsService.DeleteNewsAsync(id);

    _logger.LogInformation("News deleted successfully: {Id}", id);
    
    return NoContent();
}
```

## 🎨 HTTP Status Codes

Use appropriate status codes:

### Success Codes (2xx)
- **200 OK** - GET, PUT successful with response body
- **201 Created** - POST successful, resource created
- **204 No Content** - PUT, DELETE successful, no response body

### Client Error Codes (4xx)
- **400 Bad Request** - Invalid input, validation failure
- **401 Unauthorized** - Authentication required
- **403 Forbidden** - Authenticated but not authorized
- **404 Not Found** - Resource doesn't exist
- **409 Conflict** - Resource conflict (duplicate, etc.)

### Server Error Codes (5xx)
- **500 Internal Server Error** - Unexpected server error
- **503 Service Unavailable** - Service temporarily unavailable

## 🔒 Authorization Patterns

### Require Authentication
```csharp
[HttpPost]
[Authorize] // JWT required
public async Task<ActionResult<News>> Create([FromBody] CreateNewsDto dto)
{
    // Method implementation
}
```

### Require Specific Role
```csharp
[HttpDelete("{id}")]
[Authorize(Roles = "Admin")]
public async Task<IActionResult> Delete(string id)
{
    // Method implementation
}
```

### Optional Authentication
```csharp
[HttpGet]
[AllowAnonymous] // Explicitly allow anonymous access
public async Task<ActionResult<IEnumerable<News>>> GetAll()
{
    // Method implementation
}
```

## 📊 Parameter Binding

### From Route
```csharp
[HttpGet("{id}")]
public async Task<ActionResult<News>> GetById([FromRoute] string id)
{
    // id comes from URL path
}
```

### From Query String
```csharp
[HttpGet]
public async Task<ActionResult<IEnumerable<News>>> GetAll(
    [FromQuery] string? category = null,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10)
{
    // category, page, pageSize from query string
    // ?category=Tech&page=1&pageSize=10
}
```

### From Request Body
```csharp
[HttpPost]
public async Task<ActionResult<News>> Create([FromBody] CreateNewsDto dto)
{
    // dto comes from request body JSON
}
```

### From Header
```csharp
[HttpGet]
public async Task<ActionResult<IEnumerable<News>>> GetAll(
    [FromHeader(Name = "X-Api-Key")] string? apiKey = null)
{
    // apiKey from header
}
```

## ✅ Best Practices

### DO:
- ✅ Keep controllers thin
- ✅ Use `async`/`await` for all I/O
- ✅ Return `ActionResult<T>` for type safety
- ✅ Add `[ApiController]` attribute
- ✅ Use `[ProducesResponseType]` for all status codes
- ✅ Add XML documentation comments
- ✅ Log entry and exit points
- ✅ Validate input (handled by validators)
- ✅ Use proper HTTP status codes
- ✅ Return DTOs, not domain entities

### DON'T:
- ❌ Put business logic in controllers
- ❌ Access repositories directly
- ❌ Catch exceptions here (use middleware)
- ❌ Return domain entities
- ❌ Use `Task` instead of `Task<IActionResult>`
- ❌ Use `async void`
- ❌ Hardcode values
- ❌ Perform data transformations
- ❌ Make external API calls directly

## 🔍 Error Handling

Let middleware handle exceptions:

```csharp
// ❌ Bad - Controller handles exceptions
[HttpGet("{id}")]
public async Task<ActionResult<News>> GetById(string id)
{
    try
    {
        var news = await _newsService.GetByIdAsync(id);
        return Ok(news);
    }
    catch (NotFoundException)
    {
        return NotFound();
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error");
        return StatusCode(500);
    }
}

// ✅ Good - Let middleware handle
[HttpGet("{id}")]
public async Task<ActionResult<News>> GetById(string id)
{
    var news = await _newsService.GetByIdAsync(id);
    
    if (news == null)
    {
        return NotFound();
    }
    
    return Ok(news);
}
```

## 📝 Documentation Requirements

Every action must have:

```csharp
/// <summary>
/// Brief description of what the endpoint does
/// </summary>
/// <param name="paramName">Description of parameter</param>
/// <returns>Description of return value</returns>
/// <response code="200">Success scenario description</response>
/// <response code="400">Bad request scenario description</response>
/// <response code="404">Not found scenario description</response>
/// <remarks>
/// Sample request:
///     POST /api/news
///     {
///        "category": "Technology",
///        "caption": "Breaking News",
///        "content": "News content here"
///     }
/// </remarks>
[HttpPost]
[ProducesResponseType(typeof(News), StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public async Task<ActionResult<News>> Create([FromBody] CreateNewsDto dto)
{
    // Implementation
}
```

## 🧪 Testing Controllers

Controllers should be tested with integration tests:

```csharp
public class NewsControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public NewsControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetById_ValidId_ReturnsOk()
    {
        // Arrange
        var id = "valid-id";

        // Act
        var response = await _client.GetAsync($"/api/news/{id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
```

## 🎯 Route Patterns

### RESTful Routes
```csharp
GET    /api/news              // Get all
GET    /api/news/{id}         // Get by ID
POST   /api/news              // Create
PUT    /api/news/{id}         // Update
DELETE /api/news/{id}         // Delete
```

### Additional Routes
```csharp
GET    /api/news/by-url?url={slug}     // Get by URL slug
GET    /api/news/search?q={query}      // Search
GET    /api/news/category/{category}   // Get by category
```

## 📚 References

- [ASP.NET Core Web API](https://learn.microsoft.com/en-us/aspnet/core/web-api/)
- [Controller Action Return Types](https://learn.microsoft.com/en-us/aspnet/core/web-api/action-return-types)
- [HTTP Status Codes](https://developer.mozilla.org/en-US/docs/Web/HTTP/Status)
- [RESTful API Design](https://restfulapi.net/)
