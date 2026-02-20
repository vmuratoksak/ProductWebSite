using System.Collections.Generic;

namespace WebApplication1.Repositories.Interfaces
{
    public interface IRepository<T> where T : class
    {
        List<T> GetAll();
        T GetById(string id);
        void Insert(T entity);
        void Update(string id, T entity);
        void Delete(string id);
    }
}
