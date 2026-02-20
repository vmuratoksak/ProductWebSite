using WebApplication1.Services.Interfaces;
using WebApplication1.Repositories.Interfaces;
using WebApplication1.Models.Entities;
using System;
using System.Linq;
using System.Collections.Generic;
using WebApplication1.Models.ViewModels;

namespace WebApplication1.Services
{
    public class CartService : ICartService
    {
        private readonly IRepository<CartEntity> _cartRepo;
        private readonly IRepository<ProductEntity> _productRepo;

        public CartService(
            IRepository<CartEntity> cartRepo,
            IRepository<ProductEntity> productRepo)
        {
            _cartRepo = cartRepo;
            _productRepo = productRepo;
        }

        public void AddToCart(string userId, string productId)
        {
            Console.WriteLine("AddToCart çalıştı");
            if (string.IsNullOrEmpty(userId))
                throw new Exception("Kullanıcı giriş yapmamış.");

            var product = _productRepo.GetById(productId);

            if (product == null)
                throw new Exception("Ürün bulunamadı.");

            if (product.Stock <= 0)
                throw new Exception("Stokta ürün yok.");

            var existingItem = _cartRepo.GetAll()
                .FirstOrDefault(x => x.UserId == userId && x.ProductId == productId);

            if (existingItem != null)
            {
                if (product.Stock <= existingItem.Quantity)
                    throw new Exception("Stok yetersiz.");

                existingItem.Quantity += 1;
                _cartRepo.Update(existingItem.Id, existingItem);
            }
            else
            {
                var newItem = new CartEntity
                {
                    UserId = userId,
                    ProductId = productId,
                    Quantity = 1,
                    AddedAt = DateTime.UtcNow
                };

                _cartRepo.Insert(newItem);
            }
        }

        public void RemoveFromCart(string cartId)
        {
            _cartRepo.Delete(cartId);
        }

        public void IncreaseQuantity(string cartId)
        {
            var item = _cartRepo.GetById(cartId);
            if (item == null)
                throw new Exception("Sepet öğesi bulunamadı.");

            var product = _productRepo.GetById(item.ProductId);

            if (product.Stock <= item.Quantity)
                throw new Exception("Stok yetersiz.");

            item.Quantity += 1;
            _cartRepo.Update(item.Id, item);
        }

        public void DecreaseQuantity(string cartId)
        {
            var item = _cartRepo.GetById(cartId);
            if (item == null)
                throw new Exception("Sepet öğesi bulunamadı.");

            if (item.Quantity <= 1)
            {
                _cartRepo.Delete(cartId);
                return;
            }

            item.Quantity -= 1;
            _cartRepo.Update(item.Id, item);
        }

        public List<CartViewModel> GetCartItems(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return new List<CartViewModel>();

            var cartItems = _cartRepo.GetAll()
                .Where(x => x.UserId == userId)
                .ToList();

            var result = cartItems.Select(item =>
            {
                var product = _productRepo.GetById(item.ProductId);

                return new CartViewModel
                {
                    Id = item.Id,
                    ProductId = item.ProductId,
                    ProductName = product?.Name ?? "Ürün silinmiş",
                    Price = product?.Price ?? 0,
                    Quantity = item.Quantity
                };
            }).ToList();

            return result;
        }
    }
}