using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartSaver.EntityFrameworkCore;
using SmartSaver.EntityFrameworkCore.Models;
using SmartSaver.MVC.Models;

namespace SmartSaver.MVC.Controllers
{
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
            if (User.Identity.IsAuthenticated)
            {
                var model = new TransactionViewModel()
                {
                    Transactions = _context.Transactions
                        .Include(p => p.Category) // Includes Category object.
                        .OrderByDescending(a => a.ActionTime) // Order transactions from newest to oldest.
                        .ToList(),

                    Categories = _context.Categories.ToList()
                };

                return View(model);
            }

            return RedirectToAction("Index", "Home");
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
            // Set action time to entry datetime (DateTime.Now)
            transaction.ActionTime = DateTime.UtcNow;

            // Set AccountId to user account id.
            transaction.AccountId = 1; // TODO: Changes this to real behavior

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
    }
}
