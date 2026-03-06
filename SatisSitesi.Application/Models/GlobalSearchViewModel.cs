using SatisSitesi.Domain.Entities;
using System.Collections.Generic;

namespace SatisSitesi.Application.Models
{
    public class GlobalSearchViewModel
    {
        public string Query { get; set; }
        public List<ProductEntity> Products { get; set; } = new List<ProductEntity>();
        public List<UserEntity> Users { get; set; } = new List<UserEntity>(); // Changed to UserEntity
        public List<OrderEntity> Orders { get; set; } = new List<OrderEntity>();

        public int TotalResults => Products.Count + Users.Count + Orders.Count;
    }
}
