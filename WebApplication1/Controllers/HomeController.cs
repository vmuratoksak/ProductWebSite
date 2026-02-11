using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data;
using WebApplication1.Models;
using System.Linq;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var vm = new HelloIndexViewModel
            {
                Names = _context.Names.ToList(),
                NewName = new NameEntity()
            };

            return View(vm);
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
