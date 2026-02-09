using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HelloController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            var model = new HelloViewModel
            {
                Message = TempData["Message"] as string,
                Date = DateTime.Now
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Index(HelloViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
            {
                TempData["Message"] = "İsim zorunludur";
                return RedirectToAction("Index");
            }

            TempData["Message"] = $"Merhaba {model.Name} 👋";
            return RedirectToAction("Index");
        }
    }
}
