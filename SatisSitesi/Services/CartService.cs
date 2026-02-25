using SatisSitesi.Services.Interfaces;
using SatisSitesi.Repositories.Interfaces;
using SatisSitesi.Models.Entities;
using System;
using System.Linq;
using System.Collections.Generic;
using SatisSitesi.Models.ViewModels;

namespace SatisSitesi.Services
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
           
            if (string.IsNullOrEmpty(userId))
                throw new Exception("Kullanıcı giriş yapmamış.");

            var product = _productRepo.GetById(productId);

            if (product == null)
                throw new Exception("Ürün bulunamadı.");

            if (product.Stock <= 0)
                throw new Exception("Stokta ürün yok.");

            var cart = _cartRepo.GetAll().FirstOrDefault(x => x.UserId == userId);

            if (cart != null)
            {
                var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);
                if (existingItem != null)
                {
                    if (product.Stock <= existingItem.Quantity)
                        throw new Exception("Stok yetersiz.");

                    existingItem.Quantity += 1;
                }
                else
                {
                    cart.Items.Add(new CartItem
                    {
                        ProductId = productId,
                        Quantity = 1,
                        AddedAt = DateTime.UtcNow
                    });
                }
                cart.UpdatedAt = DateTime.UtcNow;
                _cartRepo.Update(cart.Id, cart);
            }
            else
            {
                var newCart = new CartEntity
                {
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Items = new List<CartItem>
                    {
                        new CartItem
                        {
                            ProductId = productId,
                            Quantity = 1,
                            AddedAt = DateTime.UtcNow
                        }
                    }
                };

                _cartRepo.Insert(newCart);
            }
        }

        public void RemoveFromCart(string userId, string productId)
        {
            var cart = _cartRepo.GetAll().FirstOrDefault(x => x.UserId == userId);
            if (cart != null)
            {
                var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
                if (item != null)
                {
                    cart.Items.Remove(item);
                    if (cart.Items.Count == 0)
                    {
                        _cartRepo.Delete(cart.Id);
                    }
                    else
                    {
                        cart.UpdatedAt = DateTime.UtcNow;
                        _cartRepo.Update(cart.Id, cart);
                    }
                }
            }
        }

        public void IncreaseQuantity(string userId, string productId)
        {
            var cart = _cartRepo.GetAll().FirstOrDefault(x => x.UserId == userId);
            if (cart == null)
                throw new Exception("Sepet bulunamadı.");

            var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (item == null)
                throw new Exception("Sepet öğesi bulunamadı.");

            var product = _productRepo.GetById(productId);

            if (product.Stock <= item.Quantity)
                throw new Exception("Stok yetersiz.");

            item.Quantity += 1;
            cart.UpdatedAt = DateTime.UtcNow;
            _cartRepo.Update(cart.Id, cart);
        }

        public void DecreaseQuantity(string userId, string productId)
        {
            var cart = _cartRepo.GetAll().FirstOrDefault(x => x.UserId == userId);
            if (cart == null)
                throw new Exception("Sepet bulunamadı.");

            var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (item == null)
                throw new Exception("Sepet öğesi bulunamadı.");

            if (item.Quantity <= 1)
            {
                cart.Items.Remove(item);
                if (cart.Items.Count == 0)
                {
                    _cartRepo.Delete(cart.Id);
                    return;
                }
            }
            else
            {
                item.Quantity -= 1;
            }

            cart.UpdatedAt = DateTime.UtcNow;
            _cartRepo.Update(cart.Id, cart);
        }

        public List<CartViewModel> GetCartItems(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return new List<CartViewModel>();

            var cart = _cartRepo.GetAll().FirstOrDefault(x => x.UserId == userId);
            if (cart == null || cart.Items == null || !cart.Items.Any())
                return new List<CartViewModel>();

            var result = cart.Items.Select(item =>
            {
                var product = _productRepo.GetById(item.ProductId);

                return new CartViewModel
                {
                    Id = item.ProductId, // Ensure Id in the view corresponds to ProductId inherently
                    ProductId = item.ProductId,
                    ProductName = product?.Name ?? "Ürün silinmiş",
                    Price = product?.Price ?? 0,
                    Quantity = item.Quantity,
                    NameTranslations = product?.NameTranslations ?? new Dictionary<string, string>()
                };
            }).ToList();

            return result;
        }
    }
}
