using Microsoft.AspNetCore.Mvc;
using SatisSitesi.Application.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;

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
        var role = HttpContext.Session.GetString("UserRole");

        if (string.IsNullOrEmpty(userId))
            return RedirectToAction("Login", "Auth");

        var model = _homeService.GetDashboardData(userId, role);
        return View(model);
    }

    [HttpGet]
    public IActionResult Search(string query)
    {
        var userId = HttpContext.Session.GetString("UserId");
        var role = HttpContext.Session.GetString("UserRole");

        var results = _homeService.GetGlobalSearchResults(query, userId, role);
        return View(results);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Technologies()
    {
        return View();
    }

    [HttpGet]
    public IActionResult GetChartData()
    {
        var userId = HttpContext.Session.GetString("UserId");
        var role = HttpContext.Session.GetString("UserRole");
        var model = _homeService.GetChartData(userId ?? "", role ?? "");
        return Json(model);
    }

    [HttpGet]
    public IActionResult GetCartCount()
    {
        var userId = HttpContext.Session.GetString("UserId");
        if (string.IsNullOrEmpty(userId)) return Json(0);
        
        // Note: In a real app we'd inject ICartService, but for brevity/existing patterns 
        // we assume we can get it from service provider or just return a mock/real count if we inject it.
        // Let's assume we can use the injected services if we add them to the constructor or use HttpContext.RequestServices
        var cartService = HttpContext.RequestServices.GetService<ICartService>();
        var count = cartService?.GetCartItems(userId).Count ?? 0;
        return Json(count);
    }

    [HttpGet]
    public IActionResult GetNotificationCount()
    {
        var role = HttpContext.Session.GetString("UserRole");
        var userId = HttpContext.Session.GetString("UserId");

        var orderService = HttpContext.RequestServices.GetService<IOrderService>();
        if (role == "Admin")
        {
            var count = orderService?.GetPendingOrdersCount() ?? 0;
            return Json(count);
        }
        else
        {
            var count = orderService?.GetUserNotificationCount(userId ?? "") ?? 0;
            return Json(count);
        }
    }

    [HttpGet]
    public IActionResult GetSearchSuggestions(string query)
    {
        var userId = HttpContext.Session.GetString("UserId");
        var role = HttpContext.Session.GetString("UserRole");
        var results = _homeService.GetGlobalSearchResults(query, userId, role);
        
        var suggestionsList = new List<dynamic>();

        if (results.Products != null)
            suggestionsList.AddRange(results.Products.Select(p => new { title = p.Name, type = "Ürün", url = $"/Product/Index?search={p.Name}" }));
        
        if (results.Users != null)
            suggestionsList.AddRange(results.Users.Select(u => new { title = u.Username, type = "Müşteri", url = $"/Customer/Index?search={u.Username}" }));

        if (results.Orders != null)
        {
            var orderAction = role == "Admin" ? "AdminOrders" : "MyOrders";
            suggestionsList.AddRange(results.Orders.Select(o => new { 
                title = $"Sipariş #{ (o.Id?.Length > 6 ? o.Id.Substring(o.Id.Length - 6) : o.Id) }", 
                type = "Sipariş", 
                url = $"/Order/{orderAction}?search={o.Id}" 
            }));
        }

        return Json(suggestionsList.Take(5));
    }
}
