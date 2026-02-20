using Microsoft.AspNetCore.Mvc;
using WebApplication1.Services.Interfaces;
using WebApplication1.Models.Entities;

public class ProductController : Controller
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    public IActionResult Index()
    {
        var products = _productService.GetAll();
        return View(products);
    }

    public IActionResult Add()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Add(ProductEntity product)
    {
        try
        {
            _productService.Create(product);
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            return View(product);
        }
    }

    public IActionResult Edit(string id)
    {
        var product = _productService.GetById(id);
        return View(product);
    }

    [HttpPost]
    public IActionResult Edit(ProductEntity product)
    {
        try
        {
            _productService.Update(product);
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            return View(product);
        }
    }

    public IActionResult Delete(string id)
    {
        _productService.Delete(id);
        return RedirectToAction("Index");
    }
}
