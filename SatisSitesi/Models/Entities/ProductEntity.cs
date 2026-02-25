using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace SatisSitesi.Models.Entities
{
    public class ProductEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [Required(ErrorMessage = "Ürün adı zorunludur.")]
        [StringLength(100)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Açıklama zorunludur.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Fiyat zorunludur.")]
        [Range(0.01, 1000000)]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Stok zorunludur.")]
        [Range(0, 100000)]
        public int Stock { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Dictionary<string, string> NameTranslations { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> DescriptionTranslations { get; set; } = new Dictionary<string, string>();

        public string GetLocalizedName(string cultureCode, Microsoft.Extensions.Localization.IStringLocalizer localizer = null)
        {
            if (NameTranslations != null && NameTranslations.ContainsKey(cultureCode) && !string.IsNullOrWhiteSpace(NameTranslations[cultureCode]))
                return NameTranslations[cultureCode];

            if (localizer != null)
            {
                var loc = localizer[Name ?? ""];
                if (!loc.ResourceNotFound) return loc.Value;
            }

            return Name ?? "";
        }

        public string GetLocalizedDescription(string cultureCode, Microsoft.Extensions.Localization.IStringLocalizer localizer = null)
        {
            if (DescriptionTranslations != null && DescriptionTranslations.ContainsKey(cultureCode) && !string.IsNullOrWhiteSpace(DescriptionTranslations[cultureCode]))
                return DescriptionTranslations[cultureCode];

            if (localizer != null)
            {
                var loc = localizer[Description ?? ""];
                if (!loc.ResourceNotFound) return loc.Value;
            }

            return Description ?? "";
        }
    }
}
