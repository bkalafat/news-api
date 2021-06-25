using MongoDB.Bson.Serialization.Attributes;
using System;

namespace newsApi.Models
{
    public class Comment
    {
        [BsonId]
        public Guid Id { get; set; }
        public string NewsId { get; set; }
        public string UserName { get; set; }
        public string Text { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public bool IsActive { get; set; }
    }

}