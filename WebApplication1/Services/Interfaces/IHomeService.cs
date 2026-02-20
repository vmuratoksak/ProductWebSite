using WebApplication1.Models.ViewModels;
namespace WebApplication1.Services.Interfaces
{
    public interface IHomeService
    {
        DashboardViewModel GetDashboardData();
    }
}