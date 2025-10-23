using FluentAssertions;
using NewsApi.Domain.Entities;

namespace NewsApi.Tests.Unit.Domain;

public class NewsEntityTests
{
    [Fact]
    public void News_WhenCreated_ShouldHaveDefaultValues()
    {
        // Act
        var news = new News();

        // Assert
        news.Id.Should().NotBeEmpty(); // Changed: News entity generates a default ObjectId
        news.Category.Should().BeEmpty();
        news.Type.Should().BeEmpty();
        news.Caption.Should().BeEmpty();
        news.Keywords.Should().BeEmpty();
        news.SocialTags.Should().BeEmpty();
        news.Summary.Should().BeEmpty();
        news.ImgPath.Should().BeEmpty();
        news.ImgAlt.Should().BeEmpty();
        news.Content.Should().BeEmpty();
        news.Url.Should().BeEmpty();
        news.Subjects.Should().BeEmpty();
        news.Authors.Should().BeEmpty();
        news.Priority.Should().Be(0);
        news.ViewCount.Should().Be(0);
        news.IsActive.Should().BeFalse();
        news.IsSecondPageNews.Should().BeFalse();
        news.ExpressDate.Should().Be(DateTime.MinValue);
        news.CreateDate.Should().Be(DateTime.MinValue);
        news.UpdateDate.Should().Be(DateTime.MinValue);
    }

    [Fact]
    public void News_WhenPropertiesSet_ShouldRetainValues()
    {
        // Arrange
        var id = Guid.NewGuid().ToString();
        var category = "Technology";
        var type = "Article";
        var caption = "Test Caption";
        var keywords = "test, keywords";
        var socialTags = "#tech #news";
        var summary = "Test summary";
        var imgPath = "/images/test.jpg";
        var imgAlt = "Test image";
        var content = "Test content";
        var url = "test-article";
        var subjects = new[] { "Tech", "AI" };
        var authors = new[] { "John Doe", "Jane Smith" };
        var priority = 5;
        var viewCount = 100;
        var isActive = true;
        var isSecondPageNews = true;
        var expressDate = DateTime.UtcNow;
        var createDate = DateTime.UtcNow.AddDays(-1);
        var updateDate = DateTime.UtcNow;

        // Act
        var news = new News
        {
            Id = id,
            Category = category,
            Type = type,
            Caption = caption,
            Keywords = keywords,
            SocialTags = socialTags,
            Summary = summary,
            ImgPath = imgPath,
            ImgAlt = imgAlt,
            Content = content,
            Url = url,
            Subjects = subjects,
            Authors = authors,
            Priority = priority,
            ViewCount = viewCount,
            IsActive = isActive,
            IsSecondPageNews = isSecondPageNews,
            ExpressDate = expressDate,
            CreateDate = createDate,
            UpdateDate = updateDate,
        };

        // Assert
        news.Id.Should().Be(id);
        news.Category.Should().Be(category);
        news.Type.Should().Be(type);
        news.Caption.Should().Be(caption);
        news.Keywords.Should().Be(keywords);
        news.SocialTags.Should().Be(socialTags);
        news.Summary.Should().Be(summary);
        news.ImgPath.Should().Be(imgPath);
        news.ImgAlt.Should().Be(imgAlt);
        news.Content.Should().Be(content);
        news.Url.Should().Be(url);
        news.Subjects.Should().BeEquivalentTo(subjects);
        news.Authors.Should().BeEquivalentTo(authors);
        news.Priority.Should().Be(priority);
        news.ViewCount.Should().Be(viewCount);
        news.IsActive.Should().Be(isActive);
        news.IsSecondPageNews.Should().Be(isSecondPageNews);
        news.ExpressDate.Should().Be(expressDate);
        news.CreateDate.Should().Be(createDate);
        news.UpdateDate.Should().Be(updateDate);
    }

    [Theory]
    [InlineData("")]
    [InlineData("Short")]
    [InlineData(
        "This is a very long category name that could potentially exceed normal limits but should still be handled properly by the entity"
    )]
    public void News_Category_ShouldAcceptVariousLengths(string category)
    {
        // Arrange & Act
        var news = new News { Category = category };

        // Assert
        news.Category.Should().Be(category);
    }

    [Theory]
    [InlineData("Breaking News")]
    [InlineData("Feature")]
    [InlineData("Opinion")]
    [InlineData("Analysis")]
    public void News_Type_ShouldAcceptValidTypes(string type)
    {
        // Arrange & Act
        var news = new News { Type = type };

        // Assert
        news.Type.Should().Be(type);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(50)]
    [InlineData(100)]
    [InlineData(0)]
    [InlineData(-1)]
    public void News_Priority_ShouldAcceptAnyIntegerValue(int priority)
    {
        // Arrange & Act
        var news = new News { Priority = priority };

        // Assert
        news.Priority.Should().Be(priority);
    }

    [Fact]
    public void News_Subjects_ShouldBeEmptyArrayByDefault()
    {
        // Act
        var news = new News();

        // Assert
        news.Subjects.Should().NotBeNull();
        news.Subjects.Should().BeEmpty();
    }

    [Fact]
    public void News_Authors_ShouldBeEmptyArrayByDefault()
    {
        // Act
        var news = new News();

        // Assert
        news.Authors.Should().NotBeNull();
        news.Authors.Should().BeEmpty();
    }

    [Fact]
    public void News_ViewCount_ShouldBeZeroByDefault()
    {
        // Act
        var news = new News();

        // Assert
        news.ViewCount.Should().Be(0);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(1000)]
    [InlineData(int.MaxValue)]
    public void News_ViewCount_ShouldAcceptPositiveValues(int viewCount)
    {
        // Arrange & Act
        var news = new News { ViewCount = viewCount };

        // Assert
        news.ViewCount.Should().Be(viewCount);
    }

    [Fact]
    public void News_Url_ShouldSupportSeoFriendlyFormats()
    {
        // Arrange
        var seoUrl = "breaking-news-about-technology-trends-2024";

        // Act
        var news = new News { Url = seoUrl };

        // Assert
        news.Url.Should().Be(seoUrl);
    }
}
