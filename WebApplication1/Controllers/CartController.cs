using Microsoft.AspNetCore.Mvc;
using WebApplication1.Services.Interfaces;

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

        if (string.IsNullOrEmpty(userId))
            return RedirectToAction("Login", "Auth");

        try
        {
            _cartService.AddToCart(userId, productId);
            TempData["Success"] = "Ürün sepete eklendi.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction("Index", "Product");
    }

    public IActionResult Remove(string id)
    {
        if (!IsUserLoggedIn())
            return RedirectToAction("Login", "Auth");

        _cartService.RemoveFromCart(id);
        return RedirectToAction("Index");
    }

    public IActionResult Increase(string id)
    {
        try
        {
            _cartService.IncreaseQuantity(id);
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction("Index");
    }

    public IActionResult Decrease(string id)
    {
        try
        {
            _cartService.DecreaseQuantity(id);
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction("Index");
    }
}