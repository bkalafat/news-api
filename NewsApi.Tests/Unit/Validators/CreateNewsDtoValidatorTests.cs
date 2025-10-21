using FluentValidation.TestHelper;
using NewsApi.Application.Validators;
using NewsApi.Tests.Helpers;

namespace NewsApi.Tests.Unit.Validators;

public class CreateNewsDtoValidatorTests
{
    private readonly CreateNewsDtoValidator _validator;

    public CreateNewsDtoValidatorTests()
    {
        _validator = new CreateNewsDtoValidator();
    }

    #region Category Tests

    [Fact]
    public void Category_WhenEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var dto = CreateNewsDtoBuilder.Create().WithCategory(string.Empty).Build();

    // Act
        var result = _validator.TestValidate(dto);

   // Assert
        result.ShouldHaveValidationErrorFor(x => x.Category)
            .WithErrorMessage("Category is required");
    }

    [Fact]
    public void Category_WhenExceedsMaxLength_ShouldHaveValidationError()
    {
        // Arrange
        var dto = CreateNewsDtoBuilder.Create().WithCategory(new string('a', 101)).Build();

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Category)
     .WithErrorMessage("Category must not exceed 100 characters");
    }

    [Fact]
    public void Category_WhenValid_ShouldNotHaveValidationError()
    {
        // Arrange
   var dto = CreateNewsDtoBuilder.Create().WithCategory("Technology").Build();

  // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Category);
    }

    #endregion

    #region Type Tests

    [Fact]
  public void Type_WhenEmpty_ShouldHaveValidationError()
    {
        // Arrange
   var dto = CreateNewsDtoBuilder.Create().WithType(string.Empty).Build();

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Type)
            .WithErrorMessage("Type is required");
    }

    [Fact]
    public void Type_WhenExceedsMaxLength_ShouldHaveValidationError()
 {
     // Arrange
        var dto = CreateNewsDtoBuilder.Create().WithType(new string('a', 51)).Build();

    // Act
        var result = _validator.TestValidate(dto);

  // Assert
        result.ShouldHaveValidationErrorFor(x => x.Type)
            .WithErrorMessage("Type must not exceed 50 characters");
    }

    [Fact]
    public void Type_WhenValid_ShouldNotHaveValidationError()
    {
        // Arrange
        var dto = CreateNewsDtoBuilder.Create().WithType("Article").Build();

        // Act
        var result = _validator.TestValidate(dto);

   // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Type);
    }

    #endregion

    #region Caption Tests

    [Fact]
    public void Caption_WhenEmpty_ShouldHaveValidationError()
  {
        // Arrange
        var dto = CreateNewsDtoBuilder.Create().WithCaption(string.Empty).Build();

        // Act
        var result = _validator.TestValidate(dto);

     // Assert
        result.ShouldHaveValidationErrorFor(x => x.Caption)
        .WithErrorMessage("Caption is required");
    }

    [Fact]
    public void Caption_WhenExceedsMaxLength_ShouldHaveValidationError()
    {
        // Arrange
        var dto = CreateNewsDtoBuilder.Create().WithCaption(new string('a', 501)).Build();

        // Act
        var result = _validator.TestValidate(dto);

     // Assert
 result.ShouldHaveValidationErrorFor(x => x.Caption)
            .WithErrorMessage("Caption must not exceed 500 characters");
    }

    [Fact]
    public void Caption_WhenValid_ShouldNotHaveValidationError()
    {
        // Arrange
        var dto = CreateNewsDtoBuilder.Create().WithCaption("Valid Caption").Build();

        // Act
        var result = _validator.TestValidate(dto);

    // Assert
     result.ShouldNotHaveValidationErrorFor(x => x.Caption);
    }

    #endregion

    #region Keywords Tests

    [Fact]
    public void Keywords_WhenExceedsMaxLength_ShouldHaveValidationError()
    {
        // Arrange
    var dto = CreateNewsDtoBuilder.Create().WithKeywords(new string('a', 1001)).Build();

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Keywords)
    .WithErrorMessage("Keywords must not exceed 1000 characters");
    }

    [Fact]
 public void Keywords_WhenValid_ShouldNotHaveValidationError()
    {
        // Arrange
        var dto = CreateNewsDtoBuilder.Create().WithKeywords("tech, news, innovation").Build();

        // Act
      var result = _validator.TestValidate(dto);

      // Assert
  result.ShouldNotHaveValidationErrorFor(x => x.Keywords);
    }

    [Fact]
    public void Keywords_WhenNull_ShouldNotHaveValidationError()
    {
    // Arrange
        var dto = CreateNewsDtoBuilder.Create().WithKeywords(null).Build();

        // Act
        var result = _validator.TestValidate(dto);

      // Assert
    result.ShouldNotHaveValidationErrorFor(x => x.Keywords);
    }

    #endregion

  #region SocialTags Tests

    [Fact]
    public void SocialTags_WhenExceedsMaxLength_ShouldHaveValidationError()
    {
        // Arrange
    var dto = CreateNewsDtoBuilder.Create().WithSocialTags(new string('a', 501)).Build();

     // Act
        var result = _validator.TestValidate(dto);

        // Assert
    result.ShouldHaveValidationErrorFor(x => x.SocialTags)
         .WithErrorMessage("Social tags must not exceed 500 characters");
    }

    [Fact]
    public void SocialTags_WhenValid_ShouldNotHaveValidationError()
    {
        // Arrange
        var dto = CreateNewsDtoBuilder.Create().WithSocialTags("#tech #news").Build();

    // Act
  var result = _validator.TestValidate(dto);

        // Assert
   result.ShouldNotHaveValidationErrorFor(x => x.SocialTags);
    }

    #endregion

    #region Summary Tests

    [Fact]
    public void Summary_WhenEmpty_ShouldHaveValidationError()
    {
 // Arrange
        var dto = CreateNewsDtoBuilder.Create().WithSummary(string.Empty).Build();

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Summary)
 .WithErrorMessage("Summary is required");
    }

    [Fact]
    public void Summary_WhenExceedsMaxLength_ShouldHaveValidationError()
    {
   // Arrange
 var dto = CreateNewsDtoBuilder.Create().WithSummary(new string('a', 2001)).Build();

        // Act
        var result = _validator.TestValidate(dto);

  // Assert
        result.ShouldHaveValidationErrorFor(x => x.Summary)
            .WithErrorMessage("Summary must not exceed 2000 characters");
    }

  [Fact]
    public void Summary_WhenValid_ShouldNotHaveValidationError()
    {
        // Arrange
    var dto = CreateNewsDtoBuilder.Create().WithSummary("Valid summary text").Build();

        // Act
        var result = _validator.TestValidate(dto);

  // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Summary);
    }

 #endregion

    #region Content Tests

    [Fact]
  public void Content_WhenEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var dto = CreateNewsDtoBuilder.Create().WithContent(string.Empty).Build();

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
   result.ShouldHaveValidationErrorFor(x => x.Content)
    .WithErrorMessage("Content is required");
    }

    [Fact]
    public void Content_WhenValid_ShouldNotHaveValidationError()
    {
        // Arrange
        var dto = CreateNewsDtoBuilder.Create().WithContent("Valid news content").Build();

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Content);
    }

    #endregion

    #region ExpressDate Tests

    [Fact]
    public void ExpressDate_WhenMinValue_ShouldHaveValidationError()
    {
      // Arrange
  var dto = CreateNewsDtoBuilder.Create().WithExpressDate(DateTime.MinValue).Build();

        // Act
   var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ExpressDate);
    }

    [Fact]
    public void ExpressDate_WhenValid_ShouldNotHaveValidationError()
    {
     // Arrange
var dto = CreateNewsDtoBuilder.Create().WithExpressDate(DateTime.UtcNow).Build();

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.ExpressDate);
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
        var dto = CreateNewsDtoBuilder.Create().WithPriority(priority).Build();

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
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
        var dto = CreateNewsDtoBuilder.Create().WithPriority(priority).Build();

 // Act
 var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Priority);
    }

    #endregion

    #region Url Tests

  [Fact]
    public void Url_WhenExceedsMaxLength_ShouldHaveValidationError()
    {
        // Arrange
        var dto = CreateNewsDtoBuilder.Create().WithUrl(new string('a', 501)).Build();

        // Act
        var result = _validator.TestValidate(dto);

   // Assert
        result.ShouldHaveValidationErrorFor(x => x.Url)
            .WithErrorMessage("URL must not exceed 500 characters");
    }

    [Fact]
    public void Url_WhenValid_ShouldNotHaveValidationError()
    {
      // Arrange
        var dto = CreateNewsDtoBuilder.Create().WithUrl("valid-url-slug").Build();

        // Act
        var result = _validator.TestValidate(dto);

  // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Url);
    }

    #endregion

    #region Image Tests

    [Fact]
    public void ImgPath_WhenExceedsMaxLength_ShouldHaveValidationError()
    {
        // Arrange
        var dto = CreateNewsDtoBuilder.Create().WithImagePath(new string('a', 501)).Build();

    // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ImgPath)
      .WithErrorMessage("Image path must not exceed 500 characters");
    }

    [Fact]
    public void ImgAlt_WhenExceedsMaxLength_ShouldHaveValidationError()
    {
        // Arrange
        var dto = CreateNewsDtoBuilder.Create().WithImageAlt(new string('a', 201)).Build();

      // Act
        var result = _validator.TestValidate(dto);

        // Assert
    result.ShouldHaveValidationErrorFor(x => x.ImgAlt)
   .WithErrorMessage("Image alt text must not exceed 200 characters");
    }

    #endregion

    #region Complete DTO Tests

 [Fact]
    public void CompleteValidDto_ShouldNotHaveAnyValidationErrors()
    {
    // Arrange
      var dto = CreateNewsDtoBuilder.Create().AsValidTechnologyNews().Build();

        // Act
    var result = _validator.TestValidate(dto);

      // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void InvalidDto_WithMultipleErrors_ShouldHaveMultipleValidationErrors()
    {
        // Arrange
   var dto = CreateNewsDtoBuilder.Create().AsInvalidDto().Build();

        // Act
var result = _validator.TestValidate(dto);

   // Assert
        result.ShouldHaveValidationErrorFor(x => x.Category);
     result.ShouldHaveValidationErrorFor(x => x.Type);
        result.ShouldHaveValidationErrorFor(x => x.Caption);
   result.ShouldHaveValidationErrorFor(x => x.Summary);
        result.ShouldHaveValidationErrorFor(x => x.Content);
    }

    #endregion
}
