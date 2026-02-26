using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using SatisSitesi.Application.Interfaces.Services;
using SatisSitesi.Domain.Entities;

namespace SatisSitesi.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("UserRole") == "Admin";
        }

        // ✅ Ürün Listeleme
        public IActionResult Index(string search, string sortBy, decimal? minPrice, decimal? maxPrice, bool inStockOnly, int page = 1)
        {
            var model = _productService.GetPagedProducts(search, sortBy, page, 8, minPrice, maxPrice, inStockOnly); // sayfa başı 8 ürün
            return View(model);
        }

        // ✅ Yeni Ürün Sayfası (Admin Only)
        public IActionResult Add()
        {
            if (!IsAdmin())
                return RedirectToAction("Index");

            return View();
        }

        // ✅ Yeni Ürün Ekle (Admin Only)
        [HttpPost]
        public IActionResult Add(ProductEntity product)
        {
            if (!IsAdmin())
                return RedirectToAction("Index");

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

        // ✅ Ürün Düzenleme Sayfası (Admin Only)
        public IActionResult Edit(string id)
        {
            if (!IsAdmin())
                return RedirectToAction("Index");

            var product = _productService.GetById(id);

            if (product == null)
                return RedirectToAction("Index");

            return View(product);
        }

        // ✅ Ürün Güncelleme (Admin Only)
        [HttpPost]
        public IActionResult Edit(ProductEntity product)
        {
            if (!IsAdmin())
                return RedirectToAction("Index");

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

        // ✅ Ürün Silme (Admin Only)
        public IActionResult Delete(string id)
        {
            if (!IsAdmin())
                return RedirectToAction("Index");

            _productService.Delete(id);
            return RedirectToAction("Index");
        }

        // ✅ Stok Arttır (Admin Only)
        public IActionResult IncreaseStock(string id)
        {
            if (!IsAdmin())
                return RedirectToAction("Index");

            try
            {
                _productService.IncreaseStock(id);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Index");
        }

        // ✅ Stok Azalt (Admin Only)
        public IActionResult DecreaseStock(string id)
        {
            if (!IsAdmin())
                return RedirectToAction("Index");

            try
            {
                _productService.DecreaseStock(id);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Index");
        }
    }
}
