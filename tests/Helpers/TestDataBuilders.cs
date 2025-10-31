namespace NewsApi.Tests.Helpers;

/// <summary>
/// Builder class for creating test NewsArticle entities with fluent API
/// </summary>
internal class NewsBuilder
{
    private readonly NewsArticle _news;

    public NewsBuilder()
    {
        _news = new NewsArticle
        {
            Id = Guid.NewGuid().ToString(),
            Category = "Technology",
            Type = "Article",
            Caption = "Default Test Caption",
            Keywords = "test, default",
            SocialTags = "#test #default",
            Summary = "Default test summary",
            ImgPath = "/images/default-test.jpg",
            ImgAlt = "Default test image",
            Content = "Default test content for news article",
            Subjects = ["Testing"],
            Authors = ["Test Author"],
            ExpressDate = DateTime.UtcNow,
            CreateDate = DateTime.UtcNow,
            UpdateDate = DateTime.UtcNow,
            Priority = 1,
            IsActive = true,
            ViewCount = 0,
            IsSecondPageNews = false,
        };
    }

    public static NewsBuilder Create() => new();

    public NewsBuilder WithId(Guid id)
    {
        _news.Id = id.ToString();
        return this;
    }

    public NewsBuilder WithCategory(string category)
    {
        _news.Category = category;
        return this;
    }

    public NewsBuilder WithType(string type)
    {
        _news.Type = type;
        return this;
    }

    public NewsBuilder WithCaption(string caption)
    {
        _news.Caption = caption;
        return this;
    }

    public NewsBuilder WithKeywords(string keywords)
    {
        _news.Keywords = keywords;
        return this;
    }

    public NewsBuilder WithSocialTags(string socialTags)
    {
        _news.SocialTags = socialTags;
        return this;
    }

    public NewsBuilder WithSummary(string summary)
    {
        _news.Summary = summary;
        return this;
    }

    public NewsBuilder WithImagePath(string imgPath)
    {
        _news.ImgPath = imgPath;
        return this;
    }

    public NewsBuilder WithImageAlt(string imgAlt)
    {
        _news.ImgAlt = imgAlt;
        return this;
    }

    public NewsBuilder WithContent(string content)
    {
        _news.Content = content;
        return this;
    }

    public NewsBuilder WithSubjects(params string[] subjects)
    {
        _news.Subjects = subjects;
        return this;
    }

    public NewsBuilder WithAuthors(params string[] authors)
    {
        _news.Authors = authors;
        return this;
    }

    public NewsBuilder WithExpressDate(DateTime expressDate)
    {
        _news.ExpressDate = expressDate;
        return this;
    }

    public NewsBuilder WithCreateDate(DateTime createDate)
    {
        _news.CreateDate = createDate;
        return this;
    }

    public NewsBuilder WithUpdateDate(DateTime updateDate)
    {
        _news.UpdateDate = updateDate;
        return this;
    }

    public NewsBuilder WithPriority(int priority)
    {
        _news.Priority = priority;
        return this;
    }

    public NewsBuilder WithIsActive(bool isActive)
    {
        _news.IsActive = isActive;
        return this;
    }

    public NewsBuilder WithViewCount(int viewCount)
    {
        _news.ViewCount = viewCount;
        return this;
    }

    public NewsBuilder WithIsSecondPageNews(bool isSecondPageNews)
    {
        _news.IsSecondPageNews = isSecondPageNews;
        return this;
    }

    public NewsBuilder AsTechnologyNews()
    {
        return WithCategory("Technology")
            .WithType("Article")
            .WithCaption("Technology NewsArticle Article")
            .WithKeywords("tech, innovation, digital")
            .WithSocialTags("#tech #innovation #digital")
            .WithSubjects("Technology", "Innovation");
    }

    public NewsBuilder AsSportsNews()
    {
        return WithCategory("Sports")
            .WithType("Breaking")
            .WithCaption("Sports Breaking NewsArticle")
            .WithKeywords("sports, game, competition")
            .WithSocialTags("#sports #game #competition")
            .WithSubjects("Sports", "Competition");
    }

    public NewsBuilder AsBreakingNews()
    {
        return WithType("Breaking")
            .WithPriority(10)
            .WithExpressDate(DateTime.UtcNow)
            .WithCaption("Breaking NewsArticle Alert");
    }

    public NewsBuilder AsInactive()
    {
        return WithIsActive(false);
    }

    public NewsBuilder AsPopular()
    {
        return WithViewCount(1000).WithPriority(8);
    }

    public NewsBuilder AsSecondPageNews()
    {
        return WithIsSecondPageNews(true).WithPriority(1);
    }

    public NewsArticle Build() => _news;

    public List<NewsArticle> BuildMany(int count)
    {
        var newsList = new List<NewsArticle>();
        for (int i = 0; i < count; i++)
        {
            var news = Build();
            news.Id = Guid.NewGuid().ToString(); // Ensure unique IDs
            news.Caption = $"{news.Caption} {i + 1}";
            newsList.Add(news);
        }
        return newsList;
    }
}

/// <summary>
/// Builder class for creating test CreateNewsArticleDto objects
/// </summary>
internal class CreateNewsArticleDtoBuilder
{
    private readonly CreateNewsArticleDto _dto;

    public CreateNewsArticleDtoBuilder()
    {
        _dto = new CreateNewsArticleDto
        {
            Category = "Technology",
            Type = "Article",
            Caption = "Default Test Caption",
            Keywords = "test, default",
            SocialTags = "#test #default",
            Summary = "Default test summary",
            ImgPath = "/images/default-test.jpg",
            ImgAlt = "Default test image",
            Content = "Default test content for news article",
            Subjects = ["Testing"],
            Authors = ["Test Author"],
            ExpressDate = DateTime.UtcNow,
            Priority = 1,
        };
    }

    public static CreateNewsArticleDtoBuilder Create() => new();

    public CreateNewsArticleDtoBuilder WithCategory(string category)
    {
        _dto.Category = category;
        return this;
    }

    public CreateNewsArticleDtoBuilder WithType(string type)
    {
        _dto.Type = type;
        return this;
    }

    public CreateNewsArticleDtoBuilder WithCaption(string caption)
    {
        _dto.Caption = caption;
        return this;
    }

    public CreateNewsArticleDtoBuilder WithKeywords(string? keywords)
    {
        _dto.Keywords = keywords ?? string.Empty;
        return this;
    }

    public CreateNewsArticleDtoBuilder WithSocialTags(string? socialTags)
    {
        _dto.SocialTags = socialTags ?? string.Empty;
        return this;
    }

    public CreateNewsArticleDtoBuilder WithSummary(string summary)
    {
        _dto.Summary = summary;
        return this;
    }

    public CreateNewsArticleDtoBuilder WithImagePath(string? imgPath)
    {
        _dto.ImgPath = imgPath ?? string.Empty;
        return this;
    }

    public CreateNewsArticleDtoBuilder WithImageAlt(string? imgAlt)
    {
        _dto.ImgAlt = imgAlt ?? string.Empty;
        return this;
    }

    public CreateNewsArticleDtoBuilder WithContent(string content)
    {
        _dto.Content = content;
        return this;
    }

    public CreateNewsArticleDtoBuilder WithSubjects(params string[]? subjects)
    {
        _dto.Subjects = subjects ?? [];
        return this;
    }

    public CreateNewsArticleDtoBuilder WithAuthors(params string[]? authors)
    {
        _dto.Authors = authors ?? [];
        return this;
    }

    public CreateNewsArticleDtoBuilder WithExpressDate(DateTime expressDate)
    {
        _dto.ExpressDate = expressDate;
        return this;
    }

    public CreateNewsArticleDtoBuilder WithPriority(int priority)
    {
        _dto.Priority = priority;
        return this;
    }

    public CreateNewsArticleDtoBuilder AsValidTechnologyNews()
    {
        return WithCategory("Technology")
            .WithType("Article")
            .WithCaption("Valid Technology NewsArticle")
            .WithSummary("A valid technology news article for testing")
            .WithContent("This is a comprehensive technology article with valid content")
            .WithExpressDate(DateTime.UtcNow)
            .WithPriority(1);
    }

    public CreateNewsArticleDtoBuilder AsInvalidDto()
    {
        return WithCategory("")
            .WithType("")
            .WithCaption("")
            .WithSummary("")
            .WithContent("")
            .WithExpressDate(DateTime.MinValue)
            .WithPriority(0);
    }

    public CreateNewsArticleDtoBuilder WithTooLongFields()
    {
        return WithCategory(new string('a', 101))
            .WithType(new string('b', 51))
            .WithCaption(new string('c', 501))
            .WithKeywords(new string('d', 1001))
            .WithSocialTags(new string('e', 501))
            .WithSummary(new string('f', 2001))
            .WithImagePath(new string('g', 501))
            .WithImageAlt(new string('h', 201))
            .WithPriority(101);
    }

    public CreateNewsArticleDto Build() => _dto;
}

/// <summary>
/// Builder class for creating test UpdateNewsArticleDto objects
/// </summary>
internal class UpdateNewsArticleDtoBuilder
{
    private readonly UpdateNewsArticleDto _dto;

    public UpdateNewsArticleDtoBuilder()
    {
        _dto = new UpdateNewsArticleDto
        {
            Category = "Technology",
            Type = "Article",
            Caption = "Updated Test Caption",
            Keywords = "updated, test",
            SocialTags = "#updated #test",
            Summary = "Updated test summary",
            ImgPath = "/images/updated-test.jpg",
            ImgAlt = "Updated test image",
            Content = "Updated test content for news article",
            Subjects = ["Updated Testing"],
            Authors = ["Updated Test Author"],
            ExpressDate = DateTime.UtcNow,
            Priority = 2,
        };
    }

    public static UpdateNewsArticleDtoBuilder Create() => new();

    public UpdateNewsArticleDtoBuilder WithCategory(string category)
    {
        _dto.Category = category;
        return this;
    }

    public UpdateNewsArticleDtoBuilder WithCaption(string caption)
    {
        _dto.Caption = caption;
        return this;
    }

    public UpdateNewsArticleDtoBuilder WithSummary(string summary)
    {
        _dto.Summary = summary;
        return this;
    }

    public UpdateNewsArticleDtoBuilder WithContent(string content)
    {
        _dto.Content = content;
        return this;
    }

    public UpdateNewsArticleDto Build() => _dto;
}
