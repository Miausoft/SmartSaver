using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartSaver.EntityFrameworkCore.Models
{
    public class Transaction
    {
        [Key]
        public int Id { get; set; }
        public DateTime ActionTime { get; set; }
        public decimal Amount { get; set; }

        [ForeignKey("AccountId")]
        public Account Account { get; set; }
        public int AccountId { get; set; }

        [ForeignKey("CategoryId")] 
        public Category Category { get; set; }
        public int CategoryId { get; set; }
    }
}
