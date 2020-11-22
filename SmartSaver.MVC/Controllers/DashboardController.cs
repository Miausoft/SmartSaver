using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartSaver.EntityFrameworkCore;
using SmartSaver.EntityFrameworkCore.Models;
using SmartSaver.MVC.Models;
using SmartSaver.Domain.Services.TransactionsCounterService;
using SmartSaver.Domain.Services.SavingMethodSuggestion;
using SmartSaver.Domain.Managers;

namespace SmartSaver.MVC.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly TransactionManager _manager;

        public DashboardController(ApplicationDbContext context, TransactionManager manager)
        {
            _context = context;
            _manager = manager;
        }
        
        [HttpGet]
        public IActionResult Index()
        {
            var account = GetAccountAuth();
            if (!account.AccountValid())
            {
                return View(nameof(Complete));
            }

            Account acc = GetAccountAuth();

            DashboardViewModel dvm = new DashboardViewModel()
            {
                SavedCurrentMonth = TransactionsCounter
                    .AmountSavedCurrentMonth(_context.Transactions
                    .Where(t => t.AccountId == acc.Id)),
                
                ToSaveCurrentMonth = MoneyCounter
                    .AmountToSaveAMonth(acc.Goal, acc.GoalStartDate, acc.GoalEndDate),
                
                FirstChartData = _manager.GetBalanceHistory(acc.Id),
                
                SpendingTransactions = _manager.GetAccountSpendings(acc.Id),
                
                Transactions = _context.Accounts.Include(nameof(Transaction) + "s").First(a => a.Id == acc.Id).Transactions.ToList()
            };

            return View(dvm);
        }

        [HttpGet]
        public IActionResult Complete()
        {
            var account = GetAccountAuth();
            if (!account.AccountValid())
            {
                return View(nameof(Complete));
            }

            return RedirectToAction(nameof(Index), account);
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

                    return View(nameof(Index));
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
                .First(u => u.Email.Equals(User.Identity.Name))
                .Account;
        }
    }
}
