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

        public T GetById(object id)
        {
            return _table.Find(id);
        }

        public IQueryable<T> SearchFor(Expression<Func<T, bool>> predicate)
        {
            return _table.Where(predicate);
        }

        public void Insert(T obj)
        {
            _table.Add(obj);
        }

        public void Delete(object id)
        {
            _table.Remove(_table.Find(id));
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
