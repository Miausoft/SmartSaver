using System;
using System.Linq;

namespace SmartSaver.EntityFrameworkCore.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _db;

        public UserRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public bool DoesUsernameExist(string username)
        {
            return _db.Users.FirstOrDefault(u => u.Username.Equals(username)) != null;
        }

        public bool DoesEmailExist(string email)
        {
            return _db.Users.FirstOrDefault(u => u.Email.Equals(email)) != null;
        }

        public string GetId(string email)
        {
            return _db.Users.Where(u => u.Email == email).Select(u => u.Id).FirstOrDefault().ToString();
        }
    }
}
