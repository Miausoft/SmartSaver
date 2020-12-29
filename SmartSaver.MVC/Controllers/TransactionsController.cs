using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartSaver.Domain.Services.DocumentServices;
using SmartSaver.EntityFrameworkCore;
using SmartSaver.EntityFrameworkCore.Models;
using SmartSaver.MVC.Models;
using SmartSaver.Domain.Repositories;
using System.Threading.Tasks;

namespace SmartSaver.MVC.Controllers
{
    [Authorize]
    public class TransactionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAccountRepoository _accountRepo;
        private readonly ITransactionRepoositry _transactionRepo;

        public TransactionsController(ApplicationDbContext context,
                                      IAccountRepoository accountRepo, 
                                      ITransactionRepoositry transactionRepo)
        {
            _context = context;
            _accountRepo = accountRepo;
            _transactionRepo = transactionRepo;
        }

        // GET: TransactionsController
        public ActionResult Index()
        {
            return View(new TransactionViewModel()
            {
                Transactions = _transactionRepo.GetByAccountId(int.Parse(User.Identity.Name)).ToList(),
                Categories = _context.Categories.ToList()
            });
        }

        // GET: TransactionsController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TransactionsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind("Amount,CategoryId")] Transaction transaction)
        {
            transaction.AccountId = _accountRepo.GetAccountById(User.Identity.Name).Id;
            await _transactionRepo.CreateTransaction(transaction);

            return RedirectToAction(nameof(Index));
        }

        // GET: TransactionsController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: TransactionsController/Delete/5
        [HttpPost]
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
        public ActionResult CreatePDF(TransactionViewModel model)
        {
            var fromDate = DateTime.Parse(model.FromDate);
            var toDate = DateTime.Parse(model.ToDate);

            if (fromDate > toDate)
            {
                return RedirectToAction(nameof(Index));
            }

            int userAccountId = _context.Users.Where(a => a.Id.ToString().Equals(User.Identity.Name)).Select(a => a.AccountId).FirstOrDefault();
            var categories = _context.Categories.ToList();
            var transactions = _context.Transactions.Where(a => a.AccountId.Equals(userAccountId)).ToList();
            byte[] bytes = new PDFCreator().GeneratePDF(transactions, categories, fromDate, toDate);

            return File(bytes, "application/pdf");
        }
    }
}
