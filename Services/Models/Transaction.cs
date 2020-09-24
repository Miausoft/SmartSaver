using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSaver.Models
{
    public class Transaction
    {
        public int TransactionId { get; set; }
        public DateTime DateTime { get; set; }
        public string Category { get; set; }
        public double Amount { get; set; }
    }
}
