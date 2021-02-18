using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SmartSaver.Domain.Repositories;
using SmartSaver.Domain.Services.AuthenticationServices.Jwt;
using SmartSaver.EntityFrameworkCore.Models;

namespace SmartSaver.Domain.Services.AuthenticationServices
{
    public class BasicAuthentication : IAuthentication
    {
        private readonly IRepository<User> _userRepo;
        private readonly ITokenAuthentication _tokenAuth;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BasicAuthentication(IRepository<User> userRepo, 
                                   ITokenAuthentication tokenAuth, 
                                   IHttpContextAccessor httpContextAccessor)
        {
            _userRepo = userRepo;
            _tokenAuth = tokenAuth;
            _httpContextAccessor = httpContextAccessor;
        }

        public virtual User Login(string email, string password)
        {
            return _userRepo.SearchFor(u => password != null && u.Email.Equals(email)).FirstOrDefault();
        }

        public virtual async Task<RegistrationResult> RegisterAsync(User user)
        {
            await _userRepo.InsertAsync(user);

            try
            {
                await _userRepo.SaveAsync();
                user.Token = _tokenAuth.GenerateToken(user.Id);
                await _userRepo.SaveAsync();
            }
            catch (DbUpdateException)
            {
                return RegistrationResult.Failure;
            }

            return RegistrationResult.Success;
        }

        public virtual async Task<bool> VerifyEmailAsync(User user)
        {
            user.Token = null;
            try
            {
               await _userRepo.SaveAsync();
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
