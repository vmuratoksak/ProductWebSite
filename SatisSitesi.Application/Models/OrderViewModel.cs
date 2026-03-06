using System;
using System.Collections.Generic;
using SatisSitesi.Domain.Entities;

namespace SatisSitesi.Application.Models
{
    public class OrderViewModel
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string UserEmail { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        
        public List<OrderItemViewModel> Items { get; set; } = new List<OrderItemViewModel>();
    }

    public class OrderItemViewModel
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string ImageUrl { get; set; }
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
