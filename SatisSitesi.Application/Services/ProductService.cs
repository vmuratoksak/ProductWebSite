using SatisSitesi.Domain.Entities;
using SatisSitesi.Application.Interfaces.Services;
using SatisSitesi.Application.Interfaces.Repositories;
using SatisSitesi.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SatisSitesi.Application.Services
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

        public ProductIndexModel GetPagedProducts(string search, string sortBy, int page, int pageSize, decimal? minPrice = null, decimal? maxPrice = null, bool inStockOnly = false)
        {
            var query = _productRepo.GetAll().AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var lowerSearch = search.ToLower();
                query = query.Where(x => 
                    (x.Name != null && x.Name.ToLower().Contains(lowerSearch)) || 
                    (x.Description != null && x.Description.ToLower().Contains(lowerSearch)));
            }
            
            if (minPrice.HasValue)
            {
                query = query.Where(x => x.Price >= minPrice.Value);
            }
            
            if (maxPrice.HasValue)
            {
                query = query.Where(x => x.Price <= maxPrice.Value);
            }
            
            if (inStockOnly)
            {
                query = query.Where(x => x.Stock > 0);
            }

            // Sorting
            query = sortBy switch
            {
                "PriceAsc" => query.OrderBy(x => x.Price),
                "PriceDesc" => query.OrderByDescending(x => x.Price),
                "Oldest" => query.OrderBy(x => x.CreatedAt),
                _ => query.OrderByDescending(x => x.CreatedAt), // Newest by default
            };

            var totalCount = query.Count();

            var products = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var viewModels = new List<ProductViewModel>();
            foreach (var p in products)
            {
                string badgeText = string.Empty;
                string badgeClass = string.Empty;
                if (p.Stock == 0)
                {
                    badgeText = "Out_Of_Stock";
                    badgeClass = "badge-premium-out";
                }
                else if (p.Stock < 10)
                {
                    badgeText = "Low_Stock";
                    badgeClass = "badge-premium-low";
                }

                viewModels.Add(new ProductViewModel
                {
                    Id = p.Id,
                    Name = p.Name ?? "Unnamed Product",
                    Description = p.Description ?? "No description available.",
                    Price = p.Price,
                    Stock = p.Stock,
                    ImageUrl = p.GetResolvedImageUrl(),
                    BadgeText = badgeText,
                    BadgeClass = badgeClass
                });
            }

            return new ProductIndexModel
            {
                Products = viewModels,
                Search = search,
                SortBy = sortBy,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                InStockOnly = inStockOnly,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                TotalProducts = totalCount
            };
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
                throw new Exception("Fiyat 0'dan büyük olmalidir.");

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
                throw new Exception("Fiyat 0'dan büyük olmalidir.");

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

        // ?? DOGRU YER BURASI

        public void IncreaseStock(string id)
        {
            var product = _productRepo.GetById(id);

            if (product == null)
                throw new Exception("Ürün bulunamadi.");

            product.Stock += 1;
            product.UpdatedAt = DateTime.Now;

            _productRepo.Update(product.Id, product);
        }

        public void DecreaseStock(string id)
        {
            var product = _productRepo.GetById(id);

            if (product == null)
                throw new Exception("Ürün bulunamadi.");

            if (product.Stock <= 0)
                throw new Exception("Stok zaten 0.");

            product.Stock -= 1;
            product.UpdatedAt = DateTime.Now;

            _productRepo.Update(product.Id, product);
        }
    }
}
