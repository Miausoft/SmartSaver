using System;
using System.Collections.Generic;
using System.Linq;
using SmartSaver.EntityFrameworkCore.Models;

namespace SmartSaver.Domain.Services.TransactionsCounter
{
    public static class TransactionsCounter
    {
        /*
         * returns list of all accounts' transactions, filtered by date
         */
        public static IEnumerable<Transaction> FilterAccount(IEnumerable<Transaction> transactions, int accountId, DateTime from, DateTime to)
        {
             return transactions.Where(t => t.AccountId == accountId && InRange(t.ActionTime, from, to));
        }

        /*
        * returns account's spent amount of money, filtered by date
        */
        public static double TotalAmountSpentByAccount(IEnumerable<Transaction> transaction, int accountId, DateTime from, DateTime to)
        {
            return transaction.Where(t => t.AccountId == accountId && t.Amount < 0 && InRange(t.ActionTime, from, to)).Sum(c => c.Amount);
        }

        /*
        * returns account's saved amount of money, filtered by date
        */
        public static double TotalAmountSavedByAccount(IEnumerable<Transaction> transaction, int accountId, DateTime from, DateTime to)
        {
            return transaction.Where(t => t.AccountId == accountId && t.Amount > 0 && InRange(t.ActionTime, from, to)).Sum(c => c.Amount);
        }

        /*
        * returns list of money account saved in every category, filtered by date
        */
        public static IEnumerable<Transaction> AmountSavedByCategory(IEnumerable<Transaction> transaction, int accountId, DateTime from, DateTime to)
        {
            return transaction.GroupBy(t => new { t.CategoryId, t.AccountId }).Select(x => new Transaction
            {
                Amount = x.Where(x => x.Amount > 0).Sum(y => y.Amount),
                CategoryId = x.Key.CategoryId
            }).Where(x => x.AccountId == accountId && x.Amount > 0 && InRange(x.ActionTime, from, to)).OrderBy(x => x.CategoryId);
        }

        /*
        * returns list of money account spent in every category, filtered by date
        */
        public static IEnumerable<Transaction> AmountSpentByCategory(IEnumerable<Transaction> transaction, int accountId, DateTime from, DateTime to)
        {
            return transaction.GroupBy(t => new { t.CategoryId, t.AccountId }).Select(x => new Transaction
            {
                Amount = x.Where(x => x.Amount < 0).Sum(y => y.Amount),
                CategoryId = x.Key.CategoryId
            }).Where(x => x.AccountId == accountId && x.Amount < 0 && InRange(x.ActionTime, from, to)).OrderBy(x => x.CategoryId);
        }

        /*
        * returns if date is in range between two dates
        */
        public static bool InRange(this DateTime dateToCheck, DateTime startDate, DateTime endDate)
        {
            return dateToCheck >= startDate && dateToCheck < endDate;
        }
    }
}
