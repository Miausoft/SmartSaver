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
using SmartSaver.Domain.Repositories;

namespace SmartSaver.MVC.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ITransactionRepoositry _transactionRepo;
        private readonly IAccountRepoository _accountRepo;
        private readonly ICategoryRepository _categoryRepo;

        public DashboardController(ApplicationDbContext context, 
                                   ITransactionRepoositry transactionRepo, 
                                   IAccountRepoository accountRepo,
                                   ICategoryRepository categoryRepo)
        {
            _context = context;
            _transactionRepo = transactionRepo;
            _accountRepo = accountRepo;
            _categoryRepo = categoryRepo;
        }
        
        [HttpGet]
        public IActionResult Index()
        {
            var account = _accountRepo.GetAccountById(User.Identity.Name);
            if (!_accountRepo.IsAccountValid(account))
            {
                return View(nameof(Complete));
            }

            account.Transactions = _context.Transactions
                    .Where(t => t.AccountId == account.Id).ToList();

            DashboardViewModel dvm = new DashboardViewModel()
            {
                SavedCurrentMonth = TransactionsCounter
                    .AmountSavedCurrentMonth(account.Transactions),
                
                ToSaveCurrentMonth = MoneyCounter
                    .AmountToSaveAMonth(account),

                CurrentMonthBalanceHistory = TransactionsCounter
                    .BalanceHistory(account.Transactions)
                    .Where(x => x.Key.Year == DateTime.Now.Year && x.Key.Month == DateTime.Now.Month)
                    .ToDictionary(x => x.Key, x => x.Value),

                CurrentMonthTotalExpenseByCategory = TransactionsCounter
                    .TotalExpenseByCategory(account.Transactions, _categoryRepo.GetMultiple(), new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1), DateTime.Now.AddDays(1))
            };

            return View(dvm);
        }

        [HttpGet]
        public IActionResult Complete()
        {
            var account = _accountRepo.GetAccountById(User.Identity.Name);
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
                    Account current = _accountRepo.GetAccountById(User.Identity.Name);

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
