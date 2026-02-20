using WebApplication1.Models.Entities;
using System.Collections.Generic;

namespace WebApplication1.Services.Interfaces
{
    public interface IProductService
    {
        List<ProductEntity> GetAll();
        ProductEntity GetById(string id);
        void Create(ProductEntity product);
        void Update(ProductEntity product);
        void Delete(string id);
    }
}