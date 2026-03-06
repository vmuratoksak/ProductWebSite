using SatisSitesi.Application.Models;
namespace SatisSitesi.Application.Interfaces.Services
{
    public interface IHomeService
    {
        DashboardModel GetDashboardData(string userId, string role);
        ChartDataModel GetChartData(string userId, string role);
        GlobalSearchViewModel GetGlobalSearchResults(string query, string userId, string role);
    }
}
