using NewsApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NewsApi.Application.Services;

public interface INewsService
{
    Task<List<News>> GetAllNewsAsync();
    Task<News?> GetNewsByIdAsync(string id);
    Task<News?> GetNewsByUrlAsync(string url);
    Task<News> CreateNewsAsync(News news);
    Task UpdateNewsAsync(string id, News news);
    Task DeleteNewsAsync(string id);
}