using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using SatisSitesi.Resources;
using System.Globalization;
using System.Reflection;

namespace SatisSitesi.Controllers
{
    [Route("test/loc")]
    public class TestController : Controller
    {
        private readonly IStringLocalizer<SharedResource> _localizer;

        public TestController(IStringLocalizer<SharedResource> localizer)
        {
            _localizer = localizer;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var result = new
            {
                CurrentCulture = CultureInfo.CurrentCulture.Name,
                CurrentUICulture = CultureInfo.CurrentUICulture.Name,
                LocalizerValueForNameManagement = _localizer["Name_Management"].Value,
                LocalizerValueForLogout = _localizer["Logout"].Value,
                FallbackOccurred = _localizer["Name_Management"].ResourceNotFound,
                AssemblyCulture = Assembly.GetExecutingAssembly().GetName().CultureInfo?.Name ?? "Neutral",
            };

            return Json(result);
        }
    }
}
