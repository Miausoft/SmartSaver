using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmartSaver.EntityFrameworkCore.Models
{
    public class Account
    {
        [Key]
        public int Id { get; set; }
        public decimal Goal { get; set; }
        public DateTime GoalStartDate { get; set; }
        public DateTime GoalEndDate { get; set; }
        public double Revenue { get; set; }
        public double MonthlyExpenses { get; set; }
        public List<Transaction> Transactions { get; set; }
    }
}
