using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    public class HelloController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.Message = "Merhaba MVC 👋";
            ViewBag.Date = DateTime.Now;

            return View();
        }

    }
}
