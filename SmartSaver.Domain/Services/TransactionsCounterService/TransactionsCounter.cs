using System;
using System.Collections.Generic;
using System.Linq;
using SmartSaver.Domain.Services.SavingMethodSuggestion;
using SmartSaver.EntityFrameworkCore.Models;

namespace SmartSaver.Domain.Services.TransactionsCounterService
{
    public static class TransactionsCounter
    {
        public static decimal AmountSavedCurrentMonth(IEnumerable<Transaction> transaction)
        {
            return SavedSum(transaction, new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1), DateTime.Now.AddDays(1));
        }

        public static decimal AmountSpentCurrentMonth(IEnumerable<Transaction> transaction)
        {
            return TotalExpense(transaction, new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1), DateTime.Now.AddDays(1));
        }

        public static decimal TotalExpense(IEnumerable<Transaction> transaction, DateTime from, DateTime to)
        {
            return transaction.Where(t => t.Amount < 0 && DateCounter.InRange(t.ActionTime, from, to)).Sum(c => c.Amount);
        }

        public static decimal TotalIncome(IEnumerable<Transaction> transaction, DateTime from, DateTime to)
        {
            return transaction.Where(t => t.Amount > 0 && DateCounter.InRange(t.ActionTime, from, to)).Sum(c => c.Amount);
        }

        public static decimal SavedSum(IEnumerable<Transaction> transaction, DateTime from, DateTime to)
        {
            return TotalIncome(transaction, from, to) + TotalExpense(transaction, from, to);
        }

        public static IDictionary<string, decimal> TotalIncomeByCategory(IEnumerable<Transaction> transaction, IEnumerable<Category> category, DateTime from, DateTime to)
        {
            return transaction.GroupBy(t => new { t.CategoryId, t.ActionTime }).Select(x => new Transaction
            {
                Amount = x.Where(x => x.Amount > 0).Sum(y => y.Amount),
                CategoryId = x.Key.CategoryId,
                ActionTime = x.Key.ActionTime
            }).Where(x => x.Amount > 0 && DateCounter.InRange(x.ActionTime, from, to)).GroupBy(x => x.CategoryId).Select(z => new Transaction
            {
                Amount = z.Where(z => z.Amount > 0).Select(z => z.Amount).Sum(),
                CategoryId = z.Select(z => z.CategoryId).First()
            }
            ).OrderBy(x => x.CategoryId).ToDictionary(x => category.Where(a => x.CategoryId == a.Id).Select(a => a.Title).First(), x => x.Amount);
        }

        public static IDictionary<string, decimal> TotalExpenseByCategory(IEnumerable<Transaction> transaction, IEnumerable<Category> category, DateTime from, DateTime to)
        {
            return transaction.GroupBy(t => new { t.CategoryId, t.ActionTime }).Select(x => new Transaction
            {
                Amount = x.Where(x => x.Amount < 0).Select(x => x.Amount).Sum(),
                CategoryId = x.Key.CategoryId,
                ActionTime = x.Key.ActionTime
            }).Where(x => x.Amount < 0 && DateCounter.InRange(x.ActionTime, from, to)).GroupBy(x => x.CategoryId).Select(z => new Transaction
            {
                Amount = z.Where(z => z.Amount < 0).Select(z => z.Amount).Sum(),
                CategoryId = z.Select(z => z.CategoryId).First()
            }
            ).OrderBy(z => z.CategoryId).ToDictionary(x => category.Where(a => x.CategoryId == a.Id).Select(a => a.Title).First(), x => x.Amount /* / TransactionsCounter.TotalExpense(transaction, from, to) * 100*/);
        }

        public static IDictionary<DateTime, decimal> BalanceHistory(IEnumerable<Transaction> transaction)
        {
            return transaction.GroupBy(t => new { t.ActionTime }).Select(x => new Transaction
            {
                Amount = x.Sum(y => y.Amount),
                ActionTime = DateCounter.TruncateToDayStart(x.Key.ActionTime)
            }).GroupBy(t => new { t.ActionTime }).Select(x => new Transaction
            {
                Amount = x.Sum(y => y.Amount),
                ActionTime = x.Key.ActionTime
            }).OrderBy(x => x.ActionTime).ToDictionary(x => x.ActionTime, x => x.Amount);
        }
    }
}
