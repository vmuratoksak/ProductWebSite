using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HelloController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View(new HelloViewModel
            {
                Date = DateTime.Now
            });
        }

        [HttpPost]
        public IActionResult Index(HelloViewModel model)
        {
            model.Date = DateTime.Now;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            model.Message = $"Merhaba {model.Name} 👋";
            return View(model);
        }
    }
}
