using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace SatisSitesi.ViewComponents;

public class LanguageSelectorViewComponent : ViewComponent
{
    private readonly IOptions<RequestLocalizationOptions> _localizationOptions;

    public LanguageSelectorViewComponent(IOptions<RequestLocalizationOptions> localizationOptions)
    {
        _localizationOptions = localizationOptions;
    }

    public IViewComponentResult Invoke()
    {
        var cultureFeature = HttpContext.Features.Get<IRequestCultureFeature>();
        var currentCulture = cultureFeature?.RequestCulture.UICulture;

        var supportedCultures = _localizationOptions.Value.SupportedUICultures?.ToList() ?? new List<System.Globalization.CultureInfo>();

        var model = new LanguageSelectorModel
        {
            CurrentUICulture = currentCulture,
            SupportedCultures = supportedCultures
        };

        return View(model);
    }
}

public class LanguageSelectorModel
{
    public System.Globalization.CultureInfo? CurrentUICulture { get; set; }
    public List<System.Globalization.CultureInfo> SupportedCultures { get; set; } = new();
}
