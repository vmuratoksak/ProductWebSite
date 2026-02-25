using Microsoft.AspNetCore.Mvc;
using SatisSitesi.Models.Entities;
using SatisSitesi.Services.Interfaces;

namespace SatisSitesi.Controllers
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

        // ================= REGISTER =================

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(string Username, string Email, string Password)
        {
            try
            {
                var newUser = new UserEntity
                {
                    Username = Username,
                    Email = Email,
                    Password = Password
                };

                _authService.Register(newUser);

                TempData["Success"] = "Kayıt başarılı! Giriş yapabilirsiniz.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View();
            }
        }


        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
