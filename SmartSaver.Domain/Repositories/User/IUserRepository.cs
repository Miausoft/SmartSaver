using SmartSaver.EntityFrameworkCore.Models;
using SmartSaver.Domain.Repositories;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
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
        public IEnumerable<UserDto> Get();
        public IEnumerable<UserDto> Get(Expression<Func<UserDto, bool>> expression);
        public bool DoesUsernameExist(string username);
        public bool DoesEmailExist(string email);
        public T GetId<T>(string email);
        UserDto GetSingle(Expression<Func<UserDto, bool>> expression);
        Task<CreateUserResponse> CreateAsync(UserDto user);
    }
}
