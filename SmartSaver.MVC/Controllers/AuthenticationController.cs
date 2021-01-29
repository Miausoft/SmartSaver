using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SmartSaver.Domain.Services.EmailServices;
using SmartSaver.Domain.TokenValidation;
using SmartSaver.EntityFrameworkCore.Models;
using SmartSaver.Domain.Repositories;
using SmartSaver.MVC.Models;

namespace SmartSaver.MVC.Controllers
{
    [AllowAnonymous]
    public class AuthenticationController : Controller
    {
        private readonly Domain.Services.AuthenticationServices.IAuthenticationService _auth;
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepo;
        private readonly ITokenValidationService _tokenValidation;
        private readonly IMailer _mailer;

        public AuthenticationController(Domain.Services.AuthenticationServices.IAuthenticationService auth,
                                        IConfiguration configuration,
                                        IUserRepository userRepo,
                                        ITokenValidationService tokenValidation,
                                        IMailer mailer)
        {
            _auth = auth;
            _configuration = configuration;
            _userRepo = userRepo;
            _tokenValidation = tokenValidation;
            _mailer = mailer;
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
                return RedirectToAction(nameof(Verify), new { email = model.Email });
            }

            return View();
        }

        [HttpGet]
        public IActionResult Verify(string email)
        {
            User user = _userRepo.GetSingle(e => e.Email == email);

            if (String.IsNullOrEmpty(email) || user == null || user.Token == null)
            {
                return RedirectToAction(nameof(Login));
            }

            var confirmationLink = Url.Action("ConfirmEmail", "Authentication", new { token = user.Token }, Request.Scheme);

            _mailer.SendEmailAsync(
                new MailMessage(
                    _configuration["Email:Address"],
                    _configuration["Email:Address"],
                    "Verify your email address",
                     System.IO.File.ReadAllText(_configuration["TemplatePaths:Email"]).Replace("@ViewBag.VerifyLink", confirmationLink))
                { IsBodyHtml = true });

            return View(nameof(Verify), ViewBag.Email = email);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(AuthenticationViewModel user, string returnUrl)
        {
            if (_auth.Login(user.Email, user.Password) == null)
            {
                ModelState.AddModelError(nameof(user.Email), "Incorrect username or password.");
                return View();
            }

            if (_userRepo.GetSingle(e => e.Email == user.Email).Token != null)
            {
                return View(nameof(Verify), ViewBag.Email = user.Email);
            }

            await UserAuthenticationAsync(_userRepo.GetId<string>(user.Email));
            return LocalRedirect(returnUrl ?? Url.Content("~/"));
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
                await UserAuthenticationAsync(_userRepo.GetId<string>(email).ToString());
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
            var claim = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userId)
            };
            var identity = new ClaimsIdentity(claim, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }

        private bool AddNewUser(string username, string email, string password)
        {
            if (_userRepo.GetSingle(u => u.Username == username || u.Email == email) != null)
            {
                return false;
            }

            _auth.Register(new User() { Username = username, Email = email, Password = password });

            if (password != null)
            {
                _userRepo.GetSingle(u => u.Email == email).Token = _tokenValidation.GenerateToken(_userRepo.GetId<string>(email));
                _userRepo.Save().Wait();
            }

            return true;
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmailAsync(string token)
        {
            if (token == null)
            {
                return RedirectToAction(nameof(Index), nameof(HomeController).Replace("Controller", ""));
            }

            if (!_tokenValidation.ValidateToken(token))
            {
                return RedirectToAction(nameof(Index), nameof(HomeController).Replace("Controller", ""));
            }

            var claim = _tokenValidation.GetClaim(token, "nameid");
            if (String.IsNullOrEmpty(claim))
            {
                return RedirectToAction(nameof(Index), nameof(HomeController).Replace("Controller", ""));
            }

            var userToken = _userRepo.GetSingle(u => u.Id.ToString().Equals(claim)).Token;
            if (token == userToken)
            {
                _userRepo.GetSingle(u => u.Token == token).Token = null;
                _userRepo.Save().Wait();
                await UserAuthenticationAsync(claim);
                return RedirectToAction(nameof(DashboardController.Complete), nameof(DashboardController).Replace("Controller", ""));
            }

            return RedirectToAction(nameof(Index), nameof(HomeController).Replace("Controller", ""));
        }

        public User GetUser(string user)
        {
            return _userRepo.GetSingle(u => u.Username == user || u.Email == user);
        }
    }
}
