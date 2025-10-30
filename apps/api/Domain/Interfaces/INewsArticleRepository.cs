using System.Collections.Generic;
using System.Threading.Tasks;
using NewsApi.Domain.Entities;

namespace NewsApi.Domain.Interfaces;

public interface INewsArticleRepository
{
    Task<List<NewsArticle>> GetAllAsync();

    Task<NewsArticle?> GetByIdAsync(string id);

    Task<NewsArticle?> GetBySlugAsync(string slug);

    Task<NewsArticle> CreateAsync(NewsArticle newsArticle);

    Task UpdateAsync(string id, NewsArticle newsArticle);

    Task DeleteAsync(string id);
}
