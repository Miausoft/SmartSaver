﻿using System;
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
        private readonly ITransactionRepoositry _transactionRepo;
        private readonly IAccountRepoository _accountRepo;
        private readonly ICategoryRepository _categoryRepo;

        public DashboardController(ITransactionRepoositry transactionRepo,
                                   IAccountRepoository accountRepo,
                                   ICategoryRepository categoryRepo)
        {
            _transactionRepo = transactionRepo;
            _accountRepo = accountRepo;
            _categoryRepo = categoryRepo;
        }

        [HttpGet]
        public IActionResult Index()
        {
            Account account = _accountRepo.GetById(User.Identity.Name);
            if (!_accountRepo.IsValid(account))
            {
                return View(nameof(Complete));
            }

            account.Transactions = _transactionRepo
                .GetByAccountId(account.Id)
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
                    .TotalExpenseByCategory(account.Transactions, _categoryRepo.GetMultiple(), new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1), DateTime.Now.AddDays(1)),

                FreeMoneyToSpend = MoneyCounter.FreeMoneyToSpend(account),

                EstimatedTime = MoneyCounter.EstimatedTime(account)
            });
        }

        [HttpGet]
        public IActionResult Complete()
        {
            var account = _accountRepo.GetById(User.Identity.Name);
            if (!_accountRepo.IsValid(account))
            {
                return View(nameof(Complete));
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Complete(Account account)
        {
            if (ModelState.IsValid)
            {
                if (account.GoalEndDate <= DateTime.Now)
                {
                    ModelState.AddModelError("GoalEndDate", "Invalid date");
                    return View();
                }

                Account current = _accountRepo.GetById(User.Identity.Name);
                current.Goal = account.Goal;
                current.Revenue = account.Revenue;
                current.GoalStartDate = DateTime.Today;
                current.GoalEndDate = account.GoalEndDate;
                _accountRepo.Save().Wait();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                if (account.GoalEndDate <= DateTime.Now)
                {
                    ModelState.AddModelError("GoalEndDate", "Invalid date");
                }

                return View();
            }
        }

        [HttpPost]
        public IActionResult Delete()
        {
            Account current = _accountRepo.GetById(User.Identity.Name);
            current.GoalStartDate = DateTime.MinValue;
            current.GoalEndDate = DateTime.MinValue;
            current.Goal = 0;
            _accountRepo.Save().Wait();

            return View(nameof(Complete));
        }
    }
}
