using System;
using System.Collections.Generic;
using SmartSaver.EntityFrameworkCore.Models;
using SmartSaver.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using SmartSaver.Domain.CustomExceptions;

namespace SmartSaver.Domain.Repositories
{
    public class TransactionRepository : ITransactionRepoositry
    {
        private readonly ApplicationDbContext _context;

        public TransactionRepository(ApplicationDbContext context)
        {
            _context = context;
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

        public async Task<int> CreateTransaction(Transaction transaction)
        {
            if (transaction.Amount == 0)
            {
                throw new InvalidModelException("Amount can not be 0");
            }

            transaction.Category = _context.Categories.First(c => c.Id == transaction.CategoryId);
            transaction.ActionTime = DateTime.UtcNow;

            if (!transaction.Category.TypeOfIncome)
            {
                transaction.Amount *= -1;
            }
            
            var created = _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return created.Entity.Id;
        }

        public Transaction GetById(int transactionId)
        {
            return _context.Transactions.First(t => t.Id == transactionId);
        }

        public Task<int> DeleteById(int transactionId)
        {
            Transaction transaction = _context.Transactions.First(t => t.Id == transactionId);
            _context.Transactions.Remove(transaction);
            return _context.SaveChangesAsync();
        }
    }
}
