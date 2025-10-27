using FluentAssertions;
using Moq;
using NewsApi.Application.Services;
using NewsApi.Common;
using NewsApi.Domain.Entities;
using NewsApi.Domain.Interfaces;
using NewsApi.Tests.Helpers;

namespace NewsApi.Tests.Unit.Application;

public class NewsArticleServiceTests
{
    private readonly Mock<INewsArticleRepository> _mockRepository;
    private readonly TestMemoryCache _cache;
    private readonly NewsArticleService _NewsArticleService;

    public NewsArticleServiceTests()
    {
        _mockRepository = new Mock<INewsArticleRepository>();
        _cache = new TestMemoryCache();
        _NewsArticleService = new NewsArticleService(_mockRepository.Object, _cache);
    }

    [Fact]
    public async Task GetAllNewsAsync_WhenCacheHit_ShouldReturnCachedData()
    {
        // Arrange
        var cachedNews = new List<NewsArticle>
        {
            new() { Id = Guid.NewGuid().ToString(), Caption = "Cached NewsArticle 1" },
            new() { Id = Guid.NewGuid().ToString(), Caption = "Cached NewsArticle 2" },
        };

        _cache.Set(CacheKeys.NewsList, cachedNews);

        // Act
        var result = await _NewsArticleService.GetAllNewsAsync();

        // Assert
        result.Should().BeEquivalentTo(cachedNews);
        _mockRepository.Verify(x => x.GetAllAsync(), Times.Never);
    }

    [Fact]
    public async Task GetAllNewsAsync_WhenCacheMiss_ShouldFetchFromRepositoryAndCache()
    {
        // Arrange
        var newsFromRepo = new List<NewsArticle>
        {
            new() { Id = Guid.NewGuid().ToString(), Caption = "Repo NewsArticle 1" },
            new() { Id = Guid.NewGuid().ToString(), Caption = "Repo NewsArticle 2" },
        };

        _mockRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(newsFromRepo);

        // Act
        var result = await _NewsArticleService.GetAllNewsAsync();

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
        var newsId = Guid.NewGuid().ToString();
        var cachedNews = new NewsArticle { Id = newsId, Caption = "Cached NewsArticle" };

        _cache.Set(newsId, cachedNews);

        // Act
        var result = await _NewsArticleService.GetNewsByIdAsync(newsId);

        // Assert
        result.Should().BeEquivalentTo(cachedNews);
        _mockRepository.Verify(x => x.GetByIdAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task GetNewsByIdAsync_WhenCacheMissAndNewsExists_ShouldFetchAndCache()
    {
        // Arrange
        var newsId = Guid.NewGuid().ToString();
        var newsFromRepo = new NewsArticle { Id = newsId, Caption = "Repo NewsArticle" };

        _mockRepository.Setup(x => x.GetByIdAsync(newsId)).ReturnsAsync(newsFromRepo);

        // Act
        var result = await _NewsArticleService.GetNewsByIdAsync(newsId);

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
        var newsId = Guid.NewGuid().ToString();

        _mockRepository.Setup(x => x.GetByIdAsync(newsId)).ReturnsAsync((NewsArticle?)null);

        // Act
        var result = await _NewsArticleService.GetNewsByIdAsync(newsId);

        // Assert
        result.Should().BeNull();
        _mockRepository.Verify(x => x.GetByIdAsync(newsId), Times.Once);

        // Verify nothing was cached
        _cache.TryGetValue(newsId, out _).Should().BeFalse();
    }

    [Fact]
    public async Task CreateNewsAsync_ShouldSetIdAndDatesAndInvalidateCache()
    {
        // Arrange
        var news = new NewsArticle { Caption = "New NewsArticle Article" };
        var beforeCreate = DateTime.UtcNow.AddSeconds(-1);

        // Setup cache with some data to verify it gets invalidated
        _cache.Set(CacheKeys.NewsList, new List<NewsArticle>());

        _mockRepository.Setup(x => x.CreateAsync(It.IsAny<NewsArticle>())).ReturnsAsync((NewsArticle n) => n);

        // Act
        var result = await _NewsArticleService.CreateNewsAsync(news);
        var afterCreate = DateTime.UtcNow.AddSeconds(1);

        // Assert
        result.Id.Should().NotBeEmpty();
        result.CreateDate.Should().BeAfter(beforeCreate).And.BeBefore(afterCreate);
        result.UpdateDate.Should().BeAfter(beforeCreate).And.BeBefore(afterCreate);
        result.Caption.Should().Be("New NewsArticle Article");

        _mockRepository.Verify(
            x =>
                x.CreateAsync(
                    It.Is<NewsArticle>(n =>
                        n.Id != string.Empty && n.CreateDate > beforeCreate && n.UpdateDate > beforeCreate
                    )
                ),
            Times.Once
        );

        // Verify cache was invalidated
        _cache.TryGetValue(CacheKeys.NewsList, out _).Should().BeFalse();
    }

    [Fact]
    public async Task UpdateNewsAsync_ShouldUpdateDateAndInvalidateCache()
    {
        // Arrange
        var newsId = Guid.NewGuid().ToString();
        var news = new NewsArticle { Caption = "Updated NewsArticle Article" };
        var beforeUpdate = DateTime.UtcNow.AddSeconds(-1);

        // Setup cache with some data to verify it gets invalidated
        _cache.Set(CacheKeys.NewsList, new List<NewsArticle>());
        _cache.Set(newsId, new NewsArticle());

        // Act
        await _NewsArticleService.UpdateNewsAsync(newsId, news);
        var afterUpdate = DateTime.UtcNow.AddSeconds(1);

        // Assert
        news.UpdateDate.Should().BeAfter(beforeUpdate).And.BeBefore(afterUpdate);

        _mockRepository.Verify(
            x => x.UpdateAsync(newsId, It.Is<NewsArticle>(n => n.UpdateDate > beforeUpdate)),
            Times.Once
        );

        // Verify cache was invalidated
        _cache.TryGetValue(CacheKeys.NewsList, out _).Should().BeFalse();
        _cache.TryGetValue(newsId, out _).Should().BeFalse();
    }

    [Fact]
    public async Task DeleteNewsAsync_ShouldCallRepositoryAndInvalidateCache()
    {
        // Arrange
        var newsId = Guid.NewGuid().ToString();

        // Setup cache with some data to verify it gets invalidated
        _cache.Set(CacheKeys.NewsList, new List<NewsArticle>());
        _cache.Set(newsId, new NewsArticle());

        // Act
        await _NewsArticleService.DeleteNewsAsync(newsId);

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
        var news = new NewsArticle { Caption = "Test NewsArticle" };
        var expectedException = new InvalidOperationException("Database error");

        _mockRepository.Setup(x => x.CreateAsync(It.IsAny<NewsArticle>())).ThrowsAsync(expectedException);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _NewsArticleService.CreateNewsAsync(news)
        );

        exception.Should().Be(expectedException);
    }

    [Fact]
    public async Task UpdateNewsAsync_WhenRepositoryThrows_ShouldPropagateException()
    {
        // Arrange
        var newsId = Guid.NewGuid().ToString();
        var news = new NewsArticle { Caption = "Updated NewsArticle" };
        var expectedException = new InvalidOperationException("Update failed");

        _mockRepository.Setup(x => x.UpdateAsync(newsId, It.IsAny<NewsArticle>())).ThrowsAsync(expectedException);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _NewsArticleService.UpdateNewsAsync(newsId, news)
        );

        exception.Should().Be(expectedException);
    }

    [Fact]
    public async Task DeleteNewsAsync_WhenRepositoryThrows_ShouldPropagateException()
    {
        // Arrange
        var newsId = Guid.NewGuid().ToString();
        var expectedException = new InvalidOperationException("Delete failed");

        _mockRepository.Setup(x => x.DeleteAsync(newsId)).ThrowsAsync(expectedException);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _NewsArticleService.DeleteNewsAsync(newsId)
        );

        exception.Should().Be(expectedException);
    }
}
