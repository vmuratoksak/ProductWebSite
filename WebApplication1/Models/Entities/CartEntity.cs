using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace WebApplication1.Models.Entities
{
    public class CartEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string UserId { get; set; }
        public string ProductId { get; set; }

        public int Quantity { get; set; }

        public DateTime AddedAt { get; set; }
    }
}
