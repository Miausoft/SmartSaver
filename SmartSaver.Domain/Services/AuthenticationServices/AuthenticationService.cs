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
    public class AuthenticationService : IAuthenticationService
    {
        /// <summary>
        /// If Username is registered in database and password is correct,
        /// logs in.
        /// Fields Username and Password are required.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>Users object or null if not found.</returns>
        public virtual Account Login(string username, string password)
        {
            using var db = new ApplicationDbContext();

            var user = db.Users.FirstOrDefault(p => p.Username.Equals(username));
            if (user == null)
                return null;

            return !password.Verify(user.Password)
                ? null 
                : db.Accounts
                    .Include(a => a.Transactions)
                    .FirstOrDefault(a => a.Id.Equals(user.AccountId));
        }

        /// <summary>
        /// Registers new user into database.
        /// Fields Username, Password and PhoneNumber are required.
        /// </summary>
        /// <param name="newUser"></param>
        /// <returns>RegistrationResult</returns>
        public virtual RegistrationResult Register(User newUser)
        {
            if (newUser.Username == null || newUser.Password == null || newUser.PhoneNumber == null)
                throw new Exception("Register user object is invalid.");

            using var db = new ApplicationDbContext();

            if (CheckUserExist(newUser.Username, newUser.PhoneNumber))
                return RegistrationResult.UserAlreadyExist;

            if (!newUser.IsPasswordValid())
                return RegistrationResult.BadPasswordFormat;

            Insert(newUser);

            return RegistrationResult.Success;
        }

        /// <summary>
        /// AddMandatoryFields is automatically called in Insert() method before
        /// inserting into database.
        /// </summary>
        /// <param name="user"></param>
        protected virtual void AddMandatoryFields(ref User user)
        {
            user.Password = user.Password.Hash();
            user.DateJoined = DateTime.Now;
            user.Account = new Account();
        }

        protected void Insert(User user)
        {
            AddMandatoryFields(ref user);

            using var db = new ApplicationDbContext();
            db.Users.Add(user);
            db.SaveChanges();
        }

        protected virtual bool CheckUserExist(string username, string number)
        {
            using var db = new ApplicationDbContext();

            var user = db.Users.FirstOrDefault(p => p.Username.Equals(username) || p.PhoneNumber.Equals(number));
            return user != null;
        }
    }
}
