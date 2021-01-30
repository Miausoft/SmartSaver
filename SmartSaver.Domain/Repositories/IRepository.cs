using System;
using System.Linq;
using System.Linq.Expressions;

namespace SmartSaver.Domain.Repositories
{
    public interface IRepository<T> where T : class
    {
        T GetById(object id);
        IQueryable<T> GetAll();
        IQueryable<T> SearchFor(Expression<Func<T, bool>> predicate);
        void Insert(T obj);
        void Delete(object id);
        void Save();
    }
}
