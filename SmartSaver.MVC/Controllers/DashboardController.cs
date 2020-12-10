using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartSaver.EntityFrameworkCore;
using SmartSaver.EntityFrameworkCore.Models;
using SmartSaver.MVC.Models;
using SmartSaver.Domain.Services.TransactionsCounterService;
using SmartSaver.Domain.Services.SavingMethodSuggestion;
using SmartSaver.EntityFrameworkCore.Repositories;

namespace SmartSaver.MVC.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ITransactionRepo _transactionRepo;
        private readonly IAccountRepo _accountRepo;

        public DashboardController(ApplicationDbContext context, ITransactionRepo transactionRepo, IAccountRepo accountRepo)
        {
            _context = context;
            _transactionRepo = transactionRepo;
            _accountRepo = accountRepo;
        }
        
        [HttpGet]
        public IActionResult Index()
        {
            var account = _accountRepo.GetAccountByUsername(User.Identity.Name);
            if (!_accountRepo.IsAccountValid(account))
            {
                return View(nameof(Complete));
            }

            DashboardViewModel dvm = new DashboardViewModel()
            {
                SavedCurrentMonth = TransactionsCounter
                    .AmountSavedCurrentMonth(_context.Transactions
                    .Where(t => t.AccountId == account.Id)),
                
                ToSaveCurrentMonth = MoneyCounter
                    .AmountToSaveAMonth(account.Goal, account.GoalStartDate, account.GoalEndDate),
                
                FirstChartData = _transactionRepo.GetBalanceHistory(account.Id),
                
                SpendingTransactions = _transactionRepo.GetAccountSpendings(account.Id),
                
                Transactions = _context.Accounts.Include(nameof(Transaction) + "s").First(a => a.Id == account.Id).Transactions.ToList()
            };

            return View(dvm);
        }

        [HttpGet]
        public IActionResult Complete()
        {
            var account = _accountRepo.GetAccountByUsername(User.Identity.Name);
            if (!_accountRepo.IsAccountValid(account))
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
                    Account current = _accountRepo.GetAccountByUsername(User.Identity.Name);

                    current.Goal = account.Goal;
                    current.Revenue = account.Revenue;
                    current.GoalStartDate = DateTime.Now;
                    current.GoalEndDate = account.GoalEndDate;

                    _context.SaveChanges();

                    return RedirectToAction(nameof(Index), account);
                }
                ModelState.AddModelError(nameof(account.GoalEndDate), "Invalid date");
            }

            return View();
        }
    }
}
