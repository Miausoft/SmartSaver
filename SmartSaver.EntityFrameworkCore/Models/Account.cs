using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmartSaver.EntityFrameworkCore.Models
{
    public class Account
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// ** Provided during registration.
        /// 
        /// Amount he wants to save.
        /// </summary>
        [RegularExpression(@"^(?:[1-9]\d*)?(?:[\.\,]\d+)?$", ErrorMessage = "Invalid imput")]
        [Display(Name = "Amount")]
        public decimal Goal { get; set; }

        /// <summary>
        /// ** Provided during registration.
        /// 
        /// Time interval in which goal has to be reached.
        /// </summary>
        public DateTime GoalStartDate { get; set; }

        [Display(Name = "Goal Day")]
        public DateTime GoalEndDate { get; set; }

        /// <summary>
        /// ** Provided during registration.
        /// !! Can be received from open banking api.
        /// 
        /// Monthly income.
        /// </summary>
        [RegularExpression(@"^(?:[1-9]\d*)?(?:[\.\,]\d+)?$", ErrorMessage = "Invalid imput")]
        [Display(Name = "Revenue")]
        public double Revenue { get; set; }

        /// <summary>
        /// ** Provided during registration.
        /// !! Can be received from open banking api.
        /// 
        /// How much of that Revenue is spent during the month.
        /// </summary>
        public double MonthlyExpenses { get; set; }

        /// <summary>
        /// List of purchases and incomes to user balance.
        /// in v1 this information is held in separate .csv file.
        /// </summary>
        public List<Transaction> Transactions { get; set; }
    }
}
