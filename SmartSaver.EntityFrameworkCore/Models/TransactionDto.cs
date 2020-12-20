using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartSaver.EntityFrameworkCore.Models
{
    public class TransactionDto
    {
        [Key]
        public int Id { get; set; }
        public DateTime ActionTime { get; set; }
        public decimal Amount { get; set; }

        public int AccountId { get; set; }
        public AccountDto Account { get; set; }

        public int CategoryId { get; set; }
        public CategoryDto Category { get; set; }
    }
}
