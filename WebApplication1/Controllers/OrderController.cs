using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using WebApplication1.Models;
using System.Linq;

namespace WebApplication1.Controllers
{
    public class OrderController : Controller
    {
        private readonly IMongoCollection<OrderEntity> _orderCollection;

        public OrderController(IMongoClient client, IOptions<MongoSettings> settings)
        {
            var database = client.GetDatabase(settings.Value.DatabaseName);
            _orderCollection = database.GetCollection<OrderEntity>("Orders");
        }

        public IActionResult MyOrders()
        {
            var userId = HttpContext.Session.GetString("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Auth");

            var orders = _orderCollection
                .Find(x => x.UserId == userId)
                .SortByDescending(x => x.CreatedAt)
                .ToList();

            return View(orders);
        }
    }
}
