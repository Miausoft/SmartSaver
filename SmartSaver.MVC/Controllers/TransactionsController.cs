using System;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartSaver.Domain.Services.DocumentServices;
using SmartSaver.EntityFrameworkCore;
using SmartSaver.EntityFrameworkCore.Models;
using SmartSaver.MVC.Models;

namespace SmartSaver.MVC.Controllers
{
    [Authorize]
    public class TransactionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TransactionsController> _logger;

        public TransactionsController(ApplicationDbContext context, ILogger<TransactionsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: TransactionsController
        public ActionResult Index()
        {
            var model = new TransactionViewModel()
            {
                Transactions = _context.Transactions
                    .Include(p => p.Category)// Includes Category object.
                    .Where(t => t.AccountId == GetAccountAuth().Id)
                    .OrderByDescending(a => a.ActionTime) // Order transactions from newest to oldest.
                    .ToList(),

                Categories = _context.Categories.ToList()
            };

            return View(model);
        }

        // GET: TransactionsController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TransactionsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind("Amount,CategoryId")] Transaction transaction)
        {
            if (transaction.Amount == 0)
            {
                // TODO: with error message
                return RedirectToAction(nameof(Index));
            }

            transaction.Category = _context.Categories.First(c => c.Id == transaction.CategoryId);

            // Set action time to entry datetime (DateTime.Now)
            transaction.ActionTime = DateTime.UtcNow;

            // Set AccountId to user account id.
            transaction.AccountId = GetAccountAuth().Id;

            // If there is a category, means amount is spending and should be negative.
            if (!transaction.Category.TypeOfIncome)
            {
                transaction.Amount *= -1;
            }

            _context.Transactions.Add(transaction);
            _context.SaveChanges();

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

        /// <summary>
        /// Returns account object for authorized user.
        /// </summary>
        /// <returns></returns>
        private Account GetAccountAuth()
        {
            return _context.Users
                .Include(u => u.Account)
                .First(u => u.Id.ToString().Equals(User.Identity.Name))
                .Account;
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
