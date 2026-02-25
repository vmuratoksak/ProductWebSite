using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace SatisSitesi.Models.Entities
{
    public class CartEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string UserId { get; set; }

        public List<CartItem> Items { get; set; } = new List<CartItem>();

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CartItem
    {
        public string ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime AddedAt { get; set; }
    }
}
