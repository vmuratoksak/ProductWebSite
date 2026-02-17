using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using WebApplication1.Models;
using System;
using WebApplication1.Models.Entities;

namespace WebApplication1.Controllers
{
    public class AuthController : Controller
    {
        
        private readonly IMongoCollection<UserEntity> _collection;
        public AuthController(IMongoClient client, IOptions<MongoSettings> settings)
        {
            var database = client.GetDatabase(settings.Value.DatabaseName);
            _collection = database.GetCollection<UserEntity>("Users");

            // Email için unique index
            var indexKeys = Builders<UserEntity>.IndexKeys.Ascending(x => x.Email);
            var indexOptions = new CreateIndexOptions { Unique = true };
            _collection.Indexes.CreateOne(new CreateIndexModel<UserEntity>(indexKeys, indexOptions));
        }

        // REGISTER PAGE
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(UserEntity model)
        {
            if (!ModelState.IsValid)
                return View(model);

            model.CreatedAt = DateTime.Now;

            // Eğer özel bir email ise admin yap (geçici sistem)
            if (model.Email == "admin@site.com")
                model.Role = "Admin";
            else
                model.Role = "User";

            _collection.InsertOne(model);

            return RedirectToAction("Login");
        }

        // LOGIN PAGE
        // LOGIN GET
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // LOGIN POST
        [HttpPost]
        public IActionResult Login(string Email, string password)
        {
            var user = _collection
                .Find(x => x.Email == Email && x.Password == password)
                .FirstOrDefault();

            if (user == null)
            {
                ViewBag.Error = "Kullanıcı adı veya şifre yanlış.";
                return View();
            }

            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("UserRole", user.Role);
            HttpContext.Session.SetString("UserId", user.Id);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
