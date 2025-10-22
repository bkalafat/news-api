using System;
using FluentValidation;
using NewsApi.Application.DTOs;

namespace NewsApi.Application.Validators;

public class CreateNewsDtoValidator : AbstractValidator<CreateNewsDto>
{
    public CreateNewsDtoValidator()
    {
        RuleFor(x => x.Category)
            .NotEmpty()
            .WithMessage("Category is required")
            .MaximumLength(100)
            .WithMessage("Category must not exceed 100 characters");

        RuleFor(x => x.Type)
            .NotEmpty()
            .WithMessage("Type is required")
            .MaximumLength(50)
            .WithMessage("Type must not exceed 50 characters");

        RuleFor(x => x.Caption)
            .NotEmpty()
            .WithMessage("Caption is required")
            .MaximumLength(500)
            .WithMessage("Caption must not exceed 500 characters");

        RuleFor(x => x.Keywords).MaximumLength(1000).WithMessage("Keywords must not exceed 1000 characters");

        RuleFor(x => x.SocialTags).MaximumLength(500).WithMessage("Social tags must not exceed 500 characters");

        RuleFor(x => x.Summary)
            .NotEmpty()
            .WithMessage("Summary is required")
            .MaximumLength(2000)
            .WithMessage("Summary must not exceed 2000 characters");

        RuleFor(x => x.ImgPath).MaximumLength(500).WithMessage("Image path must not exceed 500 characters");

        RuleFor(x => x.ImgAlt).MaximumLength(200).WithMessage("Image alt text must not exceed 200 characters");

        RuleFor(x => x.Content).NotEmpty().WithMessage("Content is required");

        RuleFor(x => x.ExpressDate)
            .NotEmpty()
            .WithMessage("Express date is required")
            .GreaterThan(DateTime.MinValue)
            .WithMessage("Express date must be valid");

        RuleFor(x => x.Priority).InclusiveBetween(1, 100).WithMessage("Priority must be between 1 and 100");

        RuleFor(x => x.Url).MaximumLength(500).WithMessage("URL must not exceed 500 characters");
    }
}

public class UpdateNewsDtoValidator : AbstractValidator<UpdateNewsDto>
{
    public UpdateNewsDtoValidator()
    {
        RuleFor(x => x.Category)
            .MaximumLength(100)
            .WithMessage("Category must not exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.Category));

        RuleFor(x => x.Type)
            .MaximumLength(50)
            .WithMessage("Type must not exceed 50 characters")
            .When(x => !string.IsNullOrEmpty(x.Type));

        RuleFor(x => x.Caption)
            .MaximumLength(500)
            .WithMessage("Caption must not exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.Caption));

        RuleFor(x => x.Keywords)
            .MaximumLength(1000)
            .WithMessage("Keywords must not exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.Keywords));

        RuleFor(x => x.SocialTags)
            .MaximumLength(500)
            .WithMessage("Social tags must not exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.SocialTags));

        RuleFor(x => x.Summary)
            .MaximumLength(2000)
            .WithMessage("Summary must not exceed 2000 characters")
            .When(x => !string.IsNullOrEmpty(x.Summary));

        RuleFor(x => x.ImgPath)
            .MaximumLength(500)
            .WithMessage("Image path must not exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.ImgPath));

        RuleFor(x => x.ImgAlt)
            .MaximumLength(200)
            .WithMessage("Image alt text must not exceed 200 characters")
            .When(x => !string.IsNullOrEmpty(x.ImgAlt));

        RuleFor(x => x.ExpressDate)
            .GreaterThan(DateTime.MinValue)
            .WithMessage("Express date must be valid")
            .When(x => x.ExpressDate.HasValue);

        RuleFor(x => x.Priority)
            .InclusiveBetween(1, 100)
            .WithMessage("Priority must be between 1 and 100")
            .When(x => x.Priority.HasValue);

        RuleFor(x => x.Url)
            .MaximumLength(500)
            .WithMessage("URL must not exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.Url));
    }
}
