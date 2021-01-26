using SmartSaver.EntityFrameworkCore.Models;
using System.Collections.Generic;
using System.Linq.Expressions;
using System;
using System.Threading.Tasks;

namespace SmartSaver.Domain.Repositories
{
    public enum CreateUserResponse
    {
        Success,
        StateNotValid
    }

    public interface IUserRepository
    {
        public IEnumerable<User> Get();
        public IEnumerable<User> Get(Expression<Func<User, bool>> expression);
        public T GetId<T>(string email);
        User GetSingle(Expression<Func<User, bool>> expression);
        Task<CreateUserResponse> CreateAsync(User user);
    }
}
