using FluentAssertions;
using FluentValidation.TestHelper;
using NewsApi.Application.DTOs;
using NewsApi.Application.Validators;

namespace NewsApi.Tests.Unit.Application;

public class NewsValidatorTests
{
    private readonly CreateNewsDtoValidator _validator;

    public NewsValidatorTests()
    {
        _validator = new CreateNewsDtoValidator();
    }

    [Fact]
    public void Category_WhenEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new CreateNewsDto { Category = "" };

        // Act & Assert
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Category)
              .WithErrorMessage("Category is required");
    }

    [Fact]
    public void Category_WhenNull_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new CreateNewsDto { Category = null! };

        // Act & Assert
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Category);
    }

    [Fact]
    public void Category_WhenTooLong_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new CreateNewsDto { Category = new string('a', 101) };

        // Act & Assert
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Category)
              .WithErrorMessage("Category must not exceed 100 characters");
    }

    [Fact]
    public void Category_WhenValid_ShouldNotHaveValidationError()
    {
        // Arrange
        var dto = new CreateNewsDto { Category = "Technology" };

        // Act & Assert
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.Category);
    }

    [Fact]
    public void Type_WhenEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new CreateNewsDto { Type = "" };

        // Act & Assert
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Type)
              .WithErrorMessage("Type is required");
    }

    [Fact]
    public void Type_WhenTooLong_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new CreateNewsDto { Type = new string('a', 51) };

        // Act & Assert
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Type)
              .WithErrorMessage("Type must not exceed 50 characters");
    }

    [Fact]
    public void Caption_WhenEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new CreateNewsDto { Caption = "" };

        // Act & Assert
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Caption)
              .WithErrorMessage("Caption is required");
    }

    [Fact]
    public void Caption_WhenTooLong_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new CreateNewsDto { Caption = new string('a', 501) };

        // Act & Assert
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Caption)
              .WithErrorMessage("Caption must not exceed 500 characters");
    }

    [Fact]
    public void Keywords_WhenTooLong_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new CreateNewsDto { Keywords = new string('a', 1001) };

        // Act & Assert
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Keywords)
              .WithErrorMessage("Keywords must not exceed 1000 characters");
    }

    [Fact]
    public void Keywords_WhenEmpty_ShouldNotHaveValidationError()
    {
        // Arrange
        var dto = new CreateNewsDto { Keywords = "" };

        // Act & Assert
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.Keywords);
    }

    [Fact]
    public void SocialTags_WhenTooLong_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new CreateNewsDto { SocialTags = new string('a', 501) };

        // Act & Assert
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.SocialTags)
              .WithErrorMessage("Social tags must not exceed 500 characters");
    }

    [Fact]
    public void Summary_WhenEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new CreateNewsDto { Summary = "" };

        // Act & Assert
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Summary)
              .WithErrorMessage("Summary is required");
    }

    [Fact]
    public void Summary_WhenTooLong_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new CreateNewsDto { Summary = new string('a', 2001) };

        // Act & Assert
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Summary)
              .WithErrorMessage("Summary must not exceed 2000 characters");
    }

    [Fact]
    public void ImgPath_WhenTooLong_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new CreateNewsDto { ImgPath = new string('a', 501) };

        // Act & Assert
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.ImgPath)
              .WithErrorMessage("Image path must not exceed 500 characters");
    }

    [Fact]
    public void ImgAlt_WhenTooLong_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new CreateNewsDto { ImgAlt = new string('a', 201) };

        // Act & Assert
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.ImgAlt)
              .WithErrorMessage("Image alt text must not exceed 200 characters");
    }

    [Fact]
    public void Content_WhenEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new CreateNewsDto { Content = "" };

        // Act & Assert
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Content)
              .WithErrorMessage("Content is required");
    }

    [Fact]
    public void ExpressDate_WhenEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new CreateNewsDto { ExpressDate = DateTime.MinValue };

        // Act & Assert
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.ExpressDate);
    }

    [Fact]
    public void ExpressDate_WhenValid_ShouldNotHaveValidationError()
    {
        // Arrange
        var dto = new CreateNewsDto { ExpressDate = DateTime.UtcNow };

        // Act & Assert
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.ExpressDate);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(101)]
    public void Priority_WhenOutOfRange_ShouldHaveValidationError(int priority)
    {
        // Arrange
        var dto = new CreateNewsDto { Priority = priority };

        // Act & Assert
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Priority)
              .WithErrorMessage("Priority must be between 1 and 100");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(50)]
    [InlineData(100)]
    public void Priority_WhenInRange_ShouldNotHaveValidationError(int priority)
    {
        // Arrange
        var dto = new CreateNewsDto { Priority = priority };

        // Act & Assert
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.Priority);
    }

    [Fact]
    public void Url_WhenTooLong_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new CreateNewsDto { Url = new string('a', 501) };

        // Act & Assert
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Url)
              .WithErrorMessage("URL must not exceed 500 characters");
    }

    [Fact]
    public void ValidDto_ShouldNotHaveAnyValidationErrors()
    {
        // Arrange
        var dto = new CreateNewsDto
        {
            Category = "Technology",
            Type = "Article",
            Caption = "Test Caption",
            Keywords = "test, keywords",
            SocialTags = "#tech #news",
            Summary = "Test summary content",
            ImgPath = "/images/test.jpg",
            ImgAlt = "Test image",
            Content = "Full content of the news article",
            ExpressDate = DateTime.UtcNow,
            Priority = 1,
            Url = "test-article"
        };

        // Act & Assert
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }
}