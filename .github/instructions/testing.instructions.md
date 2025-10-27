---
applyTo: '**/Tests/**/*.cs'
---

# Testing Instructions

These instructions apply to all test files in the NewsApi.Tests project.

## 🧪 Testing Framework

- **Test Framework**: xUnit
- **Mocking**: Moq
- **Integration Testing**: WebApplicationFactory
- **Test Runner**: dotnet test

## 🎯 Testing Principles

### Test Naming Convention
```
MethodName_Scenario_ExpectedBehavior
```

**Examples**:
- `GetByIdAsync_ValidId_ReturnsNews`
- `GetByIdAsync_InvalidId_ReturnsNull`
- `CreateAsync_ValidDto_ReturnsCreatedNews`
- `CreateAsync_InvalidDto_ThrowsValidationException`

### AAA Pattern (Arrange-Act-Assert)
All tests must follow this structure:

```csharp
[Fact]
public async Task GetByIdAsync_ValidId_ReturnsNews()
{
    // Arrange - Set up test data and mocks
    var expectedNews = new News { Id = "123", Caption = "Test News" };
    _mockRepository.Setup(r => r.GetByIdAsync("123"))
        .ReturnsAsync(expectedNews);

    // Act - Execute the method under test
    var result = await _service.GetByIdAsync("123");

    // Assert - Verify the results
    Assert.NotNull(result);
    Assert.Equal("Test News", result.Caption);
    Assert.Equal("123", result.Id);
}
```

## 📝 Test Categories

### Unit Tests
**Location**: `tests/Unit/`

**Purpose**: Test individual components in isolation
- Services
- Validators
- DTOs
- Mappers
- Utilities

**Example**:
```csharp
public class NewsServiceTests
{
    private readonly Mock<INewsRepository> _mockRepository;
    private readonly Mock<IMemoryCache> _mockCache;
    private readonly Mock<ILogger<NewsService>> _mockLogger;
    private readonly NewsService _service;

    public NewsServiceTests()
    {
        _mockRepository = new Mock<INewsRepository>();
        _mockCache = new Mock<IMemoryCache>();
        _mockLogger = new Mock<ILogger<NewsService>>();
        _service = new NewsService(_mockRepository.Object, _mockCache.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetAllNewsAsync_NoCategory_ReturnsAllNews()
    {
        // Arrange
        var expectedNews = new List<News>
        {
            new News { Id = "1", Category = "Tech" },
            new News { Id = "2", Category = "Science" }
        };
        
        _mockRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(expectedNews);
        
        object? cacheEntry = null;
        _mockCache.Setup(c => c.TryGetValue(It.IsAny<object>(), out cacheEntry))
            .Returns(false);

        // Act
        var result = await _service.GetAllNewsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }
}
```

### Integration Tests
**Location**: `tests/Integration/`

**Purpose**: Test components working together
- Controllers with full HTTP pipeline
- Database operations
- Middleware

**Example**:
```csharp
public class NewsControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public NewsControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_ReturnsSuccessAndNews()
    {
        // Act
        var response = await _client.GetAsync("/api/news");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.NotNull(content);
        Assert.Contains("Technology", content);
    }

    [Fact]
    public async Task GetById_ValidId_ReturnsNews()
    {
        // Arrange
        var newsId = "valid-test-id";

        // Act
        var response = await _client.GetAsync($"/api/news/{newsId}");

        // Assert
        if (response.StatusCode == System.Net.HttpStatusCode.OK)
        {
            var content = await response.Content.ReadAsStringAsync();
            Assert.NotNull(content);
            Assert.Contains(newsId, content);
        }
        else
        {
            Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
```

### Performance Tests
**Location**: `tests/Performance/`

**Purpose**: Benchmark critical operations

**Example**:
```csharp
[Fact]
public async Task GetAllNews_PerformanceTest()
{
    // Arrange
    var stopwatch = Stopwatch.StartNew();

    // Act
    var result = await _service.GetAllNewsAsync();
    stopwatch.Stop();

    // Assert
    Assert.True(stopwatch.ElapsedMilliseconds < 200, 
        $"GetAllNews took {stopwatch.ElapsedMilliseconds}ms, expected < 200ms");
}
```

## 🔧 Moq Guidelines

### Setup Methods
```csharp
// Simple return
_mockRepository.Setup(r => r.GetByIdAsync("123"))
    .ReturnsAsync(expectedNews);

// Return based on parameter
_mockRepository.Setup(r => r.GetByIdAsync(It.IsAny<string>()))
    .ReturnsAsync((string id) => id == "123" ? expectedNews : null);

// Throw exception
_mockRepository.Setup(r => r.DeleteAsync("invalid"))
    .ThrowsAsync(new NotFoundException("News not found"));

// Verify method was called
_mockRepository.Verify(r => r.GetByIdAsync("123"), Times.Once);
_mockRepository.Verify(r => r.CreateAsync(It.IsAny<News>()), Times.Never);
```

### Argument Matching
```csharp
// Any value
_mockService.Setup(s => s.Process(It.IsAny<string>()))
    .Returns(true);

// Specific value
_mockService.Setup(s => s.Process("test"))
    .Returns(true);

// Condition
_mockService.Setup(s => s.Process(It.Is<string>(x => x.Length > 5)))
    .Returns(true);

// Regex match
_mockService.Setup(s => s.Process(It.IsRegex(@"^\d+$")))
    .Returns(true);
```

## 📊 Test Data Builders

Use test data builders for complex objects:

```csharp
public class NewsTestDataBuilder
{
    private string _id = Guid.NewGuid().ToString();
    private string _category = "Technology";
    private string _caption = "Test News";
    private string _content = "Test Content";
    private bool _isActive = true;

    public NewsTestDataBuilder WithId(string id)
    {
        _id = id;
        return this;
    }

    public NewsTestDataBuilder WithCategory(string category)
    {
        _category = category;
        return this;
    }

    public NewsTestDataBuilder WithCaption(string caption)
    {
        _caption = caption;
        return this;
    }

    public NewsTestDataBuilder Inactive()
    {
        _isActive = false;
        return this;
    }

    public News Build()
    {
        return new News
        {
            Id = _id,
            Category = _category,
            Caption = _caption,
            Content = _content,
            IsActive = _isActive,
            CreateDate = DateTime.UtcNow,
            ExpressDate = DateTime.UtcNow
        };
    }
}

// Usage
var testNews = new NewsTestDataBuilder()
    .WithCategory("Science")
    .WithCaption("Breaking Discovery")
    .Inactive()
    .Build();
```

## ✅ What to Test

### Services
- ✅ All public methods
- ✅ Edge cases (null, empty, boundary values)
- ✅ Error conditions
- ✅ Cache hits and misses
- ✅ Repository interactions

### Validators
- ✅ Required field validation
- ✅ Max length validation
- ✅ Format validation (URLs, dates)
- ✅ Business rule validation
- ✅ Custom validation logic

### Controllers
- ✅ All HTTP endpoints
- ✅ Success scenarios (200, 201, 204)
- ✅ Error scenarios (400, 404, 500)
- ✅ Authentication/Authorization
- ✅ Request/Response serialization

### Repositories
- ✅ CRUD operations
- ✅ Query filtering
- ✅ Mapping between domain and database models
- ✅ Error handling

## ❌ What NOT to Test

- ❌ Private methods (test through public interface)
- ❌ Third-party library internals
- ❌ Auto-properties (getters/setters)
- ❌ Framework code (ASP.NET, Entity Framework)
- ❌ Trivial code (obvious assignments)

## 🎭 Test Doubles

### Mock vs Stub vs Fake

```csharp
// Mock - Verify interactions
var mockRepo = new Mock<INewsRepository>();
mockRepo.Setup(r => r.GetByIdAsync("123")).ReturnsAsync(news);
// ... test code ...
mockRepo.Verify(r => r.GetByIdAsync("123"), Times.Once);

// Stub - Provide canned responses
var stubCache = new TestMemoryCache(); // Simple implementation
stubCache.Set("key", value);

// Fake - Working implementation
var fakeRepo = new InMemoryNewsRepository(); // Actual logic, no database
```

## 📐 Test Coverage Goals

- **Unit Tests**: > 80% code coverage
- **Integration Tests**: Cover all API endpoints
- **Critical Paths**: 100% coverage for business logic

## 🧩 Test Organization

### File Structure
```
tests/
├── Unit/
│   ├── Application/
│   │   ├── Services/
│   │   │   └── NewsServiceTests.cs
│   │   └── Validators/
│   │       └── NewsDtoValidatorTests.cs
│   ├── Domain/
│   │   └── Entities/
│   │       └── NewsTests.cs
│   └── Infrastructure/
│       └── Repositories/
│           └── NewsRepositoryTests.cs
├── Integration/
│   └── Controllers/
│       └── NewsControllerTests.cs
├── Performance/
│   └── NewsPerformanceTests.cs
└── Helpers/
    ├── TestDataBuilders.cs
    └── TestMemoryCache.cs
```

### Test Class Naming
- Test class name = `{ClassUnderTest}Tests`
- Example: `NewsService` → `NewsServiceTests`

## 🔍 Assertions

### xUnit Assertions
```csharp
// Equality
Assert.Equal(expected, actual);
Assert.NotEqual(expected, actual);

// Null checks
Assert.Null(value);
Assert.NotNull(value);

// Boolean
Assert.True(condition);
Assert.False(condition);

// Exceptions
await Assert.ThrowsAsync<ArgumentException>(() => service.MethodAsync(null));

// Collections
Assert.Empty(collection);
Assert.NotEmpty(collection);
Assert.Contains(item, collection);
Assert.DoesNotContain(item, collection);
Assert.All(collection, item => Assert.True(item > 0));

// Type checks
Assert.IsType<News>(result);
Assert.IsAssignableFrom<IEnumerable<News>>(result);

// Ranges
Assert.InRange(value, low, high);
```

## 🎯 Test Attributes

```csharp
// Simple test
[Fact]
public void SimpleTest() { }

// Parameterized test
[Theory]
[InlineData("Technology")]
[InlineData("Science")]
[InlineData("Business")]
public void TestWithParameters(string category) { }

// Multiple data sets
[Theory]
[MemberData(nameof(GetTestData))]
public void TestWithComplexData(string input, int expected) { }

public static IEnumerable<object[]> GetTestData()
{
    yield return new object[] { "test", 4 };
    yield return new object[] { "hello", 5 };
}

// Skip test
[Fact(Skip = "Not implemented yet")]
public void FutureTest() { }
```

## 📚 Testing Best Practices

1. **Independence**: Tests should not depend on each other
2. **Repeatability**: Tests should produce same results every time
3. **Fast**: Unit tests should complete in milliseconds
4. **Readable**: Test names and structure should be clear
5. **One Concept**: Test one thing at a time
6. **No Logic**: Avoid conditionals and loops in tests
7. **Setup/Teardown**: Use constructors and IDisposable for cleanup

## 🔗 References

- [xUnit Documentation](https://xunit.net/)
- [Moq Quickstart](https://github.com/moq/moq4/wiki/Quickstart)
- [Testing ASP.NET Core Applications](https://learn.microsoft.com/en-us/aspnet/core/test/)

