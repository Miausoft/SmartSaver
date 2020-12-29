using System.Collections.Generic;
using System;

namespace SmartSaver.MVC.Models
{
    public class DashboardViewModel
    {
        public decimal SavedCurrentMonth { get; set; }
        public decimal ToSaveCurrentMonth { get; set; }
        public IDictionary<DateTime, decimal> CurrentMonthBalanceHistory { get; set; }
        public IDictionary<string, decimal> CurrentMonthTotalExpenseByCategory { get; set; }
    }
}