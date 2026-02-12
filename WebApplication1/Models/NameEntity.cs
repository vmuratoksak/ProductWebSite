using System;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebApplication1.Models
{
    public class NameEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }   // ← BURASI DEĞİŞTİ

        [Required(ErrorMessage = "İsim zorunludur.")]
        [StringLength(50, ErrorMessage = "İsim en fazla 50 karakter olabilir.")]
        public string Name { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
