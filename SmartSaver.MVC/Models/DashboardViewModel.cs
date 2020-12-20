using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SmartSaver.EntityFrameworkCore.Models;
using SmartSaver.Domain.Repositories;

namespace SmartSaver.MVC.Models
{
    public class DashboardViewModel
    {
        public decimal SavedCurrentMonth { get; set; }
        public decimal ToSaveCurrentMonth { get; set; }
        public List<Balance> FirstChartData { get; set; }
        public List<TransactionDto> Transactions { get; set; }
        public List<TransactionDto> SpendingTransactions { get; set; }
    }

    public class CategorySpending
    {
        public decimal Amount { get; set; }
        public string Title { get; set; }
    }
}