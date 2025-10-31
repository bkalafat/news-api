using FluentValidation.TestHelper;
using NewsApi.Application.DTOs;
using NewsApi.Application.Validators;
using NewsApi.Tests.Helpers;

namespace NewsApi.Tests.Unit.Validators;

internal class UpdateNewsArticleDtoValidatorTests
{
    private readonly UpdateNewsArticleDtoValidator _validator;

    public UpdateNewsArticleDtoValidatorTests()
    {
        _validator = new UpdateNewsArticleDtoValidator();
    }

    [Fact]
    public void Category_WhenExceedsMaxLength_ShouldHaveValidationError()
    {
        var dto = UpdateNewsArticleDtoBuilder.Create().WithCategory(new string('a', 101)).Build();
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(dto => dto.Category);
    }

    [Fact]
    public void Category_WhenValidLength_ShouldNotHaveValidationError()
    {
        var dto = UpdateNewsArticleDtoBuilder.Create().WithCategory("Technology").Build();
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(dto => dto.Category);
    }

    [Fact]
    public void Category_WhenNull_ShouldNotHaveValidationError()
    {
        var dto = new UpdateNewsArticleDto { Category = null };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(dto => dto.Category);
    }

    [Fact]
    public void Type_WhenExceedsMaxLength_ShouldHaveValidationError()
    {
        var dto = UpdateNewsArticleDtoBuilder.Create().WithSummary(new string('a', 2001)).Build();
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(dto => dto.Summary);
    }

    [Fact]
    public void Caption_WhenExceedsMaxLength_ShouldHaveValidationError()
    {
        var dto = UpdateNewsArticleDtoBuilder.Create().WithCaption(new string('a', 501)).Build();
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(dto => dto.Caption);
    }

    [Fact]
    public void Priority_WhenOutOfRange_ShouldHaveValidationError()
    {
        var dto = new UpdateNewsArticleDto { Priority = 101 };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(dto => dto.Priority);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(50)]
    [InlineData(100)]
    public void Priority_WhenInRange_ShouldNotHaveValidationError(int priority)
    {
        var dto = new UpdateNewsArticleDto { Priority = priority };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(dto => dto.Priority);
    }

    [Fact]
    public void Priority_WhenNull_ShouldNotHaveValidationError()
    {
        var dto = new UpdateNewsArticleDto { Priority = null };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(dto => dto.Priority);
    }

    [Fact]
    public void ExpressDate_WhenMinValue_ShouldHaveValidationError()
    {
        var dto = new UpdateNewsArticleDto { ExpressDate = DateTime.MinValue };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(dto => dto.ExpressDate);
    }

    [Fact]
    public void ExpressDate_WhenValid_ShouldNotHaveValidationError()
    {
        var dto = new UpdateNewsArticleDto { ExpressDate = DateTime.UtcNow };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(dto => dto.ExpressDate);
    }

    [Fact]
    public void CompleteValidDto_ShouldNotHaveAnyValidationErrors()
    {
        var dto = UpdateNewsArticleDtoBuilder.Create().Build();
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void AllFieldsNull_ShouldNotHaveValidationErrors()
    {
        var dto = new UpdateNewsArticleDto();
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
