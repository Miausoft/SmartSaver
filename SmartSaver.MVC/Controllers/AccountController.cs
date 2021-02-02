using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartSaver.Domain.CustomAttributes;
using SmartSaver.Domain.Repositories;
using SmartSaver.EntityFrameworkCore.Models;
using SmartSaver.MVC.Models;
using System;
using System.Linq;

namespace SmartSaver.MVC.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly IRepository<Account> _accountRepo;

        public AccountController(IRepository<Account> accountRepo)
        {
            _accountRepo = accountRepo;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var accounts = _accountRepo.SearchFor(a => a.UserId.ToString().Equals(User.Identity.Name)).ToList();
            if (!accounts.Any())
            {
                return View(nameof(Complete));
            }

            return View(accounts);
        }

        [HttpGet]
        public IActionResult Complete()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Complete(AccountViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            _accountRepo.Insert(new Account
            {
                Name = model.Name,
                UserId = int.Parse(User.Identity.Name),
                Goal = model.Goal,
                Revenue = model.Revenue,
                GoalStartDate = DateTime.Today,
                GoalEndDate = model.GoalEndDate
            });
            _accountRepo.Save();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [RequiresAccount]
        public IActionResult Delete()
        {
            _accountRepo.Delete(_accountRepo.SearchFor(a => a.UserId.ToString().Equals(User.Identity.Name)).FirstOrDefault());
            _accountRepo.Save();

            return RedirectToAction(nameof(Index));
        }
    }
}
