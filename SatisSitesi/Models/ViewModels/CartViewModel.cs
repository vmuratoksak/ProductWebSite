using Microsoft.Extensions.Localization;
using System.Collections.Generic;

namespace SatisSitesi.Models.ViewModels
{
    public class CartViewModel
    {
        public string Id { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        public decimal TotalPrice => Price * Quantity;

        public Dictionary<string, string> NameTranslations { get; set; } = new Dictionary<string, string>();

        public string GetLocalizedName(string cultureCode, Microsoft.Extensions.Localization.IStringLocalizer localizer = null)
        {
            if (NameTranslations != null && NameTranslations.ContainsKey(cultureCode) && !string.IsNullOrWhiteSpace(NameTranslations[cultureCode]))
                return NameTranslations[cultureCode];

            if (localizer != null)
            {
                var loc = localizer[ProductName ?? ""];
                if (!loc.ResourceNotFound) return loc.Value;
            }

            return ProductName ?? "";
        }
    }
}
