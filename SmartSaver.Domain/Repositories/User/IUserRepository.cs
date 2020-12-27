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
        public bool DoesUsernameExist<T>(T username);
        public bool DoesEmailExist<T>(T email);
        public T GetId<T>(T email);
        User GetSingle(Expression<Func<User, bool>> expression);
        Task<CreateUserResponse> CreateAsync(User user);
    }
}
