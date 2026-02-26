using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Localization;

namespace SatisSitesi.Domain.Entities
{
    public class OrderEntity : BaseEntity
    {
        public string UserId { get; set; }

        public string UserEmail { get; set; }

        public List<OrderItem> Items { get; set; }

        public decimal TotalAmount { get; set; }

        public string Status { get; set; } = "Bekliyor"; // Bekliyor, Onaylandi, Iptal Edildi
    }

    public class OrderItem
    {
        public string ProductId { get; set; }

        public string ProductName { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }

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
