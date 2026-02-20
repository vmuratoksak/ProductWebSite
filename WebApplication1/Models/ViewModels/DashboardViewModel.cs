namespace WebApplication1.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalNames { get; set; }
        public int TodayNames { get; set; }
        public string LastUpdatedName { get; set; }

        public int TotalProducts { get; set; }
        public int TotalOrders { get; set; }
    }
}