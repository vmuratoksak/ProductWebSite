using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HelloController : Controller
    {
        // SAYFA AÇILIRKEN (GET)
        [HttpGet]
        public IActionResult Index()
        {
            var model = new HelloViewModel
            {
                Message = "Lütfen adınızı girin",
                Date = DateTime.Now
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Index(HelloViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Message = "Lütfen adınızı girin";
                model.Date = DateTime.Now;
                return View(model);
            }

            // 🔥 BAŞARILI DURUM
            model.Message = $"Merhaba {model.Name} 👋";
            model.Date = DateTime.Now;

            // ❗ Redirect YOK
            return View(model);
        }


    }
}
