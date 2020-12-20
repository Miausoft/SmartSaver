using System;
using System.Collections.Generic;
using System.Text;
using SmartSaver.EntityFrameworkCore.Models;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SmartSaver.Domain.Repositories
{
    public enum CreateCategoryResponse
    {
        Success,
        BadRequest
    }

    public interface ICategoryRepo
    {
        IEnumerable<CategoryDto> GetMultiple();
        IEnumerable<CategoryDto> GetMultiple(Expression<Func<CategoryDto, bool>> predicate);
        CategoryDto GetSingle(Expression<Func<CategoryDto, bool>> predicate);
        Task<int> CreateAsync(CategoryDto category);
    }
}
