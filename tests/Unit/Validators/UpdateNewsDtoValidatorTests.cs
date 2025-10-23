using FluentValidation.TestHelper;
using NewsApi.Application.DTOs;
using NewsApi.Application.Validators;
using NewsApi.Tests.Helpers;

namespace NewsApi.Tests.Unit.Validators;

public class UpdateNewsDtoValidatorTests
{
    private readonly UpdateNewsDtoValidator _validator;

    public UpdateNewsDtoValidatorTests()
    {
        _validator = new UpdateNewsDtoValidator();
    }

    [Fact]
    public void Category_WhenExceedsMaxLength_ShouldHaveValidationError()
    {
        var dto = UpdateNewsDtoBuilder.Create().WithCategory(new string('a', 101)).Build();
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Category);
    }

    [Fact]
    public void Category_WhenValidLength_ShouldNotHaveValidationError()
    {
        var dto = UpdateNewsDtoBuilder.Create().WithCategory("Technology").Build();
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.Category);
    }

    [Fact]
    public void Category_WhenNull_ShouldNotHaveValidationError()
    {
        var dto = new UpdateNewsDto { Category = null };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.Category);
    }

    [Fact]
    public void Type_WhenExceedsMaxLength_ShouldHaveValidationError()
    {
        var dto = UpdateNewsDtoBuilder.Create().WithSummary(new string('a', 2001)).Build();
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Summary);
    }

    [Fact]
    public void Caption_WhenExceedsMaxLength_ShouldHaveValidationError()
    {
        var dto = UpdateNewsDtoBuilder.Create().WithCaption(new string('a', 501)).Build();
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Caption);
    }

    [Fact]
    public void Priority_WhenOutOfRange_ShouldHaveValidationError()
    {
        var dto = new UpdateNewsDto { Priority = 101 };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Priority);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(50)]
    [InlineData(100)]
    public void Priority_WhenInRange_ShouldNotHaveValidationError(int priority)
    {
        var dto = new UpdateNewsDto { Priority = priority };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.Priority);
    }

    [Fact]
    public void Priority_WhenNull_ShouldNotHaveValidationError()
    {
        var dto = new UpdateNewsDto { Priority = null };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.Priority);
    }

    [Fact]
    public void ExpressDate_WhenMinValue_ShouldHaveValidationError()
    {
        var dto = new UpdateNewsDto { ExpressDate = DateTime.MinValue };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.ExpressDate);
    }

    [Fact]
    public void ExpressDate_WhenValid_ShouldNotHaveValidationError()
    {
        var dto = new UpdateNewsDto { ExpressDate = DateTime.UtcNow };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.ExpressDate);
    }

    [Fact]
    public void CompleteValidDto_ShouldNotHaveAnyValidationErrors()
    {
        var dto = UpdateNewsDtoBuilder.Create().Build();
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void AllFieldsNull_ShouldNotHaveValidationErrors()
    {
        var dto = new UpdateNewsDto();
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
