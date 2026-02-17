using WebApplication1.Models.Entities;
using WebApplication1.Repositories.Interfaces;
using WebApplication1.Services.Interfaces;
using System;
using System.Linq;
using System.Collections.Generic;

namespace WebApplication1.Services
{
    public class OrderService : IOrderService
    {
        private readonly IRepository<ProductEntity> _productRepo;
        private readonly IRepository<CartEntity> _cartRepo;
        private readonly IRepository<OrderEntity> _orderRepo;

        public OrderService(
            IRepository<ProductEntity> productRepo,
            IRepository<CartEntity> cartRepo,
            IRepository<OrderEntity> orderRepo)
        {
            _productRepo = productRepo;
            _cartRepo = cartRepo;
            _orderRepo = orderRepo;
        }

        public void Checkout(string userId, string userEmail)
        {
            var cartItems = _cartRepo.GetAll()
                .Where(x => x.UserId == userId)
                .ToList();

            if (!cartItems.Any())
                throw new Exception("Sepet boş.");

            var products = _productRepo.GetAll();

            var orderItems = new List<OrderItem>();
            decimal total = 0;

            foreach (var item in cartItems)
            {
                var product = products.FirstOrDefault(p => p.Id == item.ProductId);

                if (product.Stock < item.Quantity)
                    throw new Exception($"{product.Name} için stok yetersiz.");

                product.Stock -= item.Quantity;
                _productRepo.Update(product.Id, product);

                orderItems.Add(new OrderItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Price = product.Price,
                    Quantity = item.Quantity
                });

                total += product.Price * item.Quantity;
            }

            var order = new OrderEntity
            {
                UserId = userId,
                UserEmail = userEmail,
                Items = orderItems,
                TotalAmount = total,
                CreatedAt = DateTime.Now
            };

            _orderRepo.Insert(order);

            foreach (var item in cartItems)
                _cartRepo.Delete(item.Id);
        }
    }
}
