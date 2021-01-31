using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        private readonly IRepository<Transaction> _transactionRepo;
        private readonly IRepository<Account> _accountRepo;
        private readonly IRepository<Category> _categoryRepo;

        public DashboardController(IRepository<Transaction> transactionRepo,
                                   IRepository<Account> accountRepo,
                                   IRepository<Category> categoryRepo)
        {
            _transactionRepo = transactionRepo;
            _accountRepo = accountRepo;
            _categoryRepo = categoryRepo;
        }

        [HttpGet]
        public IActionResult Index()
        {
            Account account = _accountRepo.SearchFor(a => a.UserId.ToString().Equals(User.Identity.Name)).FirstOrDefault();
            if (account == null)
            {
                return View(nameof(AccountController.Index));
            }

            account.Transactions = _transactionRepo
                .SearchFor(t => t.AccountId.ToString().Equals(account.Id.ToString()))
                .ToList();

            return View(new DashboardViewModel()
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
                    .TotalExpenseByCategory(account.Transactions, _categoryRepo.GetAll(), new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1), DateTime.Now.AddDays(1)),

                FreeMoneyToSpend = MoneyCounter.FreeMoneyToSpend(account),

                EstimatedTime = MoneyCounter.EstimatedTime(account)
            });
        }
    }
}
