using System;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;
using MongoDB.Driver;
using newsApi.Common;
using newsApi.Models;

namespace newsApi.Data
{
    public class NewsService : INewsService
    {
        private readonly IMongoCollection<News> _newsList;
        private readonly IMemoryCache _cache;

        public NewsService(INewsDatabaseSettings settings, IMemoryCache memoryCache)
        {
            _cache = memoryCache;
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _newsList = database.GetCollection<News>(settings.NewsCollectionName);
        }

        public List<News> Get()
        {
            if (_cache.TryGetValue(CacheKeys.NewsList, out List<News> newsList)) return newsList;

            newsList = _newsList.Find(news => true).SortByDescending(news => news.CreateDate).Limit(100).ToList();
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(1));

            _cache.Set(CacheKeys.NewsList, newsList, cacheEntryOptions);

            return newsList;
        }

        public News Get(Guid id)
        {
            if (_cache.TryGetValue(id, out News news)) return news;

            news = _newsList.Find(n => n.Id == id).FirstOrDefault();
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(20));
            _cache.Set(id, news, cacheEntryOptions);

            return news;
        }

        public News Get(string slug)
        {
            if (_cache.TryGetValue(slug, out News news)) return news;

            news = _newsList.Find(n => n.Url.Contains(slug)).FirstOrDefault();
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(20));
            _cache.Set(slug, news, cacheEntryOptions);

            return news;
        }

        public List<News> GetLastNews()
        {
            if (_cache.TryGetValue(CacheKeys.LastNews, out List<News> newsList)) return newsList;

            newsList = _newsList.Find(news => true).SortByDescending(news => news.CreateDate).Limit(7).ToList();
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromHours(3));

            _cache.Set(CacheKeys.LastNews, newsList, cacheEntryOptions);

            return newsList;
        }

        public News Create(News news)
        {
            _newsList.InsertOne(news);
            return news;
        }

        public void Update(Guid id, News newsIn) =>
            _newsList.ReplaceOne(news => news.Id == id, newsIn);

        public void Remove(News newsIn) =>
            _newsList.DeleteOne(news => news.Id == newsIn.Id);

        public void Remove(Guid id) =>
            _newsList.DeleteOne(news => news.Id == id);
    }
}