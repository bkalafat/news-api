using MongoDB.Bson.Serialization.Attributes;
using System;

namespace newsApi.Models
{
    public class User
    {
        [BsonId]
        public Guid Id { get; set; }
        public string ExpoNotificationRequest { get; set; }
        
    }
}