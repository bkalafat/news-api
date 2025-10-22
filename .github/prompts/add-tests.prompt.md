---
description: 'Add comprehensive tests for existing code'
---

# Add Tests for Existing Code

This prompt guides you through adding comprehensive test coverage for existing News API code.

## 🎯 Goal

Add unit and integration tests following the AAA pattern (Arrange-Act-Assert) with proper mocking and edge case coverage.

## 📝 Test Coverage Analysis

### Check Current Coverage
```bash
# Run tests with coverage
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover

# View coverage report
# Coverage results will show which files need tests
```

### Identify Gaps
Look for:
- Methods without tests
- Edge cases not covered
- Error paths not tested
- Integration scenarios missing

## 🧪 Unit Test Template

### Service Test Structure
```csharp
using Xunit;
using Moq;
using NewsApi.Application.Services;
using NewsApi.Domain.Interfaces;

namespace NewsApi.Tests.Unit.Application.Services
{
    public class NewsServiceTests
    {
        private readonly Mock<INewsRepository> _mockRepository;
        private readonly Mock<IMemoryCache> _mockCache;
        private readonly Mock<ILogger<NewsService>> _mockLogger;
        private readonly NewsService _service;

        public NewsServiceTests()
        {
            // Arrange - Set up mocks
            _mockRepository = new Mock<INewsRepository>();
            _mockCache = new Mock<IMemoryCache>();
            _mockLogger = new Mock<ILogger<NewsService>>();
            
            // Create service with mocks
            _service = new NewsService(
                _mockRepository.Object,
                _mockCache.Object,
                _mockLogger.Object);
        }

        [Fact]
        public async Task MethodName_HappyPath_ReturnsExpectedResult()
        {
            // Arrange
            var input = CreateTestInput();
            var expectedOutput = CreateExpectedOutput();
            
            _mockRepository
                .Setup(r => r.SomeMethod(It.IsAny<Parameter>()))
                .ReturnsAsync(expectedOutput);

            // Act
            var result = await _service.MethodName(input);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedOutput.Property, result.Property);
            _mockRepository.Verify(r => r.SomeMethod(It.IsAny<Parameter>()), Times.Once);
        }

        [Fact]
        public async Task MethodName_InvalidInput_ThrowsException()
        {
            // Arrange
            var invalidInput = CreateInvalidInput();

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(
                () => _service.MethodName(invalidInput));
        }

        [Fact]
        public async Task MethodName_NotFound_ReturnsNull()
        {
            // Arrange
            _mockRepository
                .Setup(r => r.GetById(It.IsAny<string>()))
                .ReturnsAsync((News?)null);

            // Act
            var result = await _service.GetByIdAsync("nonexistent");

            // Assert
            Assert.Null(result);
        }

        // Helper methods
        private static InputDto CreateTestInput()
        {
            return new InputDto
            {
                // Set properties
            };
        }

        private static ExpectedOutput CreateExpectedOutput()
        {
            return new ExpectedOutput
            {
                // Set properties
            };
        }
    }
}
```

### Validator Test Structure
```csharp
using Xunit;
using FluentValidation.TestHelper;

namespace NewsApi.Tests.Unit.Validators
{
    public class CreateNewsDtoValidatorTests
    {
        private readonly CreateNewsDtoValidator _validator;

        public CreateNewsDtoValidatorTests()
        {
            _validator = new CreateNewsDtoValidator();
        }

        [Fact]
        public void Validate_ValidDto_PassesValidation()
        {
            // Arrange
            var dto = new CreateNewsDto
            {
                Category = "Technology",
                Caption = "Valid Caption",
                Content = "Valid Content",
                Summary = "Valid Summary",
                ExpressDate = DateTime.UtcNow
            };

            // Act
            var result = _validator.TestValidate(dto);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_EmptyCaption_FailsValidation()
        {
            // Arrange
            var dto = new CreateNewsDto { Caption = "" };

            // Act
            var result = _validator.TestValidate(dto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Caption);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Validate_InvalidCategory_FailsValidation(string category)
        {
            // Arrange
            var dto = new CreateNewsDto { Category = category };

            // Act
            var result = _validator.TestValidate(dto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Category);
        }

        [Fact]
        public void Validate_CaptionTooLong_FailsValidation()
        {
            // Arrange
            var dto = new CreateNewsDto 
            { 
                Caption = new string('a', 501) // Exceeds 500 char limit
            };

            // Act
            var result = _validator.TestValidate(dto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Caption)
                  .WithErrorMessage("Caption must not exceed 500 characters");
        }
    }
}
```

## 🌐 Integration Test Template

### Controller Test Structure
```csharp
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using Xunit;

namespace NewsApi.Tests.Integration.Controllers
{
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
            Assert.Equal("application/json; charset=utf-8", 
                response.Content.Headers.ContentType?.ToString());
            
            var news = await response.Content.ReadFromJsonAsync<List<News>>();
            Assert.NotNull(news);
        }

        [Fact]
        public async Task GetAll_WithCategory_FiltersResults()
        {
            // Act
            var response = await _client.GetAsync("/api/news?category=Technology");

            // Assert
            response.EnsureSuccessStatusCode();
            var news = await response.Content.ReadFromJsonAsync<List<News>>();
            Assert.All(news!, n => Assert.Equal("Technology", n.Category));
        }

        [Fact]
        public async Task GetById_ValidId_ReturnsNews()
        {
            // Arrange - Get a valid ID first
            var allNews = await _client.GetFromJsonAsync<List<News>>("/api/news");
            var validId = allNews?.FirstOrDefault()?.Id;
            
            if (validId != null)
            {
                // Act
                var response = await _client.GetAsync($"/api/news/{validId}");

                // Assert
                response.EnsureSuccessStatusCode();
                var news = await response.Content.ReadFromJsonAsync<News>();
                Assert.NotNull(news);
                Assert.Equal(validId, news.Id);
            }
        }

        [Fact]
        public async Task GetById_InvalidId_ReturnsNotFound()
        {
            // Act
            var response = await _client.GetAsync("/api/news/nonexistent-id");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Create_WithoutAuth_ReturnsUnauthorized()
        {
            // Arrange
            var dto = new CreateNewsDto
            {
                Category = "Technology",
                Caption = "Test Article",
                Content = "Test Content"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/news", dto);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
```

## 🎯 Test Scenarios to Cover

### For Services
- ✅ Happy path with valid input
- ✅ Invalid/null input
- ✅ Not found scenarios
- ✅ Exception handling
- ✅ Cache hit scenarios
- ✅ Cache miss scenarios
- ✅ Multiple results
- ✅ Empty results
- ✅ Concurrent operations (if applicable)

### For Validators
- ✅ Valid input passes
- ✅ Required fields missing
- ✅ Length constraints (min/max)
- ✅ Format validation (URLs, emails, dates)
- ✅ Business rule validation
- ✅ Edge cases (boundary values)
- ✅ Multiple validation failures

### For Controllers
- ✅ All HTTP methods (GET, POST, PUT, DELETE)
- ✅ Success responses (200, 201, 204)
- ✅ Error responses (400, 401, 404, 500)
- ✅ Query parameter variations
- ✅ Request body validation
- ✅ Authentication/Authorization
- ✅ Content negotiation

### For Repositories
- ✅ CRUD operations
- ✅ Filtering and querying
- ✅ Mapping to/from database models
- ✅ Not found scenarios
- ✅ Database exceptions
- ✅ Null handling

## 🔍 Testing Checklist

Before considering tests complete:

- [ ] All public methods have tests
- [ ] Happy path is tested
- [ ] Error paths are tested
- [ ] Edge cases are covered
- [ ] Null/empty inputs are tested
- [ ] Boundary values are tested
- [ ] Async methods are properly tested
- [ ] Mocks are verified
- [ ] Integration tests cover API endpoints
- [ ] Test names follow convention
- [ ] Tests are independent
- [ ] Tests are fast (unit tests < 100ms)
- [ ] No flaky tests
- [ ] Code coverage > 80%

## 🛠️ Helper Utilities

### Test Data Builders
```csharp
public class NewsTestDataBuilder
{
    private string _id = Guid.NewGuid().ToString();
    private string _category = "Technology";
    private string _caption = "Test News Article";
    private string _content = "Test content";
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
    .Inactive()
    .Build();
```

### Mock Cache Helper
```csharp
public class TestMemoryCache : IMemoryCache
{
    private readonly Dictionary<object, object> _cache = new();

    public ICacheEntry CreateEntry(object key)
    {
        var entry = new TestCacheEntry(key);
        entry.PostEvictionCallbacks.Add(new PostEvictionCallbackRegistration
        {
            EvictionCallback = (k, v, r, s) => _cache.Remove(k)
        });
        return entry;
    }

    public void Remove(object key) => _cache.Remove(key);

    public bool TryGetValue(object key, out object? value) 
        => _cache.TryGetValue(key, out value);

    public void Dispose() => _cache.Clear();
}
```

## 📊 Running Tests

```bash
# Run all tests
dotnet test

# Run specific test class
dotnet test --filter "FullyQualifiedName~NewsServiceTests"

# Run tests matching pattern
dotnet test --filter "FullyQualifiedName~GetById"

# Run with verbose output
dotnet test --logger "console;verbosity=detailed"

# Run with coverage
dotnet test /p:CollectCoverage=true
```

## 📚 Additional Resources

- See `testing.instructions.md` for detailed testing guidelines
- See `csharp.instructions.md` for code patterns
- Review existing tests in `tests/` for examples
- [xUnit Documentation](https://xunit.net/)
- [Moq Documentation](https://github.com/moq/moq4)

