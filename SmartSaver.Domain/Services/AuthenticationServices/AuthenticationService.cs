using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartSaver.Domain.Services.PasswordHash;
using SmartSaver.Domain.Services.Regex;
using SmartSaver.EntityFrameworkCore;
using SmartSaver.EntityFrameworkCore.Models;

namespace SmartSaver.Domain.Services.AuthenticationServices
{
    public class AuthenticationService : BasicAuthenticationService
    {
        private readonly IPasswordHasherService _hasher;
        private readonly IPasswordRegex _passwordRegex;
        private readonly ApplicationDbContext _context;

        public AuthenticationService(ApplicationDbContext context, IPasswordHasherService hasher, IPasswordRegex passwordRegex)
        {
            _hasher = hasher;
            _passwordRegex = passwordRegex;
            _context = context;
        }

        public override User Login(string username, string password)
        {
            User user = base.Login(username, password);
            if (user == null || !_hasher.Verify(password: password, passwordHash: user.Password))
            {
                return null;
            }

            return user;
        }

        public override RegistrationResult Register(User user)
        {
            if (!_passwordRegex.Match(user.Password))
            {
                return RegistrationResult.BadPasswordFormat;
            }

            if (_context.Users.FirstOrDefault(u => u.Username.Equals(user.Username)) != null)
            {
                return RegistrationResult.UserAlreadyExist;
            }

            user.Password = _hasher.Hash(user.Password);
            return base.Register(user);
        }
    }
}
