using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using NewsApi.Domain.Entities;
using NewsApi.Domain.Interfaces;

namespace NewsApi.Infrastructure.Data.Repositories;

/// <summary>
/// MongoDB repository for news articles with optimized queries.
/// </summary>
public sealed class NewsArticleRepository(MongoDbContext context) : INewsArticleRepository
{
    private readonly IMongoCollection<NewsArticle> _newsCollection = context.News;

    public async Task<List<NewsArticle>> GetAllAsync() =>
        await _newsCollection
            .Find(news => news.IsActive)
            .SortByDescending(news => news.ExpressDate)
            .ToListAsync()
            .ConfigureAwait(false);

    public async Task<NewsArticle?> GetByIdAsync(string id)
    {
        if (!ObjectId.TryParse(id, out _))
        {
            return null;
        }

        try
        {
            return await _newsCollection
                .Find(news => news.Id == id && news.IsActive)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            // Defensive: log to console during tests to aid debugging
            Console.WriteLine($"NewsArticleRepository.GetByIdAsync error for id={id}: {ex}");
            return null;
        }
    }

    public async Task<NewsArticle?> GetBySlugAsync(string slug) =>
        await _newsCollection
            .Find(news => news.Slug == slug && news.IsActive)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);

    public async Task<NewsArticle> CreateAsync(NewsArticle newsArticle)
    {
        await _newsCollection.InsertOneAsync(newsArticle).ConfigureAwait(false);
        return newsArticle;
    }

    public async Task UpdateAsync(string id, NewsArticle newsArticle) =>
        await _newsCollection.ReplaceOneAsync(filter: n => n.Id == id, replacement: newsArticle).ConfigureAwait(false);

    public async Task DeleteAsync(string id) =>
        await _newsCollection.DeleteOneAsync(news => news.Id == id).ConfigureAwait(false);
}
