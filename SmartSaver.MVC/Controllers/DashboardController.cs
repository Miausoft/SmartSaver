using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartSaver.EntityFrameworkCore.Models;
using SmartSaver.MVC.Models;
using SmartSaver.Domain.Services.Transactions;
using SmartSaver.Domain.Services.SavingSuggestions;
using SmartSaver.Domain.Repositories;
using SmartSaver.Domain.CustomAttributes;
using System;
using System.Linq;
using SmartSaver.Domain.Helpers;

namespace SmartSaver.MVC.Controllers
{
    [Authorize, RequiresAccount]
    public class DashboardController : Controller
    {
        private readonly IRepository<Account> _accountRepo;
        private readonly IRepository<Transaction> _transactionRepo;
        private readonly IRepository<Category> _categoryRepo;
        private readonly ITransactionsService _transactionsService;
        private readonly ISuggestions _suggestions;

        public DashboardController(IRepository<Account> accountRepo,
                                   IRepository<Transaction> transactionRepo,
                                   IRepository<Category> categoryRepo,
                                   ITransactionsService transactionsService,
                                   ISuggestions suggestions)
        {
            _accountRepo = accountRepo;
            _transactionRepo = transactionRepo;
            _categoryRepo = categoryRepo;
            _transactionsService = transactionsService;
            _suggestions = suggestions;
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
                SavedCurrentMonth = _transactionsService
                    .SavedSum(
                        account.Transactions,
                        DateTimeHelper.TruncateToDayStart(DateTime.Now),
                        DateTimeHelper.TruncateToDayStart(DateTime.Now.AddMonths(1))),

                ToSaveCurrentMonth = _suggestions.AmountToSaveAMonth(account),

                CurrentMonthBalanceHistory = _transactionsService
                    .BalanceHistory(account.Transactions)
                    .Where(x => x.Key.Year == DateTime.Now.Year && x.Key.Month == DateTime.Now.Month)
                    .ToDictionary(x => x.Key, x => x.Value),

                CurrentMonthTotalExpenseByCategory = _transactionsService
                    .TotalExpenseByCategory(
                        account.Transactions,
                        _categoryRepo.GetAll(),
                        DateTimeHelper.TruncateToDayStart(DateTime.Now),
                        DateTimeHelper.TruncateToDayStart(DateTime.Now.AddMonths(1))),

                FreeMoneyToSpend = _suggestions.FreeMoneyToSpend(account),

                EstimatedTime = _suggestions.EstimatedTime(account)
            });
        }
    }
}
