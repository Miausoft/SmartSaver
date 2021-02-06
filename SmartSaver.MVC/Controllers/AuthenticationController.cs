using System.Linq;
using System.Net;
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
using SmartSaver.Domain.CustomExceptions;
using SmartSaver.Domain.Services.EmailServices;
using SmartSaver.Domain.Repositories;
using SmartSaver.EntityFrameworkCore.Models;
using SmartSaver.MVC.Models;
using SmartSaver.Domain.Services.AuthenticationServices;
using SmartSaver.Domain.TokenValidation;

namespace SmartSaver.MVC.Controllers
{
    [AnonymousOnly]
    public class AuthenticationController : Controller
    {
        private readonly Domain.Services.AuthenticationServices.IAuthenticationService _auth;
        private readonly ITokenValidationService _tokenValidation;
        private readonly IConfiguration _configuration;
        private readonly IRepository<User> _userRepo;
        private readonly IMailer _mailer;

        public AuthenticationController(Domain.Services.AuthenticationServices.IAuthenticationService auth,
                                        IConfiguration configuration,
                                        ITokenValidationService tokenValidation,
                                        IRepository<User> userRepo,
                                        IMailer mailer)
        {
            _auth = auth;
            _configuration = configuration;
            _tokenValidation = tokenValidation;
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
        public IActionResult Register(AuthenticationViewModel model)
        {
            if (ModelState.IsValid)
            {
                var registrationResult = _auth.Register(new User() { Username = model.Username, Email = model.Email, Password = model.Password });
                if (registrationResult == RegistrationResult.Success)
                {
                    return RedirectToAction(nameof(Verify), new { model.Email });
                }
            }

            return View();
        }

        [HttpGet]
        public IActionResult Verify(string email)
        {
            User user = _userRepo.SearchFor(e => e.Email.Equals(email)).FirstOrDefault();

            if (user == null || user.Token == null)
            {
                throw new HttpStatusException(HttpStatusCode.NotFound, "404 Error Occured.");
            }

            var confirmationLink = Url.Action(
                nameof(ConfirmEmail), 
                nameof(AuthenticationController).Replace(nameof(Controller), ""), 
                new { token = _tokenValidation.GenerateToken(user.Id) }, 
                Request.Scheme);

            _mailer.SendEmailAsync(
                new MailMessage(
                    _configuration["Email:Address"],
                    _configuration["Email:Address"],
                    "Verify your email address",
                     System.IO.File.ReadAllText(_configuration["TemplatePaths:Email"]).Replace("@ViewBag.VerifyLink", confirmationLink))
                { IsBodyHtml = true });

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
        public IActionResult ExternalResponse(string returnUrl, string provider)
        {
            var result = HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (result.Result.Succeeded)
            {
                var email = result.Result.Principal.FindFirstValue(ClaimTypes.Email);
                var registrationResult = _auth.Register(new User() { Username = email, Email = email, Password = null });

                if (registrationResult == RegistrationResult.Success || registrationResult == RegistrationResult.UserAlreadyExist)
                {
                    _auth.SignInAsync(_userRepo.SearchFor(e => e.Email.Equals(email)).FirstOrDefault().Id);
                    return LocalRedirect(returnUrl ?? Url.Content("~/"));
                }
            }

            ModelState.AddModelError(string.Empty, $"Something unexpected happened with {provider} authentication. Please try again, or if this doesn't work, contact us for help");
            return View(nameof(Login));
        }

        [HttpGet]
        public IActionResult ConfirmEmail (string token)
        {
            if (_auth.VerifyEmail(_userRepo.SearchFor(u => u.Token.Equals(token)).FirstOrDefault()))
            {
                return View("Success");
            }
            else
            {
                return View("Failure");
            }
        }

        public User GetUser(string user)
        {
            return _userRepo.SearchFor(u => u.Username.Equals(user) || u.Email.Equals(user)).FirstOrDefault();
        }
    }
}
