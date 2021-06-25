using System;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;
using MongoDB.Driver;
using newsApi.Common;
using newsApi.Models;

namespace newsApi.Data
{
    //TODO: bkalafat Instead cache remove _cache try get value update it and set to _cache again.
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
            _cache.Set(CacheKeys.NewsList, newsList);

            return newsList;
        }

        public News Get(Guid id)
        {
            if (_cache.TryGetValue(id, out News news)) return news;

            news = _newsList.Find(n => n.Id == id).FirstOrDefault();
            _cache.Set(id, news);

            return news;
        }

        public News Get(string slug)
        {
            if (_cache.TryGetValue(slug, out News news)) return news;

            news = _newsList.Find(n => n.Url.Contains(slug)).FirstOrDefault();
            _cache.Set(slug, news);

            return news;
        }

        public List<News> GetLastNews()
        {
            if (_cache.TryGetValue(CacheKeys.LastNews, out List<News> newsList)) return newsList;

            newsList = _newsList.Find(news => news.IsActive).SortByDescending(news => news.CreateDate).Limit(7).ToList();
            _cache.Set(CacheKeys.LastNews, newsList);

            return newsList;
        }

        public News Create(News news)
        {
            _cache.Remove(CacheKeys.NewsList);
            _cache.Remove(CacheKeys.LastNews);
            _newsList.InsertOne(news);
            return news;
        }

        public void Update(Guid id, News newsIn)
        {
            _cache.Remove(CacheKeys.NewsList);
            _cache.Remove(CacheKeys.LastNews);
            _cache.Remove(newsIn.Slug);
            _cache.Remove(id);
            _newsList.ReplaceOne(news => news.Id == id, newsIn);
        }

        public void Remove(News newsIn)
        {
            _cache.Remove(CacheKeys.NewsList);
            _cache.Remove(CacheKeys.LastNews);
            _newsList.DeleteOne(news => news.Id == newsIn.Id);
        }

        public void Remove(Guid id)
        {
            _cache.Remove(CacheKeys.NewsList);
            _cache.Remove(CacheKeys.LastNews);
            _newsList.DeleteOne(news => news.Id == id);
        }
    }
}