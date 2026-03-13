using SatisSitesi.Domain.Entities;
using SatisSitesi.Application.Models;
using SatisSitesi.Application.Interfaces.Repositories;
using SatisSitesi.Application.Interfaces.Services;
using System;
using System.Linq;
using System.Collections.Generic;

namespace SatisSitesi.Application.Services
{
    public class HomeService : IHomeService
    {
        private readonly IRepository<NameEntity> _customerRepo;
        private readonly IRepository<ProductEntity> _productRepo;
        private readonly IRepository<OrderEntity> _orderRepo;
        private readonly IRepository<UserEntity> _userRepo;
        private readonly ICurrencyService _currencyService;
        private readonly IRepository<ContactMessageEntity> _contactRepo;

        public HomeService(
            IRepository<NameEntity> customerRepo,
            IRepository<ProductEntity> productRepo,
            IRepository<OrderEntity> orderRepo,
            IRepository<UserEntity> userRepo,
            ICurrencyService currencyService,
            IRepository<ContactMessageEntity> contactRepo)
        {
            _customerRepo = customerRepo;
            _productRepo = productRepo;
            _orderRepo = orderRepo;
            _userRepo = userRepo;
            _currencyService = currencyService;
            _contactRepo = contactRepo;
        }

        public DashboardModel GetDashboardData(string userId, string role)
        {
            IEnumerable<OrderEntity> ordersQuery = _orderRepo.GetAll();
            if (role != "Admin")
            {
                ordersQuery = ordersQuery.Where(o => o.UserId == userId);
            }
            var allOrders = ordersQuery.ToList();
            var allProducts = _productRepo.GetAll().ToList();
            var allUsers = _userRepo.GetAll().ToList(); 

            var totalRevenue = allOrders.Sum(o => o.TotalAmount);
            var now = DateTime.Now;
            
            var thisMonthOrders = allOrders.Where(o => o.CreatedAt.Month == now.Month && o.CreatedAt.Year == now.Year).ToList();
            var lastMonthOrders = allOrders.Where(o => o.CreatedAt.Month == now.AddMonths(-1).Month && o.CreatedAt.Year == now.AddMonths(-1).Year).ToList();

            var thisMonthRevenue = thisMonthOrders.Sum(o => o.TotalAmount);
            var lastMonthRevenue = lastMonthOrders.Sum(o => o.TotalAmount);
            var revenueGrowthPct = 0.0;
            if (lastMonthRevenue > 0)
                revenueGrowthPct = (double)((thisMonthRevenue - lastMonthRevenue) / lastMonthRevenue * 100);
            else if (thisMonthRevenue > 0)
                revenueGrowthPct = 100.0;

            var totalUsers = allUsers.Count;
            var thisMonthUsers = allUsers.Count(u => u.CreatedAt.Month == now.Month && u.CreatedAt.Year == now.Year);
            var lastMonthUsers = allUsers.Count(u => u.CreatedAt.Month == now.AddMonths(-1).Month && u.CreatedAt.Year == now.AddMonths(-1).Year);
            var userGrowthPct = 0.0;
            if (lastMonthUsers > 0)
                userGrowthPct = (double)((thisMonthUsers - lastMonthUsers) / (double)lastMonthUsers * 100);
            else if (thisMonthUsers > 0)
                userGrowthPct = 100.0;

            var totalSales = allOrders.Count;
            var thisMonthSalesCount = thisMonthOrders.Count;
            var lastMonthSalesCount = lastMonthOrders.Count;
            var salesGrowthPct = 0.0;
            if (lastMonthSalesCount > 0)
                salesGrowthPct = (double)((thisMonthSalesCount - lastMonthSalesCount) / (double)lastMonthSalesCount * 100);
            else if (thisMonthSalesCount > 0)
                salesGrowthPct = 100.0;

            var totalProducts = allProducts.Count;
            var inStockProducts = allProducts.Count(p => p.Stock > 0);

            var recentOrders = allOrders
                .OrderByDescending(o => o.CreatedAt)
                .Take(5)
                .Select(o => new RecentSaleModel
                {
                    Initials = string.IsNullOrEmpty(o.UserEmail) ? "??" : o.UserEmail.Substring(0, Math.Min(2, o.UserEmail.Length)).ToUpper(),
                    Name = allUsers.FirstOrDefault(u => u.Id == o.UserId)?.Username ?? o.UserEmail ?? "Unknown",
                    Email = o.UserEmail ?? "",
                    Amount = o.TotalAmount,
                    ColorClass = GetColorForEmail(o.UserEmail)
                })
                .ToList();

            var productSalesCounts = allOrders
                .SelectMany(o => o.Items)
                .GroupBy(i => i.ProductId)
                .Select(g => new { ProductId = g.Key, Count = g.Sum(i => i.Quantity) })
                .OrderByDescending(x => x.Count)
                .Take(4)
                .ToList();

            var maxSales = productSalesCounts.FirstOrDefault()?.Count ?? 1;
            var topProducts = productSalesCounts.Select(ps =>
            {
                var prod = allProducts.FirstOrDefault(p => p.Id == ps.ProductId);
                return new TopProductModel
                {
                    Name = prod?.Name ?? "Bilinmeyen Ürün",
                    SalesCount = ps.Count,
                    Price = prod?.Price ?? 0,
                    ImageUrl = prod?.GetResolvedImageUrl() ?? "/images/placeholder.png",
                    ProgressPercentage = maxSales == 0 ? 0 : (int)((ps.Count / (double)maxSales) * 100)
                };
            }).ToList();

            return new DashboardModel
            {
                TotalRevenue = totalRevenue,
                RevenueGrowth = $"{(revenueGrowthPct >= 0 ? "+" : "")}{revenueGrowthPct:F1}% bu ay",
                IsRevenueUp = revenueGrowthPct >= 0,

                ActiveUsers = role == "Admin" ? totalUsers : 0,
                UsersGrowth = role == "Admin" ? $"{(userGrowthPct >= 0 ? "+" : "")}{userGrowthPct:F1}% bu ay" : "",
                IsUsersUp = userGrowthPct >= 0,

                Sales = totalSales,
                SalesGrowth = $"{(salesGrowthPct >= 0 ? "+" : "")}{salesGrowthPct:F1}% bu ay",
                IsSalesUp = salesGrowthPct >= 0,

                ActiveProducts = totalProducts,
                ProductsGrowth = $"{inStockProducts} stokta",
                IsProductsUp = inStockProducts > 0,

                TotalRecentSalesCount = totalSales,
                RecentSales = recentOrders,
                TopProducts = topProducts
            };
        }

        public ChartDataModel GetChartData(string userId, string role)
        {
            IEnumerable<OrderEntity> ordersQuery = _orderRepo.GetAll();
            if (role != "Admin")
            {
                ordersQuery = ordersQuery.Where(o => o.UserId == userId);
            }
            var allOrders = ordersQuery.ToList();
            var now = DateTime.Now;

            var labels = new List<string>();
            var revenueData = new List<decimal>();
            var ordersData = new List<int>();

            for (int i = 5; i >= 0; i--)
            {
                var month = now.AddMonths(-i);
                labels.Add(month.ToString("MMM"));

                var monthOrders = allOrders.Where(o =>
                    o.CreatedAt.Month == month.Month &&
                    o.CreatedAt.Year == month.Year).ToList();

                var monthRevenue = monthOrders.Sum(o => o.TotalAmount);
                var convertedRevenue = _currencyService.GetConvertedPrice(monthRevenue);
                
                revenueData.Add(convertedRevenue);
                ordersData.Add(monthOrders.Count);
            }

            return new ChartDataModel
            {
                ChartLabels = labels,
                RevenueChartData = revenueData,
                OrdersChartData = ordersData
            };
        }

        public GlobalSearchViewModel GetGlobalSearchResults(string query, string userId, string role)
        {
            if (string.IsNullOrWhiteSpace(query)) return new GlobalSearchViewModel { Query = "" };

            var lowerQuery = query.ToLower();
            var results = new GlobalSearchViewModel { Query = query };

            // 1. Products
            var productQuery = _productRepo.GetAll().AsQueryable();
            if (role != "Admin")
            {
                productQuery = productQuery.Where(p => p.IsVisible);
            }

            results.Products = productQuery
                .Where(p => (p.Name != null && p.Name.ToLower().Contains(lowerQuery)) || 
                            (p.Description != null && p.Description.ToLower().Contains(lowerQuery)))
                .ToList();

            // 2. Users
            if (role == "Admin")
            {
                results.Users = _userRepo.GetAll()
                    .Where(u => u.Username.ToLower().Contains(lowerQuery) || u.Email.ToLower().Contains(lowerQuery))
                    .ToList();
            }

            // 3. Orders
            IEnumerable<OrderEntity> orderQuery = _orderRepo.GetAll();
            if (role != "Admin")
            {
                orderQuery = orderQuery.Where(o => o.UserId == userId);
            }

            results.Orders = orderQuery
                .Where(o => o.Id.ToLower().Contains(lowerQuery) || (o.UserEmail != null && o.UserEmail.ToLower().Contains(lowerQuery)))
                .ToList();

            return results;
        }

        public void SaveContactMessage(ContactMessageEntity message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            // Basic validation
            if (string.IsNullOrWhiteSpace(message.FullName)) throw new Exception("Ad Soyad boş olamaz.");
            if (string.IsNullOrWhiteSpace(message.Email)) throw new Exception("E-posta adresi boş olamaz.");
            if (string.IsNullOrWhiteSpace(message.Phone)) throw new Exception("Telefon numarası boş olamaz.");
            if (string.IsNullOrWhiteSpace(message.Subject)) throw new Exception("Konu seçilmelidir.");
            if (string.IsNullOrWhiteSpace(message.Message)) throw new Exception("Mesaj boş olamaz.");

            // Email format validation
            if (!System.Text.RegularExpressions.Regex.IsMatch(message.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                throw new Exception("Geçersiz e-posta formatı.");
            }

            // Phone format validation
            if (!System.Text.RegularExpressions.Regex.IsMatch(message.Phone, @"^[+]?[0-9\s-]{10,20}$"))
            {
                throw new Exception("Geçersiz telefon numarası formatı.");
            }

            _contactRepo.Insert(message);
        }

        public List<ContactMessageEntity> GetContactMessages()
        {
            return _contactRepo.GetAll().OrderByDescending(m => m.SentAt).ToList();
        }

        public void DeleteContactMessage(string id)
        {
            _contactRepo.Delete(id);
        }

        private static readonly string[] Colors = { "bg-primary", "bg-success", "bg-warning", "bg-danger", "bg-info" };
        private static string GetColorForEmail(string email)
        {
            if (string.IsNullOrEmpty(email)) return "bg-secondary";
            return Colors[Math.Abs(email.GetHashCode()) % Colors.Length];
        }
    }
}
