using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Torrefactor.Models
{
    public class GroupCoffeeOrder
    {
        [BsonId]
        public ObjectId Id { get; private set; }

        [BsonElement("createdAt"), BsonRequired]
        public DateTime CreatedAt { get; private set; }
		
        [BsonElement("isSent"), BsonRequired]
        public bool IsSent { get; private set; }

        public GroupCoffeeOrder()
        {
            CreatedAt = DateTime.UtcNow;
        }
    }
}