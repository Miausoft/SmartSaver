using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        public AuthenticationController(ApplicationDbContext db, Domain.Services.AuthenticationServices.IAuthenticationService auth)
        {
            _db = db;
            _auth = auth;
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
                return RedirectToAction(nameof(Login));
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(User user, string returnUrl)
        {
            if (ModelState.IsValid && _auth.Login(user.Email, user.Password) != null)
            {
                UserAuthentication(user.Email);
                return LocalRedirect(returnUrl ?? Url.Content("~/"));
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

            var properties = new AuthenticationProperties { RedirectUri = Url.Action(nameof(ExternalResponse), new { ReturnUrl = returnUrl}) };
            return Challenge(properties, provider);
        }

        public async Task<IActionResult> ExternalResponse (string returnUrl)
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (result.Succeeded)
            {
                var email = result.Principal.FindFirstValue(ClaimTypes.Email);
                AddNewUser(email, email, null);
                UserAuthentication(email);
                return LocalRedirect(returnUrl ?? Url.Content("~/"));
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Error while trying to login");
                return View(nameof(Login));
            }
        }

        private void UserAuthentication(string email)
        {
            var claim = new List<Claim> { new Claim(ClaimTypes.Name, email) };
            var identity = new ClaimsIdentity(claim, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal).Wait();
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
    }
}
