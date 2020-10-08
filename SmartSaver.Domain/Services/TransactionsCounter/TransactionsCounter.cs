using System.Collections.Generic;
using System.Linq;
using SmartSaver.EntityFrameworkCore.Models;

namespace SmartSaver.Domain.Services.TransactionsCounter
{
    class TransactionsCounter
    {
        public IEnumerable<Transaction> FilterAccount(IEnumerable<Transaction> transactions, int accountId)
        {
            // return transactions.Where(t => t.AccountId == accountId);
            return from t in transactions where t.AccountId == accountId select t;
        }

        public IEnumerable<Transaction> TotalAmountSpentByAccount(IEnumerable<Transaction> transaction) //kiek is viso isleido vartotojas
        {
            return transaction.GroupBy(t => t.AccountId).Select(x => new Transaction
            {
                AccountId = x.First().AccountId,
                Amount = x.Where(c => c.Amount < 0).Sum(c => c.Amount),
            });
        }

        public IEnumerable<Transaction> TotalAmountSavedByAccount(IEnumerable<Transaction> transaction) //kiek is viso sutaupe vartotojas
        {
            return transaction.GroupBy(t => t.AccountId).Select(x => new Transaction
            {
                AccountId = x.First().AccountId,
                Amount = x.Where(y => y.Amount > 0).Sum(y => y.Amount),
            });
        }

        public IEnumerable<Transaction> AmountSavedByCategory(IEnumerable<Transaction> transaction) // kiek vartotojas sutaupe atskiroje kategorijoje
        {
            return transaction.GroupBy(t => new { t.CategoryId, t.AccountId }).Select(x => new Transaction
            {
                Amount = x.Where(x => x.Amount > 0).Sum(y => y.Amount),
                AccountId = x.Key.AccountId,
                CategoryId = x.Key.CategoryId
            }).OrderBy(x => x.AccountId);
        }

        public IEnumerable<Transaction> AmountSpentByCategory(IEnumerable<Transaction> transaction) // kiek vartotojas isleido atskiroje kategorijoje
        {
            return transaction.GroupBy(t => new { t.CategoryId, t.AccountId }).Select(x => new Transaction
            {
                Amount = x.Where(x => x.Amount < 0).Sum(y => y.Amount),
                AccountId = x.Key.AccountId,
                CategoryId = x.Key.CategoryId
            }).OrderBy(x => x.AccountId);
        }
    }
}
