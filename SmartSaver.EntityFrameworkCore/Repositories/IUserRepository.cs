using System;

namespace SmartSaver.EntityFrameworkCore.Repositories
{
    public interface IUserRepository
    {
        public bool DoesUsernameExist(string username);
        public bool DoesEmailExist(string email);
        public string GetId(string email);
    }
}
