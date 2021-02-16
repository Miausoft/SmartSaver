using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartSaver.Domain.Helpers;
using SmartSaver.Domain.Services.Transactions;
using SmartSaver.Domain.Services.SavingSuggestions;
using SmartSaver.Domain.Repositories;
using SmartSaver.Domain.CustomAttributes;
using SmartSaver.EntityFrameworkCore.Models;
using SmartSaver.MVC.Models;
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
                                t.AccountId == account.Id)
                .ToList();

            var nowStart = account.GoalStartDate.Year == DateTime.Now.Year && account.GoalStartDate.Month == DateTime.Now.Month
                ? account.GoalStartDate
                : DateTimeHelper.TruncateToDayStart(DateTime.Now);

            var nowEnd = account.GoalEndDate.Year == DateTime.Now.Year && account.GoalEndDate.Month == DateTime.Now.Month
                ? account.GoalEndDate
                : DateTimeHelper.TruncateToDayStart(DateTime.Now.AddMonths(1));

            return View(new DashboardViewModel()
            {
                SavedCurrentMonth = _transactionsService
                    .AmountSaved(account.Transactions, nowStart, nowEnd),

                ToSaveCurrentMonth = _suggestions.AmountToSaveAMonth(account),

                CurrentMonthBalanceHistory = _transactionsService
                    .BalanceHistory(account.Transactions)
                    .Where(x => DateTimeHelper.InRange(x.Key.Date, nowStart, nowEnd))
                    .ToDictionary(x => x.Key, x => x.Value),

                CurrentMonthTotalExpenseByCategory = _transactionsService
                    .TotalExpenseByCategory(account.Transactions, _categoryRepo.GetAll(), nowStart, nowEnd),

                FreeMoneyToSpend = _suggestions.FreeMoneyToSpend(account),

                EstimatedTime = _suggestions.EstimatedTime(account)
            });
        }
    }
}
