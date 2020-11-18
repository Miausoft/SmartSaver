using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartSaver.EntityFrameworkCore;
using SmartSaver.EntityFrameworkCore.Models;

namespace SmartSaver.MVC.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var account = GetAccountAuth();
            if (!account.AccountValid())
            {
                return View(nameof(Complete));
            }

            return View(account);
        }

        public IActionResult Complete()
        {
            var account = GetAccountAuth();
            if (!account.AccountValid())
            {
                return View(nameof(Complete));
            }

            return View(nameof(Index), account);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Complete(Account account)
        {
            if (ModelState.IsValid)
            {
                if (account.DateValid())
                {
                    Account current = GetAccountAuth();

                    current.Goal = account.Goal;
                    current.Revenue = account.Revenue;
                    current.GoalStartDate = DateTime.Now;
                    current.GoalEndDate = account.GoalEndDate;

                    _context.SaveChanges();

                    return View(nameof(Index), account);
                }
                ModelState.AddModelError(nameof(account.GoalEndDate), "Invalid date");
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Delete()
        {
            await Task.Run(() =>
            {
                Thread.Sleep(1000);
                Account acc = GetAccountAuth();
                acc.GoalStartDate = DateTime.MinValue;
                acc.GoalEndDate = DateTime.MinValue;
                acc.Goal = 0;
                _context.SaveChanges();
            });
            return RedirectToAction(nameof(HomeController.Index), nameof(HomeController).Replace("Controller", ""));
        }

        private Account GetAccountAuth()
        {
            return _context.Users
                .Include(u => u.Account)
                .First(u => u.Id.ToString().Equals(User.Identity.Name))
                .Account;
        }
    }
}
