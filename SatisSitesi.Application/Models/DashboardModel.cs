using SatisSitesi.Domain.Entities;
using System.Collections.Generic;

namespace SatisSitesi.Application.Models
{
    public class DashboardModel
    {
        public decimal TotalRevenue { get; set; }
        public string RevenueGrowth { get; set; }
        public bool IsRevenueUp { get; set; }
        
        public int ActiveUsers { get; set; }
        public string UsersGrowth { get; set; }
        public bool IsUsersUp { get; set; }
        
        public int Sales { get; set; }
        public string SalesGrowth { get; set; }
        public bool IsSalesUp { get; set; }
        
        public int ActiveProducts { get; set; }
        public string ProductsGrowth { get; set; }
        public bool IsProductsUp { get; set; }
        
        public List<RecentSaleModel> RecentSales { get; set; } = new List<RecentSaleModel>();
        public List<TopProductModel> TopProducts { get; set; } = new List<TopProductModel>();
        public int TotalRecentSalesCount { get; set; }
    }

    public class ChartDataModel 
    {
        public List<decimal> RevenueChartData { get; set; } = new List<decimal>();
        public List<int> OrdersChartData { get; set; } = new List<int>();
        public List<string> ChartLabels { get; set; } = new List<string>();
    }

    public class RecentSaleModel
    {
        public string Initials { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public decimal Amount { get; set; }
        public string ColorClass { get; set; }
    }

    public class TopProductModel
    {
        public string Name { get; set; }
        public int SalesCount { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public int ProgressPercentage { get; set; }
    }
}
