using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace SatisSitesi.Models.Entities
{
    public class TranslationCacheEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string Key { get; set; } = string.Empty;

        // Stores Culture -> Translated String (e.g., "en" -> "Privacy", "de" -> "Datenschutz")
        public Dictionary<string, string> Translations { get; set; } = new Dictionary<string, string>();
    }
}
