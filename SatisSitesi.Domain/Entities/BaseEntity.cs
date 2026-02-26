using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SatisSitesi.Domain.Entities
{
    public class BaseEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Müşteri verisi kalıcılığı (Soft Delete)
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }

        public bool IsActive { get; set; } = true;

        // İzlenebilirlik (Audit)
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
