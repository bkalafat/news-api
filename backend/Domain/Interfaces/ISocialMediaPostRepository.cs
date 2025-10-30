using System.Collections.Generic;
using System.Threading.Tasks;
using NewsApi.Domain.Entities;

namespace NewsApi.Domain.Interfaces;

/// <summary>
/// Repository interface for social media posts
/// </summary>
public interface ISocialMediaPostRepository
{
    /// <summary>
    /// Gets all social media posts
    /// </summary>
    Task<List<SocialMediaPost>> GetAllAsync();

    /// <summary>
    /// Gets posts filtered by platform
    /// </summary>
    Task<List<SocialMediaPost>> GetByPlatformAsync(string platform);

    /// <summary>
    /// Gets posts filtered by category (subreddit, hashtag, etc.)
    /// </summary>
    Task<List<SocialMediaPost>> GetByCategoryAsync(string category);

    /// <summary>
    /// Gets top posts by upvotes/likes
    /// </summary>
    Task<List<SocialMediaPost>> GetTopPostsAsync(int limit, string? platform = null);

    /// <summary>
    /// Gets a post by ID
    /// </summary>
    Task<SocialMediaPost?> GetByIdAsync(string id);

    /// <summary>
    /// Gets a post by external ID and platform (to avoid duplicates)
    /// </summary>
    Task<SocialMediaPost?> GetByExternalIdAsync(string externalId, string platform);

    /// <summary>
    /// Creates a new social media post
    /// </summary>
    Task<SocialMediaPost> CreateAsync(SocialMediaPost post);

    /// <summary>
    /// Updates an existing post
    /// </summary>
    Task UpdateAsync(string id, SocialMediaPost post);

    /// <summary>
    /// Deletes a post
    /// </summary>
    Task DeleteAsync(string id);

    /// <summary>
    /// Updates post metrics (likes, comments, etc.)
    /// </summary>
    Task UpdateMetricsAsync(string id, int upvotes, int downvotes, int commentCount, int shareCount);
}
