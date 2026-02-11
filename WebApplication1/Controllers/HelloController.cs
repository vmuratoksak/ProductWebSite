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
        private const int PageSize = 5;

        public HelloController(AppDbContext context)
        {
            _context = context;
        }

        // GET
        public IActionResult Index(string searchTerm, int page = 1)
        {
            var query = _context.Names.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(x => x.Name.Contains(searchTerm));
            }

            int totalItems = query.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)PageSize);

            var names = query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            var vm = new HelloIndexViewModel
            {
                Names = names,
                SearchTerm = searchTerm,
                CurrentPage = page,
                TotalPages = totalPages,
                NewName = new NameEntity()
            };

            return View(vm);
        }

        // POST (CREATE)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(HelloIndexViewModel model)
        {
            if (model.NewName == null || string.IsNullOrWhiteSpace(model.NewName.Name))
            {
                ModelState.AddModelError("NewName.Name", "İsim zorunludur.");
            }

            if (!ModelState.IsValid)
            {
                // Pagination tekrar doldurulmalı
                model.Names = _context.Names
                    .OrderByDescending(x => x.CreatedAt)
                    .Take(PageSize)
                    .ToList();

                model.TotalPages = 1;
                model.CurrentPage = 1;

                return View(model);
            }

            model.NewName.CreatedAt = DateTime.Now;
            model.NewName.UpdatedAt = DateTime.Now;

            _context.Names.Add(model.NewName);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // DELETE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var item = _context.Names.Find(id);
            if (item == null) return NotFound();

            _context.Names.Remove(item);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
