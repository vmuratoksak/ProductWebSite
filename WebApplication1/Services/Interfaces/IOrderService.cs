using WebApplication1.Models.Entities;

namespace WebApplication1.Services.Interfaces
{
    public interface IOrderService
    {
        void Checkout(string userId, string userEmail);
        List<OrderEntity> GetUserOrders(string userId);
    }
}