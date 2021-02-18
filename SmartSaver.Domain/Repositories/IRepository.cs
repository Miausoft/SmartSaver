using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SmartSaver.Domain.Repositories
{
    public interface IRepository<T> where T : class
    {
        public IQueryable<T> GetAll();
        public Task<T> GetByIdAsync(params object[] keyValues);
        public IQueryable<T> SearchFor(Expression<Func<T, bool>> predicate);
        public ValueTask<EntityEntry<T>> InsertAsync(T entity);
        public void Delete(T entity);
        public Task SaveAsync();
    }
}
