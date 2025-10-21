using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using NewsApi.Application.Services;
using NewsApi.Common;
using NewsApi.Domain.Entities;
using NewsApi.Domain.Interfaces;
using NewsApi.Tests.Helpers;

namespace NewsApi.Tests.Unit.Application;

public class NewsServiceTests
{
    private readonly Mock<INewsRepository> _mockRepository;
    private readonly TestMemoryCache _cache;
    private readonly NewsService _newsService;

    public NewsServiceTests()
    {
        _mockRepository = new Mock<INewsRepository>();
        _cache = new TestMemoryCache();
        _newsService = new NewsService(_mockRepository.Object, _cache);
    }

    [Fact]
    public async Task GetAllNewsAsync_WhenCacheHit_ShouldReturnCachedData()
    {
        // Arrange
        var cachedNews = new List<News>
        {
            new() { Id = Guid.NewGuid(), Caption = "Cached News 1" },
            new() { Id = Guid.NewGuid(), Caption = "Cached News 2" }
        };

        _cache.Set(CacheKeys.NewsList, cachedNews);

        // Act
        var result = await _newsService.GetAllNewsAsync();

        // Assert
        result.Should().BeEquivalentTo(cachedNews);
        _mockRepository.Verify(x => x.GetAllAsync(), Times.Never);
    }

    [Fact]
    public async Task GetAllNewsAsync_WhenCacheMiss_ShouldFetchFromRepositoryAndCache()
    {
        // Arrange
        var newsFromRepo = new List<News>
        {
            new() { Id = Guid.NewGuid(), Caption = "Repo News 1" },
            new() { Id = Guid.NewGuid(), Caption = "Repo News 2" }
        };

        _mockRepository.Setup(x => x.GetAllAsync())
                       .ReturnsAsync(newsFromRepo);

        // Act
        var result = await _newsService.GetAllNewsAsync();

        // Assert
        result.Should().BeEquivalentTo(newsFromRepo);
        _mockRepository.Verify(x => x.GetAllAsync(), Times.Once);
        
        // Verify it was cached
        _cache.TryGetValue(CacheKeys.NewsList, out var cachedValue).Should().BeTrue();
        cachedValue.Should().BeEquivalentTo(newsFromRepo);
    }

    [Fact]
    public async Task GetNewsByIdAsync_WhenCacheHit_ShouldReturnCachedNews()
    {
        // Arrange
        var newsId = Guid.NewGuid();
        var cachedNews = new News { Id = newsId, Caption = "Cached News" };

        _cache.Set(newsId.ToString(), cachedNews);

        // Act
        var result = await _newsService.GetNewsByIdAsync(newsId.ToString());

        // Assert
        result.Should().BeEquivalentTo(cachedNews);
        _mockRepository.Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task GetNewsByIdAsync_WhenCacheMissAndNewsExists_ShouldFetchAndCache()
    {
        // Arrange
        var newsId = Guid.NewGuid();
        var newsFromRepo = new News { Id = newsId, Caption = "Repo News" };

        _mockRepository.Setup(x => x.GetByIdAsync(newsId))
                       .ReturnsAsync(newsFromRepo);

        // Act
        var result = await _newsService.GetNewsByIdAsync(newsId.ToString());

        // Assert
        result.Should().BeEquivalentTo(newsFromRepo);
        _mockRepository.Verify(x => x.GetByIdAsync(newsId), Times.Once);
        
        // Verify it was cached
        _cache.TryGetValue(newsId, out var cachedValue).Should().BeTrue();
        cachedValue.Should().BeEquivalentTo(newsFromRepo);
    }

    [Fact]
    public async Task GetNewsByIdAsync_WhenCacheMissAndNewsNotExists_ShouldReturnNull()
    {
        // Arrange
        var newsId = Guid.NewGuid();

        _mockRepository.Setup(x => x.GetByIdAsync(newsId))
                       .ReturnsAsync((News?)null);

        // Act
        var result = await _newsService.GetNewsByIdAsync(newsId.ToString());

        // Assert
        result.Should().BeNull();
        _mockRepository.Verify(x => x.GetByIdAsync(newsId), Times.Once);
        
        // Verify nothing was cached
        _cache.TryGetValue(newsId, out _).Should().BeFalse();
    }

    [Fact]
    public async Task GetNewsByUrlAsync_ShouldCallRepository()
    {
        // Arrange
        var url = "test-news-url";
        var expectedNews = new News { Id = Guid.NewGuid(), Url = url };

        _mockRepository.Setup(x => x.GetByUrlAsync(url))
                       .ReturnsAsync(expectedNews);

        // Act
        var result = await _newsService.GetNewsByUrlAsync(url);

        // Assert
        result.Should().BeEquivalentTo(expectedNews);
        _mockRepository.Verify(x => x.GetByUrlAsync(url), Times.Once);
    }

    [Fact]
    public async Task CreateNewsAsync_ShouldSetIdAndDatesAndInvalidateCache()
    {
        // Arrange
        var news = new News { Caption = "New News Article" };
        var beforeCreate = DateTime.UtcNow.AddSeconds(-1);

        // Setup cache with some data to verify it gets invalidated
        _cache.Set(CacheKeys.NewsList, new List<News>());

        _mockRepository.Setup(x => x.CreateAsync(It.IsAny<News>()))
                       .ReturnsAsync((News n) => n);

        // Act
        var result = await _newsService.CreateNewsAsync(news);
        var afterCreate = DateTime.UtcNow.AddSeconds(1);

        // Assert
        result.Id.Should().NotBeEmpty();
        result.CreateDate.Should().BeAfter(beforeCreate).And.BeBefore(afterCreate);
        result.UpdateDate.Should().BeAfter(beforeCreate).And.BeBefore(afterCreate);
        result.Caption.Should().Be("New News Article");

        _mockRepository.Verify(x => x.CreateAsync(It.Is<News>(n =>
            n.Id != Guid.Empty &&
            n.CreateDate > beforeCreate &&
            n.UpdateDate > beforeCreate)), Times.Once);

        // Verify cache was invalidated
        _cache.TryGetValue(CacheKeys.NewsList, out _).Should().BeFalse();
    }

    [Fact]
    public async Task UpdateNewsAsync_ShouldUpdateDateAndInvalidateCache()
    {
        // Arrange
        var newsId = Guid.NewGuid();
        var news = new News { Caption = "Updated News Article" };
        var beforeUpdate = DateTime.UtcNow.AddSeconds(-1);

        // Setup cache with some data to verify it gets invalidated
        _cache.Set(CacheKeys.NewsList, new List<News>());
        _cache.Set(newsId, new News());

        // Act
        await _newsService.UpdateNewsAsync(newsId, news);
        var afterUpdate = DateTime.UtcNow.AddSeconds(1);

        // Assert
        news.UpdateDate.Should().BeAfter(beforeUpdate).And.BeBefore(afterUpdate);

        _mockRepository.Verify(x => x.UpdateAsync(newsId, It.Is<News>(n =>
            n.UpdateDate > beforeUpdate)), Times.Once);

        // Verify cache was invalidated
        _cache.TryGetValue(CacheKeys.NewsList, out _).Should().BeFalse();
        _cache.TryGetValue(newsId, out _).Should().BeFalse();
    }

    [Fact]
    public async Task DeleteNewsAsync_ShouldCallRepositoryAndInvalidateCache()
    {
        // Arrange
        var newsId = Guid.NewGuid();

        // Setup cache with some data to verify it gets invalidated
        _cache.Set(CacheKeys.NewsList, new List<News>());
        _cache.Set(newsId, new News());

        // Act
        await _newsService.DeleteNewsAsync(newsId);

        // Assert
        _mockRepository.Verify(x => x.DeleteAsync(newsId), Times.Once);

        // Verify cache was invalidated
        _cache.TryGetValue(CacheKeys.NewsList, out _).Should().BeFalse();
        _cache.TryGetValue(newsId, out _).Should().BeFalse();
    }

    [Fact]
    public async Task CreateNewsAsync_WhenRepositoryThrows_ShouldPropagateException()
    {
        // Arrange
        var news = new News { Caption = "Test News" };
        var expectedException = new InvalidOperationException("Database error");

        _mockRepository.Setup(x => x.CreateAsync(It.IsAny<News>()))
                       .ThrowsAsync(expectedException);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _newsService.CreateNewsAsync(news));

        exception.Should().Be(expectedException);
    }

    [Fact]
    public async Task UpdateNewsAsync_WhenRepositoryThrows_ShouldPropagateException()
    {
        // Arrange
        var newsId = Guid.NewGuid();
        var news = new News { Caption = "Updated News" };
        var expectedException = new InvalidOperationException("Update failed");

        _mockRepository.Setup(x => x.UpdateAsync(newsId, It.IsAny<News>()))
                       .ThrowsAsync(expectedException);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _newsService.UpdateNewsAsync(newsId, news));

        exception.Should().Be(expectedException);
    }

    [Fact]
    public async Task DeleteNewsAsync_WhenRepositoryThrows_ShouldPropagateException()
    {
        // Arrange
        var newsId = Guid.NewGuid();
        var expectedException = new InvalidOperationException("Delete failed");

        _mockRepository.Setup(x => x.DeleteAsync(newsId))
                       .ThrowsAsync(expectedException);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _newsService.DeleteNewsAsync(newsId));

        exception.Should().Be(expectedException);
    }
}