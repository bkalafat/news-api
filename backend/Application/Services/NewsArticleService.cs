using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using MongoDB.Bson;
using NewsApi.Common;
using NewsApi.Domain.Entities;
using NewsApi.Domain.Interfaces;

namespace NewsApi.Application.Services;

/// <summary>
/// Service for managing news articles with caching support.
/// </summary>
internal sealed class NewsArticleService(INewsArticleRepository newsArticleRepository, IMemoryCache memoryCache)
    : INewsArticleService
{
    public async Task<List<NewsArticle>> GetAllNewsAsync()
    {
        if (memoryCache.TryGetValue(CacheKeys.NewsList, out List<NewsArticle>? cachedNews) && cachedNews is not null)
        {
            return cachedNews;
        }

        var news = await newsArticleRepository.GetAllAsync().ConfigureAwait(false);
        memoryCache.Set(CacheKeys.NewsList, news, TimeSpan.FromMinutes(30));

        return news;
    }

    public async Task<NewsArticle?> GetNewsByIdAsync(string id)
    {
        if (memoryCache.TryGetValue(id, out NewsArticle? cachedNews) && cachedNews is not null)
        {
            return cachedNews;
        }

        var news = await newsArticleRepository.GetByIdAsync(id).ConfigureAwait(false);
        if (news is not null)
        {
            memoryCache.Set(id, news, TimeSpan.FromMinutes(30));
        }

        return news;
    }

    public Task<NewsArticle?> GetNewsBySlugAsync(string slug) =>
        newsArticleRepository.GetBySlugAsync(slug);

    public async Task<NewsArticle> CreateNewsAsync(NewsArticle newsArticle)
    {
        newsArticle.Id = ObjectId.GenerateNewId().ToString();
        newsArticle.CreateDate = DateTime.UtcNow;
        newsArticle.UpdateDate = DateTime.UtcNow;

        var createdNews = await newsArticleRepository.CreateAsync(newsArticle).ConfigureAwait(false);

        // Invalidate cache
        InvalidateNewsCache(newsArticle.Id);

        return createdNews;
    }

    public async Task UpdateNewsAsync(string id, NewsArticle newsArticle)
    {
        newsArticle.UpdateDate = DateTime.UtcNow;
        await newsArticleRepository.UpdateAsync(id, newsArticle).ConfigureAwait(false);

        // Invalidate cache
        InvalidateNewsCache(id);
    }

    public async Task DeleteNewsAsync(string id)
    {
        await newsArticleRepository.DeleteAsync(id).ConfigureAwait(false);

        // Invalidate cache
        InvalidateNewsCache(id);
    }

    /// <summary>
    /// Invalidates all news-related cache entries.
    /// </summary>
    /// <param name="newsId">The ID of the specific news article to invalidate (optional).</param>
    private static void InvalidateNewsCache(string? newsId = null)
    {
        memoryCache.Remove(CacheKeys.NewsList);
        if (!string.IsNullOrEmpty(newsId))
        {
            memoryCache.Remove(newsId);
        }
    }
}
