using WebApplication1.Models.Entities;
using WebApplication1.Models.ViewModels;
namespace WebApplication1.Services.Interfaces
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