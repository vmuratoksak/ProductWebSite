using WebApplication1.Models.Entities;
using System.Collections.Generic;

namespace WebApplication1.Services.Interfaces
{
    public interface IOrderService
    {
        void Checkout(string userId);
        List<OrderEntity> GetUserOrders(string userId);
    }
}