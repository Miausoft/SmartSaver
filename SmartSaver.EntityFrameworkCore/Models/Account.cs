using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartSaver.EntityFrameworkCore.Models
{
    public class Account
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
        public int UserId { get; set; }
        public decimal Goal { get; set; }
        public DateTime GoalStartDate { get; set; }
        public DateTime GoalEndDate { get; set; }
        public double Revenue { get; set; }
        public double MonthlyExpenses { get; set; }

        [NotMapped]
        public List<Transaction> Transactions { get; set; }
    }
}
