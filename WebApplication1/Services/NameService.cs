using WebApplication1.Models.Entities;
using WebApplication1.Models.ViewModels;
using WebApplication1.Repositories.Interfaces;
using MongoDB.Driver;
using WebApplication1.Services.Interfaces;
namespace WebApplication1.Services
{
    public class NameService : INameService
    {
        private readonly IRepository<NameEntity> _repository;

        public NameService(IRepository<NameEntity> repository)
        {
            _repository = repository;
        }

        public NameIndexViewModel GetPaged(string search, int page, int pageSize)
        {
            var query = _repository.GetAll().AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.Name.ToLower().Contains(search.ToLower()));
            }

            var totalCount = query.Count();

            var names = query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new NameIndexViewModel
            {
                Names = names,
                Search = search,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }

        public void Add(NameEntity model)
        {
            if (_repository.GetAll().Any(x => x.Name == model.Name))
                throw new Exception("Bu isim zaten mevcut.");

            model.CreatedAt = DateTime.Now;
            model.UpdatedAt = DateTime.Now;

            _repository.Insert(model);
        }

        public NameEntity GetById(string id)
        {
            return _repository.GetById(id);
        }

        public void Update(NameEntity model)
        {
            var existing = _repository.GetById(model.Id);
            if (existing == null)
                return;

            existing.Name = model.Name;
            existing.UpdatedAt = DateTime.Now;

            _repository.Update(existing.Id, existing);
        }

        public void Delete(string id)
        {
            _repository.Delete(id);
        }
    }
}