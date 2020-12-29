using System.Collections.Generic;
using SmartSaver.EntityFrameworkCore.Models;
using System;

namespace SmartSaver.MVC.Models
{
    public class DashboardViewModel
    {
        public decimal SavedCurrentMonth { get; set; }
        public decimal ToSaveCurrentMonth { get; set; }
        public IDictionary<DateTime, decimal> BalanceHistory { get; set; }
        public List<Transaction> Transactions { get; set; }
        public List<Transaction> SpendingTransactions { get; set; }
    }

    public class CategorySpending
    {
        public decimal Amount { get; set; }
        public string Title { get; set; }
    }
}