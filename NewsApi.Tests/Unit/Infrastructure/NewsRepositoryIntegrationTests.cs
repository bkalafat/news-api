using FluentAssertions;
using MongoDB.Driver;
using NewsApi.Domain.Entities;
using NewsApi.Infrastructure.Data;
using NewsApi.Infrastructure.Data.Configurations;
using NewsApi.Infrastructure.Data.Repositories;
using Testcontainers.MongoDb;

namespace NewsApi.Tests.Unit.Infrastructure;

public class NewsRepositoryIntegrationTests : IAsyncLifetime
{
    private readonly MongoDbContainer _mongoContainer;
    private MongoDbContext _context = null!;
    private NewsRepository _repository = null!;

    public NewsRepositoryIntegrationTests()
    {
        _mongoContainer = new MongoDbBuilder()
            .WithImage("mongo:7.0")
            .WithPortBinding(27017, true)
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _mongoContainer.StartAsync();

        var connectionString = _mongoContainer.GetConnectionString();
        var settings = new MongoDbSettings
        {
            ConnectionString = connectionString,
            DatabaseName = "TestNewsDb",
            NewsCollectionName = "News"
        };

        _context = new MongoDbContext(settings);
        _repository = new NewsRepository(_context);
    }

    public async Task DisposeAsync()
    {
        await _mongoContainer.DisposeAsync();
    }

    [Fact]
    public async Task GetAllAsync_WhenNoActiveNews_ShouldReturnEmptyList()
    {
        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllAsync_WhenActiveNewsExists_ShouldReturnOnlyActiveNews()
    {
        // Arrange
        var activeNews = new News
        {
            Id = Guid.NewGuid(),
            Caption = "Active News",
            IsActive = true,
            ExpressDate = DateTime.UtcNow
        };

        var inactiveNews = new News
        {
            Id = Guid.NewGuid(),
            Caption = "Inactive News",
            IsActive = false,
            ExpressDate = DateTime.UtcNow
        };

        await _context.News.InsertManyAsync([activeNews, inactiveNews]);

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(1);
        result[0].Should().BeEquivalentTo(activeNews);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnNewsOrderedByExpressDateDescending()
    {
        // Arrange
        var olderNews = new News
        {
            Id = Guid.NewGuid(),
            Caption = "Older News",
            IsActive = true,
            ExpressDate = DateTime.UtcNow.AddDays(-1)
        };

        var newerNews = new News
        {
            Id = Guid.NewGuid(),
            Caption = "Newer News",
            IsActive = true,
            ExpressDate = DateTime.UtcNow
        };

        await _context.News.InsertManyAsync([olderNews, newerNews]);

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
        result[0].Caption.Should().Be("Newer News");
        result[1].Caption.Should().Be("Older News");
    }

    [Fact]
    public async Task GetByIdAsync_WhenNewsExists_ShouldReturnNews()
    {
        // Arrange
        var news = new News
        {
            Id = Guid.NewGuid(),
            Caption = "Test News",
            IsActive = true
        };

        await _context.News.InsertOneAsync(news);

        // Act
        var result = await _repository.GetByIdAsync(news.Id);

        // Assert
        result.Should().BeEquivalentTo(news);
    }

    [Fact]
    public async Task GetByIdAsync_WhenNewsNotExists_ShouldReturnNull()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _repository.GetByIdAsync(nonExistentId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_WhenNewsExistsButInactive_ShouldReturnNull()
    {
        // Arrange
        var inactiveNews = new News
        {
            Id = Guid.NewGuid(),
            Caption = "Inactive News",
            IsActive = false
        };

        await _context.News.InsertOneAsync(inactiveNews);

        // Act
        var result = await _repository.GetByIdAsync(inactiveNews.Id);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByUrlAsync_WhenNewsExists_ShouldReturnNews()
    {
        // Arrange
        var news = new News
        {
            Id = Guid.NewGuid(),
            Caption = "Test News",
            Url = "test-news-url",
            IsActive = true
        };

        await _context.News.InsertOneAsync(news);

        // Act
        var result = await _repository.GetByUrlAsync(news.Url);

        // Assert
        result.Should().BeEquivalentTo(news);
    }

    [Fact]
    public async Task GetByUrlAsync_WhenNewsNotExists_ShouldReturnNull()
    {
        // Act
        var result = await _repository.GetByUrlAsync("non-existent-url");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByUrlAsync_WhenNewsExistsButInactive_ShouldReturnNull()
    {
        // Arrange
        var inactiveNews = new News
        {
            Id = Guid.NewGuid(),
            Caption = "Inactive News",
            Url = "inactive-news-url",
            IsActive = false
        };

        await _context.News.InsertOneAsync(inactiveNews);

        // Act
        var result = await _repository.GetByUrlAsync(inactiveNews.Url);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_ShouldInsertNewsAndReturnIt()
    {
        // Arrange
        var news = new News
        {
            Id = Guid.NewGuid(),
            Caption = "New News",
            Category = "Technology",
            Type = "Article",
            Summary = "Test summary",
            Content = "Test content",
            IsActive = true,
            CreateDate = DateTime.UtcNow,
            UpdateDate = DateTime.UtcNow,
            ExpressDate = DateTime.UtcNow,
            Priority = 1
        };

        // Act
        var result = await _repository.CreateAsync(news);

        // Assert
        result.Should().BeEquivalentTo(news);

        // Verify it was actually inserted
        var insertedNews = await _context.News.Find(n => n.Id == news.Id).FirstOrDefaultAsync();
        insertedNews.Should().BeEquivalentTo(news);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateExistingNews()
    {
        // Arrange
        var originalNews = new News
        {
            Id = Guid.NewGuid(),
            Caption = "Original Caption",
            Category = "Technology",
            IsActive = true
        };

        await _context.News.InsertOneAsync(originalNews);

        var updatedNews = new News
        {
            Id = originalNews.Id,
            Caption = "Updated Caption",
            Category = "Science",
            IsActive = true,
            UpdateDate = DateTime.UtcNow
        };

        // Act
        await _repository.UpdateAsync(originalNews.Id, updatedNews);

        // Assert
        var result = await _context.News.Find(n => n.Id == originalNews.Id).FirstOrDefaultAsync();
        result.Should().NotBeNull();
        result!.Caption.Should().Be("Updated Caption");
        result.Category.Should().Be("Science");
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveNews()
    {
        // Arrange
        var news = new News
        {
            Id = Guid.NewGuid(),
            Caption = "News to Delete",
            IsActive = true
        };

        await _context.News.InsertOneAsync(news);

        // Verify it exists
        var existsBefore = await _context.News.Find(n => n.Id == news.Id).AnyAsync();
        existsBefore.Should().BeTrue();

        // Act
        await _repository.DeleteAsync(news.Id);

        // Assert
        var existsAfter = await _context.News.Find(n => n.Id == news.Id).AnyAsync();
        existsAfter.Should().BeFalse();
    }

    [Fact]
    public async Task CreateAsync_WithComplexNewsObject_ShouldPreserveAllProperties()
    {
        // Arrange
        var news = new News
        {
            Id = Guid.NewGuid(),
            Category = "Technology",
            Type = "Breaking News",
            Caption = "Complex News Article",
            Keywords = "tech, innovation, AI",
            SocialTags = "#tech #AI #innovation",
            Summary = "A comprehensive summary of technological advances",
            ImgPath = "/images/tech-news.jpg",
            ImgAlt = "Technology news image",
            Content = "Detailed content about the latest technological innovations and their impact on society.",
            Subjects = new[] { "AI", "Machine Learning", "Innovation" },
            Authors = new[] { "John Doe", "Jane Smith" },
            ExpressDate = DateTime.UtcNow,
            CreateDate = DateTime.UtcNow,
            UpdateDate = DateTime.UtcNow,
            Priority = 5,
            IsActive = true,
            Url = "complex-tech-news-article",
            ViewCount = 0,
            IsSecondPageNews = false
        };

        // Act
        var result = await _repository.CreateAsync(news);

        // Assert
        result.Should().BeEquivalentTo(news);

        // Verify all properties were preserved
        var retrievedNews = await _repository.GetByIdAsync(news.Id);
        retrievedNews.Should().NotBeNull();
        retrievedNews!.Category.Should().Be(news.Category);
        retrievedNews.Type.Should().Be(news.Type);
        retrievedNews.Caption.Should().Be(news.Caption);
        retrievedNews.Keywords.Should().Be(news.Keywords);
        retrievedNews.SocialTags.Should().Be(news.SocialTags);
        retrievedNews.Summary.Should().Be(news.Summary);
        retrievedNews.ImgPath.Should().Be(news.ImgPath);
        retrievedNews.ImgAlt.Should().Be(news.ImgAlt);
        retrievedNews.Content.Should().Be(news.Content);
        retrievedNews.Subjects.Should().BeEquivalentTo(news.Subjects);
        retrievedNews.Authors.Should().BeEquivalentTo(news.Authors);
        retrievedNews.Priority.Should().Be(news.Priority);
        retrievedNews.IsActive.Should().Be(news.IsActive);
        retrievedNews.Url.Should().Be(news.Url);
        retrievedNews.ViewCount.Should().Be(news.ViewCount);
        retrievedNews.IsSecondPageNews.Should().Be(news.IsSecondPageNews);
    }
}