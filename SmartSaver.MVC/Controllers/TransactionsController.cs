using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartSaver.Domain.Services.DocumentServices;
using SmartSaver.EntityFrameworkCore.Models;
using SmartSaver.MVC.Models;
using SmartSaver.Domain.Repositories;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SmartSaver.MVC.Controllers
{
    [Authorize]
    public class TransactionsController : Controller
    {
        private readonly IAccountRepoository _accountRepo;
        private readonly ITransactionRepoositry _transactionRepo;
        private readonly ICategoryRepository _categoryRepo;

        public TransactionsController(IAccountRepoository accountRepo, 
                                      ITransactionRepoositry transactionRepo,
                                      ICategoryRepository categoryRepo)
        {
            _accountRepo = accountRepo;
            _transactionRepo = transactionRepo;
            _categoryRepo = categoryRepo;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View(new TransactionViewModel()
            {
                Transactions = _transactionRepo.GetByAccountId(User.Identity.Name).ToList(),
                Categories = _categoryRepo.GetMultiple().ToList()
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind("Amount,CategoryId")] Transaction transaction)
        {
            transaction.AccountId = _accountRepo.GetById(User.Identity.Name).Id;
            await _transactionRepo.Create(transaction);

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

            List<Category> categories = _categoryRepo.GetMultiple().ToList();
            List<Transaction> transactions = _transactionRepo.GetByAccountId(User.Identity.Name).ToList();
            byte[] bytes = new PDFCreator().GeneratePDF(transactions, categories, fromDate, toDate);

            return File(bytes, "application/pdf");
        }
    }
}
