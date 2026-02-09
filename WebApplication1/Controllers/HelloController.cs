using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using System;

namespace WebApplication1.Controllers
{
    public class HelloController : Controller
    {
        // 1️⃣ SAYFA İLK AÇILDIĞINDA (GET)
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

        // 2️⃣ FORM GÖNDERİLDİĞİNDE (POST)
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

            // ✔️ Başarılıysa TempData
            TempData["Message"] = $"Merhaba {model.Name} 👋";

            // ✔️ PRG
            return RedirectToAction("Index");
        }
    }
}
