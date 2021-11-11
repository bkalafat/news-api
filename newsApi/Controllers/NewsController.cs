using Microsoft.AspNetCore.Mvc;
using newsApi.Data;
using newsApi.Models;
using System;
using System.Collections.Generic;

namespace newsApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private readonly INewsService _newsService;

        public NewsController(INewsService newsService)
        {
            _newsService = newsService;
        }

        // GET: api/News
        [HttpGet]
        public IEnumerable<News> Get()
        {
            return _newsService.Get();
        }


        // GET: api/News/5
        [HttpGet("{id}", Name = "Get")]
        public News Get(Guid id)
        {

            var news = _newsService.Get(id);

            return news;
        }

        // POST: api/News
        [HttpPost]
        public News Post([FromBody] News news)
        {
            return _newsService.Create(news);
        }

        // PUT: api/News/5
        [HttpPut("{id}")]
        public void Put([FromBody] News news, Guid? id = null)
        {
            _newsService.Update(id ?? new Guid(), news);
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(Guid id)
        {
            _newsService.Remove(id);
        }

        [HttpGet("{slug}")]
        [ActionName("GetBySlug")]
        public News GetBySlug(string slug)
        {
            var news = _newsService.Get(slug);
            return news;
        }

        [HttpGet]
        [ActionName("GetLastNews")]
        public List<News> GetLastNews()
        {
            return _newsService.GetLastNews();
        }
    }
}
