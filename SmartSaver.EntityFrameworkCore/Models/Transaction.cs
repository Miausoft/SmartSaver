using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Text;

namespace SmartSaver.EntityFrameworkCore.Models
{
    public class Transaction
    {
        [Key]
        public int Id { get; set; }
        public DateTime ActionTime { get; set; }
        public double Amount { get; set; }

        [ForeignKey("AccountId")]
        public Account Account { get; set; }
        public int AccountId { get; set; }

        [ForeignKey("CategoryId")] 
        public Category Category { get; set; }

        public DateTime Date { get; set; }
        public int CategoryId { get; set; }
    }
}
