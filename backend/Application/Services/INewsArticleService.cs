using System.Collections.Generic;
using System.Threading.Tasks;
using NewsApi.Domain.Entities;

namespace NewsApi.Application.Services;

internal interface INewsArticleService
{
    Task<List<NewsArticle>> GetAllNewsAsync();

    Task<NewsArticle?> GetNewsByIdAsync(string id);

    Task<NewsArticle?> GetNewsBySlugAsync(string slug);

    Task<NewsArticle> CreateNewsAsync(NewsArticle newsArticle);

    Task UpdateNewsAsync(string id, NewsArticle newsArticle);

    Task DeleteNewsAsync(string id);

    /// <summary>
    /// Clears all cached news data.
    /// </summary>
    void ClearCache();
}
