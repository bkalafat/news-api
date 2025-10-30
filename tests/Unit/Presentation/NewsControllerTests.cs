using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NewsApi.Application.Services;
using NewsApi.Domain.Entities;
using NewsApi.Domain.Interfaces;
using NewsApi.Presentation.Controllers;
using NewsApi.Tests.Helpers;

namespace NewsApi.Tests.Unit.Presentation;

public class NewsArticleControllerTests
{
    private readonly Mock<INewsArticleService> _mockNewsArticleService;
    private readonly Mock<IImageStorageService> _mockImageStorageService;
    private readonly NewsArticleController _controller;

    public NewsArticleControllerTests()
    {
        _mockNewsArticleService = new Mock<INewsArticleService>();
        _mockImageStorageService = new Mock<IImageStorageService>();
        _controller = new NewsArticleController(_mockNewsArticleService.Object, _mockImageStorageService.Object);
    }

    #region GetAllNews Tests

    [Fact]
    public async Task GetAllNews_WithoutFilters_ShouldReturnAllNews()
    {
        // Arrange
        var newsList = new List<NewsArticle>
        {
            NewsBuilder.Create().WithCategory("Technology").Build(),
            NewsBuilder.Create().WithCategory("Sports").Build(),
            NewsBuilder.Create().WithCategory("Politics").Build(),
        };

        _mockNewsArticleService.Setup(service => service.GetAllNewsAsync()).ReturnsAsync(newsList);

        // Act
        var result = await _controller.GetAllNews();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedNews = okResult.Value.Should().BeAssignableTo<List<NewsArticle>>().Subject;
        returnedNews.Should().HaveCount(3);
        _mockNewsArticleService.Verify(service => service.GetAllNewsAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllNews_WithCategoryFilter_ShouldReturnFilteredNews()
    {
        // Arrange
        var newsList = new List<NewsArticle>
        {
            NewsBuilder.Create().WithCategory("Technology").Build(),
            NewsBuilder.Create().WithCategory("Sports").Build(),
            NewsBuilder.Create().WithCategory("Technology").Build(),
        };

        _mockNewsArticleService.Setup(service => service.GetAllNewsAsync()).ReturnsAsync(newsList);

        // Act
        var result = await _controller.GetAllNews(category: "Technology");

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedNews = okResult.Value.Should().BeAssignableTo<List<NewsArticle>>().Subject;
        returnedNews.Should().HaveCount(2);
        returnedNews.Should().OnlyContain(article => article.Category == "Technology");
    }

    [Fact]
    public async Task GetAllNews_WithTypeFilter_ShouldReturnFilteredNews()
    {
        // Arrange
        var newsList = new List<NewsArticle>
        {
            NewsBuilder.Create().WithType("Breaking").Build(),
            NewsBuilder.Create().WithType("Article").Build(),
            NewsBuilder.Create().WithType("Breaking").Build(),
        };

        _mockNewsArticleService.Setup(service => service.GetAllNewsAsync()).ReturnsAsync(newsList);

        // Act
        var result = await _controller.GetAllNews(type: "Breaking");

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedNews = okResult.Value.Should().BeAssignableTo<List<NewsArticle>>().Subject;
        returnedNews.Should().HaveCount(2);
        returnedNews.Should().OnlyContain(article => article.Type == "Breaking");
    }

    [Fact]
    public async Task GetAllNews_WithBothFilters_ShouldReturnFilteredNews()
    {
        // Arrange
        var newsList = new List<NewsArticle>
        {
            NewsBuilder.Create().WithCategory("Technology").WithType("Breaking").Build(),
            NewsBuilder.Create().WithCategory("Sports").WithType("Breaking").Build(),
            NewsBuilder.Create().WithCategory("Technology").WithType("Article").Build(),
        };

        _mockNewsArticleService.Setup(service => service.GetAllNewsAsync()).ReturnsAsync(newsList);

        // Act
        var result = await _controller.GetAllNews(category: "Technology", type: "Breaking");

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedNews = okResult.Value.Should().BeAssignableTo<List<NewsArticle>>().Subject;
        returnedNews.Should().HaveCount(1);
        returnedNews.First().Category.Should().Be("Technology");
        returnedNews.First().Type.Should().Be("Breaking");
    }

    [Fact]
    public async Task GetAllNews_WhenServiceThrows_ShouldReturn500()
    {
        // Arrange
        _mockNewsArticleService.Setup(service => service.GetAllNewsAsync()).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetAllNews();

        // Assert
        var statusResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
        statusResult.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task GetAllNews_WithEmptyList_ShouldReturnEmptyList()
    {
        // Arrange
        _mockNewsArticleService.Setup(service => service.GetAllNewsAsync()).ReturnsAsync(new List<NewsArticle>());

        // Act
        var result = await _controller.GetAllNews();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedNews = okResult.Value.Should().BeAssignableTo<List<NewsArticle>>().Subject;
        returnedNews.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllNews_WithCaseInsensitiveCategory_ShouldReturnFilteredNews()
    {
        // Arrange
        var newsList = new List<NewsArticle>
        {
            NewsBuilder.Create().WithCategory("Technology").Build(),
            NewsBuilder.Create().WithCategory("TECHNOLOGY").Build(),
            NewsBuilder.Create().WithCategory("Sports").Build(),
        };

        _mockNewsArticleService.Setup(service => service.GetAllNewsAsync()).ReturnsAsync(newsList);

        // Act
        var result = await _controller.GetAllNews(category: "technology");

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedNews = okResult.Value.Should().BeAssignableTo<List<NewsArticle>>().Subject;
        returnedNews.Should().HaveCount(2);
    }

    #endregion

    #region GetNewsById Tests

    [Fact]
    public async Task GetNewsById_WithValidId_ShouldReturnNews()
    {
        // Arrange
        var newsId = Guid.NewGuid().ToString();
        var news = NewsBuilder.Create().WithId(Guid.Parse(newsId)).Build();

        _mockNewsArticleService.Setup(service => service.GetNewsByIdAsync(newsId)).ReturnsAsync(news);

        // Act
        var result = await _controller.GetNewsById(newsId);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedNews = okResult.Value.Should().BeOfType<NewsArticle>().Subject;
        returnedNews.Id.Should().Be(newsId);
    }

    [Fact]
    public async Task GetNewsById_WithInvalidId_ShouldReturn404()
    {
        // Arrange
        var newsId = Guid.NewGuid().ToString();
        _mockNewsArticleService.Setup(service => service.GetNewsByIdAsync(newsId)).ReturnsAsync((NewsArticle?)null);

        // Act
        var result = await _controller.GetNewsById(newsId);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task GetNewsById_WhenServiceThrows_ShouldReturn500()
    {
        // Arrange
        var newsId = Guid.NewGuid().ToString();
        _mockNewsArticleService.Setup(service => service.GetNewsByIdAsync(newsId)).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetNewsById(newsId);

        // Assert
        var statusResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
        statusResult.StatusCode.Should().Be(500);
    }

    #endregion

    #region CreateNews Tests

    [Fact]
    public async Task CreateNews_WithValidData_ShouldReturnCreatedNews()
    {
        // Arrange
        var createDto = CreateNewsArticleDtoBuilder.Create().AsValidTechnologyNews().Build();
        var createdNews = NewsBuilder.Create().WithCaption(createDto.Caption).Build();

        _mockNewsArticleService.Setup(service => service.CreateNewsAsync(It.IsAny<NewsArticle>())).ReturnsAsync(createdNews);

        // Act
        var result = await _controller.CreateNews(createDto);

        // Assert
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.ActionName.Should().Be(nameof(NewsArticleController.GetNewsById));
        var returnedNews = createdResult.Value.Should().BeOfType<NewsArticle>().Subject;
        returnedNews.Caption.Should().Be(createDto.Caption);
    }

    [Fact]
    public async Task CreateNews_WhenServiceThrows_ShouldReturn500()
    {
        // Arrange
        var createDto = CreateNewsArticleDtoBuilder.Create().AsValidTechnologyNews().Build();
        _mockNewsArticleService
            .Setup(service => service.CreateNewsAsync(It.IsAny<NewsArticle>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.CreateNews(createDto);

        // Assert
        var statusResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
        statusResult.StatusCode.Should().Be(500);
    }

    #endregion

    #region UpdateNews Tests

    [Fact]
    public async Task UpdateNews_WithValidData_ShouldReturnNoContent()
    {
        // Arrange
        var newsId = Guid.NewGuid().ToString();
        var existingNews = NewsBuilder.Create().WithId(Guid.Parse(newsId)).Build();
        var updateDto = UpdateNewsArticleDtoBuilder.Create().Build();

        _mockNewsArticleService.Setup(service => service.GetNewsByIdAsync(newsId)).ReturnsAsync(existingNews);
        _mockNewsArticleService
            .Setup(service => service.UpdateNewsAsync(newsId, It.IsAny<NewsArticle>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.UpdateNews(newsId, updateDto);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task UpdateNews_WithInvalidId_ShouldReturn404()
    {
        // Arrange
        var newsId = Guid.NewGuid().ToString();
        var updateDto = UpdateNewsArticleDtoBuilder.Create().Build();

        _mockNewsArticleService.Setup(service => service.GetNewsByIdAsync(newsId)).ReturnsAsync((NewsArticle?)null);

        // Act
        var result = await _controller.UpdateNews(newsId, updateDto);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task UpdateNews_WhenServiceThrows_ShouldReturn500()
    {
        // Arrange
        var newsId = Guid.NewGuid().ToString();
        var existingNews = NewsBuilder.Create().WithId(Guid.Parse(newsId)).Build();
        var updateDto = UpdateNewsArticleDtoBuilder.Create().Build();

        _mockNewsArticleService.Setup(service => service.GetNewsByIdAsync(newsId)).ReturnsAsync(existingNews);
        _mockNewsArticleService
            .Setup(service => service.UpdateNewsAsync(newsId, It.IsAny<NewsArticle>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.UpdateNews(newsId, updateDto);

        // Assert
        var statusResult = result.Should().BeOfType<ObjectResult>().Subject;
        statusResult.StatusCode.Should().Be(500);
    }

    #endregion

    #region DeleteNews Tests

    [Fact]
    public async Task DeleteNews_WithValidId_ShouldReturnNoContent()
    {
        // Arrange
        var newsId = Guid.NewGuid().ToString();
        var existingNews = NewsBuilder.Create().WithId(Guid.Parse(newsId)).Build();

        _mockNewsArticleService.Setup(service => service.GetNewsByIdAsync(newsId)).ReturnsAsync(existingNews);
        _mockNewsArticleService.Setup(service => service.DeleteNewsAsync(newsId)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteNews(newsId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task DeleteNews_WithInvalidId_ShouldReturn404()
    {
        // Arrange
        var newsId = Guid.NewGuid().ToString();

        _mockNewsArticleService.Setup(service => service.GetNewsByIdAsync(newsId)).ReturnsAsync((NewsArticle?)null);

        // Act
        var result = await _controller.DeleteNews(newsId);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task DeleteNews_WhenServiceThrows_ShouldReturn500()
    {
        // Arrange
        var newsId = Guid.NewGuid().ToString();
        var existingNews = NewsBuilder.Create().WithId(Guid.Parse(newsId)).Build();

        _mockNewsArticleService.Setup(service => service.GetNewsByIdAsync(newsId)).ReturnsAsync(existingNews);
        _mockNewsArticleService.Setup(service => service.DeleteNewsAsync(newsId)).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.DeleteNews(newsId);

        // Assert
        var statusResult = result.Should().BeOfType<ObjectResult>().Subject;
        statusResult.StatusCode.Should().Be(500);
    }

    #endregion
}
