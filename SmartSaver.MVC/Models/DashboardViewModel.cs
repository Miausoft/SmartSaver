using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;

namespace SmartSaver.MVC.Models
{
    public class DashboardViewModel
    {
        [Required]
        [Range(double.Epsilon, double.MaxValue, ErrorMessage = "Invalid input")]
        [Display(Name = "Amount")]
        public decimal Goal { get; set; }

        [Required]
        [Display(Name = "Goal Day")]
        public DateTime GoalEndDate { get; set; }

        [Required]
        [Range(double.Epsilon, double.MaxValue, ErrorMessage = "Invalid input")]
        [Display(Name = "Revenue")]
        public double Revenue { get; set; }
        public decimal SavedCurrentMonth { get; set; }
        public decimal ToSaveCurrentMonth { get; set; }
        public decimal FreeMoneyToSpend { get; set; }
        public DateTime EstimatedTime { get; set; }
        public IDictionary<DateTime, decimal> CurrentMonthBalanceHistory { get; set; }
        public IDictionary<string, decimal> CurrentMonthTotalExpenseByCategory { get; set; }
    }
}
