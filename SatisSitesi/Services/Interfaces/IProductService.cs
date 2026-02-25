using SatisSitesi.Models.Entities;
using System.Collections.Generic;

namespace SatisSitesi.Services.Interfaces
{
    public interface IProductService
    {
        List<ProductEntity> GetAll();
        ProductEntity GetById(string id);
        void Create(ProductEntity product);
        void Update(ProductEntity product);
        void Delete(string id);

        // 🔥 EKLENMESİ GEREKENLER
        void IncreaseStock(string id);
        void DecreaseStock(string id);
    }
}
