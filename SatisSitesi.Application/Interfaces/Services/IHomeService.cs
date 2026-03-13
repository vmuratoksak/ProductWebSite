using SatisSitesi.Application.Models;
using SatisSitesi.Domain.Entities;
namespace SatisSitesi.Application.Interfaces.Services
    
{
    public interface IHomeService
    {
        DashboardModel GetDashboardData(string userId, string role);
        ChartDataModel GetChartData(string userId, string role);
        GlobalSearchViewModel GetGlobalSearchResults(string query, string userId, string role);
        void SaveContactMessage(ContactMessageEntity message);
        List<ContactMessageEntity> GetContactMessages();
        void DeleteContactMessage(string id);
    }
}
