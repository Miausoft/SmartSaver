using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Models
{
    public class Transaction
    {
        public int TransactionId { get; set; }
        public DateTime DateTime { get; set; }
        public string Category { get; set; }
        public double Amount { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }
}
