using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HelloController : Controller
    {
        private readonly AppDbContext _context;

        public HelloController(AppDbContext context)
        {
            _context = context;
        }

        // LIST + SEARCH + PAGINATION
        public IActionResult Index(string search, int page = 1)
        {
            int pageSize = 5;

            var query = _context.Names.AsQueryable();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(x => x.Name.Contains(search));

            int totalCount = query.Count();

            var names = query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var vm = new HelloIndexViewModel
            {
                Names = names,
                Search = search,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return View(vm);
        }

        // ADD PAGE
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(NameEntity model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (_context.Names.Any(x => x.Name == model.Name))
            {
                ModelState.AddModelError("", "Bu isim zaten mevcut.");
                return View(model);
            }

            model.CreatedAt = DateTime.Now;
            model.UpdatedAt = DateTime.Now;

            _context.Names.Add(model);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // EDIT
        public IActionResult Edit(int id)
        {
            var item = _context.Names.Find(id);
            if (item == null) return NotFound();
            return View(item);
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
            var item = _context.Names.Find(id);
            if (item == null) return NotFound();

            _context.Names.Remove(item);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
