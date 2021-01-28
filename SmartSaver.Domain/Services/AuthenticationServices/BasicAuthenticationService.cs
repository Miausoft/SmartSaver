using System;
using System.Linq;
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
        /// Returns user with the given username and password.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>Account</returns>
        public virtual User Login(string email, string password)
        {
            return Context.Users.FirstOrDefault(u => u.Email.Equals(email));
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

        public virtual void FillMandatoryData(ref User user)
        {
            user.DateJoined = DateTime.UtcNow;
            Context.Accounts.Add(new Account { User = user, UserId = user.Id });
        }
    }
}
