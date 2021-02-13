using System;
using Microsoft.AspNetCore.Http;
using SmartSaver.Domain.Services.PasswordHash;
using SmartSaver.Domain.Services.Regex;
using SmartSaver.EntityFrameworkCore.Models;
using SmartSaver.Domain.Repositories;
using SmartSaver.Domain.Services.AuthenticationServices.Jwt;
using System.Linq;

namespace SmartSaver.Domain.Services.AuthenticationServices
{
    public class Authentication : BasicAuthentication
    {
        private readonly IRepository<User> _userRepo;
        private readonly IPasswordHasherService _hasher;
        private readonly IPasswordRegex _passwordRegex;
        private readonly ITokenAuthentication _tokenAuth;

        public Authentication(IRepository<User> userRepo,
                              IPasswordHasherService hasher,
                              IPasswordRegex passwordRegex,
                              ITokenAuthentication tokenAuth,
                              IHttpContextAccessor httpContextAccessor) : base(userRepo, tokenAuth, httpContextAccessor)
        {
            _userRepo = userRepo;
            _hasher = hasher;
            _passwordRegex = passwordRegex;
            _tokenAuth = tokenAuth;
        }

        public override User Login(string email, string password)
        {
            User user = base.Login(email, password);
            if (user == null || !_hasher.Verify(password: password, passwordHash: user.Password))
            {
                return null;
            }

            return user;
        }

        public override RegistrationResult Register(User user)
        {
            if (!String.IsNullOrEmpty(user.Password))
            {
                if (!_passwordRegex.Match(user.Password))
                {
                    return RegistrationResult.BadPasswordFormat;
                }

                user.Password = _hasher.Hash(user.Password);
            }

            if (_userRepo.SearchFor(u => u.Email.Equals(user.Email)).Any())
            {
                return RegistrationResult.UserAlreadyExist;
            }

            return base.Register(user);
        }

        public override bool VerifyEmail(User user)
        {
            if (user == null || !_tokenAuth.ValidateToken(user.Token))
            {
                return false;
            }

            var claim = _tokenAuth.GetClaim(user.Token, "nameid");
            if (!user.Id.ToString().Equals(claim))
            {
                return false;
            }

            return base.VerifyEmail(user);
        }
    }
}
