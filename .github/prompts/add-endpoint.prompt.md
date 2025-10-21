---
description: 'Add a new API endpoint with full implementation'
---

# Add New API Endpoint

This prompt guides you through adding a complete new API endpoint to the News API project.

## 📋 Checklist

When implementing a new endpoint, ensure all these steps are completed:

- [ ] Define DTO in `Application/DTOs/`
- [ ] Create validator in `Application/Validators/`
- [ ] Implement service method in `Application/Services/`
- [ ] Add controller action in `Presentation/Controllers/`
- [ ] Add XML documentation comments
- [ ] Write unit tests
- [ ] Write integration tests
- [ ] Update Swagger documentation
- [ ] Test manually via Swagger UI

## 🎯 Implementation Steps

### 1. Define the DTO

**Location**: `newsApi/Application/DTOs/`

```csharp
/// <summary>
/// Data transfer object for [describe purpose]
/// </summary>
public class YourDto
{
    /// <summary>
    /// [Field description]
    /// </summary>
    public string PropertyName { get; set; } = string.Empty;
    
    // Add all required properties
}
```

### 2. Create Validator

**Location**: `newsApi/Application/Validators/`

```csharp
using FluentValidation;

public class YourDtoValidator : AbstractValidator<YourDto>
{
    public YourDtoValidator()
    {
        RuleFor(x => x.PropertyName)
            .NotEmpty().WithMessage("PropertyName is required")
            .MaximumLength(100).WithMessage("PropertyName must not exceed 100 characters");
        
        // Add all validation rules
    }
}
```

### 3. Add Service Interface Method

**Location**: `newsApi/Application/Services/INewsService.cs`

```csharp
/// <summary>
/// [Method description]
/// </summary>
/// <param name="dto">The input data</param>
/// <returns>[Return description]</returns>
Task<Result> YourMethodAsync(YourDto dto);
```

### 4. Implement Service Method

**Location**: `newsApi/Application/Services/NewsService.cs`

```csharp
public async Task<Result> YourMethodAsync(YourDto dto)
{
    try
    {
        _logger.LogInformation("Starting YourMethod with {Param}", dto.PropertyName);
        
        // 1. Business logic validation
        // 2. Call repository
        // 3. Handle caching if needed
        // 4. Return result
        
        _logger.LogInformation("Successfully completed YourMethod");
        return result;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error in YourMethod");
        throw;
    }
}
```

### 5. Add Controller Action

**Location**: `newsApi/Presentation/Controllers/NewsController.cs`

```csharp
/// <summary>
/// [Endpoint description]
/// </summary>
/// <param name="dto">The request data</param>
/// <returns>[Response description]</returns>
/// <response code="200">Success</response>
/// <response code="400">Bad request</response>
/// <response code="404">Not found</response>
/// <response code="500">Server error</response>
[HttpPost("route")]
[ProducesResponseType(typeof(ResultType), StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public async Task<ActionResult<ResultType>> YourAction([FromBody] YourDto dto)
{
    var result = await _newsService.YourMethodAsync(dto);
    return Ok(result);
}
```

### 6. Write Unit Tests

**Location**: `NewsApi.Tests/Unit/Application/Services/NewsServiceTests.cs`

```csharp
[Fact]
public async Task YourMethodAsync_ValidInput_ReturnsSuccess()
{
    // Arrange
    var dto = new YourDto { PropertyName = "Test" };
    var expectedResult = new Result();
    _mockRepository.Setup(r => r.RepositoryMethod(It.IsAny<Parameter>()))
        .ReturnsAsync(expectedResult);

    // Act
    var result = await _service.YourMethodAsync(dto);

    // Assert
    Assert.NotNull(result);
    _mockRepository.Verify(r => r.RepositoryMethod(It.IsAny<Parameter>()), Times.Once);
}

[Fact]
public async Task YourMethodAsync_InvalidInput_ThrowsException()
{
    // Arrange
    var dto = new YourDto { PropertyName = null };

    // Act & Assert
    await Assert.ThrowsAsync<ValidationException>(() => _service.YourMethodAsync(dto));
}
```

### 7. Write Integration Tests

**Location**: `NewsApi.Tests/Integration/Controllers/NewsControllerIntegrationTests.cs`

```csharp
[Fact]
public async Task YourAction_ValidRequest_ReturnsOk()
{
    // Arrange
    var dto = new YourDto { PropertyName = "Test" };
    var content = new StringContent(
        JsonSerializer.Serialize(dto),
        Encoding.UTF8,
        "application/json");

    // Act
    var response = await _client.PostAsync("/api/news/route", content);

    // Assert
    response.EnsureSuccessStatusCode();
    var responseBody = await response.Content.ReadAsStringAsync();
    Assert.NotNull(responseBody);
}
```

## 💡 Common Patterns

### GET Endpoint (Retrieve)
```csharp
[HttpGet("{id}")]
[ProducesResponseType(typeof(News), StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
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

### POST Endpoint (Create)
```csharp
[HttpPost]
[Authorize]
[ProducesResponseType(typeof(News), StatusCodes.Status201Created)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
public async Task<ActionResult<News>> Create([FromBody] CreateNewsDto dto)
{
    var news = await _newsService.CreateNewsAsync(dto);
    return CreatedAtAction(nameof(GetById), new { id = news.Id }, news);
}
```

### PUT Endpoint (Update)
```csharp
[HttpPut("{id}")]
[Authorize]
[ProducesResponseType(StatusCodes.Status204NoContent)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public async Task<IActionResult> Update(string id, [FromBody] UpdateNewsDto dto)
{
    await _newsService.UpdateNewsAsync(id, dto);
    return NoContent();
}
```

### DELETE Endpoint
```csharp
[HttpDelete("{id}")]
[Authorize]
[ProducesResponseType(StatusCodes.Status204NoContent)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public async Task<IActionResult> Delete(string id)
{
    await _newsService.DeleteNewsAsync(id);
    return NoContent();
}
```

## 🔍 Testing Checklist

Test all scenarios:
- ✅ Happy path (valid input, successful execution)
- ✅ Invalid input (null, empty, malformed)
- ✅ Not found scenarios
- ✅ Unauthorized access (if protected)
- ✅ Business rule violations
- ✅ Exception handling
- ✅ Cache behavior (if applicable)

## 📝 Documentation Requirements

Ensure you add:
1. **XML Comments** on controller actions
2. **ProducesResponseType** attributes
3. **Summary** and **remarks** tags
4. **Parameter** descriptions
5. **Response code** documentation

## 🚀 Verification Steps

After implementation:
1. Run `dotnet build` - Must succeed
2. Run `dotnet test` - All tests must pass
3. Start the API - `dotnet run`
4. Open Swagger UI - `http://localhost:5000/swagger`
5. Test the endpoint manually
6. Verify response codes
7. Check error handling

## Example: Complete Implementation

Here's a complete example for a "Search News" endpoint:

### DTO
```csharp
public class SearchNewsDto
{
    public string? Query { get; set; }
    public string? Category { get; set; }
    public int PageSize { get; set; } = 10;
}
```

### Validator
```csharp
public class SearchNewsDtoValidator : AbstractValidator<SearchNewsDto>
{
    public SearchNewsDtoValidator()
    {
        RuleFor(x => x.Query)
            .MinimumLength(3).When(x => !string.IsNullOrEmpty(x.Query))
            .WithMessage("Query must be at least 3 characters");
        
        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("PageSize must be between 1 and 100");
    }
}
```

### Service
```csharp
public async Task<IEnumerable<News>> SearchNewsAsync(SearchNewsDto dto)
{
    var allNews = await GetAllNewsAsync(dto.Category);
    
    if (!string.IsNullOrEmpty(dto.Query))
    {
        allNews = allNews.Where(n => 
            n.Caption.Contains(dto.Query, StringComparison.OrdinalIgnoreCase) ||
            n.Content.Contains(dto.Query, StringComparison.OrdinalIgnoreCase));
    }
    
    return allNews.Take(dto.PageSize);
}
```

### Controller
```csharp
[HttpPost("search")]
[ProducesResponseType(typeof(IEnumerable<News>), StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
public async Task<ActionResult<IEnumerable<News>>> Search([FromBody] SearchNewsDto dto)
{
    var results = await _newsService.SearchNewsAsync(dto);
    return Ok(results);
}
```

## 📚 Additional Resources

- See `NEWS_API_DOCUMENTATION.md` for API conventions
- See `csharp.instructions.md` for coding standards
- See `testing.instructions.md` for testing guidelines
