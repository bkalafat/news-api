using MongoDB.Bson.Serialization.Attributes;
using System;

namespace newsApi.Models
{
    public class News
    {
        [BsonId]
        public Guid Id { get; set; }
        public string Category { get; set; }
        public string Type { get; set; }
        public string Caption { get; set; }
        public string Keywords { get; set; }
        public string SocialTags { get; set; }
        public string Summary { get; set; }
        public string ImgPath { get; set; }
        public string ImgAlt { get; set; }
        public string Content { get; set; }
        public string[] Subjects { get; set; }
        public string[] Authors { get; set; }
        public DateTime ExpressDate { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public int Priority { get; set; }
        public bool IsActive { get; set; }
        public bool ShowNotification { get; set; }
        public string Slug { get; set; }
        public string Url { get; set; }
        public int ViewCount { get; set; }
        public bool IsSecondPageNews { get; set; }
    }
}