using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public int Goal { get; set; }

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
