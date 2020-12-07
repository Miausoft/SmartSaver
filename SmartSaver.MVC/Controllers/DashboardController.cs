using System;
using System.Collections.Generic;
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

            Account acc = GetAccountAuth();

            DashboardViewModel dvm = new DashboardViewModel()
            {
                SavedCurrentMonth = TransactionsCounter
                    .AmountSavedCurrentMonth(_context.Transactions
                    .Where(t => t.AccountId == acc.Id)),
                
                ToSaveCurrentMonth = MoneyCounter
                    .AmountToSaveAMonth(acc.Goal, acc.GoalStartDate, acc.GoalEndDate),
                
                FirstChartData = GetBalanceHistory(),
                
                SpendingTransactions = GetSpendingsGroupedByCategory()
            };

            return View(dvm);
        }

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
                .First(u => u.Id.ToString().Equals(User.Identity.Name))
                .Account;
        }

        private bool AccountValid()
        {
            return GetAccountAuth().Goal > 0;
        }

        private DateTime CurrentMonthFirstDayDate()
        {
            var date = new DateTime();
            var thisMonth = date.Month;
            var thisYear = date.Year;

            return new DateTime(thisYear, thisMonth, 1);
        }
        
        private List<Balance> GetBalanceHistory()
        {
            var chartData = new List<Balance>();
            
            var transactions = _context.Transactions.Include(nameof(Category))
                .Where(t => t.ActionTime > CurrentMonthFirstDayDate() && t.AccountId == GetAccountAuth().Id);
            
            decimal balance = 0;
            foreach (var t in transactions)
            {
                balance += t.Amount;
                chartData.Add(new Balance()
                {
                    Amount = balance,
                    ActionTime = t.ActionTime
                });
            }

            return chartData;
        }

        private List<Transaction> GetSpendingsGroupedByCategory()
        {

            var id = GetAccountAuth().Id;
            var date = CurrentMonthFirstDayDate();

            var transactions = _context.Transactions
                .Include(nameof(Category))
                .Where(t => t.ActionTime > date &&
                            t.AccountId == id
                            && t.Category.TypeOfIncome == false).ToList();
            return transactions;
        }
    }
}
