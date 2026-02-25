using SatisSitesi.Models.ViewModels;
using System.Collections.Generic;

namespace SatisSitesi.Services.Interfaces
{
    public interface ICartService
    {
        void AddToCart(string userId, string productId);
        void RemoveFromCart(string userId, string productId);
        void IncreaseQuantity(string userId, string productId);
        void DecreaseQuantity(string userId, string productId);

        List<CartViewModel> GetCartItems(string userId);
    }
}
