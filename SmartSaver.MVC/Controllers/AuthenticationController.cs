using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SmartSaver.Domain.CustomAttributes;
using SmartSaver.Domain.Repositories;
using SmartSaver.Domain.Services.AuthenticationServices;
using SmartSaver.Domain.Services.EmailServices;
using SmartSaver.Domain.Services.AuthenticationServices.Jwt;
using SmartSaver.EntityFrameworkCore.Models;
using SmartSaver.MVC.Models;
using System.Threading;

namespace SmartSaver.MVC.Controllers
{
    [AnonymousOnly]
    public class AuthenticationController : Controller
    {
        private readonly IAuthentication _auth;
        private readonly ITokenAuthentication _tokenAuth;
        private readonly IConfiguration _configuration;
        private readonly IRepository<User> _userRepo;
        private readonly IMailer _mailer;

        public AuthenticationController(IAuthentication auth,
                                        IConfiguration configuration,
                                        ITokenAuthentication tokenAuth,
                                        IRepository<User> userRepo,
                                        IMailer mailer)
        {
            _auth = auth;
            _configuration = configuration;
            _tokenAuth = tokenAuth;
            _userRepo = userRepo;
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
        public async Task<IActionResult> Register(AuthenticationViewModel model)
        {
            if (ModelState.IsValid)
            {
                var registrationResult = await _auth.RegisterAsync(new User() { Username = model.Username, Email = model.Email, Password = model.Password });
                if (registrationResult == RegistrationResult.Success)
                {
                    return RedirectToAction(nameof(Verify), new { model.Email });
                }
            }

            ModelState.AddModelError(string.Empty, "Registration failed. Please try again.");
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Verify(string email)
        {
            User user = _userRepo.SearchFor(e => e.Email.Equals(email)).FirstOrDefault();

            if (user == null || user.Token == null)
            {
                return new NotFoundResult();
            }

            user.Token = _tokenAuth.GenerateToken(user.Id);
            var saveTask = _userRepo.SaveAsync();

            var confirmationLink = Url.Action(
                nameof(ConfirmEmail),
                nameof(AuthenticationController).Replace(nameof(Controller), ""),
                new { token = user.Token },
                Request.Scheme);

            _mailer.SendEmailAsync(
                new MailMessage(
                    _configuration["Email:Address"],
                    _configuration["Email:Address"],
                    "Verify your email address",
                    System.IO.File.ReadAllText(_configuration["TemplatePaths:Email"]).Replace("@ViewBag.VerifyLink", confirmationLink))
            { IsBodyHtml = true });

            await saveTask;

            return View(nameof(Verify), ViewBag.Email = user.Email);
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

            _auth.SignInAsync(user.Id).Wait();
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
            var properties = new AuthenticationProperties { RedirectUri = Url.Action(nameof(ExternalResponse), new { returnUrl, provider }) };
            return Challenge(properties, provider);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalResponse(string returnUrl, string provider)
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (result.Succeeded)
            {
                var email = result.Principal.FindFirstValue(ClaimTypes.Email);
                var registrationResult = await _auth.RegisterAsync(new User() { Username = email, Email = email, Password = null });

                if (registrationResult == RegistrationResult.Success || registrationResult == RegistrationResult.UserAlreadyExist)
                {
                    await _auth.SignInAsync(_userRepo.SearchFor(e => e.Email.Equals(email)).FirstOrDefault().Id);
                    return LocalRedirect(returnUrl ?? Url.Content("~/"));
                }
            }

            ModelState.AddModelError(string.Empty, $"Something unexpected happened with {provider} authentication. Please try again, or if this doesn't work, contact us for help");
            return View(nameof(Login));
        }

        [HttpGet]
        public IActionResult ConfirmEmail(string token)
        {
            if (_auth.VerifyEmailAsync(_userRepo.SearchFor(u => u.Token.Equals(token)).FirstOrDefault()).Result)
            {
                return View("Success");
            }
            else
            {
                return View("Failure");
            }
        }
    }
}
