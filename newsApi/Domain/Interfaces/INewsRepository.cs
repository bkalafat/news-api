using NewsApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NewsApi.Domain.Interfaces;

public interface INewsRepository
{
    Task<List<News>> GetAllAsync();
    Task<News?> GetByIdAsync(string id);
    Task<News?> GetByUrlAsync(string url);
    Task<News> CreateAsync(News news);
    Task UpdateAsync(string id, News news);
    Task DeleteAsync(string id);
}