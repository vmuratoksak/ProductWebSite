using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class AuthController : Controller
    {
        private readonly IMongoCollection<UserEntity> _users;

        public AuthController(IMongoClient client, IOptions<MongoSettings> settings)
        {
            var database = client.GetDatabase(settings.Value.DatabaseName);
            _users = database.GetCollection<UserEntity>("Users");

            // Email için unique index
            var indexKeys = Builders<UserEntity>.IndexKeys.Ascending(x => x.Email);
            var indexOptions = new CreateIndexOptions { Unique = true };
            _users.Indexes.CreateOne(new CreateIndexModel<UserEntity>(indexKeys, indexOptions));
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

            try
            {
                model.CreatedAt = DateTime.Now;
                model.Role = "User";

                _users.InsertOne(model);

                TempData["Success"] = "Kayıt başarılı. Giriş yapabilirsiniz.";
                return RedirectToAction("Login");
            }
            catch
            {
                ModelState.AddModelError("", "Bu email zaten kayıtlı.");
                return View(model);
            }
        }

        // LOGIN PAGE
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var user = _users.Find(x => x.Email == email && x.Password == password).FirstOrDefault();

            if (user == null)
            {
                ViewBag.Error = "Email veya şifre hatalı.";
                return View();
            }

            HttpContext.Session.SetString("UserId", user.Id);
            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("Role", user.Role);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
