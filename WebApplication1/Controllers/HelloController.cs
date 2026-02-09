using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using System;

namespace WebApplication1.Controllers
{
    public class HelloController : Controller
    {
        // GET → Formu göster
        [HttpGet]
        public IActionResult Index()
        {
            var model = new HelloViewModel
            {
                Message = "Lütfen adınızı girin",
                Date = DateTime.Now
            };

            // TempData varsa oku
            if (TempData["Message"] != null)
            {
                model.Message = TempData["Message"].ToString();
            }

            return View(model);
        }

        // POST → İşle → Redirect
        [HttpPost]
        public IActionResult Index(HelloViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Message"] = "Lütfen adınızı girin";
                return RedirectToAction("Index");
            }

            TempData["Message"] = $"Merhaba {model.Name} 👋";
            return RedirectToAction("Index");
        }
    }
}
