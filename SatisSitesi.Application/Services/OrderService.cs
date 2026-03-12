using SatisSitesi.Domain.Entities;
using SatisSitesi.Application.Interfaces.Services;
using SatisSitesi.Application.Interfaces.Repositories;
using SatisSitesi.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SatisSitesi.Application.Services
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

        public void Checkout(string userId, string userEmail, string address = null)
        {
            var cart = _cartRepo.GetAll().FirstOrDefault(x => x.UserId == userId);

            if (cart == null || cart.Items == null || !cart.Items.Any())
                throw new Exception("Sepet bos.");

            var orderItems = new List<OrderItem>();
            decimal total = 0;

            foreach (var item in cart.Items)
            {
                var product = _productRepo.GetById(item.ProductId);

                if (product == null)
                    throw new Exception("Ürün bulunamadi.");

                // Sadece sipariş verirken kontrol et, düşümü admin onaylarsa yap
                if (product.Stock < item.Quantity)
                    throw new Exception($"{product.Name} için stok yetersiz.");

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
                Address = address,
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

        private OrderIndexModel BuildOrderIndexModel(IQueryable<OrderEntity> query, string search, string sortBy, int page, int pageSize)
        {
            // Arama: ID'de VEYA (eğer varsa) Kullanıcı Email'inde arar
            if (!string.IsNullOrWhiteSpace(search))
            {
                var lowerSearch = search.ToLower();
                query = query.Where(x => 
                    x.Id.ToLower().Contains(lowerSearch) || 
                    (x.UserEmail != null && x.UserEmail.ToLower().Contains(lowerSearch)));
            }

            query = sortBy switch
            {
                "Oldest" => query.OrderBy(x => x.CreatedAt),
                "TotalDesc" => query.OrderByDescending(x => x.TotalAmount),
                "TotalAsc" => query.OrderBy(x => x.TotalAmount),
                _ => query.OrderByDescending(x => x.CreatedAt), // Varsayılan En Yeni
            };

            var totalCount = query.Count();

            var orders = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new OrderIndexModel
            {
                Orders = orders,
                Search = search,
                SortBy = sortBy,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                TotalOrders = totalCount
            };
        }

        public OrderIndexModel GetPagedUserOrders(string userId, string search, string sortBy, int page, int pageSize)
        {
            var query = _orderRepo.GetAll().Where(x => x.UserId == userId).AsQueryable();
            return BuildOrderIndexModel(query, search, sortBy, page, pageSize);
        }

        public OrderIndexModel GetPagedAdminOrders(string search, string sortBy, int page, int pageSize)
        {
            var query = _orderRepo.GetAll().AsQueryable();
            return BuildOrderIndexModel(query, search, sortBy, page, pageSize);
        }

        public void UpdateOrderStatus(string orderId, string newStatus)
        {
            var order = _orderRepo.GetById(orderId);
            if (order == null)
                throw new Exception("Siparis bulunamadi.");

            // Eğer Admin siparişi onaylıyorsa VE sipariş daha önce onaylanmamışsa, stokları şimdi düş.
            if (newStatus == "Onaylandı" && order.Status != "Onaylandı")
            {
                foreach (var item in order.Items)
                {
                    var product = _productRepo.GetById(item.ProductId);
                    if (product != null)
                    {
                        if (product.Stock >= item.Quantity)
                        {
                            product.Stock -= item.Quantity;
                            _productRepo.Update(product.Id, product);
                        }
                        else
                        {
                            // İsteğe bağlı: Stok yetersizse admin onayını iptal et ve hata fırlat
                            throw new Exception($"{product.Name} isimli ürün için yeterli stok kalmadı.");
                        }
                    }
                }
            }
            // Eğer Admin onaylanmış bir siparişi "İptal Edildi"ye çekerse, stokları geri iade et.
            else if (newStatus == "İptal Edildi" && order.Status == "Onaylandı")
            {
                foreach (var item in order.Items)
                {
                    var product = _productRepo.GetById(item.ProductId);
                    if (product != null)
                    {
                        product.Stock += item.Quantity;
                        _productRepo.Update(product.Id, product);
                    }
                }
            }

            order.Status = newStatus;
            _orderRepo.Update(order.Id, order);
        }

        public OrderViewModel GetOrderById(string orderId)
        {
            var order = _orderRepo.GetById(orderId);
            if (order == null) return null;

            var viewModel = new OrderViewModel
            {
                Id = order.Id,
                UserId = order.UserId,
                UserEmail = order.UserEmail,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                CreatedAt = order.CreatedAt,
                Items = order.Items.Select(item => {
                    var product = _productRepo.GetById(item.ProductId);
                    return new OrderItemViewModel
                    {
                        ProductId = item.ProductId,
                        ProductName = item.ProductName,
                        Price = item.Price,
                        Quantity = item.Quantity,
                        ImageUrl = product != null && !string.IsNullOrEmpty(product.ImageUrl) ? product.ImageUrl : "/images/placeholder.png",
                        NameTranslations = item.NameTranslations
                    };
                }).ToList()
            };

            return viewModel;
        }

        public int GetPendingOrdersCount()
        {
            return _orderRepo.GetAll().Count(x => x.Status == "Bekliyor");
        }

        public int GetUserNotificationCount(string userId)
        {
            // For users, show non-pending and non-cancelled orders as "notifications" (simple logic for now)
            return _orderRepo.GetAll().Count(x => x.UserId == userId && x.Status != "Bekliyor" && x.Status != "İptal Edildi");
        }
    }
}
