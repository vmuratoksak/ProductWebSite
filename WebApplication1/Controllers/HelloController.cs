using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using System;

namespace WebApplication1.Controllers
{
    public class HelloController : Controller
    {
        // SAYFA İLK AÇILDIĞINDA
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

        // FORM GÖNDERİLDİĞİNDE
        [HttpPost]
        public IActionResult Index(HelloViewModel model)
        {
            // GEÇİCİ OLARAK ModelState KONTROLÜNÜ KALDIR
            model.Message = $"Merhaba {model.Name} 👋";
            model.Date = DateTime.Now;

            return View(model);
        }

    }
}

