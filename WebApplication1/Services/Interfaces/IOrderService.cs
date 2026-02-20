using WebApplication1.Models.Entities;
using System.Collections.Generic; // List<T> için gerekli

namespace WebApplication1.Services.Interfaces
{
    public interface IOrderService
    {
        void Checkout(string userId);
        List<OrderEntity> GetUserOrders(string userId);   // 🔥 EKLE
    }
}