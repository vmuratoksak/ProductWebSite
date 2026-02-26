using SatisSitesi.Domain.Entities;
using SatisSitesi.Application.Models;
using SatisSitesi.Application.Interfaces.Repositories;
using SatisSitesi.Application.Interfaces.Services;

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

    public DashboardModel GetDashboardData()
    {
        var names = _nameRepo.GetAll();
        var products = _productRepo.GetAll();
        var orders = _orderRepo.GetAll();

        return new DashboardModel
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
