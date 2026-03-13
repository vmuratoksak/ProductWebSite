using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using SatisSitesi.Application.Interfaces.Services;

namespace SatisSitesi.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // Redirect /Order to the correct page based on role
        public IActionResult Index()
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role == "Admin")
                return RedirectToAction("AdminOrders");
            return RedirectToAction("MyOrders");
        }

        [HttpGet]
        public IActionResult Address()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Auth");
                
            ViewData["Title"] = "Teslimat Adresi";
            return View();
        }

        [HttpPost]
        public IActionResult Address(string addressDetails)
        {
            var userId = HttpContext.Session.GetString("UserId");
            var userEmail = HttpContext.Session.GetString("UserEmail");

            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Auth");

            // Basit doğrulama
            if (string.IsNullOrWhiteSpace(addressDetails) || addressDetails.Length < 10)
            {
                TempData["Error"] = "Geçerli ve açık bir adres giriniz.";
                return View();
            }

            try
            {
                _orderService.Checkout(userId, userEmail, addressDetails);
                TempData["Success"] = "Siparişiniz başarıyla alındı!";
                return RedirectToAction("MyOrders");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index", "Cart");
            }
        }

        public IActionResult MyOrders(string search, string sortBy, int page = 1)
        {
            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Auth");

            ViewData["Title"] = "Siparişlerim";
            var model = _orderService.GetPagedUserOrders(userId, search, sortBy, page, 8); // sayfa başı 8 sipariş

            return View(model);
        }

        public IActionResult AdminOrders(string search, string sortBy, int page = 1)
        {
            var role = HttpContext.Session.GetString("UserRole");

            if (role != "Admin")
            {
                TempData["Error"] = "Bu sayfaya erişim yetkiniz yok.";
                return RedirectToAction("Index", "Home");
            }

            ViewData["Title"] = "Tüm Siparişler (Admin)";
            var model = _orderService.GetPagedAdminOrders(search, sortBy, page, 8); // sayfa başı 8 sipariş
            return View(model);
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
        public IActionResult Details(string id)
        {
            var userId = HttpContext.Session.GetString("UserId");
            var role = HttpContext.Session.GetString("UserRole");

            if (string.IsNullOrEmpty(userId) && role != "Admin")
                return RedirectToAction("Login", "Auth");

            var order = _orderService.GetOrderById(id);
            
            if (order == null)
            {
                TempData["Error"] = "Siparis bulunamadi.";
                return RedirectToAction("Index", "Home");
            }

            // A user can only see their own order, unless they are Admin
            if (role != "Admin" && order.UserId != userId)
            {
                TempData["Error"] = "Bu siparise eisim yetkiniz yok.";
                return RedirectToAction("MyOrders");
            }

            ViewData["Title"] = $"Sipariş Detayı #{order.Id.Substring(0, 8)}";
            return View(order);
        }
    }
}
