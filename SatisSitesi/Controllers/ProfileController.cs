using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using SatisSitesi.Application.Interfaces.Services;
using SatisSitesi.Domain.Entities;
using System;

namespace SatisSitesi.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IAuthService _authService;

        public ProfileController(IAuthService authService)
        {
            _authService = authService;
        }

        private string CurrentUserId => HttpContext.Session.GetString("UserId");

        public IActionResult Index()
        {
            if (string.IsNullOrEmpty(CurrentUserId))
                return RedirectToAction("Login", "Auth");

            var user = _authService.GetUserById(CurrentUserId);
            return View(user);
        }

        [HttpPost]
        public IActionResult UpdateProfile(string Username, string Email)
        {
            if (string.IsNullOrEmpty(CurrentUserId))
                return RedirectToAction("Login", "Auth");

            try
            {
                _authService.UpdateProfile(CurrentUserId, Username, Email);
                
                // Update session
                HttpContext.Session.SetString("Username", Username);
                HttpContext.Session.SetString("UserEmail", Email);

                TempData["Success"] = "Profiliniz başarıyla güncellendi.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Index");
        }

        public IActionResult Settings()
        {
            if (string.IsNullOrEmpty(CurrentUserId))
                return RedirectToAction("Login", "Auth");

            return View();
        }

        [HttpPost]
        public IActionResult ChangePassword(string CurrentPassword, string NewPassword, string ConfirmPassword)
        {
            if (string.IsNullOrEmpty(CurrentUserId))
                return RedirectToAction("Login", "Auth");

            if (NewPassword != ConfirmPassword)
            {
                TempData["Error"] = "Şifreler uyuşmuyor.";
                return RedirectToAction("Settings");
            }

            try
            {
                _authService.ChangePassword(CurrentUserId, CurrentPassword, NewPassword);
                TempData["Success"] = "Şifreniz başarıyla değiştirildi.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Settings");
        }

        [HttpPost]
        public IActionResult DeleteAccount()
        {
            if (string.IsNullOrEmpty(CurrentUserId))
                return RedirectToAction("Login", "Auth");

            try
            {
                _authService.DeleteAccount(CurrentUserId);
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Auth");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Settings");
            }
        }
    }
}
