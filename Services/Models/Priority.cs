using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Models
{
    public enum PriorityImportance 
    {
        High = 1,
        Medium = 2,
        Low = 3
    }

    public class Priority
    {
        public int PriorityId { get; set; }
        public string Name { get; set; }
        public PriorityImportance Importance { get; set; }

        public int TransactionId { get; set; }
        public Transaction Transaction { get; set; }
    }

}
