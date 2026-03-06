using SatisSitesi.Domain.Entities;
using SatisSitesi.Application.Models;

namespace SatisSitesi.Application.Interfaces.Services
{
    public interface ICustomerService
    {
        CustomerIndexModel GetPaged(string search, string role, string status, int page, int pageSize);
        UserEntity GetById(string id);
        void Update(UserEntity model);
        void Delete(string id);
        void Add(UserEntity user);
    }
}
