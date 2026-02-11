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

        // =======================
        // LİSTE + ARAMA
        // =======================
        public IActionResult Index(string searchTerm, int page = 1)
        {
            int pageSize = 5;

            var query = _context.Names.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(x => x.Name.Contains(searchTerm));
            }

            var totalCount = query.Count();

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
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };

            return View(vm);
        }

        // =======================
        // CREATE GET
        // =======================
        public IActionResult Create()
        {
            return View();
        }

        // =======================
        // CREATE POST
        // =======================
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

        // =======================
        // DELETE
        // =======================
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
