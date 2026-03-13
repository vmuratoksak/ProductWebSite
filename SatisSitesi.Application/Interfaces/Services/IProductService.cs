using SatisSitesi.Domain.Entities;
using SatisSitesi.Application.Models;
using System.Collections.Generic;

namespace SatisSitesi.Application.Interfaces.Services
{
    public interface IProductService
    {
        List<ProductEntity> GetAll();
        ProductIndexModel GetPagedProducts(string search, string sortBy, int page, int pageSize, decimal? minPrice = null, decimal? maxPrice = null, bool inStockOnly = false, bool onlyVisible = false);
        ProductEntity GetById(string id);
        void Create(ProductEntity product);
        void Update(ProductEntity product);
        void Delete(string id);

        // ?? EKLENMESI GEREKENLER
        void IncreaseStock(string id);
        void DecreaseStock(string id);
    }
}
