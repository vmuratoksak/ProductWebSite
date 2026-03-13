using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace SatisSitesi.Domain.Entities
{
    public class ContactMessageEntity : BaseEntity
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public DateTime SentAt { get; set; } = DateTime.Now;
        public bool IsRead { get; set; } = false;
    }
}
