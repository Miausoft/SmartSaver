using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Services.Models
{
    public class Transaction
    {
        [Key]
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public string Category { get; set; }
        public double Amount { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
