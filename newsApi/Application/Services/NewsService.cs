using Microsoft.Extensions.Caching.Memory;
using NewsApi.Application.Services;
using NewsApi.Common;
using NewsApi.Domain.Entities;
using NewsApi.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        var news = await _newsRepository.GetAllAsync();
        _cache.Set(CacheKeys.NewsList, news, TimeSpan.FromMinutes(30));
        return news;
    }

    public async Task<News?> GetNewsByIdAsync(Guid id)
    {
        if (_cache.TryGetValue(id, out News? cachedNews) && cachedNews != null)
        {
            return cachedNews;
        }

        var news = await _newsRepository.GetByIdAsync(id);
        if (news != null)
        {
            _cache.Set(id, news, TimeSpan.FromMinutes(30));
        }
        return news;
    }

    public async Task<News?> GetNewsByUrlAsync(string url)
    {
        return await _newsRepository.GetByUrlAsync(url);
    }

    public async Task<News> CreateNewsAsync(News news)
    {
        news.Id = Guid.NewGuid();
        news.CreateDate = DateTime.UtcNow;
        news.UpdateDate = DateTime.UtcNow;

        var createdNews = await _newsRepository.CreateAsync(news);
        
        // Invalidate cache
        _cache.Remove(CacheKeys.NewsList);
        
        return createdNews;
    }

    public async Task UpdateNewsAsync(Guid id, News news)
    {
        news.UpdateDate = DateTime.UtcNow;
        await _newsRepository.UpdateAsync(id, news);
        
        // Invalidate cache
        _cache.Remove(CacheKeys.NewsList);
        _cache.Remove(id);
    }

    public async Task DeleteNewsAsync(Guid id)
    {
        await _newsRepository.DeleteAsync(id);
        
        // Invalidate cache
        _cache.Remove(CacheKeys.NewsList);
        _cache.Remove(id);
    }
}