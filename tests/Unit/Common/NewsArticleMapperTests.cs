namespace NewsApi.Tests.Unit.Common;

/// <summary>
/// Tests for NewsArticleMapper
/// </summary>
public class NewsArticleMapperTests
{
    [Fact]
    public void ToEntity_ValidDto_MapsAllProperties()
    {
        // Arrange
        var dto = new CreateNewsArticleDto
        {
            Category = "Technology",
            Type = "News",
            Caption = "Test Article",
            Keywords = "test, article",
            SocialTags = "#test #article",
            Summary = "A test article summary",
            ImgPath = "/images/test.jpg",
            ImgAlt = "Test image",
            ImageUrl = "http://example.com/image.jpg",
            ThumbnailUrl = "http://example.com/thumb.jpg",
            Content = "Article content here",
            Subjects = new[] { "Subject1", "Subject2" },
            Authors = new[] { "Author1", "Author2" },
            ExpressDate = new DateTime(2025, 1, 1),
            Priority = 5,
            IsActive = true,
            IsSecondPageNews = false,
        };

        // Act
        var entity = NewsArticleMapper.ToEntity(dto);

        // Assert
        Assert.Equal(dto.Category, entity.Category);
        Assert.Equal(dto.Type, entity.Type);
        Assert.Equal(dto.Caption, entity.Caption);
        Assert.NotEmpty(entity.Slug); // Slug should be generated
        Assert.Equal(dto.Keywords, entity.Keywords);
        Assert.Equal(dto.SocialTags, entity.SocialTags);
        Assert.Equal(dto.Summary, entity.Summary);
        Assert.Equal(dto.ImgPath, entity.ImgPath);
        Assert.Equal(dto.ImgAlt, entity.ImgAlt);
        Assert.Equal(dto.ImageUrl, entity.ImageUrl);
        Assert.Equal(dto.ThumbnailUrl, entity.ThumbnailUrl);
        Assert.Equal(dto.Content, entity.Content);
        Assert.Equal(dto.Subjects, entity.Subjects);
        Assert.Equal(dto.Authors, entity.Authors);
        Assert.Equal(dto.ExpressDate, entity.ExpressDate);
        Assert.Equal(dto.Priority, entity.Priority);
        Assert.Equal(dto.IsActive, entity.IsActive);
        Assert.Equal(dto.IsSecondPageNews, entity.IsSecondPageNews);
    }

    [Fact]
    public void ToEntity_GeneratesSlugFromCaption()
    {
        // Arrange
        var dto = new CreateNewsArticleDto
        {
            Category = "Technology",
            Type = "News",
            Caption = "Breaking News: AI Revolution",
            Summary = "Summary",
            Content = "Content",
            ExpressDate = DateTime.UtcNow,
        };

        // Act
        var entity = NewsArticleMapper.ToEntity(dto);

        // Assert
        Assert.NotEmpty(entity.Slug);
        Assert.Contains("breaking", entity.Slug.ToLowerInvariant());
        Assert.Contains("news", entity.Slug.ToLowerInvariant());
    }

    [Fact]
    public void UpdateFromDto_NullValues_DoesNotUpdateEntity()
    {
        // Arrange
        var entity = new NewsArticle
        {
            Category = "Original Category",
            Type = "Original Type",
            Caption = "Original Caption",
            Priority = 10,
        };

        var dto = new UpdateNewsArticleDto
        {
            // All properties are null/not set
        };

        // Act
        NewsArticleMapper.UpdateFromDto(entity, dto);

        // Assert
        Assert.Equal("Original Category", entity.Category);
        Assert.Equal("Original Type", entity.Type);
        Assert.Equal("Original Caption", entity.Caption);
        Assert.Equal(10, entity.Priority);
    }

    [Fact]
    public void UpdateFromDto_ProvidedValues_UpdatesEntity()
    {
        // Arrange
        var entity = new NewsArticle
        {
            Category = "Old Category",
            Type = "Old Type",
            Caption = "Old Caption",
            Priority = 1,
            IsActive = false,
        };

        var dto = new UpdateNewsArticleDto
        {
            Category = "New Category",
            Caption = "New Caption",
            Priority = 5,
            IsActive = true,
        };

        // Act
        NewsArticleMapper.UpdateFromDto(entity, dto);

        // Assert
        Assert.Equal("New Category", entity.Category);
        Assert.Equal("New Caption", entity.Caption);
        Assert.NotEmpty(entity.Slug); // Slug should be regenerated
        Assert.Equal(5, entity.Priority);
        Assert.True(entity.IsActive);
        Assert.Equal("Old Type", entity.Type); // Should not change
    }

    [Fact]
    public void UpdateFromDto_UpdatesSlugWhenCaptionChanged()
    {
        // Arrange
        var entity = new NewsArticle
        {
            Caption = "Old Caption",
            Slug = "old-caption",
        };

        var dto = new UpdateNewsArticleDto { Caption = "New Amazing Caption" };

        // Act
        NewsArticleMapper.UpdateFromDto(entity, dto);

        // Assert
        Assert.Equal("New Amazing Caption", entity.Caption);
        Assert.NotEqual("old-caption", entity.Slug);
        Assert.Contains("new", entity.Slug.ToLowerInvariant());
    }

    [Fact]
    public void UpdateFromDto_PartialUpdate_OnlyUpdatesProvidedFields()
    {
        // Arrange
        var entity = new NewsArticle
        {
            Category = "Technology",
            Type = "News",
            Caption = "Test",
            Content = "Original Content",
            Priority = 3,
            IsActive = true,
            IsSecondPageNews = false,
        };

        var dto = new UpdateNewsArticleDto
        {
            Content = "Updated Content",
            Priority = 7,
            // Other fields are null/not set
        };

        // Act
        NewsArticleMapper.UpdateFromDto(entity, dto);

        // Assert
        Assert.Equal("Updated Content", entity.Content);
        Assert.Equal(7, entity.Priority);
        // Unchanged fields
        Assert.Equal("Technology", entity.Category);
        Assert.Equal("News", entity.Type);
        Assert.Equal("Test", entity.Caption);
        Assert.True(entity.IsActive);
        Assert.False(entity.IsSecondPageNews);
    }

    [Fact]
    public void UpdateFromDto_UpdatesArrayProperties()
    {
        // Arrange
        var entity = new NewsArticle
        {
            Subjects = new[] { "Old Subject" },
            Authors = new[] { "Old Author" },
        };

        var dto = new UpdateNewsArticleDto
        {
            Subjects = new[] { "New Subject 1", "New Subject 2" },
            Authors = new[] { "New Author" },
        };

        // Act
        NewsArticleMapper.UpdateFromDto(entity, dto);

        // Assert
        Assert.Equal(2, entity.Subjects.Length);
        Assert.Contains("New Subject 1", entity.Subjects);
        Assert.Contains("New Subject 2", entity.Subjects);
        Assert.Single(entity.Authors);
        Assert.Contains("New Author", entity.Authors);
    }

    [Fact]
    public void UpdateFromDto_UpdatesImageUrls()
    {
        // Arrange
        var entity = new NewsArticle
        {
            ImageUrl = "http://old.com/image.jpg",
            ThumbnailUrl = "http://old.com/thumb.jpg",
        };

        var dto = new UpdateNewsArticleDto
        {
            ImageUrl = "http://new.com/image.jpg",
            ThumbnailUrl = "http://new.com/thumb.jpg",
        };

        // Act
        NewsArticleMapper.UpdateFromDto(entity, dto);

        // Assert
        Assert.Equal("http://new.com/image.jpg", entity.ImageUrl);
        Assert.Equal("http://new.com/thumb.jpg", entity.ThumbnailUrl);
    }
}
