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
internal sealed class NewsArticleRepository(MongoDbContext context) : INewsArticleRepository
{
    private readonly IMongoCollection<NewsArticle> _newsCollection = context.News;

    public Task<List<NewsArticle>> GetAllAsync() =>
        _newsCollection
            .Find(article => article.IsActive)
            .SortByDescending(article => article.ExpressDate)
            .ToListAsync();

    public async Task<NewsArticle?> GetByIdAsync(string id)
    {
        if (!ObjectId.TryParse(id, out _))
        {
            return null;
        }

        try
        {
            return await _newsCollection
                .Find(article => article.Id == id && article.IsActive)
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

    public Task<NewsArticle?> GetBySlugAsync(string slug) =>
        _newsCollection
            .Find(article => article.Slug == slug && article.IsActive)
            .FirstOrDefaultAsync();

    public async Task<NewsArticle> CreateAsync(NewsArticle newsArticle)
    {
        await _newsCollection.InsertOneAsync(newsArticle).ConfigureAwait(false);
        return newsArticle;
    }

    public async Task UpdateAsync(string id, NewsArticle newsArticle) =>
        await _newsCollection.ReplaceOneAsync(filter: article => article.Id == id, replacement: newsArticle).ConfigureAwait(false);

    public async Task DeleteAsync(string id) =>
        await _newsCollection.DeleteOneAsync(article => article.Id == id).ConfigureAwait(false);
}
