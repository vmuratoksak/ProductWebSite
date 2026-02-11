using Microsoft.AspNetCore.Mvc;
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

        // LIST + FORM
        [HttpGet]
        public IActionResult Index()
        {
            var vm = new HelloIndexViewModel
            {
                Names = _context.Names
                    .OrderByDescending(x => x.CreatedAt)
                    .ToList()
            };

            return View(vm);
        }

        // CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(HelloIndexViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.Names = _context.Names
                    .OrderByDescending(x => x.CreatedAt)
                    .ToList();

                return View(vm);
            }

            vm.NewName.CreatedAt = DateTime.Now;
            _context.Names.Add(vm.NewName);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index)); // PRG
        }

        // EDIT
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var name = _context.Names.Find(id);
            if (name == null) return NotFound();

            return View(name);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(NameEntity model)
        {
            if (!ModelState.IsValid)
                return View(model);

            _context.Names.Update(model);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        // DELETE
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var name = _context.Names.Find(id);
            if (name == null) return NotFound();

            _context.Names.Remove(name);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult UpdateAsync(int id)
        {
            var recordData = _context.Names.Find(id);

            recordData.Name = "Updated Name";

            _context.Names.Update(recordData);

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}
