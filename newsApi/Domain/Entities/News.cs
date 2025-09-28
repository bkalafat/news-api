using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace NewsApi.Domain.Entities;

public class News
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
    
    public string Category { get; set; } = string.Empty;
    
    public string Type { get; set; } = string.Empty;
    
    public string Caption { get; set; } = string.Empty;
    
    public string Keywords { get; set; } = string.Empty;
    
    public string SocialTags { get; set; } = string.Empty;
    
    public string Summary { get; set; } = string.Empty;
    
    public string ImgPath { get; set; } = string.Empty;
    
    public string ImgAlt { get; set; } = string.Empty;
    
    public string Content { get; set; } = string.Empty;
    
    public string[] Subjects { get; set; } = Array.Empty<string>();
    
    public string[] Authors { get; set; } = Array.Empty<string>();
    
    public DateTime ExpressDate { get; set; }
    
    public DateTime CreateDate { get; set; }
    
    public DateTime UpdateDate { get; set; }
    
    public int Priority { get; set; }
    
    public bool IsActive { get; set; }
    
    public string Url { get; set; } = string.Empty;
    
    public int ViewCount { get; set; }
    
    public bool IsSecondPageNews { get; set; }
}