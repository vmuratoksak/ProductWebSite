using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Services.Interfaces;
[Authorize]
public class CartController : Controller
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    public IActionResult Index()
    {
        var userId = HttpContext.Session.GetString("UserId");

        if (string.IsNullOrEmpty(userId))
            return RedirectToAction("Login", "Account");

        var cartItems = _cartService.GetCartItems(userId);
        return View(cartItems);
    }

    public IActionResult Add(string productId)
    {
        var userId = HttpContext.Session.GetString("UserId");

        if (string.IsNullOrEmpty(userId))
            return RedirectToAction("Login", "Account");

        _cartService.AddToCart(userId, productId);

        return RedirectToAction("Index");
    }



    public IActionResult Remove(string id)
    {
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
