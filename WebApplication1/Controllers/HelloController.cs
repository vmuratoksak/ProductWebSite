using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HelloController : Controller
    {
        // GET → sayfa ilk açılış
        [HttpGet]
        public IActionResult Index()
        {
            var model = new HelloViewModel
            {
                Message = TempData["Message"] as string ?? "Lütfen adınızı girin",
                Date = DateTime.Now
            };

            return View(model);
        }

        // POST → form gönderimi
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(HelloViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Message = "Lütfen adınızı girin";
                model.Date = DateTime.Now;
                return View(model); // ❗ redirect YOK
            }

            // ✔ başarılıysa mesajı TempData’ya koy
            TempData["Message"] = $"Merhaba {model.Name} 👋";

            // ✔ PRG
            return RedirectToAction(nameof(Index));
        }
    }
}
