using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using NewsApi.Application.DTOs;
using NewsApi.Domain.Entities;
using NewsApi.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;

namespace NewsApi.Tests.Integration;

public class NewsControllerIntegrationTests : IClassFixture<NewsApiWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly NewsApiWebApplicationFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    public NewsControllerIntegrationTests(NewsApiWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    [Fact]
    public async Task GetAllNews_WhenNoNews_ShouldReturnEmptyArray()
    {
        // Act
        var response = await _client.GetAsync("/api/news");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        var news = JsonSerializer.Deserialize<List<News>>(content, _jsonOptions);
        news.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllNews_WhenNewsExists_ShouldReturnNews()
    {
        // Arrange
        await SeedTestData();

        // Act
        var response = await _client.GetAsync("/api/news");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        var news = JsonSerializer.Deserialize<List<News>>(content, _jsonOptions);
        news.Should().NotBeEmpty();
        news!.Count.Should().BeGreaterThan(0);
        news.All(n => n.IsActive).Should().BeTrue();
    }

    [Fact]
    public async Task GetAllNews_WithCategoryFilter_ShouldReturnFilteredNews()
    {
        // Arrange
        await SeedTestData();

        // Act
        var response = await _client.GetAsync("/api/news?category=Technology");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        var news = JsonSerializer.Deserialize<List<News>>(content, _jsonOptions);
        news.Should().NotBeEmpty();
        news!.All(n => n.Category.Equals("Technology", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
    }

    [Fact]
    public async Task GetAllNews_WithTypeFilter_ShouldReturnFilteredNews()
    {
        // Arrange
        await SeedTestData();

        // Act
        var response = await _client.GetAsync("/api/news?type=Article");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        var news = JsonSerializer.Deserialize<List<News>>(content, _jsonOptions);
        news.Should().NotBeEmpty();
        news!.All(n => n.Type.Equals("Article", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
    }

    [Fact]
    public async Task GetNewsById_WhenExists_ShouldReturnNews()
    {
        // Arrange
        var newsId = await SeedSingleNews();

        // Act
        var response = await _client.GetAsync($"/api/news/{newsId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        var news = JsonSerializer.Deserialize<News>(content, _jsonOptions);
        news.Should().NotBeNull();
        news!.Id.Should().Be(newsId);
    }

    [Fact]
    public async Task GetNewsById_WhenNotExists_ShouldReturnNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/news/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetNewsByUrl_WhenExists_ShouldReturnNews()
    {
        // Arrange
        var newsUrl = "test-article-url";
        await SeedNewsWithUrl(newsUrl);

        // Act
        var response = await _client.GetAsync($"/api/news/url/{newsUrl}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        var news = JsonSerializer.Deserialize<News>(content, _jsonOptions);
        news.Should().NotBeNull();
        news!.Url.Should().Be(newsUrl);
    }

    [Fact]
    public async Task GetNewsByUrl_WhenNotExists_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/news/url/non-existent-url");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateNews_WithValidData_ShouldCreateNews()
    {
        // Arrange
        var createDto = new CreateNewsDto
        {
            Category = "Technology",
            Type = "Article",
            Caption = "Integration Test News",
            Keywords = "test, integration",
            SocialTags = "#test #integration",
            Summary = "This is a test news article for integration testing",
            ImgPath = "/images/test.jpg",
            ImgAlt = "Test image",
            Content = "Full content of the integration test news article",
            ExpressDate = DateTime.UtcNow,
            Priority = 1,
            Url = "integration-test-news",
            Subjects = new[] { "Testing", "Integration" },
            Authors = new[] { "Test Author" }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/news", createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var content = await response.Content.ReadAsStringAsync();
        var createdNews = JsonSerializer.Deserialize<News>(content, _jsonOptions);
        
        createdNews.Should().NotBeNull();
        createdNews!.Id.Should().NotBeEmpty();
        createdNews.Category.Should().Be(createDto.Category);
        createdNews.Caption.Should().Be(createDto.Caption);
        createdNews.IsActive.Should().BeTrue();
        createdNews.CreateDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
    }

    [Fact]
    public async Task CreateNews_WithInvalidData_ShouldReturnBadRequest()
    {
        // Arrange - Missing required fields
        var invalidDto = new CreateNewsDto
        {
            Category = "", // Required field is empty
            Caption = "", // Required field is empty
            Summary = "", // Required field is empty
            Content = "" // Required field is empty
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/news", invalidDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateNews_WithValidData_ShouldUpdateNews()
    {
        // Arrange
        var newsId = await SeedSingleNews();
        var updateDto = new UpdateNewsDto
        {
            Category = "Updated Category",
            Caption = "Updated Caption",
            Summary = "Updated summary",
            Content = "Updated content"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/news/{newsId}", updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify the update
        var getResponse = await _client.GetAsync($"/api/news/{newsId}");
        var content = await getResponse.Content.ReadAsStringAsync();
        var updatedNews = JsonSerializer.Deserialize<News>(content, _jsonOptions);
        
        updatedNews!.Category.Should().Be(updateDto.Category);
        updatedNews.Caption.Should().Be(updateDto.Caption);
        updatedNews.Summary.Should().Be(updateDto.Summary);
        updatedNews.Content.Should().Be(updateDto.Content);
    }

    [Fact]
    public async Task UpdateNews_WhenNotExists_ShouldReturnNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var updateDto = new UpdateNewsDto
        {
            Category = "Updated Category",
            Caption = "Updated Caption"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/news/{nonExistentId}", updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteNews_WhenExists_ShouldDeleteNews()
    {
        // Arrange
        var newsId = await SeedSingleNews();

        // Act
        var response = await _client.DeleteAsync($"/api/news/{newsId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify the news is deleted
        var getResponse = await _client.GetAsync($"/api/news/{newsId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteNews_WhenNotExists_ShouldReturnNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await _client.DeleteAsync($"/api/news/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private async Task SeedTestData()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<MongoDbContext>();

        var testNews = new List<News>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Category = "Technology",
                Type = "Article",
                Caption = "Tech News 1",
                Summary = "Technology summary",
                Content = "Technology content",
                IsActive = true,
                ExpressDate = DateTime.UtcNow,
                CreateDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow,
                Priority = 1
            },
            new()
            {
                Id = Guid.NewGuid(),
                Category = "Sports",
                Type = "Breaking",
                Caption = "Sports News 1",
                Summary = "Sports summary",
                Content = "Sports content",
                IsActive = true,
                ExpressDate = DateTime.UtcNow,
                CreateDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow,
                Priority = 2
            },
            new()
            {
                Id = Guid.NewGuid(),
                Category = "Technology",
                Type = "Feature",
                Caption = "Tech News 2",
                Summary = "Another tech summary",
                Content = "Another tech content",
                IsActive = true,
                ExpressDate = DateTime.UtcNow,
                CreateDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow,
                Priority = 1
            }
        };

        await context.News.InsertManyAsync(testNews);
    }

    private async Task<Guid> SeedSingleNews()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<MongoDbContext>();

        var news = new News
        {
            Id = Guid.NewGuid(),
            Category = "Test Category",
            Type = "Test Type",
            Caption = "Test News",
            Summary = "Test summary",
            Content = "Test content",
            IsActive = true,
            ExpressDate = DateTime.UtcNow,
            CreateDate = DateTime.UtcNow,
            UpdateDate = DateTime.UtcNow,
            Priority = 1
        };

        await context.News.InsertOneAsync(news);
        return news.Id;
    }

    private async Task SeedNewsWithUrl(string url)
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<MongoDbContext>();

        var news = new News
        {
            Id = Guid.NewGuid(),
            Category = "Test Category",
            Type = "Test Type",
            Caption = "Test News with URL",
            Summary = "Test summary",
            Content = "Test content",
            Url = url,
            IsActive = true,
            ExpressDate = DateTime.UtcNow,
            CreateDate = DateTime.UtcNow,
            UpdateDate = DateTime.UtcNow,
            Priority = 1
        };

        await context.News.InsertOneAsync(news);
    }
}