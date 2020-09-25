using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Services.Models
{
    public class UserFinances
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Amount he wants to save.
        /// </summary>
        public double Goal { get; set; }
        /// <summary>
        /// List of purchases and incomes to user balance.
        /// in v1 this information is held in separate .csv file.
        /// </summary>
        public List<Transaction> Transactions { get; set; }
        /// <summary>
        /// User specified priorities. int 1 is the highest priority, n - lowest.
        /// in v1 this information is held in separate .csv file.
        /// </summary>
        public List<Priority> Priorities { get; set; }
    }
}
