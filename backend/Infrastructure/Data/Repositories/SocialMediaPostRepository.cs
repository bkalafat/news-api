using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using NewsApi.Domain.Entities;
using NewsApi.Domain.Interfaces;

namespace NewsApi.Infrastructure.Data.Repositories;

/// <summary>
/// MongoDB repository for social media posts
/// </summary>
public sealed class SocialMediaPostRepository : ISocialMediaPostRepository
{
    private readonly IMongoCollection<SocialMediaPost> _collection;

    public SocialMediaPostRepository(MongoDbContext context)
    {
        _collection = context.SocialMediaPosts;
    }

    public async Task<List<SocialMediaPost>> GetAllAsync() =>
        await _collection
            .Find(post => post.IsActive)
            .SortByDescending(post => post.PostedAt)
            .ToListAsync()
            .ConfigureAwait(false);

    public async Task<List<SocialMediaPost>> GetByPlatformAsync(string platform) =>
        await _collection
            .Find(post => post.Platform == platform && post.IsActive)
            .SortByDescending(post => post.PostedAt)
            .ToListAsync()
            .ConfigureAwait(false);

    public async Task<List<SocialMediaPost>> GetByCategoryAsync(string category) =>
        await _collection
            .Find(post => post.Category == category && post.IsActive)
            .SortByDescending(post => post.PostedAt)
            .ToListAsync()
            .ConfigureAwait(false);

    public async Task<List<SocialMediaPost>> GetTopPostsAsync(int limit, string? platform = null)
    {
        var filter = platform == null
            ? Builders<SocialMediaPost>.Filter.Eq(post => post.IsActive, true)
            : Builders<SocialMediaPost>.Filter.And(
                Builders<SocialMediaPost>.Filter.Eq(post => post.IsActive, true),
                Builders<SocialMediaPost>.Filter.Eq(post => post.Platform, platform)
            );

        return await _collection
            .Find(filter)
            .SortByDescending(post => post.Upvotes)
            .Limit(limit)
            .ToListAsync()
            .ConfigureAwait(false);
    }

    public async Task<SocialMediaPost?> GetByIdAsync(string id)
    {
        if (!ObjectId.TryParse(id, out _))
        {
            return null;
        }

        try
        {
            return await _collection
                .Find(post => post.Id == id && post.IsActive)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"SocialMediaPostRepository.GetByIdAsync error for id={id}: {ex}");
            return null;
        }
    }

    public async Task<SocialMediaPost?> GetByExternalIdAsync(string externalId, string platform) =>
        await _collection
            .Find(post => post.ExternalId == externalId && post.Platform == platform)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);

    public async Task<SocialMediaPost> CreateAsync(SocialMediaPost post)
    {
        await _collection.InsertOneAsync(post).ConfigureAwait(false);
        return post;
    }

    public async Task UpdateAsync(string id, SocialMediaPost post) =>
        await _collection
            .ReplaceOneAsync(filter: p => p.Id == id, replacement: post)
            .ConfigureAwait(false);

    public async Task DeleteAsync(string id) =>
        await _collection
            .DeleteOneAsync(post => post.Id == id)
            .ConfigureAwait(false);

    public async Task UpdateMetricsAsync(string id, int upvotes, int downvotes, int commentCount, int shareCount)
    {
        var update = Builders<SocialMediaPost>.Update
            .Set(post => post.Upvotes, upvotes)
            .Set(post => post.Downvotes, downvotes)
            .Set(post => post.CommentCount, commentCount)
            .Set(post => post.ShareCount, shareCount)
            .Set(post => post.LastUpdated, DateTime.UtcNow);

        await _collection
            .UpdateOneAsync(post => post.Id == id, update)
            .ConfigureAwait(false);
    }
}
