using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartSaver.Domain.Services.DocumentServices;
using SmartSaver.EntityFrameworkCore.Models;
using SmartSaver.MVC.Models;
using SmartSaver.Domain.Repositories;
using System.Collections.Generic;
using cloudscribe.Pagination.Models;
using SmartSaver.Domain.CustomAttributes;

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

        [Route("Transactions/{pageNumber?}")]
        [HttpGet]
        public ActionResult Index(int pageNumber = 1, int pageSize = 10)
        {
            Account account = _accountRepo.SearchFor(a => a.UserId.ToString().Equals(User.Identity.Name)).FirstOrDefault();
            var transactions = _transactionRepo.SearchFor(t => t.AccountId.ToString().Equals(account.Id.ToString()));
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
        public ActionResult Create([Bind("Amount,CategoryId,ActionTime")] Transaction transaction)
        {
            if(transaction.Amount <= 0 || !_categoryRepo.GetAll().Any(c => c.Id == transaction.CategoryId))
            {
                return RedirectToAction(nameof(Index));
            }

            transaction.Amount = _categoryRepo.GetById(transaction.CategoryId).TypeOfIncome ? transaction.Amount : -transaction.Amount;
            transaction.AccountId = _accountRepo.SearchFor(a => a.UserId.ToString().Equals(User.Identity.Name)).FirstOrDefault().Id;
            _transactionRepo.Insert(transaction);
            _transactionRepo.Save();

            return RedirectToAction(nameof(Index));
        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        [HttpGet] 
        public ActionResult CreatePDF(DateTime fromDate, DateTime toDate)
        {
            if (fromDate > toDate)
            {
                return RedirectToAction(nameof(Index));
            }

            List<Transaction> transactions = _transactionRepo.SearchFor(t => t.AccountId.ToString().Equals(_accountRepo.SearchFor(a => a.UserId.ToString().Equals(User.Identity.Name)).FirstOrDefault())).ToList();
            byte[] bytes = new PDFCreator().GeneratePDF(transactions, _categoryRepo.GetAll(), fromDate, toDate);

            return File(bytes, "application/pdf");
        }
    }
}
