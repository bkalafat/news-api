using FluentValidation.TestHelper;
using NewsApi.Application.DTOs;
using NewsApi.Application.Validators;

namespace NewsApi.Tests.Unit.Application;

internal class NewsValidatorTests
{
    private readonly CreateNewsArticleDtoValidator _validator;

    public NewsValidatorTests()
    {
        _validator = new CreateNewsArticleDtoValidator();
    }

    [Fact]
    public void Category_WhenEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new CreateNewsArticleDto { Category = "" };

        // Act & Assert
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(dto => dto.Category).WithErrorMessage("Category is required");
    }

    [Fact]
    public void Category_WhenNull_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new CreateNewsArticleDto { Category = null! };

        // Act & Assert
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(dto => dto.Category);
    }

    [Fact]
    public void Category_WhenTooLong_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new CreateNewsArticleDto { Category = new string('a', 101) };

        // Act & Assert
        var result = _validator.TestValidate(dto);
        result
            .ShouldHaveValidationErrorFor(dto => dto.Category)
            .WithErrorMessage("Category must not exceed 100 characters");
    }

    [Fact]
    public void Category_WhenValid_ShouldNotHaveValidationError()
    {
        // Arrange
        var dto = new CreateNewsArticleDto { Category = "Technology" };

        // Act & Assert
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(dto => dto.Category);
    }

    [Fact]
    public void Type_WhenEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new CreateNewsArticleDto { Type = "" };

        // Act & Assert
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(dto => dto.Type).WithErrorMessage("Type is required");
    }

    [Fact]
    public void Type_WhenTooLong_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new CreateNewsArticleDto { Type = new string('a', 51) };

        // Act & Assert
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(dto => dto.Type).WithErrorMessage("Type must not exceed 50 characters");
    }

    [Fact]
    public void Caption_WhenEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new CreateNewsArticleDto { Caption = "" };

        // Act & Assert
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(dto => dto.Caption).WithErrorMessage("Caption is required");
    }

    [Fact]
    public void Caption_WhenTooLong_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new CreateNewsArticleDto { Caption = new string('a', 501) };

        // Act & Assert
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(dto => dto.Caption).WithErrorMessage("Caption must not exceed 500 characters");
    }

    [Fact]
    public void Keywords_WhenTooLong_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new CreateNewsArticleDto { Keywords = new string('a', 1001) };

        // Act & Assert
        var result = _validator.TestValidate(dto);
        result
            .ShouldHaveValidationErrorFor(dto => dto.Keywords)
            .WithErrorMessage("Keywords must not exceed 1000 characters");
    }

    [Fact]
    public void Keywords_WhenEmpty_ShouldNotHaveValidationError()
    {
        // Arrange
        var dto = new CreateNewsArticleDto { Keywords = "" };

        // Act & Assert
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(dto => dto.Keywords);
    }

    [Fact]
    public void SocialTags_WhenTooLong_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new CreateNewsArticleDto { SocialTags = new string('a', 501) };

        // Act & Assert
        var result = _validator.TestValidate(dto);
        result
            .ShouldHaveValidationErrorFor(dto => dto.SocialTags)
            .WithErrorMessage("Social tags must not exceed 500 characters");
    }

    [Fact]
    public void Summary_WhenEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new CreateNewsArticleDto { Summary = "" };

        // Act & Assert
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(dto => dto.Summary).WithErrorMessage("Summary is required");
    }

    [Fact]
    public void Summary_WhenTooLong_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new CreateNewsArticleDto { Summary = new string('a', 2001) };

        // Act & Assert
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(dto => dto.Summary).WithErrorMessage("Summary must not exceed 2000 characters");
    }

    [Fact]
    public void ImgPath_WhenTooLong_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new CreateNewsArticleDto { ImgPath = new string('a', 501) };

        // Act & Assert
        var result = _validator.TestValidate(dto);
        result
            .ShouldHaveValidationErrorFor(dto => dto.ImgPath)
            .WithErrorMessage("Image path must not exceed 500 characters");
    }

    [Fact]
    public void ImgAlt_WhenTooLong_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new CreateNewsArticleDto { ImgAlt = new string('a', 201) };

        // Act & Assert
        var result = _validator.TestValidate(dto);
        result
            .ShouldHaveValidationErrorFor(dto => dto.ImgAlt)
            .WithErrorMessage("Image alt text must not exceed 200 characters");
    }

    [Fact]
    public void Content_WhenEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new CreateNewsArticleDto { Content = "" };

        // Act & Assert
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(dto => dto.Content).WithErrorMessage("Content is required");
    }

    [Fact]
    public void ExpressDate_WhenEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var dto = new CreateNewsArticleDto { ExpressDate = DateTime.MinValue };

        // Act & Assert
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(dto => dto.ExpressDate);
    }

    [Fact]
    public void ExpressDate_WhenValid_ShouldNotHaveValidationError()
    {
        // Arrange
        var dto = new CreateNewsArticleDto { ExpressDate = DateTime.UtcNow };

        // Act & Assert
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(dto => dto.ExpressDate);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(101)]
    public void Priority_WhenOutOfRange_ShouldHaveValidationError(int priority)
    {
        // Arrange
        var dto = new CreateNewsArticleDto { Priority = priority };

        // Act & Assert
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(dto => dto.Priority).WithErrorMessage("Priority must be between 1 and 100");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(50)]
    [InlineData(100)]
    public void Priority_WhenInRange_ShouldNotHaveValidationError(int priority)
    {
        // Arrange
        var dto = new CreateNewsArticleDto { Priority = priority };

        // Act & Assert
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(dto => dto.Priority);
    }

    [Fact]
    public void ValidDto_ShouldNotHaveAnyValidationErrors()
    {
        // Arrange
        var dto = new CreateNewsArticleDto
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
        };

        // Act & Assert
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
