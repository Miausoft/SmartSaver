using System;
using System.Linq;
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
            if (!AccountValid())
                return RedirectToAction(nameof(Complete));

            return View();
        }

        public IActionResult Complete()
        {
            var account = GetAccountAuth();
            return View(account);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Complete([Bind("Goal,Revenue,GoalEndDate")] Account account)
        {
            Account current = _context.Users
                .Include(u => u.Account)
                .First(u => u.Username.Equals(User.Identity.Name))
                .Account;

            current.Goal = account.Goal;
            current.Revenue = account.Revenue;
            current.GoalStartDate = DateTime.Now;
            current.GoalEndDate = account.GoalEndDate;

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        private Account GetAccountAuth()
        {
            return _context.Users
                .Include(u => u.Account)
                .First(u => u.Username.Equals(User.Identity.Name))
                .Account;
        }

        private bool AccountValid()
        {
            return GetAccountAuth().Goal > 0;
        }
    }
}
