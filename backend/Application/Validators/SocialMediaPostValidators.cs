using System;
using System.Linq;
using FluentValidation;
using NewsApi.Application.DTOs;

namespace NewsApi.Application.Validators;

/// <summary>
/// Validator for CreateSocialMediaPostDto
/// </summary>
internal sealed class CreateSocialMediaPostDtoValidator : AbstractValidator<CreateSocialMediaPostDto>
{
    private static readonly string[] ValidPlatforms =
    {
        "Reddit", "Twitter", "LinkedIn", "Facebook", "Instagram", "YouTube",
    };

    public CreateSocialMediaPostDtoValidator()
    {
        RuleFor(x => x.Platform)
            .NotEmpty().WithMessage("Platform is required")
            .Must(p => ValidPlatforms.Contains(p, StringComparer.Ordinal)).WithMessage($"Platform must be one of: {string.Join(", ", ValidPlatforms)}");

        RuleFor(x => x.ExternalId)
            .NotEmpty().WithMessage("External ID is required")
            .MaximumLength(200).WithMessage("External ID cannot exceed 200 characters");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(500).WithMessage("Title cannot exceed 500 characters");

        RuleFor(x => x.Content)
            .MaximumLength(5000).WithMessage("Content cannot exceed 5000 characters");

        RuleFor(x => x.Author)
            .NotEmpty().WithMessage("Author is required")
            .MaximumLength(200).WithMessage("Author cannot exceed 200 characters");

        RuleFor(x => x.AuthorUrl)
            .MaximumLength(1000).WithMessage("Author URL cannot exceed 1000 characters");

        RuleFor(x => x.PostUrl)
            .NotEmpty().WithMessage("Post URL is required")
            .MaximumLength(1000).WithMessage("Post URL cannot exceed 1000 characters");

        RuleFor(x => x.Category)
            .MaximumLength(200).WithMessage("Category cannot exceed 200 characters");

        RuleFor(x => x.PostedAt)
            .NotEmpty().WithMessage("Posted date is required")
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Posted date cannot be in the future");

        RuleFor(x => x.Upvotes)
            .GreaterThanOrEqualTo(0).WithMessage("Upvotes must be non-negative");

        RuleFor(x => x.Downvotes)
            .GreaterThanOrEqualTo(0).WithMessage("Downvotes must be non-negative");

        RuleFor(x => x.CommentCount)
            .GreaterThanOrEqualTo(0).WithMessage("Comment count must be non-negative");

        RuleFor(x => x.ShareCount)
            .GreaterThanOrEqualTo(0).WithMessage("Share count must be non-negative");

        RuleFor(x => x.Priority)
            .InclusiveBetween(0, 100).WithMessage("Priority must be between 0 and 100");

        RuleFor(x => x.Language)
            .NotEmpty().WithMessage("Language is required")
            .Length(2).WithMessage("Language must be a 2-character ISO code");
    }
}

/// <summary>
/// Validator for UpdateSocialMediaPostDto
/// </summary>
internal sealed class UpdateSocialMediaPostDtoValidator : AbstractValidator<UpdateSocialMediaPostDto>
{
    public UpdateSocialMediaPostDtoValidator()
    {
        When(x => x.Title != null, () =>
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title cannot be empty")
                .MaximumLength(500).WithMessage("Title cannot exceed 500 characters");
        });

        When(x => x.Content != null, () =>
        {
            RuleFor(x => x.Content)
                .MaximumLength(5000).WithMessage("Content cannot exceed 5000 characters");
        });

        When(x => x.Upvotes != null, () =>
        {
            RuleFor(x => x.Upvotes)
                .GreaterThanOrEqualTo(0).WithMessage("Upvotes must be non-negative");
        });

        When(x => x.Downvotes != null, () =>
        {
            RuleFor(x => x.Downvotes)
                .GreaterThanOrEqualTo(0).WithMessage("Downvotes must be non-negative");
        });

        When(x => x.CommentCount != null, () =>
        {
            RuleFor(x => x.CommentCount)
                .GreaterThanOrEqualTo(0).WithMessage("Comment count must be non-negative");
        });

        When(x => x.ShareCount != null, () =>
        {
            RuleFor(x => x.ShareCount)
                .GreaterThanOrEqualTo(0).WithMessage("Share count must be non-negative");
        });

        When(x => x.Priority != null, () =>
        {
            RuleFor(x => x.Priority)
                .InclusiveBetween(0, 100).WithMessage("Priority must be between 0 and 100");
        });
    }
}
