using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NewsApi.Application.Services;
using NewsApi.Domain.Entities;
using NewsApi.Domain.Interfaces;
using NewsApi.Presentation.Controllers;
using NewsApi.Tests.Helpers;

namespace NewsApi.Tests.Unit.Presentation;

public class NewsControllerTests
{
    private readonly Mock<INewsService> _mockNewsService;
    private readonly Mock<IImageStorageService> _mockImageStorageService;
    private readonly NewsController _controller;

    public NewsControllerTests()
    {
        _mockNewsService = new Mock<INewsService>();
        _mockImageStorageService = new Mock<IImageStorageService>();
        _controller = new NewsController(_mockNewsService.Object, _mockImageStorageService.Object);
    }

    #region GetAllNews Tests

    [Fact]
    public async Task GetAllNews_WithoutFilters_ShouldReturnAllNews()
    {
        // Arrange
        var newsList = new List<News>
        {
            NewsBuilder.Create().WithCategory("Technology").Build(),
            NewsBuilder.Create().WithCategory("Sports").Build(),
            NewsBuilder.Create().WithCategory("Politics").Build(),
        };

        _mockNewsService.Setup(x => x.GetAllNewsAsync()).ReturnsAsync(newsList);

        // Act
        var result = await _controller.GetAllNews();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedNews = okResult.Value.Should().BeAssignableTo<List<News>>().Subject;
        returnedNews.Should().HaveCount(3);
        _mockNewsService.Verify(x => x.GetAllNewsAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllNews_WithCategoryFilter_ShouldReturnFilteredNews()
    {
        // Arrange
        var newsList = new List<News>
        {
            NewsBuilder.Create().WithCategory("Technology").Build(),
            NewsBuilder.Create().WithCategory("Sports").Build(),
            NewsBuilder.Create().WithCategory("Technology").Build(),
        };

        _mockNewsService.Setup(x => x.GetAllNewsAsync()).ReturnsAsync(newsList);

        // Act
        var result = await _controller.GetAllNews(category: "Technology");

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedNews = okResult.Value.Should().BeAssignableTo<List<News>>().Subject;
        returnedNews.Should().HaveCount(2);
        returnedNews.Should().OnlyContain(n => n.Category == "Technology");
    }

    [Fact]
    public async Task GetAllNews_WithTypeFilter_ShouldReturnFilteredNews()
    {
        // Arrange
        var newsList = new List<News>
        {
            NewsBuilder.Create().WithType("Breaking").Build(),
            NewsBuilder.Create().WithType("Article").Build(),
            NewsBuilder.Create().WithType("Breaking").Build(),
        };

        _mockNewsService.Setup(x => x.GetAllNewsAsync()).ReturnsAsync(newsList);

        // Act
        var result = await _controller.GetAllNews(type: "Breaking");

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedNews = okResult.Value.Should().BeAssignableTo<List<News>>().Subject;
        returnedNews.Should().HaveCount(2);
        returnedNews.Should().OnlyContain(n => n.Type == "Breaking");
    }

    [Fact]
    public async Task GetAllNews_WithBothFilters_ShouldReturnFilteredNews()
    {
        // Arrange
        var newsList = new List<News>
        {
            NewsBuilder.Create().WithCategory("Technology").WithType("Breaking").Build(),
            NewsBuilder.Create().WithCategory("Sports").WithType("Breaking").Build(),
            NewsBuilder.Create().WithCategory("Technology").WithType("Article").Build(),
        };

        _mockNewsService.Setup(x => x.GetAllNewsAsync()).ReturnsAsync(newsList);

        // Act
        var result = await _controller.GetAllNews(category: "Technology", type: "Breaking");

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedNews = okResult.Value.Should().BeAssignableTo<List<News>>().Subject;
        returnedNews.Should().HaveCount(1);
        returnedNews.First().Category.Should().Be("Technology");
        returnedNews.First().Type.Should().Be("Breaking");
    }

    [Fact]
    public async Task GetAllNews_WhenServiceThrows_ShouldReturn500()
    {
        // Arrange
        _mockNewsService.Setup(x => x.GetAllNewsAsync()).ThrowsAsync(new Exception("Database error"));

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
        _mockNewsService.Setup(x => x.GetAllNewsAsync()).ReturnsAsync(new List<News>());

        // Act
        var result = await _controller.GetAllNews();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedNews = okResult.Value.Should().BeAssignableTo<List<News>>().Subject;
        returnedNews.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllNews_WithCaseInsensitiveCategory_ShouldReturnFilteredNews()
    {
        // Arrange
        var newsList = new List<News>
        {
            NewsBuilder.Create().WithCategory("Technology").Build(),
            NewsBuilder.Create().WithCategory("TECHNOLOGY").Build(),
            NewsBuilder.Create().WithCategory("Sports").Build(),
        };

        _mockNewsService.Setup(x => x.GetAllNewsAsync()).ReturnsAsync(newsList);

        // Act
        var result = await _controller.GetAllNews(category: "technology");

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedNews = okResult.Value.Should().BeAssignableTo<List<News>>().Subject;
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

        _mockNewsService.Setup(x => x.GetNewsByIdAsync(newsId)).ReturnsAsync(news);

        // Act
        var result = await _controller.GetNewsById(newsId);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedNews = okResult.Value.Should().BeOfType<News>().Subject;
        returnedNews.Id.Should().Be(newsId);
    }

    [Fact]
    public async Task GetNewsById_WithInvalidId_ShouldReturn404()
    {
        // Arrange
        var newsId = Guid.NewGuid().ToString();
        _mockNewsService.Setup(x => x.GetNewsByIdAsync(newsId)).ReturnsAsync((News?)null);

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
        _mockNewsService.Setup(x => x.GetNewsByIdAsync(newsId)).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetNewsById(newsId);

        // Assert
        var statusResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
        statusResult.StatusCode.Should().Be(500);
    }

    #endregion

    #region GetNewsByUrl Tests

    [Fact]
    public async Task GetNewsByUrl_WithValidUrl_ShouldReturnNews()
    {
        // Arrange
        var url = "technology-news-2024";
        var news = NewsBuilder.Create().WithUrl(url).Build();

        _mockNewsService.Setup(x => x.GetNewsByUrlAsync(url)).ReturnsAsync(news);

        // Act
        var result = await _controller.GetNewsByUrl(url);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedNews = okResult.Value.Should().BeOfType<News>().Subject;
        returnedNews.Url.Should().Be(url);
    }

    [Fact]
    public async Task GetNewsByUrl_WithInvalidUrl_ShouldReturn404()
    {
        // Arrange
        var url = "non-existent-url";
        _mockNewsService.Setup(x => x.GetNewsByUrlAsync(url)).ReturnsAsync((News?)null);

        // Act
        var result = await _controller.GetNewsByUrl(url);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task GetNewsByUrl_WithEmptyUrl_ShouldReturn400()
    {
        // Act
        var result = await _controller.GetNewsByUrl(string.Empty);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task GetNewsByUrl_WhenServiceThrows_ShouldReturn500()
    {
        // Arrange
        var url = "test-url";
        _mockNewsService.Setup(x => x.GetNewsByUrlAsync(url)).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetNewsByUrl(url);

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
        var createDto = CreateNewsDtoBuilder.Create().AsValidTechnologyNews().Build();
        var createdNews = NewsBuilder.Create().WithCaption(createDto.Caption).Build();

        _mockNewsService.Setup(x => x.CreateNewsAsync(It.IsAny<News>())).ReturnsAsync(createdNews);

        // Act
        var result = await _controller.CreateNews(createDto);

        // Assert
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.ActionName.Should().Be(nameof(NewsController.GetNewsById));
        var returnedNews = createdResult.Value.Should().BeOfType<News>().Subject;
        returnedNews.Caption.Should().Be(createDto.Caption);
    }

    [Fact]
    public async Task CreateNews_WhenServiceThrows_ShouldReturn500()
    {
        // Arrange
        var createDto = CreateNewsDtoBuilder.Create().AsValidTechnologyNews().Build();
        _mockNewsService.Setup(x => x.CreateNewsAsync(It.IsAny<News>())).ThrowsAsync(new Exception("Database error"));

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
        var updateDto = UpdateNewsDtoBuilder.Create().Build();

        _mockNewsService.Setup(x => x.GetNewsByIdAsync(newsId)).ReturnsAsync(existingNews);
        _mockNewsService.Setup(x => x.UpdateNewsAsync(newsId, It.IsAny<News>())).Returns(Task.CompletedTask);

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
        var updateDto = UpdateNewsDtoBuilder.Create().Build();

        _mockNewsService.Setup(x => x.GetNewsByIdAsync(newsId)).ReturnsAsync((News?)null);

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
        var updateDto = UpdateNewsDtoBuilder.Create().Build();

        _mockNewsService.Setup(x => x.GetNewsByIdAsync(newsId)).ReturnsAsync(existingNews);
        _mockNewsService
            .Setup(x => x.UpdateNewsAsync(newsId, It.IsAny<News>()))
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

        _mockNewsService.Setup(x => x.GetNewsByIdAsync(newsId)).ReturnsAsync(existingNews);
        _mockNewsService.Setup(x => x.DeleteNewsAsync(newsId)).Returns(Task.CompletedTask);

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

        _mockNewsService.Setup(x => x.GetNewsByIdAsync(newsId)).ReturnsAsync((News?)null);

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

        _mockNewsService.Setup(x => x.GetNewsByIdAsync(newsId)).ReturnsAsync(existingNews);
        _mockNewsService.Setup(x => x.DeleteNewsAsync(newsId)).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.DeleteNews(newsId);

        // Assert
        var statusResult = result.Should().BeOfType<ObjectResult>().Subject;
        statusResult.StatusCode.Should().Be(500);
    }

    #endregion
}
