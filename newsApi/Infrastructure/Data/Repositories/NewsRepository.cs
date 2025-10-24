using MongoDB.Bson;
using MongoDB.Driver;
using NewsApi.Domain.Entities;
using NewsApi.Domain.Interfaces;
using NewsApi.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NewsApi.Infrastructure.Data.Repositories;

public class NewsRepository : INewsRepository
{
    private readonly IMongoCollection<News> _newsCollection;

    public NewsRepository(MongoDbContext context)
    {
        _newsCollection = context.News;
    }

    public async Task<List<News>> GetAllAsync()
    {
        return await _newsCollection
            .Find(news => news.IsActive)
            .SortByDescending(news => news.ExpressDate)
            .ToListAsync();
    }

    public async Task<News?> GetByIdAsync(string id)
    {
        return await _newsCollection
            .Find(news => news.Id == id && news.IsActive)
            .FirstOrDefaultAsync();
    }

    public async Task<News?> GetByUrlAsync(string url)
    {
        return await _newsCollection
            .Find(news => news.Url == url && news.IsActive)
            .FirstOrDefaultAsync();
    }

    public async Task<News> CreateAsync(News news)
    {
        await _newsCollection.InsertOneAsync(news);
        return news;
    }

    public async Task UpdateAsync(string id, News news)
    {
        await _newsCollection.ReplaceOneAsync(
            filter: n => n.Id == id,
            replacement: news
        );
    }

    public async Task DeleteAsync(string id)
    {
        await _newsCollection.DeleteOneAsync(news => news.Id == id);
    }
}