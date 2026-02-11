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

        // INDEX (Liste + Arama + Sayfalama)
        public IActionResult Index(string searchTerm, int page = 1)
        {
            int pageSize = 5;

            var query = _context.Names.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(x => x.Name.Contains(searchTerm));
            }

            int totalItems = query.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var names = query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var vm = new HelloIndexViewModel
            {
                Names = names,
                SearchTerm = searchTerm,
                CurrentPage = page,
                TotalPages = totalPages
            };

            return View(vm);
        }

        // CREATE
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(NameEntity model)
        {
            if (_context.Names.Any(x => x.Name == model.Name))
            {
                ModelState.AddModelError("Name", "Bu isim zaten mevcut.");
            }

            if (!ModelState.IsValid)
                return View(model);

            model.CreatedAt = DateTime.Now;
            model.UpdatedAt = DateTime.Now;

            _context.Names.Add(model);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // EDIT
        public IActionResult Edit(int id)
        {
            var entity = _context.Names.Find(id);
            if (entity == null) return NotFound();

            return View(entity);
        }

        [HttpPost]
        public IActionResult Edit(NameEntity model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var entity = _context.Names.Find(model.Id);
            if (entity == null) return NotFound();

            entity.Name = model.Name;
            entity.UpdatedAt = DateTime.Now;

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // DELETE
        public IActionResult Delete(int id)
        {
            var entity = _context.Names.Find(id);
            if (entity == null) return NotFound();

            _context.Names.Remove(entity);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
