using FluentAssertions;
using NewsApi.Application.DTOs;

namespace NewsApi.Tests.Unit.DTOs;

public class UpdateNewsDtoTests
{
    [Fact]
    public void UpdateNewsDto_WithAllProperties_ShouldAllowPartialUpdates()
    {
        // Arrange & Act
        var dto = new UpdateNewsDto
        {
            Category = "Updated Category",
            Type = "Updated Type",
            Caption = "Updated Caption",
            Keywords = "updated, keywords",
            SocialTags = "#updated",
            Summary = "Updated summary",
            ImgPath = "/images/updated.jpg",
            ImgAlt = "Updated image",
            Content = "Updated content",
            Subjects = new[] { "Updated Subject" },
            Authors = new[] { "Updated Author" },
            ExpressDate = DateTime.UtcNow,
            Priority = 10,
            IsActive = false,
            Url = "updated-article",
            IsSecondPageNews = true,
        };

        // Assert
        dto.Category.Should().Be("Updated Category");
        dto.Type.Should().Be("Updated Type");
        dto.Caption.Should().Be("Updated Caption");
        dto.Priority.Should().Be(10);
        dto.IsActive.Should().BeFalse();
        dto.IsSecondPageNews.Should().BeTrue();
    }

    [Fact]
    public void UpdateNewsDto_WithOnlyCategory_ShouldLeaveOthersNull()
    {
        // Arrange & Act
        var dto = new UpdateNewsDto { Category = "Updated Category" };

        // Assert
        dto.Category.Should().Be("Updated Category");
        dto.Type.Should().BeNull();
        dto.Caption.Should().BeNull();
        dto.Priority.Should().BeNull();
        dto.IsActive.Should().BeNull();
    }

    [Fact]
    public void UpdateNewsDto_WithNullValues_ShouldAllowNulls()
    {
        // Arrange & Act
        var dto = new UpdateNewsDto
        {
            Category = null,
            Type = null,
            Caption = null,
            Priority = null,
            IsActive = null,
        };

        // Assert
        dto.Category.Should().BeNull();
        dto.Type.Should().BeNull();
        dto.Caption.Should().BeNull();
        dto.Priority.Should().BeNull();
        dto.IsActive.Should().BeNull();
    }

    [Fact]
    public void UpdateNewsDto_WithPriorityChange_ShouldUpdatePriority()
    {
        // Arrange & Act
        var dto = new UpdateNewsDto { Priority = 100 };

        // Assert
        dto.Priority.Should().Be(100);
        dto.Priority.HasValue.Should().BeTrue();
    }

    [Fact]
    public void UpdateNewsDto_WithIsActiveChange_ShouldUpdateStatus()
    {
        // Arrange & Act
        var dto = new UpdateNewsDto { IsActive = false };

        // Assert
        dto.IsActive.Should().BeFalse();
        dto.IsActive.HasValue.Should().BeTrue();
    }

    [Fact]
    public void UpdateNewsDto_WithExpressDateChange_ShouldUpdateDate()
    {
        // Arrange
        var newDate = DateTime.UtcNow.AddDays(5);

        // Act
        var dto = new UpdateNewsDto { ExpressDate = newDate };

        // Assert
        dto.ExpressDate.Should().Be(newDate);
        dto.ExpressDate.HasValue.Should().BeTrue();
    }

    [Fact]
    public void UpdateNewsDto_WithArrays_ShouldReplaceArrays()
    {
        // Arrange & Act
        var dto = new UpdateNewsDto
        {
            Subjects = new[] { "New Subject 1", "New Subject 2" },
            Authors = new[] { "New Author 1" },
        };

        // Assert
        dto.Subjects.Should().HaveCount(2);
        dto.Authors.Should().HaveCount(1);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void UpdateNewsDto_WithIsSecondPageNews_ShouldUpdate(bool value)
    {
        // Arrange & Act
        var dto = new UpdateNewsDto { IsSecondPageNews = value };

        // Assert
        dto.IsSecondPageNews.Should().Be(value);
        dto.IsSecondPageNews.HasValue.Should().BeTrue();
    }

    [Fact]
    public void UpdateNewsDto_WhenEmpty_ShouldHaveAllNullValues()
    {
        // Arrange & Act
        var dto = new UpdateNewsDto();

        // Assert
        dto.Category.Should().BeNull();
        dto.Type.Should().BeNull();
        dto.Caption.Should().BeNull();
        dto.Keywords.Should().BeNull();
        dto.SocialTags.Should().BeNull();
        dto.Summary.Should().BeNull();
        dto.ImgPath.Should().BeNull();
        dto.ImgAlt.Should().BeNull();
        dto.Content.Should().BeNull();
        dto.Subjects.Should().BeNull();
        dto.Authors.Should().BeNull();
        dto.ExpressDate.Should().BeNull();
        dto.Priority.Should().BeNull();
        dto.IsActive.Should().BeNull();
        dto.Url.Should().BeNull();
        dto.IsSecondPageNews.Should().BeNull();
    }
}
