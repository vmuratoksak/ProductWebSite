using Microsoft.AspNetCore.Mvc;
using WebApplication1.Services.Interfaces;

public class HomeController : Controller
{
    private readonly IHomeService _homeService;

    public HomeController(IHomeService homeService)
    {
        _homeService = homeService;
    }

    public IActionResult Index()
    {
        var userEmail = HttpContext.Session.GetString("UserEmail");

        if (string.IsNullOrEmpty(userEmail))
            return RedirectToAction("Login", "Auth");

        var model = _homeService.GetDashboardData();

        return View(model);
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
