using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Services.EFCore.Models
{
    public class UserFinances
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// ** Provided during registration.
        /// 
        /// Amount he wants to save.
        /// </summary>
        public double Goal { get; set; }

        /// <summary>
        /// ** Provided during registration.
        /// !! Can be received from open banking api.
        /// 
        /// Monthly income.
        /// </summary>
        public double Revenue { get; set; }                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                   

        /// <summary>
        /// ** Provided during registration.
        /// !! Can be received from open banking api.
        /// 
        /// How much of that Revenue is spent during the month.
        /// </summary>
        public double MonthlyExpenses { get; set; }

        /// <summary>
        /// ** Provided during registration.
        /// 
        /// Time interval in which goal has to be reached.
        /// </summary>
        public int TimeMonths { get; set; }

        /// <summary>
        /// ** Provided during registration.
        /// 
        /// User specified priorities. int 1 is the highest priority, n - lowest.
        /// in v1 this information is held in separate .csv file.
        /// </summary>
        public List<Category> Priorities { get; set; }

        /// <summary>
        /// List of purchases and incomes to user balance.
        /// in v1 this information is held in separate .csv file.
        /// </summary>
        public List<Transaction> Transactions { get; set; }
    }
}
