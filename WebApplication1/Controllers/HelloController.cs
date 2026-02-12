using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HelloController : Controller
    {
        private readonly IMongoCollection<NameEntity> _collection;

        public HelloController(IMongoClient client, IOptions<MongoSettings> settings)
        {
            var database = client.GetDatabase(settings.Value.DatabaseName);
            _collection = database.GetCollection<NameEntity>(settings.Value.CollectionName);

            // UNIQUE INDEX (Name alanı için)
            var indexKeys = Builders<NameEntity>.IndexKeys.Ascending(x => x.Name);
            var indexOptions = new CreateIndexOptions { Unique = true };
            _collection.Indexes.CreateOne(new CreateIndexModel<NameEntity>(indexKeys, indexOptions));
        }

        // LIST + SEARCH + PAGINATION
        public IActionResult Index(string search, int page = 1)
        {
            int pageSize = 5;

            var filter = Builders<NameEntity>.Filter.Empty;

            if (!string.IsNullOrEmpty(search))
            {
                filter = Builders<NameEntity>.Filter.Regex(
                    x => x.Name,
                    new MongoDB.Bson.BsonRegularExpression(search, "i")
                );
            }

            var totalCount = _collection.CountDocuments(filter);

            var names = _collection
                .Find(filter)
                .SortByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToList();

            var vm = new HelloIndexViewModel
            {
                Names = names,
                Search = search,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return View(vm);
        }

        // ADD PAGE
        public IActionResult Add()
        {
            return View();
        }

        /*[HttpPost]
        public IActionResult Add(NameEntity model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                model.CreatedAt = DateTime.Now;
                model.UpdatedAt = DateTime.Now;

                _collection.InsertOne(model);

                TempData["Success"] = "İsim başarıyla eklendi.";
                return RedirectToAction("Index");
            }
            catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
            {
                TempData["Error"] = "Bu isim zaten mevcut.";
                return View(model);
            }
        } */

        // EDIT
        public IActionResult Edit(string id)
        {
            var item = _collection.Find(x => x.Id == id).FirstOrDefault();
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
        public IActionResult Edit(NameEntity model)
        {
            if (!ModelState.IsValid)
                return View(model);

            model.UpdatedAt = DateTime.Now;

            _collection.ReplaceOne(x => x.Id == model.Id, model);

            return RedirectToAction("Index");
        }

        // DELETE
        public IActionResult Delete(string id)
        {
            _collection.DeleteOne(x => x.Id == id);
            return RedirectToAction("Index");
        }
        /*[HttpPost]
        public IActionResult Add(NameEntity model)
        {
            Console.WriteLine("POST ÇALIŞTI");

            model.CreatedAt = DateTime.Now;
            model.UpdatedAt = DateTime.Now;

            _collection.InsertOne(model);

            Console.WriteLine("KAYIT ATILDI");

            return RedirectToAction("Index");
        }*/
        [HttpPost]
        public IActionResult Add(NameEntity model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                model.CreatedAt = DateTime.Now;
                model.UpdatedAt = DateTime.Now;

                _collection.InsertOne(model);

                TempData["Success"] = "İsim başarıyla eklendi.";
                return RedirectToAction("Index");
            }
            catch (MongoWriteException ex)
            {
                // Duplicate key error code
                if (ex.WriteError?.Code == 11000)
                {
                    ModelState.AddModelError("Name", "Bu isim zaten mevcut.");
                    return View(model);
                }

                throw; // başka hata varsa fırlat
            }
        }

    }
}
