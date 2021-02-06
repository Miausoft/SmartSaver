using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SmartSaver.Domain.Repositories;
using SmartSaver.Domain.TokenValidation;
using SmartSaver.EntityFrameworkCore.Models;

namespace SmartSaver.Domain.Services.AuthenticationServices
{
    public class BasicAuthenticationService : IAuthenticationService
    {
        private readonly IRepository<User> _userRepo;
        private readonly ITokenValidationService _tokenValidation;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BasicAuthenticationService(IRepository<User> userRepo, 
                                          ITokenValidationService tokenValidation, 
                                          IHttpContextAccessor httpContextAccessor)
        {
            _userRepo = userRepo;
            _tokenValidation = tokenValidation;
            _httpContextAccessor = httpContextAccessor;
        }

        public virtual User Login(string email, string password)
        {
            return _userRepo.SearchFor(u => password != null && u.Email.Equals(email)).FirstOrDefault();
        }

        public virtual RegistrationResult Register(User user)
        {
            _userRepo.Insert(user);

            try
            {
                _userRepo.Save();
                user.Token = _tokenValidation.GenerateToken(user.Id);
                _userRepo.Save();

            }
            catch (DbUpdateException)
            {
                return RegistrationResult.Failure;
            }

            return RegistrationResult.Success;
        }

        public virtual bool VerifyEmail(User user)
        {
            user.Token = null;
            try
            {
                _userRepo.Save();
            }
            catch (DbUpdateException)
            {
                return false;
            }

            return true;
        }

        public virtual Task SignInAsync<T>(T userId)
        {
            var claim = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userId.ToString())
            };
            var identity = new ClaimsIdentity(claim, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            return _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }
    }
}
