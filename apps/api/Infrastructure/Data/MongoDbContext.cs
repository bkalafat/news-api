using MongoDB.Driver;
using NewsApi.Domain.Entities;
using NewsApi.Infrastructure.Data.Configurations;

namespace NewsApi.Infrastructure.Data;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(MongoDbSettings settings)
    {
        var client = new MongoClient(settings.ConnectionString);
        _database = client.GetDatabase(settings.DatabaseName);
    }

    public IMongoCollection<NewsArticle> News => _database.GetCollection<NewsArticle>("News");
}
