using System;
using System.Collections.Generic;
using System.Linq;
using SmartSaver.Domain.Helpers;
using SmartSaver.EntityFrameworkCore.Models;

namespace SmartSaver.Domain.Services.Transactions
{
    public class TransactionsService : ITransactionsService
    {
        public decimal AmountSavedCurrentMonth(IEnumerable<Transaction> transaction)
        {
            return SavedSum(transaction, new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1), DateTime.Now.AddDays(1));
        }

        public decimal AmountSpentCurrentMonth(IEnumerable<Transaction> transaction)
        {
            return TotalExpense(transaction, new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1), DateTime.Now.AddDays(1));
        }

        public decimal TotalExpense(IEnumerable<Transaction> transaction, DateTime from, DateTime to)
        {
            return transaction.Where(t => t.Amount < 0 && DateTimeHelper.InRange(t.ActionTime, from, to)).Sum(c => c.Amount);
        }

        public decimal TotalIncome(IEnumerable<Transaction> transaction, DateTime from, DateTime to)
        {
            return transaction.Where(t => t.Amount > 0 && DateTimeHelper.InRange(t.ActionTime, from, to)).Sum(c => c.Amount);
        }

        public decimal SavedSum(IEnumerable<Transaction> transaction, DateTime from, DateTime to)
        {
            return TotalIncome(transaction, from, to) + TotalExpense(transaction, from, to);
        }

        public IDictionary<string, decimal> TotalIncomeByCategory(IEnumerable<Transaction> transaction, IEnumerable<Category> category, DateTime from, DateTime to)
        {
            return transaction.GroupBy(t => new { t.CategoryId, t.ActionTime }).Select(x => new Transaction
            {
                Amount = x.Where(x => x.Amount > 0).Sum(y => y.Amount),
                CategoryId = x.Key.CategoryId,
                ActionTime = x.Key.ActionTime
            }).Where(x => x.Amount > 0 && DateTimeHelper.InRange(x.ActionTime, from, to)).GroupBy(x => x.CategoryId).Select(z => new Transaction
            {
                Amount = z.Where(z => z.Amount > 0).Select(z => z.Amount).Sum(),
                CategoryId = z.Select(z => z.CategoryId).First()
            }
            ).OrderBy(x => x.CategoryId).ToDictionary(x => category.Where(a => x.CategoryId == a.Id).Select(a => a.Title).First(), x => x.Amount);
        }

        public IDictionary<string, decimal> TotalExpenseByCategory(IEnumerable<Transaction> transaction, IEnumerable<Category> category, DateTime from, DateTime to)
        {
            return transaction.GroupBy(t => new { t.CategoryId, t.ActionTime }).Select(x => new Transaction
            {
                Amount = x.Where(x => x.Amount < 0).Select(x => x.Amount).Sum(),
                CategoryId = x.Key.CategoryId,
                ActionTime = x.Key.ActionTime
            }).Where(x => x.Amount < 0 && DateTimeHelper.InRange(x.ActionTime, from, to)).GroupBy(x => x.CategoryId).Select(z => new Transaction
            {
                Amount = z.Where(z => z.Amount < 0).Select(z => z.Amount).Sum(),
                CategoryId = z.Select(z => z.CategoryId).First()
            }
            ).OrderBy(z => z.CategoryId).ToDictionary(x => category.Where(a => x.CategoryId == a.Id).Select(a => a.Title).First(), x => x.Amount /* / TransactionsCounter.TotalExpense(transaction, from, to) * 100*/);
        }

        public IDictionary<DateTime, decimal> BalanceHistory(IEnumerable<Transaction> transaction)
        {
            return transaction.GroupBy(t => new { t.ActionTime }).Select(x => new Transaction
            {
                Amount = x.Sum(y => y.Amount),
                ActionTime = DateTimeHelper.TruncateToDayStart(x.Key.ActionTime)
            }).GroupBy(t => new { t.ActionTime }).Select(x => new Transaction
            {
                Amount = x.Sum(y => y.Amount),
                ActionTime = x.Key.ActionTime
            }).OrderBy(x => x.ActionTime).ToDictionary(x => x.ActionTime, x => x.Amount);
        }
    }
}
