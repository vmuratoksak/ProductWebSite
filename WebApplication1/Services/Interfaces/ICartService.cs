using WebApplication1.Models.ViewModels;
using System.Collections.Generic;

namespace WebApplication1.Services.Interfaces
{
    public interface ICartService
    {
        void AddToCart(string userId, string productId);
        void RemoveFromCart(string cartId);
        void IncreaseQuantity(string cartId);
        void DecreaseQuantity(string cartId);

        List<CartViewModel> GetCartItems(string userId);
    }
}