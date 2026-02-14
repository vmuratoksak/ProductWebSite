using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using WebApplication1.Models;
using System;
using System.Linq;
using System.Collections.Generic;

namespace WebApplication1.Controllers
{
    public class CartController : Controller
    {
        private readonly IMongoCollection<CartEntity> _cartCollection;
        private readonly IMongoCollection<ProductEntity> _productCollection;
        private readonly IMongoCollection<OrderEntity> _orderCollection;

        public CartController(IMongoClient client, IOptions<MongoSettings> settings)
        {
            var database = client.GetDatabase(settings.Value.DatabaseName);

            _cartCollection = database.GetCollection<CartEntity>("Carts");
            _productCollection = database.GetCollection<ProductEntity>("Products");
            _orderCollection = database.GetCollection<OrderEntity>("Orders");
        }

        // SEPET LİSTELE
        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetString("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Auth");

            var cartItems = _cartCollection.Find(x => x.UserId == userId).ToList();

            var viewModel = new List<CartViewModel>();

            foreach (var item in cartItems)
            {
                var product = _productCollection.Find(x => x.Id == item.ProductId).FirstOrDefault();

                if (product != null)
                {
                    viewModel.Add(new CartViewModel
                    {
                        CartId = item.Id,
                        ProductId = product.Id,
                        ProductName = product.Name,
                        Price = product.Price,
                        Quantity = item.Quantity
                    });

                }
            }

            ViewBag.GrandTotal = viewModel.Sum(x => x.TotalPrice);

            return View(viewModel);
        }

        // SEPETE EKLE
        public IActionResult Add(string productId)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Auth");

            var product = _productCollection
                .Find(x => x.Id == productId)
                .FirstOrDefault();

            if (product == null)
                return RedirectToAction("Index", "Product");

            if (product.Stock <= 0)
            {
                TempData["Error"] = "Bu ürün stokta yok.";
                return RedirectToAction("Index", "Product");
            }

            var existingItem = _cartCollection
                .Find(x => x.UserId == userId && x.ProductId == productId)
                .FirstOrDefault();

            if (existingItem != null)
            {
                if (existingItem.Quantity >= product.Stock)
                {
                    TempData["Error"] = "Stok limitine ulaştınız.";
                    return RedirectToAction("Index", "Product");
                }

                var update = Builders<CartEntity>.Update
                    .Inc(x => x.Quantity, 1);

                _cartCollection.UpdateOne(x => x.Id == existingItem.Id, update);
            }
            else
            {
                _cartCollection.InsertOne(new CartEntity
                {
                    UserId = userId,
                    ProductId = productId,
                    Quantity = 1,
                    AddedAt = DateTime.Now
                });
            }

            return RedirectToAction("Index", "Product");
        }


        // SEPETTEN SİL
        public IActionResult Remove(string id)
        {
            _cartCollection.DeleteOne(x => x.Id == id);
            return RedirectToAction("Index");
        }

        // CHECKOUT
        public IActionResult Checkout()
        {
            var userId = HttpContext.Session.GetString("UserId");
            var userEmail = HttpContext.Session.GetString("UserEmail");

            if (userId == null)
                return RedirectToAction("Login", "Auth");

            var cartItems = _cartCollection
                .Find(x => x.UserId == userId)
                .ToList();

            if (!cartItems.Any())
                return RedirectToAction("Index");

            var orderItems = new List<OrderItem>();
            decimal total = 0;

            foreach (var cartItem in cartItems)
            {
                var product = _productCollection
                    .Find(x => x.Id == cartItem.ProductId)
                    .FirstOrDefault();

                if (product == null)
                    continue;

                // ✅ STOK KONTROLÜ
                if (product.Stock < cartItem.Quantity)
                {
                    TempData["Error"] = $"{product.Name} için yeterli stok yok.";
                    return RedirectToAction("Index");
                }

                // ✅ STOK DÜŞÜR
                var stockUpdate = Builders<ProductEntity>.Update
                    .Inc(x => x.Stock, -cartItem.Quantity);

                var updateResult = _productCollection.UpdateOne(
                    x => x.Id == product.Id,
                    stockUpdate
                );

                Console.WriteLine("STOK UPDATE COUNT: " + updateResult.ModifiedCount);

                total += product.Price * cartItem.Quantity;

                orderItems.Add(new OrderItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Price = product.Price,
                    Quantity = cartItem.Quantity
                });
            }

            var order = new OrderEntity
            {
                UserId = userId,
                UserEmail = userEmail,
                Items = orderItems,
                TotalAmount = total,
                CreatedAt = DateTime.Now
            };

            _orderCollection.InsertOne(order);

            _cartCollection.DeleteMany(x => x.UserId == userId);

            return RedirectToAction("MyOrders", "Order");
        }


        
            
            public IActionResult Increase(string id)
        {
            var cartItem = _cartCollection.Find(x => x.Id == id).FirstOrDefault();
            var product = _productCollection.Find(x => x.Id == cartItem.ProductId).FirstOrDefault();

            if (cartItem.Quantity < product.Stock)
            {
                var update = Builders<CartEntity>.Update.Inc(x => x.Quantity, 1);
                _cartCollection.UpdateOne(x => x.Id == id, update);
            }

            return RedirectToAction("Index");
        }
        public IActionResult Decrease(string id)
        {
            var cartItem = _cartCollection.Find(x => x.Id == id).FirstOrDefault();

            if (cartItem.Quantity > 1)
            {
                var update = Builders<CartEntity>.Update.Inc(x => x.Quantity, -1);
                _cartCollection.UpdateOne(x => x.Id == id, update);
            }

            return RedirectToAction("Index");
        }


    }
}

