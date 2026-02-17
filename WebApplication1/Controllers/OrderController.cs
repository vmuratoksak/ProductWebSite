using Microsoft.AspNetCore.Mvc;
using WebApplication1.Services.Interfaces;

public class OrderController : Controller
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    public IActionResult Checkout()
    {
        var userId = HttpContext.Session.GetString("UserId");
        var userEmail = HttpContext.Session.GetString("UserEmail");

        try
        {
            _orderService.Checkout(userId, userEmail);
            return RedirectToAction("MyOrders");
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction("Index", "Cart");
        }
    }
}
