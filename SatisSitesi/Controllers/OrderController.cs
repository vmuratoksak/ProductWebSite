using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using SatisSitesi.Services.Interfaces;

namespace SatisSitesi.Controllers
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
            var userEmail = HttpContext.Session.GetString("UserEmail");

            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Auth");

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

        public IActionResult MyOrders()
        {
            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Auth");

            var orders = _orderService.GetUserOrders(userId);

            return View(orders);
        }

        public IActionResult AdminOrders()
        {
            var role = HttpContext.Session.GetString("UserRole");

            if (role != "Admin")
            {
                TempData["Error"] = "Bu sayfaya erişim yetkiniz yok.";
                return RedirectToAction("Index", "Home");
            }

            var orders = _orderService.GetAllOrders();
            return View(orders);
        }

        [HttpPost]
        public IActionResult UpdateStatus(string orderId, string status)
        {
            var role = HttpContext.Session.GetString("UserRole");

            if (role != "Admin")
            {
                TempData["Error"] = "Bu işlemi yapma yetkiniz yok.";
                return RedirectToAction("Index", "Home");
            }

            try
            {
                _orderService.UpdateOrderStatus(orderId, status);
                TempData["Success"] = "Sipariş durumu güncellendi.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("AdminOrders");
        }
    }
}
