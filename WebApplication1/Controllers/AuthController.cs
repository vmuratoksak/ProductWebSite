using Microsoft.AspNetCore.Mvc;
using WebApplication1.Services.Interfaces;
using WebApplication1.Models.Entities;

public class AuthController : Controller
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Register(UserEntity user)
    {
        try
        {
            _authService.Register(user);
            return RedirectToAction("Login");
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            return View(user);
        }
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Login(UserEntity user)
    {
        var existingUser = _authService.Login(user.Email, user.Password);

        if (existingUser == null)
        {
            TempData["Error"] = "Email veya şifre yanlış!";
            return RedirectToAction("Login");
        }

        HttpContext.Session.SetString("UserEmail", existingUser.Email);
        HttpContext.Session.SetString("Username", existingUser.Username);

        TempData["Success"] = "Hoşgeldin " + existingUser.Username;

        return RedirectToAction("Index", "Home");
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();

        TempData["Info"] = "Çıkış yapıldı";

        return RedirectToAction("Index", "Home");
    }
}
