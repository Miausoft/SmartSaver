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
using System.Linq;
using SmartSaver.Domain.CustomAttributes;

namespace SmartSaver.MVC.Controllers
{
    [AnonymousOnly]
    public class AuthenticationController : Controller
    {
        private readonly Domain.Services.AuthenticationServices.IAuthenticationService _auth;
        private readonly IConfiguration _configuration;
        private readonly IRepository<User> _userRepo;
        private readonly ITokenValidationService _tokenValidation;
        private readonly IMailer _mailer;

        public AuthenticationController(Domain.Services.AuthenticationServices.IAuthenticationService auth,
                                        IConfiguration configuration,
                                        IRepository<User> userRepo,
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
            return View();
        }

        public IActionResult Login()
        {
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
            User user = _userRepo.SearchFor(e => e.Email.Equals(email)).FirstOrDefault();

            if (String.IsNullOrEmpty(email) || user == null || user.Token == null)
            {
                return RedirectToAction(nameof(Login));
            }

            var confirmationLink = Url.Action(nameof(ConfirmEmail), nameof(AuthenticationController).Replace(nameof(Controller), ""), new { token = user.Token }, Request.Scheme);

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
        public IActionResult Login(AuthenticationViewModel model, string returnUrl)
        {
            User user = _auth.Login(model.Email, model.Password);

            if (user == null)
            {
                ModelState.AddModelError(nameof(model.Email), "Incorrect username or password.");
                return View();
            }

            if (user.Token != null)
            {
                return View(nameof(Verify), ViewBag.Email = model.Email);
            }

            UserAuthentication(user.Id);
            return LocalRedirect(returnUrl ?? Url.Content("~/"));
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction(nameof(HomeController.Index), nameof(HomeController).Replace(nameof(Controller), ""));
        }

        [HttpPost]
        public IActionResult ExternalLogin(string returnUrl, string provider)
        {
            var properties = new AuthenticationProperties { RedirectUri = Url.Action(nameof(ExternalResponse), new { ReturnUrl = returnUrl }) };
            return Challenge(properties, provider);
        }

        [AllowAnonymous]
        public IActionResult ExternalResponse(string returnUrl)
        {
            var result = HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (result.Result.Succeeded)
            {
                var email = result.Result.Principal.FindFirstValue(ClaimTypes.Email);
                AddNewUser(email, email, null);
                UserAuthentication(_userRepo.SearchFor(e => e.Email.Equals(email)).FirstOrDefault().Id);
                return LocalRedirect(returnUrl ?? Url.Content("~/"));
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Error while trying to login.");
                return View(nameof(Login));
            }
        }

        private void UserAuthentication<T>(T userId)
        {
            var claim = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userId.ToString())
            };
            var identity = new ClaimsIdentity(claim, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal).Wait();
        }

        private bool AddNewUser(string username, string email, string password)
        {
            if (_userRepo.SearchFor(u => u.Email.Equals(email)).FirstOrDefault() != null)
            {
                return false;
            }

            _auth.Register(new User() { Username = username, Email = email, Password = password });

            if (password != null)
            {
                User user = _userRepo.SearchFor(u => u.Email.Equals(email)).FirstOrDefault();
                user.Token = _tokenValidation.GenerateToken(user.Id);
                _userRepo.Save();
            }

            return true;
        }

        [HttpGet]
        public IActionResult ConfirmEmail(string token)
        {
            if (token == null)
            {
                return RedirectToAction(nameof(HomeController.Index), nameof(HomeController).Replace(nameof(Controller), ""));
            }

            if (!_tokenValidation.ValidateToken(token))
            {
                return RedirectToAction(nameof(HomeController.Index), nameof(HomeController).Replace(nameof(Controller), ""));
            }

            var claim = _tokenValidation.GetClaim(token, "nameid");
            if (String.IsNullOrEmpty(claim))
            {
                return RedirectToAction(nameof(HomeController.Index), nameof(HomeController).Replace(nameof(Controller), ""));
            }

            User user = _userRepo.SearchFor(u => u.Id.ToString().Equals(claim)).FirstOrDefault();
            if (token == user.Token)
            {
                user.Token = null;
                _userRepo.Save();
                UserAuthentication(claim);
                return RedirectToAction(nameof(AccountController.Complete), nameof(AccountController).Replace(nameof(Controller), ""));
            }

            return RedirectToAction(nameof(HomeController.Index), nameof(HomeController).Replace(nameof(Controller), ""));
        }

        public User GetUser(string user)
        {
            return _userRepo.SearchFor(u => u.Username.Equals(user) || u.Email.Equals(user)).FirstOrDefault();
        }
    }
}
