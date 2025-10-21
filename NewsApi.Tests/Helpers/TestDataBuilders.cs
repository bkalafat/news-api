using NewsApi.Application.DTOs;
using NewsApi.Domain.Entities;

namespace NewsApi.Tests.Helpers;

/// <summary>
/// Builder class for creating test News entities with fluent API
/// </summary>
public class NewsBuilder
{
    private readonly News _news;

    public NewsBuilder()
    {
        _news = new News
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
      Subjects = new[] { "Testing" },
      Authors = new[] { "Test Author" },
      ExpressDate = DateTime.UtcNow,
      CreateDate = DateTime.UtcNow,
      UpdateDate = DateTime.UtcNow,
      Priority = 1,
      IsActive = true,
      Url = "default-test-news",
      ViewCount = 0,
      IsSecondPageNews = false
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

    public NewsBuilder WithUrl(string url)
    {
        _news.Url = url;
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
               .WithCaption("Technology News Article")
               .WithKeywords("tech, innovation, digital")
               .WithSocialTags("#tech #innovation #digital")
               .WithSubjects("Technology", "Innovation")
               .WithUrl("technology-news-article");
    }

    public NewsBuilder AsSportsNews()
    {
        return WithCategory("Sports")
               .WithType("Breaking")
               .WithCaption("Sports Breaking News")
               .WithKeywords("sports, game, competition")
               .WithSocialTags("#sports #game #competition")
               .WithSubjects("Sports", "Competition")
               .WithUrl("sports-breaking-news");
    }

    public NewsBuilder AsBreakingNews()
    {
        return WithType("Breaking")
               .WithPriority(10)
               .WithExpressDate(DateTime.UtcNow)
               .WithCaption("Breaking News Alert");
    }

    public NewsBuilder AsInactive()
    {
        return WithIsActive(false);
    }

    public NewsBuilder AsPopular()
    {
        return WithViewCount(1000)
               .WithPriority(8);
    }

    public NewsBuilder AsSecondPageNews()
    {
        return WithIsSecondPageNews(true)
               .WithPriority(1);
    }

    public News Build() => _news;

    public List<News> BuildMany(int count)
    {
        var newsList = new List<News>();
        for (int i = 0; i < count; i++)
        {
            var news = Build();
            news.Id = Guid.NewGuid().ToString(); // Ensure unique IDs
            news.Caption = $"{news.Caption} {i + 1}";
            news.Url = $"{news.Url}-{i + 1}";
            newsList.Add(news);
        }
        return newsList;
    }
}

/// <summary>
/// Builder class for creating test CreateNewsDto objects
/// </summary>
public class CreateNewsDtoBuilder
{
    private readonly CreateNewsDto _dto;

    public CreateNewsDtoBuilder()
    {
        _dto = new CreateNewsDto
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
            Subjects = new[] { "Testing" },
            Authors = new[] { "Test Author" },
            ExpressDate = DateTime.UtcNow,
            Priority = 1,
            Url = "default-test-news"
        };
    }

    public static CreateNewsDtoBuilder Create() => new();

    public CreateNewsDtoBuilder WithCategory(string category)
    {
        _dto.Category = category;
        return this;
    }

    public CreateNewsDtoBuilder WithType(string type)
    {
        _dto.Type = type;
        return this;
    }

    public CreateNewsDtoBuilder WithCaption(string caption)
    {
        _dto.Caption = caption;
        return this;
    }

    public CreateNewsDtoBuilder WithKeywords(string? keywords)
    {
        _dto.Keywords = keywords;
        return this;
    }

    public CreateNewsDtoBuilder WithSocialTags(string? socialTags)
    {
        _dto.SocialTags = socialTags;
        return this;
    }

    public CreateNewsDtoBuilder WithSummary(string summary)
    {
        _dto.Summary = summary;
        return this;
    }

    public CreateNewsDtoBuilder WithImagePath(string? imgPath)
    {
        _dto.ImgPath = imgPath;
        return this;
    }

    public CreateNewsDtoBuilder WithImageAlt(string? imgAlt)
    {
        _dto.ImgAlt = imgAlt;
        return this;
    }

    public CreateNewsDtoBuilder WithContent(string content)
    {
        _dto.Content = content;
        return this;
    }

    public CreateNewsDtoBuilder WithSubjects(params string[]? subjects)
    {
        _dto.Subjects = subjects;
        return this;
    }

    public CreateNewsDtoBuilder WithAuthors(params string[]? authors)
    {
        _dto.Authors = authors;
        return this;
    }

    public CreateNewsDtoBuilder WithExpressDate(DateTime expressDate)
    {
        _dto.ExpressDate = expressDate;
        return this;
    }

    public CreateNewsDtoBuilder WithPriority(int priority)
    {
        _dto.Priority = priority;
        return this;
    }

    public CreateNewsDtoBuilder WithUrl(string? url)
    {
        _dto.Url = url;
        return this;
    }

    public CreateNewsDtoBuilder AsValidTechnologyNews()
    {
        return WithCategory("Technology")
               .WithType("Article")
               .WithCaption("Valid Technology News")
               .WithSummary("A valid technology news article for testing")
               .WithContent("This is a comprehensive technology article with valid content")
               .WithExpressDate(DateTime.UtcNow)
               .WithPriority(1);
    }

    public CreateNewsDtoBuilder AsInvalidDto()
    {
        return WithCategory("")
               .WithType("")
               .WithCaption("")
               .WithSummary("")
               .WithContent("")
               .WithExpressDate(DateTime.MinValue)
               .WithPriority(0);
    }

    public CreateNewsDtoBuilder WithTooLongFields()
    {
        return WithCategory(new string('a', 101))
               .WithType(new string('b', 51))
               .WithCaption(new string('c', 501))
               .WithKeywords(new string('d', 1001))
               .WithSocialTags(new string('e', 501))
               .WithSummary(new string('f', 2001))
               .WithImagePath(new string('g', 501))
               .WithImageAlt(new string('h', 201))
               .WithUrl(new string('i', 501))
               .WithPriority(101);
    }

    public CreateNewsDto Build() => _dto;
}

/// <summary>
/// Builder class for creating test UpdateNewsDto objects
/// </summary>
public class UpdateNewsDtoBuilder
{
    private readonly UpdateNewsDto _dto;

    public UpdateNewsDtoBuilder()
    {
        _dto = new UpdateNewsDto
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
            Subjects = new[] { "Updated Testing" },
            Authors = new[] { "Updated Test Author" },
            ExpressDate = DateTime.UtcNow,
            Priority = 2,
            Url = "updated-test-news"
        };
    }

    public static UpdateNewsDtoBuilder Create() => new();

    public UpdateNewsDtoBuilder WithCategory(string category)
    {
        _dto.Category = category;
        return this;
    }

    public UpdateNewsDtoBuilder WithCaption(string caption)
    {
        _dto.Caption = caption;
        return this;
    }

    public UpdateNewsDtoBuilder WithSummary(string summary)
    {
        _dto.Summary = summary;
        return this;
    }

    public UpdateNewsDtoBuilder WithContent(string content)
    {
        _dto.Content = content;
        return this;
    }

    public UpdateNewsDto Build() => _dto;
}