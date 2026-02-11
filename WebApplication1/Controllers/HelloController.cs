using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data;
using WebApplication1.Models;
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

        // ======================
        // LIST + SEARCH
        // ======================
        public IActionResult Index(string searchTerm)
        {
            var query = _context.Names.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
                query = query.Where(x => x.Name.Contains(searchTerm));

            var vm = new HelloIndexViewModel
            {
                Names = query
                        .OrderByDescending(x => x.CreatedAt)
                        .ToList(),
                SearchTerm = searchTerm
            };

            return View(vm);
        }

        // ======================
        // CREATE
        // ======================
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(NameEntity model)
        {
            if (!ModelState.IsValid)
                return View(model);

            model.CreatedAt = DateTime.Now;
            model.UpdatedAt = DateTime.Now;

            _context.Names.Add(model);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // ======================
        // EDIT
        // ======================
        public IActionResult Edit(int id)
        {
            var entity = _context.Names.Find(id);
            if (entity == null)
                return NotFound();

            return View(entity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(NameEntity model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var entity = _context.Names.Find(model.Id);
            if (entity == null)
                return NotFound();

            entity.Name = model.Name;
            entity.UpdatedAt = DateTime.Now;

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // ======================
        // DELETE
        // ======================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var entity = _context.Names.Find(id);
            if (entity != null)
            {
                _context.Names.Remove(entity);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}
