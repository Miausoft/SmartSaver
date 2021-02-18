using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SmartSaver.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SmartSaver.Domain.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _table;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _table = _context.Set<T>();
        }

        public IQueryable<T> GetAll()
        {
            return _table;
        }

        public async Task<T> GetByIdAsync(params object[] keyValues)
        {
            return await _table.FindAsync(keyValues);
        }

        public IQueryable<T> SearchFor(Expression<Func<T, bool>> predicate)
        {
            return _table.Where(predicate);
        }

        public ValueTask<EntityEntry<T>> InsertAsync(T entity)
        {
            return _table.AddAsync(entity);
        }

        public void Delete(T entity)
        {
            _table.Remove(entity);
        }

        public Task SaveAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}
