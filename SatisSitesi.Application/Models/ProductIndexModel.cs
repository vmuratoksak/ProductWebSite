using System.Collections.Generic;

namespace SatisSitesi.Application.Models
{
    public class ProductIndexModel
    {
        public List<ProductViewModel> Products { get; set; } = new List<ProductViewModel>();
        public string Search { get; set; } = string.Empty;
        public string SortBy { get; set; } = string.Empty;
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public bool InStockOnly { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalProducts { get; set; }
    }

    public class ProductViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string ImageUrl { get; set; }
        // Indicates if stock is low or out of stock
        public string BadgeText { get; set; }
        public string BadgeClass { get; set; }
    }
}
