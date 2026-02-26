using System.Collections.Generic;
using SatisSitesi.Domain.Entities;

namespace SatisSitesi.Application.Models
{
    public class OrderIndexModel
    {
        public List<OrderEntity> Orders { get; set; } = new List<OrderEntity>();
        public string Search { get; set; }
        public string SortBy { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
