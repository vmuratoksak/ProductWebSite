using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Controllers
{
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

            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Auth");

            try
            {
                _orderService.Checkout(userId);
                return RedirectToAction("MyOrders");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index", "Cart");
            }
        }

        public IActionResult MyOrders()
        {
            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Auth");

            var orders = _orderService.GetUserOrders(userId);

            return View(orders);
        }
    }
}