using System;
using System.Collections.Generic;
using System.Text;
using SmartSaver.EntityFrameworkCore.Models;
using SmartSaver.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace SmartSaver.EntityFrameworkCore.Repositories
{
    public class TransactionRepo : ITransactionRepo
    {
        public TransactionRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        private readonly ApplicationDbContext _context;

        public List<Transaction> GetThisMonthAccountSpendings(int accId)
        {
            var transactions = _context.Transactions
                .Include(p => p.Category)
                .Where(t => t.ActionTime > CurrentMonthFirstDayDate() &&
                            t.AccountId == accId
                            && t.Category.TypeOfIncome == false).ToList();
            return transactions;
        }

        public List<Balance> GetBalanceHistory(int accId)
        {
            var chartData = new List<Balance>();

            var transactions = _context.Transactions.Include(nameof(Category))
                .Where(t => t.ActionTime > CurrentMonthFirstDayDate() && t.AccountId == accId);

            decimal balance = 0;
            foreach (var t in transactions)
            {
                balance += t.Amount;
                chartData.Add(new Balance()
                {
                    Amount = balance,
                    ActionTime = t.ActionTime
                });
            }

            return chartData;
        }

        private DateTime CurrentMonthFirstDayDate()
        {
            var date = new DateTime();
            var thisMonth = date.Month;
            var thisYear = date.Year;

            return new DateTime(thisYear, thisMonth, 1);
        }

        public List<Transaction> GetByAccountId(int accId)
        {
            return _context.Transactions
                    .Include(p => p.Category)// Includes Category object.
                    .Where(t => t.AccountId == accId)
                    .OrderByDescending(a => a.ActionTime) // Order transactions from newest to oldest.
                    .ToList();
        }

        public IEnumerable<Transaction> GetByAccountForDateRange(int accId, DateTime startData, DateTime endDate)
        {
            return _context.Transactions
                    .Include(p => p.Category)// Includes Category object.
                    .Where(t => t.AccountId == accId && t.ActionTime >= startData && t.ActionTime <= endDate)
                    .OrderByDescending(a => a.ActionTime)
                    .AsEnumerable(); // Order transactions from newest to oldest.
        }

        public async Task<CreateTransactionResponse> CreateTransactionForAccount(Transaction transaction, int accountId)
        {
            if (transaction.Amount == 0)
            {
                return CreateTransactionResponse.BadAmountError;
            }

            transaction.Category = _context.Categories.First(c => c.Id == transaction.CategoryId);
            transaction.ActionTime = DateTime.UtcNow;
            transaction.AccountId = accountId;

            if (!transaction.Category.TypeOfIncome)
            {
                transaction.Amount *= -1;
            }

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return CreateTransactionResponse.Success;
        }

        public Transaction GetById(int transactionId)
        {
            return _context.Transactions.First(t => t.Id == transactionId);
        }
    }

    public class Balance
    {
        public decimal Amount { get; set; }
        public DateTime ActionTime { get; set; }
    }
}
