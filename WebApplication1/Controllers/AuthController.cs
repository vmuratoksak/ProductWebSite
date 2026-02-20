using Microsoft.AspNetCore.Mvc;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string Email, string Password)
        {
            var user = _authService.Login(Email, Password);

            if (user == null)
            {
                TempData["Error"] = "Email veya şifre yanlış!";
                return View();
            }

            // ✅ TÜM SESSION DEĞERLERİ SET EDİLİYOR
            HttpContext.Session.SetString("UserId", user.Id);
            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("UserRole", user.Role);
            HttpContext.Session.SetString("UserEmail", user.Email);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}