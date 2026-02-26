using SatisSitesi.Application.Models;
using System.Collections.Generic;

namespace SatisSitesi.Application.Interfaces.Services
{
    public interface ICartService
    {
        void AddToCart(string userId, string productId);
        void RemoveFromCart(string userId, string productId);
        void IncreaseQuantity(string userId, string productId);
        void DecreaseQuantity(string userId, string productId);

        List<CartModel> GetCartItems(string userId);
    }
}
