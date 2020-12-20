using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using SmartSaver.EntityFrameworkCore.Models;
using SmartSaver.Domain.CustomExceptions;
using SmartSaver.EntityFrameworkCore;

namespace SmartSaver.Domain.Repositories
{
    public class CategoryRepo : ICategoryRepo
    {
        public CategoryRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        private ApplicationDbContext _context;

        public IEnumerable<CategoryDto> GetMultiple()
        {
            return _context.Categories;
        }

        public IEnumerable<CategoryDto> GetMultiple(Expression<Func<CategoryDto, bool>> predicate)
        {
            return _context.Categories.Where(predicate);
        }

        public CategoryDto GetSingle(Expression<Func<CategoryDto, bool>> predicate)
        {
            return _context.Categories.First(predicate);
        }

        public async Task<int> CreateAsync(CategoryDto category)
        {
            _context.Categories.Add(category);

            if (await _context.SaveChangesAsync() == 0)
            {
                throw new InvalidModelException();
            }

            return _context.Categories
                .First(c => c.Title.Equals(category.Title))
                .Id;
        }
    }
}
