using System;
using System.Linq;
using System.Linq.Expressions;

namespace SmartSaver.Domain.Repositories
{
    public interface IRepository<T> where T : class
    {
        T GetById(params object[] keyValues);
        IQueryable<T> GetAll();
        IQueryable<T> SearchFor(Expression<Func<T, bool>> predicate);
        void Insert(T entity);
        void Delete(T entity);
        void Save();
    }
}
