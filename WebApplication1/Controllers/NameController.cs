using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models.Entities;
using WebApplication1.Services.Interfaces;

public class NameController : Controller
{
    private readonly INameService _nameService;

    public NameController(INameService nameService)
    {
        _nameService = nameService;
    }

    public IActionResult Index(string search, int page = 1)
    {
        var userEmail = HttpContext.Session.GetString("UserEmail");

        if (string.IsNullOrEmpty(userEmail))
            return RedirectToAction("Login", "Auth");

        var model = _nameService.GetPaged(search, page, 5);

        return View(model);
    }

    public IActionResult Add()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Add(NameEntity model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            _nameService.Add(model);
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("Name", ex.Message);
            return View(model);
        }
    }

    public IActionResult Edit(string id)
    {
        var item = _nameService.GetById(id);
        if (item == null) return NotFound();

        return View(item);
    }

    [HttpPost]
    public IActionResult Edit(NameEntity model)
    {
        if (!ModelState.IsValid)
            return View(model);

        _nameService.Update(model);
        return RedirectToAction("Index");
    }

    public IActionResult Delete(string id)
    {
        _nameService.Delete(id);
        return RedirectToAction("Index");
    }
}
