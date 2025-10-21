---
description: 'Debug and fix errors in the News API'
---

# Debug and Fix Issues

This prompt helps you systematically debug and resolve issues in the News API.

## 🔍 Debugging Workflow

### 1. Identify the Problem

**Questions to ask:**
- What is the expected behavior?
- What is the actual behavior?
- When did it start happening?
- Can you reproduce it consistently?
- What changed recently?

### 2. Gather Information

**Use these tools:**
```bash
# Check for compile errors
dotnet build

# Run all tests
dotnet test

# Check specific test category
dotnet test --filter "FullyQualifiedName~Unit"

# Run with verbose output
dotnet test --logger "console;verbosity=detailed"

# Check for lint/analysis warnings
dotnet build /p:TreatWarningsAsErrors=true
```

### 3. Examine Logs

**Check application logs:**
```csharp
// Look for log entries in console output
// Search for ERROR, WARNING, or exception stack traces

// Common log patterns in the codebase:
_logger.LogInformation("Operation successful: {Details}", details);
_logger.LogWarning("Potential issue: {Issue}", issue);
_logger.LogError(ex, "Error occurred: {Context}", context);
```

### 4. Use Debugging Tools

**Set breakpoints:**
- Controllers: Check incoming requests
- Services: Verify business logic
- Repositories: Examine data access
- Validators: Check validation rules

**Inspect variables:**
- Request parameters
- Service results
- Database queries
- Cache keys/values

## 🐛 Common Issues and Solutions

### Issue: NullReferenceException

**Symptoms:**
```
System.NullReferenceException: Object reference not set to an instance of an object
```

**Common Causes:**
1. Dependency not injected
2. Database query returns null
3. Configuration value missing
4. Cache miss without fallback

**Solution Pattern:**
```csharp
// ❌ Bad - No null check
public async Task<ActionResult<News>> GetById(string id)
{
    var news = await _service.GetByIdAsync(id);
    return Ok(news.Caption); // NullReferenceException if news is null
}

// ✅ Good - Proper null handling
public async Task<ActionResult<News>> GetById(string id)
{
    var news = await _service.GetByIdAsync(id);
    if (news == null)
    {
        return NotFound();
    }
    return Ok(news);
}

// ✅ Good - Null-conditional operator
public async Task<string> GetCaption(string id)
{
    var news = await _service.GetByIdAsync(id);
    return news?.Caption ?? "No caption available";
}
```

### Issue: MongoDB Connection Failure

**Symptoms:**
```
MongoDB.Driver.MongoConnectionException: Unable to connect to MongoDB
```

**Troubleshooting Steps:**
1. Check MongoDB is running: `mongosh` or `mongo`
2. Verify connection string in User Secrets
3. Check database name is correct
4. Verify network connectivity
5. Check MongoDB logs

**Solution:**
```bash
# Set correct connection string
dotnet user-secrets set "DatabaseSettings:ConnectionString" "mongodb://localhost:27017"
dotnet user-secrets set "DatabaseSettings:DatabaseName" "NewsDb"

# Verify secrets
dotnet user-secrets list
```

### Issue: Validation Errors

**Symptoms:**
```
FluentValidation.ValidationException: Validation failed
```

**Debugging:**
```csharp
// Add detailed logging in validators
public class CreateNewsDtoValidator : AbstractValidator<CreateNewsDto>
{
    public CreateNewsDtoValidator()
    {
        RuleFor(x => x.Caption)
            .NotEmpty().WithMessage("Caption is required")
            .MaximumLength(500).WithMessage("Caption must not exceed 500 characters")
            .Must(BeValidString).WithMessage("Caption contains invalid characters");
    }

    private bool BeValidString(string value)
    {
        // Add logging to debug validation
        Console.WriteLine($"Validating caption: {value}");
        return !string.IsNullOrWhiteSpace(value);
    }
}
```

**Test validators directly:**
```csharp
[Fact]
public void Validator_InvalidCaption_FailsValidation()
{
    // Arrange
    var validator = new CreateNewsDtoValidator();
    var dto = new CreateNewsDto { Caption = "" };

    // Act
    var result = validator.Validate(dto);

    // Assert
    Assert.False(result.IsValid);
    Assert.Contains(result.Errors, e => e.PropertyName == nameof(dto.Caption));
    
    // Output errors for debugging
    foreach (var error in result.Errors)
    {
        Console.WriteLine($"{error.PropertyName}: {error.ErrorMessage}");
    }
}
```

### Issue: JWT Authentication Failure

**Symptoms:**
```
401 Unauthorized
```

**Troubleshooting:**
1. Check JWT secret is set
2. Verify token format
3. Check token expiration
4. Validate issuer/audience

**Solution:**
```bash
# Ensure JWT settings are configured
dotnet user-secrets set "JwtSettings:SecretKey" "your-secret-key-must-be-at-least-32-characters-long"
dotnet user-secrets set "JwtSettings:Issuer" "NewsApi"
dotnet user-secrets set "JwtSettings:Audience" "NewsApiUsers"
dotnet user-secrets set "JwtSettings:ExpirationMinutes" "60"
```

**Test authentication:**
```csharp
[Fact]
public async Task CreateNews_WithoutAuth_ReturnsUnauthorized()
{
    // Arrange
    var dto = new CreateNewsDto { Caption = "Test" };
    var content = new StringContent(JsonSerializer.Serialize(dto));

    // Act
    var response = await _client.PostAsync("/api/news", content);

    // Assert
    Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
}
```

### Issue: Caching Problems

**Symptoms:**
- Stale data returned
- Cache not being used
- Memory leaks

**Debugging:**
```csharp
public async Task<IEnumerable<News>> GetAllNewsAsync(string? category = null)
{
    var cacheKey = category != null 
        ? $"{CacheKeys.AllNews}:{category}" 
        : CacheKeys.AllNews;

    // Add logging to debug cache behavior
    _logger.LogInformation("Checking cache for key: {CacheKey}", cacheKey);

    if (_cache.TryGetValue(cacheKey, out IEnumerable<News>? cachedNews))
    {
        _logger.LogInformation("Cache HIT for key: {CacheKey}", cacheKey);
        return cachedNews!;
    }

    _logger.LogInformation("Cache MISS for key: {CacheKey}", cacheKey);
    var news = await _repository.GetAllAsync();
    
    // Log cache set operation
    _logger.LogInformation("Setting cache for key: {CacheKey}", cacheKey);
    _cache.Set(cacheKey, news, TimeSpan.FromMinutes(30));
    
    return news;
}
```

**Clear cache for testing:**
```csharp
// In service
public void ClearCache()
{
    _cache.Remove(CacheKeys.AllNews);
    // Remove category-specific caches
}
```

### Issue: Test Failures

**Symptoms:**
```
Test Failed
Expected: 5
Actual: 3
```

**Debugging Tests:**
```csharp
[Fact]
public async Task GetAllNews_ReturnsCorrectCount()
{
    // Arrange
    var expectedNews = new List<News>
    {
        new News { Id = "1", Category = "Tech" },
        new News { Id = "2", Category = "Science" }
    };

    _mockRepository.Setup(r => r.GetAllAsync())
        .ReturnsAsync(expectedNews);

    // Add debugging output
    var setupResult = _mockRepository.Object.GetAllAsync().Result;
    _output.WriteLine($"Mock returns {setupResult.Count()} items");

    // Act
    var result = await _service.GetAllNewsAsync();

    // Debug output
    _output.WriteLine($"Service returned {result.Count()} items");
    foreach (var news in result)
    {
        _output.WriteLine($"News: {news.Id} - {news.Category}");
    }

    // Assert
    Assert.Equal(expectedNews.Count, result.Count());
}
```

## 🔧 Debugging Techniques

### 1. Add Detailed Logging

```csharp
public async Task<News> CreateNewsAsync(CreateNewsDto dto)
{
    _logger.LogInformation("CreateNewsAsync started with dto: {@Dto}", dto);
    
    try
    {
        var news = MapToNews(dto);
        _logger.LogInformation("Mapped dto to news entity: {@News}", news);
        
        var created = await _repository.CreateAsync(news);
        _logger.LogInformation("News created successfully with Id: {Id}", created.Id);
        
        return created;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error creating news from dto: {@Dto}", dto);
        throw;
    }
}
```

### 2. Use Swagger UI for Manual Testing

1. Navigate to `http://localhost:5000/swagger`
2. Find the problematic endpoint
3. Click "Try it out"
4. Fill in test data
5. Click "Execute"
6. Examine request/response
7. Check status code and body

### 3. Inspect MongoDB Directly

```bash
# Connect to MongoDB
mongosh

# Use the database
use NewsDb

# Find all documents
db.news.find().pretty()

# Find specific document
db.news.findOne({ _id: "your-id" })

# Count documents
db.news.countDocuments()

# Check indexes
db.news.getIndexes()
```

### 4. Use Postman/cURL for Testing

```bash
# GET request
curl -X GET "http://localhost:5000/api/news" -H "accept: application/json"

# POST request with auth
curl -X POST "http://localhost:5000/api/news" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"category":"Tech","caption":"Test","content":"Content"}'

# PUT request
curl -X PUT "http://localhost:5000/api/news/123" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"caption":"Updated"}'

# DELETE request
curl -X DELETE "http://localhost:5000/api/news/123" \
  -H "Authorization: Bearer YOUR_TOKEN"
```

## 🎯 Systematic Debugging Steps

### Step 1: Reproduce the Issue
- Create a minimal test case
- Document exact steps to reproduce
- Note any error messages

### Step 2: Isolate the Problem
- Is it in the controller, service, or repository?
- Is it a data issue or logic issue?
- Does it happen in tests or only in production?

### Step 3: Form a Hypothesis
- What do you think is causing the issue?
- What would you expect to see if your hypothesis is correct?

### Step 4: Test Your Hypothesis
- Add logging or breakpoints
- Run the code
- Examine the results

### Step 5: Fix and Verify
- Implement the fix
- Run all tests
- Manually verify the fix
- Document what was wrong and how you fixed it

## 📝 Debugging Checklist

When debugging, check:
- [ ] All dependencies are injected correctly
- [ ] Configuration values are set (User Secrets)
- [ ] MongoDB is running and accessible
- [ ] All required fields have values
- [ ] Data types match between DTO and entity
- [ ] Validation rules are correct
- [ ] Async methods use `await`
- [ ] Exception handling is in place
- [ ] Logging statements are present
- [ ] Tests cover the scenario
- [ ] Cache keys are correct
- [ ] HTTP status codes are appropriate

## 🚨 Emergency Fixes

### Quick Reset
```bash
# Clear all caches and rebuild
dotnet clean
dotnet restore
dotnet build

# Reset user secrets
dotnet user-secrets clear
# Re-add necessary secrets

# Restart MongoDB
# Windows: Restart MongoDB service
# Mac/Linux: sudo systemctl restart mongod

# Run tests
dotnet test
```

### Check Health
```bash
# Hit health endpoint
curl http://localhost:5000/health

# Should return "Healthy" if everything is OK
```

## 📚 Additional Resources

- Check `NEWS_API_DOCUMENTATION.md` for API reference
- Review `csharp.instructions.md` for coding patterns
- See `testing.instructions.md` for test debugging
- Examine recent git commits for changes: `git log --oneline -10`
