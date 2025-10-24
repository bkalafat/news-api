using System.Diagnostics;
using FluentAssertions;
using Moq;
using NewsApi.Application.Services;
using NewsApi.Common;
using NewsApi.Domain.Entities;
using NewsApi.Domain.Interfaces;
using NewsApi.Tests.Helpers;

namespace NewsApi.Tests.Unit.Performance;

public class NewsArticleServicePerformanceTests
{
    private readonly Mock<INewsArticleRepository> _mockRepository;
    private readonly TestMemoryCache _cache;
    private readonly NewsArticleService _NewsArticleService;

    public NewsArticleServicePerformanceTests()
    {
        _mockRepository = new Mock<INewsArticleRepository>();
        _cache = new TestMemoryCache();
        _NewsArticleService = new NewsArticleService(_mockRepository.Object, _cache);
    }

    [Fact]
    public async Task GetAllNewsAsync_WithCache_ShouldBeFasterThanWithoutCache()
    {
        // Arrange
        var newsList = NewsBuilder.Create().BuildMany(1000);
        _mockRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(newsList);

        // First call (no cache)
        var stopwatch1 = Stopwatch.StartNew();
        await _NewsArticleService.GetAllNewsAsync();
        stopwatch1.Stop();
        var firstCallTime = stopwatch1.ElapsedMilliseconds;

        // Second call (with cache)
        var stopwatch2 = Stopwatch.StartNew();
        await _NewsArticleService.GetAllNewsAsync();
        stopwatch2.Stop();
        var secondCallTime = stopwatch2.ElapsedMilliseconds;

        // Assert - second call should be less than or equal to first call (cache is faster or equal)
        // Due to fast execution, we also verify the repository is only called once
        secondCallTime.Should().BeLessThanOrEqualTo(firstCallTime);
        _mockRepository.Verify(x => x.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllNewsAsync_WithLargeDataset_ShouldCompleteWithinTimeout()
    {
        // Arrange
        var largeNewsList = NewsBuilder.Create().BuildMany(10000);
        _mockRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(largeNewsList);

        // Act
        var stopwatch = Stopwatch.StartNew();
        var result = await _NewsArticleService.GetAllNewsAsync();
        stopwatch.Stop();

        // Assert
        result.Should().HaveCount(10000);
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(1000); // Should complete within 1 second
    }

    [Fact]
    public async Task GetNewsByIdAsync_WithCache_ShouldBeFaster()
    {
        // Arrange
        var newsId = Guid.NewGuid().ToString();
        var news = NewsBuilder.Create().WithId(Guid.Parse(newsId)).Build();
        _mockRepository.Setup(x => x.GetByIdAsync(newsId)).ReturnsAsync(news);

        // First call (no cache)
        var stopwatch1 = Stopwatch.StartNew();
        await _NewsArticleService.GetNewsByIdAsync(newsId);
        stopwatch1.Stop();
        var firstCallTime = stopwatch1.ElapsedMilliseconds;

        // Second call (with cache)
        var stopwatch2 = Stopwatch.StartNew();
        await _NewsArticleService.GetNewsByIdAsync(newsId);
        stopwatch2.Stop();
        var secondCallTime = stopwatch2.ElapsedMilliseconds;

        // Assert - second call should be less than or equal to first call (cache is faster or equal)
        // Due to fast execution, we also verify the repository is only called once
        secondCallTime.Should().BeLessThanOrEqualTo(firstCallTime);
        _mockRepository.Verify(x => x.GetByIdAsync(newsId), Times.Once);
    }

    [Fact]
    public async Task CreateNewsAsync_MultipleConcurrentCalls_ShouldHandleCorrectly()
    {
        // Arrange
        _mockRepository.Setup(x => x.CreateAsync(It.IsAny<NewsArticle>())).ReturnsAsync((NewsArticle n) => n);

        var tasks = new List<Task<NewsArticle>>();

        // Act
        for (int i = 0; i < 100; i++)
        {
            var news = NewsBuilder.Create().WithCaption($"NewsArticle {i}").Build();
            tasks.Add(_NewsArticleService.CreateNewsAsync(news));
        }

        var stopwatch = Stopwatch.StartNew();
        var results = await Task.WhenAll(tasks);
        stopwatch.Stop();

        // Assert
        results.Should().HaveCount(100);
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(5000); // Should complete within 5 seconds
        _mockRepository.Verify(x => x.CreateAsync(It.IsAny<NewsArticle>()), Times.Exactly(100));
    }

    [Fact]
    public async Task GetAllNewsAsync_RepeatedCalls_ShouldUseCacheEfficiently()
    {
        // Arrange
        var newsList = NewsBuilder.Create().BuildMany(100);
        _mockRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(newsList);

        // Act - Make 100 calls
        var stopwatch = Stopwatch.StartNew();
        for (int i = 0; i < 100; i++)
        {
            await _NewsArticleService.GetAllNewsAsync();
        }
        stopwatch.Stop();

        // Assert
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(1000); // All 100 calls should complete within 1 second
        _mockRepository.Verify(x => x.GetAllAsync(), Times.Once); // Repository should only be called once
    }

    [Fact]
    public async Task CreateNewsAsync_WithLargeContent_ShouldHandleEfficiently()
    {
        // Arrange
        var largeContent = new string('a', 1000000); // 1MB of content
        var news = NewsBuilder.Create().WithContent(largeContent).Build();
        _mockRepository.Setup(x => x.CreateAsync(It.IsAny<NewsArticle>())).ReturnsAsync(news);

        // Act
        var stopwatch = Stopwatch.StartNew();
        var result = await _NewsArticleService.CreateNewsAsync(news);
        stopwatch.Stop();

        // Assert
        result.Content.Should().HaveLength(1000000);
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(100); // Should complete quickly
    }

    [Fact]
    public async Task UpdateNewsAsync_CacheInvalidation_ShouldBeEfficient()
    {
        // Arrange
        var newsId = Guid.NewGuid().ToString();
        var news = NewsBuilder.Create().WithId(Guid.Parse(newsId)).Build();
        _cache.Set(CacheKeys.NewsList, new List<NewsArticle> { news });
        _cache.Set(newsId, news);

        // Act
        var stopwatch = Stopwatch.StartNew();
        await _NewsArticleService.UpdateNewsAsync(newsId, news);
        stopwatch.Stop();

        // Assert
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(50);
        _cache.TryGetValue(CacheKeys.NewsList, out _).Should().BeFalse();
        _cache.TryGetValue(newsId, out _).Should().BeFalse();
    }

    [Fact]
    public async Task DeleteNewsAsync_CacheInvalidation_ShouldBeEfficient()
    {
        // Arrange
        var newsId = Guid.NewGuid().ToString();
        var news = NewsBuilder.Create().WithId(Guid.Parse(newsId)).Build();
        _cache.Set(CacheKeys.NewsList, new List<NewsArticle> { news });
        _cache.Set(newsId, news);

        // Act
        var stopwatch = Stopwatch.StartNew();
        await _NewsArticleService.DeleteNewsAsync(newsId);
        stopwatch.Stop();

        // Assert
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(50);
        _cache.TryGetValue(CacheKeys.NewsList, out _).Should().BeFalse();
        _cache.TryGetValue(newsId, out _).Should().BeFalse();
    }

    [Fact]
    public async Task GetNewsByIdAsync_WithMultipleConcurrentRequests_ShouldHandleCorrectly()
    {
        // Arrange
        var newsId = Guid.NewGuid().ToString();
        var news = NewsBuilder.Create().WithId(Guid.Parse(newsId)).Build();
        _mockRepository.Setup(x => x.GetByIdAsync(newsId)).ReturnsAsync(news);

        // Act
        var tasks = Enumerable.Range(0, 50).Select(_ => _NewsArticleService.GetNewsByIdAsync(newsId)).ToList();

        var stopwatch = Stopwatch.StartNew();
        var results = await Task.WhenAll(tasks);
        stopwatch.Stop();

        // Assert
        results.Should().HaveCount(50);
        results.Should().OnlyContain(n => n!.Id == newsId);
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(1000);
    }

    [Fact]
    public async Task NewsArticleService_MemoryUsage_ShouldNotGrowExcessively()
    {
        // Arrange
        var newsList = NewsBuilder.Create().BuildMany(1000);
        _mockRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(newsList);

        var initialMemory = GC.GetTotalMemory(true);

        // Act - Make multiple calls
        for (int i = 0; i < 100; i++)
        {
            await _NewsArticleService.GetAllNewsAsync();
        }

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        var finalMemory = GC.GetTotalMemory(true);
        var memoryGrowth = finalMemory - initialMemory;

        // Assert
        memoryGrowth.Should().BeLessThan(10_000_000); // Less than 10MB growth
    }
}
