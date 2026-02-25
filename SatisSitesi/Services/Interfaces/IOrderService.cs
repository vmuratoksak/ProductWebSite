using SatisSitesi.Models.Entities;
using System.Collections.Generic; // List<T> için gerekli

namespace SatisSitesi.Services.Interfaces
{
    public interface IOrderService
    {
        void Checkout(string userId, string userEmail);
        List<OrderEntity> GetUserOrders(string userId);
        List<OrderEntity> GetAllOrders();
        void UpdateOrderStatus(string orderId, string newStatus);
    }
}
