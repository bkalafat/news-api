using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using newsApi.Data;
using newsApi.Models;

namespace newsApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        // GET: api/Comment
        [HttpGet]
        public IEnumerable<Comment> Get()
        {
            return _commentService.Get();
        }


        // GET: api/Comment/5
        [HttpGet("{newsId}", Name = "GetByNewsId")]
        [ActionName("GetByNewsId")]
        public IEnumerable<Comment> GetByNewsId(string newsId)
        {
            var comment = _commentService.Get(newsId);
            return comment;
        }

        // POST: api/Comment
        [HttpPost]
        public Comment Post([FromBody] Comment comment)
        {
            return _commentService.Create(comment);
        }

        // PUT: api/Comment/5
        [HttpPut("{id:guid}")]
        public void Put([FromBody] Comment comment, Guid? id = null)
        {
            _commentService.Update(id ?? new Guid() , comment);
        }

        // DELETE: api/ApiWithActions/5
        //public void Delete([FromBody] Comment comment)
        //{
        //    _commentService.Remove(comment);
        //}

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{newsId}")]
        [ActionName("DeleteByNewsId")]
        public void DeleteByNewsId(string newsId)
        {
            _commentService.Remove(newsId);
        }

        [HttpGet("{slug}")]
        [ActionName("GetBySlug")]
        public IEnumerable<Comment> GetBySlug(string slug)
        {
            return _commentService.Get(slug);
        }
    }
}