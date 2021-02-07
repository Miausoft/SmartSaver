using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartSaver.EntityFrameworkCore.Models;
using SmartSaver.MVC.Models;
using SmartSaver.Domain.Services.TransactionsCounterService;
using SmartSaver.Domain.Services.SavingMethodSuggestion;
using SmartSaver.Domain.Repositories;
using SmartSaver.Domain.CustomAttributes;
using System;
using System.Linq;

namespace SmartSaver.MVC.Controllers
{
    [Authorize, RequiresAccount]
    public class DashboardController : Controller
    {
        private readonly IRepository<Account> _accountRepo;
        private readonly IRepository<Transaction> _transactionRepo;
        private readonly IRepository<Category> _categoryRepo;

        public DashboardController(IRepository<Account> accountRepo,
                                   IRepository<Transaction> transactionRepo,
                                   IRepository<Category> categoryRepo)
        {
            _accountRepo = accountRepo;
            _transactionRepo = transactionRepo;
            _categoryRepo = categoryRepo;
        }

        [HttpGet]
        public IActionResult Index(string name)
        {
            var account = _accountRepo
                .SearchFor(a => a.UserId.ToString().Equals(User.Identity.Name) &&
                                a.Name.Equals(name))
                .FirstOrDefault();

            account.Transactions = _transactionRepo
                .SearchFor(t => t.UserId.ToString().Equals(User.Identity.Name) &&
                                t.AccountId.ToString().Equals(account.Id.ToString()))
                .ToList();

            return View(new DashboardViewModel()
            {
                SavedCurrentMonth = TransactionsCounter
                    .SavedSum(
                        account.Transactions,
                        DateCounter.TruncateToDayStart(DateTime.Now),
                        DateCounter.TruncateToDayStart(DateTime.Now.AddMonths(1))),

                ToSaveCurrentMonth = MoneyCounter.AmountToSaveAMonth(account),

                CurrentMonthBalanceHistory = TransactionsCounter
                    .BalanceHistory(account.Transactions)
                    .Where(x => x.Key.Year == DateTime.Now.Year && x.Key.Month == DateTime.Now.Month)
                    .ToDictionary(x => x.Key, x => x.Value),

                CurrentMonthTotalExpenseByCategory = TransactionsCounter
                    .TotalExpenseByCategory(
                        account.Transactions,
                        _categoryRepo.GetAll(),
                        DateCounter.TruncateToDayStart(DateTime.Now),
                        DateCounter.TruncateToDayStart(DateTime.Now.AddMonths(1))),

                FreeMoneyToSpend = MoneyCounter.FreeMoneyToSpend(account),

                EstimatedTime = MoneyCounter.EstimatedTime(account)
            });
        }
    }
}
