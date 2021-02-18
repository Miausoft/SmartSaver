using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartSaver.Domain.Repositories;
using SmartSaver.EntityFrameworkCore.Models;
using SmartSaver.MVC.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

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
            var accounts = _accountRepo.SearchFor(a => a.UserId.ToString().Equals(User.Identity.Name));
            return !accounts.Any() ? View(nameof(Complete)) : View(accounts);
        }

        [HttpGet]
        public IActionResult Complete()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Complete(AccountViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            await _accountRepo.InsertAsync(new Account
            {
                Name = model.Name,
                UserId = int.Parse(User.Identity.Name),
                Goal = model.Goal,
                Revenue = model.Revenue,
                GoalStartDate = DateTime.Today,
                GoalEndDate = model.GoalEndDate
            });

            try
            {
                await _accountRepo.SaveAsync();
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(string.Empty, $"{model.Name} goal is already set. Please select a new name");
                return View();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string name)
        {
            try
            {
                var account = _accountRepo
                    .SearchFor(a => a.UserId.ToString().Equals(User.Identity.Name) &&
                                    a.Name.Equals(name))
                    .FirstOrDefault();

                _accountRepo.Delete(account);
                await _accountRepo.SaveAsync();
            }
            catch (ArgumentNullException)
            {
                // TODO: need to send the error message to the Index view
            }
            catch (DbUpdateException)
            {
                // TODO: catching racing condition. Need to send the error message to the Index view
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
