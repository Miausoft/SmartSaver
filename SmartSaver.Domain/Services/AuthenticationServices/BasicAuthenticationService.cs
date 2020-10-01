using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using SmartSaver.EntityFrameworkCore;
using SmartSaver.EntityFrameworkCore.Models;

namespace SmartSaver.Domain.Services.AuthenticationServices
{
    public class BasicAuthenticationService : IBasicAuthenticationService
    {
        private readonly ApplicationDbContext _context;

        public BasicAuthenticationService()
        {
            _context = new ApplicationDbContext();
        }

        ~BasicAuthenticationService()
        {
            _context.Dispose();
        }

        /// <summary>
        /// Returns account object that belongs to a user with given username and password.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>Account</returns>
        public Account Login(string username, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username.Equals(username) && u.Password.Equals(password));

            if (user == null)
                return null;

            return _context.Accounts
                .Include(a => a.Transactions)
                .FirstOrDefault(a => a.Id.Equals(user.AccountId));
        }

        /// <summary>
        /// Registers user and returns Success message.
        /// </summary>
        /// <param name="user"></param>
        /// <returns>RegistrationResult.Success</returns>
        public RegistrationResult Register(User user)
        {
            _context.Users.Add(user);
            FillMandatoryData(ref user);

            return RegistrationResult.Success;
        }

        private void FillMandatoryData(ref User user)
        {
            user.DateJoined = DateTime.Now;
            user.Account = new Account();
        }
    }
}
