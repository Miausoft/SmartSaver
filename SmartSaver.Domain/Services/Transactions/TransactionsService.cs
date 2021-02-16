using System;
using System.Collections.Generic;
using System.Linq;
using SmartSaver.Domain.Helpers;
using SmartSaver.EntityFrameworkCore.Models;

namespace SmartSaver.Domain.Services.Transactions
{
    public class TransactionsService : ITransactionsService
    {
        public decimal TotalExpense(IEnumerable<Transaction> transaction, DateTime from, DateTime to)
        {
            return transaction.Where(t => t.Amount < 0 && DateTimeHelper.InRange(t.ActionTime, from, to)).Sum(c => c.Amount);
        }

        public decimal TotalIncome(IEnumerable<Transaction> transaction, DateTime from, DateTime to)
        {
            return transaction.Where(t => t.Amount > 0 && DateTimeHelper.InRange(t.ActionTime, from, to)).Sum(c => c.Amount);
        }

        public decimal AmountSaved(IEnumerable<Transaction> transaction, DateTime from, DateTime to)
        {
            return TotalIncome(transaction, from, to) + TotalExpense(transaction, from, to);
        }

        public IDictionary<string, decimal> TotalIncomeByCategory(IEnumerable<Transaction> transaction, IEnumerable<Category> category, DateTime from, DateTime to)
        {
            return transaction.Where(x => x.Amount > 0 && DateTimeHelper.InRange(x.ActionTime, from, to)).GroupBy(x => x.CategoryId).Select(z => new Transaction
            {
                Amount = z.Where(z => z.Amount > 0).Select(z => z.Amount).Sum(),
                CategoryId = z.Select(z => z.CategoryId).First()
            }).OrderBy(x => x.CategoryId).ToDictionary(x => category.First(a => x.CategoryId == a.Id).Title, x => x.Amount);
        }

        public IDictionary<string, decimal> TotalExpenseByCategory(IEnumerable<Transaction> transaction, IEnumerable<Category> category, DateTime from, DateTime to)
        {
            return transaction.Where(x => x.Amount < 0 && DateTimeHelper.InRange(x.ActionTime, from, to)).GroupBy(x => x.CategoryId).Select(z => new Transaction
            {
                Amount = z.Where(z => z.Amount < 0).Select(z => z.Amount).Sum(),
                CategoryId = z.Select(z => z.CategoryId).First()
            }).OrderBy(z => z.CategoryId).ToDictionary(x => category.First(a => x.CategoryId == a.Id).Title, x => x.Amount /* / TransactionsCounter.TotalExpense(transaction, from, to) * 100*/);
        }

        public IDictionary<DateTime, decimal> BalanceHistory(IEnumerable<Transaction> transaction)
        {
            var currentTotal = 0m;
            return transaction.OrderBy(t => t.ActionTime).GroupBy(t => new { t.ActionTime.Year, t.ActionTime.Month, t.ActionTime.Day }).Select(x => new Transaction
            {
                Amount = (currentTotal += x.Sum(y => y.Amount)),
                ActionTime = new DateTime(x.Key.Year, x.Key.Month, x.Key.Day)
            }).ToDictionary(x => x.ActionTime, x => x.Amount);
        }
    }
}
