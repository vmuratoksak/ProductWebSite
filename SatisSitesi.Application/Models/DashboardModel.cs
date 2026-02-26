namespace SatisSitesi.Application.Models
{
    public class DashboardModel
    {
        public int TotalNames { get; set; }
        public int TodayNames { get; set; }
        public string LastUpdatedName { get; set; }

        public int TotalProducts { get; set; }
        public int TotalOrders { get; set; }
    }
}
