using FluentAssertions;
using NewsApi.Application.DTOs;
using NewsApi.Domain.Entities;

namespace NewsApi.Tests.Unit.DTOs;

internal class CreateNewsArticleDtoTests
{
    [Fact]
    public void CreateNewsArticleDto_WithAllProperties_ShouldMapToNewsEntity()
    {
        // Arrange
        var dto = new CreateNewsArticleDto
        {
            Category = "Technology",
            Type = "Article",
            Caption = "Test Caption",
            Keywords = "test, keywords",
            SocialTags = "#test #tech",
            Summary = "Test summary",
            ImgPath = "/images/test.jpg",
            ImgAlt = "Test image",
            Content = "Test content",
            Subjects = new[] { "Tech", "AI" },
            Authors = new[] { "John Doe" },
            ExpressDate = DateTime.UtcNow,
            Priority = 5,
            IsActive = true,
            IsSecondPageNews = false,
        };

        // Act
        var news = new NewsArticle
        {
            Category = dto.Category,
            Type = dto.Type,
            Caption = dto.Caption,
            Keywords = dto.Keywords,
            SocialTags = dto.SocialTags,
            Summary = dto.Summary,
            ImgPath = dto.ImgPath,
            ImgAlt = dto.ImgAlt,
            Content = dto.Content,
            Subjects = dto.Subjects,
            Authors = dto.Authors,
            ExpressDate = dto.ExpressDate,
            Priority = dto.Priority,
            IsActive = dto.IsActive,
            IsSecondPageNews = dto.IsSecondPageNews,
        };

        // Assert
        news.Category.Should().Be(dto.Category);
        news.Type.Should().Be(dto.Type);
        news.Caption.Should().Be(dto.Caption);
        news.Keywords.Should().Be(dto.Keywords);
        news.SocialTags.Should().Be(dto.SocialTags);
        news.Summary.Should().Be(dto.Summary);
        news.ImgPath.Should().Be(dto.ImgPath);
        news.ImgAlt.Should().Be(dto.ImgAlt);
        news.Content.Should().Be(dto.Content);
        news.Subjects.Should().BeEquivalentTo(dto.Subjects);
        news.Authors.Should().BeEquivalentTo(dto.Authors);
        news.ExpressDate.Should().Be(dto.ExpressDate);
        news.Priority.Should().Be(dto.Priority);
        news.IsActive.Should().Be(dto.IsActive);
        news.IsSecondPageNews.Should().Be(dto.IsSecondPageNews);
    }

    [Fact]
    public void CreateNewsArticleDto_WithMinimalProperties_ShouldBeValid()
    {
        // Arrange & Act
        var dto = new CreateNewsArticleDto
        {
            Category = "Tech",
            Type = "Article",
            Caption = "Caption",
            Summary = "Summary",
            Content = "Content",
            ExpressDate = DateTime.UtcNow,
            Priority = 1,
        };

        // Assert
        dto.Should().NotBeNull();
        dto.Category.Should().NotBeEmpty();
        dto.Type.Should().NotBeEmpty();
        dto.Caption.Should().NotBeEmpty();
        dto.Summary.Should().NotBeEmpty();
        dto.Content.Should().NotBeEmpty();
    }

    [Fact]
    public void CreateNewsArticleDto_WithArrays_ShouldHandleMultipleValues()
    {
        // Arrange & Act
        var dto = new CreateNewsArticleDto
        {
            Category = "Technology",
            Type = "Article",
            Caption = "Test",
            Summary = "Test summary",
            Content = "Test content",
            Subjects = new[] { "AI", "ML", "Cloud", "Security" },
            Authors = new[] { "Author 1", "Author 2", "Author 3" },
            ExpressDate = DateTime.UtcNow,
            Priority = 1,
        };

        // Assert
        dto.Subjects.Should().HaveCount(4);
        dto.Authors.Should().HaveCount(3);
    }

    [Fact]
    public void CreateNewsArticleDto_WithNullableProperties_ShouldAllowNulls()
    {
        // Arrange & Act
        var dto = new CreateNewsArticleDto
        {
            Category = "Tech",
            Type = "Article",
            Caption = "Caption",
            Summary = "Summary",
            Content = "Content",
            ExpressDate = DateTime.UtcNow,
            Priority = 1,
            Keywords = string.Empty,
            SocialTags = string.Empty,
            ImgPath = string.Empty,
            ImgAlt = string.Empty,
            Subjects = Array.Empty<string>(),
            Authors = Array.Empty<string>(),
        };

        // Assert
        dto.Should().NotBeNull();
        dto.Keywords.Should().BeEmpty();
        dto.SocialTags.Should().BeEmpty();
        dto.ImgPath.Should().BeEmpty();
        dto.ImgAlt.Should().BeEmpty();
    }

    [Fact]
    public void CreateNewsArticleDto_WithFutureExpressDate_ShouldBeValid()
    {
        // Arrange & Act
        var futureDate = DateTime.UtcNow.AddDays(7);
        var dto = new CreateNewsArticleDto
        {
            Category = "Tech",
            Type = "Article",
            Caption = "Caption",
            Summary = "Summary",
            Content = "Content",
            ExpressDate = futureDate,
            Priority = 1,
        };

        // Assert
        dto.ExpressDate.Should().BeAfter(DateTime.UtcNow);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(50)]
    [InlineData(100)]
    public void CreateNewsArticleDto_WithValidPriority_ShouldBeAccepted(int priority)
    {
        // Arrange & Act
        var dto = new CreateNewsArticleDto
        {
            Category = "Tech",
            Type = "Article",
            Caption = "Caption",
            Summary = "Summary",
            Content = "Content",
            ExpressDate = DateTime.UtcNow,
            Priority = priority,
        };

        // Assert
        dto.Priority.Should().Be(priority);
    }
}
