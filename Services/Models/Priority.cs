using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public PriorityImportance Importance { get; set; }

    }

}
