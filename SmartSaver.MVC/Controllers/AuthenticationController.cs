using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SmartSaver.Domain.Services.AuthenticationServices;
using SmartSaver.EntityFrameworkCore;
using SmartSaver.EntityFrameworkCore.Models;
using SmartSaver.MVC.Models;

namespace SmartSaver.MVC.Controllers
{
    [AllowAnonymous]
    public class AuthenticationController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly Domain.Services.AuthenticationServices.IAuthenticationService _auth;
        private readonly IConfiguration _configuration;

        public AuthenticationController(ApplicationDbContext db, Domain.Services.AuthenticationServices.IAuthenticationService auth, IConfiguration configuration)
        {
            _db = db;
            _auth = auth;
            _configuration = configuration;
        }

        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction(nameof(DashboardController.Index), nameof(DashboardController).Replace("Controller", ""));
            }

            return View();
        }

        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction(nameof(DashboardController.Index), nameof(DashboardController).Replace("Controller", ""));
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(AuthenticationViewModel model, EmailVerification data)
        {
            if (ModelState.IsValid && AddNewUser(model.Username, model.Email, model.Password))
            {
                data.EmailVerified = false;
                data.UserId = getId(model.Email);
                data.Token = GenerateToken(data.UserId);
                _db.EmailVerifications.Add(data);
                _db.SaveChanges();

                var confirmationLink = Url.Action("ConfirmEmail", "Authentication", new { token = data.Token }, Request.Scheme);
                
                //need to send condirmationLink
                //return RedirectToAction(nameof(Login));
                
                return Json(confirmationLink);
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(User user, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                bool isVerified = IsVerified(user.Email);
                if (_auth.Login(user.Email, user.Password) != null && isVerified)
                {
                    await UserAuthenticationAsync(getId(user.Email).ToString());
                    return RedirectToAction(nameof(DashboardController.Index), nameof(DashboardController).Replace("Controller", ""));
                }

                ModelState.AddModelError(nameof(user.Email), "Email is not confirmed.");
                return View();
            }

            ModelState.AddModelError(nameof(user.Email), "Incorrect username or password.");
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction(nameof(HomeController.Index), nameof(HomeController).Replace("Controller", ""));
        }

        [HttpPost]
        public IActionResult ExternalLogin(string returnUrl, string provider)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction(nameof(DashboardController.Index), nameof(DashboardController).Replace("Controller", ""));
            }

            var properties = new AuthenticationProperties { RedirectUri = Url.Action(nameof(ExternalResponse), new { ReturnUrl = returnUrl }) };
            return Challenge(properties, provider);
        }

        public async Task<IActionResult> ExternalResponse(string returnUrl)
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (result.Succeeded)
            {
                var email = result.Principal.FindFirstValue(ClaimTypes.Email);
                AddNewUser(email, email, null);
                await UserAuthenticationAsync(getId(email).ToString());
                return LocalRedirect(returnUrl ?? Url.Content("~/"));
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Error while trying to login.");
                return View(nameof(Login));
            }
        }

        private async Task UserAuthenticationAsync(string userId)
        {
            var claim = new List<Claim> { new Claim(ClaimTypes.Name, userId) };
            var identity = new ClaimsIdentity(claim, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }

        private bool AddNewUser(string username, string email, string password)
        {
            if (!DoesUsernameExist(username) && !DoesEmailExist(email))
            {
                _auth.Register(new User() { Username = username, Email = email, Password = password });
                return true;
            }

            return false;
        }

        public bool DoesUsernameExist(string username)
        {
            return _db.Users.FirstOrDefault(u => u.Username.Equals(username)) != null;
        }

        public bool DoesEmailExist(string email)
        {
            return _db.Users.FirstOrDefault(u => u.Email.Equals(email)) != null;
        }

        public int getId(string email)
        {
            return _db.Users.Where(u => u.Email == email).Select(u => u.Id).FirstOrDefault();
        }

        public bool IsVerified(string email)
        {
            return _db.EmailVerifications.Include(a => a.User).Where(a => a.UserId.Equals(getId(email))).Select(a => a.EmailVerified).First();
        }

        public string GenerateToken(int userId)
        {
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                Issuer = "SmartSaver",
                Audience = "SmartSaver",
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["Token:MySecret"])), SecurityAlgorithms.HmacSha256Signature),
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public bool ValidateCurrentToken(string token)
        {
            try
            {
                new JwtSecurityTokenHandler().ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = "SmartSaver",
                    ValidAudience = "SmartSaver",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["Token:MySecret"]))
                }, out SecurityToken validatedToken);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public string GetClaim(string token, string claimType)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.ReadToken(token) as JwtSecurityToken;
            if (!securityToken.Claims.Any(claim => claim.Type == claimType))
            {
                return String.Empty;
            }

            return securityToken.Claims.FirstOrDefault(claim => claim.Type == claimType).Value;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmailAsync(string token)
        {
            if (token == null)
            {
                return RedirectToAction(nameof(Index), nameof(HomeController).Replace("Controller", ""));
            }

            if (!ValidateCurrentToken(token))
            {
                return RedirectToAction(nameof(Index), nameof(HomeController).Replace("Controller", ""));
            }

            var claim = GetClaim(token, "nameid");
            if (String.IsNullOrEmpty(claim))
            {
                return RedirectToAction(nameof(Index), nameof(HomeController).Replace("Controller", ""));
            }
            
            var userToken = _db.EmailVerifications.Include(a => a.User).Where(a => a.UserId.ToString().Equals(claim)).Select(a => a.Token).FirstOrDefault();
            if (token == userToken)
            {
                _db.EmailVerifications.FirstOrDefault(a => a.UserId.ToString().Equals(claim)).EmailVerified = true;
                _db.SaveChanges();
                await UserAuthenticationAsync(claim);
                return RedirectToAction(nameof(DashboardController.Complete), nameof(DashboardController).Replace("Controller", ""));
            }
            
            return RedirectToAction(nameof(Index), nameof(HomeController).Replace("Controller", ""));
        }
    }
}