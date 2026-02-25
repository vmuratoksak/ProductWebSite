using SatisSitesi.Models.Entities;
using SatisSitesi.Models.ViewModels;
namespace SatisSitesi.Services.Interfaces
{

    public interface INameService
    {
        NameIndexViewModel GetPaged(string search, int page, int pageSize);
        void Add(NameEntity model);
        NameEntity GetById(string id);
        void Update(NameEntity model);
        void Delete(string id);
    }
}
