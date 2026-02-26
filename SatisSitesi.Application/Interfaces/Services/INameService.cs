using SatisSitesi.Domain.Entities;
using SatisSitesi.Application.Models;
namespace SatisSitesi.Application.Interfaces.Services
{

    public interface INameService
    {
        NameIndexModel GetPaged(string search, int page, int pageSize);
        void Add(NameEntity model);
        NameEntity GetById(string id);
        void Update(NameEntity model);
        void Delete(string id);
    }
}
