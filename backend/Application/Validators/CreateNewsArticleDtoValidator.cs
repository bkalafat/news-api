using System;
using System.Linq;
using FluentValidation;
using NewsApi.Application.DTOs;

namespace NewsApi.Application.Validators;

public class CreateNewsArticleDtoValidator : AbstractValidator<CreateNewsArticleDto>
{
    public CreateNewsArticleDtoValidator()
    {
        var allowedCategories = new[] { "reddit", "github", "twitter", "linkedin", "facebook", "instagram", "tiktok", "youtube", "technology" };

        RuleFor(dto => dto.Category)
            .NotEmpty()
            .WithMessage("Category is required")
            .MaximumLength(100)
            .WithMessage("Category must not exceed 100 characters")
            .Must(c => !string.IsNullOrEmpty(c) && allowedCategories.Contains(c.ToLower()))
            .WithMessage($"Category must be one of: {string.Join(", ", allowedCategories)}");

        RuleFor(dto => dto.Type)
            .NotEmpty()
            .WithMessage("Type is required")
            .MaximumLength(50)
            .WithMessage("Type must not exceed 50 characters");

        RuleFor(dto => dto.Caption)
            .NotEmpty()
            .WithMessage("Caption is required")
            .MaximumLength(500)
            .WithMessage("Caption must not exceed 500 characters");

        RuleFor(dto => dto.Keywords).MaximumLength(1000).WithMessage("Keywords must not exceed 1000 characters");

        RuleFor(dto => dto.SocialTags).MaximumLength(500).WithMessage("Social tags must not exceed 500 characters");

        RuleFor(dto => dto.Summary)
            .NotEmpty()
            .WithMessage("Summary is required")
            .MaximumLength(2000)
            .WithMessage("Summary must not exceed 2000 characters");

        RuleFor(dto => dto.ImgPath).MaximumLength(500).WithMessage("Image path must not exceed 500 characters");

        RuleFor(dto => dto.ImgAlt).MaximumLength(200).WithMessage("Image alt text must not exceed 200 characters");

        RuleFor(dto => dto.Content).NotEmpty().WithMessage("Content is required");

        RuleFor(dto => dto.ExpressDate)
            .NotEmpty()
            .WithMessage("Express date is required")
            .GreaterThan(DateTime.MinValue)
            .WithMessage("Express date must be valid");

        RuleFor(dto => dto.Priority).InclusiveBetween(1, 100).WithMessage("Priority must be between 1 and 100");
    }
}

public class UpdateNewsArticleDtoValidator : AbstractValidator<UpdateNewsArticleDto>
{
    public UpdateNewsArticleDtoValidator()
    {
        var allowedCategories = new[] { "reddit", "github", "twitter", "linkedin", "facebook", "instagram", "tiktok", "youtube", "technology" };

        RuleFor(dto => dto.Category)
            .MaximumLength(100)
            .WithMessage("Category must not exceed 100 characters")
            .Must(c => !string.IsNullOrEmpty(c) && allowedCategories.Contains(c.ToLower()))
            .WithMessage($"Category must be one of: {string.Join(", ", allowedCategories)}")
            .When(dto => !string.IsNullOrEmpty(dto.Category));

        RuleFor(dto => dto.Type)
            .MaximumLength(50)
            .WithMessage("Type must not exceed 50 characters")
            .When(dto => !string.IsNullOrEmpty(dto.Type));

        RuleFor(dto => dto.Caption)
            .MaximumLength(500)
            .WithMessage("Caption must not exceed 500 characters")
            .When(dto => !string.IsNullOrEmpty(dto.Caption));

        RuleFor(dto => dto.Keywords)
            .MaximumLength(1000)
            .WithMessage("Keywords must not exceed 1000 characters")
            .When(dto => !string.IsNullOrEmpty(dto.Keywords));

        RuleFor(dto => dto.SocialTags)
            .MaximumLength(500)
            .WithMessage("Social tags must not exceed 500 characters")
            .When(dto => !string.IsNullOrEmpty(dto.SocialTags));

        RuleFor(dto => dto.Summary)
            .MaximumLength(2000)
            .WithMessage("Summary must not exceed 2000 characters")
            .When(dto => !string.IsNullOrEmpty(dto.Summary));

        RuleFor(dto => dto.ImgPath)
            .MaximumLength(500)
            .WithMessage("Image path must not exceed 500 characters")
            .When(dto => !string.IsNullOrEmpty(dto.ImgPath));

        RuleFor(dto => dto.ImgAlt)
            .MaximumLength(200)
            .WithMessage("Image alt text must not exceed 200 characters")
            .When(dto => !string.IsNullOrEmpty(dto.ImgAlt));

        RuleFor(dto => dto.ExpressDate)
            .GreaterThan(DateTime.MinValue)
            .WithMessage("Express date must be valid")
            .When(dto => dto.ExpressDate.HasValue);

        RuleFor(dto => dto.Priority)
            .InclusiveBetween(1, 100)
            .WithMessage("Priority must be between 1 and 100")
            .When(dto => dto.Priority.HasValue);
    }
}
