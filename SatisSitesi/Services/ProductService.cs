using SatisSitesi.Services.Interfaces;
using SatisSitesi.Repositories.Interfaces;
using SatisSitesi.Models.Entities;
using System;
using System.Collections.Generic;

namespace SatisSitesi.Services
{
    public class ProductService : IProductService
    {
        private readonly IRepository<ProductEntity> _productRepo;
        private readonly ITranslationService _translationService;

        public ProductService(IRepository<ProductEntity> productRepo, ITranslationService translationService)
        {
            _productRepo = productRepo;
            _translationService = translationService;
        }

        public List<ProductEntity> GetAll()
        {
            return _productRepo.GetAll();
        }

        public ProductEntity GetById(string id)
        {
            return _productRepo.GetById(id);
        }

        private void EnsureTranslations(ProductEntity product)
        {
            var targetLangs = new[] { "en", "de", "fr", "ar" };

            // We use Task.WhenAll synchronously here because standard ProductService matches IProductService signature.
            // In a larger refactor, ProductService Interfaces should switch to async/await instead of locking threads.
            var tasks = new List<System.Threading.Tasks.Task>();

            foreach (var lang in targetLangs)
            {
                // Verify Name Translation
                if (!product.NameTranslations.ContainsKey(lang) || string.IsNullOrWhiteSpace(product.NameTranslations[lang]))
                {
                    tasks.Add(System.Threading.Tasks.Task.Run(async () =>
                    {
                        var translation = await _translationService.TranslateAsync(product.Name, "tr", lang);
                        lock (product.NameTranslations) { product.NameTranslations[lang] = translation; }
                    }));
                }

                // Verify Description Translation
                if (!product.DescriptionTranslations.ContainsKey(lang) || string.IsNullOrWhiteSpace(product.DescriptionTranslations[lang]))
                {
                    tasks.Add(System.Threading.Tasks.Task.Run(async () =>
                    {
                        var translation = await _translationService.TranslateAsync(product.Description, "tr", lang);
                        lock (product.DescriptionTranslations) { product.DescriptionTranslations[lang] = translation; }
                    }));
                }
            }

            System.Threading.Tasks.Task.WaitAll(tasks.ToArray());
        }

        public void Create(ProductEntity product)
        {
            if (product.Price <= 0)
                throw new Exception("Fiyat 0'dan büyük olmalıdır.");

            if (product.Stock < 0)
                throw new Exception("Stok negatif olamaz.");

            product.CreatedAt = DateTime.Now;
            
            // Auto Translate missing strings synchronously before persisting
            EnsureTranslations(product);

            _productRepo.Insert(product);
        }

        public void Update(ProductEntity product)
        {
            if (product.Price <= 0)
                throw new Exception("Fiyat 0'dan büyük olmalıdır.");

            if (product.Stock < 0)
                throw new Exception("Stok negatif olamaz.");

            product.UpdatedAt = DateTime.Now;

            // Auto Translate missing strings synchronously before persisting
            EnsureTranslations(product);

            _productRepo.Update(product.Id, product);
        }

        public void Delete(string id)
        {
            _productRepo.Delete(id);
        }

        // 🔥 DOĞRU YER BURASI

        public void IncreaseStock(string id)
        {
            var product = _productRepo.GetById(id);

            if (product == null)
                throw new Exception("Ürün bulunamadı.");

            product.Stock += 1;
            product.UpdatedAt = DateTime.Now;

            _productRepo.Update(product.Id, product);
        }

        public void DecreaseStock(string id)
        {
            var product = _productRepo.GetById(id);

            if (product == null)
                throw new Exception("Ürün bulunamadı.");

            if (product.Stock <= 0)
                throw new Exception("Stok zaten 0.");

            product.Stock -= 1;
            product.UpdatedAt = DateTime.Now;

            _productRepo.Update(product.Id, product);
        }
    }
}
