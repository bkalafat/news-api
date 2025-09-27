using NewsApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NewsApi.Domain.Interfaces;

public interface INewsRepository
{
    Task<List<News>> GetAllAsync();
    Task<News?> GetByIdAsync(Guid id);
    Task<News?> GetByUrlAsync(string url);
    Task<News> CreateAsync(News news);
    Task UpdateAsync(Guid id, News news);
    Task DeleteAsync(Guid id);
}