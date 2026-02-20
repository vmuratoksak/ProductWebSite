using WebApplication1.Services.Interfaces;
using WebApplication1.Repositories.Interfaces;
using WebApplication1.Models.Entities;
using System;
using System.Collections.Generic;

namespace WebApplication1.Services
{
    public class ProductService : IProductService
    {
        private readonly IRepository<ProductEntity> _productRepo;

        public ProductService(IRepository<ProductEntity> productRepo)
        {
            _productRepo = productRepo;
        }

        public List<ProductEntity> GetAll()
        {
            return _productRepo.GetAll();
        }

        public ProductEntity GetById(string id)
        {
            return _productRepo.GetById(id);
        }

        public void Create(ProductEntity product)
        {
            if (product.Price <= 0)
                throw new Exception("Fiyat 0'dan büyük olmalıdır.");

            if (product.Stock < 0)
                throw new Exception("Stok negatif olamaz.");

            product.CreatedAt = DateTime.Now;

            _productRepo.Insert(product);
        }

        public void Update(ProductEntity product)
        {
            if (product.Price <= 0)
                throw new Exception("Fiyat 0'dan büyük olmalıdır.");

            if (product.Stock < 0)
                throw new Exception("Stok negatif olamaz.");

            product.UpdatedAt = DateTime.Now;

            _productRepo.Update(product.Id, product);
        }

        public void Delete(string id)
        {
            _productRepo.Delete(id);
        }
    }
}
