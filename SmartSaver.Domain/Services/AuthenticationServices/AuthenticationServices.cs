using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;
using Microsoft.EntityFrameworkCore;
using SmartSaver.Domain.ExtensionMethods;
using SmartSaver.EntityFrameworkCore;
using SmartSaver.EntityFrameworkCore.Models;

namespace SmartSaver.Domain.Services.AuthenticationServices
{
    public class AuthenticationServices : IAuthenticationServices
    {
        /// <summary>
        /// Tries to match username with password.
        /// Logs user in.
        /// </summary>
        /// <param name="username">His username</param>
        /// <param name="password">His password</param>
        /// <returns>User object if successful.</returns>
        public User Login(string username, string password)
        {
            using var db = new ApplicationDbContext();

            var user = db.User.FirstOrDefault(p => p.Username.Equals(username));
            if (user == null)
                return null;

            if (!password.Verify(user.PasswordHash))
                return null;

            user.Account = db.Account.FirstOrDefault(p => p.Id.Equals(user.AccountId));

            return user;
        }

        /// <summary>
        /// Registers new user with given username, password, mobile number.
        /// </summary>
        /// <param name="username">User's username</param>
        /// <param name="password">His password</param>
        /// <param name="number">His mobile number.</param>
        /// <returns>RegistrationResult</returns>
        public RegistrationResult Register(string username, string password, string number)
        {
            using var db = new ApplicationDbContext();

            if (CheckUserExist(username, number))
                return RegistrationResult.UserAlreadyExist;

            var user = new User()
            {
                Username = username,
                PasswordHash = password,
                PhoneNumber = number,
                DateJoined = DateTime.Now,
                Account = new Account()
            };

            if (!user.IsFormatCorrect())
                return RegistrationResult.InvalidUserObject;

            user.PasswordHash = password.Hash();

            db.User.Add(user);
            db.SaveChanges();

            return RegistrationResult.Success;
        }

        public bool CheckUserExist(string username, string number)
        {
            using var db = new ApplicationDbContext();

            var user = db.User.FirstOrDefault(p => p.Username.Equals(username) || p.PhoneNumber.Equals(number));
            if (user != null) return true;

            return false;
        }
    }
}
