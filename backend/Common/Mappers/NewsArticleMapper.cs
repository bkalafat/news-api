using NewsApi.Application.DTOs;
using NewsApi.Domain.Entities;

namespace NewsApi.Common.Mappers;

/// <summary>
/// Mapper for converting between NewsArticle entities and DTOs.
/// </summary>
public static class NewsArticleMapper
{
    /// <summary>
    /// Maps a CreateNewsArticleDto to a NewsArticle entity.
    /// </summary>
    /// <param name="dto">The DTO to map from.</param>
    /// <returns>A new NewsArticle entity with properties from the DTO.</returns>
    public static NewsArticle ToEntity(CreateNewsArticleDto dto)
    {
        return new NewsArticle
        {
            Category = dto.Category,
            Type = dto.Type,
            Caption = dto.Caption,
            Slug = SlugHelper.GenerateSlug(dto.Caption),
            Keywords = dto.Keywords,
            SocialTags = dto.SocialTags,
            Summary = dto.Summary,
            ImgPath = dto.ImgPath,
            ImgAlt = dto.ImgAlt,
            ImageUrl = dto.ImageUrl,
            ThumbnailUrl = dto.ThumbnailUrl,
            Content = dto.Content,
            Subjects = dto.Subjects,
            Authors = dto.Authors,
            ExpressDate = dto.ExpressDate,
            Priority = dto.Priority,
            IsActive = dto.IsActive,
            IsSecondPageNews = dto.IsSecondPageNews,
        };
    }

    /// <summary>
    /// Updates an existing NewsArticle entity with non-null values from UpdateNewsArticleDto.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    /// <param name="dto">The DTO containing update values.</param>
    public static void UpdateFromDto(NewsArticle entity, UpdateNewsArticleDto dto)
    {
        if (dto.Category != null)
            entity.Category = dto.Category;

        if (dto.Type != null)
            entity.Type = dto.Type;

        if (dto.Caption != null)
        {
            entity.Caption = dto.Caption;
            entity.Slug = SlugHelper.GenerateSlug(dto.Caption);
        }

        if (dto.Keywords != null)
            entity.Keywords = dto.Keywords;

        if (dto.SocialTags != null)
            entity.SocialTags = dto.SocialTags;

        if (dto.Summary != null)
            entity.Summary = dto.Summary;

        if (dto.ImgPath != null)
            entity.ImgPath = dto.ImgPath;

        if (dto.ImgAlt != null)
            entity.ImgAlt = dto.ImgAlt;

        if (dto.Content != null)
            entity.Content = dto.Content;

        if (dto.Subjects != null)
            entity.Subjects = dto.Subjects;

        if (dto.Authors != null)
            entity.Authors = dto.Authors;

        if (dto.ExpressDate.HasValue)
            entity.ExpressDate = dto.ExpressDate.Value;

        if (dto.Priority.HasValue)
            entity.Priority = dto.Priority.Value;

        if (dto.IsActive.HasValue)
            entity.IsActive = dto.IsActive.Value;

        if (dto.IsSecondPageNews.HasValue)
            entity.IsSecondPageNews = dto.IsSecondPageNews.Value;

        if (dto.ImageUrl != null)
            entity.ImageUrl = dto.ImageUrl;

        if (dto.ThumbnailUrl != null)
            entity.ThumbnailUrl = dto.ThumbnailUrl;
    }
}
