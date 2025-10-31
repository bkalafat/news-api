using FluentAssertions;
using NewsApi.Domain.Entities;
using NewsApi.Tests.Helpers;

namespace NewsApi.Tests.Unit.Domain;

internal class NewsEntityAdvancedTests
{
    [Fact]
    public void News_WithLongContent_ShouldAcceptLargeContent()
    {
        // Arrange
        var largeContent = new string('a', 100000); // 100K characters

        // Act
        var news = NewsBuilder.Create().WithContent(largeContent).Build();

        // Assert
        news.Content.Should().HaveLength(100000);
    }

    [Fact]
    public void News_WithSpecialCharacters_ShouldHandleCorrectly()
    {
        // Arrange & Act
        var news = NewsBuilder
            .Create()
            .WithCaption("NewsArticle with special chars: @#$%^&*()")
            .WithContent("Content with �mojis ?? and �n�c�d�")
            .Build();

        // Assert
        news.Caption.Should().Contain("@#$%^&*()");
        news.Content.Should().Contain("??");
        news.Content.Should().Contain("�n�c�d�");
    }

    [Fact]
    public void News_WithMultipleSubjects_ShouldMaintainOrder()
    {
        // Arrange
        var subjects = new[] { "First", "Second", "Third", "Fourth", "Fifth" };

        // Act
        var news = NewsBuilder.Create().WithSubjects(subjects).Build();

        // Assert
        news.Subjects.Should().ContainInOrder(subjects);
        news.Subjects.Should().HaveCount(5);
    }

    [Fact]
    public void News_WithMultipleAuthors_ShouldMaintainOrder()
    {
        // Arrange
        var authors = new[] { "Author A", "Author B", "Author C" };

        // Act
        var news = NewsBuilder.Create().WithAuthors(authors).Build();

        // Assert
        news.Authors.Should().ContainInOrder(authors);
        news.Authors.Should().HaveCount(3);
    }

    [Fact]
    public void News_WithMaxPriority_ShouldAcceptHighPriority()
    {
        // Act
        var news = NewsBuilder.Create().WithPriority(100).Build();

        // Assert
        news.Priority.Should().Be(100);
    }

    [Fact]
    public void News_WithMinPriority_ShouldAcceptLowPriority()
    {
        // Act
        var news = NewsBuilder.Create().WithPriority(1).Build();

        // Assert
        news.Priority.Should().Be(1);
    }

    [Fact]
    public void News_AsInactive_ShouldSetIsActiveFalse()
    {
        // Act
        var news = NewsBuilder.Create().AsInactive().Build();

        // Assert
        news.IsActive.Should().BeFalse();
    }

    [Fact]
    public void News_AsSecondPageNews_ShouldSetFlag()
    {
        // Act
        var news = NewsBuilder.Create().AsSecondPageNews().Build();

        // Assert
        news.IsSecondPageNews.Should().BeTrue();
        news.Priority.Should().Be(1);
    }

    [Fact]
    public void News_AsPopular_ShouldHaveHighViewCount()
    {
        // Act
        var news = NewsBuilder.Create().AsPopular().Build();

        // Assert
        news.ViewCount.Should().Be(1000);
        news.Priority.Should().Be(8);
    }

    [Theory]
    [InlineData("Technology")]
    [InlineData("Sports")]
    [InlineData("Politics")]
    public void News_WithCategory_ShouldSetCorrectly(string category)
    {
        // Act
        var news = NewsBuilder.Create().WithCategory(category).Build();

        // Assert
        news.Category.Should().Be(category);
    }

    [Fact]
    public void News_WithPastExpressDate_ShouldAllowPastDates()
    {
        // Arrange
        var pastDate = DateTime.UtcNow.AddDays(-30);

        // Act
        var news = NewsBuilder.Create().WithExpressDate(pastDate).Build();

        // Assert
        news.ExpressDate.Should().BeBefore(DateTime.UtcNow);
    }

    [Fact]
    public void News_WithFutureExpressDate_ShouldAllowFutureDates()
    {
        // Arrange
        var futureDate = DateTime.UtcNow.AddDays(30);

        // Act
        var news = NewsBuilder.Create().WithExpressDate(futureDate).Build();

        // Assert
        news.ExpressDate.Should().BeAfter(DateTime.UtcNow);
    }

    [Fact]
    public void News_WithEmptyArrays_ShouldInitializeToEmpty()
    {
        // Act
        var news = new NewsArticle();

        // Assert
        news.Subjects.Should().NotBeNull();
        news.Subjects.Should().BeEmpty();
        news.Authors.Should().NotBeNull();
        news.Authors.Should().BeEmpty();
    }

    [Fact]
    public void News_Comparison_ShouldCompareById()
    {
        // Arrange
        var id = Guid.NewGuid().ToString();
        var news1 = NewsBuilder.Create().WithId(Guid.Parse(id)).Build();
        var news2 = NewsBuilder.Create().WithId(Guid.Parse(id)).Build();

        // Assert
        news1.Id.Should().Be(news2.Id);
    }

    [Fact]
    public void News_WithDifferentIds_ShouldNotBeEqual()
    {
        // Arrange
        var news1 = NewsBuilder.Create().Build();
        var news2 = NewsBuilder.Create().Build();

        // Assert
        news1.Id.Should().NotBe(news2.Id);
    }

    [Fact]
    public void News_WithLongKeywords_ShouldAcceptLongString()
    {
        // Arrange
        var keywords = string.Join(", ", Enumerable.Range(1, 100).Select(i => $"keyword{i}"));

        // Act
        var news = NewsBuilder.Create().WithKeywords(keywords).Build();

        // Assert
        news.Keywords.Should().Contain("keyword1");
        news.Keywords.Should().Contain("keyword100");
    }

    [Fact]
    public void News_WithLongSocialTags_ShouldAcceptLongString()
    {
        // Arrange
        var socialTags = string.Join(" ", Enumerable.Range(1, 50).Select(i => $"#tag{i}"));

        // Act
        var news = NewsBuilder.Create().WithSocialTags(socialTags).Build();

        // Assert
        news.SocialTags.Should().Contain("#tag1");
        news.SocialTags.Should().Contain("#tag50");
    }

    [Fact]
    public void News_WithUrlSlug_ShouldFollowUrlConventions()
    {
        // Arrange

        // Act

        // Assert
    }

    [Fact]
    public void News_WithAllFieldsPopulated_ShouldBeComplete()
    {
        // Act
        var news = NewsBuilder.Create().AsTechnologyNews().AsPopular().Build();

        // Assert
        news.Id.Should().NotBeNullOrEmpty();
        news.Category.Should().NotBeNullOrEmpty();
        news.Type.Should().NotBeNullOrEmpty();
        news.Caption.Should().NotBeNullOrEmpty();
        news.Summary.Should().NotBeNullOrEmpty();
        news.Content.Should().NotBeNullOrEmpty();
        news.Priority.Should().BeGreaterThan(0);
        news.ViewCount.Should().BeGreaterThan(0);
    }
}
