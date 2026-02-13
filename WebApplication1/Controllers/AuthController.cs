using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using WebApplication1.Models;
using System;

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
            Console.WriteLine("REGISTER POST ÇALIŞTI");

            if (!ModelState.IsValid)
            {
                Console.WriteLine("MODEL INVALID");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }
                return View(model);
            }

            Console.WriteLine("MODEL VALID");

            model.CreatedAt = DateTime.Now;

            _collection.InsertOne(model);

            Console.WriteLine("KAYIT ATILDI");

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
        public IActionResult Login(UserEntity model)
        {
            var user = _collection
                .Find(x => x.Email == model.Email && x.Password == model.Password)
                .FirstOrDefault();

            if (user == null)
            {
                ViewBag.Error = "Email veya şifre yanlış.";
                return View(model);
            }

            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("Username", user.Username ?? user.Email);

            return RedirectToAction("Index", "Home");
        }







        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

    }
}
