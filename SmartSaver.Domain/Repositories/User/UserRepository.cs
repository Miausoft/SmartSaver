using SmartSaver.EntityFrameworkCore;
using SmartSaver.EntityFrameworkCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SmartSaver.Domain.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _db;

        public UserRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public T GetId<T>(string email)
        {
            return (T)Convert.ChangeType(_db.Users.Where(u => u.Email == email).Select(u => u.Id).FirstOrDefault(), typeof(T));
        }

        public IEnumerable<User> Get()
        {
            return _db.Users;
        }

        public IEnumerable<User> Get(Expression<Func<User, bool>> expression)
        {
            return _db.Users.Where(expression);
        }

        public User GetSingle(Expression<Func<User, bool>> expression)
        {
            return Get(expression).FirstOrDefault();
        }

        public async Task<CreateUserResponse> CreateAsync(User user)
        {
            _db.Users.Add(user);
            await Save();

            return CreateUserResponse.Success;
        }
        public Task<int> Save()
        {
            return _db.SaveChangesAsync();
        }
    }
}
