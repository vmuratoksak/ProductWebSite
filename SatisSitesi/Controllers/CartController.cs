using Microsoft.AspNetCore.Mvc;
using SatisSitesi.Application.Interfaces.Services;

public class CartController : Controller
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    private bool IsUserLoggedIn()
    {
        return !string.IsNullOrEmpty(HttpContext.Session.GetString("UserId"));
    }

    public IActionResult Index()
    {
        var userId = HttpContext.Session.GetString("UserId");

        if (string.IsNullOrEmpty(userId))
            return RedirectToAction("Login", "Auth");

        var cartItems = _cartService.GetCartItems(userId);
        return View(cartItems);
    }

    public IActionResult Add(string productId)
    {
        var userId = HttpContext.Session.GetString("UserId");
        var role = HttpContext.Session.GetString("UserRole");

        if (string.IsNullOrEmpty(userId))
            return RedirectToAction("Login", "Auth");

        // ✅ ADMIN SEPETE EKLEYEMEZ
        if (role == "Admin")
        {
            TempData["Error"] = "Admin kullanıcı sepete ürün ekleyemez.";
            return RedirectToAction("Index", "Product");
        }

        try
        {
            _cartService.AddToCart(userId, productId);
            TempData["Success"] = "Ürün sepete eklendi.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction("Index");
    }
    public IActionResult Remove(string id)
    {
        var userId = HttpContext.Session.GetString("UserId");
        if (string.IsNullOrEmpty(userId))
            return RedirectToAction("Login", "Auth");

        _cartService.RemoveFromCart(userId, id);
        return RedirectToAction("Index");
    }

    public IActionResult Increase(string id)
    {
        var userId = HttpContext.Session.GetString("UserId");
        if (string.IsNullOrEmpty(userId))
            return RedirectToAction("Login", "Auth");

        try
        {
            _cartService.IncreaseQuantity(userId, id);
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction("Index");
    }

    public IActionResult Decrease(string id)
    {
        var userId = HttpContext.Session.GetString("UserId");
        if (string.IsNullOrEmpty(userId))
            return RedirectToAction("Login", "Auth");

        try
        {
            _cartService.DecreaseQuantity(userId, id);
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction("Index");
    }
}
