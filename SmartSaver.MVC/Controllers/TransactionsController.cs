﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartSaver.Domain.CustomAttributes;
using SmartSaver.Domain.Services.DocumentServices;
using SmartSaver.EntityFrameworkCore.Models;
using SmartSaver.MVC.Models;
using SmartSaver.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using cloudscribe.Pagination.Models;
using SmartSaver.Domain.Services.Transactions;
using System.Threading.Tasks;

namespace SmartSaver.MVC.Controllers
{
    [Authorize, RequiresAccount]
    public class TransactionsController : Controller
    {
        private readonly IRepository<Account> _accountRepo;
        private readonly IRepository<Transaction> _transactionRepo;
        private readonly IRepository<Category> _categoryRepo;
        private readonly ITransactionsService _transactionsService;

        public TransactionsController(IRepository<Account> accountRepo,
                                      IRepository<Transaction> transactionRepo,
                                      IRepository<Category> categoryRepo,
                                      ITransactionsService transactionsService)
        {
            _accountRepo = accountRepo;
            _transactionRepo = transactionRepo;
            _categoryRepo = categoryRepo;
            _transactionsService = transactionsService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string name, int pageNumber = 1, int pageSize = 10)
        {
            var account = _accountRepo
                .SearchFor(a => a.UserId.ToString().Equals(User.Identity.Name) &&
                           a.Name.Equals(name))
                .FirstOrDefault();
            
            var transactions = await _transactionRepo
                .SearchFor(t => t.UserId.ToString().Equals(User.Identity.Name) &&
                                t.AccountId == account.Id)
                .ToListAsync();

            return View(new PagedResult<TransactionViewModel>
            {
                TotalItems = transactions.Count(),
                PageNumber = pageNumber,
                PageSize = pageSize,
                Data = new List<TransactionViewModel>
                {
                    new TransactionViewModel
                    {
                        Transactions = transactions
                                       .OrderBy(t => t.ActionTime)
                                       .Skip((pageSize * pageNumber) - pageSize)
                                       .Take(pageSize)
                                       .ToList(),
                        Categories = _categoryRepo.GetAll().OrderBy(t => t.Title).ToList()
                    }
                }
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Amount,CategoryId,ActionTime")] string name, Transaction transaction)
        {   
            if (transaction.Amount <= 0 || !_categoryRepo.GetAll().Any(c => c.Id == transaction.CategoryId))
            {
                return RedirectToAction(nameof(Index));
            }

            transaction.Amount = (await _categoryRepo
                .GetByIdAsync(transaction.CategoryId))
                .TypeOfIncome ? transaction.Amount : -transaction.Amount;

            transaction.AccountId = _accountRepo
                .SearchFor(a => a.UserId.ToString().Equals(User.Identity.Name) && 
                           a.Name.Equals(name))
                .FirstOrDefault().Id;

            transaction.UserId = int.Parse(User.Identity.Name);

            await _transactionRepo.InsertAsync(transaction);
            await _transactionRepo.SaveAsync();

            return RedirectToAction(nameof(Index), new { Name = name });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, string name)
        {
            var account = _accountRepo
                .SearchFor(a => a.UserId.ToString().Equals(User.Identity.Name) &&
                                a.Name.Equals(name))
                .FirstOrDefault();

            var transaction = _transactionRepo
                .SearchFor(t => t.UserId.ToString().Equals(User.Identity.Name) &&
                                t.AccountId == account.Id &&
                                t.Id == id)
                .FirstOrDefault();

            try
            {
                _transactionRepo.Delete(transaction);
                await _transactionRepo.SaveAsync();
            }
            catch (ArgumentNullException)
            {
                // TODO: nedd to send error message to the Index view
            }
            catch (DbUpdateException)
            {
                // TODO: nedd to send error message to the Index view
            }

            return RedirectToAction(nameof(Index), new { Name = name });
        }

        [HttpGet] 
        public async Task<IActionResult> CreatePDFAsync(string name, DateTime fromDate, DateTime toDate)
        {
            if (fromDate > toDate)
            {
                return RedirectToAction(nameof(Index));
            }

            var account = _accountRepo
                .SearchFor(a => a.UserId.ToString().Equals(User.Identity.Name) &&
                                a.Name.Equals(name))
                .FirstOrDefault();

            var transactions = _transactionRepo
                .SearchFor(t => t.UserId.ToString().Equals(User.Identity.Name) &&
                                t.AccountId == account.Id)
                .ToListAsync();

            var bytes = new PDFCreator().GeneratePDF(_transactionsService.TotalExpenseByCategory(await transactions, _categoryRepo.GetAll(), fromDate, toDate), fromDate, toDate);

            return File(bytes, "application/pdf");
        }
    }
}
