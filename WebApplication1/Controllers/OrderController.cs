using Microsoft.AspNetCore.Mvc;
using WebApplication1.Services.Interfaces;
using Microsoft.AspNetCore.Http;

namespace WebApplication1.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        private bool IsUserLoggedIn()
        {
            return !string.IsNullOrEmpty(HttpContext.Session.GetString("UserId"));
        }

        public IActionResult Checkout()
        {
            if (!IsUserLoggedIn())
                return RedirectToAction("Login", "Auth");

            var userId = HttpContext.Session.GetString("UserId");

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
            if (!IsUserLoggedIn())
                return RedirectToAction("Login", "Auth");

            var userId = HttpContext.Session.GetString("UserId");
            var orders = _orderService.GetUserOrders(userId);

            return View(orders);
        }
    }
}