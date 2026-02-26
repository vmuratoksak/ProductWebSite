using System.Collections.Generic;
using SatisSitesi.Domain.Entities;

namespace SatisSitesi.Application.Models
{
    public class ProductIndexModel
    {
        public List<ProductEntity> Products { get; set; } = new List<ProductEntity>();
        public string Search { get; set; } = string.Empty;
        public string SortBy { get; set; } = string.Empty;
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public bool InStockOnly { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
