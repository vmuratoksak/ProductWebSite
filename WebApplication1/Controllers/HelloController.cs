using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using System;
using System.Collections.Generic;

namespace WebApplication1.Controllers
{
    public class HelloController : Controller
    {
        // RAM’de tutulacak isim listesi (Database yerine)
        private static List<string> Names = new List<string>();

        // 1️⃣ SAYFA İLK AÇILDIĞINDA (GET)
        [HttpGet]
        public IActionResult Index()
        {
            var model = new HelloViewModel
            {
                Message = TempData["Message"] as string ?? "Lütfen adınızı girin",
                Date = DateTime.Now,
                Names = Names   // 🔴 LİSTEYİ VIEW’A GÖNDER
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
                model.Names = Names; // 🔴 HATA DURUMUNDA DA LİSTEYİ GÖNDER
                return View(model);
            }

            // ✔️ CREATE (Ekle)
            Names.Add(model.Name);

            TempData["Message"] = $"Merhaba {model.Name} 👋";

            // ✔️ PRG
            return RedirectToAction("Index");
        }

        // 3️⃣ DELETE (Sil)
        public IActionResult Delete(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                Names.Remove(name);
            }

            return RedirectToAction("Index");
        }
    }
}
