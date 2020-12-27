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

        public bool DoesUsernameExist<T>(T username)
        {
            return _db.Users.FirstOrDefault(u => u.Username.Equals(username.ToString())) != null;
        }

        public bool DoesEmailExist<T>(T email)
        {
            return _db.Users.FirstOrDefault(u => u.Email.Equals(email)) != null;
        }

        public T GetId<T>(T email)
        {
            return (T)Convert.ChangeType(_db.Users.Where(u => u.Email == email.ToString()).Select(u => u.Id).FirstOrDefault(), typeof(T));
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
            return Get(expression).First();
        }

        public async Task<CreateUserResponse> CreateAsync(User user)
        {
            _db.Users.Add(user);
            var response = await _db.SaveChangesAsync();

            return CreateUserResponse.Success;
        }
    }
}
