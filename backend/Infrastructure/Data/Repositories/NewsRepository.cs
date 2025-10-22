using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using NewsApi.Domain.Entities;
using NewsApi.Domain.Interfaces;

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
        try
        {
            // The News.Id is stored as an ObjectId in MongoDB (BsonRepresentation on the entity).
            // If the provided id is not a valid ObjectId string the driver may try to convert it
            // and throw. Return null early so callers get a NotFound instead of an exception.
            if (!ObjectId.TryParse(id, out _))
            {
                return null;
            }

            return await _newsCollection.Find(news => news.Id == id && news.IsActive).FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            // Defensive: log to console during tests to aid debugging and return null so callers
            // can return NotFound instead of causing a 500.
            try
            {
                Console.WriteLine($"NewsRepository.GetByIdAsync error for id={id}: {ex}");
            }
            catch { }
            return null;
        }
    }

    public async Task<News?> GetByUrlAsync(string url)
    {
        return await _newsCollection.Find(news => news.Url == url && news.IsActive).FirstOrDefaultAsync();
    }

    public async Task<News> CreateAsync(News news)
    {
        await _newsCollection.InsertOneAsync(news);
        return news;
    }

    public async Task UpdateAsync(string id, News news)
    {
        await _newsCollection.ReplaceOneAsync(filter: n => n.Id == id, replacement: news);
    }

    public async Task DeleteAsync(string id)
    {
        await _newsCollection.DeleteOneAsync(news => news.Id == id);
    }
}
