using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SmartSaver.Domain.ExtensionMethods;
using SmartSaver.EntityFrameworkCore;
using SmartSaver.EntityFrameworkCore.Models;

namespace SmartSaver.Domain.Services.AuthenticationServices
{
    public class AuthenticationServices : IAuthenticationServices
    {

        public User Login(string username, string password)
        {
            throw new NotImplementedException();
        }

        public bool Register(string username, string password, string number)
        {
            var user = new User()
            {
                Username = username,
                PasswordHash = password.Hash(),
                PhoneNumber = number,
                UserFinances = new UserFinances()
            };

            if (!IsCorrect(user))
                return false;

            using var db = new ApplicationDbContext();

            db.User.Add(user);
            db.SaveChanges();

            return true;
        }

        private bool IsCorrect(User user)
        {
            return true;
        }
    }
}
