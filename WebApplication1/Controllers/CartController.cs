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

            var existingItem = _cartCollection
                .Find(x => x.UserId == userId && x.ProductId == productId)
                .FirstOrDefault();

            if (existingItem != null)
            {
                var update = Builders<CartEntity>.Update
                    .Inc(x => x.Quantity, 1);

                _cartCollection.UpdateOne(x => x.Id == existingItem.Id, update);
            }
            else
            {
                var cartItem = new CartEntity
                {
                    UserId = userId,
                    ProductId = productId,
                    Quantity = 1,
                    AddedAt = DateTime.Now
                };

                _cartCollection.InsertOne(cartItem);
            }

            return RedirectToAction("Index");
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

            var cartItems = _cartCollection.Find(x => x.UserId == userId).ToList();

            if (!cartItems.Any())
                return RedirectToAction("Index");

            var orderItems = new List<OrderItem>();
            decimal total = 0;

            foreach (var item in cartItems)
            {
                var product = _productCollection.Find(x => x.Id == item.ProductId).FirstOrDefault();

                if (product != null)
                {
                    var orderItem = new OrderItem
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        Price = product.Price,
                        Quantity = item.Quantity
                    };

                    total += product.Price * item.Quantity;

                    orderItems.Add(orderItem);
                }
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
    }
}
