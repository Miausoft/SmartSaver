using Microsoft.EntityFrameworkCore;
using SmartSaver.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;

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

        public T GetById(params object[] keyValues)
        {
            return _table.Find(keyValues);
        }

        public IQueryable<T> SearchFor(Expression<Func<T, bool>> predicate)
        {
            return _table.Where(predicate);
        }

        public void Insert(T entity)
        {
            _table.Add(entity);
        }

        public void Delete(T entity)
        {
            _table.Remove(entity);
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
