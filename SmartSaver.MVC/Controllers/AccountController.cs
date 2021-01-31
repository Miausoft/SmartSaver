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
        [RequiresAccount]
        public IActionResult Index()
        {
            return View();
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
            _accountRepo.Delete(_accountRepo.SearchFor(a => a.UserId.ToString().Equals(User.Identity.Name)).FirstOrDefault().Id);
            _accountRepo.Save();

            return RedirectToAction(nameof(Index));
        }
    }
}
