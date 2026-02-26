using SatisSitesi.Domain.Entities;
using SatisSitesi.Application.Models;
using System.Collections.Generic; // List<T> için gerekli

namespace SatisSitesi.Application.Interfaces.Services
{
    public interface IOrderService
    {
        void Checkout(string userId, string userEmail);
        List<OrderEntity> GetUserOrders(string userId);
        OrderIndexModel GetPagedUserOrders(string userId, string search, string sortBy, int page, int pageSize);
        List<OrderEntity> GetAllOrders();
        OrderIndexModel GetPagedAdminOrders(string search, string sortBy, int page, int pageSize);
        void UpdateOrderStatus(string orderId, string newStatus);
    }
}
