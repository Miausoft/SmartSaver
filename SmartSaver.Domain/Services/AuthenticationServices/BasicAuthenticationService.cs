using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using SmartSaver.EntityFrameworkCore;
using SmartSaver.EntityFrameworkCore.Models;

namespace SmartSaver.Domain.Services.AuthenticationServices
{
    public class BasicAuthenticationService : IAuthenticationService
    {
        protected readonly ApplicationDbContext Context;

        public BasicAuthenticationService(ApplicationDbContext context)
        {
            Context = context;
        }

        /// <summary>
        /// Returns account object that belongs to a user with given username and password.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>Account</returns>
        public virtual User Login(string username, string password)
        {
            var user = Context.Users.FirstOrDefault(u => u.Username.Equals(username));
            if (user == null)
            {
                return null;
            }

            user.Account = Context.Accounts.FirstOrDefault(a => a.Id == user.AccountId);

            if (user.Account != null)
                user.Account.Transactions = Context.Transactions.Where(t => t.AccountId == user.AccountId).ToList();

            return user;
        }

        /// <summary>
        /// Registers user and returns Success message.
        /// </summary>
        /// <param name="user"></param>
        /// <returns>RegistrationResult.Success</returns>
        public virtual RegistrationResult Register(User user)
        {
            FillMandatoryData(ref user);
            Context.Users.Add(user);
            Context.SaveChanges();

            return RegistrationResult.Success;
        }

        private static void FillMandatoryData(ref User user)
        {
            user.DateJoined = DateTime.UtcNow;
            user.Account = new Account();
        }
    }
}
