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
        return !string.IsNullOrEmpty(HttpContext.Session.GetString("UserEmail"));
    }

    public IActionResult Index()
    {
        if (!IsUserLoggedIn())
            return RedirectToAction("Login", "Auth");

        var userEmail = HttpContext.Session.GetString("UserEmail");
        var cartItems = _cartService.GetCartItems(userEmail);

        return View(cartItems);
    }

    public IActionResult Add(string productId)
    {
        var userId = HttpContext.Session.GetString("UserId");

        if (string.IsNullOrEmpty(userId))
            return RedirectToAction("Login", "Auth");

        _cartService.AddToCart(userId, productId);

        return RedirectToAction("Index");
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