using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data;
using WebApplication1.Models;
using System;
using System.Linq;

namespace WebApplication1.Controllers
{
    public class HelloController : Controller
    {
        private readonly AppDbContext _context;

        public HelloController(AppDbContext context)
        {
            _context = context;
        }

        // GET
        public IActionResult Index()
        {
            var model = new HelloViewModel
            {
                Hellos = _context.Hellos
                                 .OrderByDescending(x => x.CreatedAt)
                                 .ToList()
            };

            ViewBag.Message = TempData["Message"] ?? "Lütfen adınızı girin";
            return View(model);
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(HelloViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Hellos = _context.Hellos.ToList();
                ViewBag.Message = "Lütfen adınızı girin";
                return View(model);
            }

            var entity = new HelloEntity
            {
                Name = model.Name,
                CreatedAt = DateTime.Now
            };

            _context.Hellos.Add(entity);
            _context.SaveChanges();

            TempData["Message"] = $"Merhaba {model.Name} 👋";
            return RedirectToAction(nameof(Index));
        }
    }
}
