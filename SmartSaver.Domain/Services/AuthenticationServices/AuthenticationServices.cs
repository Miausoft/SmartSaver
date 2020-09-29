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
        /// If Username is registered in database and password is correct,
        /// loggs in.
        /// Fields Username and Password are required.
        /// </summary>
        /// <param name="login"></param>
        /// <returns>User object or null if not found.</returns>
        public virtual User Login(User login)
        {
            if (login.Username == null || login.Password == null)
                throw new Exception("Invalid login user object.");

            using var db = new ApplicationDbContext();

            var user = GetUser<string>(login.Username);
            if (user == null)
                return null;

            if (!login.Password.Verify(user.Password))
                return null;

            user.Account = db.Account.FirstOrDefault(p => p.Id.Equals(user.AccountId));

            return user;
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
                return RegistrationResult.InvalidUserObject;

            Insert(newUser);

            return RegistrationResult.Success;
        }

        protected virtual User GetUser<T>(T value)
        {
            using var db = new ApplicationDbContext();

            return db.User.FirstOrDefault(p => p.Username.Equals(value));
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
            db.User.Add(user);
            db.SaveChanges();
        }

        private bool CheckUserExist(string username, string number)
        {
            using var db = new ApplicationDbContext();

            var user = db.User.FirstOrDefault(p => p.Username.Equals(username) || p.PhoneNumber.Equals(number));
            if (user != null) return true;

            return false;
        }
    }
}
