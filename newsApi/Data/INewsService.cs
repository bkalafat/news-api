using System;
using System.Collections.Generic;
using newsApi.Models;

namespace newsApi.Data
{
    public interface INewsService
    {
        public List<News> Get();

        public News Get(Guid id);

        public News Get(string url);

        public News Create(News news);

        public void Update(Guid id, News newsIn);

        public void Remove(News newsIn);

        public void Remove(Guid id);
    }
}