using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HelloController : Controller
    {
        [HttpPost]
        public IActionResult Index(HelloViewModel model)
        {
            model.Message = $"Merhaba {model.Name} 👋";
            model.Date = DateTime.Now;

            return View(model);
        }

    }
}
