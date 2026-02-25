using SatisSitesi.Models.Entities;
using SatisSitesi.Repositories.Interfaces;
using SatisSitesi.Services.Interfaces;
using System;
using System.Linq;
using System.Collections.Generic;

namespace SatisSitesi.Services
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
            var cart = _cartRepo.GetAll().FirstOrDefault(x => x.UserId == userId);

            if (cart == null || cart.Items == null || !cart.Items.Any())
                throw new Exception("Sepet boş.");

            var orderItems = new List<OrderItem>();
            decimal total = 0;

            foreach (var item in cart.Items)
            {
                var product = _productRepo.GetById(item.ProductId);

                if (product == null)
                    throw new Exception("Ürün bulunamadı.");

                if (product.Stock < item.Quantity)
                    throw new Exception($"{product.Name} için stok yetersiz.");

                product.Stock -= item.Quantity;
                _productRepo.Update(product.Id, product);

                orderItems.Add(new OrderItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Price = product.Price,
                    Quantity = item.Quantity,
                    NameTranslations = product.NameTranslations != null ? new Dictionary<string, string>(product.NameTranslations) : new Dictionary<string, string>()
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

            _cartRepo.Delete(cart.Id);
        }

        public List<OrderEntity> GetUserOrders(string userId)
        {
            return _orderRepo.GetAll()
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToList();
        }

        public List<OrderEntity> GetAllOrders()
        {
            return _orderRepo.GetAll()
                .OrderByDescending(x => x.CreatedAt)
                .ToList();
        }

        public void UpdateOrderStatus(string orderId, string newStatus)
        {
            var order = _orderRepo.GetById(orderId);
            if (order == null)
                throw new Exception("Sipariş bulunamadı.");

            order.Status = newStatus;
            _orderRepo.Update(order.Id, order);
        }
    }
}
