using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    public class HelloController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
