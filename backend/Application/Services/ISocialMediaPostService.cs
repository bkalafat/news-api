using System.Collections.Generic;
using System.Threading.Tasks;
using NewsApi.Application.DTOs;
using NewsApi.Domain.Entities;

namespace NewsApi.Application.Services;

/// <summary>
/// Service interface for social media posts
/// </summary>
public interface ISocialMediaPostService
{
    Task<List<SocialMediaPostDto>> GetAllPostsAsync();
    Task<List<SocialMediaPostDto>> GetByPlatformAsync(string platform);
    Task<List<SocialMediaPostDto>> GetByCategoryAsync(string category);
    Task<List<SocialMediaPostDto>> GetTopPostsAsync(int limit, string? platform = null);
    Task<SocialMediaPostDto?> GetByIdAsync(string id);
    Task<SocialMediaPostDto> CreatePostAsync(CreateSocialMediaPostDto dto);
    Task UpdatePostAsync(string id, UpdateSocialMediaPostDto dto);
    Task DeletePostAsync(string id);
    Task UpdateMetricsAsync(string id, int upvotes, int downvotes, int commentCount, int shareCount);
}
