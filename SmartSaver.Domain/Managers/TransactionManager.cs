using System;
using SmartSaver.EntityFrameworkCore;
using SmartSaver.EntityFrameworkCore.Models;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SmartSaver.Domain.Managers {
    public class TransactionManager : ITransactionManager {
        private readonly ApplicationDbContext _context;

        public TransactionManager(ApplicationDbContext context) 
        {
            _context = context;
        }

        public List<Transaction> GetAccountSpendings(int accId)
        {
            var transactions = _context.Transactions
                .Include(nameof(Category))
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
    }

    public class Balance
    {
        public decimal Amount { get; set; }
        public DateTime ActionTime { get; set; }
    }
}