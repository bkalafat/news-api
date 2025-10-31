using FluentValidation.TestHelper;
using NewsApi.Tests.Helpers;

namespace NewsApi.Tests.Unit.Validators;

public class CreateNewsArticleDtoValidatorTests
{
    private readonly CreateNewsArticleDtoValidator _validator;

    public CreateNewsArticleDtoValidatorTests()
    {
        _validator = new CreateNewsArticleDtoValidator();
    }

    #region Category Tests

    [Fact]
    public void Category_WhenEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var dto = CreateNewsArticleDtoBuilder.Create().WithCategory(string.Empty).Build();

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(dto => dto.Category).WithErrorMessage("Category is required");
    }

    [Fact]
    public void Category_WhenExceedsMaxLength_ShouldHaveValidationError()
    {
        // Arrange
        var dto = CreateNewsArticleDtoBuilder.Create().WithCategory(new string('a', 101)).Build();

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result
            .ShouldHaveValidationErrorFor(dto => dto.Category)
            .WithErrorMessage("Category must not exceed 100 characters");
    }

    [Fact]
    public void Category_WhenValid_ShouldNotHaveValidationError()
    {
        // Arrange
        var dto = CreateNewsArticleDtoBuilder.Create().WithCategory("Technology").Build();

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveValidationErrorFor(dto => dto.Category);
    }

    #endregion

    #region Type Tests

    [Fact]
    public void Type_WhenEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var dto = CreateNewsArticleDtoBuilder.Create().WithType(string.Empty).Build();

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(dto => dto.Type).WithErrorMessage("Type is required");
    }

    [Fact]
    public void Type_WhenExceedsMaxLength_ShouldHaveValidationError()
    {
        // Arrange
        var dto = CreateNewsArticleDtoBuilder.Create().WithType(new string('a', 51)).Build();

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(dto => dto.Type).WithErrorMessage("Type must not exceed 50 characters");
    }

    [Fact]
    public void Type_WhenValid_ShouldNotHaveValidationError()
    {
        // Arrange
        var dto = CreateNewsArticleDtoBuilder.Create().WithType("Article").Build();

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveValidationErrorFor(dto => dto.Type);
    }

    #endregion

    #region Caption Tests

    [Fact]
    public void Caption_WhenEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var dto = CreateNewsArticleDtoBuilder.Create().WithCaption(string.Empty).Build();

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(dto => dto.Caption).WithErrorMessage("Caption is required");
    }

    [Fact]
    public void Caption_WhenExceedsMaxLength_ShouldHaveValidationError()
    {
        // Arrange
        var dto = CreateNewsArticleDtoBuilder.Create().WithCaption(new string('a', 501)).Build();

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(dto => dto.Caption).WithErrorMessage("Caption must not exceed 500 characters");
    }

    [Fact]
    public void Caption_WhenValid_ShouldNotHaveValidationError()
    {
        // Arrange
        var dto = CreateNewsArticleDtoBuilder.Create().WithCaption("Valid Caption").Build();

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveValidationErrorFor(dto => dto.Caption);
    }

    #endregion

    #region Keywords Tests

    [Fact]
    public void Keywords_WhenExceedsMaxLength_ShouldHaveValidationError()
    {
        // Arrange
        var dto = CreateNewsArticleDtoBuilder.Create().WithKeywords(new string('a', 1001)).Build();

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result
            .ShouldHaveValidationErrorFor(dto => dto.Keywords)
            .WithErrorMessage("Keywords must not exceed 1000 characters");
    }

    [Fact]
    public void Keywords_WhenValid_ShouldNotHaveValidationError()
    {
        // Arrange
        var dto = CreateNewsArticleDtoBuilder.Create().WithKeywords("tech, news, innovation").Build();

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveValidationErrorFor(dto => dto.Keywords);
    }

    [Fact]
    public void Keywords_WhenNull_ShouldNotHaveValidationError()
    {
        // Arrange
        var dto = CreateNewsArticleDtoBuilder.Create().WithKeywords(null).Build();

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveValidationErrorFor(dto => dto.Keywords);
    }

    #endregion

    #region SocialTags Tests

    [Fact]
    public void SocialTags_WhenExceedsMaxLength_ShouldHaveValidationError()
    {
        // Arrange
        var dto = CreateNewsArticleDtoBuilder.Create().WithSocialTags(new string('a', 501)).Build();

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result
            .ShouldHaveValidationErrorFor(dto => dto.SocialTags)
            .WithErrorMessage("Social tags must not exceed 500 characters");
    }

    [Fact]
    public void SocialTags_WhenValid_ShouldNotHaveValidationError()
    {
        // Arrange
        var dto = CreateNewsArticleDtoBuilder.Create().WithSocialTags("#tech #news").Build();

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveValidationErrorFor(dto => dto.SocialTags);
    }

    #endregion

    #region Summary Tests

    [Fact]
    public void Summary_WhenEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var dto = CreateNewsArticleDtoBuilder.Create().WithSummary(string.Empty).Build();

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(dto => dto.Summary).WithErrorMessage("Summary is required");
    }

    [Fact]
    public void Summary_WhenExceedsMaxLength_ShouldHaveValidationError()
    {
        // Arrange
        var dto = CreateNewsArticleDtoBuilder.Create().WithSummary(new string('a', 2001)).Build();

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result
            .ShouldHaveValidationErrorFor(dto => dto.Summary)
            .WithErrorMessage("Summary must not exceed 2000 characters");
    }

    [Fact]
    public void Summary_WhenValid_ShouldNotHaveValidationError()
    {
        // Arrange
        var dto = CreateNewsArticleDtoBuilder.Create().WithSummary("Valid summary text").Build();

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveValidationErrorFor(dto => dto.Summary);
    }

    #endregion

    #region Content Tests

    [Fact]
    public void Content_WhenEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var dto = CreateNewsArticleDtoBuilder.Create().WithContent(string.Empty).Build();

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(dto => dto.Content).WithErrorMessage("Content is required");
    }

    [Fact]
    public void Content_WhenValid_ShouldNotHaveValidationError()
    {
        // Arrange
        var dto = CreateNewsArticleDtoBuilder.Create().WithContent("Valid news content").Build();

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveValidationErrorFor(dto => dto.Content);
    }

    #endregion

    #region ExpressDate Tests

    [Fact]
    public void ExpressDate_WhenMinValue_ShouldHaveValidationError()
    {
        // Arrange
        var dto = CreateNewsArticleDtoBuilder.Create().WithExpressDate(DateTime.MinValue).Build();

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(dto => dto.ExpressDate);
    }

    [Fact]
    public void ExpressDate_WhenValid_ShouldNotHaveValidationError()
    {
        // Arrange
        var dto = CreateNewsArticleDtoBuilder.Create().WithExpressDate(DateTime.UtcNow).Build();

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveValidationErrorFor(dto => dto.ExpressDate);
    }

    #endregion

    #region Priority Tests

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(101)]
    [InlineData(1000)]
    public void Priority_WhenOutOfRange_ShouldHaveValidationError(int priority)
    {
        // Arrange
        var dto = CreateNewsArticleDtoBuilder.Create().WithPriority(priority).Build();

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(dto => dto.Priority).WithErrorMessage("Priority must be between 1 and 100");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(50)]
    [InlineData(100)]
    public void Priority_WhenInRange_ShouldNotHaveValidationError(int priority)
    {
        // Arrange
        var dto = CreateNewsArticleDtoBuilder.Create().WithPriority(priority).Build();

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveValidationErrorFor(dto => dto.Priority);
    }

    #endregion

    #region Image Tests

    [Fact]
    public void ImgPath_WhenExceedsMaxLength_ShouldHaveValidationError()
    {
        // Arrange
        var dto = CreateNewsArticleDtoBuilder.Create().WithImagePath(new string('a', 501)).Build();

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result
            .ShouldHaveValidationErrorFor(dto => dto.ImgPath)
            .WithErrorMessage("Image path must not exceed 500 characters");
    }

    [Fact]
    public void ImgAlt_WhenExceedsMaxLength_ShouldHaveValidationError()
    {
        // Arrange
        var dto = CreateNewsArticleDtoBuilder.Create().WithImageAlt(new string('a', 201)).Build();

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result
            .ShouldHaveValidationErrorFor(dto => dto.ImgAlt)
            .WithErrorMessage("Image alt text must not exceed 200 characters");
    }

    #endregion

    #region Complete DTO Tests

    [Fact]
    public void CompleteValidDto_ShouldNotHaveAnyValidationErrors()
    {
        // Arrange
        var dto = CreateNewsArticleDtoBuilder.Create().AsValidTechnologyNews().Build();

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void InvalidDto_WithMultipleErrors_ShouldHaveMultipleValidationErrors()
    {
        // Arrange
        var dto = CreateNewsArticleDtoBuilder.Create().AsInvalidDto().Build();

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(dto => dto.Category);
        result.ShouldHaveValidationErrorFor(dto => dto.Type);
        result.ShouldHaveValidationErrorFor(dto => dto.Caption);
        result.ShouldHaveValidationErrorFor(dto => dto.Summary);
        result.ShouldHaveValidationErrorFor(dto => dto.Content);
    }

    #endregion
}
