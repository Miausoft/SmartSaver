using Microsoft.AspNetCore.Authorization;
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

namespace SmartSaver.MVC.Controllers
{
    [Authorize, RequiresAccount]
    public class TransactionsController : Controller
    {
        private readonly IRepository<Account> _accountRepo;
        private readonly IRepository<Transaction> _transactionRepo;
        private readonly IRepository<Category> _categoryRepo;

        public TransactionsController(IRepository<Account> accountRepo,
                                      IRepository<Transaction> transactionRepo,
                                      IRepository<Category> categoryRepo)
        {
            _accountRepo = accountRepo;
            _transactionRepo = transactionRepo;
            _categoryRepo = categoryRepo;
        }

        [HttpGet]
        public IActionResult Index(string name, int pageNumber = 1, int pageSize = 10)
        {
            var account = _accountRepo
                .SearchFor(a => a.UserId.ToString().Equals(User.Identity.Name) &&
                           a.Name.Equals(name))
                .FirstOrDefault();
            
            var transactions = _transactionRepo
                .SearchFor(t => t.UserId.ToString().Equals(User.Identity.Name) &&
                                t.AccountId == account.Id);

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
                                       .Skip((pageSize * pageNumber) - pageSize)
                                       .Take(pageSize)
                                       .ToList(),
                        Categories = _categoryRepo.GetAll().ToList()
                    }
                }
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind("Amount,CategoryId,ActionTime")] string name, Transaction transaction)
        {   
            if (transaction.Amount <= 0 || !_categoryRepo.GetAll().Any(c => c.Id == transaction.CategoryId))
            {
                return RedirectToAction(nameof(Index));
            }

            transaction.Amount = _categoryRepo
                .GetById(transaction.CategoryId)
                .TypeOfIncome ? transaction.Amount : -transaction.Amount;

            transaction.AccountId = _accountRepo
                .SearchFor(a => a.UserId.ToString().Equals(User.Identity.Name) && 
                           a.Name.Equals(name))
                .FirstOrDefault().Id;

            transaction.UserId = int.Parse(User.Identity.Name);

            _transactionRepo.Insert(transaction);
            _transactionRepo.Save();

            return RedirectToAction(nameof(Index), new { Name = name });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, string name)
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
                _transactionRepo.Save();
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
        public ActionResult CreatePDF(string name, DateTime fromDate, DateTime toDate)
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
                                t.AccountId == account.Id);

            var bytes = new PDFCreator()
                .GeneratePDF(transactions, _categoryRepo.GetAll(), fromDate, toDate);

            return File(bytes, "application/pdf");
        }
    }
}
