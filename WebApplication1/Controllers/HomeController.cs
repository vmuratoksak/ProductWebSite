using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        // Dashboard
        public IActionResult Index()
        {
            ViewBag.TotalNames = _context.Names.Count();
            ViewBag.TodayAdded = _context.Names
                .Count(x => x.CreatedAt.Date == DateTime.Today);

            return View();
        }


        // Yönetim Paneli
        public IActionResult Privacy()
        {
            return View();
        }
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }


    }
}
