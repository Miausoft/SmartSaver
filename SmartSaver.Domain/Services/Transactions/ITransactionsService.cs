﻿using SmartSaver.EntityFrameworkCore.Models;
using System;
using System.Collections.Generic;

namespace SmartSaver.Domain.Services.Transactions
{
    public interface ITransactionsService
    {
        public decimal TotalExpense(IEnumerable<Transaction> transaction, DateTime from, DateTime to);
        public decimal TotalIncome(IEnumerable<Transaction> transaction, DateTime from, DateTime to);
        public decimal AmountSaved(IEnumerable<Transaction> transaction, DateTime from, DateTime to);
        public IDictionary<string, decimal> TotalIncomeByCategory(IEnumerable<Transaction> transaction, IEnumerable<Category> category, DateTime from, DateTime to);
        public IDictionary<string, decimal> TotalExpenseByCategory(IEnumerable<Transaction> transaction, IEnumerable<Category> category, DateTime from, DateTime to);
        public IDictionary<DateTime, decimal> BalanceHistory(IEnumerable<Transaction> transaction);
    }
}
