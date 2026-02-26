using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SatisSitesi.Application.Interfaces.Repositories
{
    public interface IRepository<T> where T : class
    {
        List<T> GetAll();
        T GetById(string id);
        T GetFirstOrDefault(Expression<Func<T, bool>> filter);
        void Insert(T entity);
        void Update(string id, T entity);
        void Delete(string id);
    }
}
