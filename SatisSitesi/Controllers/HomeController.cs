using Microsoft.AspNetCore.Mvc;
using SatisSitesi.Application.Interfaces.Services;

public class HomeController : Controller
{
    private readonly IHomeService _homeService;

    public HomeController(IHomeService homeService)
    {
        _homeService = homeService;
    }

    public IActionResult Index()
    {
        var userId = HttpContext.Session.GetString("UserId");

        if (string.IsNullOrEmpty(userId))
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
