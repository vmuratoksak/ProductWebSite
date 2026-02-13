using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMongoCollection<NameEntity> _collection;

        public HomeController(IMongoClient client, IOptions<MongoSettings> settings)
        {
            var database = client.GetDatabase(settings.Value.DatabaseName);
            _collection = database.GetCollection<NameEntity>(settings.Value.CollectionName);

        }

        // Dashboard
        public IActionResult Index()
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");

            if (string.IsNullOrEmpty(userEmail))
                return RedirectToAction("Login", "Auth");

            var totalNames = _collection.CountDocuments(_ => true);

            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            var todayAdded = _collection.CountDocuments(x =>
                x.CreatedAt >= today && x.CreatedAt < tomorrow);

            ViewBag.TotalNames = totalNames;
            ViewBag.TodayAdded = todayAdded;

            return View();
        }


        // Yönetim Paneli
        public IActionResult Privacy()
        {
            return View();
        }
    }
}
