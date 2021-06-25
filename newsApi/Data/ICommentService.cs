using System;
using System.Collections.Generic;
using newsApi.Models;

namespace newsApi.Data
{
    public interface ICommentService
    {
        public IEnumerable<Comment> Get();

        public IEnumerable<Comment> Get(Guid newsId);

        public IEnumerable<Comment> Get(string slug);

        public Comment Create(Comment comment);

        public void Update(Guid id, Comment comment);

        public void Remove(Comment comment);

        public void Remove(string newsId);
    }
}