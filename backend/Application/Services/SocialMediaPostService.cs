using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using MongoDB.Bson;
using NewsApi.Application.DTOs;
using NewsApi.Domain.Entities;
using NewsApi.Domain.Interfaces;

namespace NewsApi.Application.Services;

/// <summary>
/// Service for managing social media posts with caching support
/// </summary>
public sealed class SocialMediaPostService : ISocialMediaPostService
{
    private readonly ISocialMediaPostRepository _repository;
    private readonly IMemoryCache _cache;
    private const string CacheKeyPrefix = "SocialMediaPosts_";
    private const string AllPostsCacheKey = "SocialMediaPosts_All";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(30);

    public SocialMediaPostService(ISocialMediaPostRepository repository, IMemoryCache cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task<List<SocialMediaPostDto>> GetAllPostsAsync()
    {
        if (_cache.TryGetValue(AllPostsCacheKey, out List<SocialMediaPostDto>? cachedPosts) && cachedPosts is not null)
        {
            return cachedPosts;
        }

        var posts = await _repository.GetAllAsync().ConfigureAwait(false);
        var dtos = posts.Select(MapToDto).ToList();
        _cache.Set(AllPostsCacheKey, dtos, CacheDuration);

        return dtos;
    }

    public async Task<List<SocialMediaPostDto>> GetByPlatformAsync(string platform)
    {
        var cacheKey = $"{CacheKeyPrefix}Platform_{platform}";
        if (_cache.TryGetValue(cacheKey, out List<SocialMediaPostDto>? cachedPosts) && cachedPosts is not null)
        {
            return cachedPosts;
        }

        var posts = await _repository.GetByPlatformAsync(platform).ConfigureAwait(false);
        var dtos = posts.Select(MapToDto).ToList();
        _cache.Set(cacheKey, dtos, CacheDuration);

        return dtos;
    }

    public async Task<List<SocialMediaPostDto>> GetByCategoryAsync(string category)
    {
        var cacheKey = $"{CacheKeyPrefix}Category_{category}";
        if (_cache.TryGetValue(cacheKey, out List<SocialMediaPostDto>? cachedPosts) && cachedPosts is not null)
        {
            return cachedPosts;
        }

        var posts = await _repository.GetByCategoryAsync(category).ConfigureAwait(false);
        var dtos = posts.Select(MapToDto).ToList();
        _cache.Set(cacheKey, dtos, CacheDuration);

        return dtos;
    }

    public async Task<List<SocialMediaPostDto>> GetTopPostsAsync(int limit, string? platform = null)
    {
        var cacheKey = platform == null
            ? $"{CacheKeyPrefix}Top_{limit}"
            : $"{CacheKeyPrefix}Top_{platform}_{limit}";

        if (_cache.TryGetValue(cacheKey, out List<SocialMediaPostDto>? cachedPosts) && cachedPosts is not null)
        {
            return cachedPosts;
        }

        var posts = await _repository.GetTopPostsAsync(limit, platform).ConfigureAwait(false);
        var dtos = posts.Select(MapToDto).ToList();
        _cache.Set(cacheKey, dtos, CacheDuration);

        return dtos;
    }

    public async Task<SocialMediaPostDto?> GetByIdAsync(string id)
    {
        var cacheKey = $"{CacheKeyPrefix}{id}";
        if (_cache.TryGetValue(cacheKey, out SocialMediaPostDto? cachedPost) && cachedPost is not null)
        {
            return cachedPost;
        }

        var post = await _repository.GetByIdAsync(id).ConfigureAwait(false);
        if (post is null)
        {
            return null;
        }

        var dto = MapToDto(post);
        _cache.Set(cacheKey, dto, CacheDuration);

        return dto;
    }

    public async Task<SocialMediaPostDto> CreatePostAsync(CreateSocialMediaPostDto dto)
    {
        // Check if post already exists
        var existing = await _repository.GetByExternalIdAsync(dto.ExternalId, dto.Platform).ConfigureAwait(false);
        if (existing is not null)
        {
            throw new InvalidOperationException($"Post with external ID '{dto.ExternalId}' from {dto.Platform} already exists");
        }

        var post = new SocialMediaPost
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Platform = dto.Platform,
            ExternalId = dto.ExternalId,
            Title = dto.Title,
            Content = dto.Content,
            Author = dto.Author,
            AuthorUrl = dto.AuthorUrl,
            AuthorAvatar = dto.AuthorAvatar,
            PostUrl = dto.PostUrl,
            ImageUrls = dto.ImageUrls,
            VideoUrl = dto.VideoUrl,
            Upvotes = dto.Upvotes,
            Downvotes = dto.Downvotes,
            CommentCount = dto.CommentCount,
            ShareCount = dto.ShareCount,
            Tags = dto.Tags,
            Category = dto.Category,
            PostedAt = dto.PostedAt,
            FetchedAt = DateTime.UtcNow,
            IsActive = true,
            Priority = dto.Priority,
            Language = dto.Language
        };

        var created = await _repository.CreateAsync(post).ConfigureAwait(false);
        InvalidateCache();

        return MapToDto(created);
    }

    public async Task UpdatePostAsync(string id, UpdateSocialMediaPostDto dto)
    {
        var existing = await _repository.GetByIdAsync(id).ConfigureAwait(false);
        if (existing is null)
        {
            throw new InvalidOperationException($"Post with ID '{id}' not found");
        }

        // Update only provided fields
        if (dto.Title is not null) existing.Title = dto.Title;
        if (dto.Content is not null) existing.Content = dto.Content;
        if (dto.ImageUrls is not null) existing.ImageUrls = dto.ImageUrls;
        if (dto.VideoUrl is not null) existing.VideoUrl = dto.VideoUrl;
        if (dto.Upvotes.HasValue) existing.Upvotes = dto.Upvotes.Value;
        if (dto.Downvotes.HasValue) existing.Downvotes = dto.Downvotes.Value;
        if (dto.CommentCount.HasValue) existing.CommentCount = dto.CommentCount.Value;
        if (dto.ShareCount.HasValue) existing.ShareCount = dto.ShareCount.Value;
        if (dto.Tags is not null) existing.Tags = dto.Tags;
        if (dto.Priority.HasValue) existing.Priority = dto.Priority.Value;
        if (dto.IsActive.HasValue) existing.IsActive = dto.IsActive.Value;

        existing.LastUpdated = DateTime.UtcNow;

        await _repository.UpdateAsync(id, existing).ConfigureAwait(false);
        InvalidateCache(id);
    }

    public async Task DeletePostAsync(string id)
    {
        await _repository.DeleteAsync(id).ConfigureAwait(false);
        InvalidateCache(id);
    }

    public async Task UpdateMetricsAsync(string id, int upvotes, int downvotes, int commentCount, int shareCount)
    {
        await _repository.UpdateMetricsAsync(id, upvotes, downvotes, commentCount, shareCount).ConfigureAwait(false);
        InvalidateCache(id);
    }

    private void InvalidateCache(string? postId = null)
    {
        _cache.Remove(AllPostsCacheKey);
        if (!string.IsNullOrEmpty(postId))
        {
            _cache.Remove($"{CacheKeyPrefix}{postId}");
        }
    }

    private static SocialMediaPostDto MapToDto(SocialMediaPost post) => new()
    {
        Id = post.Id,
        Platform = post.Platform,
        ExternalId = post.ExternalId,
        Title = post.Title,
        Content = post.Content,
        Author = post.Author,
        AuthorUrl = post.AuthorUrl,
        AuthorAvatar = post.AuthorAvatar,
        PostUrl = post.PostUrl,
        ImageUrls = post.ImageUrls,
        VideoUrl = post.VideoUrl,
        Upvotes = post.Upvotes,
        Downvotes = post.Downvotes,
        CommentCount = post.CommentCount,
        ShareCount = post.ShareCount,
        Tags = post.Tags,
        Category = post.Category,
        PostedAt = post.PostedAt,
        FetchedAt = post.FetchedAt,
        LastUpdated = post.LastUpdated,
        IsActive = post.IsActive,
        Priority = post.Priority,
        Language = post.Language
    };
}
