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

        public IEnumerable<Transaction> GetByAccountId<T>(T accId)
        {
            return _context.Transactions
                    .Include(p => p.Category)
                    .Where(t => t.AccountId.ToString() == accId.ToString())
                    .OrderByDescending(a => a.ActionTime);
        }

        public IEnumerable<Transaction> GetByAccountForDateRange<T>(T accId, DateTime startData, DateTime endDate)
        {
            return _context.Transactions
                    .Include(p => p.Category)
                    .Where(t => t.AccountId.ToString() == accId.ToString() && t.ActionTime >= startData && t.ActionTime <= endDate)
                    .OrderByDescending(a => a.ActionTime);
        }

        public async Task<int> Create(Transaction transaction)
        {
            if (transaction.Amount <= 0)
            {
                throw new InvalidModelException("Amount cannot be less than or equal to 0");
            }

            var category = _context.Categories.FirstOrDefault(x => x.Id.Equals(transaction.CategoryId));

            if (category == null)
            {
                throw new InvalidModelException("There is no such category");
            }
            
            if (!category.TypeOfIncome)
            {
                transaction.Amount *= -1;
            }

            var created = _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return created.Entity.Id;
        }

        public Transaction GetById<T>(T transactionId)
        {
            return _context.Transactions.First(t => t.Id.ToString() == transactionId.ToString());
        }

        public Task<int> DeleteById<T>(T transactionId)
        {
            Transaction transaction = _context.Transactions.First(t => t.Id.ToString() == transactionId.ToString());
            _context.Transactions.Remove(transaction);
            return _context.SaveChangesAsync();
        }
    }
}
