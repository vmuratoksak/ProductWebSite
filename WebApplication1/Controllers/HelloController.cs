using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using System;

namespace WebApplication1.Controllers
{
    public class HelloController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.Message = TempData["Message"] ?? "Lütfen adınızı girin";
            ViewBag.Date = DateTime.Now;

            return View();
        }

        [HttpPost]
        public IActionResult Index(HelloViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            TempData["Message"] = $"Merhaba {model.Name} 👋";

            return RedirectToAction("Index");
        }

    }
}
