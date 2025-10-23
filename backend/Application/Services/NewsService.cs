using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using MongoDB.Bson;
using NewsApi.Common;
using NewsApi.Domain.Entities;
using NewsApi.Domain.Interfaces;

namespace NewsApi.Application.Services;

public class NewsService : INewsService
{
    private readonly INewsRepository _newsRepository;
    private readonly IMemoryCache _cache;

    public NewsService(INewsRepository newsRepository, IMemoryCache memoryCache)
    {
        _newsRepository = newsRepository;
        _cache = memoryCache;
    }

    public async Task<List<News>> GetAllNewsAsync()
    {
        if (_cache.TryGetValue(CacheKeys.NewsList, out List<News>? cachedNews) && cachedNews != null)
        {
            return cachedNews;
        }

        var news = await _newsRepository.GetAllAsync().ConfigureAwait(false);
        _cache.Set(CacheKeys.NewsList, news, TimeSpan.FromMinutes(30));

        return news;
    }

    public async Task<News?> GetNewsByIdAsync(string id)
    {
        if (_cache.TryGetValue(id, out News? cachedNews) && cachedNews != null)
        {
            return cachedNews;
        }

        var news = await _newsRepository.GetByIdAsync(id).ConfigureAwait(false);
        if (news != null)
        {
            _cache.Set(id, news, TimeSpan.FromMinutes(30));
        }

        return news;
    }

    public async Task<News?> GetNewsByUrlAsync(string url)
    {
        return await _newsRepository.GetByUrlAsync(url).ConfigureAwait(false);
    }

    public async Task<News> CreateNewsAsync(News news)
    {
        news.Id = ObjectId.GenerateNewId().ToString();
        news.CreateDate = DateTime.UtcNow;
        news.UpdateDate = DateTime.UtcNow;

        var createdNews = await _newsRepository.CreateAsync(news).ConfigureAwait(false);

        // Invalidate cache
        _cache.Remove(CacheKeys.NewsList);

        return createdNews;
    }

    public async Task UpdateNewsAsync(string id, News news)
    {
        news.UpdateDate = DateTime.UtcNow;
        await _newsRepository.UpdateAsync(id, news).ConfigureAwait(false);

        // Invalidate cache
        _cache.Remove(CacheKeys.NewsList);
        _cache.Remove(id);
    }

    public async Task DeleteNewsAsync(string id)
    {
        await _newsRepository.DeleteAsync(id).ConfigureAwait(false);

        // Invalidate cache
        _cache.Remove(CacheKeys.NewsList);
        _cache.Remove(id);
    }
}
