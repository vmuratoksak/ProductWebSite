using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMongoCollection<NameEntity> _nameCollection;
        private readonly IMongoCollection<ProductEntity> _productCollection;
        private readonly IMongoCollection<OrderEntity> _orderCollection;

        public HomeController(IMongoClient client, IOptions<MongoSettings> settings)
        {
            var database = client.GetDatabase(settings.Value.DatabaseName);

            _nameCollection = database.GetCollection<NameEntity>("Names");
            _productCollection = database.GetCollection<ProductEntity>("Products");
            _orderCollection = database.GetCollection<OrderEntity>("Orders");
        }

        public IActionResult Index()
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");

            if (string.IsNullOrEmpty(userEmail))
                return RedirectToAction("Login", "Auth");

            var totalNames = _nameCollection.CountDocuments(_ => true);
            var totalProducts = _productCollection.CountDocuments(_ => true);
            var totalOrders = _orderCollection.CountDocuments(_ => true);

            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            var todayAdded = _nameCollection.CountDocuments(x =>
                x.CreatedAt >= today && x.CreatedAt < tomorrow);

            var lastUpdatedName = _nameCollection
                .Find(_ => true)
                .SortByDescending(x => x.UpdatedAt)
                .FirstOrDefault();

            ViewBag.TotalNames = totalNames;
            ViewBag.TotalProducts = totalProducts;
            ViewBag.TotalOrders = totalOrders;
            ViewBag.TodayAdded = todayAdded;
            ViewBag.LastUpdatedName = lastUpdatedName?.Name ?? "Yok";

            return View();
        }


        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Technologies()
        {
            return View();
        }
    }
}
