using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartSaver.EntityFrameworkCore;
using SmartSaver.EntityFrameworkCore.Models;

namespace SmartSaver.Domain.Services.AuthenticationServices
{
    class DevAuthenticationService : IAuthenticationService
    {
        public Account Login(string username, string _ = "")
        {
            using var db = new ApplicationDbContext();

            var user = db.User.FirstOrDefault(p => p.Username.Equals(username));
            return user == null ? null : db.Account.FirstOrDefault(p => p.Id.Equals(user.AccountId));
        }

        public RegistrationResult Register(User user)
        {
            throw new NotImplementedException();
        }
    }
}
