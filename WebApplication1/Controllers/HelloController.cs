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

        // LIST
        public IActionResult Index()
        {
            var vm = new HelloIndexViewModel
            {
                Names = _context.Names
                                .OrderByDescending(x => x.CreatedAt)
                                .ToList(),
                NewName = new NameEntity()
            };

            return View(vm);
        }

        // CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(HelloIndexViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Names = _context.Names.ToList();
                return View(model);
            }

            model.NewName.CreatedAt = DateTime.Now;
            model.NewName.UpdatedAt = DateTime.Now;

            _context.Names.Add(model.NewName);
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
        [ValidateAntiForgeryToken]
        public IActionResult Edit(NameEntity model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var item = _context.Names.Find(model.Id);
            if (item == null) return NotFound();

            item.Name = model.Name;
            item.UpdatedAt = DateTime.Now;

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
