---
applyTo: '**/*.cs'
---

# C# / .NET Coding Instructions

These instructions apply to all C# files in the News API project.

## 🧠 Language Context

- **Language**: C# 12+
- **Framework**: .NET 10.0
- **Runtime**: ASP.NET Core
- **Architecture**: Clean Architecture

## 🔧 C# Specific Guidelines

### Naming Conventions
- **PascalCase** for: Classes, interfaces, methods, properties, constants, enums
- **camelCase** for: Local variables, parameters, method arguments
- **_camelCase** for: Private fields (with underscore prefix)
- **IPascalCase** for: Interfaces (prefix with 'I')
- **TPascalCase** for: Generic type parameters (prefix with 'T')

### Language Features
- **Use async/await** for all I/O operations (database, HTTP, file)
- **Use nullable reference types** - `#nullable enable` is project-wide
- **Use pattern matching** - C# 12 switch expressions and patterns
- **Use records** for immutable DTOs when appropriate
- **Use `var`** when type is obvious from right side
- **Use expression-bodied members** for single-line methods/properties
- **Use collection expressions** (`[1, 2, 3]`) in C# 12+
- **Use primary constructors** for simple dependency injection
- **Use target-typed new** (`List<string> items = new();`)

### Code Organization
- **One class per file** (except nested classes)
- **File name matches class name** (NewsController.cs contains NewsController)
- **Using statements** at top of file
- **Namespace matches folder structure**
- **Order members**: Constants → Fields → Constructors → Properties → Methods

### Member Order (within class)
1. Private constants
2. Private fields
3. Constructor(s)
4. Public properties
5. Public methods
6. Private helper methods

### Method Guidelines
- **Keep methods small** - Single responsibility, ideally < 20 lines
- **Use descriptive names** - Verb-noun pattern (GetNewsById, CreateNews)
- **Parameters** - Max 3-4 parameters, use objects for more
- **Return types** - Use `Task<T>` for async, `ActionResult<T>` for controllers
- **Avoid ref/out** - Return tuples or custom types instead

### Exception Handling
```csharp
// ✅ Good - Specific exception types
try
{
    var news = await _repository.GetByIdAsync(id);
    return Ok(news);
}
catch (NotFoundException ex)
{
    _logger.LogWarning(ex, "News not found: {Id}", id);
    return NotFound();
}
catch (DatabaseException ex)
{
    _logger.LogError(ex, "Database error retrieving news: {Id}", id);
    return StatusCode(500, "An error occurred");
}

// ❌ Bad - Catching generic Exception
catch (Exception ex) 
{
    // Too broad
}
```

### Async/Await Best Practices
```csharp
// ✅ Good - Async all the way
public async Task<ActionResult<News>> GetByIdAsync(string id)
{
    var news = await _service.GetByIdAsync(id);
    return Ok(news);
}

// ❌ Bad - Blocking on async
public ActionResult<News> GetById(string id)
{
    var news = _service.GetByIdAsync(id).Result; // Don't do this
    return Ok(news);
}

// ✅ Good - ConfigureAwait for library code
public async Task<News> GetByIdAsync(string id)
{
    return await _repository.GetByIdAsync(id).ConfigureAwait(false);
}
```

### Nullable Reference Types
```csharp
// ✅ Good - Explicit nullability
public string? FindUserName(int id) // Can return null
{
    return _users.FirstOrDefault(u => u.Id == id)?.Name;
}

public string GetUserName(int id) // Never null
{
    return _users.First(u => u.Id == id).Name ?? "Unknown";
}

// ✅ Good - Null-conditional and null-coalescing
var name = user?.Profile?.Name ?? "Guest";

// ✅ Good - Null checking
if (user is null)
{
    throw new ArgumentNullException(nameof(user));
}
```

### LINQ Best Practices
```csharp
// ✅ Good - Method syntax for complex queries
var activeNews = news
    .Where(n => n.IsActive)
    .OrderByDescending(n => n.CreateDate)
    .Take(10)
    .ToList();

// ✅ Good - Query syntax for multiple sources
var result = from n in news
             join c in categories on n.CategoryId equals c.Id
             where n.IsActive
             select new { n.Caption, c.Name };

// ❌ Bad - Multiple enumerations
var count = news.Where(n => n.IsActive).Count();
var items = news.Where(n => n.IsActive).ToList(); // Queries twice

// ✅ Good - Single enumeration
var items = news.Where(n => n.IsActive).ToList();
var count = items.Count;
```

### String Handling
```csharp
// ✅ Good - String interpolation
var message = $"News {id} created by {author} at {DateTime.Now}";

// ✅ Good - StringBuilder for loops
var sb = new StringBuilder();
foreach (var item in items)
{
    sb.AppendLine(item);
}

// ✅ Good - Verbatim strings for paths/SQL
var path = @"C:\data\news\images\";

// ✅ Good - Raw string literals (C# 11+)
var json = """
    {
        "name": "Article",
        "category": "Tech"
    }
    """;
```

### Dependency Injection
```csharp
// ✅ Good - Constructor injection
public class NewsService : INewsService
{
    private readonly INewsRepository _repository;
    private readonly ILogger<NewsService> _logger;
    private readonly IMemoryCache _cache;

    public NewsService(
        INewsRepository repository,
        ILogger<NewsService> logger,
        IMemoryCache cache)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }
}

// ❌ Bad - Property injection (avoid in this project)
public INewsRepository Repository { get; set; }
```

### XML Documentation
```csharp
/// <summary>
/// Retrieves a news article by its unique identifier.
/// </summary>
/// <param name="id">The unique identifier of the news article.</param>
/// <returns>The news article if found; otherwise, null.</returns>
/// <exception cref="ArgumentException">Thrown when id is null or empty.</exception>
public async Task<News?> GetByIdAsync(string id)
{
    // Implementation
}
```

## 🧶 Patterns to Follow

### Repository Pattern
```csharp
// Interface in Domain layer
public interface INewsRepository
{
    Task<News?> GetByIdAsync(string id);
    Task<IEnumerable<News>> GetAllAsync();
    Task<News> CreateAsync(News news);
    Task UpdateAsync(string id, News news);
    Task DeleteAsync(string id);
}

// Implementation in Infrastructure layer
public class NewsRepository : INewsRepository
{
    private readonly MongoDbContext _context;
    
    public NewsRepository(MongoDbContext context)
    {
        _context = context;
    }
    
    public async Task<News?> GetByIdAsync(string id)
    {
        var filter = Builders<NewsDbModel>.Filter.Eq(n => n.Id, id);
        var dbModel = await _context.News.Find(filter).FirstOrDefaultAsync();
        return dbModel != null ? NewsMapper.ToDomain(dbModel) : null;
    }
}
```

### Service Pattern
```csharp
public class NewsService : INewsService
{
    private readonly INewsRepository _repository;
    private readonly IMemoryCache _cache;
    private readonly ILogger<NewsService> _logger;

    public NewsService(
        INewsRepository repository,
        IMemoryCache cache,
        ILogger<NewsService> logger)
    {
        _repository = repository;
        _cache = cache;
        _logger = logger;
    }

    public async Task<IEnumerable<News>> GetAllNewsAsync(string? category = null)
    {
        var cacheKey = category != null 
            ? $"{CacheKeys.AllNews}:{category}" 
            : CacheKeys.AllNews;

        if (_cache.TryGetValue(cacheKey, out IEnumerable<News>? cachedNews))
        {
            _logger.LogInformation("Retrieved news from cache: {CacheKey}", cacheKey);
            return cachedNews!;
        }

        var news = await _repository.GetAllAsync();
        
        if (category != null)
        {
            news = news.Where(n => n.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
        }

        _cache.Set(cacheKey, news, TimeSpan.FromMinutes(30));
        return news;
    }
}
```

### Controller Pattern
```csharp
[ApiController]
[Route("api/[controller]")]
public class NewsController : ControllerBase
{
    private readonly INewsService _newsService;
    private readonly ILogger<NewsController> _logger;

    public NewsController(INewsService newsService, ILogger<NewsController> logger)
    {
        _newsService = newsService;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves all news articles.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<News>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<News>>> GetAll(
        [FromQuery] string? category = null)
    {
        var news = await _newsService.GetAllNewsAsync(category);
        return Ok(news);
    }

    /// <summary>
    /// Creates a new news article.
    /// </summary>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(News), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<News>> Create([FromBody] CreateNewsDto dto)
    {
        var news = await _newsService.CreateNewsAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = news.Id }, news);
    }
}
```

### Validation Pattern (FluentValidation)
```csharp
public class CreateNewsDtoValidator : AbstractValidator<CreateNewsDto>
{
    public CreateNewsDtoValidator()
    {
        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("Category is required")
            .MaximumLength(100).WithMessage("Category must not exceed 100 characters");

        RuleFor(x => x.Caption)
            .NotEmpty().WithMessage("Caption is required")
            .MaximumLength(500).WithMessage("Caption must not exceed 500 characters");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Content is required");

        RuleFor(x => x.ImgPath)
            .Must(BeAValidUrl).When(x => !string.IsNullOrEmpty(x.ImgPath))
            .WithMessage("ImgPath must be a valid HTTP or HTTPS URL");

        RuleFor(x => x.Priority)
            .InclusiveBetween(1, 100).When(x => x.Priority.HasValue)
            .WithMessage("Priority must be between 1 and 100");
    }

    private bool BeAValidUrl(string? url)
    {
        if (string.IsNullOrEmpty(url)) return true;
        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }
}
```

## 🚫 Patterns to Avoid

```csharp
// ❌ Bad - Magic strings
if (category == "Technology") { }

// ✅ Good - Constants or enums
if (category == NewsCategories.Technology) { }

// ❌ Bad - Nested conditionals
if (news != null)
{
    if (news.IsActive)
    {
        if (news.Category == "Tech")
        {
            // ...
        }
    }
}

// ✅ Good - Early returns
if (news == null) return null;
if (!news.IsActive) return null;
if (news.Category != "Tech") return null;
// ...

// ❌ Bad - Async void (except event handlers)
public async void SaveNews() { }

// ✅ Good - Async Task
public async Task SaveNewsAsync() { }

// ❌ Bad - Exposing domain entities
public ActionResult<News> Get() // Domain entity
{
    return _repository.GetById(id);
}

// ✅ Good - Use DTOs
public ActionResult<NewsDto> Get()
{
    var news = _repository.GetById(id);
    return _mapper.Map<NewsDto>(news);
}
```

## 🧪 Testing Guidelines

```csharp
// Test naming: MethodName_Scenario_ExpectedBehavior
[Fact]
public async Task GetByIdAsync_ValidId_ReturnsNews()
{
    // Arrange
    var expectedNews = new News { Id = "123", Caption = "Test" };
    _mockRepository.Setup(r => r.GetByIdAsync("123"))
        .ReturnsAsync(expectedNews);

    // Act
    var result = await _service.GetByIdAsync("123");

    // Assert
    Assert.NotNull(result);
    Assert.Equal("Test", result.Caption);
}

[Fact]
public async Task GetByIdAsync_InvalidId_ReturnsNull()
{
    // Arrange
    _mockRepository.Setup(r => r.GetByIdAsync("999"))
        .ReturnsAsync((News?)null);

    // Act
    var result = await _service.GetByIdAsync("999");

    // Assert
    Assert.Null(result);
}
```

## 📚 References

- [Microsoft C# Coding Conventions](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- [C# Language Reference](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/)
- [Async/Await Best Practices](https://learn.microsoft.com/en-us/archive/msdn-magazine/2013/march/async-await-best-practices-in-asynchronous-programming)
