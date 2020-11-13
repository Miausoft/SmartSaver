using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
    public class AuthenticationController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly Domain.Services.AuthenticationServices.IAuthenticationService _auth;
        private User _user;

        public AuthenticationController(ApplicationDbContext db, Domain.Services.AuthenticationServices.IAuthenticationService auth)
        {
            _db = db;
            _auth = auth;
        }

        [AllowAnonymous]
        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction(nameof(DashboardController.Index), nameof(DashboardController).Replace("Controller", ""));
            }
            return View();
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            if(User.Identity.IsAuthenticated)
            {
                return RedirectToAction(nameof(DashboardController.Index), nameof(DashboardController).Replace("Controller", ""));
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (!DoesUsernameExist(model.Username))
                {
                    RegistrationResult registrationResult = _auth.Register(new User()
                    {
                        Username = model.Username,
                        Password = model.Password
                    });

                    // TODO: check for registration result.

                    return RedirectToAction(nameof(Login));
                }

                ModelState.AddModelError(nameof(model.Username), "Username is already taken.");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid && _auth.Login(model.Username, model.Password) != null)
            {
                _user = _auth.Login(model.Username, model.Password);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, model.Username)
                };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                var props = new AuthenticationProperties();
                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, props).Wait();

                return RedirectToAction(nameof(DashboardController.Index), nameof(DashboardController).Replace("Controller", ""));
            }

            ModelState.AddModelError(nameof(model.Username), "Incorrect username or password.");
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction(nameof(HomeController.Index), nameof(HomeController).Replace("Controller", ""));
        }

        public bool DoesUsernameExist(string username)
        {
            System.Threading.Thread.Sleep(200);

            if (_db.Users.FirstOrDefault(u => u.Username.Equals(username)) != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
