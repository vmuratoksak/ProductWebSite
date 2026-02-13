using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using WebApplication1.Models;
using System;

namespace WebApplication1.Controllers
{
    public class ProductController : Controller
    {
        private readonly IMongoCollection<ProductEntity> _collection;

        public ProductController(IMongoClient client, IOptions<MongoSettings> settings)
        {
            var database = client.GetDatabase(settings.Value.DatabaseName);
            _collection = database.GetCollection<ProductEntity>("Products");
        }

        // LIST
        public IActionResult Index()
        {
            var products = _collection
                .Find(_ => true)
                .SortByDescending(x => x.CreatedAt)
                .ToList();

            return View(products);
        }

        // ADD GET
        public IActionResult Add()
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return Unauthorized();

            return View();
        }


        // ADD POST
        [HttpPost]
        public IActionResult Add(ProductEntity model)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return Unauthorized();

            model.CreatedAt = DateTime.Now;
            _collection.InsertOne(model);

            return RedirectToAction("Index");
        }



        // DELETE
        public IActionResult Delete(string id)
        {
            _collection.DeleteOne(x => x.Id == id);
            return RedirectToAction("Index");
        }
    }
}
