using NewsApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NewsApi.Application.Services;

public interface INewsService
{
    Task<List<News>> GetAllNewsAsync();
    Task<News?> GetNewsByIdAsync(Guid id);
    Task<News?> GetNewsByUrlAsync(string url);
    Task<News> CreateNewsAsync(News news);
    Task UpdateNewsAsync(Guid id, News news);
    Task DeleteNewsAsync(Guid id);
}