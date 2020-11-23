using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SmartSaver.Domain.Services.AuthenticationServices;
using SmartSaver.Domain.TokenValidation;
using SmartSaver.EntityFrameworkCore.Models;
using SmartSaver.EntityFrameworkCore.Repositories;
using SmartSaver.MVC.Models;

namespace SmartSaver.MVC.Controllers
{
    [AllowAnonymous]
    public class AuthenticationController : Controller
    {
        private readonly Domain.Services.AuthenticationServices.IAuthenticationService _auth;
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepo;
        private readonly IEmailVerificationRepository _emailRepo;
        private readonly ITokenValidation _tokenValidation;

        public AuthenticationController(Domain.Services.AuthenticationServices.IAuthenticationService auth, 
                                        IConfiguration configuration, 
                                        IUserRepository userRepo,
                                        IEmailVerificationRepository emailRepo,
                                        ITokenValidation tokenValidation)
        {
            _auth = auth;
            _configuration = configuration;
            _userRepo = userRepo;
            _emailRepo = emailRepo;
            _tokenValidation = tokenValidation;
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
        public IActionResult Register(AuthenticationViewModel model)
        {
            if (ModelState.IsValid && AddNewUser(model.Username, model.Email, model.Password))
            {
                var confirmationLink = Url.Action("ConfirmEmail", "Authentication", new { token = _emailRepo.GetUserToken(_userRepo.GetId(model.Email).ToString()) }, Request.Scheme);
                
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
            if (!ModelState.IsValid)
            {
                return View();
            }

            if (_auth.Login(user.Email, user.Password) == null)
            {
                ModelState.AddModelError(nameof(user.Email), "Incorrect username or password.");
                return View();
            }

            if (!_emailRepo.IsVerified(_userRepo.GetId(user.Email)))
            {
                ModelState.AddModelError(nameof(user.Email), "Email is not confirmed.");
                return View();
            }

            await UserAuthenticationAsync(_userRepo.GetId(user.Email).ToString());
            return RedirectToAction(nameof(DashboardController.Index), nameof(DashboardController).Replace("Controller", ""));
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
                await UserAuthenticationAsync(_userRepo.GetId(email).ToString());
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
            if (!_userRepo.DoesUsernameExist(username) && !_userRepo.DoesEmailExist(email))
            {
                _auth.Register(new User() { Username = username, Email = email, Password = password });
                _emailRepo.Create(new EmailVerification { UserId = int.Parse(_userRepo.GetId(email)), EmailVerified = false, Token = tokenValidation.GenerateToken(_userRepo.GetId(email)) });
                return true;
            }

            return false;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmailAsync(string token)
        {
            if (token == null)
            {
                return RedirectToAction(nameof(Index), nameof(HomeController).Replace("Controller", ""));
            }

            if (!tokenValidation.ValidateToken(token))
            {
                return RedirectToAction(nameof(Index), nameof(HomeController).Replace("Controller", ""));
            }

            var claim = tokenValidation.GetClaim(token, "nameid");
            if (String.IsNullOrEmpty(claim))
            {
                return RedirectToAction(nameof(Index), nameof(HomeController).Replace("Controller", ""));
            }

            var userToken = _emailRepo.GetUserToken(claim);
            if (token == userToken)
            {
                _emailRepo.Delete(claim);
                await UserAuthenticationAsync(claim);
                return RedirectToAction(nameof(DashboardController.Complete), nameof(DashboardController).Replace("Controller", ""));
            }
            
            return RedirectToAction(nameof(Index), nameof(HomeController).Replace("Controller", ""));
        }

        public bool DoesUsernameExist(string username) => _userRepo.DoesUsernameExist(username);

        public bool DoesEmailExist(string email) => _userRepo.DoesEmailExist(email);
    }
}