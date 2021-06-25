using System;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;
using MongoDB.Driver;
using newsApi.Common;
using newsApi.Models;

namespace newsApi.Data
{
    //TODO: bkalafat Instead cache remove _cache try get value update it and set to _cache again.
    public class CommentService : ICommentService
    {
        private readonly IMongoCollection<Comment> _commentList;
        private readonly IMongoCollection<News> _newsList;
        private readonly IMemoryCache _cache;

        public CommentService(INewsDatabaseSettings settings, IMemoryCache memoryCache)
        {
            _cache = memoryCache;
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _newsList = database.GetCollection<News>(settings.NewsCollectionName);
            _commentList = database.GetCollection<Comment>(settings.CommentCollectionName);
        }

        public IEnumerable<Comment> Get()
        {
            if (_cache.TryGetValue(CacheKeys.CommentList, out List<Comment> commentList)) return commentList;
            commentList = _commentList.Find(comment => true).SortByDescending(comment => comment.CreateDate).Limit(100).ToList();
            _cache.Set(CacheKeys.CommentList, commentList);
            return commentList;
        }

        public IEnumerable<Comment> Get(Guid newsId)
        {
            if (_cache.TryGetValue(CacheKeys.Comment + newsId, out IEnumerable<Comment> commentList)) return commentList;

            commentList = _commentList.Find(n => n.Id == newsId).ToList();
            _cache.Set(CacheKeys.Comment + newsId, commentList);

            return commentList;
        }

        public IEnumerable<Comment> Get(string slug)
        {
            if (!_cache.TryGetValue(slug, out News news))
            {
                news = _newsList.Find(n => n.Url.Contains(slug)).FirstOrDefault();
            }
            if (news != null && _cache.TryGetValue(CacheKeys.Comment + news.Id, out IEnumerable<Comment> commentList))
            {
                return commentList;
            }

            commentList = _commentList.Find(n => news != null && n.NewsId == news.Id.ToString()).ToList();
            if (news != null) _cache.Set(CacheKeys.Comment + news.Id, commentList);

            return commentList;
        }

        public Comment Create(Comment comment)
        {
            _cache.Remove(CacheKeys.Comment + comment.NewsId);
            _cache.Remove(CacheKeys.CommentList);
            _commentList.InsertOne(comment);
            return comment;
        }

        public void Update(Guid id, Comment comment)
        {
            _cache.Remove(CacheKeys.Comment + comment.NewsId);
            _commentList.ReplaceOne(c => c.Id == id, comment);
        }

        public void Remove(Comment comment)
        {
            _cache.Remove(CacheKeys.Comment + comment.NewsId);
            _commentList.DeleteOne(c => c.Id == comment.Id);
        }

        public void Remove(string newsId)
        {
            _cache.Remove(CacheKeys.Comment + newsId);
            _commentList.DeleteMany(comment => comment.NewsId == newsId);
        }
    }
}