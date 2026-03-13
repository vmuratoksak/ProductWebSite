using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic; // Added for Dictionary
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Localization; // Added for IStringLocalizer

namespace SatisSitesi.Domain.Entities
{
    public class ProductEntity : BaseEntity
    {
        [Required(ErrorMessage = "Ürün adi zorunludur.")]
        [StringLength(100)]
        public string Name { get; set; }
        public bool IsVisible { get; set; } = true;

        [Required(ErrorMessage = "Açiklama zorunludur.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Fiyat zorunludur.")]
        [Range(0.01, 1000000)]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Stok zorunludur.")]
        [Range(0, 100000)]
        public int Stock { get; set; }

        public string? ImageUrl { get; set; }

        public Dictionary<string, string> NameTranslations { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> DescriptionTranslations { get; set; } = new Dictionary<string, string>();

        public string GetLocalizedName(string cultureCode, IStringLocalizer localizer = null)
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

        public string GetResolvedImageUrl()
        {
            const string placeholder = "/images/placeholder.png";
            
            // Priority 1: Explicitly set ImageUrl
            if (!string.IsNullOrEmpty(ImageUrl))
            {
                if (ImageUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                    return ImageUrl;

                if (ImageUrl.Contains("/") || ImageUrl.Contains("."))
                    return ImageUrl.StartsWith("/") ? ImageUrl : $"/{ImageUrl}";
            }

            // Priority 2: Try to map product name to local images
            string source = !string.IsNullOrEmpty(Name) ? Name : "";
            if (string.IsNullOrEmpty(source)) return placeholder;

            var cleanName = source.ToLower()
                .Replace(" ", "-")
                .Replace("ş", "s")
                .Replace("ı", "i")
                .Replace("ğ", "g")
                .Replace("ü", "u")
                .Replace("ç", "c")
                .Replace("ö", "o");

            // List of known local images (from current fs)
            var localImages = new List<string> { "kitaplik", "kulaklik", "masa", "telefon-kilifi" };
            
            if (localImages.Contains(cleanName))
            {
                return $"/images/products/{cleanName}.png";
            }

            // Priority 3: Dynamic fallback based on name
            // Uses LoremFlickr for professional, high-quality, relevant images
            return $"https://loremflickr.com/640/480/{Uri.EscapeDataString(cleanName)}";
        }
    }
}
