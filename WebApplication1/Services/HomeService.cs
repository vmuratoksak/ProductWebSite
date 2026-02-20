using WebApplication1.Models.Entities;
using WebApplication1.Models.ViewModels;
using WebApplication1.Repositories.Interfaces;
using WebApplication1.Services.Interfaces;

public class HomeService : IHomeService
{
    private readonly IRepository<NameEntity> _nameRepo;
    private readonly IRepository<ProductEntity> _productRepo;
    private readonly IRepository<OrderEntity> _orderRepo;

    public HomeService(
        IRepository<NameEntity> nameRepo,
        IRepository<ProductEntity> productRepo,
        IRepository<OrderEntity> orderRepo)
    {
        _nameRepo = nameRepo;
        _productRepo = productRepo;
        _orderRepo = orderRepo;
    }

    public DashboardViewModel GetDashboardData()
    {
        var names = _nameRepo.GetAll();
        var products = _productRepo.GetAll();
        var orders = _orderRepo.GetAll();

        return new DashboardViewModel
        {
            TotalNames = names.Count(),
            TodayNames = names.Count(x => x.CreatedAt.Date == DateTime.Today),
            LastUpdatedName = names
                .OrderByDescending(x => x.UpdatedAt)
                .FirstOrDefault()?.Name ?? "-",

            TotalProducts = products.Count(),
            TotalOrders = orders.Count()
        };
    }
}