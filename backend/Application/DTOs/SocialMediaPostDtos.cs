using System;

namespace NewsApi.Application.DTOs;

/// <summary>
/// DTO for creating a new social media post
/// </summary>
internal record CreateSocialMediaPostDto
{
    public string Platform { get; init; } = string.Empty;

    public string ExternalId { get; init; } = string.Empty;

    public string Title { get; init; } = string.Empty;

    public string Content { get; init; } = string.Empty;

    public string Author { get; init; } = string.Empty;

    public string AuthorUrl { get; init; } = string.Empty;

    public string AuthorAvatar { get; init; } = string.Empty;

    public string PostUrl { get; init; } = string.Empty;

    public string[] ImageUrls { get; init; } = [];

    public string? VideoUrl { get; init; }

    public int Upvotes { get; init; }

    public int Downvotes { get; init; }

    public int CommentCount { get; init; }

    public int ShareCount { get; init; }

    public string[] Tags { get; init; } = [];

    public string Category { get; init; } = string.Empty;

    public DateTime PostedAt { get; init; }

    public int Priority { get; init; }

    public string Language { get; init; } = "en";
}

/// <summary>
/// DTO for updating a social media post
/// </summary>
internal record UpdateSocialMediaPostDto
{
    public string? Title { get; init; }

    public string? Content { get; init; }

    public string[]? ImageUrls { get; init; }

    public string? VideoUrl { get; init; }

    public int? Upvotes { get; init; }

    public int? Downvotes { get; init; }

    public int? CommentCount { get; init; }

    public int? ShareCount { get; init; }

    public string[]? Tags { get; init; }

    public int? Priority { get; init; }

    public bool? IsActive { get; init; }
}

/// <summary>
/// DTO for social media post responses
/// </summary>
internal record SocialMediaPostDto
{
    public string Id { get; init; } = string.Empty;

    public string Platform { get; init; } = string.Empty;

    public string ExternalId { get; init; } = string.Empty;

    public string Title { get; init; } = string.Empty;

    public string Content { get; init; } = string.Empty;

    public string Author { get; init; } = string.Empty;

    public string AuthorUrl { get; init; } = string.Empty;

    public string AuthorAvatar { get; init; } = string.Empty;

    public string PostUrl { get; init; } = string.Empty;

    public string[] ImageUrls { get; init; } = [];

    public string? VideoUrl { get; init; }

    public int Upvotes { get; init; }

    public int Downvotes { get; init; }

    public int CommentCount { get; init; }

    public int ShareCount { get; init; }

    public string[] Tags { get; init; } = [];

    public string Category { get; init; } = string.Empty;

    public DateTime PostedAt { get; init; }

    public DateTime FetchedAt { get; init; }

    public DateTime? LastUpdated { get; init; }

    public bool IsActive { get; init; }

    public int Priority { get; init; }

    public string Language { get; init; } = string.Empty;
}
